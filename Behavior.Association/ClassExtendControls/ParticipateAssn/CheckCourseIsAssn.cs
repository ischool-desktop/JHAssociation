using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using FISCA.Presentation.Controls;

namespace JHSchool.Association
{
    class CheckCourseIsAssn
    {
        /// <summary>
        /// 社團課程清單
        /// </summary>
        public List<string> _AssnCourseList = new List<string>();

        /// <summary>
        /// 傳入學年度學期,取得該學期的社團課程清單(_AssnCourseList)
        /// </summary>
        /// <param name="SchoolYear"></param>
        /// <param name="Semester"></param>
        public CheckCourseIsAssn(int SchoolYear, int Semester)
        {
            //取得課程標籤設定
            List<JHTagConfigRecord> TagConfigList = JHTagConfig.SelectByCategory(K12.Data.TagCategory.Course);
            List<string> AssnCourseTag = new List<string>();
            foreach (JHTagConfigRecord each in TagConfigList)
            {
                if (each.Name == "聯課活動" || each.Name == "社團")
                {
                    AssnCourseTag.Add(each.ID);
                }
            }

            List<JHCourseTagRecord> TagList = JHCourseTag.SelectAll();
            _AssnCourseList.Clear();
            JHCourse.SelectAll();
            foreach (JHCourseTagRecord each in TagList)
            {
                bool 有數值 = each.Course.SchoolYear.HasValue && each.Course.Semester.HasValue;

                if (有數值)
                {
                    bool 是社團 = AssnCourseTag.Contains(each.RefTagID);
                    bool 相等 = each.Course.SchoolYear.Value == SchoolYear && each.Course.Semester.Value == Semester;
                    bool 不重覆 = !_AssnCourseList.Contains(each.Course.ID);

                    if (是社團 && 相等 && 不重覆)
                    {
                        _AssnCourseList.Add(each.Course.ID);
                    }
                }

                //標籤是社團1
                //if (AssnCourseTag.Contains(each.RefTagID))
                //{
                //    //指定的學期
                //    if (each.Course.SchoolYear.HasValue && each.Course.Semester.HasValue)
                //    {                        
                //        if (each.Course.SchoolYear.Value == SchoolYear && each.Course.Semester.Value == Semester)
                //        {
                //            //重覆(避免爆)
                //            if (!_AssnCourseList.Contains(each.Course.ID))
                //            {
                //                _AssnCourseList.Add(each.Course.ID);
                //            }
                //        }
                //    }
                //}

                //標籤是社團2
                //if (AssnCourseTag.Contains(each.RefTagID) &&
                //    "" + each.Course.SchoolYear == "" + SchoolYear && "" + each.Course.Semester == "" + Semester &&
                //    !_AssnCourseList.Contains(each.Course.ID))
                //{
                //    _AssnCourseList.Add(each.Course.ID);
                //}
            }
        }

        /// <summary>
        /// 此課程是否為社團標籤??
        /// </summary>
        /// <param name="CourseID"></param>
        /// <returns></returns>
        public bool IsAssnCourse(string CourseID)
        {
            return _AssnCourseList.Contains(CourseID);
        }
    }
}
