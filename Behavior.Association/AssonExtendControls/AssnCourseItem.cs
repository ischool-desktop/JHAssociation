using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using JHSchool.Data;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;
using FISCA.LogAgent;
using FISCA.UDT;

namespace JHSchool.Association
{
    [FCode("JHSchool.Association.Detail0010", "社團基本資料")]
    public partial class AssnCourseItem : DetailContentBase
    {
        private ChangeListener DataListener { get; set; }

        private BackgroundWorker BgW = new BackgroundWorker();
        private bool BkWBool = false;
        private JHCourseRecord _Record;

        //UDT物件
        private AccessHelper _accessHelper = new AccessHelper();

        //上課地點
        private List<AssnAddress> Addresslist = new List<AssnAddress>();

        internal static Framework.Security.FeatureAce UserPermission;

        //private List<JHTeacherRecord> TeahcerList = new List<JHTeacherRecord>();
        //private Dictionary<string, JHTeacherRecord> Teacherdic = new Dictionary<string, JHTeacherRecord>();

        public AssnCourseItem()
        {
            InitializeComponent();

            #region 建構子
            Group = "社團基本資料";
            UserPermission = User.Acl[FCode.GetCode(GetType())];

            this.Enabled = UserPermission.Editable;

            //如果課程資料更新了
            JHCourse.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHCourse_AfterChange); 
            Course.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Course_ItemUpdated);

            //課程關聯更新
            JHTCInstruct.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHTCInstruct_AfterInsert);

            //社團更新鈕的事件
            AssnEvents.AssnChanged += new EventHandler(AssnEvents_AssnChanged);

            //如果教師資料更新了
            JHTeacher.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHTeacher_AfterChange); 
            Teacher.Instance.ItemUpdated += new EventHandler<ItemUpdatedEventArgs>(Teacher_ItemUpdated); 

            //背景模式
            BgW.DoWork += new DoWorkEventHandler(BkW_DoWork);
            BgW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BkW_RunWorkerCompleted);

            //如果修改了內容
            DataListener = new ChangeListener();
            DataListener.Add(new TextBoxSource(txtCourseName)); //社團名稱
            DataListener.Add(new ComboBoxSource(cbxTeacher, ComboBoxSource.ListenAttribute.SelectedIndex));
            DataListener.Add(new ComboBoxSource(cbxSchoolYear, ComboBoxSource.ListenAttribute.SelectedIndex));
            DataListener.Add(new ComboBoxSource(cbxSemeter, ComboBoxSource.ListenAttribute.SelectedIndex));
            DataListener.Add(new ComboBoxSource(cbxEAddress, ComboBoxSource.ListenAttribute.SelectedIndex)); //上課地點
            DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(DataListener_StatusChanged);

            ChengSchoolYearSemester(JHSchool.School.DefaultSchoolYear, School.DefaultSemester);

            SetupTeacher();

            #endregion
        }

        void AssnEvents_AssnChanged(object sender, EventArgs e)
        {
            if (this.PrimaryKey != "")
            {
                this.Loading = true;
                errorProvider1.Clear();

                if (BgW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    SetupTeacher();
                    BgW.RunWorkerAsync();
                }
            }
        }

        /// <summary>
        /// 重設教師資料
        /// </summary>
        private void SetupTeacher()
        {
            JHTeacher.RemoveAll();
            cbxTeacher.DisplayMember = "FullName";
            cbxTeacher.Items.Clear();
            cbxTeacher.Items.Add(new TeacherItme()); //先增加一個空位置

            List<JHTeacherRecord> list = JHTeacher.SelectAll();
            list.Sort(SortTeacher);
            foreach (JHTeacherRecord each in list)
            {
                if (each.Status == K12.Data.TeacherRecord.TeacherStatus.一般)
                {
                    cbxTeacher.Items.Add(new TeacherItme(each));
                }
            }
        }

        private int SortTeacher(JHTeacherRecord x1, JHTeacherRecord x2)
        {
            return x1.Name.CompareTo(x2.Name);
        }

        void JHCourse_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            #region 課程資料更新
            if (this.PrimaryKey != "")
            {
                List<JHCourseTagRecord> crList = JHCourseTag.SelectByCourseIDs(e.PrimaryKeys);
                foreach (JHCourseTagRecord each in crList)
                {
                    if (each.Name == "社團" || each.Name == "聯課活動")
                    {

                        if (BgW.IsBusy)
                        {
                            BkWBool = true;
                        }
                        else
                        {
                            BgW.RunWorkerAsync();
                        }
                        break;
                    }
                }
            }
            #endregion
        }

        void Course_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            #region 課程資料更新
            if (this.PrimaryKey != "")
            {
                List<JHCourseTagRecord> crList = JHCourseTag.SelectByCourseIDs(e.PrimaryKeys);
                foreach (JHCourseTagRecord each in crList)
                {
                    if (each.Name == "社團" || each.Name == "聯課活動")
                    {

                        if (BgW.IsBusy)
                        {
                            BkWBool = true;
                        }
                        else
                        {
                            BgW.RunWorkerAsync();
                        }
                        break;
                    }
                }
            }
            #endregion
        }

        void JHTeacher_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            #region 教師資料更新
            if (this.PrimaryKey != "")
            {
                this.Loading = true;
                errorProvider1.Clear();

                if (BgW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    SetupTeacher();
                    BgW.RunWorkerAsync();
                }
            }
            #endregion
        }

        void Teacher_ItemUpdated(object sender, ItemUpdatedEventArgs e)
        {
            #region 教師資料更新
            if (this.PrimaryKey != "")
            {
                this.Loading = true;
                errorProvider1.Clear();

                if (BgW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    SetupTeacher();
                    BgW.RunWorkerAsync();
                }
            }
            #endregion
        }

        void JHTCInstruct_AfterInsert(object sender, K12.Data.DataChangedEventArgs e)
        {
            #region 教師資料更新
            if (this.PrimaryKey != "")
            {
                this.Loading = true;
                errorProvider1.Clear();

                if (BgW.IsBusy)
                {
                    BkWBool = true;
                }
                else
                {
                    SetupTeacher();
                    BgW.RunWorkerAsync();
                }
            }
            #endregion
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            #region PrimaryKey更新
            if (this.PrimaryKey != "")
            {
                this.Loading = true;

                errorProvider1.Clear();

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

        void BkW_DoWork(object sender, DoWorkEventArgs e)
        {
            #region 背景模式
            JHCourse.RemoveAll();

            _Record = JHCourse.SelectByID(this.PrimaryKey);

            //取得社團上課地點
            if (_Record != null)
            {
                string bkw_SchoolYear = _Record.SchoolYear.HasValue ? _Record.SchoolYear.Value.ToString() : "";
                string bkw_Semester = _Record.Semester.HasValue ? _Record.Semester.Value.ToString() : "";
                Addresslist.Clear();
                Addresslist = _accessHelper.Select<AssnAddress>(string.Format("AssociationID='{0}'", this.PrimaryKey));
            }
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

        private void ChengSchoolYearSemester(string SchoolYear, string Semester)
        {
            #region 學年度/學期
            cbxSchoolYear.Items.Clear();
            cbxSchoolYear.Items.Add(int.Parse(SchoolYear) - 3);
            cbxSchoolYear.Items.Add(int.Parse(SchoolYear) - 2);
            cbxSchoolYear.Items.Add(int.Parse(SchoolYear) - 1);
            int SchoolYearIndex = cbxSchoolYear.Items.Add(int.Parse(SchoolYear));
            cbxSchoolYear.Items.Add(int.Parse(SchoolYear) + 1);
            cbxSchoolYear.Items.Add(int.Parse(SchoolYear) + 2);
            cbxSchoolYear.SelectedIndex = SchoolYearIndex;

            cbxSemeter.Items.Clear();
            cbxSemeter.Items.Add("1");
            cbxSemeter.Items.Add("2");
            cbxSemeter.SelectedIndex = Semester == "1" ? 0 : 1;
            #endregion
        }

        private int TeacherCompare(JHTeacherRecord x, JHTeacherRecord y)
        {
            return x.Name.CompareTo(y.Name);
        }

        private class TeacherItme
        {
            #region TeacherItme

            public TeacherItme(JHTeacherRecord record)
            {
                Record = record;

                FullName = GetFullName(record);
            }

            public TeacherItme()
            {
                Record = null;
                FullName = "";
            }

            public static string GetFullName(JHTeacherRecord record)
            {
                if (record == null)
                {
                    return "";
                }
                if (string.IsNullOrEmpty(record.Nickname))
                    return record.Name;
                else
                    return string.Format("{0}({1})", record.Name, record.Nickname);
            }

            public string FullName { get; private set; }

            public JHTeacherRecord Record { get; private set; } 

            #endregion
        }

        private void BindData()
        {
            #region 更新畫面資料

            if (_Record == null)
                return;

            DataListener.SuspendListen(); //終止變更判斷

            CmbChange();

            //如果有上課地點
            if (Addresslist.Count >= 1)
            {
                cbxEAddress.Text = Addresslist[0].Address;
            }
            else
            {
                cbxEAddress.Text = string.Empty;
            }

            txtCourseName.Text = _Record.Name;

            //是否有老師
            if (_Record.MajorTeacherID != "")
            {
                JHTeacherRecord trecord = JHTeacher.SelectByID(_Record.MajorTeacherID);
                int index = cbxTeacher.FindString(TeacherItme.GetFullName(trecord));
                if (index >= 0) //大於0或等於0就是有
                    cbxTeacher.SelectedIndex = index;
                else
                    cbxTeacher.SelectedIndex = 0;
            }
            else
            {
                cbxTeacher.SelectedIndex = 0;
            }

            //學年度/學期
            if (_Record.SchoolYear.HasValue && _Record.Semester.HasValue)
            {
                ChengSchoolYearSemester(_Record.SchoolYear.Value.ToString(), _Record.Semester.Value.ToString());
            }

            if (!_Record.SchoolYear.HasValue)
            {
                cbxSchoolYear.Items.Clear();
            }

            if (!_Record.Semester.HasValue)
            {
                cbxSemeter.Items.Clear();
            }

            SaveButtonVisible = false;
            CancelButtonVisible = false;

            DataListener.Reset();
            DataListener.ResumeListen();


            #endregion
        }

        private void CmbChange()
        {
            cbxEAddress.Items.Clear();
            K12.Data.Configuration.ConfigData DateConfig = K12.Data.School.Configuration["社團模組_上課地點清單"];
            if (!string.IsNullOrEmpty(DateConfig["上課地點清單"]))
            {
                //至少會add一個空白..
                cbxEAddress.Items.AddRange(DateConfig["上課地點清單"].Split(','));
            }
        }

        StringBuilder sb = new StringBuilder();

        protected override void OnSaveButtonClick(EventArgs e)
        {
            if (CheckData())
            {
                return;
            }

            #region 儲存資料

            //上課地點Log
            string addressString1 = "";
            string addressString2 = "";

            //如果沒有此課程則離開
            JHCourseRecord AtnCourse = JHCourse.SelectByID(this.PrimaryKey);
            if (AtnCourse == null)
                return;


            //取得社團上課地點(並刪除)
            string bkw_SchoolYear = AtnCourse.SchoolYear.HasValue ? AtnCourse.SchoolYear.Value.ToString() : "";
            string bkw_Semester = AtnCourse.Semester.HasValue ? AtnCourse.Semester.Value.ToString() : "";
            Addresslist.Clear();
            //Addresslist = _accessHelper.Select<AssnAddress>(string.Format("AssociationID='{0}' and SchoolYear='{1}' and Semester='{2}'", this.PrimaryKey, bkw_SchoolYear, bkw_Semester));
            Addresslist = _accessHelper.Select<AssnAddress>(string.Format("AssociationID='{0}'", this.PrimaryKey));
            addressString1 = Addresslist.Count != 0 ? Addresslist[0].Address : "";
            _accessHelper.DeletedValues(Addresslist.ToArray());
            Addresslist.Clear();

            //新增
            if (cbxEAddress.Text != "")
            {
                AssnAddress AA = new AssnAddress();
                AA.AssociationID = this.PrimaryKey;
                AA.SchoolYear = bkw_SchoolYear;
                AA.Semester = bkw_Semester;
                AA.Address = addressString2 = cbxEAddress.Text;
                Addresslist.Add(AA);
                _accessHelper.InsertValues(Addresslist.ToArray());
            }

            sb.Remove(0, sb.Length);
            sb.AppendLine("社團課程原有資料狀態：");
            sb.AppendLine("名稱：「" + _Record.Name + "」");
            sb.AppendLine("授課老師：「" + _Record.MajorTeacherName + "」");
            sb.AppendLine("學年度：「" + _Record.SchoolYear + "」");
            sb.AppendLine("學期：「" + _Record.Semester + "」");
            sb.AppendLine("上課地點：「" + addressString1 + "」");

            sb.AppendLine("修改後資料狀態：");
            sb.AppendLine("名稱：「" + txtCourseName.Text + "」");

            TeacherItme Titem = (TeacherItme)cbxTeacher.SelectedItem;

            JHTeacherRecord Teacher = Titem.Record;

            List<JHTCInstructRecord> TCinseruct = JHTCInstruct.SelectByTeacherIDAndCourseID(new List<string>(), new List<string>() { _Record.ID });

            if (Titem.Record == null) //如果是null就清空
            {
                sb.AppendLine("授課老師：「」");
                JHTCInstruct.Delete(TCinseruct);
            }
            else
            {

                sb.AppendLine("授課老師：「" + Teacher.Name + "」");

                if (TCinseruct.Count != 0) //如果不等於0,就刪除後新增
                {
                    JHTCInstruct.Delete(TCinseruct);

                    JHTCInstructRecord InsertTeacher = new JHTCInstructRecord();
                    InsertTeacher.RefCourseID = _Record.ID;
                    InsertTeacher.RefTeacherID = Teacher.ID;
                    InsertTeacher.Sequence = 1;
                    JHTCInstruct.Insert(InsertTeacher);
                }
                else //等於0就直接新增
                {
                    JHTCInstructRecord InsertTeacher = new JHTCInstructRecord();
                    InsertTeacher.RefCourseID = _Record.ID;
                    InsertTeacher.RefTeacherID = Teacher.ID;
                    InsertTeacher.Sequence = 1;
                    JHTCInstruct.Insert(InsertTeacher);
                }
            }

            sb.AppendLine("學年度：「" + cbxSchoolYear.Text + "」");
            sb.AppendLine("學期：「" + cbxSemeter.Text + "」");
            sb.AppendLine("上課地點：「" + addressString2 + "」");

            _Record.Name = txtCourseName.Text;
            _Record.SchoolYear = int.Parse(cbxSchoolYear.Text);
            _Record.Semester = int.Parse(cbxSemeter.Text);
            JHCourse.Update(_Record);

            ApplicationLog.Log("社團外掛模組", "修改社團基本資料", "course", _Record.ID, sb.ToString());

            SaveButtonVisible = false;
            CancelButtonVisible = false;

            #endregion
        }

        private bool CheckData()
        {
            //老師檢查
            if (cbxTeacher.FindString(cbxTeacher.Text) == -1)
            {
                MsgBox.Show("輸入教師不存在!!");
                return true;
            }

            if (cbxSchoolYear.Text == "" || cbxSemeter.Text == "")
            {
                MsgBox.Show("無學年度學期,無法儲存");
                return true;
            }

            //資料檢查
            List<JHCourseRecord> NewRecord = JHCourse.SelectBySchoolYearAndSemester(int.Parse(cbxSchoolYear.Text), int.Parse(cbxSemeter.Text), txtCourseName.Text);

            if (NewRecord.Count == 1)
            {

                if (NewRecord[0].ID != _Record.ID) //又不是同一筆
                {
                    MsgBox.Show("課程名稱,學年度,學期 不可重覆");
                    return true;
                }
                else
                {
                    return false;
                }

            }
            else
            {
                return false;
            }

            
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            #region 取消變更

            SaveButtonVisible = false;
            CancelButtonVisible = false;

            DataListener.SuspendListen(); //終止變更判斷
            BgW.RunWorkerAsync(); //背景作業,取得並重新填入原資料

            #endregion
        }

        void DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        private void cbxTeacher_Validated(object sender, EventArgs e)
        {
            if (cbxTeacher.FindString(cbxTeacher.Text) == -1)
            {
                errorProvider1.SetError(cbxTeacher, "輸入教師不存在!");
            }
            else
            {
                errorProvider1.Clear();
            }
        }

        private void lLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AddressNameList ANL = new AddressNameList();
            ANL.ShowDialog();

            CmbChange();
        }
    }
}
