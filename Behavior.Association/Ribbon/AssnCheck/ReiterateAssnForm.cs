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
    public partial class ReiterateAssnForm : BaseForm
    {
        private BackgroundWorker BGW = new BackgroundWorker();
        private int SchoolYear;
        private int Semester;

        //重覆參與社團的學生
        private List<JHStudentRecord> ReiterateAssn = new List<JHStudentRecord>();
        //重覆社團的學生
        Dictionary<string, List<JHCourseRecord>> ReiCourseList = new Dictionary<string, List<JHCourseRecord>>();
        //刪除的列
        List<DataGridViewColumn> ColumnClearList = new List<DataGridViewColumn>();

        public ReiterateAssnForm()
        {
            InitializeComponent();
        }

        private void ReiterateAssnForm_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);
            AssnAdmin.Instance.TempSourceChanged += new EventHandler(Instance_TempSourceChanged);

            txtTemp1.Text = "待處理學生：" + K12.Presentation.NLDPanels.Student.TempSource.Count.ToString() + "人";

            txtTemp2.Text = "待處理社團：" + AssnAdmin.Instance.TempSource.Count.ToString() + "個社團";

            //設定學年度學期
            SchoolYear = intSchoolYear.Value = int.Parse(School.DefaultSchoolYear);
            Semester = intSemester.Value = int.Parse(School.DefaultSemester);
        }

        private void btnStart_Click(object sender, EventArgs e)
        {
            #region 開始查詢
            dataGridViewX1.Rows.Clear();

            foreach (DataGridViewColumn each in ColumnClearList)
            {
                if (dataGridViewX1.Columns.Contains(each))
                {
                    dataGridViewX1.Columns.Remove(each);
                }
            }

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
            #endregion
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region BGW_DoWork

            //取得當學年度學期的社團課程清單
            CheckCourseIsAssn CheckAllAssn = new CheckCourseIsAssn(SchoolYear, Semester);

            //不存在
            List<string> list = new List<string>();

            ReiCourseList.Clear();
            foreach (JHSCAttendRecord each in JHSCAttend.SelectByCourseIDs(CheckAllAssn._AssnCourseList))
            {
                if (!ReiCourseList.ContainsKey(each.RefStudentID))
                {
                    ReiCourseList.Add(each.RefStudentID, new List<JHCourseRecord>());
                }
                ReiCourseList[each.RefStudentID].Add(each.Course);
            }

            foreach (string each in ReiCourseList.Keys)
            {
                if (ReiCourseList[each].Count > 1)
                {
                    list.Add(each);
                }
            }

            ReiterateAssn.Clear();
            ReiterateAssn = JHStudent.SelectByIDs(list);
            ReiterateAssn.Sort(new Comparison<JHStudentRecord>(SortCompareTo.ParseStudent));

            #endregion
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region BGW_RunWorkerCompleted

            int counrCourse = 1;
            foreach (List<JHCourseRecord> each in ReiCourseList.Values)
            {
                if (counrCourse < each.Count)
                {
                    counrCourse = each.Count;
                }
            }

            ColumnClearList.Clear();

            for (int x = 1; x <= counrCourse; x++)
            {
                dataGridViewX1.Columns.Add("重複社團" + x, "重複社團" + x);
                ColumnClearList.Add(dataGridViewX1.Columns["重複社團" + x]);
            }

            foreach (JHStudentRecord each in ReiterateAssn)
            {
                if (each.Status == K12.Data.StudentRecord.StudentStatus.一般)
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

                    int nowCount = 5;
                    foreach (JHCourseRecord cours in ReiCourseList[each.ID])
                    {
                        row.Cells[nowCount].Value = cours.Name;
                        row.Cells[nowCount].Tag = cours.ID;
                        nowCount++;
                    }

                    dataGridViewX1.Rows.Add(row);
                }
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

        //全部社團加入待處理
        private void btnAddAssn_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow each in dataGridViewX1.Rows)
            {
                foreach (DataGridViewCell cell in each.Cells)
                {
                    if (cell.Tag is string)
                    {
                        string CourseID = (string)cell.Tag;
                        if (!list.Contains(CourseID))
                        {
                            list.Add((string)cell.Tag);
                        }
                    }
                }
            }
            AssnAdmin.Instance.AddToTemp(list);
        }

        //清空社團待處理
        private void btnClearAssn_Click(object sender, EventArgs e)
        {
            AssnAdmin.Instance.RemoveFromTemp(AssnAdmin.Instance.TempSource);
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
            txtTemp1.Text = "待處理學生：" + K12.Presentation.NLDPanels.Student.TempSource.Count.ToString() + "人";
        }

        void Instance_TempSourceChanged(object sender, EventArgs e)
        {
            txtTemp2.Text = "待處理社團：" + AssnAdmin.Instance.TempSource.Count.ToString() + "個社團";
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
                SaveFileDialog1.FileName = "重覆參與社團檢查";

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

        private void 將選擇社團加入社團待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
            {
                foreach (DataGridViewCell cell in each.Cells)
                {
                    if (cell.Tag is string)
                    {
                        string CourseID = (string)cell.Tag;
                        if (!list.Contains(CourseID))
                        {
                            list.Add((string)cell.Tag);
                        }
                    }
                }
            }
            AssnAdmin.Instance.AddToTemp(list);
        }

        private void 將選擇社團移出社團待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow each in dataGridViewX1.SelectedRows)
            {
                foreach (DataGridViewCell cell in each.Cells)
                {
                    if (cell.Tag is string)
                    {
                        string CourseID = (string)cell.Tag;
                        if (!list.Contains(CourseID))
                        {
                            list.Add((string)cell.Tag);
                        }
                    }
                }
            }
            AssnAdmin.Instance.RemoveFromTemp(list);
        }
    }
}
