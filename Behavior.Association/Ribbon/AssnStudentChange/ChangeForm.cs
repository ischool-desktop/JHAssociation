using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using Framework;
using FISCA.LogAgent;

namespace JHSchool.Association
{
    public partial class ChangeForm : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();
        SCAItem SCA;

        string SchoolYear;
        string Semester;
        JHCourseRecord course;
        Dictionary<string, JHCourseRecord> AllAssnDic = new Dictionary<string, JHCourseRecord>();

        public ChangeForm()
        {
            InitializeComponent();
        }

        private void AssnStudentChange_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            BGW.RunWorkerAsync();
        }

        /// <summary>
        /// 背景模式
        /// </summary>
        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得課程
            course = JHCourse.SelectByID(AssnAdmin.Instance.SelectedSource[0]);
            SchoolYear = course.SchoolYear.HasValue ? course.SchoolYear.Value.ToString() : "";
            Semester = course.Semester.HasValue ? course.Semester.Value.ToString() : "";

            //取得修課學生
            SCA = new SCAItem(AssnAdmin.Instance.SelectedSource[0]);

            //取得選擇的課程,該學年度學期的社團課程
            List<JHCourseTagRecord> JHCourseTaglist = JHCourseTag.SelectAll();
            List<JHCourseRecord> AllAssn1 = new List<JHCourseRecord>();
            AllAssnDic.Clear();

            List<string> list = new List<string>();

            JHCourse.SelectAll();

            foreach (JHCourseTagRecord each in JHCourseTaglist)
            {
                if (each.FullName.Contains("聯課活動") || each.FullName.Contains("社團"))
                {
                    AllAssn1.Add(each.Course);
                }
            }

            foreach (JHCourseRecord each in AllAssn1)
            {
                if (each.ID == course.ID) //排除自己
                    continue;

                if (each.SchoolYear.HasValue && each.Semester.HasValue)
                {
                    if (each.SchoolYear.Value.ToString() == SchoolYear && each.Semester.Value.ToString() == Semester)
                    {
                        AllAssnDic.Add(each.Name, each);
                        list.Add(each.Name);
                    }
                }
            }

            e.Result = list;
        }

        /// <summary>
        /// 背景模式完成
        /// </summary>
        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            List<string> list = (List<string>)e.Result;

            //標頭
            StringBuilder sb = new StringBuilder();
            sb.Append("社團名稱：" + course.Name);
            sb.Append("　學年度：" + (course.SchoolYear.HasValue ? course.SchoolYear.Value.ToString() : ""));
            sb.Append("　學期：" + (course.Semester.HasValue ? course.Semester.Value.ToString() : ""));
            lbAssnCourseName.Text = sb.ToString();

            //將ComBox加入List
            list.Sort();
            Column5.Items.AddRange(list.ToArray());


            foreach (JHStudentRecord each in SCA.Students)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);
                row.Cells[0].Value = each.Class != null ? each.Class.Name : ""; //班級名稱
                row.Cells[1].Value = each.SeatNo.HasValue ? each.SeatNo.Value.ToString() : ""; //座號
                row.Cells[2].Value = each.StudentNumber; //課程名稱
                row.Cells[3].Value = each.Name; //學生姓名
                row.Tag = each; //記住學生

                dataGridViewX1.Rows.Add(row);
            }
        }

        /// <summary>
        /// 儲存
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            List<JHSCAttendRecord> InsertSCAttendList = new List<JHSCAttendRecord>();
            List<string> StudentID = new List<string>();
            List<string> DelCourseID = new List<string>();
            List<string> InsertCourseID = new List<string>();
            DelCourseID.Add(course.ID);

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("變更學生參與社團明細：");

            //用迴圈判斷更新哪些學生的資料
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Cells[4].Value != null) //非null就是已變更選擇
                {
                    //1.取得學生                                        
                    JHStudentRecord student = (JHStudentRecord)row.Tag;
                    //取得目標課程
                    JHCourseRecord newCourse = AllAssnDic["" + row.Cells[4].Value];
                    //加入新修課
                    JHSCAttendRecord newSCAttendRecord = new JHSCAttendRecord();
                    newSCAttendRecord.RefCourseID = newCourse.ID;
                    InsertCourseID.Add(newCourse.ID); //新增的課程ID
                    newSCAttendRecord.RefStudentID = student.ID;
                    StudentID.Add(student.ID); //學生ID

                    InsertSCAttendList.Add(newSCAttendRecord);

                    sb.Append("學生「" + student.Name + "」參與社團，");
                    sb.Append("由「" + course.Name + "」社團");
                    sb.AppendLine("變更為「" + newCourse.Name + "」社團");
                }
            }

            //新增前先檢查是否已修課
            List<JHSCAttendRecord> SetList = JHSCAttend.SelectByStudentIDAndCourseID(StudentID, InsertCourseID);
            if (SetList.Count != 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("變更社團發生錯誤!!\n變更清單中,有學生重覆參與社團!");
                return;
            }

            try
            {
                JHSCAttend.Insert(InsertSCAttendList); //新增
            }
            catch
            {
                FISCA.Presentation.Controls.MsgBox.Show("新增社團記錄時,發生錯誤!!");
                return;
            }


            //取得原修課記錄
            List<JHSCAttendRecord> DelList = JHSCAttend.SelectByStudentIDAndCourseID(StudentID, DelCourseID);

            try
            {
                JHSCAttend.Delete(DelList); //移除
            }
            catch
            {
                FISCA.Presentation.Controls.MsgBox.Show("移除原社團參與記錄時,發生錯誤!!");
                return;
            }

            ApplicationLog.Log("社團外掛模組", "變更社團參與學生", sb.ToString());

            FISCA.Presentation.Controls.MsgBox.Show("社員變更參與社團,變更成功!");
            this.Close();
        }

        /// <summary>
        /// 離開
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewX1_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            dataGridViewX1.BeginEdit(true);
        }
    }
}
