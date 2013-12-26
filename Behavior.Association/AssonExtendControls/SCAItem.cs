using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using System.Data;

namespace JHSchool.Association
{
    class SCAItem
    {
        //private List<JHStudentRecord> _AllStudent; //全校學生
        //private List<string> _SetAllStudentIDs; //全校學生ID
        //private List<JHSCAttendRecord> _SCAList = new List<JHSCAttendRecord>(); //修本課程之學生
        private List<JHStudentRecord> _Students = new List<JHStudentRecord>();
        private Dictionary<string, JHSCAttendRecord> _SCADic = new Dictionary<string, JHSCAttendRecord>();
        private string _PKey;

        /// <summary>
        /// 傳入課程ID,取得修課資料清單
        /// </summary>
        /// <param name="PKey"></param>
        public SCAItem(string PKey)
        {
            _PKey = PKey;
            Reset();
        }

        ///// <summary>
        ///// 取得傳入的課程,學生修課資料
        ///// </summary>
        //public List<JHSCAttendRecord> SCAList
        //{
        //    get { return _SCAList; }
        //}

        /// <summary>
        /// 取得該課程修課學生Record
        /// </summary>
        public List<JHStudentRecord> Students
        {
            get { return _Students; }
        }

        /// <summary>
        /// 學生ID : 學生修課資料
        /// </summary>
        public Dictionary<string, JHSCAttendRecord> SCADic
        {
            get { return _SCADic; }
        }

        /// <summary>
        /// 學生ID : 學生資料
        /// </summary>
        //public Dictionary<string, JHStudentRecord> DicStudent
        //{
        //    get { return _DicStudent; }
        //}


        /// <summary>
        /// 更新學生修課資料
        /// </summary>
        public void Reset()
        {
            _Students.Clear();
            _SCADic.Clear();

            #region 新方法

            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id from sc_attend ");
            sb.Append("join student on sc_attend.ref_student_id=student.id ");
            sb.Append(string.Format("where ref_course_id={0} ", _PKey));
            sb.Append("and student.status=1");

            DataTable dt = _queryHelper.Select(sb.ToString());
            List<string> StudentIDList = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string studentID = "" + row["id"];
                if (!StudentIDList.Contains(studentID))
                {
                    StudentIDList.Add(studentID);
                }
            }

            //取得學生
            _Students = JHStudent.SelectByIDs(StudentIDList);
            _Students.Sort(new Comparison<JHStudentRecord>(SortCompareTo.ParseStudent)); 

            List<string> CourseIDList = new List<string>();
            CourseIDList.Add(_PKey);
            foreach (JHSCAttendRecord each in JHSCAttend.SelectByCourseIDs(CourseIDList))
            {
                //需包含於QueryHelper取回之學生清單
                if (StudentIDList.Contains(each.RefStudentID))
                {
                    if (!_SCADic.ContainsKey(each.RefStudentID))
                    {
                        _SCADic.Add(each.RefStudentID, each);
                    }
                }
            }

            #endregion

            #region MyRegion

            //List<string> CourseIDList = new List<string>();
            //CourseIDList.Add(_PKey);
            ////取得修課清單
            //_SCAList.Clear();
            //_SCAList = JHSCAttend.SelectByCourseIDs(CourseIDList);

            //JHStudent.SelectAll();

            //_Students.Clear();
            //_SCADic.Clear();

            //foreach (JHSCAttendRecord each in _SCAList)
            //{
            //    JHStudentRecord sr = each.Student;
            //    if (sr.Status == K12.Data.StudentRecord.StudentStatus.一般)
            //    {
            //        if (!_SCADic.ContainsKey(each.RefStudentID))
            //        {
            //            _Students.Add(each.Student);
            //            _SCADic.Add(each.RefStudentID, each);
            //        }
            //    }
            //}

            //_Students.Sort(new Comparison<JHStudentRecord>(SortCompareTo.ParseStudent));
            #endregion
        }
    }
}
