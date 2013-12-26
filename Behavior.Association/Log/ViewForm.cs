using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.LogAgent.AccessLayer;
using FISCA.LogAgent;
using System.Xml;

namespace JHSchool.Association
{
    public partial class ViewForm : BaseForm
    {

        private const string TimeDisplayFormat = "yyyy/MM/dd HH:mm";

        private BackgroundWorker BGW = new BackgroundWorker();

        ActionRecordCollection ActionColl = new ActionRecordCollection();


        FiscaAccessLayer fl = new FiscaAccessLayer();

        //模式
        string StartString = "1";

        //開始時間
        DateTime time1 = new DateTime();
        //結束時間
        DateTime time2 = new DateTime();
        //關鍵字
        string StringTextBoxX1;

        //先進先出
        Queue<string> timerList2 = new Queue<string>();

        string _GTarget;
        List<string> _TargetIDList = new List<string>();

        /// <summary>
        /// 總體瀏覽模式
        /// </summary>
        public ViewForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 依照學生/班級/教師/課程,的瀏覽模式
        /// </summary>
        /// <param name="GTarget">傳入模式</param>
        /// <param name="TargetID">傳入ID</param>
        public ViewForm(string GTarget, List<string> TargetIDList)
        {
            InitializeComponent();

            _GTarget = GTarget;
            _TargetIDList = TargetIDList;
            guoupPanel2();
        }

        /// <summary>
        /// 使用者瀏覽模式
        /// </summary>
        /// <param name="GTarget">傳入使用者帳號</param>
        public ViewForm(string UserName)
        {
            InitializeComponent();
            textBoxX1.Text = UserName;
            checkBoxX4.Checked = true;
            guoupPanel2();
        }

        private void guoupPanel2()
        {
            checkBoxX1.Enabled = false;
            checkBoxX2.Enabled = false;
            checkBoxX3.Enabled = false;
            checkBoxX4.Enabled = false;
            checkBoxX5.Enabled = false;
            checkBoxX6.Enabled = false;
            labelX3.Enabled = false;
            textBoxX1.Enabled = false;
            groupPanel2.Text = "步驟2選擇篩選條件(本模式,無法使用篩選條件)";
        }

        private void ViewForm_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            DateTime dt1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateTime dt2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            dateTimeInput1.Value = dt1.AddDays(-7);
            dateTimeInput2.Value = dt2;

            timerString();
            Random r = new Random();
            for (int i = 0; i < r.Next(timerList2.Count) + 1; i++)
                UpdateTip();

            btnReF_Click(null, null);
        }

        private void btnReF_Click(object sender, EventArgs e)
        {
            RunBGW();

            if (string.IsNullOrEmpty(textBoxX1.Text) && !checkBoxX1.Checked)
            {
                MsgBox.Show("除日期條件不使用關鍵字\nIP篩選,電腦名稱篩選,登入帳號篩選,動作篩選,描述篩選\n皆需輸入篩選關鍵字。");
                onLock(true);
                return;
            }

            if (!string.IsNullOrEmpty(_GTarget) && _TargetIDList.Count != 0) //依類別+ID
            {
                StartString = "7";
                BGW.RunWorkerAsync();
            }
            else if (checkBoxX2.Checked) //依IP位置
            {
                StartString = "2";
                BGW.RunWorkerAsync();
            }
            else if (checkBoxX3.Checked) //依電腦名稱
            {
                StartString = "3";
                BGW.RunWorkerAsync();
            }
            else if (checkBoxX4.Checked) //依登入帳號
            {
                StartString = "4";
                BGW.RunWorkerAsync();
            }
            else if (checkBoxX5.Checked) //依動作
            {
                StartString = "5";
                BGW.RunWorkerAsync();
            }
            else if (checkBoxX6.Checked) //依描述
            {
                StartString = "6";
                BGW.RunWorkerAsync();
            }
            else //(預設)依日期
            {
                StartString = "1";
                BGW.RunWorkerAsync();
            }
        }

        private void RunBGW()
        {
            onLock(false);
            this.Text = "查詢資料中...";
            time1 = dateTimeInput1.Value;
            time2 = dateTimeInput2.Value.AddDays(1).AddSeconds(-1);
            StringTextBoxX1 = textBoxX1.Text;
            dataGridViewX1.Rows.Clear();
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            if (StartString == "4") //使用登入帳號條件
            {
                ActionColl = fl.GetActorLog(time1, time2, StringTextBoxX1);
            }
            else if (StartString == "5") //使用動作條件
            {
                ActionColl = fl.GetActionByLog(time1, time2, StringTextBoxX1);
            }
            else //1,2,3,6,7都是用全部資料再進行篩選
            {
                ActionColl = fl.GetLog(time1, time2);
            }
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.Text = "系統歷程總覽 - 日誌記錄瀏覽器";
            onLock(true);

            if (StartString == "1")
            {
                foreach (ActionRecord ar in ActionColl)
                {
                    dataGridViewX1.Rows.Add(ValueInser(ar));
                }
            }
            else if (StartString == "2")
            {
                foreach (ActionRecord ar in ActionColl)
                {
                    XmlElement xml = ar.ClientInfo.OutputResult();
                    foreach (XmlElement each in xml.SelectNodes("NetworkAdapterList/NetworkAdapter"))
                    {
                        //如果IP相同
                        if (each.SelectSingleNode("IPAddress").InnerText == textBoxX1.Text)
                        {
                            dataGridViewX1.Rows.Add(ValueInser(ar));
                            continue;
                        }
                    }
                }
            }
            else if (StartString == "3")
            {
                foreach (ActionRecord ar in ActionColl)
                {
                    if (ar.ClientInfo.HostName.Contains(textBoxX1.Text))
                    {
                        dataGridViewX1.Rows.Add(ValueInser(ar));
                    }
                }
            }
            else if (StartString == "4")
            {
                foreach (ActionRecord ar in ActionColl)
                {
                    dataGridViewX1.Rows.Add(ValueInser(ar));
                }
            }
            else if (StartString == "5")
            {
                foreach (ActionRecord ar in ActionColl)
                {
                    dataGridViewX1.Rows.Add(ValueInser(ar));
                }
            }
            else if (StartString == "6")
            {
                foreach (ActionRecord ar in ActionColl)
                {
                    if (ar.Description.Contains(textBoxX1.Text))
                    {
                        dataGridViewX1.Rows.Add(ValueInser(ar));
                    }
                }
            }
            else if (StartString == "7")
            {
                foreach (ActionRecord ar in ActionColl)
                {
                    if (_TargetIDList.Contains(ar.TargetID) && ar.TargetCategory == _GTarget)
                    {
                        dataGridViewX1.Rows.Add(ValueInser(ar));
                    }
                }
            }

            dataGridViewX1.Sort(Column1, ListSortDirection.Descending);
        }

        //傳入Recrod,將該記錄建立為DataGridViewRow
        private DataGridViewRow ValueInser(ActionRecord ar)
        {
            DataGridViewRow row = new DataGridViewRow();
            row.CreateCells(dataGridViewX1);
            row.Tag = ar;
            //時間
            row.Cells[0].Value = DateTime.Parse(ar.ServerTime).ToString(TimeDisplayFormat);
            //IP          
            XmlElement xml = ar.ClientInfo.OutputResult();
            List<string> list = new List<string>();
            foreach (XmlElement each in xml.SelectNodes("NetworkAdapterList/NetworkAdapter"))
            {
                if (each.SelectSingleNode("IPAddress").InnerText != "127.0.0.1")
                {
                    list.Add(each.SelectSingleNode("IPAddress").InnerText);
                }
            }
            row.Cells[1].Value = string.Join("；", list.ToArray());

            //電腦名稱
            row.Cells[2].Value = ar.ClientInfo.HostName;
            //登入帳號
            row.Cells[3].Value = ar.Actor;
            //動作
            row.Cells[4].Value = ar.ActionBy;
            //描述
            row.Cells[5].Value = ar.Description;

            return row;
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            #region 匯出
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;

            DataGridViewExport export = new DataGridViewExport(dataGridViewX1);
            export.Save(saveFileDialog1.FileName);

            if (new CompleteForm().ShowDialog() == DialogResult.Yes)
                System.Diagnostics.Process.Start(saveFileDialog1.FileName);
            #endregion
        }

        private void onLock(bool ol)
        {
            dateTimeInput1.Enabled = ol;
            dateTimeInput2.Enabled = ol;
            groupPanel2.Enabled = ol;
            buttonX2.Enabled = ol;
            btnReF.Enabled = ol;
        }

        private void dataGridViewX1_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex == Column5.Index && e.RowIndex != -1)
            {
                TextBoxForm vf = new TextBoxForm(dataGridViewX1.Rows[e.RowIndex].Tag as ActionRecord);
                vf.ShowDialog();
            }
        }

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBoxX1.Checked)
            {
                textBoxX1.Enabled = false;
                textBoxX1.Text = "";
                btnReF.Pulse(5);
            }
            else
            {
                textBoxX1.Enabled = true;
            }
        }

        private void textBoxX1_TextChanged(object sender, EventArgs e)
        {
            btnReF.Pulse(5);
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void timerString()
        {
            timerList2.Enqueue("提示：使用滑鼠左鍵,連點描述欄位,可開啟詳細說明");
            timerList2.Enqueue("提示：開始日期/結束日期 輸入8/15將會自動轉換為2010/8/15");
            timerList2.Enqueue("提示：[登入帳號/電腦名稱]篩選條件 英文字母 大/小寫有所差異");
            timerList2.Enqueue("提示：[動作]篩選條件 請務必輸入完整動作名稱");
            timerList2.Enqueue("提示：[描述]篩選條件 只需要輸入包含的文字即可,如姓名/學號");
            timerList2.Enqueue("提示：如果日期區間定義過大,資料量過多,需要較多處理時間!");
            timerList2.Enqueue("提示：外掛撰寫者,如未使用Log機制寫入Log,將不會有相關記錄");
        }

        private void UpdateTip()
        {
            string msg = timerList2.Dequeue();
            labelX4.Text = msg;
            timerList2.Enqueue(msg);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateTip();
        }
    }
}
