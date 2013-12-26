using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Data;
using DevComponents.DotNetBar;
using FISCA.Presentation.Controls;
using FCode = Framework.Security.FeatureCodeAttribute;
using Framework;
using FISCA.LogAgent;

namespace JHSchool.Association
{
    [FCode("JHSchool.Association.Detail0020", "社員清單")]
    public partial class StudentDetailItem : DetailContentBase
    {
        private BackgroundWorker BgW = new BackgroundWorker();
        private bool BkWBool = false;
        private SCAItem sc; //課程學生修課資料
        private List<string> ReMoveTemp = new List<string>(); //已加入的清單

        internal static Framework.Security.FeatureAce UserPermission;

        public StudentDetailItem()
        {
            InitializeComponent();

            Group = "社員清單";
            UserPermission = User.Acl[FCode.GetCode(GetType())];

            this.Enabled = UserPermission.Editable;

            //如果課程資料更新了
            //JHCourse.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHCourse_AfterChange);

            //社團更新鈕的事件
            AssnEvents.AssnChanged += new EventHandler(AssnEvents_AssnChanged);

            //如果修課資料更新了
            JHSCAttend.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHSCAttend_AfterUpdate);
            JHSCAttend.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHSCAttend_AfterUpdate);
            JHSCAttend.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHSCAttend_AfterUpdate);

            K12.Presentation.NLDPanels.Student.TempSourceChanged += new EventHandler(Student_TempSourceChanged);

            //背景模式
            BgW.DoWork += new DoWorkEventHandler(BkW_DoWork);
            BgW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkW_RunWorkerCompleted);
        }

        void AssnEvents_AssnChanged(object sender, EventArgs e)
        {
            if (this.PrimaryKey != "")
            {
                this.Loading = true;

                if (BgW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    BgW.RunWorkerAsync();
                }
            }
        }

        void JHSCAttend_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            #region PrimaryKey更新

            if (InvokeRequired)
            {
                Invoke(new Action<object, K12.Data.DataChangedEventArgs>(JHSCAttend_AfterUpdate), sender, e);
            }
            else
            {
                if (this.PrimaryKey != "")
                {
                    if (BgW.IsBusy)
                    {
                        BkWBool = true;
                    }
                    else
                    {
                        BgW.RunWorkerAsync();
                    }
                }
            }

            #endregion
        }

        //void JHCourse_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        //{
        //    #region 資料更新事件
        //    List<JHCourseTagRecord> crList = JHCourseTag.SelectByCourseIDs(e.PrimaryKeys);
        //    foreach (JHCourseTagRecord each in crList)
        //    {
        //        if (each.Name == "社團" || each.Name == "聯課活動")
        //        {
        //            if (!BgW.IsBusy)
        //            {
        //                BgW.RunWorkerAsync();
        //            }

        //            break;
        //        }
        //    }
        //    #endregion
        //}

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            #region PrimaryKey更新
            if (this.PrimaryKey != "")
            {
                this.Loading = true;

                if (BgW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    BgW.RunWorkerAsync();
                }
            }
            #endregion
        }

        JHCourseRecord jhCR;

        void BkW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 背景模式
            jhCR = JHCourse.SelectByID(this.PrimaryKey);
            if (jhCR != null)
            {
                sc = new SCAItem(this.PrimaryKey);
            }

            //JHStudent.RemoveAll();
            //JHStudent.SelectAll();
            #endregion
        }

        void BkW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            #region 背景模式完成
            if (BkWBool)
            {
                BkWBool = false;
                BgW.RunWorkerAsync();
                return;
            }

            BindData();

            this.Loading = false;
            #endregion
        }

        private void BindData()
        {
            if (jhCR == null)
                return;

            #region 更新畫面資料

            lvStudents.Items.Clear();
            foreach (JHStudentRecord each in sc.Students)
            {
                lvStudents.Items.Add(SetListView(each));
            }

            CreateStudentMenuItem();

            lbCourseCount.Text = "社團社員人數：" + sc.Students.Count.ToString();

            btnInserStudent.Text = "由待處理加入社員(" + K12.Presentation.NLDPanels.Student.TempSource.Count.ToString() + ")";

            #endregion
        }

        private ListViewItem SetListView(JHStudentRecord STUD)
        {
            #region 依學生建立ListView
            string ClassName = "";

            if (STUD.Class != null)
            {
                ClassName = STUD.Class.Name;
            }

            ListViewItem item = new ListViewItem(ClassName);

            if (STUD.SeatNo.HasValue)
            {
                item.SubItems.Add(STUD.SeatNo.Value.ToString());
            }
            else
            {
                item.SubItems.Add("");
            }

            item.SubItems.Add(STUD.Name);
            item.SubItems.Add(STUD.Gender);
            item.SubItems.Add(STUD.StudentNumber);

            item.Tag = sc.SCADic[STUD.ID];

            return item;
            #endregion
        }

        void Student_TempSourceChanged(object sender, EventArgs e)
        {
            #region 待處理內容更新

            btnInserStudent.Text = "由待處理加入社員(" + K12.Presentation.NLDPanels.Student.TempSource.Count.ToString() + ")";

            CreateStudentMenuItem();

            #endregion
        }

        private void btnInserStudent_Click(object sender, EventArgs e)
        {
            #region 將全部待處理學生加入此課程

            AddListViewInTemp(K12.Presentation.NLDPanels.Student.TempSource); //將待處理學生加入修課清單

            #endregion
        }

        private void btnInserStudent_PopupOpen(object sender, EventArgs e)
        {
            #region 開啟待處理按扭清單

            CreateStudentMenuItem(); //建立待處理學生在按鈕內

            #endregion
        }

        private void CreateStudentMenuItem()
        {
            #region 依待處理AddButton按鈕
            btnInserStudent.SubItems.Clear();

            if (K12.Presentation.NLDPanels.Student.TempSource.Count == 0)
            {
                LabelItem item = new LabelItem("No", "沒有任何學生在待處理");
                btnInserStudent.SubItems.Add(item);
                return;
            }

            List<JHStudentRecord> StudentList = JHStudent.SelectByIDs(K12.Presentation.NLDPanels.Student.TempSource);

            foreach (JHStudentRecord each in StudentList)
            {
                if (each.Class != null)
                {
                    ButtonItem item = new ButtonItem(each.ID, each.Name + " (" + each.Class.Name + ")");
                    item.Tag = each;
                    item.Click += new EventHandler(AttendStudent_Click);
                    btnInserStudent.SubItems.Add(item);
                }
                else
                {
                    ButtonItem item = new ButtonItem(each.ID, each.Name);
                    item.Tag = each;
                    item.Click += new EventHandler(AttendStudent_Click);
                    btnInserStudent.SubItems.Add(item);
                }

            }
            #endregion
        }

        private void AttendStudent_Click(object sender, EventArgs e)
        {
            #region 從按鈕加入
            ButtonItem each = (ButtonItem)sender;
            JHStudentRecord eachTag = (JHStudentRecord)each.Tag;
            List<string> list = new List<string>();
            list.Add(eachTag.ID);
            AddListViewInTemp(list);

            #endregion
        }

        private void SyncStudentMenuItemStatus()
        {
            #region 已在清單內的學生,則把按鈕關閉
            Dictionary<string, ButtonItem> _students = new Dictionary<string, ButtonItem>();
            foreach (object obj in btnInserStudent.SubItems)
            {
                ButtonItem each = obj as ButtonItem;
                if (each == null) continue;
                JHStudentRecord stu = each.Tag as JHStudentRecord;
            }

            //foreach (ListViewItem each in lvStudents.Items)
            //{
            //    if (each.Tag as JHSCAttendRecord)
            //    {

            //    }

            //    if (_students.ContainsKey(info.RefStudentID.ToString()))
            //    {
            //        _students[info.RefStudentID.ToString()].Enabled = false;
            //        _students[info.RefStudentID.ToString()].Tooltip = "此學生已在修課清單中";
            //    }
            //} 
            #endregion
        }

        private void AddListViewInTemp(List<string> StudListID)
        {
            #region 將傳入的學生ID,加入此課程

            ReMoveTemp.Clear();

            List<string> IsSaft = CheckSaftStudent(StudListID);

            if (IsSaft.Count != 0)
            {
                List<string> InsertList = CheckTempStudentInCourse(IsSaft); //排除已存在學生

                if (InsertList.Count != 0)
                {
                    List<JHSCAttendRecord> list = new List<JHSCAttendRecord>();
                    foreach (string each in InsertList)
                    {
                        JHSCAttendRecord JHs = new JHSCAttendRecord();
                        JHs.RefStudentID = each; //修課學生
                        JHs.RefCourseID = this.PrimaryKey;
                        list.Add(JHs);
                    }

                    JHSCAttend.Insert(list);
                    JHCourseRecord cr = JHCourse.SelectByID(this.PrimaryKey);
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("社團名稱：「" + cr.Name + "」");
                    sb.AppendLine("新增社員清單：");
                    foreach (JHSCAttendRecord each in list)
                    {
                        if (each.Student.Class != null)
                        {
                            sb.Append("班級：「" + each.Student.Class.Name + "」");
                        }
                        else
                        {
                            sb.Append("班級：「」");
                        }
                        sb.Append("座號：「" + each.Student.SeatNo + "」");
                        sb.Append("姓名：「" + each.Student.Name + "」\n");
                    }

                    ApplicationLog.Log("社團外掛模組", "新增社團修課學生", sb.ToString());

                    //移出待處理
                    StringBuilder sbHelp = new StringBuilder();
                    sbHelp.AppendLine("已由待處理加入社員\n共 " + list.Count.ToString() + " 名學生\n");
                    sbHelp.AppendLine("是否將其移出待處理?");
                    DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show(sbHelp.ToString(), MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button1);
                    if (dr == DialogResult.Yes)
                    {
                        K12.Presentation.NLDPanels.Student.RemoveFromTemp(InsertList);
                    }

                    sc.Reset();
                }
                
                if(ReMoveTemp.Count != 0)
                {
                    FISCA.Presentation.Controls.MsgBox.Show(ReMoveTemp.Count + "名學生,重覆加入社團!!", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                FISCA.Presentation.Controls.MsgBox.Show("請檢查\n1.待處理無學生\n2.學生狀態有誤(非一般生)");
            }

            #endregion
        }

        /// <summary>
        /// 傳入學生ID,判斷是否為一般學生
        /// </summary>
        /// <param name="StudListID"></param>
        /// <returns></returns>
        private List<string> CheckSaftStudent(List<string> StudListID)
        {
            List<JHStudentRecord> IsDeleteStudent = JHStudent.SelectByIDs(StudListID);
            List<string> IsSaft = new List<string>();
            foreach (JHStudentRecord each in IsDeleteStudent)
            {
                if (each.Status == K12.Data.StudentRecord.StudentStatus.一般)
                {
                    IsSaft.Add(each.ID);
                }
            }

            return IsSaft;
        }

        /// <summary>
        /// 檢查傳入學生ID是否修過本課程
        /// </summary>
        /// <param name="StudListID"></param>
        /// <returns></returns>
        private List<string> CheckTempStudentInCourse(List<string> StudListID)
        {
            #region 排除已修過課程的學生
            sc.Reset();
            List<string> list1 = new List<string>(); //本課程的修課學生ID
            foreach (JHStudentRecord each in sc.Students)
            {
                list1.Add(each.ID);
            }

            List<string> list2 = new List<string>(); //沒有在修課學生ID清單內的學生ID
            foreach (string each in StudListID)
            {
                if (!list1.Contains(each))
                {
                    list2.Add(each);
                }
                else
                {
                    ReMoveTemp.Add(each);
                }
            }

            return list2; 
            #endregion
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            #region 加入待處理
            List<string> list = new List<string>();

            foreach (ListViewItem each in lvStudents.SelectedItems)
            {
                JHSCAttendRecord stud = (JHSCAttendRecord)each.Tag;
                list.Add(stud.RefStudentID);
            }
            K12.Presentation.NLDPanels.Student.AddToTemp(list); 
            #endregion
        }


        private void toolStripMenuItem3_Click(object sender, EventArgs e)
        {
            #region 移出待處理
            //將選擇學生移出待處理

            //List<string> list = new List<string>();

            //foreach (ListViewItem each in lvStudents.SelectedItems)
            //{
            //    JHSCAttendRecord stud = (JHSCAttendRecord)each.Tag;
            //    list.Add(stud.RefStudentID);
            //}
            //K12.Presentation.NLDPanels.Student.RemoveFromTemp(list); 
            #endregion
        }

        private void ClearStudentInAssn_Click(object sender, EventArgs e)
        {
            ClearStudent();
        }

        private void btnClearStudent_Click(object sender, EventArgs e)
        {
            ClearStudent();
        }

        private void ClearStudent()
        {
            #region 移除社團學生

            DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show("是否移除選取的修課學生?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);

            if (dr == DialogResult.No)
                return;

            List<JHSCAttendRecord> list = new List<JHSCAttendRecord>();

            foreach (ListViewItem each in lvStudents.SelectedItems)
            {
                JHSCAttendRecord stud = (JHSCAttendRecord)each.Tag;
                list.Add(stud);
            }

            JHSCAttend.Delete(list);

            JHCourseRecord cr = JHCourse.SelectByID(this.PrimaryKey);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("社團名稱：「" + cr.Name + "」");
            sb.AppendLine("移除社員清單：");
            foreach (JHSCAttendRecord each in list)
            {
                if (each.Student.Class != null)
                {
                    sb.Append("班級：「" + each.Student.Class.Name + "」");
                }
                else
                {
                    sb.Append("班級：「」");
                }
                sb.Append("座號：「" + each.Student.SeatNo + "」");
                sb.Append("姓名：「" + each.Student.Name + "」\n");
            }

            ApplicationLog.Log("社團外掛模組", "移除社團修課學生", sb.ToString());
            #endregion
        }

        private void 清空學生待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            #region 清空學生待處理

            K12.Presentation.NLDPanels.Student.RemoveFromTemp(K12.Presentation.NLDPanels.Student.TempSource); 





            #endregion
        }
    }
}
