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
using FISCA.UDT;
using System.Diagnostics;
using FISCA.LogAgent;

namespace JHSchool.Association
{
    public partial class AssnAddressForm : BaseForm
    {
        //課程清單
        private List<JHCourseRecord> JHCourselist = new List<JHCourseRecord>();

        //資料收集清單(課程ID & 資料)
        private Dictionary<string, info> infoDic = new Dictionary<string, info>();

        //UDT物件
        private AccessHelper _accessHelper = new AccessHelper();

        public AssnAddressForm()
        {
            InitializeComponent();

            AssnAdmin.Instance.TempSourceChanged += new EventHandler(Instance_TempSourceChanged);
            //取得上課地點下拉式清單
            GetAddressList();

            //取得課程
            GetCourseList();

            //取得上課人數
            GetCourseStudent();

            //取得上課地點物件
            GetAssnAddressList();

            //處理畫面
            BingDate();


        }

        private void GetAddressList()
        {
            Column3.Items.Add("");
            K12.Data.Configuration.ConfigData DateConfig = K12.Data.School.Configuration["社團模組_上課地點清單"];
            if (!string.IsNullOrEmpty(DateConfig["上課地點清單"]))
            {
                //至少會add一個空白..
                Column3.Items.AddRange(DateConfig["上課地點清單"].Split(','));
            }
        }

        private void GetCourseList()
        {
            List<JHCourseRecord> JHCourselist = JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource);
            JHCourselist.Sort(SortCourse);
            foreach (JHCourseRecord each in JHCourselist)
            {
                if (!infoDic.ContainsKey(each.ID))
                {
                    info i = new info();
                    i.CourseID = each.ID;
                    i.CourseRecord = each;

                    //取得指導老師
                    if (!string.IsNullOrEmpty(each.MajorTeacherName))
                    {
                        i.指導老師 = each.MajorTeacherName;
                    }

                    infoDic.Add(each.ID, i);
                }
            }
        }

        private void GetCourseStudent()
        {
            List<JHSCAttendRecord> scaList = JHSCAttend.SelectByCourseIDs(AssnAdmin.Instance.SelectedSource);
            foreach (JHSCAttendRecord each in scaList)
            {
                //存在的課程
                if (infoDic.ContainsKey(each.RefCourseID))
                {
                    //如果一般狀態的學生
                    if (each.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)
                    {
                        //取得物件內的學生清單,如果不包含在裡面
                        if (!infoDic[each.RefCourseID].StudentList.Contains(each.RefStudentID))
                        {
                            infoDic[each.RefCourseID].StudentList.Add(each.RefStudentID);
                        }
                    }
                }
            }
        }

        private void GetAssnAddressList()
        {
            //取得所有社團課程的上課地點物件
            //foreach (info each in infoDic.Values)
            //{
            //    each.上課地點 = null;
            //}

            foreach (AssnAddress each in _accessHelper.Select<AssnAddress>())
            {
                if (infoDic.ContainsKey(each.AssociationID))
                {
                    infoDic[each.AssociationID].上課地點 = each;
                }
            }
        }

        private void BingDate()
        {
            dataGridViewX1.Rows.Clear();

            foreach (info each in infoDic.Values)
            {
                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dataGridViewX1);

                //info整個物件
                row.Tag = each;
                //課程物件
                row.Cells[0].Tag = each.CourseRecord;

                row.Cells[0].Value = each.CourseRecord.SchoolYear.HasValue ? each.CourseRecord.SchoolYear.Value.ToString() : "";
                row.Cells[1].Value = each.CourseRecord.Semester.HasValue ? each.CourseRecord.Semester.Value.ToString() : "";
                row.Cells[2].Value = each.CourseRecord.Name;
                row.Cells[3].Value = each.指導老師;
                row.Cells[4].Value = each.StudentList.Count.ToString();
                if (each.上課地點 != null)
                {
                    row.Cells[5].Value = each.上課地點.Address;
                }
                else
                {
                    row.Cells[5].Value = "";
                }
                dataGridViewX1.Rows.Add(row);
            }
        }

        private int SortCourse(JHCourseRecord x,JHCourseRecord y)
        {
            return x.Name.CompareTo(y.Name);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            //this.Close();
        }

        private void lLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            AddressNameList ANL = new AddressNameList();
            ANL.ShowDialog();

            Column3.Items.Clear();
            Column3.Items.Add("");
            K12.Data.Configuration.ConfigData DateConfig = K12.Data.School.Configuration["社團模組_上課地點清單"];
            if (!string.IsNullOrEmpty(DateConfig["上課地點清單"]))
            {
                Column3.Items.AddRange(DateConfig["上課地點清單"].Split(','));
            }

        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            try
            {
                DataGridViewExport export = new DataGridViewExport(dataGridViewX1);
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();
                SaveFileDialog1.Filter = "Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
                SaveFileDialog1.FileName = "社團資訊清單(上課地點)";

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    export.Save(SaveFileDialog1.FileName);
                    Process.Start(SaveFileDialog1.FileName);
                }
                else
                {
                    MsgBox.Show("檔案未儲存");
                }
            }
            catch
            {
                MsgBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("已進行社團上課地點批次登錄");
            sb.AppendLine("詳細資料如下：");

            List<AssnAddress> Deletelist = new List<AssnAddress>();
            List<AssnAddress> Insertlist = new List<AssnAddress>();
            foreach (DataGridViewRow row in dataGridViewX1.Rows)
            {
                //準備刪除資料
                if (row.Tag is info)
                {
                    info aa = row.Tag as info;
                    if (aa.上課地點 != null)
                    {
                        Deletelist.Add(aa.上課地點);
                    }
                }

                //建立新增資料
                if (!string.IsNullOrEmpty("" + row.Cells[5].Value))
                {
                    JHCourseRecord cr = row.Cells[0].Tag as JHCourseRecord;


                    AssnAddress aa = new AssnAddress();
                    aa.Address = "" + row.Cells[5].Value;
                    aa.AssociationID = cr.ID;
                    aa.SchoolYear = cr.SchoolYear.HasValue ? cr.SchoolYear.Value.ToString() : "";
                    aa.Semester = cr.Semester.HasValue ? cr.Semester.Value.ToString() : "";
                    Insertlist.Add(aa);

                    sb.AppendLine("社團：「" + cr.Name + "」上課地點為：「" + aa.Address + "」");
 
                }
            }

            try
            {
                _accessHelper.DeletedValues(Deletelist.ToArray());
                _accessHelper.InsertValues(Insertlist.ToArray());
            }
            catch
            {
                MsgBox.Show("儲存資料發生錯誤!!");
                return;
            }

            ApplicationLog.Log("社團外掛模組", "社團上課地點批次修改", sb.ToString());
            MsgBox.Show("儲存成功!!");
            //this.Close();

            //重整資料
            //GetAssnAddressList();
            //BingDate();
        }

        private void 加入社團待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> list = new List<string>();
            foreach (DataGridViewRow row in dataGridViewX1.SelectedRows)
            {
                JHCourseRecord cr = row.Cells[0].Tag as JHCourseRecord;
                list.Add(cr.ID);
            }
            AssnAdmin.Instance.AddToTemp(list);
        }

        private void 清空社團待處理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AssnAdmin.Instance.RemoveFromTemp(AssnAdmin.Instance.TempSource);
        }

        void Instance_TempSourceChanged(object sender, EventArgs e)
        {
            lbTemp.Text = "待處理：" + AssnAdmin.Instance.TempSource.Count.ToString();
        }

        class info
        {
            public info()
            {
                StudentList = new List<string>();
                指導老師 = "";
            }
            public string CourseID { get; set; }
            public AssnAddress 上課地點 { get; set; }
            public string 指導老師 { get; set; }
            public JHCourseRecord CourseRecord { get; set; }
            public List<string> StudentList { get; set; }
        }

    }
}
