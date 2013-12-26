using System.Collections.Generic;
using FISCA.UDT;
using JHSchool.Association.UDT;
using SmartSchool.API.PlugIn;

namespace JHSchool.Association.ImportExport 
{
    class 匯出表現資料 : SmartSchool.API.PlugIn.Export.Exporter
    {
        //建構子
        public 匯出表現資料()
        {
            this.Image = null;
            this.Text = "匯出表現資料";
        }

        //覆寫
        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("細項","類別","單位","學年度","學期");

            wizard.ExportPackage += (sender,e)=>
            {
                AccessHelper helper = new AccessHelper();

                string strCondition = string.Empty;

                foreach (string ID in e.List)
                    strCondition += strCondition == string.Empty ? ID : "," + ID;

                List<活動表現紀錄> records = helper.Select<活動表現紀錄>("學生編號 in (" + strCondition + ")");

                for (int i = 0; i < records.Count; i++)
                {
                    RowData row = new RowData();
                    row.ID = "" + records[i].學生編號;

                    foreach (string field in e.ExportFields)
                    {
                        if (wizard.ExportableFields.Contains(field))
                        {
                            switch (field)
                            {
                                case "細項": row.Add(field, "" + records[i].細項); break;
                                case "類別": row.Add(field, "" + records[i].類別); break;
                                case "單位": row.Add(field, "" + records[i].單位); break;
                                case "學年度": row.Add(field, "" + records[i].學年度); break;
                                case "學期": row.Add(field, "" + records[i].學期); break;
                            }
                        }
                    }
                    e.Items.Add(row);
                }
            };
        }
    }
}