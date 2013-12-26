using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using JHSchool.Data;
using FISCA.LogAgent;
using System.Xml;
using FISCA.DSAUtil;
using FISCA;

namespace JHSchool.Association
{
    public partial class SettleAccountsForm : BaseForm
    {
        private BackgroundWorker BGW = new BackgroundWorker();
        private string SchoolYear;
        private string Semester;

        EffortMapper EfforMapperDic = new EffortMapper();

        //UDT功能物件
        private AccessHelper _accessHelper = new AccessHelper();

        //資料處理
        private List<string> StudentList = new List<string>(); //修課學生ID集合
        //private List<AssnCode> InsertAssnCode = new List<AssnCode>(); //UDT新增清單
        //private List<AssnCode> UpdataAssnCode = new List<AssnCode>(); //UDT更新清單
        private Dictionary<string, AssnCode> UDTAssnUpdataList = new Dictionary<string, AssnCode>(); //已存在資料(Key為學生ID)

        bool CheckTrue = false;

        public SettleAccountsForm()
        {
            InitializeComponent();
        }

        private void SettleAccountsForm_Load(object sender, EventArgs e)
        {
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            #region 判斷資料內容
            foreach (JHCourseRecord each in JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource))
            {
                if (SchoolYear == null && Semester == null)
                {
                    SchoolYear = each.SchoolYear.Value.ToString();
                    Semester = each.Semester.Value.ToString();
                }
                else
                {
                    if (SchoolYear != each.SchoolYear.Value.ToString() || Semester != each.Semester.Value.ToString())
                    {
                        MsgBox.Show("社團之[學年度/學期]清單\n不可由多個[學年度/學期]社團組成");
                        this.Close();
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 開始結算
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnStart_Click(object sender, EventArgs e)
        {
            //關閉畫面
            btnStart.Enabled = false;
            FISCA.Presentation.MotherForm.SetStatusBarMessage("開始結算社團成績...");
            labelX1.Text = "(社團成績結算完成前,請勿中止或關閉本畫面!!)";
            labelX1.ForeColor = Color.Red;
            //開始背景模式
            BGW.RunWorkerAsync();
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region BGW_DoWork

            //更新CourseList && AssnList && StudentList資料

            CheckTrue = CourseIsAssnQu();

            if (CheckTrue)
            {
                return; //資料檢查未過
            }

            StudentList = GetStudentList();

            //ResetCourseAndStudent();

            //判斷是否有資料
            GetUDTUpdata();

            //新增修改資料處理
            InsertAndUpdata();

            List<AssnCode> InsertList = new List<AssnCode>();
            List<AssnCode> UpdataList = new List<AssnCode>();

            foreach (AssnCode each in UDTAssnUpdataList.Values)
            {
                if (each.RecordStatus == RecordStatus.Insert)
                {
                    InsertList.Add(each);
                }
                else
                {
                    UpdataList.Add(each);
                }
            }

            if (checkFormClose)
            {
                try
                {
                    _accessHelper.InsertValues(InsertList.ToArray());
                    _accessHelper.UpdateValues(UpdataList.ToArray());
                }
                catch (Exception ex)
                {
                    MsgBox.Show("資料處理過程發生錯誤..." + ex.Message);
                    btnStart.Enabled = true;
                    return;
                }

                //Log處理器
                SetLogInfo();
            }
            else
            {
                e.Cancel = true;
            }

            #endregion
        }

        private bool CourseIsAssnQu()
        {
            //取得課程
            List<JHCourseRecord> list = JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource);

            //取得課程評量樣版
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

            if (RefAssessmentSetupID == "")
            {
                MsgBox.Show("未設定評量樣板,請使用[社團評量設定]進行設定!");
                return true;
            }
            else //有社團評量
            {
                //是否有課程不是使用社團評量樣板
                foreach (JHCourseRecord each in list)
                {
                    if (each.RefAssessmentSetupID != RefAssessmentSetupID)
                    {
                        MsgBox.Show("部份社團不是使用社團評量,請指定後再使用本功能!");
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>
        /// 建立課程ID清單 & 修課學生清單
        /// </summary>
        //private void ResetCourseAndStudent()
        //{
        //    #region 建立課程ID清單 & 修課學生清單

        //    StudentList.Clear();

        //    StudentList = GetAssnStudentList();

        //    #endregion
        //}

        /// <summary>
        /// 取得需要計算的學生清單
        /// </summary>
        /// <returns></returns>
        private List<string> GetStudentList()
        {
            #region 取得需要計算的學生清單
            List<string> _Students = new List<string>();

            List<JHSCAttendRecord> _SCAList = JHSCAttend.SelectByCourseIDs(AssnAdmin.Instance.SelectedSource);

            foreach (JHSCAttendRecord each in _SCAList)
            {
                JHStudentRecord sr = each.Student;
                if (sr.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    if (!_Students.Contains(each.RefStudentID))
                    {
                        _Students.Add(each.RefStudentID);
                    }
                }
            }

            return _Students;
            #endregion
        }

        /// <summary>
        /// 取得UDT資料以判斷是否重覆
        /// </summary>
        private void GetUDTUpdata()
        {
            #region 取得UDT資料以判斷是否重覆

            UDTAssnUpdataList.Clear();

            //取得UDT資料
            List<AssnCode> UDTAssnList = _accessHelper.Select<AssnCode>(string.Format("SchoolYear='{0}' and Semester='{1}'", SchoolYear, Semester));

            //判斷是否有資料
            foreach (AssnCode each in UDTAssnList)
            {
                //過濾是否為選擇的學生
                if (StudentList.Contains(each.StudentID))
                {
                    if (!UDTAssnUpdataList.ContainsKey(each.StudentID))
                    {
                        UDTAssnUpdataList.Add(each.StudentID, each);
                    }
                }
            }
            #endregion
        }

        /// <summary>
        /// 新增修改資料處理
        /// </summary>
        private void InsertAndUpdata()
        {
            #region 新增修改資料處理

            //InsertAssnCode.Clear();
            //UpdataAssnCode.Clear();

            //取得課程+學生成績資料
            List<AssnScoreRecord> SCETakeList = JHSCETake.SelectByStudentAndCourse(StudentList, AssnAdmin.Instance.SelectedSource).AsKHJHSCETakeRecords();

            foreach (AssnScoreRecord each in SCETakeList)
            {
                //如果是已存在資料
                if (UDTAssnUpdataList.ContainsKey(each.RefStudentID))
                {

                    AssnCode bc = UDTAssnUpdataList[each.RefStudentID];

                    XmlElement Allxml = DSXmlHelper.LoadXml(bc.Scores);

                    bool UpdataXml = true;
                    foreach (XmlNode node in Allxml.SelectNodes("Item"))
                    {
                        XmlElement xml = (XmlElement)node;
                        if (JHCourse.SelectByID(each.RefCourseID).Name == xml.GetAttribute("AssociationName")) //如果有相同課程
                        {
                            DSXmlHelper ds = new DSXmlHelper(xml);
                            ds.SetAttribute(".", "AssociationName", JHCourse.SelectByID(each.RefCourseID).Name);
                            ds.SetAttribute(".", "Score", each.Score.HasValue ? each.Score.Value.ToString() : "");
                            ds.SetAttribute(".", "Effort", each.Effort.HasValue ? EfforMapperDic.GetTextByCode(each.Effort.Value) : "");
                            ds.SetAttribute(".", "Text", each.Text);
                            bc.Scores = Allxml.OuterXml;
                            UpdataXml = false;
                        }
                    }

                    if (UpdataXml)
                    {
                        //課程結算式
                        DSXmlHelper ds = new DSXmlHelper(Allxml);
                        ds.AddElement("Item");
                        ds.SetAttribute("Item", "AssociationName", JHCourse.SelectByID(each.RefCourseID).Name);
                        ds.SetAttribute("Item", "Score", each.Score.HasValue ? each.Score.Value.ToString() : "");
                        ds.SetAttribute("Item", "Effort", each.Effort.HasValue ? EfforMapperDic.GetTextByCode(each.Effort.Value) : "");
                        ds.SetAttribute("Item", "Text", each.Text);
                        bc.Scores = ds.BaseElement.OuterXml;
                    }
                }
                else //不存在 ,更新資料
                {
                    AssnCode ac = new AssnCode();
                    ac.StudentID = each.RefStudentID; //學生ID
                    ac.SchoolYear = SchoolYear; //學年度
                    ac.Semester = Semester; //學期

                    //課程結算式
                    DSXmlHelper ds = new DSXmlHelper("Content");
                    ds.AddElement("Item");
                    ds.SetAttribute("Item", "AssociationName", JHCourse.SelectByID(each.RefCourseID).Name);
                    ds.SetAttribute("Item", "Score", each.Score.HasValue ? each.Score.Value.ToString() : "");
                    ds.SetAttribute("Item", "Effort", each.Effort.HasValue ? EfforMapperDic.GetTextByCode(each.Effort.Value) : "");
                    ds.SetAttribute("Item", "Text", each.Text);
                    ac.Scores = ds.BaseElement.OuterXml;
                    UDTAssnUpdataList.Add(each.RefStudentID, ac);
                }
            }
            #endregion
        }

        /// <summary>
        /// 呼叫Log處理器
        /// </summary>
        private void SetLogInfo()
        {
            #region Log資料處理

            List<string> LogString = new List<string>();

            foreach (AssnCode each in UDTAssnUpdataList.Values)
            {
                LogString.Add(each.StudentID);
            }

            List<JHStudentRecord> LogRecord = JHStudent.SelectByIDs(LogString);

            LogRecord.Sort(new Comparison<JHStudentRecord>(SortCompareTo.ParseStudent));

            if (UDTAssnUpdataList.Count != 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("社團成績結算清單：");

                foreach (JHStudentRecord each in LogRecord)
                {
                    string st = each.Class != null ? each.Class.Name : "";
                    sb.Append("班級：「" + st + "」");
                    sb.Append("座號：「" + each.SeatNo + "」");
                    sb.AppendLine("姓名：「" + each.Name + "」");
                }

                ApplicationLog.Log("社團外掛模組", "社團成績結算", sb.ToString());
            }
            #endregion
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                if (e.Error != null)
                {
                    MsgBox.Show("社團成績結算失敗!\n" + e.Error.Message);
                    FISCA.Presentation.MotherForm.SetStatusBarMessage("社團成績結算失敗!");
                    SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
                    RTOut.WriteError(e.Error);
                }
                else
                {
                    if (CheckTrue)
                    {
                        MsgBox.Show("社團成績結算失敗!\n社團成績評量或社團樣版有誤!");
                        FISCA.Presentation.MotherForm.SetStatusBarMessage("社團成績評量或社團樣版有誤!");
                    }
                    else
                    {
                        btnStart.Enabled = true;
                        MsgBox.Show("社團成績結算成功!");
                        FISCA.Presentation.MotherForm.SetStatusBarMessage("社團成績結算成功!");
                        this.Close();
                    }
                }
            }
            else
            {
                MsgBox.Show("由於畫面關閉\n社團成績結算已中止!!");
                FISCA.Presentation.MotherForm.SetStatusBarMessage("由於畫面關閉,社團成績結算已中止!!");
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        bool checkFormClose = true;

        /// <summary>
        /// 畫面關閉
        /// </summary>
        private void SettleAccountsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            checkFormClose = false;
        }

        /// <summary>
        /// 畫面關閉前
        /// </summary>
        private void SettleAccountsForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (BGW.IsBusy)
            {
                DialogResult dr = MsgBox.Show("系統正在進行[成績結算]中\n如果關閉畫面[社團成績]仍會繼續結算\n您確認要關閉畫面??", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr != System.Windows.Forms.DialogResult.Yes)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}