using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHSchool.Association
{
    class DivideOBJ
    {
        public DivideOBJ(string _StudentID)
        {
            StudentID = _StudentID;
            SCRList = new List<JHSCAttendRecord>();
        }

        /// <summary>
        /// 學生ID
        /// </summary>
        public string StudentID { get; set; }

        /// <summary>
        /// 學生修課記錄
        /// </summary>
        public JHSCAttendRecord SCR { get; set; }

        /// <summary>
        /// 重覆的修課記錄
        /// </summary>
        public List<JHSCAttendRecord> SCRList { get; set; }
    }
}
