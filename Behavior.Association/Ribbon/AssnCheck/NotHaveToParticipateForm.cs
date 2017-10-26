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
using System.Diagnostics;

namespace JHSchool.Association
{
    public partial class NotHaveToParticipateForm : BaseForm
    {
        private BackgroundWorker BGW = new BackgroundWorker();
        private int SchoolYear;
        private int Semester;

        //無參與社團的學生
        private List<JHStudentRecord> NotHaveToParticipateList = new List<JHStudentRecord>();

        public NotHaveToParticipateForm()
        {
            InitializeComponent();
        }

        private void NotHaveToParticipateForm_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);

            txtTemp.Text = "待處理學生：" + K12.Presentation.NLDPanels.Student.TempSource.Count.ToString() + "人";

            //設定學年度學期
            SchoolYear = intSchoolYear.Value = int.Parse(School.DefaultSchoolYear);
            Semester = intSemester.Value = int.Parse(School.DefaultSemester);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            dataGridViewX1.Rows.Clear();

            SchoolYear = intSchoolYear.Value;
            Semester = intSemester.Value;

            btnStart.Enabled = false;

            if (!BGW.IsBusy)
            {
                BGW.RunWorkerAsync();
            }
            else
            {
                btnStart.Enabled = true;
                MsgBox.Show("忙碌中,請再試一次");
            }
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region BGW_DoWork

            //取得全校狀態非畢業&刪除的學生
            List<string> AllStudList = new List<string>();

            foreach (JHStudentRecord each in JHStudent.SelectAll())
            {
                if (each.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    AllStudList.Add(each.ID);
                }
            }

            //取得當學年度學期的社團課程清單
            CheckCourseIsAssn CheckAllAssn = new CheckCourseIsAssn(SchoolYear, Semester);

            if (CheckAllAssn._AssnCourseList.Count == 0)
            {
                MsgBox.Show("本學年度/學期,無社團課程");
                return;
            }

            //取得修課資料
            List<string> StudentSCAList = new List<string>();


            //存在不存在關鍵
            foreach (JHSCAttendRecord each in JHSCAttend.SelectByCourseIDs(CheckAllAssn._AssnCourseList))
            {
                if (!StudentSCAList.Contains(each.RefStudentID))
                {
                    StudentSCAList.Add(each.RefStudentID);
                }
            }

            List<string> list = new List<string>();

            //判斷全部學生,沒有存在於社團課程清單中
            foreach (string each in AllStudList)
            {
                if (!StudentSCAList.Contains(each))
                {
                    list.Add(each);
                }
            }

            NotHaveToParticipateList.Clear();
            NotHaveToParticipateList = JHStudent.SelectByIDs(list);

            NotHaveToParticipateList.Sort(new Comparison<JHStudentRecord>(SortCompareTo.ParseStudent));

            #endregion
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region BGW_RunWorkerCompleted
            JHStudent.SelectAll();
            foreach (JHStudentRecord each in NotHaveToParticipateList)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1); //取得目前樣板
                row.Tag = each.ID;
                if (each.Class != null)
                {
                    row.Cells[0].Value = each.Class.Name;
                }
                if (each.SeatNo.HasValue)
                {
                    row.Cells[1].Value = each.SeatNo;
                }
                row.Cells[2].Value = each.StudentNumber;
                row.Cells[3].Value = each.Name;
                row.Cells[4].Value = each.Gender;

                dataGridViewX1.Rows.Add(row);
            }

            txtHelpCountStudent.Text = "清單人數：" + dataGridViewX1.Rows.Count.ToString() + "人";
            btnStart.Enabled = true;
            #endregion
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #region 待處理

        //全部加入待處理
        private void btnAllInsert_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow each in dataGridViewX1.Rows)
            {
                if (each.Tag != null)
                {
                    string StudentID = (string)each.Tag;
                    list.Add(StudentID);
                }
            }
            K12.Presentation.NLDPanels.Student.AddToTemp(list);
        }

        //清空待處理
        private void btnRemoveTemp_Click(object sender, EventArgs e)
        {
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource);
        }

        //選擇加入待處理
        private void 加入待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
            {
                if (each.Tag != null)
                {
                    string StudentID = (string)each.Tag;
                    list.Add(StudentID);
                }
            }
            K12.Presentation.NLDPanels.Student.AddToTemp(list);
        }

        //移出待處理
        private void 移出待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
            {
                if (each.Tag != null)
                {
                    string StudentID = (string)each.Tag;
                    list.Add(StudentID);
                }
            }
            K12.Presentation.NLDPanels.Student.RemoveFromTemp(list);
        }

        //待處理改變時
        void Student_TempSourceChanged(object sender, EventArgs e)
        {
            txtTemp.Text = "待處理學生：" + K12.Presentation.NLDPanels.Student.TempSource.Count.ToString() + "人";
        }
        #endregion

        private void btnExport_Click(object sender, EventArgs e)
        {
            #region 匯出
            try
            {
                DataGridViewExport export = new DataGridViewExport(dataGridViewX1);
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
                // 2017/10/26 羿均修改，更新新版Aspose，支援.xlsx檔案的匯入匯出。
                SaveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                SaveFileDialog1.FileName = "未參與社團學生清單";

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    export.Save(SaveFileDialog1.FileName);
                    Process.Start(SaveFileDialog1.FileName);
                }
                else
                {
                    MsgBox.Show("檔案未儲存");
                }
            }
            catch
            {
                MsgBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
            }
            #endregion

        }
    }
}
