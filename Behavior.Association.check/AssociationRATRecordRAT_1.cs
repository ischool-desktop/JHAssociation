using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataRationality;
using K12.Data;
using System.Data;
using System;
using System.Xml;
using FISCA.DSAUtil;
using FISCA.Presentation.Controls;

namespace JHSchool.Association.check
{
    //檢查社團成績
    //在相同學年度學期內,是否有重覆的資料

    public class AssociationRATRecord_1
    {
        #region 欄位屬性

        public string 社團成績系統編號 { get; set; }

        public string 學生系統編號 { get; set; }

        public string 學號 { get; set; }

        public string 班級 { get; set; }

        public string 座號 { get; set; }

        public string 姓名 { get; set; }

        public string 學年度 { get; set; }

        public string 學期 { get; set; }

        public string 社團名稱1 { get; set; }
        public string 成績1 { get; set; }
        public string 努力程度1 { get; set; }
        public string 文字描述1 { get; set; }

        public string 社團名稱2 { get; set; }
        public string 成績2 { get; set; }
        public string 努力程度2 { get; set; }
        public string 文字描述2 { get; set; }

        #endregion
    }

    public class AssociationRATRecordRAT_1 : ICorrectableDataRationality
    {
        private List<AssociationRATRecord_1> RATRecords = new List<AssociationRATRecord_1>();
        private Dictionary<string, AssociationRATRecord_1> RATRecordsDic = new Dictionary<string, AssociationRATRecord_1>();

        private Dictionary<string, AssnCode> AssnDic = new Dictionary<string, AssnCode>();
        FISCA.UDT.AccessHelper _A = new FISCA.UDT.AccessHelper();

        public AssociationRATRecordRAT_1()
        {
        }


        #region IDataRationality 成員

        /// <summary>
        /// 學生缺曠資料與系統假別
        /// </summary>
        public string Name
        {
            get { return "社團重覆成績檢查1(高雄市適用)"; }
        }

        /// <summary>
        /// 學務
        /// </summary>
        public string Category
        {
            get { return "社團"; }
        }

        /// <summary>
        /// 說明
        /// </summary>
        public string Description
        {
            get
            {
                StringBuilder sDescription = new StringBuilder();

                sDescription.Append("檢查範圍：所有學生");
                sDescription.AppendLine("檢查項目：社團成績,每學年度/學期只會有一筆成績資料。");
                sDescription.AppendLine("檢查意義：相同學年度/學期不會有2項社團成績記錄。");
                sDescription.AppendLine("自動修正(選擇)項目:(刪除所選)社團成績資料");
                sDescription.AppendLine("自動修正(所有)項目:(移除重覆)社團成績資料");

                return sDescription.ToString();
            }
        }

        /// <summary>
        /// 開始檢查
        /// </summary>
        /// <returns></returns>
        public DataRationalityMessage Execute()
        {
            RATRecords.Clear();
            RATRecordsDic.Clear();
            AssnDic.Clear();

            List<AssnCode> AssnList = _A.Select<AssnCode>();
            foreach (AssnCode each in AssnList)
            {
                if (!AssnDic.ContainsKey(each.UID))
                {
                    AssnDic.Add(each.UID, each);
                }
            }
            CheckOjb obj = new CheckOjb(AssnList);



            DataRationalityMessage Message = new DataRationalityMessage();


            int Count = 0;

            foreach (StudAssnObj AttRec in obj.StudAssnDic.Values)
            {
                foreach (List<AssnCode> list in AttRec._AssnDic.Values)
                {
                    Count++;

                    if (list.Count == 1)
                    {
                        #region 資料筆數為1 , 將查詢Element內容是否為2

                        foreach (AssnCode each in list)
                        {
                            if (each.Scores != "") //成績不為空白時
                            {
                                XmlElement xml = DSXmlHelper.LoadXml(each.Scores); //將文字轉為XmlElement

                                if (xml.SelectNodes("Item").Count > 1)
                                {
                                    AssociationRATRecord_1 record = new AssociationRATRecord_1();
                                    record.學生系統編號 = AttRec._Stud.ID;
                                    record.學號 = AttRec._Stud.StudentNumber;
                                    record.班級 = AttRec._Stud.Class != null ? AttRec._Stud.Class.Name : "";
                                    record.座號 = AttRec._Stud.SeatNo.HasValue ? AttRec._Stud.SeatNo.Value.ToString() : "";
                                    record.姓名 = AttRec._Stud.Name;

                                    record.學年度 = AttRec.SchoolYear;
                                    record.學期 = AttRec.Semester;
                                    record.社團成績系統編號 = each.UID;

                                    record.社團名稱1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("AssociationName");
                                    record.成績1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Score");
                                    record.努力程度1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Effort");
                                    record.文字描述1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Text");

                                    record.社團名稱2 = ((XmlElement)xml.SelectNodes("Item")[1]).GetAttribute("AssociationName");
                                    record.成績2 = ((XmlElement)xml.SelectNodes("Item")[1]).GetAttribute("Score");
                                    record.努力程度2 = ((XmlElement)xml.SelectNodes("Item")[1]).GetAttribute("Effort");
                                    record.文字描述2 = ((XmlElement)xml.SelectNodes("Item")[1]).GetAttribute("Text");

                                    RATRecords.Add(record);
                                }
                            }
                        }
                        #endregion
                    }                  
                }
            }

            foreach (AssociationRATRecord_1 each in RATRecords)
            {
                if (!RATRecordsDic.ContainsKey(each.社團成績系統編號))
                {
                    RATRecordsDic.Add(each.社團成績系統編號, each);
                }
            }

            StringBuilder strBuilder = new StringBuilder();

            strBuilder.AppendLine("檢查筆數：" + Count);
            strBuilder.AppendLine("問題筆數：" + RATRecords.Count);
            var SortedRATRecords = from record in RATRecords orderby record.班級, K12.Data.Int.ParseAllowNull(record.座號) select record;

            Message.Message = strBuilder.ToString();
            Message.Data = SortedRATRecords.ToList();

            return Message;
        }

        #endregion

        #region IDataRationality Members

        /// <summary>
        /// 全部加入待處理
        /// </summary>
        public void AddToTemp()
        {
            AddToTemp(null);
        }

        /// <summary>
        /// 將所選擇的資料加入待處理
        /// </summary>
        /// <param name="EntityIDs">第一欄內容</param>
        public void AddToTemp(IEnumerable<string> EntityIDs)
        {
            List<string> PrimaryKeys = new List<string>();

            if (K12.Data.Utility.Utility.IsNullOrEmpty(EntityIDs))
            {
                #region 全部加入待處理
                foreach (AssociationRATRecord_1 APeach in RATRecords)
                {
                    if (!PrimaryKeys.Contains(APeach.學生系統編號))
                    {
                        PrimaryKeys.Add(APeach.學生系統編號);
                    }
                }
                #endregion
            }
            else
            {
                #region 將所選擇的資料加入待處理
                foreach (string each in EntityIDs)
                {
                    if (RATRecordsDic.ContainsKey(each))
                    {
                        if (!PrimaryKeys.Contains(RATRecordsDic[each].學生系統編號))
                        {
                            PrimaryKeys.Add(RATRecordsDic[each].學生系統編號);
                        }
                    }
                }
                #endregion
            }

            K12.Presentation.NLDPanels.Student.AddToTemp(PrimaryKeys);
        }

        #endregion

        #region ICorrectableDataRationality 成員

        /// <summary>
        /// 將全部資料進行自動修正
        /// </summary>
        public void ExecuteAutoCorrect()
        {
            ExecuteAutoCorrect(null);
        }


        /// <summary>
        /// 對使用者選擇的資料自動修正
        /// </summary>
        /// <param name="EntityIDs">第一欄內容</param>
        public void ExecuteAutoCorrect(IEnumerable<string> EntityIDs)
        {

            List<AssnCode> AssnKeys = new List<AssnCode>();


            //#region 取得資料
            ////如果是null,就把所有ID都取得
            if (K12.Data.Utility.Utility.IsNullOrEmpty(EntityIDs))
            {
                foreach (AssociationRATRecord_1 each in RATRecords)
                {
                    if (AssnDic.ContainsKey(each.社團成績系統編號))
                    {
                        AssnKeys.Add(AssnDic[each.社團成績系統編號]);
                    }
                }
            }
            else
            {
                foreach (string each in EntityIDs)
                {
                    if (AssnDic.ContainsKey(each))
                    {
                        AssnKeys.Add(AssnDic[each]);
                    }
                }
            }

            _A.DeletedValues(AssnKeys);

            //#endregion

        }
        #endregion
    }
}