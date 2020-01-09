using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace JHSchool.Association
{
    public partial class ClubSectionDetail : BaseForm
    {
        BackgroundWorker BGW_setup = new BackgroundWorker();

        BackgroundWorker BGW_data = new BackgroundWorker();

        List<ClubSetting> cSettingList;
        List<ClubSchedule> cScheduleList;
        List<PeriodMappingInfo> cPeriodList;
        List<string> cPeriodNameList;
        List<string> GradeYearList;

        int gradeYear;
        int school_year;
        int semester;

        ClubSetting _clubSetting;

        public ClubSectionDetail()
        {
            InitializeComponent();
        }

        private void ClubSectionDetail_Load(object sender, EventArgs e)
        {
            //取得資料
            BGW_data.RunWorkerCompleted += BGW_data_RunWorkerCompleted;
            BGW_data.DoWork += BGW_data_DoWork;

            //設定資料
            BGW_setup.RunWorkerCompleted += BGW_setup_RunWorkerCompleted;
            BGW_setup.DoWork += BGW_setup_DoWork;

            formLock = false;
            BGW_setup.RunWorkerAsync();
        }

        private void BGW_setup_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得節次對照表
            cPeriodList = K12.Data.PeriodMapping.SelectAll();

            cPeriodNameList = new List<string>();
            foreach (PeriodMappingInfo each in cPeriodList)
            {
                if (!cPeriodNameList.Contains(each.Name))
                {
                    cPeriodNameList.Add(each.Name);
                }
            }

            //取得年級清單
            GradeYearList = new List<string>();
            DataTable dt = tool._Q.Select(@"select grade_year from class 
where grade_year is not null 
group by grade_year 
order by grade_year");
            foreach (DataRow row in dt.Rows)
            {
                GradeYearList.Add("" + row["grade_year"]);
            }

            gradeYear = 1;
            school_year = int.Parse(K12.Data.School.DefaultSchoolYear);
            semester = K12.Data.School.DefaultSemester == "1" ? 1 : 2;

        }

        private void BGW_setup_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    //節次
                    Column3.Items.Clear();
                    Column3.Items.AddRange(cPeriodNameList.ToArray());

                    //年級設定
                    cbGradeYear.Items.Clear();
                    cbGradeYear.Items.AddRange(GradeYearList.ToArray());

                    //學年度
                    intSchoolYear.Value = school_year;

                    //學期
                    cbSemester.SelectedIndex = semester - 1;

                    //年級
                    cbGradeYear.SelectedIndex = 0;
                }
                else
                {
                    MsgBox.Show("取得設定發生錯誤:\n" + e.Error.Message);
                }
            }
            else
            {
                MsgBox.Show("已終止作業");
            }
        }

        private void BGW_data_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得單雙周
            cSettingList = tool._A.Select<ClubSetting>(string.Format("school_year='{0}' and semester='{1}' and grade_year='{2}'", school_year.ToString(), semester.ToString(), gradeYear.ToString()));

            cScheduleList = new List<ClubSchedule>();

            //取得上課時間表
            cScheduleList = tool._A.Select<ClubSchedule>(string.Format("school_year='{0}' and semester='{1}' and grade_year='{2}'", school_year.ToString(), semester.ToString(), gradeYear.ToString()));

            //依期排序
            cScheduleList.Sort(ByDate);
        }

        private void BGW_data_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error == null)
                {
                    BindData();

                    formLock = true;
                }
                else
                {
                    MsgBox.Show("取得資料發生錯誤:\n" + e.Error.Message);
                }
            }
            else
            {
                MsgBox.Show("已終止作業");
            }
        }

        private void BindData()
        {
            //單雙周設定
            if (cSettingList.Count > 0)
            {
                _clubSetting = cSettingList[0];
                bool check = _clubSetting.IsSingleDoubleWeek;
                if (check)
                    cbDoubleClub.Checked = true;
                else
                    cbSingClub.Checked = true;
            }
            else
            {
                cbDoubleClub.Checked = false;
                cbSingClub.Checked = false;
            }

            //上課節次設定
            dataGridViewX1.Rows.Clear();
            if (cScheduleList.Count > 0)
            {
                foreach (ClubSchedule each in cScheduleList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridViewX1);

                    //日期欄位
                    row.Cells[Column1.Index].Value = each.OccurDate.ToString(tool.DateTimeFormat);

                    //星期欄位
                    row.Cells[Column2.Index].Value = tool.GetWeekName(each.OccurDate);

                    //節次欄位
                    row.Cells[Column3.Index].Value = each.Period;

                    dataGridViewX1.Rows.Add(row);
                }
            }
        }

        //儲存
        private void btnSave_Click(object sender, EventArgs e)
        {
            string message = DateCheck();

            if (message == "")
            {

                //儲存節次清單
                List<ClubSchedule> InsertList = new List<ClubSchedule>();

                //年級
                gradeYear = int.Parse(cbGradeYear.SelectedItem.ToString());
                school_year = intSchoolYear.Value;
                semester = int.Parse(cbSemester.SelectedItem.ToString());

                foreach (DataGridViewRow row in dataGridViewX1.Rows)
                {
                    if (row.IsNewRow)
                        continue;

                    ClubSchedule cs = new ClubSchedule();
                    cs.GradeYear = gradeYear.ToString();
                    cs.SchoolYear = school_year.ToString();
                    cs.Semester = semester.ToString();

                    cs.OccurDate = DateTime.Parse("" + row.Cells[Column1.Index].Value);
                    cs.Week = "" + row.Cells[Column2.Index].Value;
                    cs.Period = "" + row.Cells[Column3.Index].Value;
                    InsertList.Add(cs);
                }

                if (InsertList.Count > 0)
                {
                    //刪除上課時間表
                    cScheduleList = tool._A.Select<ClubSchedule>(string.Format("school_year='{0}' and semester='{1}' and grade_year='{2}'", school_year, semester, gradeYear));
                    tool._A.DeletedValues(cScheduleList);

                    //新增上課時間表
                    tool._A.InsertValues(InsertList);

                    //刪除單雙週
                    cSettingList = tool._A.Select<ClubSetting>(string.Format("school_year='{0}' and semester='{1}' and grade_year='{2}'", school_year, semester, gradeYear));
                    tool._A.DeletedValues(cSettingList);

                    //新增單雙週
                    ClubSetting setting = new ClubSetting();
                    setting.SchoolYear = school_year.ToString();
                    setting.Semester = semester.ToString();
                    setting.GradeYear = gradeYear.ToString();
                    setting.IsSingleDoubleWeek = cbDoubleClub.Checked;
                    setting.Save();

                    BGW_data.RunWorkerAsync();
                    MsgBox.Show("儲存成功");
                }
                else
                {
                    MsgBox.Show("未修改資料");
                }

            }
            else
            {
                MsgBox.Show("請修正錯誤：\n" + message);
            }
        }

        /// <summary>
        /// 資料檢查
        /// </summary>
        private string DateCheck()
        {
            //資料檢查
            //沒有ERROR就是正確的
            string message = "";

            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                foreach (DataGridViewCell cell in row.Cells)
                {
                    //日期
                    if (cell.ErrorText != "")
                    {
                        message += cell.ErrorText + "\n";
                    }
                }
            }

            return message;
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];
            DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            //資料替換(短日期(7/7)轉換為長日期(2019/7/7)
            if (e.ColumnIndex == Column1.Index)
            {
                if (CheckValueIsTime(cell) == "")
                {
                    if (cell.Tag != null)
                    {
                        DateTime dDateTime = (DateTime)cell.Tag;
                        dataGridViewX1.Rows[e.RowIndex].Cells[Column2.Index].Value = tool.GetWeekName(dDateTime);
                    }
                }

                //節次欄位是否有值,有值則進行資料重複判斷
                DataGridViewCell periodCell = dataGridViewX1.Rows[e.RowIndex].Cells[Column3.Index];
                if ("" + periodCell.Value != "")
                {
                    CheckDateRg(row);
                }
            }
            else if (e.ColumnIndex == Column3.Index)
            {
                //選擇節次後,確認日期是否有值
                //有值則進行判斷
                DataGridViewCell dateCell = dataGridViewX1.Rows[e.RowIndex].Cells[Column1.Index];
                if ("" + dateCell.Value != "")
                {
                    CheckDateRg(row);
                }
            }
        }

        /// <summary>
        /// 日期 + 節次
        /// </summary>
        /// <returns></returns>
        private bool CheckDateRg(DataGridViewRow checkRow)
        {

            int rowIndex = checkRow.Index;
            string name1 = "" + checkRow.Cells[Column1.Index].Value + "_" + checkRow.Cells[Column3.Index].Value;
            bool check = false;
            //以傳入的Row進行判斷
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                if (row.IsNewRow)
                    continue;

                if (row.Index != rowIndex)
                {
                    string name2 = "" + row.Cells[Column1.Index].Value + "_" + row.Cells[Column3.Index].Value;
                    if (name1 == name2)
                    {
                        check = true;
                        checkRow.Cells[Column1.Index].ErrorText = "日期+節次重複";
                    }
                    else
                    {
                        checkRow.Cells[Column1.Index].ErrorText = "";
                    }
                }
            }
            return check;
        }

        /// <summary>
        /// 檢查單一欄位,是否為日期格式
        /// </summary>
        private string CheckValueIsTime(DataGridViewCell cell)
        {
            string time = "" + cell.Value;
            time = time.Trim();
            DateTime dt;

            string message = "";
            //有填入資料
            if (!string.IsNullOrEmpty(time))
            {
                if (DateTime.TryParse(time, out dt))
                {
                    //是日期
                    cell.ErrorText = "";
                    cell.Value = dt.ToString("yyyy/MM/dd");
                    cell.Tag = dt;
                }
                else
                {
                    //z發生錯誤
                    cell.ErrorText = "輸入內容並非日期";
                    cell.Tag = null;

                    message = "輸入內容並非日期";
                }
            }
            else
            {
                //無填資料,清除錯誤
                cell.ErrorText = "";
            }

            return message;
        }

        bool formLock
        {
            set
            {
                dataGridViewX1.Enabled = value;
                cbSingClub.Enabled = value;
                cbDoubleClub.Enabled = value;
                btnSave.Enabled = value;
                cbGradeYear.Enabled = value;
            }
        }

        private int ByDate(ClubSchedule x, ClubSchedule y)
        {
            return x.OccurDate.CompareTo(y.OccurDate);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //如果切換學年度學期
        private void cbGreadYear_SelectedIndexChanged(object sender, EventArgs e)
        {
            gradeYear = int.Parse(cbGradeYear.SelectedItem.ToString());

            if (!BGW_data.IsBusy)
            {
                formLock = false;
                BGW_data.RunWorkerAsync();
            }
        }

        private void cbSemester_SelectedIndexChanged(object sender, EventArgs e)
        {
            semester = int.Parse(cbSemester.SelectedItem.ToString());

            if (!BGW_data.IsBusy)
            {
                formLock = false;
                BGW_data.RunWorkerAsync();
            }
        }

        private void intSchoolYear_ValueChanged(object sender, EventArgs e)
        {
            school_year = intSchoolYear.Value;

            if (!BGW_data.IsBusy)
            {
                formLock = false;
                BGW_data.RunWorkerAsync();
            }
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            //匯出畫面上的資料
            #region 匯出
            try
            {
                //使用學年度學期
                //取得
                DataTable dt = tool._Q.Select(string.Format(@"select clubsetting.school_year as 學年度,
clubsetting.semester as 學期,clubsetting.grade_year as 年級,
clubsetting.is_single_double_week as 單雙周,clubschedule.week as 星期,
clubschedule.period as 節次,clubschedule.occur_date as 日期
from $jhschool.association.udt.clubsetting clubsetting
join $jhschool.association.udt.clubschedule clubschedule 
on clubsetting.school_year=clubschedule.school_year and 
clubsetting.semester=clubschedule.semester 
and clubsetting.grade_year=clubschedule.grade_year
where clubsetting.school_year='{0}' and clubsetting.semester='{1}'", school_year, semester));

                DataGridViewExport export = new DataGridViewExport(dt);
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
                SaveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                SaveFileDialog1.FileName = string.Format("{0}學年度 第{1}學期 社團上課時間表", "" + school_year, "" + semester);

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
            catch (Exception ex)
            {
                MsgBox.Show("匯出發生錯誤!\n" + ex.Message);
            }
            #endregion




        }

        private void btnImport_Click(object sender, EventArgs e)
        {
            //匯入畫面上的資料




        }
    }
}