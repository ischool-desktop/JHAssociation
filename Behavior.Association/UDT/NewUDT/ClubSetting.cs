using FISCA.UDT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Association
{
    /// <summary>
    /// 社團相關設定值
    /// </summary>
    [TableName("jhschool.association.udt.clubsetting")]
    class ClubSetting : ActiveRecord
    {
        /// <summary>
        /// 單雙周設定
        /// True =  雙周
        /// False = 每周
        /// </summary>
        [Field(Field = "is_single_double_week")]
        public bool IsSingleDoubleWeek { get; set; }

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
