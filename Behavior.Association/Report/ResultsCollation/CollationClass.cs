using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using System.ComponentModel;
using Aspose.Cells;
using System.IO;
using JHSchool.Association.Properties;
using FISCA.Presentation.Controls;
using System.Diagnostics;
using System.Windows.Forms;

namespace JHSchool.Association
{
    class CollationClass
    {
        //排序過的課程
        List<JHCourseRecord> CourseRecordList = new List<JHCourseRecord>();
        //由學生ID取得修課資料
        //Dictionary<string, JHSCAttendRecord> SCADic = new Dictionary<string, JHSCAttendRecord>();

        //由課程ID,取得班級修課學生
        Dictionary<string, List<JHStudentRecord>> CourseInStudentList = new Dictionary<string, List<JHStudentRecord>>();
        //取得學生成績內容
        Dictionary<string, AssnScoreRecord> DicAssnScore = new Dictionary<string, AssnScoreRecord>();

        BackgroundWorker BGW = new BackgroundWorker();

        JHAEIncludeRecord newAEInc = new JHAEIncludeRecord();

        public CollationClass()
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            if (ChengNewAEInc())
            {
                MsgBox.Show("未設定社團評量,請設定後再列印!");
                return;
            }

            BGW.RunWorkerAsync();

        }

        private bool ChengNewAEInc()
        {
            #region 確認社團評量
            List<JHAssessmentSetupRecord> AssSetupList = JHAssessmentSetup.SelectAll();
            string RefAssessmentSetupID = "";
            foreach (JHAssessmentSetupRecord each in AssSetupList)
            {
                if (each.Name == "社團評量(社團模組)")
                {
                    RefAssessmentSetupID = each.ID;
                    break;
                }
            }

            if (RefAssessmentSetupID != "")
            {
                List<JHAEIncludeRecord> AEIncList = JHAEInclude.SelectAll();
                foreach (JHAEIncludeRecord each in AEIncList)
                {
                    if (each.RefAssessmentSetupID == RefAssessmentSetupID && each.ExamName == "社團評量")
                    {
                        newAEInc = each;
                        break;
                    }
                }
            }

            if (newAEInc.ID == null)
            {
                return true;
            }

            return false;

            #endregion
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得課程資料&修課學生&班級資料
            GetSCAttend();

            EffortMapper Effort = new EffortMapper();

            //取得設定檔狀態

            Workbook wb = new Workbook();

            wb.Open(new MemoryStream(Resources.社團成績校對單));

            Worksheet sheet2 = wb.Worksheets["Sheet2"];

            #region Sheet2(建立範本)

            sheet2.Cells[0, 0].PutValue(School.ChineseName);//標題
            sheet2.Cells[0, 0].Style.HorizontalAlignment = TextAlignmentType.Center;

            string TitleCourseName = string.Format("社團名稱：{0}　授課教師：{1}", "　", "　");
            sheet2.Cells[1, 0].PutValue(TitleCourseName);//標題
            sheet2.Cells[1, 0].Style.HorizontalAlignment = TextAlignmentType.Center;

            sheet2.Cells[2, 0].PutValue("班級");
            sheet2.Cells[2, 1].PutValue("座號");
            sheet2.Cells[2, 2].PutValue("學號");
            sheet2.Cells[2, 3].PutValue("姓名");

            int CountSetup = 0;
            int CountSetup1 = 0;
            int CountSetup2 = 0;
            int CountSetup3 = 0;

            if (newAEInc.UseScore)
            {
                CountSetup1 = CountSetup + 4;
                sheet2.Cells[2, CountSetup1].PutValue("成績");
                sheet2.Cells[2, CountSetup1].Style = sheet2.Cells[2, 0].Style;
                sheet2.Cells[3, CountSetup1].Style = sheet2.Cells[3, 0].Style;
                CountSetup++;
            }

            if (newAEInc.UseEffort)
            {
                CountSetup2 = CountSetup + 4;
                sheet2.Cells[2, CountSetup2].PutValue("努力程度");
                sheet2.Cells[2, CountSetup2].Style = sheet2.Cells[2, 0].Style;
                sheet2.Cells[3, CountSetup2].Style = sheet2.Cells[3, 0].Style;
                CountSetup++;
            }
            if (newAEInc.UseText)
            {
                CountSetup3 = CountSetup + 4;
                sheet2.Cells[2, CountSetup3].PutValue("文字評量");
                sheet2.Cells[2, CountSetup3].Style = sheet2.Cells[2, 0].Style;
                sheet2.Cells[3, CountSetup3].Style = sheet2.Cells[3, 0].Style;
                CountSetup++;
            }

            //合併儲存格
            sheet2.Cells.Merge(0, 0, 1, CountSetup + 4);
            sheet2.Cells.Merge(1, 0, 1, CountSetup + 4); 
            #endregion

            #region Range
            //表頭
            Range Range1 = sheet2.Cells.CreateRange(0, 3, false);
            //學生單位
            Range Range2 = sheet2.Cells.CreateRange(3, 1, false);
            //列印日期
            Range Range3 = sheet2.Cells.CreateRange(4, 1, false);
            #endregion

            Worksheet sheet1 = wb.Worksheets["Sheet1"];

            #region Sheet1

            int RowIndex = 0;

            foreach (JHCourseRecord cr in CourseRecordList)
            {
                List<JHStudentRecord> StudList = new List<JHStudentRecord>();
                if (CourseInStudentList.ContainsKey(cr.ID))
                {
                    StudList = CourseInStudentList[cr.ID];
                }
                if (StudList.Count == 0)
                {
                    continue;
                }

                sheet1.Cells.CreateRange(RowIndex, 3, false).Copy(Range1);

                string CourseTeacherName = "";
                if (cr.MajorTeacherNickname != "")
                {
                    CourseTeacherName = string.Format("社團名稱：{0}　授課教師：{1}", cr.Name, cr.MajorTeacherName + "(" + cr.MajorTeacherNickname + ")");
                }
                else
                {
                    CourseTeacherName = string.Format("社團名稱：{0}　授課教師：{1}", cr.Name, cr.MajorTeacherName);
                }

                RowIndex++; //標題

                sheet1.Cells[RowIndex, 0].PutValue(CourseTeacherName);//標題

                RowIndex += 2; //課程資訊

                StudList.Sort(new Comparison<JHStudentRecord>(SortCompareTo.ParseStudent));

                foreach (JHStudentRecord CrStud in StudList)
                {
                    #region 填入修課學生
                    sheet1.Cells.CreateRange(RowIndex, 1, false).Copy(Range2);

                    if (CrStud.Class != null)
                    {
                        sheet1.Cells[RowIndex, 0].PutValue(CrStud.Class.Name); //班級名稱
                    }
                    if (CrStud.SeatNo.HasValue)
                    {
                        sheet1.Cells[RowIndex, 1].PutValue(CrStud.SeatNo.Value); //座號
                    }
                    sheet1.Cells[RowIndex, 2].PutValue(CrStud.StudentNumber); //學號
                    sheet1.Cells[RowIndex, 3].PutValue(CrStud.Name); //姓名

                    //取得學生修課成績
                    if (DicAssnScore.ContainsKey(cr.ID + CrStud.ID))
                    {
                        //JHSCAttendRecord CrStudScar = SCADic[CrStud.ID];

                        AssnScoreRecord ASR = DicAssnScore[cr.ID + CrStud.ID];

                        if (newAEInc.UseScore && ASR.Score != null)
                        {
                            sheet1.Cells[RowIndex, CountSetup1].PutValue(ASR.Score.Value); //成績
                        }
                        if (newAEInc.UseEffort && ASR.Effort != null)
                        {
                            StringBuilder sb = new StringBuilder();
                            sb.Append(ASR.Effort.HasValue ? ASR.Effort.Value.ToString() : "");
                            sb.Append("(" + Effort.GetTextByCode(ASR.Effort.Value) + ")");
                            sheet1.Cells[RowIndex, CountSetup2].PutValue(sb.ToString()); //努力程度
                        }
                        if (newAEInc.UseText && ASR.Text != "")
                        {
                            sheet1.Cells[RowIndex, CountSetup3].PutValue(ASR.Text); //文字描述
                        }
                    }
                    RowIndex++;
                    #endregion
                }


                sheet1.Cells[RowIndex, sheet1.Cells.MaxColumn].PutValue("列印日期:" + DateTime.Now.ToShortDateString());
                //列印分隔線
                RowIndex++;
                sheet1.HPageBreaks.Add(RowIndex, 0);
                sheet1.AutoFitColumns();
                sheet1.AutoFitRows();
            } 
            #endregion

            wb.Worksheets.RemoveAt("Sheet2");
            e.Result = wb;
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
                SaveFileDialog1.FileName = "社團成績校對單";

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    wb.Save(SaveFileDialog1.FileName);
                    Process.Start(SaveFileDialog1.FileName);
                }
                else
                {
                    MsgBox.Show("檔案未儲存");
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("");
                    return;

                }
            }
            catch
            {
                MsgBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
            }

            FISCA.Presentation.MotherForm.SetStatusBarMessage("社團成績校對單 列印完成");
            #endregion
        }


        //取得學生清單&修課資訊
        private void GetSCAttend()
        {
            //取得社團課程
            List<string> CourseIDList = AssnAdmin.Instance.SelectedSource;
            List<string> StudentKeyList = new List<string>();

            CourseRecordList.Clear();
            CourseRecordList = JHCourse.SelectByIDs(CourseIDList);
            CourseRecordList.Sort(new Comparison<JHCourseRecord>(SortCourse));

            //取得學生修課資料
            List<JHSCAttendRecord> SCAList = JHSCAttend.SelectByCourseIDs(CourseIDList);
            CourseInStudentList.Clear();
            JHStudent.SelectAll();

            foreach (JHSCAttendRecord each in SCAList)
            {
                if (each.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    //整理所有學生ID
                    if (!StudentKeyList.Contains(each.RefStudentID))
                    {
                        StudentKeyList.Add(each.RefStudentID);
                    }

                    //整理Course,Student
                    if (!CourseInStudentList.ContainsKey(each.RefCourseID))
                    {
                        CourseInStudentList.Add(each.RefCourseID, new List<JHStudentRecord>());
                    }
                    CourseInStudentList[each.RefCourseID].Add(each.Student);
                }
            }

            //學生修課成績(需要課程ID & 學生ID)
            DicAssnScore.Clear();
            List<AssnScoreRecord> SCETakeList = JHSCETake.SelectByStudentAndCourse(StudentKeyList, CourseInStudentList.Keys).AsKHJHSCETakeRecords();

            foreach (AssnScoreRecord each in SCETakeList)
            {
                if (!DicAssnScore.ContainsKey(each.RefCourseID + each.RefStudentID))
                {
                    DicAssnScore.Add(each.RefCourseID + each.RefStudentID, each);
                }
            }

        }

        private int SortCourse(JHCourseRecord x, JHCourseRecord y)
        {
            return x.Name.CompareTo(y.Name);

        }
    }
}
