using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Editor;
using Framework.Legacy;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using JHSchool.Evaluation;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.LogAgent;

namespace JHSchool.Association
{
    public partial class AddCourse : BaseForm
    {

        //JHTagConfigRecord是指標籤資訊(新增標籤)
        //JHCourseTagRecord是指課程與標籤關連(課程與標籤關連)

        BackgroundWorker BGW = new BackgroundWorker();
        List<JHAssessmentSetupRecord> AssessList = new List<JHAssessmentSetupRecord>();
        List<JHCourseRecord> CourseList = new List<JHCourseRecord>();

        public AddCourse()
        {
            InitializeComponent();
        }

        private void AddCourse_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            SetSchoolYearSemester();
        }

        private void SetSchoolYearSemester()
        {
            #region 學年度/學期
            int SchoolYear = int.Parse(School.DefaultSchoolYear);
            cboSchoolYear.Items.Clear();
            cboSchoolYear.Items.Add(SchoolYear - 3);
            cboSchoolYear.Items.Add(SchoolYear - 2);
            cboSchoolYear.Items.Add(SchoolYear - 1);
            int SchoolYearIndex = cboSchoolYear.Items.Add(SchoolYear);
            cboSchoolYear.Items.Add(SchoolYear + 1);
            cboSchoolYear.Items.Add(SchoolYear + 2);
            cboSchoolYear.SelectedIndex = SchoolYearIndex;

            int Semester = int.Parse(School.DefaultSemester);
            cboSemester.Items.Clear();
            cboSemester.Items.Add("1");
            cboSemester.Items.Add("2");
            cboSemester.SelectedIndex = (Semester == 1 ? 0 : 1);
            #endregion
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Text = "新增資料...";
            BGW.RunWorkerAsync();
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 背景模式
            AssessList.Clear();
            AssessList = JHAssessmentSetup.SelectAll();
            CourseList.Clear();
            CourseList = JHCourse.SelectAll(); 
            #endregion
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (txtName.Text.Trim() == "")
            {
                MsgBox.Show("社團必須輸入名稱!");
                this.Enabled = true;
                return;
            }
            bool chkHasCourseName = false;
            int SchoolYear, Semester;
            int.TryParse(cboSchoolYear.Text, out SchoolYear);
            int.TryParse(cboSemester.Text, out Semester);

            #region 評量設定檢查

            string AssID = "";
            JHAssessmentSetupRecord newAssess = new JHAssessmentSetupRecord();
            foreach (JHAssessmentSetupRecord each in AssessList)
            {
                if (each.Name == "社團評量(社團模組)")
                {
                    newAssess = each; //已經存在
                    AssID = each.ID;
                    break;
                }
            }

            if (newAssess.ID == "") //不存在就錯誤
            {
                MsgBox.Show("無社團相關評量\n請由[社團評量設定]新增[社團評量]!");
                this.Enabled = true;
                return;
            }


            #endregion

            #region 社團名稱檢查
            StringBuilder sbx1 = new StringBuilder();
            foreach (JHCourseRecord cr in CourseList)
            {
                if (!cr.SchoolYear.HasValue || !cr.Semester.HasValue) //有值才列入檢查
                {
                    sbx1.AppendLine("課程：" + cr.Name + "，學年度學期資訊有誤!!");
                    continue;
                }

                if (cr.SchoolYear.Value == SchoolYear && cr.Semester.Value == Semester && cr.Name == txtName.Text)
                {
                    chkHasCourseName = true;
                    MsgBox.Show("社團名稱重複");
                    this.Enabled = true;
                    return;
                }
            }
            if (sbx1.ToString() != "")
            {
                Exception ex = new Exception(sbx1.ToString());
                SmartSchool.ErrorReporting.ReportingService.ReportException(ex);
                //MsgBox.Show(sbx1.ToString());
            }
            #endregion

            #region 新增社團
            //新增社團
            string NewCourseID = "";
            if (chkHasCourseName == false)
            {
                JHCourseRecord InsertCourse = new JHCourseRecord();
                InsertCourse.SchoolYear = SchoolYear;
                InsertCourse.Semester = Semester;
                InsertCourse.Name = txtName.Text;
                InsertCourse.CalculationFlag = "2";
                InsertCourse.RefAssessmentSetupID = AssID;
                try
                {
                    NewCourseID = JHCourse.Insert(InsertCourse);
                }
                catch (Exception ex)
                {
                    MsgBox.Show("新增社團發生錯誤!" + ex.Message);
                    this.Enabled = true;
                    return;
                }
            }
            #endregion

            #region 社團標籤設定
   

            JHTagConfigRecord _Tag = SetTagList(); //取得社團標籤/沒有就新增

            if (_Tag == null)
            {
                this.Enabled = true;
                return;
            }

            JHCourseRecord newCourse = JHCourse.SelectByID(NewCourseID); //取回剛剛新增的課程            
            JHCourseTagRecord NewTag = new JHCourseTagRecord(); //社團標籤
            NewTag.RefEntityID = newCourse.ID;
            NewTag.RefTagID = _Tag.ID;
            try
            {
                JHCourseTag.Insert(NewTag);
            }
            catch (Exception ex)
            {
                MsgBox.Show("建立標籤發生錯誤!" + ex.Message);
                this.Enabled = true;
                return;
            }
            #endregion

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("新增社團課程：「" + newCourse.Name + "」");
            sb.AppendLine("學年度：「" + newCourse.SchoolYear.Value + "」");
            sb.AppendLine("學期：「" + newCourse.Semester.Value + "」");
            sb.AppendLine("評量樣版：「" + newCourse.AssessmentSetup.Name + "」");
            sb.AppendLine("課程標籤：「" + _Tag.Name + "」");

            ApplicationLog.Log("社團外掛模組", "新增社團課程", "course", _Tag.ID, sb.ToString());

            this.Enabled = true;
            MsgBox.Show("社團新增成功!");

            this.Close();
        }

        private JHTagConfigRecord SetTagList()
        {
            #region 新增社團類別,沒有就New一個社團類別
            List<JHTagConfigRecord> TagList = JHTagConfig.SelectByCategory(K12.Data.TagCategory.Course); //取得課程所有標籤
            JHTagConfigRecord _Tag = new JHTagConfigRecord();

            foreach (JHTagConfigRecord each in TagList)
            {
                if (each.Name == "社團" && each.Prefix == "聯課活動")
                {
                    _Tag = each;
                }
            }

            //沒有社團Tag就新增一個
            if (_Tag.ID == null)
            {
                JHTagConfigRecord newTag = new JHTagConfigRecord();
                newTag.Name = "社團";
                newTag.Prefix = "聯課活動";
                newTag.Category = "Course";
                string TagID = "";
                try
                {
                    TagID = JHTagConfig.Insert(newTag);
                }
                catch
                {
                    MsgBox.Show("新增社團標籤時,發生錯誤!");
                    return null;
                }
                _Tag = JHTagConfig.SelectByID(TagID);

                ApplicationLog.Log("社團外掛模組", "新增社團專用標籤", "新增：「社團外掛模組」專用標籤\n群組名稱：「" + _Tag.Prefix + "」" + "標籤名稱：「" + _Tag.Name + "」");
            }

            return _Tag;
            #endregion
        } 

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close ();
        }
    }
}
