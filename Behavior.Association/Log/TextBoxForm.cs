using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.LogAgent;
using System.IO;
using System.Xml;

namespace JHSchool.Association
{
    public partial class TextBoxForm : BaseForm
    {
        private const string TimeDisplayFormat = "yyyy/MM/dd HH:mm";

        public TextBoxForm(ActionRecord tx)
        {
            InitializeComponent();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("========詳細內容========");
            sb.AppendLine(tx.Description);
            sb.AppendLine("");
            sb.AppendLine("");
            sb.AppendLine("======其他參考資訊======");
            sb.AppendLine("Log記錄編號：" + tx.ID);
            sb.AppendLine("日期時間：" + DateTime.Parse(tx.ServerTime).ToString(TimeDisplayFormat));
            sb.AppendLine("登入帳號：" + tx.Actor);
            sb.AppendLine("電腦名稱：" + tx.ClientInfo.HostName);
            sb.AppendLine("動作內容：" + tx.ActionBy);
            sb.AppendLine("物件分類：" + tx.TargetCategory);
            sb.AppendLine("物件系統編號：" + tx.TargetID);
            sb.AppendLine("動作類型：" + tx.ActionType);
            sb.AppendLine("動作分類：" + tx.Action);
            //sb.AppendLine("");
            //sb.AppendLine("「ClientInfo - XML內容」");
            //sb.AppendLine(Format(tx.ClientInfo.OutputResult().OuterXml));

            textBoxX1.Text = sb.ToString().Replace("\n", "\r\n");
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string Format(string xmlContent)
        {
            MemoryStream ms = new MemoryStream();

            XmlTextWriter writer = new XmlTextWriter(ms, System.Text.Encoding.UTF8);

            writer.Formatting = Formatting.Indented;
            writer.Indentation = 2;
            writer.IndentChar = ' ';

            XmlReader Reader = GetXmlReader(xmlContent);
            writer.WriteNode(Reader, true);
            writer.Flush();
            Reader.Close();

            ms.Position = 0;
            StreamReader sr = new StreamReader(ms, System.Text.Encoding.UTF8);

            string Result = sr.ReadToEnd();
            sr.Close();

            writer.Close();
            ms.Close();

            return Result;
        }

        private XmlReader GetXmlReader(string XmlData)
        {
            XmlReaderSettings setting = new XmlReaderSettings();
            setting.IgnoreWhitespace = true;

            XmlReader Reader = XmlReader.Create(new StringReader(XmlData), setting);

            return Reader;
        }
    }
}
