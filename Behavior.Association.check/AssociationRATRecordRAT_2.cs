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
using System.Windows.Forms;

namespace JHSchool.Association.check
{
    //檢查社團成績
    //在相同學年度學期內,是否有重覆的資料

    public class AssociationRATRecord_2
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

        #endregion
    }

    public class AssociationRATRecordRAT_2 : ICorrectableDataRationality
    {
        private List<AssociationRATRecord_2> RATRecords = new List<AssociationRATRecord_2>();
        private Dictionary<string, AssociationRATRecord_2> RATRecordsDic = new Dictionary<string, AssociationRATRecord_2>();

        private Dictionary<string, AssnCode> AssnDic = new Dictionary<string, AssnCode>();
        FISCA.UDT.AccessHelper _A = new FISCA.UDT.AccessHelper();

        CheckOjb obj { get; set; }

        public AssociationRATRecordRAT_2()
        {
        }


        #region IDataRationality 成員

        /// <summary>
        /// 學生缺曠資料與系統假別
        /// </summary>
        public string Name
        {
            get { return "社團重覆成績檢查(高雄市適用)"; }
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

            obj = new CheckOjb(AssnList);

            DataRationalityMessage Message = new DataRationalityMessage();


            int Count = 0;

            foreach (StudAssnObj AttRec in obj.StudAssnDic.Values)
            {
                foreach (List<AssnCode> list in AttRec._AssnDic.Values)
                {
                    Count++;

                    if (list.Count > 1)
                    {
                        foreach (AssnCode each in list)
                        {
                            if (each.Scores != "") //成績不為空白時
                            {
                                XmlElement xml = DSXmlHelper.LoadXml(each.Scores); //將文字轉為XmlElement

                                if (xml.SelectNodes("Item").Count >= 1)
                                {
                                    AssociationRATRecord_2 record = new AssociationRATRecord_2();
                                    record.學生系統編號 = AttRec._Stud.ID;
                                    record.學號 = AttRec._Stud.StudentNumber;
                                    record.班級 = AttRec._class != null ? AttRec._class.Name : "";
                                    record.座號 = AttRec._Stud.SeatNo.HasValue ? AttRec._Stud.SeatNo.Value.ToString() : "";
                                    record.姓名 = AttRec._Stud.Name;

                                    record.學年度 = AttRec.SchoolYear;
                                    record.學期 = AttRec.Semester;
                                    record.社團成績系統編號 = each.UID;

                                    record.社團名稱1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("AssociationName");
                                    record.成績1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Score");
                                    record.努力程度1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Effort");
                                    record.文字描述1 = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Text");

                                    RATRecords.Add(record);
                                }
                            }
                        }
                    }
                    else
                    {
                        //沒有資料
                    }
                }
            }

            foreach (AssociationRATRecord_2 each in RATRecords)
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
                foreach (AssociationRATRecord_2 APeach in RATRecords)
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

        private int SortStudent(StudAssnObj a, StudAssnObj b)
        {
            string classname_a = a._class != null ? a._class.Name.PadLeft(10, '0') : "0000000000";
            string classname_b = b._class != null ? b._class.Name.PadLeft(10, '0') : "0000000000";

            classname_a += a._Stud.SeatNo.HasValue ? a._Stud.SeatNo.Value.ToString().PadLeft(3, '0') : "000";
            classname_b += b._Stud.SeatNo.HasValue ? b._Stud.SeatNo.Value.ToString().PadLeft(3, '0') : "000";

            classname_a += a._Stud.Name.PadLeft(5, '0');
            classname_b += b._Stud.Name.PadLeft(5, '0');

            return classname_a.CompareTo(classname_b);
        }

        /// <summary>
        /// 移除重覆之資料
        /// </summary>
        public void ExecuteAutoCorrect()
        {
            DialogResult dr = MsgBox.Show("您確定要自動修正所有錯誤資料?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.Yes)
            {
                List<AssnCode> Assn_Out = new List<AssnCode>();
                List<AssnCode> Assn_in = new List<AssnCode>();
                StringBuilder SBLog = new StringBuilder();

                List<StudAssnObj> list_l = obj.StudAssnDic.Values.ToList();
                list_l.Sort(SortStudent);
                foreach (StudAssnObj AttRec in list_l)
                {
                    string classname = AttRec._class != null ? AttRec._class.Name : "";
                    string seatNo = AttRec._Stud.SeatNo.HasValue ? AttRec._Stud.SeatNo.Value.ToString() : "";

                    List<string> list_name = new List<string>();
                    foreach (List<AssnCode> list in AttRec._AssnDic.Values)
                    {
                        //大於1就是有重覆之社團資料
                        if (list.Count > 1)
                        {
                            foreach (AssnCode each in list)
                            {
                                string SchoolYear = each.SchoolYear;
                                string Semester = each.Semester;

                                if (each.Scores != "") //成績不為空白時
                                {
                                    XmlElement xml = DSXmlHelper.LoadXml(each.Scores);
                                    string AssociationName = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("AssociationName");
                                    string Score = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Score");
                                    string Effort = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Effort");
                                    string Text = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Text");

                                    if (list_name.Contains(SchoolYear + Semester + AssociationName + Score + Effort + Text))
                                    {
                                        SBLog.Append(string.Format("班級「{0}」座號「{1}」學生「{2}」", classname, seatNo, AttRec._Stud.Name));
                                        SBLog.AppendLine(string.Format("學年度「{0}」學期「{1}」社團名稱「{2}」成績「{3}」努力程度「{4}」文字描述「{5}」", SchoolYear, Semester, AssociationName, Score, Effort, Text));
                                        Assn_Out.Add(each);
                                    }
                                    else
                                    {
                                        list_name.Add(SchoolYear + Semester + AssociationName + Score + Effort + Text);
                                        Assn_in.Add(each);
                                        continue;
                                    }
                                }
                            }
                        }
                    }
                }

                FISCA.LogAgent.ApplicationLog.Log("社團成績重覆檢查", "刪除社團成績", "移除成績共「" + Assn_Out.Count + "」筆\n保留成績共「" + Assn_in.Count + "」筆\n明細如下:\n\n" + SBLog.ToString());
                //刪除
                _A.DeletedValues(Assn_Out);

                MsgBox.Show("已完成!\n移除成績「" + Assn_Out.Count + "」筆\n保留成績「" + Assn_in.Count + "」筆\n詳情請檢視系統歷程");
            }
            else
            {
                MsgBox.Show("已取消操作!!");
            }
        }


        /// <summary>
        /// 刪除所選資料
        /// </summary>
        public void ExecuteAutoCorrect(IEnumerable<string> EntityIDs)
        {
              DialogResult dr = MsgBox.Show("您確認要刪除所選資料?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);

              if (dr == DialogResult.Yes)
              {
                  StringBuilder SBLog = new StringBuilder();

                  List<AssnCode> AssnKeys = new List<AssnCode>();

                  foreach (string each in EntityIDs)
                  {
                      if (AssnDic.ContainsKey(each))
                      {
                          AssnCode a = AssnDic[each];

                          if (obj.StudAssnDic.ContainsKey(a.StudentID))
                          {
                              StudAssnObj sa = obj.StudAssnDic[a.StudentID];

                              string classname = sa._class != null ? sa._class.Name : "";
                              string seatNo = sa._Stud.SeatNo.HasValue ? sa._Stud.SeatNo.Value.ToString() : "";

                              string SchoolYear = a.SchoolYear;
                              string Semester = a.Semester;

                              if (a.Scores != "") //成績不為空白時
                              {
                                  XmlElement xml = DSXmlHelper.LoadXml(a.Scores);
                                  string AssociationName = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("AssociationName");
                                  string Score = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Score");
                                  string Effort = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Effort");
                                  string Text = ((XmlElement)xml.SelectNodes("Item")[0]).GetAttribute("Text");

                                  SBLog.Append(string.Format("班級「{0}」座號「{1}」學生「{2}」", classname, seatNo, sa._Stud.Name));
                                  SBLog.AppendLine(string.Format("學年度「{0}」學期「{1}」社團名稱「{2}」成績「{3}」努力程度「{4}」文字描述「{5}」", SchoolYear, Semester, AssociationName, Score, Effort, Text));

                                  AssnKeys.Add(a);
                              }
                          }
                      }
                  }

                  //刪除
                  FISCA.LogAgent.ApplicationLog.Log("社團成績重覆檢查", "刪除社團成績", "移除成績共「" + AssnKeys.Count + "」筆\n明細如下:\n\n" + SBLog.ToString());
                  _A.DeletedValues(AssnKeys);
                  MsgBox.Show("已完成!\n移除選擇成績共「" + AssnKeys.Count + "」筆");
              }
              else
              {
                  MsgBox.Show("已取消操作!!");
              }
        }
        #endregion
    }
}