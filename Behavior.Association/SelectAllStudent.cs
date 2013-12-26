using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHSchool.Association
{
    class SelectAllStudent
    {
        List<JHStudentRecord> Studentlist = new List<JHStudentRecord>();

        string _PKey;

        public SelectAllStudent(string PKey)
        {
            _PKey = PKey;

            SelectStudentMode();
        }

        /// <summary>
        /// 處理學生清單
        /// </summary>
        private void SelectStudentMode()
        {
            Studentlist.Clear();
            SCAItem SCA = new SCAItem(_PKey);

            foreach (JHStudentRecord each in SCA.Students)
            {
                if (each.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    Studentlist.Add(each);
                }

            }
        }

        /// <summary>
        /// 傳入班級名稱與座號取得學生物件
        /// </summary>
        /// <param name="ClassName">班級名稱</param>
        /// <param name="SeatNo">座號</param>
        /// <returns>學生物件</returns>
        public JHStudentRecord GetStudentBySeatNo(string ClassName, string SeatNo)
        {
            if (string.IsNullOrEmpty(ClassName) || string.IsNullOrEmpty(SeatNo))
                return null;

            foreach (JHStudentRecord each in Studentlist)
            {
                if (each.Class == null)
                    continue;

                if (ClassName == each.Class.Name)
                {
                    if (each.SeatNo.HasValue)
                    {
                        if (SeatNo == each.SeatNo.Value.ToString())
                        {
                            return each;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 傳入學號取得學生物件
        /// </summary>
        /// <param name="StudentNumber">學號</param>
        /// <returns>學生物件</returns>
        public JHStudentRecord GetStudentByStudentNumber(string StudentNumber)
        {
            if (string.IsNullOrEmpty(StudentNumber))
                return null;

            foreach (JHStudentRecord each in Studentlist)
            {
                if (each.StudentNumber == StudentNumber)
                {
                    return each;
                }
            }

            return null;
        }

        /// <summary>
        /// 傳入班級名稱,是否存在
        /// </summary>
        /// <param name="ClassName">班級名稱</param>
        /// <returns>是/否</returns>
        public bool ClassNameIsNotNull(string ClassName)
        {
            if (string.IsNullOrEmpty(ClassName))
                return false;

            foreach (JHStudentRecord each in Studentlist)
            {
                if (each.Class == null)
                    continue;

                if (each.Class.Name == ClassName) //有班級名稱相符
                {
                    return true;
                }
            }

            return false;

        }
    }
}
