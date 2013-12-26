using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using FISCA.UDT;
using SmartSchool.API.PlugIn;

namespace JHSchool.Association.ImportExport
{
    class ImportAssnCode : SmartSchool.API.PlugIn.Import.Importer
    {
        AccessHelper helper = new AccessHelper();
        List<string> Keys = new List<string>();

        public ImportAssnCode()
        {
            this.Image = null;
            this.Text = "匯入社團活動表現";
        }

        public override void InitializeImport(SmartSchool.API.PlugIn.Import.ImportWizard wizard)
        {
            wizard.PackageLimit = 3000;
            //可匯入的欄位
            wizard.ImportableFields.AddRange("學年度", "學期", "社團名稱", "分數", "努力程度", "文字描述");
            //必需要有的欄位
            wizard.RequiredFields.AddRange("學年度", "學期", "社團名稱");
            //開始驗證
            //wizard.ValidateStart += (sender, e) => Keys.Clear();
            //驗證每行資料的事件
            wizard.ValidateRow += new System.EventHandler<SmartSchool.API.PlugIn.Import.ValidateRowEventArgs>(wizard_ValidateRow);
            //實際匯入資料的事件
            wizard.ImportPackage += new System.EventHandler<SmartSchool.API.PlugIn.Import.ImportPackageEventArgs>(wizard_ImportPackage);
            wizard.ImportComplete += (sender, e) => MessageBox.Show("匯入完成!");
        }

        void wizard_ValidateRow(object sender, SmartSchool.API.PlugIn.Import.ValidateRowEventArgs e)
        {
            #region 驗各欄位填寫格式
            int t;
            foreach (string field in e.SelectFields)
            {
                string value = e.Data[field];
                switch (field)
                {
                    default:
                        break;
                    case "學年度":
                        if (value == "" || !int.TryParse(value, out t))
                            e.ErrorFields.Add(field, "此欄為必填欄位，必須填入整數。");
                        break;
                    case "學期":
                        if (value == "" || !int.TryParse(value, out t))
                        {
                            e.ErrorFields.Add(field, "此欄為必填欄位，必須填入整數。");
                        }
                        else if (t != 1 && t != 2)
                        {
                            e.ErrorFields.Add(field, "必須填入1或2");
                        }
                        break;
                    //case "社團名稱":
                    //    if (string.IsNullOrEmpty(value))
                    //        e.ErrorFields.Add(field, "此欄為必填欄位。");
                    //    break;
                }
            }
            #endregion
            #region 驗證主鍵

            string Key = e.Data.ID +"-"+e.Data["學年度"]+"-"+e.Data["學期"]+"-"+e.Data["社團名稱"];
            string errorMessage = string.Empty;

            if (Keys.Contains(Key))
                errorMessage = "學生編號、學年度、學期及社團名稱的組合不能重覆!";
            else
                Keys.Add(Key);

            e.ErrorMessage = errorMessage;

            #endregion
        }

        void wizard_ImportPackage(object sender, SmartSchool.API.PlugIn.Import.ImportPackageEventArgs e)
        {
            //根據學生編號、學年度、學期組成Key
            List<string> keyList = new List<string>();
            Dictionary<string, int> schoolYearMapping = new Dictionary<string, int>();
            Dictionary<string, int> semesterMapping = new Dictionary<string, int>();
            Dictionary<string, string> studentIDMapping = new Dictionary<string, string>();
            Dictionary<string, List<RowData>> rowsMapping = new Dictionary<string, List<RowData>>();
            Dictionary<string, List<AssnCode>> studentAttendanceInfo = new Dictionary<string, List<AssnCode>>();

            //掃描每行資料，定出資料的PrimaryKey，並且將PrimaryKey對應到的資料寫成Dictionary
            foreach (RowData Row in e.Items)
            {
                int schoolYear = int.Parse(Row["學年度"]);
                int semester = int.Parse(Row["學期"]);
                string studentID = Row.ID;
                string key = schoolYear + "^_^" + semester + "^_^" + studentID;

                if (!keyList.Contains(key))
                {
                    keyList.Add(key);
                    schoolYearMapping.Add(key, schoolYear);
                    semesterMapping.Add(key, semester);
                    studentIDMapping.Add(key, studentID);
                    rowsMapping.Add(key, new List<RowData>());
                }
                rowsMapping[key].Add(Row);
            }

            string strCondition = string.Empty;

            foreach (string ID in studentIDMapping.Values.Distinct())
                strCondition += strCondition == string.Empty ? "'" + ID + "'" : ",'" + ID + "'";

            List<AssnCode> records = helper.Select<AssnCode>("StudentID in (" + strCondition + ")");

            #region 抓學生現有的社團活動表現
            foreach (AssnCode var in records)
            {
                if (!studentAttendanceInfo.ContainsKey(var.StudentID))
                    studentAttendanceInfo.Add(var.StudentID, new List<AssnCode>());
                studentAttendanceInfo[var.StudentID].Add(var);
            }
            #endregion

            List<ActiveRecord> InsertAssnCodes = new List<ActiveRecord>();
            List<ActiveRecord> UpdateAssnCodes = new List<ActiveRecord>();

            foreach (string key in keyList)
            {
                //根據學生編號、學年度、學期及日期取得缺曠記錄
                List<AssnCode> AssignCodes = new List<AssnCode>();

                if (studentAttendanceInfo.ContainsKey(studentIDMapping[key]))
                    AssignCodes = studentAttendanceInfo[studentIDMapping[key]].Where(x => x.SchoolYear == schoolYearMapping[key].ToString() && x.Semester == semesterMapping[key].ToString()).ToList();

                //根據鍵值取得匯入資料，該匯入資料應該是有相同的學生編號、學年度、學期及缺曠日期
                List<RowData> Rows = rowsMapping[key];

                //該筆缺曠記錄已存在系統中
                if (AssignCodes.Count > 0)
                {
                    //根據學生編號、學年度、學期及日期取得的缺曠記錄應該只有一筆
                    AssnCode AssnCodeRec = AssignCodes[0];

                    XmlDocument xmldoc = new XmlDocument();

                    xmldoc.LoadXml(AssnCodeRec.Scores);

                    for (int i = 0; i < Rows.Count; i++)
                    {
                        //取得匯入資料的節次及假別
                        string AssName = Rows[i]["社團名稱"];

                        bool IsExist = false;

                        XmlElement Element = xmldoc.DocumentElement.SelectSingleNode("Item[@AssociationName='"+AssName+"']") as XmlElement;

                        //社團名稱已經存在會更新分數
                        if (Element != null)
                        {
                            if (e.ImportFields.Contains("分數"))
                                Element.SetAttribute("Score", Rows[i]["分數"]);
                            if (e.ImportFields.Contains("努力程度"))
                                Element.SetAttribute("Effort", Rows[i]["努力程度"]);
                            if (e.ImportFields.Contains("文字描述"))
                                Element.SetAttribute("Text", Rows[i]["文字描述"]);
                            IsExist = true;
                        }

                        //若是社團名稱不存在則會新增
                        if (!IsExist)
                        {
                            XmlElement iElement = xmldoc.CreateElement("Item");
                            iElement.SetAttribute("AssociationName", e.ImportFields.Contains("社團名稱") ? Rows[i]["社團名稱"] : string.Empty);
                            iElement.SetAttribute("Score", e.ImportFields.Contains("分數") ? Rows[i]["分數"] : string.Empty);
                            iElement.SetAttribute("Effort", e.ImportFields.Contains("努力程度") ? Rows[i]["努力程度"] : string.Empty);
                            iElement.SetAttribute("Text", e.ImportFields.Contains("文字描述") ? Rows[i]["文字描述"] : string.Empty);
                            xmldoc.DocumentElement.AppendChild(iElement);
                        }

                        AssnCodeRec.Scores = xmldoc.OuterXml;
                    }

                    UpdateAssnCodes.Add(AssnCodeRec);
                }
                else //該筆缺曠記錄沒有存在系統中
                {
                    AssnCode record = new AssnCode();

                    record.SchoolYear = schoolYearMapping[key].ToString();
                    record.Semester = semesterMapping[key].ToString();
                    record.StudentID = Rows[0].ID;

                    XmlDocument xmldoc = new XmlDocument();

                    xmldoc.LoadXml("<Content/>");

                    //將屬於同樣一筆的匯入資料都加入到同樣的缺曠記錄中的明細
                    foreach (RowData Row in Rows)
                    {
                        XmlElement iElement = xmldoc.CreateElement("Item");

                        iElement.SetAttribute("AssociationName", e.ImportFields.Contains("社團名稱") ? Row["社團名稱"] : string.Empty);
                        iElement.SetAttribute("Score", e.ImportFields.Contains("分數") ? Row["分數"] : string.Empty);
                        iElement.SetAttribute("Effort", e.ImportFields.Contains("努力程度") ? Row["努力程度"] : string.Empty);
                        iElement.SetAttribute("Text", e.ImportFields.Contains("文字描述") ? Row["文字描述"] : string.Empty);
                        xmldoc.DocumentElement.AppendChild(iElement);
                    }

                    record.Scores = xmldoc.OuterXml;

                    InsertAssnCodes.Add(record);
                }
            }

            if (InsertAssnCodes.Count > 0)
                helper.InsertValues(InsertAssnCodes);

            if (UpdateAssnCodes.Count > 0)
                helper.UpdateValues(UpdateAssnCodes);
        }
    }
}