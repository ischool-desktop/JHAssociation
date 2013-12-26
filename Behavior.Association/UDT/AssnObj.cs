using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Association
{
    class AssnObj
    {
        /// <summary>
        /// 社團課程ID
        /// </summary>
        public string AssnCouseID { set; get; }

        /// <summary>
        /// 社團課程名稱
        /// </summary>
        public string AssnCouseName { set; get; }

        /// <summary>
        /// 社團導師姓名
        /// </summary>
        public string TeacherName { get; set; }

        /// <summary>
        /// 社團課程學生人數
        /// </summary>
        public int CourseCount { get; set; }

        /// <summary>
        /// 行為表現
        /// </summary>
        public Dictionary<string, int> AssnSetupList = new Dictionary<string, int>();

        ///// <summary>
        ///// 傳入資料清單(設定檔)
        ///// </summary>
        ///// <param name="list"></param>
        //public AssnObj(List<string> list)
        //{
        //    foreach (string each in list)
        //    {
        //        if (!AssnSetupList.ContainsKey(each))
        //        {
        //            AssnSetupList.Add(each, 0);
        //        }
        //    }
        //}
    }
}
