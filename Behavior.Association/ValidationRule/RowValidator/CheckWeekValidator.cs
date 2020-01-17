using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Campus.DocumentValidator;
using FISCA.Data;

namespace JHSchool.Association
{
    /// <summary>
    /// 檢查系統中是否社團名稱/學年度/學期重覆
    /// </summary>
    public class CheckWeekValidator : IRowVaildator
    {
        private Dictionary<string,string> mAssocNames;

        public CheckWeekValidator()
        {
            mAssocNames = new Dictionary<string, string>();
        }

        #region IRowVaildator 成員

        public bool Validate(IRowStream Value)
        {
            if (Value.Contains("學年度") && Value.Contains("學期") && Value.Contains("年級") && Value.Contains("單雙周"))
            {
                string SchoolYear = Value.GetValue("學年度");
                string Semester = Value.GetValue("學期");
                string GradeYear = Value.GetValue("年級");

                string Week = Value.GetValue("單雙周");
                string Key = SchoolYear + "," + Semester + "," + GradeYear;

                if (!mAssocNames.ContainsKey(Key))
                {
                    mAssocNames.Add(Key, Week);
                    return true;
                }
                else
                {
                    if (mAssocNames[Key] == Week)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 沒有提供自動修正
        /// </summary>
        public string Correct(IRowStream Value)
        {
            return string.Empty;
        }

        /// <summary>
        /// 傳回預設樣版
        /// </summary>
        public string ToString(string template)
        {
            return template;
        }

        #endregion
    }
}