using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Campus.DocumentValidator;
using K12.Data;
using System.Data;

namespace JHSchool.Association
{
    class PeriodStatusValidator : IFieldValidator
    {
        List<string> _PeriodList { get; set; }

        public PeriodStatusValidator()
        {
            _PeriodList = GetPeriodYear();
        }

        #region IFieldValidator 成員

        public string Correct(string Value)
        {
            return Value;
        }

        public string ToString(string template)
        {
            return template;
        }

        public bool Validate(string Value)
        {
            if (_PeriodList.Contains(Value)) //包含此學號
            {
                return true;//True表示學生為一般生
            }
            return false;
        }

        /// <summary>
        /// 取得學生學號 vs 系統編號
        /// </summary>
        private List<string> GetPeriodYear()
        {
            List<string> list = new List<string>();

            List<PeriodMappingInfo> PeriodList = K12.Data.PeriodMapping.SelectAll();
            foreach (PeriodMappingInfo each in PeriodList)
            {
                if (string.IsNullOrEmpty(each.Name))
                    continue;
                if (!list.Contains(each.Name))
                {
                    list.Add(each.Name);
                }
            }
            return list;

        }

        #endregion
    }
}
