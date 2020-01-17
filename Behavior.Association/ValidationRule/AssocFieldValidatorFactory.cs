using Campus.DocumentValidator;

namespace JHSchool.Association
{
    /// <summary>
    /// 用來產生排課系統所需的自訂驗證規則
    /// </summary>
    public class AssocFieldValidatorFactory : IFieldValidatorFactory
    {
        #region IFieldValidatorFactory 成員

        /// <summary>
        /// 根據typeName建立對應的FieldValidator
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="validatorDescription"></param>
        /// <returns></returns>
        public IFieldValidator CreateFieldValidator(string typeName, System.Xml.XmlElement validatorDescription)
        {
            switch (typeName.ToUpper())
            {
                case "CHECKGRADEYEARASSOC":
                    return new GradeYearStatusValidator(); //取得ischool系統內的所有老師
                case "CHECKPERIODASSOC":
                    return new PeriodStatusValidator(); //取得ischool系統內的所有老師
                default:
                    return null;
            }
        }

        #endregion
    }
}