using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using Aspose.Cells;
using System.IO;
using System.Diagnostics;
using JHSchool.Association.Properties;
using JHSchool.Data;
using FISCA.Presentation.Controls;

namespace JHSchool.Association
{
    public partial class StudentAsson
    {        
        public StudentAsson()
        {           
            /*出現狀態Bar的訊息*/
            FISCA.Presentation.MotherForm.SetStatusBarMessage("處理中，請稍候...", 0);

            Workbook book = new Workbook();
            book.Worksheets.Clear();
            book.Open(new MemoryStream(Resources.課程修課學生清單));
            /*新增一個List接所選取的課程*/

            List<JHCourseRecord> _CourseList = new List<JHCourseRecord>();

            _CourseList = JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource);

            Range _Range1 = book.Worksheets["Temp"].Cells.CreateRange(0, 4, false);
            Range _Range2 = book.Worksheets["Temp"].Cells.CreateRange(4, 1, false);
            Range _Range3 = book.Worksheets["Temp"].Cells.CreateRange(5, 1, false);


            int seq = 0;
            int Addpage = 0;

            /*將所選取的班級，資料取出*/
            foreach (JHCourseRecord cr in _CourseList)
            {

                book.Worksheets["Sheet1"].Cells.CreateRange(seq, 4, false).Copy(_Range1);
                //課程的標題檔
                book.Worksheets[0].Cells[seq, 0].PutValue(cr.SchoolYear + "學年度第" + cr.Semester + "學期 社團修課學生清單");

                seq++;

                //課程名稱
                book.Worksheets[0].Cells[seq, 1].PutValue(cr.Name);

                //取得修課學生
                List<JHStudentRecord> StudentList = GetSACList(cr.ID);

                StudentList.Sort(new Comparison<JHStudentRecord>(SortCompareTo.ParseStudent));

                seq++;

                //取得第一位的授課教師
                book.Worksheets[0].Cells[seq, 1].PutValue(cr.MajorTeacherName);
                //人數
                book.Worksheets[0].Cells[seq, 6].PutValue(StudentList.Count);                               
                        
                //book.Worksheets[0].AutoFitColumn(6, seq, seq);

                ////取得節次
                //book.Worksheets[0].Cells[seq, 4].PutValue(cr.Period);
                //seq++;
                ////取得科目
                //book.Worksheets[0].Cells[seq, 1].PutValue(cr.Subject);

                seq+=2;

                foreach (JHStudentRecord stdrec in StudentList)
                {
                    book.Worksheets["Sheet1"].Cells.CreateRange(seq, 1, false).Copy(_Range2);
                    //取得所屬班級 
                    if (stdrec.Class != null)
                    {
                        book.Worksheets[0].Cells[seq, 0].PutValue(stdrec.Class.Name);
                    }
                    else
                    {
                        book.Worksheets[0].Cells[seq, 0].PutValue("");
                    }
                    //取得座號
                    book.Worksheets[0].Cells[seq, 1].PutValue(stdrec.SeatNo);
                    //取得學號
                    book.Worksheets[0].Cells[seq, 2].PutValue(stdrec.StudentNumber);
                    //取得姓名
                    book.Worksheets[0].Cells[seq, 3].PutValue(stdrec.Name);
                    //取得性別
                    book.Worksheets[0].Cells[seq, 5].PutValue(stdrec.Gender);

                    seq++;
                }

                //seq++;

                Addpage++;

                book.Worksheets["Sheet1"].Cells.CreateRange(seq, 1, false).Copy(_Range3);
                book.Worksheets[0].Cells[seq, 6].PutValue("第" + Addpage + "頁/共" + _CourseList.Count + "頁");

                seq++;
                // 2017/10/26 羿均修改為新版寫法
                book.Worksheets[0].HorizontalPageBreaks.Add(seq,0);
                //book.Worksheets[0].HPageBreaks.Add(seq, 0);

                //清掉範本
                book.Worksheets.RemoveAt("Temp");
            }
            #region
            try
            {
                FISCA.Presentation.MotherForm.SetStatusBarMessage("請選擇儲存位置", 100);
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
                // 2017/10/26 羿均修改，更新新版Aspose，支援.xlsx檔案的匯入匯出。
                SaveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                SaveFileDialog1.FileName = "社團修課清單";

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    book.Save(SaveFileDialog1.FileName);
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

            FISCA.Presentation.MotherForm.SetStatusBarMessage("社團修課清單 列印完成");

            #endregion

        }

        /// <summary>
        /// 傳入課程ID,以取得修課學生清單
        /// </summary>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        private List<JHStudentRecord> GetSACList(string CourseID)
        {
            List<JHSCAttendRecord> SCAttend = JHSCAttend.SelectByCourseIDs(new List<string>() { CourseID });
            List<JHStudentRecord> StudentList = new List<JHStudentRecord>();
            JHStudent.SelectAll();

            foreach (JHSCAttendRecord each in SCAttend)
            {
                if (each.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    StudentList.Add(each.Student);
                }
            }

            return StudentList;
        }
    }
}
