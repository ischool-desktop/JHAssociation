using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using System.Xml;

namespace JHSchool.Association
{
    /// <summary>
    /// 高雄國中評量成績記錄
    /// </summary>
    public class AssnScoreRecord
    {
        private JHSchool.Data.JHSCETakeRecord _record;

        /// <summary>
        /// 取得或設定學生ID
        /// </summary>
        public string RefStudentID
        {
            get { return _record.RefStudentID; }
            set { _record.RefStudentID = value; }
        }

        /// <summary>
        /// 取得或設定課程ID
        /// </summary>
        public string RefCourseID
        {
            get { return _record.RefCourseID; }
            set { _record.RefCourseID = value; }
        }

        /// <summary>
        /// 取得或設定修課ID
        /// </summary>
        public string RefSCAttendID
        {
            get { return _record.RefSCAttendID; }
            set { _record.RefSCAttendID = value; }
        }

        /// <summary>
        /// 取得或設定試別ID
        /// </summary>
        public string RefExamID
        {
            get { return _record.RefExamID; }
            set { _record.RefExamID = value; }
        }

        /// <summary>
        /// 取得或設定分數評量
        /// </summary>
        public decimal? Score
        {
            get { return _record.Score; }
            set { _record.Score = value; }
        }

        /// <summary>
        /// 取得或設定努力程度
        /// </summary>
        public int? Effort { get; set; }

        /// <summary>
        /// 取得或設定文字描述
        /// </summary>
        public string Text
        {
            get { return _record.Text; }
            set { _record.Text = value; }
        }

        /// <summary>
        /// 建構子
        /// </summary>
        /// <param name="record"></param>
        public AssnScoreRecord(JHSchool.Data.JHSCETakeRecord record)
        {
            _record = record;

            #region 嘗試取得 Effort
            XmlNode effort = record.ToXML().SelectSingleNode("Extension/Extension/Effort");
            if (effort != null) Effort = ParseIntAllowNull(effort.InnerText);
            #endregion
        }

        private int? ParseIntAllowNull(string p)
        {
            int i;
            if (int.TryParse(p, out i))
                return i;
            return null;
        }

        /// <summary>
        /// 轉型成JHSCETakeRecord
        /// </summary>
        /// <returns></returns>
        public JHSchool.Data.JHSCETakeRecord AsJHSCETakeRecord()
        {
            XmlElement root = _record.ToXML();
            XmlNode effort = root.SelectSingleNode("Extension/Extension/Effort");
            if (effort == null)
            {
                effort = root.OwnerDocument.CreateElement("Effort");
                root.SelectSingleNode("Extension/Extension").AppendChild(effort);
            }
            effort.InnerText = "" + Effort;
            _record.Load(root);
            return _record;
        }
    }

    public static class KH_JHSCETakeRecord_ExtendMethod
    {
        public static List<JHSchool.Data.JHSCETakeRecord> AsJHSCETakeRecords(this List<AssnScoreRecord> list)
        {
            List<JHSCETakeRecord> result = new List<JHSCETakeRecord>();
            foreach (var item in list)
                result.Add(item.AsJHSCETakeRecord());
            return result;
        }

        public static List<AssnScoreRecord> AsKHJHSCETakeRecords(this List<JHSchool.Data.JHSCETakeRecord> list)
        {
            List<AssnScoreRecord> result = new List<AssnScoreRecord>();
            foreach (var item in list)
                result.Add(new AssnScoreRecord(item));
            return result;
        }
    }
}
