using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace JHSchool.Association.check
{
    class CheckOjb
    {
        //學生 / 學年度學期 / 社團資料

        public List<AssnCode> _AssnList { get; set; }

        public Dictionary<string, StudAssnObj> StudAssnDic { get; set; }

        /// <summary>
        /// 傳入社團記錄
        /// </summary>
        /// <param name="AssnList"></param>
        public CheckOjb(List<AssnCode> AssnList)
        {
            //取得有社團成績的學生清單
            Dictionary<string, StudentRecord> StudDic = GetStudentDic(AssnList);
            Dictionary<string, ClassRecord> ClassDic = GetClassDic(StudDic);

            StudAssnDic = new Dictionary<string, StudAssnObj>();

            foreach (AssnCode each in AssnList)
            {
                if (StudDic.ContainsKey(each.StudentID))
                {
                    if (!StudAssnDic.ContainsKey(each.StudentID))
                    {
                        StudentRecord sr = StudDic[each.StudentID];
                        ClassRecord cr = null;
                        if (!string.IsNullOrEmpty(sr.RefClassID))
                        {
                            if (ClassDic.ContainsKey(sr.RefClassID))
                            {
                                cr = ClassDic[sr.RefClassID];
                            }
                        }

                        StudAssnDic.Add(each.StudentID, new StudAssnObj(sr, cr));
                        StudAssnDic[each.StudentID].SetSchoolYearData(each);
                    }
                    else
                    {
                        StudAssnDic[each.StudentID].SetSchoolYearData(each);
                    }
                }
            }
        }

        /// <summary>
        /// 取得學生班級清單
        /// </summary>
        private Dictionary<string, ClassRecord> GetClassDic(Dictionary<string, StudentRecord> StudDic)
        {
            Dictionary<string, ClassRecord> dic = new Dictionary<string, ClassRecord>();
            List<string> classIDList = new List<string>();

            foreach (string each in StudDic.Keys)
            {
                if (!string.IsNullOrEmpty(StudDic[each].RefClassID))
                {
                    if (!classIDList.Contains(StudDic[each].RefClassID))
                    {
                        classIDList.Add(StudDic[each].RefClassID);
                    }
                }
            }

            List<ClassRecord> classList = Class.SelectByIDs(classIDList);

            foreach (ClassRecord each in classList)
            {
                if (!dic.ContainsKey(each.ID))
                {
                    dic.Add(each.ID, each);
                }
            }

            return dic;
        }

        /// <summary>
        /// 取得所有社團成績的學生清單
        /// </summary>
        private Dictionary<string, StudentRecord> GetStudentDic(List<AssnCode> AssnList)
        {
            Dictionary<string, StudentRecord> dic = new Dictionary<string, StudentRecord>();

            List<string> StudentIDList = new List<string>();
            foreach (AssnCode each in AssnList)
            {
                if (!StudentIDList.Contains(each.StudentID))
                {
                    StudentIDList.Add(each.StudentID);
                }
            }

            List<StudentRecord> StudentList = Student.SelectByIDs(StudentIDList);

            foreach (StudentRecord each in StudentList)
            {
                //取得一般生或延修生
                if (!dic.ContainsKey(each.ID))
                {
                    dic.Add(each.ID, each);
                }
            }

            return dic;
        }

    }
}
