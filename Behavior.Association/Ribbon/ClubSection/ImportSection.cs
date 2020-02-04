using Campus.DocumentValidator;
using Campus.Import;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JHSchool.Association
{
    class ImportSection : ImportWizard
    {
        string _schoolyear { get; set; }
        string _semester { get; set; }

        public ImportSection(string schoolyear, string semester)
        {
            _schoolyear = schoolyear;
            _semester = semester;
        }
        //設定檔
        private ImportOption mOption;

        List<ClubSchedule> ClubScheduleList { get; set; }
        List<ClubSetting> ClubSettingList { get; set; }

        Dictionary<string, List<ClubSchedule>> ClubScheduleDic { get; set; }
        Dictionary<string, ClubSetting> ClubSettingDic { get; set; }
        /// <summary>
        /// 驗證規則
        /// </summary>
        public override string GetValidateRule()
        {
            return Properties.Resources.SectionValidate;
        }

        /// <summary>
        /// 準備資料
        /// </summary>
        public override void Prepare(ImportOption Option)
        {
            mOption = Option;

            //取得目前的學年期與它的時間表資料
            ClubScheduleList = tool._A.Select<ClubSchedule>();
            ClubSettingList = tool._A.Select<ClubSetting>();
            ClubScheduleDic = new Dictionary<string, List<ClubSchedule>>();
            ClubSettingDic = new Dictionary<string, ClubSetting>();

            //整理學年度學期
            foreach (ClubSchedule each in ClubScheduleList)
            {
                string SchoolYear = each.SchoolYear;
                string Semester = each.Semester;
                string GradeYear = each.GradeYear;
                string Key1 = SchoolYear + "," + Semester + "," + GradeYear;
                if (!ClubScheduleDic.ContainsKey(Key1))
                {
                    ClubScheduleDic.Add(Key1, new List<ClubSchedule>());
                }
                ClubScheduleDic[Key1].Add(each);
            }

            //整理單雙周
            foreach (ClubSetting each in ClubSettingList)
            {
                string SchoolYear = each.SchoolYear;
                string Semester = each.Semester;
                string GradeYear = each.GradeYear;
                string Key2 = SchoolYear + "," + Semester + "," + GradeYear;

                if (!ClubSettingDic.ContainsKey(Key2))
                {
                    ClubSettingDic.Add(Key2, each);
                }
            }
        }

        public override string Import(List<Campus.DocumentValidator.IRowStream> Rows)
        {
            if (mOption.Action == ImportAction.Insert)
            {
                //整理資料,以比對要刪除那些舊資料
                Dictionary<string, List<ClubSchedule>> scheduleDic = new Dictionary<string, List<ClubSchedule>>();
                Dictionary<string, ClubSetting> settingDic = new Dictionary<string, ClubSetting>();

                foreach (IRowStream Row in Rows)
                {
                    //教師名稱
                    string SchoolYear = Row.GetValue("學年度");
                    string Semester = Row.GetValue("學期");
                    string GradeYear = Row.GetValue("年級");
                    string Period = Row.GetValue("節次");
                    string OccurDate = Row.GetValue("日期");
                    string SingleDoubleWeek = Row.GetValue("上課週次");

                    ClubSchedule cs = new ClubSchedule();
                    cs.GradeYear = GradeYear;
                    cs.SchoolYear = SchoolYear;
                    cs.Semester = Semester;
                    cs.Period = Period;

                    DateTime dt = DateTime.Parse(OccurDate);
                    cs.OccurDate = dt;
                    cs.Week = tool.GetWeekName(dt);

                    //Key 學年度+學期+年級+日期+節次 : 不能重複
                    string Key1 = SchoolYear + "," + Semester + "," + GradeYear;
                    if (!scheduleDic.ContainsKey(Key1))
                    {
                        scheduleDic.Add(Key1, new List<ClubSchedule>());
                    }
                    scheduleDic[Key1].Add(cs);

                    string Key2 = SchoolYear + "," + Semester + "," + GradeYear;
                    if (!settingDic.ContainsKey(Key2))
                    {
                        ////學年度+學期+年級 : 單雙周 不重複
                        ClubSetting setting = new ClubSetting();
                        setting.SchoolYear = SchoolYear;
                        setting.Semester = Semester;
                        setting.GradeYear = GradeYear;
                        if (SingleDoubleWeek == "每週上課")
                        {
                            setting.IsSingleDoubleWeek = false;
                        }
                        else
                        {
                            setting.IsSingleDoubleWeek = true;
                        }
                        settingDic.Add(Key2, setting);
                    }
                }

                //資料比對後決定要刪除與新增哪些資料
                List<ClubSchedule> InsertScheduleList = new List<ClubSchedule>();
                List<ClubSetting> InsertSettingList = new List<ClubSetting>();
                List<ClubSchedule> DeleteScheduleList = new List<ClubSchedule>();
                List<ClubSetting> DeleteSettingList = new List<ClubSetting>();

                foreach (string each in scheduleDic.Keys)
                {
                    if (ClubScheduleDic.ContainsKey(each))
                    {
                        //新增資料
                        InsertScheduleList.AddRange(scheduleDic[each]);
                        //一併刪除當學期+年級資料
                        DeleteScheduleList.AddRange(ClubScheduleDic[each]);
                    }
                    else
                    {
                        //只新增資料,不刪除資料
                        InsertScheduleList.AddRange(scheduleDic[each]);

                    }
                }

                foreach (string each in settingDic.Keys)
                {
                    if (ClubSettingDic.ContainsKey(each))
                    {
                        InsertSettingList.Add(settingDic[each]);
                        DeleteSettingList.Add(ClubSettingDic[each]);
                    }
                    else
                    {
                        InsertSettingList.Add(settingDic[each]);
                    }
                }

                StringBuilder sb_log = new StringBuilder();

                if (InsertScheduleList.Count > 0)
                {
                    tool._A.InsertValues(InsertScheduleList);
                   tool._A.DeletedValues(DeleteScheduleList);

                    if (InsertScheduleList.Count > 0)
                    {
                        sb_log.AppendLine("匯入日期資料：");
                        foreach (ClubSchedule each in InsertScheduleList)
                        {
                            sb_log.AppendLine(string.Format("學年度「{0}」學期「{1}」年級「{2}」日期「{3}」節次「{4}」", each.SchoolYear, each.Semester, each.GradeYear, each.OccurDate.ToString("yyyy/MM/dd"), each.Period));
                        }
                    }
                    if (DeleteScheduleList.Count > 0)
                    {
                        sb_log.AppendLine("");
                        sb_log.AppendLine("被覆蓋前日期資料：");
                        foreach (ClubSchedule each in DeleteScheduleList)
                        {
                            sb_log.AppendLine(string.Format("學年度「{0}」學期「{1}」年級「{2}」日期「{3}」節次「{4}」", each.SchoolYear, each.Semester, each.GradeYear, each.OccurDate.ToString("yyyy/MM/dd"), each.Period));
                        }
                    }
                }

                if (InsertSettingList.Count > 0)
                {
                    tool._A.InsertValues(InsertSettingList);
                    tool._A.DeletedValues(DeleteSettingList);
                    if (InsertSettingList.Count > 0)
                    {
                        sb_log.AppendLine("");
                        sb_log.AppendLine("上課周次設定：");
                        foreach (ClubSetting each in InsertSettingList)
                        {
                            sb_log.AppendLine(string.Format("學年度「{0}」學期「{1}」年級「{2}」為「{3}」週", each.SchoolYear, each.Semester, each.GradeYear, each.IsSingleDoubleWeek ? "隔週上課" : "每週上課"));
                        }
                    }

                    if (DeleteSettingList.Count > 0)
                    {
                        sb_log.AppendLine("");
                        sb_log.AppendLine("被覆蓋前設定：");
                        foreach (ClubSetting each in DeleteSettingList)
                        {
                            sb_log.AppendLine(string.Format("學年度「{0}」學期「{1}」年級「{2}」為「{3}」週", each.SchoolYear, each.Semester, each.GradeYear, each.IsSingleDoubleWeek ? "隔週上課" : "每週上課"));
                        }
                    }
                }

                if (sb_log.ToString().Length > 0)
                {
                    FISCA.LogAgent.ApplicationLog.Log("社團時間表", "匯入", sb_log.ToString());
                }
            }

            return "";
        }

        public override ImportAction GetSupportActions()
        {
            //新增
            return ImportAction.Insert;
        }
    }
}
