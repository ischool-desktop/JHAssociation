using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;

namespace JHSchool.Association
{
    class SortCompareTo
    {
        /// <summary>
        /// 傳入學生,依班級+座號進行排序
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        static public int ParseStudent(JHStudentRecord x, JHStudentRecord y)
        {
            string Xstring = x.Class != null ? x.Class.Name : "";
            string Ystring = y.Class != null ? y.Class.Name : "";

            int Xint = x.SeatNo.HasValue ? x.SeatNo.Value : 0;
            int Yint = y.SeatNo.HasValue ? y.SeatNo.Value : 0;

            Xstring += ":" + Xint.ToString().PadLeft(2, '0');
            Ystring += ":" + Yint.ToString().PadLeft(2, '0');

            return Xstring.CompareTo(Ystring);

        }
    }
}
