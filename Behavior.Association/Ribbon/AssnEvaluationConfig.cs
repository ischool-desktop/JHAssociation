using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using JHSchool.Data;
using FISCA.LogAgent;

namespace JHSchool.Association
{
    public partial class AssnEvaluationConfig : BaseForm
    {
        BackgroundWorker BGW1 = new BackgroundWorker();
        BackgroundWorker BGW2 = new BackgroundWorker();
        JHAEIncludeRecord newAEInc = new JHAEIncludeRecord();

        string AssID = "";
        string ExID = "";

        List<JHAEIncludeRecord> AEIncList = new List<JHAEIncludeRecord>();

        public AssnEvaluationConfig()
        {
            InitializeComponent();
        }

        private void AssnEvaluationConfig_Load(object sender, EventArgs e)
        {
            BGW1.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            BGW2.DoWork += new DoWorkEventHandler(BGW2_DoWork);
            BGW2.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW2_RunWorkerCompleted);

            this.Text = "讀取資料中...";
            this.Enabled = false;
            BGW1.RunWorkerAsync();
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 背景模式1

            List<JHAssessmentSetupRecord> AssSetupList = JHAssessmentSetup.SelectAll();
            string RefAssessmentSetupID = "";
            foreach (JHAssessmentSetupRecord each in AssSetupList)
            {
                if (each.Name == "社團評量(社團模組)")
                {
                    RefAssessmentSetupID = each.ID;
                    break;
                }
            }

            if (RefAssessmentSetupID != "")
            {
                List<JHAEIncludeRecord> AEIncList = JHAEInclude.SelectAll();
                foreach (JHAEIncludeRecord each in AEIncList)
                {
                    if (each.RefAssessmentSetupID == RefAssessmentSetupID && each.ExamName == "社團評量")
                    {
                        newAEInc = each;
                        break;
                    }
                }
            }
            #endregion
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region 背景模式1完成(預設畫面)

            if (newAEInc.ID != null)
            {
                cbxPercentage.Checked = newAEInc.UseScore;
                cbxDegree.Checked = newAEInc.UseEffort;
                cbxDescribe.Checked = newAEInc.UseText;

                if (newAEInc.StartTime != "")
                {
                    dtiStart.Text = newAEInc.StartTime.Remove(newAEInc.StartTime.IndexOf(' '));
                }

                if (newAEInc.EndTime != "")             
                {
                    dtiEnd.Text = newAEInc.EndTime.Remove(newAEInc.EndTime.IndexOf(' '));
                }
            }
            else
            {
                cbxPercentage.Checked = false;
                cbxDegree.Checked = false;
                cbxDescribe.Checked = false;
            }

            this.Text = "社團評量設定";
            this.Enabled = true;
            #endregion
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            this.Enabled = false;
            this.Text = "儲存資料...";
            BGW2.RunWorkerAsync();
        }

        void BGW2_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 背景模式2
            AssID = ChengAssessment();
            ExID = ChengExam();

            AEIncList.Clear();
            AEIncList = JHAEInclude.SelectAll(); 
            #endregion
        }

        void BGW2_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region 背景模式2完成(儲存)
            try
            {
                ChengAEInclude(AssID, ExID);
            }
            catch (Exception ex)
            {
                this.Enabled = true;
                MsgBox.Show("儲存失敗,請檢查內容!" + ex.Message);
                return;
            }

            this.Enabled = true;
            this.Text = "社團評量設定(已儲存)";
            this.Close();
            #endregion
        }

        private string ChengAssessment()
        {
            #region 評量管理
            string AssID = "";
            List<JHAssessmentSetupRecord> AssesList = JHAssessmentSetup.SelectAll();
            JHAssessmentSetupRecord newAssess = new JHAssessmentSetupRecord();
            foreach (JHAssessmentSetupRecord each in AssesList)
            {
                if (each.Name == "社團評量(社團模組)")
                {
                    newAssess = each; //已經存在
                    AssID = each.ID;
                    break;
                }
            }

            if (newAssess.ID == "") //不存在就New
            {
                newAssess.Name = "社團評量(社團模組)";
                AssID = JHAssessmentSetup.Insert(newAssess);

                ApplicationLog.Log("社團外掛模組", "新增社團專用評量樣版", "新增：「社團外掛模組」專用評量樣版\n樣版名稱：「" + newAssess.Name + "」");
            }

            return AssID;
            #endregion
        }

        private string ChengExam()
        {
            #region 評量試別管理
            string ExID = "";
            List<JHExamRecord> ExamList = JHExam.SelectAll();
            JHExamRecord newExam = new JHExamRecord();
            foreach (JHExamRecord each in ExamList)
            {
                if (each.Name == "社團評量")
                {
                    newExam = each;
                    ExID = each.ID;
                    break;

                }
            }
            if (newExam.ID == "")
            {
                newExam.Name = "社團評量";
                newExam.DisplayOrder = 10;
                ExID = JHExam.Insert(newExam);
                ApplicationLog.Log("社團外掛模組", "新增社團專用評量試別", "新增：「社團外掛模組」專用評量試別\n試別名稱：「" + newExam.Name + "」");
                //newExam.Description
            }
            return ExID;
            #endregion
        }

        private void ChengAEInclude(string AssID, string ExID)
        {
            #region 評量內容管理
            JHAEIncludeRecord newAEInc = new JHAEIncludeRecord();
            foreach (JHAEIncludeRecord each in AEIncList)
            {
                if (each.AssessmentSetup.Name == "社團評量(社團模組)" && each.ExamName == "社團評量")
                {
                    newAEInc = each;
                    break;
                }
            }

            if (newAEInc.ID == null) //新增
            {
                #region 新增
                newAEInc.RefAssessmentSetupID = AssID;
                newAEInc.RefExamID = ExID;
                newAEInc.Weight = 100;
                newAEInc.UseScore = cbxPercentage.Checked;
                newAEInc.UseEffort = cbxDegree.Checked;
                newAEInc.UseText = cbxDescribe.Checked;
                if (dtiStart.Text != "")
                {
                    newAEInc.StartTime = dtiStart.Value.ToString("yyyy/MM/dd HH:mm");
                }
                if (dtiEnd.Text != "")
                {
                    newAEInc.EndTime = dtiEnd.Value.AddDays(1).AddMilliseconds(-1).ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    newAEInc.EndTime = "";
                }

                //
                //努力程度,文字評量,成績,輸入時間,結束時間
                //參考社團之設定畫面

                JHAEInclude.Insert(newAEInc);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("評量名稱：「社團評量(社團模組)」");
                sb.AppendLine("成績：「" + newAEInc.UseScore + "」");
                sb.AppendLine("努力程度：「" + newAEInc.UseEffort + "」");
                sb.AppendLine("文字描述：「" + newAEInc.UseText + "」");
                sb.AppendLine("評量輸入日期：");
                sb.AppendLine("開始「" + newAEInc.StartTime + "」");
                sb.AppendLine("結束「" + newAEInc.EndTime + "」");

                ApplicationLog.Log("社團外掛模組", "新增社團模組評量設定", sb.ToString()); 
                #endregion
            }
            else //更新
            {
                #region 更新
                newAEInc.UseScore = cbxPercentage.Checked;
                newAEInc.UseEffort = cbxDegree.Checked;
                newAEInc.UseText = cbxDescribe.Checked;
                if (dtiStart.Text != "")
                {
                    //年yyyy/月MM/日dd/時HH/分mm/秒ss
                    newAEInc.StartTime = dtiStart.Value.ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    newAEInc.StartTime = "";
                }
                if (dtiEnd.Text != "")
                {
                    newAEInc.EndTime = dtiEnd.Value.AddDays(1).AddMilliseconds(-1).ToString("yyyy/MM/dd HH:mm");
                }
                else
                {
                    newAEInc.EndTime = "";
                }
                JHAEInclude.Update(newAEInc);

                StringBuilder sb = new StringBuilder();
                sb.AppendLine("評量名稱：「社團評量(社團模組)」");
                sb.AppendLine("成績：「" + newAEInc.UseScore + "」");
                sb.AppendLine("努力程度：「" + newAEInc.UseEffort + "」");
                sb.AppendLine("文字描述：「" + newAEInc.UseText + "」");
                sb.AppendLine("評量輸入日期：");
                sb.AppendLine("開始「" + newAEInc.StartTime + "」");
                sb.AppendLine("結束「" + newAEInc.EndTime + "」");

                ApplicationLog.Log("社團外掛模組", "更新社團模組評量設定", sb.ToString()); 
                #endregion
            }
            #endregion
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
