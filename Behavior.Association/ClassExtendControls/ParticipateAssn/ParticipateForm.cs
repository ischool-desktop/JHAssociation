using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using Aspose.Cells;
using JHSchool.Data;
using System.IO;
using JHSchool.Association.Properties;
using System.Diagnostics;

namespace JHSchool.Association
{

    //要增加(上課地點)
    public partial class ParticipateForm : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();
        ParticipateBOT _BOT;
        int _SchoolYear;
        int _Semester;

        public ParticipateForm()
        {
            InitializeComponent();
        }

        private void ParticipateForm_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            IntSchoolYear.Value = int.Parse(School.DefaultSchoolYear);
            IntSemester.Value = int.Parse(School.DefaultSemester);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            #region 儲存

            if (IntSchoolYear.Text == "" || IntSemester.Text == "")
            {
                MsgBox.Show("學年度/學期 不可空白");
                return;
            }

            _SchoolYear = IntSchoolYear.Value;
            _Semester = IntSemester.Value;

            this.Enabled = false;

            if (!BGW.IsBusy)
            {
                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("系統忙碌中,請稍後再試!!");
            }

            #endregion
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region BGW_DoWork
            Workbook wb = new Workbook();
            wb.Open(new MemoryStream(Resources.班級社團分組名單));
            //表頭
            Range Range1 = wb.Worksheets["Sheet2"].Cells.CreateRange(0, 3, false);
            //學生單位
            Range Range2 = wb.Worksheets["Sheet2"].Cells.CreateRange(3, 1, false);
            //列印日期
            Range Range3 = wb.Worksheets["Sheet2"].Cells.CreateRange(4, 1, false);

            string SheetIndex = "Sheet1";

            //****建立選擇班級的社團清單*****
            _BOT = new ParticipateBOT(_SchoolYear, _Semester, cbAssnAddress.Checked);

            //取得班級ID並排序
            List<JHClassRecord> ClassList = JHClass.SelectByIDs(_BOT.ClassStudentList.Keys);
            ClassList.Sort(new Comparison<JHClassRecord>(SortClass));

            int RowIndex = 0;

            foreach (JHClassRecord each in ClassList)
            {
                string TitleName = string.Format("{0}學年度 第{1}學期　{2}　班級社團分組名單", _SchoolYear, _Semester, each.Name);

                //建立表頭
                wb.Worksheets[SheetIndex].Cells.CreateRange(RowIndex, 3, false).Copy(Range1);

                //表頭處理
                wb.Worksheets[SheetIndex].Cells[RowIndex, 0].PutValue(School.ChineseName);
                RowIndex++;
                wb.Worksheets[SheetIndex].Cells[RowIndex, 0].PutValue(TitleName);
                RowIndex += 2;

                //取得學生
                List<JHStudentRecord> StudentList = _BOT.ClassStudentList[each.ID];
                StudentList.Sort(new Comparison<JHStudentRecord>(SortStudent));

                foreach (JHStudentRecord stud in StudentList)
                {
                    //一名學生
                    wb.Worksheets[SheetIndex].Cells.CreateRange(RowIndex, 1, false).Copy(Range2);

                    if (stud.SeatNo.HasValue)
                    {
                        wb.Worksheets[SheetIndex].Cells[RowIndex, 0].PutValue(stud.SeatNo.Value);
                    }

                    wb.Worksheets[SheetIndex].Cells[RowIndex, 1].PutValue(stud.StudentNumber);
                    wb.Worksheets[SheetIndex].Cells[RowIndex, 2].PutValue(stud.Name);
                    if (_BOT.StudentInfo.ContainsKey(stud.ID))
                    {
                        wb.Worksheets[SheetIndex].Cells[RowIndex, 3].PutValue(_BOT.StudentInfo[stud.ID]);
                    }
                    RowIndex++;
                }

                wb.Worksheets[SheetIndex].Cells.CreateRange(RowIndex, 1, false).Copy(Range3);
                wb.Worksheets[SheetIndex].Cells[RowIndex, 3].PutValue("列印日期:" + DateTime.Now.ToShortDateString());
                RowIndex++;

                wb.Worksheets[SheetIndex].HPageBreaks.Add(RowIndex, 0);
            }

            //wb.Worksheets[SheetIndex].AutoFitColumns();

            wb.Worksheets.RemoveAt("Sheet2");
            e.Result = wb; 
            #endregion
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region BGW_RunWorkerCompleted
            Workbook wb = (Workbook)e.Result;

            try
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("請選擇儲存位置", 100);
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

                SaveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                SaveFileDialog1.FileName = "班級社團分組名單";

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    wb.Save(SaveFileDialog1.FileName);
                    Process.Start(SaveFileDialog1.FileName);
                }
                else
                {
                    this.Enabled = true;
                    MsgBox.Show("檔案未儲存");
                    FISCA.Presentation.MotherForm.SetStatusBarMessage(""); 
                    return;

                }
            }
            catch
            {
                this.Enabled = true;
                MsgBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
            }
            this.Enabled = true;
            FISCA.Presentation.MotherForm.SetStatusBarMessage("班級社團分組名單 列印完成"); 
            #endregion
        }

        //排序班級
        private int SortClass(JHClassRecord x, JHClassRecord y)
        {
            return x.Name.CompareTo(y.Name);
        }

        //排序座號
        private int SortStudent(JHStudentRecord x, JHStudentRecord y)
        {
            int xx = x.SeatNo.HasValue ? x.SeatNo.Value : 0;
            int yy = y.SeatNo.HasValue ? y.SeatNo.Value : 0;
            return xx.CompareTo(yy);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}
