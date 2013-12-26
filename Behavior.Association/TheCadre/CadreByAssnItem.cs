using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Framework;
using FCode = Framework.Security.FeatureCodeAttribute;
using FISCA.DSAUtil;
using System.Xml;
using JHSchool.Data;
using FISCA.UDT;

namespace JHSchool.Association
{
    [FCode("Behavior.TheCadre.Detail00050", "社團幹部")]
    public partial class CadreByAssnItem : DetailContentBase
    {
        private BackgroundWorker BgW = new BackgroundWorker();

        //UDT功能物件
        private AccessHelper _accessHelper = new AccessHelper();

        private string AssnCadre = "社團幹部";
        private string ConfigName = "BehaviorCadreConfig";

        private int _SchoolYear;
        private int _Semester;

        //幹部Column位置字典
        private Dictionary<string, int> CadreRowIndex = new Dictionary<string, int>();

        internal static Framework.Security.FeatureAce UserPermission;

        private ChangeListener DataListener { get; set; }
        //目前幹部清單
        private List<SchoolObject> UDTSchoolList = new List<SchoolObject>();

        private bool BkWBool = false;

        SelectAllStudent GetStudentObj;

        //全校班級座號字典
        private Dictionary<string, Dictionary<string, JHStudentRecord>> DicStudList = new Dictionary<string, Dictionary<string, JHStudentRecord>>();

        public CadreByAssnItem()
        {
            InitializeComponent();

            UserPermission = User.Acl[FCode.GetCode(GetType())];
            this.Enabled = UserPermission.Editable;

            this.Group = "社團幹部";

            BgW.DoWork += new DoWorkEventHandler(BgW_DoWork);
            BgW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BgW_RunWorkerCompleted);

            DataListener = new ChangeListener();
            DataListener.Add(new DataGridViewSource(dataGridViewX1));
            DataListener.StatusChanged += new EventHandler<ChangeEventArgs>(DataListener_StatusChanged);   
        }

        protected override void OnPrimaryKeyChanged(EventArgs e)
        {
            if (this.PrimaryKey != "") //不是空的
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

        void BgW_DoWork(object sender, DoWorkEventArgs e)
        {
            JHCourseRecord CR = JHCourse.SelectByID(this.PrimaryKey);

            _SchoolYear = CR.SchoolYear.Value;
            _Semester = CR.Semester.Value;
            GetStudentObj = new SelectAllStudent(this.PrimaryKey);

            UDTSchoolList.Clear();
            UDTSchoolList = _accessHelper.Select<SchoolObject>(string.Format("ReferenceType='{0}' and ReferenceID='{1}'", AssnCadre, this.PrimaryKey));     
        }

        void BgW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (BkWBool)
            {
                BkWBool = false;
                BgW.RunWorkerAsync();
                return;
            }

            DataListener.SuspendListen(); //終止變更判斷

            BuildData();

            this.SaveButtonVisible = false;
            this.CancelButtonVisible = false;
            DataListener.Reset();
            DataListener.ResumeListen();
            this.Loading = false;
        }

        private void BuildData()
        {
            dataGridViewX1.Rows.Clear();
            //建立畫面資料
            SetupForm();
            //填入幹部
            foreach (SchoolObject each in UDTSchoolList)
            {
                if (CadreRowIndex.ContainsKey(each.CadreName))
                {
                    //取得學生
                    JHStudentRecord stud = JHStudent.SelectByID(each.StudentID);
                    //取得row
                    DataGridViewRow row = dataGridViewX1.Rows[CadreRowIndex[each.CadreName]];

                    row.Cells[0].Value = stud.ID;
                    if (stud.Class != null)
                    {
                        row.Cells[2].Value = stud.Class.Name;
                    }
                    row.Cells[3].Value = stud.SeatNo.HasValue ? stud.SeatNo.Value.ToString() : "";
                    row.Cells[4].Value = stud.StudentNumber;
                    row.Cells[5].Value = stud.Name;
                }
            }
        }

        protected override void OnSaveButtonClick(EventArgs e)
        {
            Save();

            this.SaveButtonVisible = false;
            this.CancelButtonVisible = false;
            DataListener.Reset();
            DataListener.ResumeListen();
        }

        private void Save()
        {
            //錯誤檢查
            if (CheckError())
            {
                FISCA.Presentation.Controls.MsgBox.Show("您輸入的資料尚有錯誤,請檢查後再進行儲存");
                return;
            }

            try
            {
                _accessHelper.DeletedValues(UDTSchoolList.ToArray());
            }
            catch (Exception ex)
            {
                FISCA.Presentation.Controls.MsgBox.Show("儲存失敗,請重試一次!\n" + ex.Message);
                return;
            }

            //儲存UDT幹部資料
            List<SchoolObject> Insertlist = new List<SchoolObject>();

            foreach (DataGridViewRow each in dataGridViewX1.Rows)
            {
                if (each.IsNewRow) //NewRow下一筆
                    continue;

                if ("" + each.Cells[0].Value == "") //如果是空行則下一筆
                    continue;

                SchoolObject obj = new SchoolObject();

                obj.StudentID = "" + each.Cells[0].Value;
                obj.SchoolYear = _SchoolYear.ToString();
                obj.Semester = _Semester.ToString();

                obj.ReferenceID = this.PrimaryKey;
                obj.ReferenceType = "社團幹部";
                obj.CadreName = "" + each.Cells[1].Value; //幹部名稱
                obj.Text = "社團:" + JHCourse.SelectByID(this.PrimaryKey).Name;

                Insertlist.Add(obj);
            }

            if (Insertlist.Count != 0)
            {
                try
                {
                    _accessHelper.InsertValues(Insertlist.ToArray());
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("儲存失敗!!請重試一次。\n" + ex.Message);
                    return;
                }
            }

            FISCA.Presentation.Controls.MsgBox.Show("儲存成功!");
        }

        protected override void OnCancelButtonClick(EventArgs e)
        {
            this.SaveButtonVisible = false;
            this.CancelButtonVisible = false;

            DataListener.SuspendListen(); //終止變更判斷
            BgW.RunWorkerAsync(); //背景作業,取得並重新填入原資料
        }

        /// <summary>
        /// 針對DataGridView進行錯誤檢查
        /// </summary>
        private bool CheckError()
        {
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.ErrorText != "")
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        void DataListener_StatusChanged(object sender, ChangeEventArgs e)
        {
            this.SaveButtonVisible = (e.Status == ValueStatus.Dirty);
            this.CancelButtonVisible = (e.Status == ValueStatus.Dirty);
        }

        /// <summary>
        /// 設定畫面內容(建立位置字典)
        /// </summary>
        private void SetupForm()
        {
            //設定DataGridView
            K12.Data.Configuration.ConfigData cd = K12.Data.School.Configuration[ConfigName];
            CadreRowIndex.Clear();

            string ClassConfig = cd[AssnCadre];

            if (!string.IsNullOrEmpty(ClassConfig))
            {
                XmlElement xml = DSXmlHelper.LoadXml(ClassConfig);
                foreach (XmlElement XmlEach in xml.SelectNodes("Item"))
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridViewX1);
                    row.Cells[1].Value = XmlEach.GetAttribute("CadreName");
                    int index = dataGridViewX1.Rows.Add(row);
                    CadreRowIndex.Add(XmlEach.GetAttribute("CadreName"), index);
                }
            }
        }

        /// <summary>
        /// 自動填入資料(班級+座號)
        /// </summary>
        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            DataGridViewRow row = dataGridViewX1.Rows[e.RowIndex];
            DataGridViewCell cell = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex];

            DataGridViewCell cellClassName = dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex - 1];
            string ClassName = "" + cellClassName.Value;

            string sean = "" + cell.Value;

            //清空
            foreach (DataGridViewCell each in row.Cells)
            {
                if (each.ColumnIndex == 1)
                {
                    continue;
                }
                each.ErrorText = "";
                each.Value = "";
            }

            if (e.ColumnIndex == 2)
            {
                if (sean != "")
                {
                    if (!GetStudentObj.ClassNameIsNotNull(sean)) //True班級存在
                    {
                        cell.ErrorText = "請確認:\n1.輸入之班級名稱,非本社團學生所屬班級";
                    }
                    else
                    {
                        cell.Value = sean;
                    }
                }
            }
            else if (e.ColumnIndex == 3)
            {
                if (!string.IsNullOrEmpty(ClassName)) //如果班級名稱不是空的
                {
                    if (sean != "")
                    {
                        JHStudentRecord SR = GetStudentObj.GetStudentBySeatNo(ClassName, sean);
                        if (SR != null)
                        {
                            row.Cells[0].Value = SR.ID;
                            row.Cells[2].Value = SR.Class.Name;
                            row.Cells[3].Value = SR.SeatNo;
                            row.Cells[4].Value = SR.StudentNumber;
                            row.Cells[5].Value = SR.Name;
                        }
                        else
                        {
                            cellClassName.Value = ClassName;
                            cell.ErrorText = "輸入座號並不存在!";
                        }
                    }
                }
                else
                {
                    cellClassName.ErrorText = "請輸入班級名稱!";
                    cell.ErrorText = "請輸入班級名稱!";
                }
            }
            else if (e.ColumnIndex == 4)
            {
                if (sean != "") //學號不是空的
                {
                    JHStudentRecord SR = GetStudentObj.GetStudentByStudentNumber(sean);

                    if (SR != null)
                    {
                        row.Cells[0].Value = SR.ID;
                        row.Cells[2].Value = SR.Class.Name;
                        row.Cells[3].Value = SR.SeatNo;
                        row.Cells[4].Value = SR.StudentNumber;
                        row.Cells[5].Value = SR.Name;
                    }
                    else
                    {
                        cell.ErrorText = "請確認:\n1.輸入學號並不存在!\n2.輸入學號非本社團學生";
                    }
                }
            }
        }
    }
}
