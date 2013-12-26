using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using K12.Data;

namespace JHSchool.Association.check
{
    class StudAssnObj
    {
        public StudentRecord _Stud { get; set; }

        public ClassRecord _class { get; set; }

        public Dictionary<string, List<AssnCode>> _AssnDic { get; set; }

        public string SchoolYear { get; set; }
        public string Semester { get; set; }

        public StudAssnObj(StudentRecord Stud,ClassRecord cr)
        {
            _Stud = Stud;
            _class = cr;
            _AssnDic = new Dictionary<string, List<AssnCode>>();
        }

        public void SetSchoolYearData(AssnCode code)
        {
            string school = string.Format("學年度:{0}" + "學期:{1}", code.SchoolYear, code.Semester);
            SchoolYear = code.SchoolYear;
            Semester = code.Semester;
            if (!_AssnDic.ContainsKey(school))
            {
                _AssnDic.Add(school, new List<AssnCode>());
            }
            _AssnDic[school].Add(code);
        }
    }
}
