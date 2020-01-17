using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.DocumentValidator;
using K12.Data;
using System.Data;

namespace JHSchool.Association
{
    class GradeYearStatusValidator : IFieldValidator
    {
        List<string> _GradeList { get; set; }

        public GradeYearStatusValidator()
        {
            _GradeList = GetGradeYear();
        }

        #region IFieldValidator 成員

        public string Correct(string Value)
        {
            return Value;
        }

        public string ToString(string template)
        {
            return template;
        }

        public bool Validate(string Value)
        {
            if (_GradeList.Contains(Value))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 取得學生學號 vs 系統編號
        /// </summary>
        private List<string> GetGradeYear()
        {
            List<string> list = new List<string>();
            DataTable dt = tool._Q.Select(@"select grade_year from class where grade_year is not null 
group by grade_year order by grade_year");
            foreach (DataRow row in dt.Rows)
            {
                string grade_year = "" + row[0];

                if (string.IsNullOrEmpty(grade_year))
                    continue;
                if (!list.Contains(grade_year))
                {
                    list.Add(grade_year);
                }
            }
            return list;

        }

        #endregion
    }
}
