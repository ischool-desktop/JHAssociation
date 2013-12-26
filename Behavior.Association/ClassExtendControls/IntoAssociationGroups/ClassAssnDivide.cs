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
using System.Xml.Linq;
using FISCA.LogAgent;
using FISCA.Data;

namespace JHSchool.Association
{
    public partial class ClassAssnDivide : BaseForm
    {

        #region 全域變數
        //課程名稱 & 課程物件
        Dictionary<string, JHCourseRecord> CourseDic = new Dictionary<string, JHCourseRecord>();

        //學生ID + 修課物件(重覆部份)
        Dictionary<string, DivideOBJ> OBJDic = new Dictionary<string, DivideOBJ>();

        //學生所參與的社團課程記錄
        List<JHSCAttendRecord> SetList = new List<JHSCAttendRecord>();

        //重覆修課則為True
        bool IsRedundant = false;

        //資料變動會儲存Row至此Live
        List<DataGridViewRow> RowList = new List<DataGridViewRow>();

        //背景模式
        BackgroundWorker BGW = new BackgroundWorker();

        //社團課程
        List<JHCourseRecord> CourseRecordList = new List<JHCourseRecord>();

        //社團課程ID
        List<string> CourseIDList = new List<string>();

        //預設學年度學期
        int DefaultSchoolYear;
        int DefaultSemester;

        //學生清單
        List<JHStudentRecord> StudentList = new List<JHStudentRecord>();
        #endregion

        public ClassAssnDivide()
        {
            InitializeComponent();
        }

        private void ClassAssnDivide_Load(object sender, EventArgs e)
        {
            //預設學年度
            integerInput1.Value = DefaultSchoolYear = int.Parse(School.DefaultSchoolYear);
            integerInput2.Value = DefaultSemester = int.Parse(School.DefaultSemester);

            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            Reset();
        }

        private void SetEnabled(bool ne)
        {
            integerInput1.Enabled = ne;
            integerInput2.Enabled = ne;
            btnSave.Enabled = ne;
            refsh.Enabled = ne;
        }

        private void Reset()
        {
            if (!BGW.IsBusy)
            {
                SetEnabled(false);
                DefaultSchoolYear = integerInput1.Value;
                DefaultSemester = integerInput2.Value;

                CourseDic.Clear();
                CourseRecordList.Clear();
                CourseIDList.Clear();
                StudentList.Clear();
                Column4.Items.Clear();
                dataGridViewX1.Rows.Clear();

                this.Text = "資料讀取中...";             

                BGW.RunWorkerAsync();
            }
            else
            {
                MsgBox.Show("系統忙碌中,請重開本畫面!");
            }
        }

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            this.dataGridViewX1.CellValueChanged -= new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellValueChanged);
            this.integerInput1.ValueChanged -= new System.EventHandler(this.integerInput1_ValueChanged);
            this.integerInput2.ValueChanged -= new System.EventHandler(this.integerInput2_ValueChanged);

            QueryHelper query = new QueryHelper();
            //取得班級ID之下,學生狀態為1(一般生之學生)
            string cmd = string.Format("select * from student where ref_class_id='{0}' and status='1'", K12.Presentation.NLDPanels.Class.SelectedSource[0]);
            DataTable result = query.Select(cmd);
            List<string> studentidList = new List<string>();
            foreach (DataRow each in result.Rows)
            {
                studentidList.Add("" + each["id"]);
            }

            StudentList = JHStudent.SelectByIDs(studentidList);

            //本學年度學期的社團有哪些

            List<JHCourseTagRecord> JHCourseTaglist = JHCourseTag.SelectAll();
            foreach (JHCourseTagRecord each in JHCourseTaglist)
            {
                if (each.FullName.Contains("聯課活動") || each.FullName.Contains("社團"))
                {
                    if (DefaultSchoolYear == each.Course.SchoolYear && DefaultSemester == each.Course.Semester)
                    {
                        CourseDic.Add(each.Course.Name, each.Course);
                        CourseRecordList.Add(each.Course);
                        CourseIDList.Add(each.Course.ID);
                    }
                }
            }

            //瞭解那些學生已經參與社團
            //新增前先檢查是否已修社團課程(傳入學生清單&社團課程)
            SetList.Clear();
            SetList = JHSCAttend.SelectByStudentIDAndCourseID(StudentList.ToKeys(), CourseIDList);
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error == null)
            {
                CourseRecordList.Sort(courseSortName);
                if (CourseRecordList.Count != 0)
                {
                    foreach (JHCourseRecord each in CourseRecordList)
                    {
                        Column4.Items.Add(each.Name);
                    }
                }
                else
                {
                    SetEnabled(true);
                    this.Text = "社團快速分組(本學期無社團)";
                    MsgBox.Show("尚未開設任何社團!\n請建立社團後再開啟本畫面操作");
                    return;
                }

                OBJDic.Clear();

                //取得班級學生
                StudentList.Sort(studentSortSeatNo);

                foreach (JHStudentRecord student in StudentList)
                {
                    DataGridViewRow row = new DataGridViewRow();
                    row.CreateCells(dataGridViewX1);
                    row.Tag = student;
                    row.Cells[0].Value = student.SeatNo.HasValue ? student.SeatNo.Value.ToString() : "";
                    row.Cells[1].Value = student.Name;
                    row.Cells[2].Value = student.StudentNumber;

                    //Linq寫法
                    //if (SetList.Select(x => x.RefStudentID).Contains(student.ID))
                    //{
                    //    row.Cells[3].Value = "社團";
                    //}

                    //建立學生修課資訊物件

                    if (!OBJDic.ContainsKey(student.ID))
                    {
                        OBJDic.Add(student.ID, new DivideOBJ(student.ID));
                    }

                    string ErrorString = ""; //記錄重覆之課程

                    foreach (JHSCAttendRecord sca in SetList)
                    {
                        if (sca.RefStudentID == student.ID)
                        {
                            if (row.Cells[3].Tag == null)
                            {
                                row.Cells[3].Value = sca.Course.Name;
                                row.Cells[3].Tag = sca; //將修課記錄存入Tag
                                OBJDic[sca.RefStudentID].SCR = sca;
                            }
                            else
                            {
                                OBJDic[sca.RefStudentID].SCRList.Add(sca);
                                ErrorString += sca.Course.Name + "。"; //如果社團重覆,就Add重覆之字串
                                IsRedundant = true; //重覆的特別判斷
                            }
                        }
                    }

                    if (OBJDic[student.ID].SCRList.Count != 0)
                    {
                        row.Cells[3].ErrorText = "重覆參與社團：" + ErrorString;
                    }
                    dataGridViewX1.Rows.Add(row);
                }

                this.dataGridViewX1.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellValueChanged);
                this.integerInput1.ValueChanged += new System.EventHandler(this.integerInput1_ValueChanged);
                this.integerInput2.ValueChanged += new System.EventHandler(this.integerInput2_ValueChanged);

                SetEnabled(true);
                this.Text = "社團快速分組(班級名稱:" + JHClass.SelectByID(K12.Presentation.NLDPanels.Class.SelectedSource[0]).Name + ")";
            }
            else
            {
                MsgBox.Show("取得資料,發生錯誤!!\n" + e.Error.Message);
                SetEnabled(true);
                return;
            }
        }

        //變更事件
        private void dataGridViewX1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 3)
            {
                if (!RowList.Contains(dataGridViewX1.Rows[e.RowIndex]))
                {
                    RowList.Add(dataGridViewX1.Rows[e.RowIndex]);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (RowList.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("社團快速分組相關作業");

                //儲存?將該學生於原社團清除(提示)
                //原社團之修客記錄已存於Cell[3].Tag之內
                List<JHSCAttendRecord> DeleteRedundantList = new List<JHSCAttendRecord>();
                List<JHSCAttendRecord> DeleteList = new List<JHSCAttendRecord>();
                List<JHSCAttendRecord> InsertList = new List<JHSCAttendRecord>();

                #region 重覆
                if (IsRedundant)
                {
                    DialogResult drRedundant = FISCA.Presentation.Controls.MsgBox.Show("有重覆參與社團之學生\n是否自重覆之社團移出。", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);

                    if (drRedundant == DialogResult.Yes)
                    {
                        foreach (string StudentId in OBJDic.Keys)
                        {
                            if (OBJDic[StudentId].SCRList.Count > 0)
                            {
                                DeleteRedundantList.AddRange(OBJDic[StudentId].SCRList);
                            }
                        }

                        sb.AppendLine("重覆參與社團移出作業：");
                        //Log用
                        foreach (JHSCAttendRecord sca in DeleteRedundantList)
                        {
                            sb.AppendLine("學生「" + sca.Student.Name + "」已經自「" + sca.Course.Name + "」社團移出。");
                        }

                        try
                        {
                            JHSCAttend.Delete(DeleteRedundantList);
                        }
                        catch
                        {
                            FISCA.Presentation.Controls.MsgBox.Show("學生重覆參與社團\n移出作業失敗...");
                            return;
                        }
                    }
                }
                #endregion

                foreach (DataGridViewRow row in RowList)
                {
                    JHStudentRecord student = (JHStudentRecord)row.Tag;

                    if (OBJDic[student.ID].SCR != null)
                    {
                        if ("" + row.Cells[3].Value != OBJDic[student.ID].SCR.Course.Name) //不是原資料
                        {
                            DeleteList.Add(OBJDic[student.ID].SCR); //加起來,要刪除

                            if (CourseDic.ContainsKey("" + row.Cells[3].Value)) //真的有這個社團
                            {
                                JHCourseRecord course = CourseDic["" + row.Cells[3].Value]; //取得課程

                                JHSCAttendRecord sca = new JHSCAttendRecord();
                                sca.RefCourseID = course.ID;
                                sca.RefStudentID = student.ID;
                                InsertList.Add(sca);
                            }
                            else
                            {
                                if (!string.IsNullOrEmpty("" + row.Cells[3].Value))
                                {
                                    MsgBox.Show("系統內並無社團：" + row.Cells[3].Value + "");
                                }
                            }
                        }
                        else
                        {
                            //修改了,但是又改回原社團
                        }
                    }
                    else //如果是空的,就是原先無參與社團記錄
                    {
                        if (CourseDic.ContainsKey("" + row.Cells[3].Value)) //真的有這個社團
                        {
                            JHCourseRecord course = CourseDic["" + row.Cells[3].Value]; //取得課程
                            JHSCAttendRecord sca = new JHSCAttendRecord();
                            sca.RefCourseID = course.ID;
                            sca.RefStudentID = student.ID;
                            InsertList.Add(sca);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty("" + row.Cells[3].Value))
                            {
                                MsgBox.Show("系統內並無社團：" + row.Cells[3].Value + "");
                            }
                            //沒有此社團則不新增修課記錄
                        }
                    }
                }

                if (DeleteList.Count > 0)
                {
                    sb.AppendLine("原參與社團移出作業：");
                    //Log用
                    foreach (JHSCAttendRecord sca in DeleteList)
                    {
                        sb.AppendLine("學生「" + sca.Student.Name + "」已經自「" + sca.Course.Name + "」社團移出。");
                    }

                    try
                    {
                        JHSCAttend.Delete(DeleteList);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("刪除原社團參與記錄發生問題。");
                        return;
                    }
                }

                if (InsertList.Count > 0)
                {
                    sb.AppendLine("學生加入社團作業：");
                    //Log用
                    foreach (JHSCAttendRecord sca in InsertList)
                    {
                        sb.AppendLine("學生「" + sca.Student.Name + "」已經加入新社團「" + sca.Course.Name + "」。");
                    }

                    try
                    {
                        JHSCAttend.Insert(InsertList);
                    }
                    catch
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("新增社團參與記錄發生問題。");
                        return;
                    }
                }

                ApplicationLog.Log("社團模組.社團快速分組", "社團快速分組作業", sb.ToString());
                MsgBox.Show("已完成社團快速分組作業!!");
                this.Close();
            }
            else
            {
                MsgBox.Show("未進行任何變更!");
                this.Close();
            }
        }


        private int courseSortName(JHCourseRecord aCourse, JHCourseRecord bCourse)
        {
            return aCourse.Name.CompareTo(bCourse.Name);
        }

        private int studentSortSeatNo(JHStudentRecord aStudent, JHStudentRecord bStudent)
        {
            int inta = aStudent.SeatNo.HasValue ? aStudent.SeatNo.Value : 0;
            int intb = bStudent.SeatNo.HasValue ? bStudent.SeatNo.Value : 0;
            return inta.CompareTo(intb);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dataGridViewX1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == Column4.Index)
            {
                string now = "" + dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
                if (CourseDic.ContainsKey(now) || string.IsNullOrEmpty(now)) //真的有這個社團
                {
                    dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "";
                }
                else
                {
                    dataGridViewX1.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "查無此社團!!";
                }
            }
        }

        private void integerInput1_ValueChanged(object sender, EventArgs e)
        {
            labelX4.Visible = true;
            refsh.Pulse(10);
        }

        private void integerInput2_ValueChanged(object sender, EventArgs e)
        {
            labelX4.Visible = true;
            refsh.Pulse(10);
        }

        private void refsh_Click(object sender, EventArgs e)
        {
            labelX4.Visible = false;
            Reset();
        }
    }
}
