using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Association
{
    [TableName("jhschool.association.udt.clubschedule")]
    class ClubSchedule : ActiveRecord
    {
        /// <summary>
        /// 上課日期
        /// </summary>
        [Field(Field = "occur_date")]
        public DateTime OccurDate { get; set; }

        /// <summary>
        /// 星期
        /// </summary>
        [Field(Field = "week")]
        public string Week { get; set; }

        /// <summary>
        /// 節次
        /// </summary>
        [Field(Field = "period")]
        public string Period { get; set; }

        /// <summary>
        /// 年級
        /// </summary>
        [Field(Field = "grade_year")]
        public string GradeYear { get; set; }

        /// <summary>
        /// 日期學年度
        /// </summary>
        [Field(Field = "school_year", Indexed = false)]
        public string SchoolYear { get; set; }

        /// <summary>
        /// 日期學期
        /// </summary>
        [Field(Field = "semester", Indexed = false)]
        public string Semester { get; set; }
    }
}
