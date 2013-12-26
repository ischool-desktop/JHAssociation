using System.Collections.Generic;
using System.Xml;
using FISCA.UDT;
using SmartSchool.API.PlugIn;

namespace JHSchool.Association.ImportExport
{
    class ExportAssnCode : SmartSchool.API.PlugIn.Export.Exporter
    {
        //建構子
        public ExportAssnCode()
        {
            this.Image = null;
            this.Text = "匯出社團活動表現";
        }

        //覆寫
        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("學年度","學期","社團名稱","分數","努力程度","文字描述");

            wizard.ExportPackage += (sender,e)=>
            {
                //取得學生清單

                AccessHelper helper = new AccessHelper();

                string strCondition = string.Empty;

                foreach (string ID in e.List)
                    strCondition += strCondition == string.Empty ? "'"+ID+"'" : ",'" + ID +"'";

                List<AssnCode> records = helper.Select<AssnCode>("StudentID in ("+strCondition+")");

                for (int i = 0; i < records.Count; i++)
                {
                    XmlDocument xmldoc = new XmlDocument();

                    xmldoc.LoadXml(records[i].Scores);

                    foreach (XmlNode Node in xmldoc.DocumentElement.SelectNodes("Item"))
                    {
                        XmlElement Element = Node as XmlElement;
                        
                        if (Element!=null)
                        {
                            RowData row = new RowData();
                            row.ID = records[i].StudentID;
                            foreach (string field in e.ExportFields)
                            {
                                if (wizard.ExportableFields.Contains(field))
                                {
                                    switch (field)
                                    {
                                        case "學年度": row.Add(field, "" + records[i].SchoolYear); break;
                                        case "學期": row.Add(field, "" + records[i].Semester); break;
                                        case "社團名稱": row.Add(field, "" + Element.GetAttribute("AssociationName")); break;
                                        case "分數": row.Add(field, "" + Element.GetAttribute("Score")); break;
                                        case "努力程度": row.Add(field, "" + Element.GetAttribute("Effort")); break;
                                        case "文字描述": row.Add(field, "" + Element.GetAttribute("Text")); break;
                                    }
                                }
                            }
                            e.Items.Add(row);
                        }
                    }
                }
            };
        }
    }
}