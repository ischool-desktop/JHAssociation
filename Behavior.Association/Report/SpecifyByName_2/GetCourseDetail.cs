using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using System.Data;

namespace JHSchool.Association.SpecifyByName_2
{
    class GetCourseDetail
    {
        FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();

        public Dictionary<string, JHCourseRecord> SortDicCourse { get; set; }
        public Dictionary<string, List<super>> DicCourseStudent { get; set; }
        //建構子
        public GetCourseDetail(List<string> inCourseIDList)
        {
            string abc = string.Join(",", inCourseIDList);
            StringBuilder QuerySb = new StringBuilder();
            //取得修課(欄位:修課ID,課程ID,學生ID,班級名稱)
            QuerySb.Append("select sc_attend.id,sc_attend.ref_course_id,sc_attend.ref_student_id,class.class_name,student.seat_no,course.course_name,student.name from sc_attend ");
            //參考學生ID
            QuerySb.Append("join student on (sc_attend.ref_student_id = student.id) ");
            //參考班級名稱ID
            QuerySb.Append("join class on (student.ref_class_id = class.id) ");
            //參考課程名稱ID
            QuerySb.Append("join course on (sc_attend.ref_course_id = course.id) ");
            //狀態為'一般'之學生修課記錄
            QuerySb.Append("where student.status=1 ");
            //是社團之課程
            QuerySb.Append(string.Format("and ref_course_id in ({0})", abc));
            DataTable dt = _queryHelper.Select(QuerySb.ToString());

            #region 收集資料

            List<string> CourseIDList = new List<string>();

            Dictionary<string, List<super>> dic = new Dictionary<string, List<super>>();

            foreach (DataRow row in dt.Rows)
            {
                super su = new super();
                su.SCAttendID = "" + row["id"]; //修課記錄ID
                su.CourseID = "" + row["ref_course_id"]; //修課課程ID
                su.StudentID = "" + row["ref_student_id"]; //修課學生ID
                su.ClassName = "" + row["class_name"]; //學生班級ID
                su.SeatNo = "" + row["seat_no"]; //學生座號
                su.CourseName = "" + row["course_name"]; //課程名稱
                su.StudentName = "" + row["name"]; //學生姓名

                if (!dic.ContainsKey(su.CourseID))
                    dic.Add(su.CourseID, new List<super>());

                dic[su.CourseID].Add(su);

                if (!CourseIDList.Contains(su.CourseID))
                {
                    CourseIDList.Add(su.CourseID);
                }
            }

            //課程排序用
            List<JHCourseRecord> SortCourse = JHCourse.SelectByIDs(CourseIDList);
            SortCourse.Sort(CourseComparer);
            SortDicCourse = new Dictionary<string, JHCourseRecord>();
            foreach (JHCourseRecord each in SortCourse)
            {
                if (!SortDicCourse.ContainsKey(each.ID))
                {
                    SortDicCourse.Add(each.ID, each);
                }
            }
            #endregion
            DicCourseStudent = new Dictionary<string, List<super>>();
            foreach (string each in dic.Keys)
            {
                //學生排序用
                List<super> list = dic[each];
                //學生排序
                list.Sort(StudentComparer);
                //組合回去為資料內容
                DicCourseStudent.Add(each, list);
            }
        }

        public static int CourseComparer(JHCourseRecord x, JHCourseRecord y)
        {
            string xx = x.Name;
            string yy = y.Name;

            return xx.CompareTo(yy);
        }


        public static int StudentComparer(super x, super y)
        {
            string Xcheck;
            if (x.ClassName != "")
            {
                Xcheck = x.ClassName;
            }
            else
            {
                Xcheck = "00000";
            }
            string Ycheck;
            if (y.ClassName != "")
            {
                Ycheck = y.ClassName;
            }
            else
            {
                Ycheck = "00000";
            }

            int xx = x.SeatNo != "" ? int.Parse(x.SeatNo) : 0;
            int yy = y.SeatNo != "" ? int.Parse(y.SeatNo) : 0;

            Xcheck += xx.ToString().PadLeft(5, '0');
            Ycheck += yy.ToString().PadLeft(5, '0');


            return Xcheck.CompareTo(Ycheck);
        }
    }

    class super
    {
        /// <summary>
        /// 課程ID
        /// </summary>
        public string CourseID { get; set; }
        /// <summary>
        /// 課程名稱
        /// </summary>
        public string CourseName { get; set; }
        /// <summary>
        /// 修課記錄ID
        /// </summary>
        public string SCAttendID { get; set; }
        /// <summary>
        /// 學生ID
        /// </summary>
        public string StudentID { get; set; }
        /// <summary>
        /// 學生姓名
        /// </summary>
        public string StudentName { get; set; }
        /// <summary>
        /// 班級名稱
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 座號
        /// </summary>
        public string SeatNo { get; set; }
    }
}
