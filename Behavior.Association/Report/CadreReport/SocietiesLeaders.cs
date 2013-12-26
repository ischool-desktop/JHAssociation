using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using FISCA.Presentation.Controls;
using System.Data;
using Aspose.Cells;
using System.IO;
using System.Windows.Forms;

namespace JHSchool.Association.CadreReport
{
    class SocietiesLeaders
    {
        BackgroundWorker BGW = new BackgroundWorker();


        slsTConfig_School _config { get; set; }

        List<K12.Data.CourseRecord> CourseList { get; set; }

        public SocietiesLeaders()
        {
            if (AssnAdmin.Instance.SelectedSource.Count < 1)
            {
                MsgBox.Show("請選擇社團!");
                return;
            }

            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            if (!BGW.IsBusy)
            {
                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("系統忙碌中,請稍後再試.");
            }
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得社團課程
            CourseList = K12.Data.Course.SelectByIDs(AssnAdmin.Instance.SelectedSource);

            //課程資料學年度學期是否正確
            if (!CheckSchoolYearSemester(CourseList))
            {
                e.Cancel = true;
                return;
            }

            _config = new slsTConfig_School();
            _config.SchoolYear = CourseList[0].SchoolYear.Value; //學年度
            _config.Semester = CourseList[0].Semester.Value; //學期
            _config.Societies = CourseList; //課程

            SQLselect_School sql = new SQLselect_School(_config);

            Workbook template = new Workbook();
            template.Worksheets.Clear();
            template.Open(new MemoryStream(Properties.Resources.社團幹部總表_範本), FileFormatType.Excel2003);

            Worksheet ptws = template.Worksheets[0];
            //建立Range
            Range ptHeader = ptws.Cells.CreateRange(0, 2, false);
            Range ptEachRow = ptws.Cells.CreateRange(2, 1, false);

            Workbook wb = new Workbook();
            wb.Copy(template);
            Worksheet ws = wb.Worksheets[0];

            int studentCount = 0;
            int cutPageIndex = 49;

            foreach (string class_Name in sql._StudentObjDic.Keys) //社團ID
            {
                int cutStudentIndex = 0;
                int count = 0;
                foreach (StudentCadre_School cadre in sql._StudentObjDic[class_Name]) //每一個學生幹部資料
                {
                    count += cadre.list.Count;
                }
                if (count == 0) continue;

                //int SetOutline = 1;
                //複製 Header
                ws.Cells.CreateRange(studentCount, 2, false).Copy(ptHeader);

                string SchoolNameAndTitle = "社團幹部總表";
                ws.Cells[studentCount, 0].PutValue("社團：" + class_Name); //班級名稱
                ws.Cells[studentCount, 3].PutValue(SchoolNameAndTitle); //學校名稱 與 報表名稱
                if (sql.TeacherDic.ContainsKey(class_Name))
                    ws.Cells[studentCount, 8].PutValue("指導老師：" + sql.TeacherDic[class_Name]); //班導師
                else
                    ws.Cells[studentCount, 8].PutValue("指導老師："); //班導師

                //把班級標頭拷貝下來
                Range ClassHeader = ws.Cells.CreateRange(studentCount, 2, false);

                studentCount += 2;
                foreach (StudentCadre_School cadre in sql._StudentObjDic[class_Name]) //每一個學生幹部資料
                {
                    foreach (SchoolObject obj in cadre.list)
                    {
                        cutStudentIndex++;
                        if (cutStudentIndex == cutPageIndex)
                        {
                            cutStudentIndex = 1;
                            ws.HPageBreaks.Add(studentCount, 7);
                            ws.Cells.CreateRange(studentCount, 2, false).Copy(ClassHeader);
                            studentCount += 2;
                        }


                        ws.Cells.CreateRange(studentCount, 1, false).Copy(ptEachRow);
                        //if (SetOutline % 5 == 0 && SetOutline != 0)
                        //{
                        //    ws.Cells.CreateRange(studentCount, 0, 1, 8).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);
                        //}
                        ws.Cells[studentCount, 0].PutValue(cadre.Class_Name); //班級
                        ws.Cells[studentCount, 1].PutValue(cadre.Student_SeatNo); //座號
                        ws.Cells[studentCount, 2].PutValue(cadre.Student_Name); //姓名
                        ws.Cells[studentCount, 3].PutValue(cadre.Student_Number); //學號

                        ws.Cells[studentCount, 4].PutValue(obj.SchoolYear); //學年度
                        ws.Cells[studentCount, 5].PutValue(obj.Semester); //學期
                        ws.Cells[studentCount, 6].PutValue(obj.ReferenceType); //幹部類型
                        ws.Cells[studentCount, 7].PutValue(obj.CadreName); //幹部名稱
                        ws.Cells[studentCount, 8].PutValue(obj.Text); //說明
                        //SetOutline++;
                        studentCount++;
                    }
                }
                //ws.Cells.CreateRange(studentCount - 1, 0, 1, 8).SetOutlineBorder(BorderType.BottomBorder, CellBorderType.Medium, Color.Black);
                ws.HPageBreaks.Add(studentCount, 7);
            }

            string path = Path.Combine(Application.StartupPath, "Reports");
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            path = Path.Combine(path, "社團幹部總表" + ".xlt");
            e.Result = new object[] { "社團幹部總表", path, wb };



        }

        /// <summary>
        /// 檢查課程是否都有學年度/學期
        /// True = 資料正確
        /// False = 資料錯誤
        /// </summary>
        /// <returns></returns>
        bool CheckSchoolYearSemester(List<K12.Data.CourseRecord> list)
        {
            foreach (K12.Data.CourseRecord each in list)
            {
                if (!each.SchoolYear.HasValue || !each.Semester.HasValue)
                {
                    return false;
                }
            }

            return true;
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                //資料狀態有誤,已取消操作
                MsgBox.Show("有部份課程,沒有學年度學期,無法取得資料!!");
                return;
            }

            string BarMessage = "";
            if (e.Error == null)
            {
                string reportName;
                string path;
                Workbook wb;

                object[] result = (object[])e.Result;
                reportName = (string)result[0];
                path = (string)result[1];
                wb = (Workbook)result[2];

                if (File.Exists(path))
                {
                    int i = 1;
                    while (true)
                    {
                        string newPath = Path.GetDirectoryName(path) + "\\" + Path.GetFileNameWithoutExtension(path) + (i++) + Path.GetExtension(path);
                        if (!File.Exists(newPath))
                        {
                            path = newPath;
                            break;
                        }
                    }
                }

                try
                {
                    wb.Save(path, FileFormatType.Excel2003);
                    FISCA.Presentation.MotherForm.SetStatusBarMessage(reportName + "產生完成");
                    System.Diagnostics.Process.Start(path);
                }
                catch
                {
                    SaveFileDialog sd = new SaveFileDialog();
                    sd.Title = "另存新檔";
                    sd.FileName = reportName + ".xls";
                    sd.Filter = "Excel檔案 (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                    if (sd.ShowDialog() == DialogResult.OK)
                    {
                        try
                        {
                            wb.Save(sd.FileName, FileFormatType.Excel2003);
                        }
                        catch
                        {
                            MsgBox.Show("指定路徑無法存取。", "建立檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }
                }

                BarMessage = "社團幹部總表 列印成功!!";
                FISCA.Presentation.MotherForm.SetStatusBarMessage(BarMessage);
            }
            else
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
                BarMessage = "社團幹部總表 列印失敗!!";
                FISCA.Presentation.MotherForm.SetStatusBarMessage(BarMessage + e.Error.Message);
            }

            FISCA.Presentation.MotherForm.SetStatusBarMessage(BarMessage);
        }
    }

    class slsTConfig_School
    {
        /// <summary>
        /// 學年度
        /// </summary>
        public int SchoolYear { get; set; }

        /// <summary>
        /// 學期
        /// </summary>
        public int Semester { get; set; }

        /// <summary>
        /// 社團課程ID
        /// </summary>
        public List<K12.Data.CourseRecord> Societies { get; set; }
    }
}
