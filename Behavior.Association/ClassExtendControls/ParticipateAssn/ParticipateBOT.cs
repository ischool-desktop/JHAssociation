using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using FISCA.Presentation;
using K12.Presentation;
using FISCA.UDT;

namespace JHSchool.Association
{
    class ParticipateBOT
    {
        List<string> _ClassList;

        /// <summary>
        /// 取得學生修課字串
        /// </summary>
        public Dictionary<string, string> StudentInfo = new Dictionary<string, string>();
        /// <summary>
        /// 取得班級學生清單
        /// </summary>
        public Dictionary<string, List<JHStudentRecord>> ClassStudentList = new Dictionary<string, List<JHStudentRecord>>();

        CheckCourseIsAssn CourseIsAssn;

        //UDT功能物件
        private AccessHelper _accessHelper = new AccessHelper();

        private List<AssnAddress> AddressList = new List<AssnAddress>();

        private Dictionary<string, AssnAddress> AddressDic = new Dictionary<string, AssnAddress>();

        public ParticipateBOT(int SchoolYear,int Semester,bool AssnAddress)
        {
            //清單物件
            CourseIsAssn = new CheckCourseIsAssn(SchoolYear, Semester);

            //取得班級物件
            _ClassList = NLDPanels.Class.SelectedSource;

            //AddressList = _accessHelper.Select<AssnAddress>(string.Format("SchoolYear='{0}' and Semester='{1}'", SchoolYear.ToString(), Semester.ToString()));
            AddressList = _accessHelper.Select<AssnAddress>();
            foreach (AssnAddress each in AddressList)
            {
                if (!AddressDic.ContainsKey(each.AssociationID))
                {
                    AddressDic.Add(each.AssociationID, each);
                }
            }


            //取得學生清單
            List<JHStudentRecord> StudentList = JHStudent.SelectByClassIDs(_ClassList);
            List<string> StudentKey = new List<string>();
            foreach (JHStudentRecord each in StudentList)
            {
                if (each.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    StudentKey.Add(each.ID);
                    //建立清單
                    if (!ClassStudentList.ContainsKey(each.Class.ID))
                    {
                        ClassStudentList.Add(each.Class.ID, new List<JHStudentRecord>());
                    }
                    ClassStudentList[each.Class.ID].Add(each);
                }
            }

            //取得學生修課列表
            List<JHSCAttendRecord> SCAList = JHSCAttend.SelectByStudentIDAndCourseID(StudentKey, CourseIsAssn._AssnCourseList);
            List<string> SCACourseInAssn = new List<string>();

            JHCourse.SelectAll();
            foreach (JHSCAttendRecord each in SCAList)
            {
                if (CourseIsAssn.IsAssnCourse(each.Course.ID))
                {
                    if (!StudentInfo.ContainsKey(each.RefStudentID))
                    {
                        StudentInfo.Add(each.RefStudentID, "");
                    }
                }

                if (StudentInfo.ContainsKey(each.RefStudentID))
                {
                    if (StudentInfo[each.RefStudentID] == "")
                    {
                        if (AddressDic.ContainsKey(each.Course.ID) && AssnAddress)
                        {
                            StudentInfo[each.RefStudentID] += each.Course.Name + "(" + AddressDic[each.Course.ID].Address + ")";
                        }
                        else
                        {
                            StudentInfo[each.RefStudentID] += each.Course.Name;
                        }
                    }
                    else
                    {
                        if (AddressDic.ContainsKey(each.Course.ID) && AssnAddress)
                        {
                            StudentInfo[each.RefStudentID] += "," + each.Course.Name + "(" + AddressDic[each.Course.ID].Address + ")";
                        }
                        else
                        {
                            StudentInfo[each.RefStudentID] += "," + each.Course.Name;
                        }
                    }
                }
            }        
        }
    }
}
