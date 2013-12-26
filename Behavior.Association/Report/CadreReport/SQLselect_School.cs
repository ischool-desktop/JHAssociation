using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using FISCA.UDT;

namespace JHSchool.Association.CadreReport
{
    class SQLselect_School
    {
        public Dictionary<string, List<StudentCadre_School>> _StudentObjDic = new Dictionary<string, List<StudentCadre_School>>();

        private List<string> StudentIDList = new List<string>();

        private Dictionary<string, string> CadreSortDic = new Dictionary<string, string>();

        public Dictionary<string, string> TeacherDic = new Dictionary<string, string>();

        public List<SchoolObject> CadreList = new List<SchoolObject>();

        /// <summary>
        /// 取得全校學生資料
        /// </summary>
        public SQLselect_School(slsTConfig_School config)
        {
            #region 取得學生擔任幹部資料
            List<string> list_studentID = new List<string>();
            AccessHelper _accessHelper = new AccessHelper();
            StringBuilder sbx = new StringBuilder();
            //條件式 - 學年度/學期/學生/幹部類型
            sbx.Append(string.Format("SchoolYear in ('{0}') and Semester in('{1}') ", config.SchoolYear.ToString(), config.Semester.ToString()));
            sbx.Append("and ReferenceType in ('社團幹部') ");
            CadreList = _accessHelper.Select<SchoolObject>(sbx.ToString());
            foreach (SchoolObject each in CadreList)
            {
                if (!list_studentID.Contains(each.StudentID))
                {
                    list_studentID.Add(each.StudentID);
                }
            }
            string studentString = string.Join("','", list_studentID);
            #endregion

            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();

            List<string> list = new List<string>();
            foreach (K12.Data.CourseRecord each in config.Societies)
            {
                list.Add(each.ID);
            }
            string courseString = string.Join("','", list);

            #region 取得社團學生

            StringBuilder sb = new StringBuilder();
            sb.Append("select student.id as student_id,student.name,student.seat_no,student.student_number,class.id as class_id,class.class_name,class.display_order,course.course_name,course.id as course_id ");
            sb.Append("from sc_attend join student on sc_attend.ref_student_id=student.id ");
            sb.Append("join course on sc_attend.ref_course_id=course.id ");
            sb.Append("join class on student.ref_class_id=class.id ");
            sb.Append(string.Format("where sc_attend.ref_course_id in('{0}') ", courseString));
            sb.Append("and student.status=1 ");
            sb.Append(string.Format("and student.id in ('{0}')", studentString));
            //sb.Append("order by class.grade_year,class.display_order,class.class_name,student.seat_no,student.name");
            DataTable dt = _queryHelper.Select(sb.ToString());

            foreach (DataRow row in dt.Rows)
            {
                StudentCadre_School sc = new StudentCadre_School(row);

                if (!_StudentObjDic.ContainsKey(sc.course_name))
                {
                    _StudentObjDic.Add(sc.course_name, new List<StudentCadre_School>());
                }
                _StudentObjDic[sc.course_name].Add(sc);
            }
            #endregion

            #region 取得幹部設定檔,取得排序依據

            List<ClassCadreNameObj> ClassCadreNameList = _accessHelper.Select<ClassCadreNameObj>();

            foreach (ClassCadreNameObj each in ClassCadreNameList)
            {
                if (!CadreSortDic.ContainsKey(each.NameType + each.CadreName))
                {
                    CadreSortDic.Add(each.NameType + each.CadreName, each.NameType + each.Index.ToString().PadLeft(4, '0'));
                }
            }

            #endregion

            #region 依據社團,取得社團指導老師

            StringBuilder sbu = new StringBuilder();
            sbu.Append("select course.id as course_id,course.course_name,teacher.teacher_name,teacher.nickname from course ");
            sbu.Append("join tc_instruct on course.id=tc_instruct.ref_course_id ");
            sbu.Append("join teacher on teacher.id=tc_instruct.ref_teacher_id ");
            sbu.Append(string.Format("where course.id in('{0}') ", courseString));
            DataTable dtu = _queryHelper.Select(sbu.ToString());

            foreach (DataRow row in dtu.Rows)
            {
                string courseID = "" + row["course_id"];
                string course_name = "" + row["course_name"];
                string teacher_name = "" + row["teacher_name"];
                string nickname = "" + row["nickname"];

                if (!TeacherDic.ContainsKey(course_name))
                {
                    if (!string.IsNullOrEmpty(nickname))
                    {
                        TeacherDic.Add(course_name, teacher_name + "(" + nickname + ")");
                    }
                    else
                    {
                        TeacherDic.Add(course_name, teacher_name);
                    }
                }
            }



            #endregion

            foreach (SchoolObject each in CadreList)
            {
                bool IsTrue = false;

                foreach (string each1 in _StudentObjDic.Keys)
                {
                    if (IsTrue)
                        break;

                    foreach (StudentCadre_School each2 in _StudentObjDic[each1])
                    {
                        if (each2.Student_ID == each.StudentID)
                        {
                            each2.list.Add(each);
                            IsTrue = true;
                            break;
                        }
                    }
                }
            }

            //取得課程
            foreach (string each in _StudentObjDic.Keys)
            {
                //排序班及座號
                _StudentObjDic[each].Sort(SortCadre1);

                foreach (StudentCadre_School each1 in _StudentObjDic[each])
                {
                    each1.list.Sort(SortCadre2);
                }
            }
            //排序
            //CadreList.Sort(SortCadre);
        }

        private int SortCadre1(StudentCadre_School a1, StudentCadre_School b1)
        {
            string aa2 = a1.Class_display_order.PadLeft(4, '0');
            aa2 += a1.Class_Name.PadLeft(4, '0');
            aa2 += a1.Student_SeatNo.PadLeft(4, '0');
            string aa1 = "矓矓矓矓" + 9999;

            aa1 += aa2;

            string bb2 = b1.Class_display_order.PadLeft(4, '0');
            bb2 += b1.Class_Name.PadLeft(4, '0');
            bb2 += b1.Student_SeatNo.PadLeft(4, '0');
            string bb1 = "矓矓矓矓" + 9999;

            bb1 += bb2;
            return aa1.CompareTo(bb1);
        }

        private int SortCadre2(SchoolObject a1, SchoolObject b1)
        {
            string aa1 = "";
            if (CadreSortDic.ContainsKey(a1.ReferenceType + a1.CadreName))
            {
                aa1 = CadreSortDic[a1.ReferenceType + a1.CadreName];
            }

            string bb1 = "";
            if (CadreSortDic.ContainsKey(b1.ReferenceType + b1.CadreName))
            {
                bb1 = CadreSortDic[b1.ReferenceType + b1.CadreName];
            }

            return aa1.CompareTo(bb1);
        }
    }

    class StudentCadre_School
    {
        /// <summary>
        /// 班級系統編號
        /// </summary>
        public string Class_ID { get; set; }

        /// <summary>
        /// 班級名稱
        /// </summary>
        public string Class_Name { get; set; }

        /// <summary>
        /// 班級排序
        /// </summary>
        public string Class_display_order { get; set; }

        /// <summary>
        /// 學生系統編號
        /// </summary>
        public string Student_ID { get; set; }

        /// <summary>
        /// 姓名
        /// </summary>
        public string Student_Name { get; set; }

        /// <summary>
        /// 座號
        /// </summary>
        public string Student_SeatNo { get; set; }

        /// <summary>
        /// 學號
        /// </summary>
        public string Student_Number { get; set; }

        /// <summary>
        /// 課程ID
        /// </summary>
        public string course_id { get; set; }

        /// <summary>
        /// 課程
        /// </summary>
        public string course_name { get; set; }

        public List<SchoolObject> list = new List<SchoolObject>();

        public StudentCadre_School(DataRow row)
        {
            Class_ID = "" + row["class_id"];
            Class_Name = "" + row["class_name"];
            Class_display_order = "" + row["display_order"];

            Student_ID = "" + row["student_id"];
            Student_Name = "" + row["name"];
            Student_SeatNo = "" + row["seat_no"];
            Student_Number = "" + row["student_number"];

            course_id = "" + row["course_id"];
            course_name = "" + row["course_name"];

        }
    }
}
