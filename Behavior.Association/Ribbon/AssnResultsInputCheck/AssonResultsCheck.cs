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
using K12.Data;
using Aspose.Cells;

namespace JHSchool.Association
{
    public partial class AssonResultsCheck : BaseForm
    {

        /// <summary>
        /// 背景模式處理器
        /// </summary>
        BackgroundWorker BGW = new BackgroundWorker();

        /// <summary>
        /// 班級清單
        /// </summary>
        List<ListViewItem> ListViewRed = new List<ListViewItem>();
        List<ListViewItem> ListViewAll = new List<ListViewItem>();

        /// <summary>
        /// 資料處理器
        /// </summary>
        AssnSelect ds;

        //評量設定檔
        JHAEIncludeRecord JHAEIncludeConfig = new JHAEIncludeRecord();

        List<JHCourseRecord> Courselist = new List<JHCourseRecord>();

        string UseEffort = "努力程度";
        string UseScore = "成績";
        string UseText = "文字描述";

        //int _SchoolYear;
        //int _Semester;

        public AssonResultsCheck()
        {
            InitializeComponent();
        }

        private void AssonResultsCheck_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            AssnAdmin.Instance.TempSourceChanged += new EventHandler(Instance_TempSourceChanged);

            //intSchoolYear.Value = int.Parse(School.DefaultSchoolYear);
            //intSemester.Value = int.Parse(School.DefaultSemester);

            txtTempCount.Text = "待處理社團數: " + AssnAdmin.Instance.TempSource.Count;

            //檢查評量是否均為社團評量
            if (CheckCourseListIsAssn())
            {
                MsgBox.Show("請確認:\n1.社團評量是否已正確設定。\n2.部份社團是否並未指定為社團評量。");
                this.Close();
            }

            ColumnSetup();

            //開始背景模式
            BingForm();
        }

        /// <summary>
        /// 檢察是否有社團評量
        /// </summary>
        private bool CheckCourseListIsAssn()
        {
            #region 社團評量
            //取得社團評量
            Courselist.Clear();
            Courselist = JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource); //取得課程物件

            //取得所有社團的評量設定
            List<string> AssessmentList = new List<string>();

            foreach (JHCourseRecord each in Courselist)
            {
                //有社團是沒有評分樣版就(End/錯誤)結束
                if (string.IsNullOrEmpty(each.RefAssessmentSetupID))
                {
                    return true;
                }

                if (!AssessmentList.Contains(each.RefAssessmentSetupID))
                {
                    AssessmentList.Add(each.RefAssessmentSetupID);
                }
            }

            List<JHAssessmentSetupRecord> list = JHAssessmentSetup.SelectByIDs(AssessmentList);

            if (list.Count == 0)
            {
                return true;
            }

            JHAssessmentSetupRecord JHAssessmentConfig = new JHAssessmentSetupRecord();

            foreach (JHAssessmentSetupRecord each in list)
            {
                if (each.Name != "社團評量(社團模組)")
                {
                    return true;
                }
                else
                {
                    JHAssessmentConfig = each;
                }
            }

            if (JHAssessmentConfig.ID == null)
            {
                return true;
            }

            //由評量設定,取得評量內容
            List<JHAEIncludeRecord> AEIncList = JHAEInclude.SelectByAssessmentSetupID(JHAssessmentConfig.ID);
            //警告
            if (AEIncList.Count == 0)
            {
                return true;
            }

            //取得社團評量
            foreach (JHAEIncludeRecord each in AEIncList)
            {
                if (each.ExamName == "社團評量")
                {
                    JHAEIncludeConfig = each;
                }
            }

            if (JHAEIncludeConfig == null)
            {
                return true;
            }

            return false;

            #endregion
        }

        /// <summary>
        /// 取得設定檔,確認欄位(Column)
        /// </summary>
        private void ColumnSetup()
        {
            #region ColumnSetup

            if (JHAEIncludeConfig.UseScore)
            {
                ColumnHeader ch = listViewEx1.Columns.Add(UseScore);
                ch.Width = 100;
            }

            if (JHAEIncludeConfig.UseEffort)
            {
                ColumnHeader ch = listViewEx1.Columns.Add(UseEffort);
                ch.Width = 100;
            }

            if (JHAEIncludeConfig.UseText)
            {
                ColumnHeader ch = listViewEx1.Columns.Add(UseText);
                ch.Width = 100;
            }
            #endregion
        }

        private void BingForm()
        {
            LockForm(false);
            BGW.RunWorkerAsync();
        }

        /// <summary>
        /// 傳入bool,將特定物件關閉或開啟
        /// </summary>
        /// <param name="now"></param>
        private void LockForm(bool now)
        {
            //intSchoolYear.Enabled = now;
            //intSemester.Enabled = now;
            cbViewNotClass.Enabled = now;
            btnPrint.Enabled = now;
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            AssnSelect As = new AssnSelect(JHAEIncludeConfig);
            e.Result = As;
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region BGW_RunWorkerCompleted
            AssnSelect As = (AssnSelect)e.Result;
            LockForm(true);

            foreach (string each in As.DicAssnCode.Keys)
            {
                int ButtonString = As.DicAssnCode[each].CourseCount;

                //社團ID
                ListViewItem item = listViewEx1.Items.Add(As.DicAssnCode[each].AssnCouseID);
                //社團名稱
                item.SubItems.Add(As.DicAssnCode[each].AssnCouseName);
                //社團老師
                item.SubItems.Add(As.DicAssnCode[each].TeacherName);

                bool redBool = false;

                //成績
                if (JHAEIncludeConfig.UseScore)
                {
                    int TopString = As.DicAssnCode[each].AssnSetupList[UseScore];
                    if (ButtonString == TopString)
                    {
                        item.SubItems.Add(TopString.ToString() + "/" + ButtonString.ToString());
                    }
                    else
                    {
                        item.SubItems.Add(TopString.ToString() + "/" + ButtonString.ToString()).ForeColor = Color.Red;
                        redBool = true;
     
                    }
                }

                //努力程度
                if (JHAEIncludeConfig.UseEffort)
                {
                    int TopString = As.DicAssnCode[each].AssnSetupList[UseEffort];
                    if (ButtonString == TopString)
                    {
                        item.SubItems.Add(TopString.ToString() + "/" + ButtonString.ToString());
                    }
                    else
                    {
                        item.SubItems.Add(TopString.ToString() + "/" + ButtonString.ToString()).ForeColor = Color.Red;
                        redBool = true;
                    }
                }

                //文字描述
                if (JHAEIncludeConfig.UseText)
                {
                    int TopString = As.DicAssnCode[each].AssnSetupList[UseText];
                    if (ButtonString == TopString)
                    {
                        item.SubItems.Add(TopString.ToString() + "/" + ButtonString.ToString());
                    }
                    else
                    {
                        item.SubItems.Add(TopString.ToString() + "/" + ButtonString.ToString()).ForeColor = Color.Red;
                        redBool = true;
                    }
                }

                ListViewAll.Add(item);

                if (redBool)
                {
                    ListViewRed.Add(item);
                }
            } 
            #endregion
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BingForm();
        }

        private void cbViewNotClass_CheckedChanged(object sender, EventArgs e)
        {

            listViewEx1.Items.Clear();
            SetListViewItem();
        }

        /// <summary>
        /// 依cbViewNotClass狀態,篩選完成班級
        /// </summary>
        private void SetListViewItem()
        {
            if (cbViewNotClass.Checked)
            {
                foreach (ListViewItem each in ListViewRed)
                {
                    listViewEx1.Items.Add(each);
                }
            }
            else
            {
                foreach (ListViewItem each in ListViewAll)
                {
                    listViewEx1.Items.Add(each);
                }
            }
        }

        /// <summary>
        /// 匯出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrint_Click_1(object sender, EventArgs e)
        {
            Workbook wb = new Workbook();

            int ColumnIndex = 0;
            int ItemIndex = 0;

            //填入標題
            foreach (ColumnHeader each in listViewEx1.Columns)
            {
                if (each.Text != "社團ID")
                {
                    wb.Worksheets[0].Cells[ItemIndex, ColumnIndex].PutValue(each.Text);
                    ColumnIndex++;
                }
            }

            ItemIndex++;

            foreach (ListViewItem each in listViewEx1.Items)
            {
                ColumnIndex = 0;
                bool ClassID = false;
                foreach (ListViewItem.ListViewSubItem subitem in each.SubItems)
                {
                    if (ClassID)
                    {
                        wb.Worksheets[0].Cells[ItemIndex, ColumnIndex].PutValue(subitem.Text);
                        ColumnIndex++;

                    }
                    else
                        ClassID = true;

                }
                ItemIndex++;
            }

            SaveFileDialog sd = new System.Windows.Forms.SaveFileDialog();
            sd.Title = "另存新檔";
            sd.FileName = "社團輸入狀況檢查(匯出).xls";
            sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            if (sd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    wb.Save(sd.FileName, FileFormatType.Excel2003);
                    System.Diagnostics.Process.Start(sd.FileName);

                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    this.Enabled = true;
                    return;
                }
            }
        }


        #region 待處理

        private void ToolStripMenuItemAdd_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (ListViewItem each in listViewEx1.SelectedItems)
            {
                if (!list.Contains(each.SubItems[0].Text))
                {
                    list.Add(each.SubItems[0].Text);
                }
            }
            AssnAdmin.Instance.AddToTemp(list);
        }

        private void ToolStripMenuItemClear_Click(object sender, EventArgs e)
        {
            AssnAdmin.Instance.RemoveFromTemp(AssnAdmin.Instance.TempSource);
        }
        //更新事件
        void Instance_TempSourceChanged(object sender, EventArgs e)
        {
            txtTempCount.Text = "待處理社團數: " + AssnAdmin.Instance.TempSource.Count;
        } 

        #endregion
    }
}
