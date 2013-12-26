using System.Collections.Generic;
using FISCA.UDT;
using JHSchool.Association.UDT;
using SmartSchool.API.PlugIn;

namespace JHSchool.Association.ImportExport
{
    class 匯出對照表 : SmartSchool.API.PlugIn.Export.Exporter
    {
        //建構子
        public 匯出對照表()
        {
            this.Image = null;
            this.Text = "匯出對照表";
        }

        //覆寫
        public override void InitializeExport(SmartSchool.API.PlugIn.Export.ExportWizard wizard)
        {
            wizard.ExportableFields.AddRange("細項", "類別", "數量");

            wizard.ExportPackage += (sender, e) =>
            {
                AccessHelper helper = new AccessHelper();

                string strCondition = string.Empty;

                List<活動表現紀錄對照表> records = helper.Select<活動表現紀錄對照表>();

                for (int i = 0; i < records.Count; i++)
                {
                    RowData row = new RowData();

                    foreach (string field in e.ExportFields)
                    {
                        if (wizard.ExportableFields.Contains(field))
                        {
                            switch (field)
                            {
                                case "細項": row.Add(field, "" + records[i].細項); break;
                                case "類別": row.Add(field, "" + records[i].類別); break;
                                case "數量": row.Add(field, "" + records[i].數量); break;
                            }
                        }
                    }
                    e.Items.Add(row);
                }
            };
        }
    }
}