using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data;
using System.Xml;
using FISCA.DSAUtil;
using FISCA.LogAgent;

namespace JHSchool.Association
{
    public partial class AssociationByClassSeanNo : BaseForm
    {
        internal FISCA.UDT.AccessHelper _accessHelper = new FISCA.UDT.AccessHelper();

        internal GetAllStudent GetStudent { get; set; }

        internal SetDataGridViewRowState SetDataGrid = new SetDataGridViewRowState();

        internal BackgroundWorker BGW;

        internal List<SchoolObject> CadreList { get; set; }

        internal Dictionary<string, bool> CadreDic { get; set; }

        internal List<CadreDataRowSchool> _RowList { get; set; }

        private int DefSchoolYear { get; set; }
        private int DefSemester { get; set; }

        internal List<string> CadreRatioList { get; set; }

        //社團ID
        string _AssociationID;
        public K12.Data.CourseRecord _AssociationRecord;

        public AssociationByClassSeanNo()
        {
            InitializeComponent();
        }

        private void CadreByStudentSean_Load(object sender, EventArgs e)
        {
            BGW = new BackgroundWorker();
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            this.Text = "社團幹部登錄(資料讀取中...)";

            dataGridViewX1.AutoGenerateColumns = false;

            Reset();
        }

        private void Reset()
        {
            if (!BGW.IsBusy)
            {
                SetObjType = false;

                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("系統忙碌中...");
            }
        }

        //取得全校學生
        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得社團
            _AssociationID = AssnAdmin.Instance.SelectedSource[0];
            _AssociationRecord = K12.Data.Course.SelectByID(_AssociationID);

            GetStudent = new GetAllStudent(_AssociationID);

            if (_AssociationRecord.SchoolYear.HasValue)
            {
                DefSchoolYear = _AssociationRecord.SchoolYear.Value;
            }

            if (_AssociationRecord.Semester.HasValue)
            {
                DefSemester = _AssociationRecord.Semester.Value;
            }

            GetStudentAndCadre();
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            lbSchoolYear.Text = "學年度「" + DefSchoolYear + "」學期「" + DefSemester + "」社團「" + _AssociationRecord.Name + "」幹部登錄。";

            ChangeForm();

            this.Text = "社團幹部登錄";
            SetObjType = true;
        }

        /// <summary>
        /// 取得班級學生,且篩選出學生幹部
        /// </summary>
        private void GetStudentAndCadre()
        {
            //取得本學期所有社團幹部
            CadreList = _accessHelper.Select<SchoolObject>(string.Format("SchoolYear = '{0}' and Semester = '{1}' and ReferenceType = '{2}'", DefSchoolYear, DefSemester, "社團幹部"));

            SetObj();

        }

        /// <summary>
        /// 依幹部類別清單,建立判斷依據
        /// </summary>
        private void SetObj()
        {
            CadreDic = new Dictionary<string, bool>();
            foreach (SchoolObject each in CadreList)
            {
                if (!CadreDic.ContainsKey(each.UID))
                {
                    CadreDic.Add(each.UID, false);
                }
            }
        }

        /// <summary>
        /// 排序後回傳資料
        /// </summary>
        private List<ClassCadreNameObj> SortCadreNameList(List<ClassCadreNameObj> ClassCadreNameList)
        {
            //依順序(index內容)排序
            List<ClassCadreNameObj> neo = new List<ClassCadreNameObj>();
            var list = from Record in ClassCadreNameList orderby Record.Index select Record;

            foreach (ClassCadreNameObj each in list)
            {
                neo.Add(each);
            }
            return neo;
        }

        /// <summary>
        /// 拋出尚未填入的幹部資料
        /// </summary>
        private SchoolObject Getobj(string CadreName)
        {
            foreach (SchoolObject each in CadreList)
            {
                //判斷null,是為了確認學生來自有班座之學生
                Srecord sr = GetStudent.GetSrecord(each.StudentID);
                if (sr != null)
                {
                    //1.名稱相同
                    //2.字典內是false
                    if (CadreName == each.CadreName && !CadreDic[each.UID])
                    {
                        CadreDic[each.UID] = true;
                        return each;
                    }
                }
            }
            return null;

        }

        /// <summary>
        /// 依幹部名稱更新畫面
        /// </summary>
        private void ChangeForm()
        {
            _RowList = new List<CadreDataRowSchool>();

            //取得社團幹部類型
            List<ClassCadreNameObj> SchoolCadreNameList = _accessHelper.Select<ClassCadreNameObj>(string.Format("NameType = '{0}'", "社團幹部"));

            //取得幹部比序清單
            CadreRatioList = new List<string>();
            foreach (ClassCadreNameObj each in SchoolCadreNameList)
            {
                if (each.Ratio_Order)
                {
                    if (!CadreRatioList.Contains(each.CadreName))
                    {
                        CadreRatioList.Add(each.CadreName);
                    }
                }
            }


            SchoolCadreNameList = SortCadreNameList(SchoolCadreNameList);

            if (SchoolCadreNameList.Count != 0) //是否有字典內容
            {
                foreach (ClassCadreNameObj each in SchoolCadreNameList)
                {
                    //依照有幾個幹部名單,而增加DataGridViewRow
                    for (int x = 1; x <= each.Number; x++)
                    {
                        //取得幹部記錄
                        SchoolObject obj = Getobj(each.CadreName);

                        //尚未填入欄位
                        if (obj != null)
                        {
                            //取得學生擔任資料
                            Srecord student = GetStudent.GetSrecord(obj.StudentID);

                            if (student != null) //如果為null表示學生無座號,或是無班級,或是狀態不是一般生
                            {
                                CadreDataRowSchool cdr = new CadreDataRowSchool(student, obj, each.Index, this, DefSchoolYear, DefSemester);
                                _RowList.Add(cdr);
                            }
                            else
                            {
                                CadreDataRowSchool cdr = new CadreDataRowSchool(each.CadreName, each.Index, this, DefSchoolYear, DefSemester);
                                _RowList.Add(cdr);
                            }
                        }
                        else
                        {
                            CadreDataRowSchool cdr = new CadreDataRowSchool(each.CadreName, each.Index, this, DefSchoolYear, DefSemester);
                            _RowList.Add(cdr);
                        }
                    }
                }

                //排序
                _RowList.Sort(SortRow);

                dataGridViewX1.DataSource = new BindingList<CadreDataRowSchool>(_RowList);
            }
            else
            {
                DialogResult dr = MsgBox.Show("您未設定社團幹部清單\n是否要現在設定?", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    CadreNameChange(); //開啟設定檔畫面
                }
            }

        }

        private int SortRow(CadreDataRowSchool a, CadreDataRowSchool b)
        {
            string indexA = a._index.ToString().PadLeft(3, '0');
            string indexB = b._index.ToString().PadLeft(3, '0');
            string SeatNoA = "" + a._StudentSeatNo.PadLeft(3, '0');
            string SeatNoB = "" + b._StudentSeatNo.PadLeft(3, '0');
            indexA += SeatNoA;
            indexB += SeatNoB;
            return indexA.CompareTo(indexB);
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            CadreDataRowSchool c = dataGridViewX1.Rows[e.RowIndex].DataBoundItem as CadreDataRowSchool;

            dataGridViewX1.Rows[e.RowIndex].Cells[2].ErrorText = c.ClassNameError;
            dataGridViewX1.Rows[e.RowIndex].Cells[3].ErrorText = c.SeatNoError;
        }

        private bool CheckClassColumn(DataGridViewRow row)
        {
            //姓名欄位內容
            DataGridViewCell cellStudentName = row.Cells[colStudentName.Index];
            //班級名稱欄位內容
            DataGridViewCell cellClassName = row.Cells[colClassName.Index];
            //座號欄位內容
            DataGridViewCell cellSeatNo = row.Cells[colSeatNo.Index];

            //班級名稱"不是"空值
            if (!string.IsNullOrEmpty("" + cellClassName.Value))
            {
                //檢查班級名稱是否存在
                if (!GetStudent.GetClassIsNull("" + cellClassName.Value))
                {
                    //當班級不存在,將所有資料重置
                    cellClassName.ErrorText = "班級不存在!!";
                    SetDataGrid.IsErrorRow(row);
                    return true;
                }
                else //存在則清空錯誤訊息(正確狀態)
                {
                    cellClassName.ErrorText = "";
                    return false;
                }
            }
            else //班級名稱"是"空值,整個Row應初始化為空資料
            {
                //傳入Row以初始化資料
                SetDataGrid.IsNewRow(row);
                return true;
            }
        }

        /// <summary>
        /// 儲存
        /// </summary>
        private void btnSave_Click(object sender, EventArgs e)
        {
            //鎖定儲存時畫面
            SetObjType = false;

            List<SchoolObject> InsertList = new List<SchoolObject>();
            List<SchoolObject> DeleteList = new List<SchoolObject>();
            List<SchoolObject> DefList = new List<SchoolObject>();

            List<CadreDataRowSchool> insert = new List<CadreDataRowSchool>();
            List<CadreDataRowSchool> delete = new List<CadreDataRowSchool>();
            List<CadreDataRowSchool> Def = new List<CadreDataRowSchool>();

            foreach (CadreDataRowSchool data in _RowList)
            {
                //Record不是null,但是沒有UID就是新增資料
                if (data._CadreRecord != null && data._CadreRecord.UID == "")
                {
                    insert.Add(data);
                }

                //Del內不為null,就是有刪除資料
                if (data._CadreRecordDel != null)
                {
                    delete.Add(data);
                }

                //有資料,有ID,用來登錄獎勵使用
                if (data._CadreRecord != null && data._CadreRecord.UID != "")
                {
                    Def.Add(data);
                }
            }

            foreach (CadreDataRowSchool data in Def)
            {
                //當該筆記錄的社團名稱不相同時,更新為目前社團名稱
                if (data._CadreRecord.Text != _AssociationRecord.Name)
                {
                    data._CadreRecord.Text = _AssociationRecord.Name; //2012/2/3 目的是更新社團名稱
                }
                DefList.Add(data._CadreRecord);
            }

            ////取得所有目前社團幹部資料
            //List<SchoolObject> DeleteList = _accessHelper.Select<SchoolObject>(string.Format("SchoolYear = '{0}' and Semester = '{1}' and ReferenceType = '{2}'", School.DefaultSchoolYear, School.DefaultSemester, "學校幹部"));
            ////新增資料清單
            //List<SchoolObject> InsertList = new List<SchoolObject>();
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("學年度「" + DefSchoolYear + "」學期「" + DefSemester + "」");

            if (insert.Count != 0)
            {
                sb.AppendLine("新增社團幹部登錄：");

                foreach (CadreDataRowSchool data in insert)
                {
                    InsertList.Add(data._CadreRecord);

                    sb.AppendLine("學生「" + data._StudentName + "」擔任「社團幹部」幹部名稱「" + data._CadreName + "」已新增");
                }
            }

            if (delete.Count != 0)
            {
                sb.AppendLine("刪除舊有社團幹部記錄：");

                foreach (CadreDataRowSchool data in delete)
                {
                    DeleteList.Add(data._CadreRecordDel);
                    K12.Data.StudentRecord sr = K12.Data.Student.SelectByID(data._CadreRecordDel.StudentID);

                    sb.AppendLine("學生「" + sr.Name + "」擔任「社團幹部」幹部名稱「" + data._CadreName + "」已被刪除");
                }
            }

            List<string> CadreIDList = new List<string>();

            try
            {
                //新增資料
                CadreIDList = _accessHelper.InsertValues(InsertList.ToArray());
                //刪除舊資料
                _accessHelper.DeletedValues(DeleteList.ToArray());
                //更新資料(一般只更新社團名稱)
                _accessHelper.UpdateValues(DefList.ToArray());
            }
            catch
            {
                MsgBox.Show("新增資料發生錯誤!!");
                SetObjType = true;
                return;
            }

            if (InsertList.Count + DeleteList.Count > 0)
            {
                ApplicationLog.Log("幹部外掛模組", "社團幹部登錄", sb.ToString());
                MsgBox.Show("幹部記錄,儲存成功!!");
            }
            else
            {
                MsgBox.Show("未修改資料!!");
            }

            SetObjType = true;

            //敘獎模式
            if (checkBoxX1.Checked)
            {
                (new K12.Behavior.TheCadre.CadreMeritManage.CadreMeritManage(this.DefSchoolYear, this.DefSemester, CadreType.ClubCadre,this._AssociationID)).ShowDialog();
                // 舊功能畫面
                //List<SchoolObject> list = new List<SchoolObject>();
                //if (CadreIDList.Count != 0)
                //{
                //    String sb123 = string.Join(",", CadreIDList.ToArray());
                //    list = _accessHelper.Select<SchoolObject>(string.Format("UID in (" + sb123 + ")"));
                //}
                //list.AddRange(DefList);
                //if (list.Count != 0)
                //{
                //    //進行敘獎操作
                //    //ps:絮獎模式有兩種
                //    //1.將學生基本資料導入獎勵功能
                //    //2.透過設定值,以幹部為基準進行絮獎(較方便) <--
                //    MutiMeritDemerit mmd = new MutiMeritDemerit("獎勵", list, "社團幹部", DefSchoolYear, DefSemester);
                //    mmd.ShowDialog();
                //}
                //else
                //{
                //    MsgBox.Show("因無學生擔任幹部\n敘獎畫面將不會開啟!!");
                //}
            }

            Reset();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            CadreNameChange();
        }

        /// <summary>
        /// 開啟幹部名稱管理清單
        /// </summary>
        private void CadreNameChange()
        {
            NewCadreSetup TCN = new NewCadreSetup();
            TCN.ShowDialog();

            Reset();
        }

        /// <summary>
        /// 設定特定畫面為Enabled
        /// </summary>
        private bool SetObjType
        {
            set
            {
                linkLabel1.Enabled = value;
                btnSave.Enabled = value;
                btnExit.Enabled = value;
                checkBoxX1.Enabled = value;
                dataGridViewX1.Enabled = value;
            }
        }

        //排序學校幹部名稱物件
        private int SortIndex(ClassCadreNameObj x, ClassCadreNameObj y)
        {
            return x.Index.CompareTo(y.Index);
        }

        /// <summary>
        /// 離開
        /// </summary>
        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void checkBoxX1_CheckedChanged(object sender, EventArgs e)
        {
            //K12.Data.Configuration.ConfigData DateConfig = K12.Data.School.Configuration["幹部模組_幹部名稱清單"];
            //DateConfig["社團幹部_是否進行敘獎"] = checkBoxX1.Checked.ToString();
            //DateConfig.Save();
        }
    }
}
