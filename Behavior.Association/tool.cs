using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Association
{
    static class tool
    {
        static public FISCA.UDT.AccessHelper _A = new FISCA.UDT.AccessHelper();
        static public FISCA.Data.QueryHelper _Q = new FISCA.Data.QueryHelper();

        public static string DateTimeFormat = "yyyy/MM/dd";

        public static string GetWeekName(DateTime dDateTime)
        {
            string name = "";
            switch (dDateTime.DayOfWeek.ToString())
            {
                case "Monday": name = "星期一"; break;
                case "Tuesday": name = "星期二"; break;
                case "Wednesday": name = "星期三"; break;
                case "Thursday": name = "星期四"; break;
                case "Friday": name = "星期五"; break;
                case "Saturday": name = "星期六"; break;
                case "Sunday": name = "星期日"; break;
                default: name = ""; break;
            }
            return name;
        }
    }
}
