using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using FISCA.UDT;

namespace JHSchool.Association
{
    class AssnSelect
    {
        /// <summary>
        /// 課程ID/社團記錄物件
        /// </summary>
        public Dictionary<string, AssnObj> DicAssnCode = new Dictionary<string, AssnObj>();

        List<AssnScoreRecord> AssnScore = new List<AssnScoreRecord>();

        /// <summary>
        /// 所有學生ID
        /// </summary>
        private List<string> StudentTotelID = new List<string>();

        /// <summary>
        /// 設定檔
        /// </summary>
        private JHAEIncludeRecord _JHAEIncludeConfig = new JHAEIncludeRecord();

        string UseEffort = "努力程度";
        string UseScore = "成績";
        string UseText = "文字描述";

        /// <summary>
        /// 建構子
        /// </summary>
        public AssnSelect(JHAEIncludeRecord JHAEIncludeConfig)
        {
            _JHAEIncludeConfig = JHAEIncludeConfig;
            //取得全部社團課程
            Reset();

            DataSetup();
        }

        private void Reset()
        {
            #region Reset
            DicAssnCode.Clear();
            StudentTotelID.Clear();

            JHCourse.SelectAll();

            List<JHCourseRecord> CourseList = JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource);
            CourseList.Sort(new Comparison<JHCourseRecord>(CourseSort));

            foreach (JHCourseRecord each in CourseList)
            {
                if (!DicAssnCode.ContainsKey(each.ID))
                {
                    DicAssnCode.Add(each.ID, new AssnObj());
                    DicAssnCode[each.ID].AssnCouseID = each.ID;
                    DicAssnCode[each.ID].AssnCouseName = each.Name;
                    DicAssnCode[each.ID].TeacherName = each.MajorTeacherName;

                    if (_JHAEIncludeConfig.UseEffort)
                    {
                        DicAssnCode[each.ID].AssnSetupList.Add(UseEffort, 0);
                    }

                    if (_JHAEIncludeConfig.UseScore)
                    {
                        DicAssnCode[each.ID].AssnSetupList.Add(UseScore, 0);
                    }

                    if (_JHAEIncludeConfig.UseText)
                    {
                        DicAssnCode[each.ID].AssnSetupList.Add(UseText, 0);
                    }
                }
            }

            //傳入選擇的課程清單,取得修課學生
            List<JHSCAttendRecord> _SCAList = JHSCAttend.SelectByCourseIDs(AssnAdmin.Instance.SelectedSource);

            foreach (JHSCAttendRecord each in _SCAList)
            {
                //狀態為一般才處理
                if (each.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    //學生數++
                    DicAssnCode[each.RefCourseID].CourseCount++;

                    //記錄所有學生
                    if (!StudentTotelID.Contains(each.RefStudentID))
                    {
                        StudentTotelID.Add(each.RefStudentID);
                    }
                }
            }

            //取得所有選擇的課程&修課學生ID
            List<JHSCETakeRecord> SCETakeList = JHSCETake.SelectByStudentAndCourse(StudentTotelID, DicAssnCode.Keys);
            //轉形(學生成績資料)
            AssnScore.Clear();
            AssnScore = SCETakeList.AsKHJHSCETakeRecords();
            #endregion
        }

        /// <summary>
        /// 進行學生資料輸入狀況的加總
        /// </summary>
        private void DataSetup()
        {
            #region DataSetup
            foreach (AssnScoreRecord each in AssnScore)
            {
                if (_JHAEIncludeConfig.RefExamID == each.RefExamID)
                {
                    if (_JHAEIncludeConfig.UseEffort)
                    {
                        if (each.Effort.HasValue)
                        {
                            DicAssnCode[each.RefCourseID].AssnSetupList[UseEffort]++;
                        }
                    }
                    if (_JHAEIncludeConfig.UseScore)
                    {
                        if (each.Score.HasValue)
                        {
                            DicAssnCode[each.RefCourseID].AssnSetupList[UseScore]++;
                        }
                    }
                    if (_JHAEIncludeConfig.UseText)
                    {
                        if (each.Text != "")
                        {
                            DicAssnCode[each.RefCourseID].AssnSetupList[UseText]++;
                        }
                    }
                }
            } 
            #endregion
        }

        private int CourseSort(JHCourseRecord x, JHCourseRecord y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
