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

        ///// <summary>
        ///// 目前選社 學年度
        ///// </summary>
        //[Field(Field = "SchoolYear", Indexed = false)]
        //public string SchoolYear { get; set; }

        ///// <summary>
        ///// 目前選社 學期
        ///// </summary>
        //[Field(Field = "Semester", Indexed = false)]
        //public string Semester { get; set; }

        ///// <summary>
        ///// 開始日期
        ///// </summary>
        //[Field(Field = "start_time", Indexed = true)]
        //public DateTime StartTime { get; set; }

        ///// <summary>
        ///// 結束日期
        ///// </summary>
        //[Field(Field = "end_time", Indexed = true)]
        //public DateTime EndTime { get; set; }

    }
}
