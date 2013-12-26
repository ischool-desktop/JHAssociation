using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation.Controls;
using K12.Data.Configuration;
using JHSchool.Data;
using FISCA.DSAUtil;
using System.Xml;
using FISCA.LogAgent;
using Framework;

namespace JHSchool.Association
{
    public partial class AssnCourseResults : BaseForm
    {
        private JHCourseRecord _Course;
        //更新清單
        private Dictionary<string, DataGridViewRow> DicEditRow = new Dictionary<string, DataGridViewRow>();

        private SCAItem sc;
        //成績記錄
        private List<JHAEIncludeRecord> JHAEincludeList = new List<JHAEIncludeRecord>();
        //努力程度操作物件
        private EffortMapper EDT = new EffortMapper();

        private JHAEIncludeRecord _AEIncludeRecord = new JHAEIncludeRecord();
        //學生的ROW
        private Dictionary<string, int> StudIndexList = new Dictionary<string, int>();
        /// <summary>
        /// 社團評量
        /// </summary>
        public AssnCourseResults(string CourseID)
        {
            InitializeComponent();

            //取得課程Record
            _Course = JHCourse.SelectByID(CourseID);

            //取得課程修課學生
            sc = new SCAItem(CourseID);

            txtHelp.Text = "社團：" + _Course.Name + "　指導老師：" + _Course.MajorTeacherName + "　學生人數：" + sc.Students.Count;
        }

        private void AssonCourseResults_Load(object sender, EventArgs e)
        {
            //取得評量設定值,並建立畫面
            if (SelectCourseSetup()) //建立功為True
            {
                //取得修課學生,填入學生基本資料
                SelectStudents();

                //取得修課成績(如果有),填入相對應學生欄位
                SelectCourseResults();
            }
            else
            {
                this.Close();
            }
        }

        /// <summary>
        /// 取得社團評量項目設定值
        /// </summary>
        private bool SelectCourseSetup()
        {
            #region 取得社團評量項目設定值
            JHAssessmentSetupRecord Assessment = _Course.AssessmentSetup;
            if (_Course.AssessmentSetup == null)
            {
                FISCA.Presentation.Controls.MsgBox.Show("未指定社團評量");
                return false;
            }
            else
            {
                if (Assessment.Name != "社團評量(社團模組)")
                {
                    FISCA.Presentation.Controls.MsgBox.Show("本課程使用：[" + Assessment.Name + "]評量\n請指定為[社團評量(社團模組)]!");
                    return false;
                }
            }

            //取得評量試別
            JHAEincludeList.Clear();
            JHAEincludeList = JHAEInclude.SelectByAssessmentSetupID(_Course.RefAssessmentSetupID);
            if (JHAEincludeList.Count > 1)
            {
                DialogResult dr = FISCA.Presentation.Controls.MsgBox.Show("社團評量(社團模組)樣板有多個試別設定\n預設[社團成績]必須輸入於[社團評量]之試別!!\n您是否要手動選擇試別?\n\n(按下[否]將會自動掃瞄並取得社團評量)", MessageBoxButtons.YesNo, MessageBoxDefaultButton.Button2);
                if (dr == DialogResult.Yes)
                {
                    CourseResultsDio crd = new CourseResultsDio(JHAEincludeList);
                    crd.ShowDialog();
                    if (crd.Include != null)
                    {
                        _AEIncludeRecord = crd.Include;
                    }
                    else
                    {
                        FISCA.Presentation.Controls.MsgBox.Show("您未選擇試別!!\n畫面即將關閉!");
                        return false;
                    }
                }
                else
                {
                    foreach (JHAEIncludeRecord each in JHAEincludeList)
                    {
                        if (each.ExamName == "社團評量")
                        {
                            _AEIncludeRecord = each;
                            break;
                        }
                    }
                }
            }
            else
            {
                //取得"社團評量(社團模組)"評量樣板內,第一個"社團評量"樣版
                foreach (JHAEIncludeRecord each in JHAEincludeList)
                {
                    if (each.ExamName == "社團評量")
                    {
                        _AEIncludeRecord = each;
                        break;
                    }
                }
            }

            if (_AEIncludeRecord.AssessmentSetup == null)
            {
                FISCA.Presentation.Controls.MsgBox.Show("社團評量(社團模組)樣板內,並無[社團評量]試別名稱!!");

                return false;
            }


            if (_AEIncludeRecord.UseScore)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Name = "ColumnUseScore";
                column.HeaderText = "成績";
                column.Width = 90;
                dgvStudents.Columns.Add(column);
            }
            if (_AEIncludeRecord.UseEffort)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Name = "ColumnUseEffort";
                column.HeaderText = "努力程度";
                column.Width = 120;
                dgvStudents.Columns.Add(column);
            }
            if (_AEIncludeRecord.UseText)
            {
                DataGridViewColumn column = new DataGridViewTextBoxColumn();
                column.Name = "ColumnUseText";
                column.HeaderText = "文字評量";
                column.Width = 300;
                dgvStudents.Columns.Add(column);
            }

            return true;
            #endregion
        }

        Dictionary<string, string> DicLog = new Dictionary<string, string>();

        private void SelectStudents()
        {
            #region 修課學生建立清單
            foreach (JHStudentRecord each in sc.Students)
            {
                DicLog.Add(each.ID, "學生：「" + each.Name + "」");

                DataGridViewRow row = new DataGridViewRow();
                row.CreateCells(dgvStudents); //建立Row樣式
                row.Cells[0].Value = each.ID;

                if (each.Class != null)
                {
                    row.Cells[1].Value = each.Class.Name;
                }
                if (each.SeatNo.HasValue)
                {
                    row.Cells[2].Value = each.SeatNo.Value;
                }
                row.Cells[3].Value = each.Name;
                row.Cells[4].Value = each.StudentNumber;
                StudIndexList.Add(each.ID, dgvStudents.Rows.Add(row));
            }

            #endregion
        }

        private void SelectCourseResults()
        {
            //**************
            //用這一段是專刪特定編號"12"的評量內容
            //把所有課程取出,依評量編號取得評量內容
            //最後再全部刪掉
            //List<string> list = new List<string>();
            //foreach (JHCourseRecord each in JHCourse.SelectAll())
            //{
            //    list.Add(each.ID);
            //}
            //List<JHSCETakeRecord> SCETakeList1111 = JHSCETake.SelectByCourseAndExam(list, "12");
            //***************

            #region 修課成績
            //取得課程成績

            List<string> studentList = new List<string>();
            foreach (JHStudentRecord each in sc.Students)
            {
                studentList.Add(each.ID);
            }

            List<JHSCETakeRecord> SCETakeList = JHSCETake.SelectByStudentAndCourse(studentList, new List<string>() { _Course.ID });
            List<JHSCETakeRecord> ChengSCETakeList = new List<JHSCETakeRecord>();
            foreach (JHSCETakeRecord each in SCETakeList)
            {
                if (each.RefExamID == _AEIncludeRecord.RefExamID)
                {
                    ChengSCETakeList.Add(each);
                }
            }


            //JHSCETakeRecord轉型為AssnScoreRecord
            List<AssnScoreRecord> MyAssnScore = ChengSCETakeList.AsKHJHSCETakeRecords();

            foreach (AssnScoreRecord each in MyAssnScore)
            {
                JHCourseRecord cr = JHCourse.SelectByID(each.RefCourseID);

                if (DicLog.ContainsKey(each.RefStudentID))
                {
                    DicLog[each.RefStudentID] += "社團課程：「" + cr.Name + "」\n原有資料狀態：\n";

                    if (_AEIncludeRecord.UseScore)
                    {

                        DicLog[each.RefStudentID] += "成績：「" + each.Score + "」\n";
                        dgvStudents.Rows[StudIndexList[each.RefStudentID]].Cells["ColumnUseScore"].Value = each.Score;
                    }
                    if (_AEIncludeRecord.UseEffort)
                    {
                        DicLog[each.RefStudentID] += "努力程度：「" + each.Effort + "」\n";
                        dgvStudents.Rows[StudIndexList[each.RefStudentID]].Cells["ColumnUseEffort"].Value = each.Effort;
                    }
                    if (_AEIncludeRecord.UseText)
                    {
                        DicLog[each.RefStudentID] += "文字描述：「" + each.Text + "」\n";
                        dgvStudents.Rows[StudIndexList[each.RefStudentID]].Cells["ColumnUseText"].Value = each.Text;
                    }
                    dgvStudents.Rows[StudIndexList[each.RefStudentID]].Tag = each;
                }
            }

            //取得(Get)
            //List<JHSCETakeRecord> takes = JHSCETake.SelectAll();
            //List<AssnScoreRecord> mytakes = takes.AsKHJHSCETakeRecords();

            //新增(Insert)
            //AssnScoreRecord r = new AssnScoreRecord(new JHSCETakeRecord());
            //r.RefSCAttendID = "1";
            //r.RefExamID = "10";
            //r.Score = 99;
            //r.Effort = 1;
            //r.Text = "";

            //r.AsJHSCETakeRecord();

            //List<JHSCETakeRecord> saveTakes = mytakes.AsJHSCETakeRecords();

            #endregion
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (DataGridViewErrodCheck())
            {
                FISCA.Presentation.Controls.MsgBox.Show("資料內容有誤");
                return;
            }

            btnSave.Enabled = false;

            #region 儲存
            List<JHSCETakeRecord> InsertList = new List<JHSCETakeRecord>();
            List<JHSCETakeRecord> UpdataList = new List<JHSCETakeRecord>();

            //刪除
            List<JHSCETakeRecord> DeleteList = new List<JHSCETakeRecord>();

            foreach (DataGridViewRow each in DicEditRow.Values)
            {
                if (each.Tag is AssnScoreRecord)
                {
                    #region 修改
                    AssnScoreRecord assnScore = (AssnScoreRecord)each.Tag;

                    DicLog[assnScore.RefStudentID] += "修改後資料狀態：\n";

                    if (_AEIncludeRecord.UseScore) //成績
                    {
                        decimal OutDecimal = 0;
                        if (decimal.TryParse("" + each.Cells["ColumnUseScore"].Value, out OutDecimal))
                        {
                            DicLog[assnScore.RefStudentID] += "成績：「" + OutDecimal.ToString() + "」\n";
                            assnScore.Score = OutDecimal;
                        }
                        else
                        {
                            DicLog[assnScore.RefStudentID] += "成績：「」\n";
                            assnScore.Score = null;
                        }
                    }
                    if (_AEIncludeRecord.UseEffort) //努力程度
                    {
                        int OutInt = 0;
                        if (int.TryParse("" + each.Cells["ColumnUseEffort"].Value, out OutInt))
                        {
                            DicLog[assnScore.RefStudentID] += "努力程度：「" + OutInt.ToString() + "」\n";
                            assnScore.Effort = OutInt;
                        }
                        else
                        {
                            DicLog[assnScore.RefStudentID] += "努力程度：「」\n";
                            assnScore.Effort = null;
                        }
                    }
                    if (_AEIncludeRecord.UseText) //文字描述
                    {
                        assnScore.Text = "" + each.Cells["ColumnUseText"].Value;
                        DicLog[assnScore.RefStudentID] += "文字描述：「" + assnScore.Text + "」";
                    }

                    //當文字描述,成績,努力程度皆為空,則刪除此成績資料)
                    if (assnScore.Text.Trim() == "" && !assnScore.Effort.HasValue && !assnScore.Score.HasValue)
                    {
                        DeleteList.Add(assnScore.AsJHSCETakeRecord());
                    }
                    else
                    {
                        UpdataList.Add(assnScore.AsJHSCETakeRecord());
                    }


                    #endregion
                }
                else //新增
                {
                    #region 新增
                    AssnScoreRecord assnScore = new AssnScoreRecord(new JHSCETakeRecord());
                    string studentID = "" + each.Cells[0].Value;
                    assnScore.RefStudentID = studentID;
                    assnScore.RefCourseID = _Course.ID;
                    assnScore.RefExamID = _AEIncludeRecord.RefExamID;
                    assnScore.RefSCAttendID = sc.SCADic[studentID].ID;

                    JHCourseRecord cr = JHCourse.SelectByID(_Course.ID);
                    DicLog[assnScore.RefStudentID] += "社團課程：「" + cr.Name + "」\n";

                    if (_AEIncludeRecord.UseScore) //成績
                    {
                        if ("" + each.Cells["ColumnUseScore"].Value != "")
                        {
                            assnScore.Score = decimal.Parse("" + each.Cells["ColumnUseScore"].Value);
                            DicLog[studentID] += "成績：「" + each.Cells["ColumnUseScore"].Value + "」\n";
                        }
                    }
                    if (_AEIncludeRecord.UseEffort) //努力程度
                    {
                        if ("" + each.Cells["ColumnUseEffort"].Value != "")
                        {
                            assnScore.Effort = int.Parse("" + each.Cells["ColumnUseEffort"].Value);
                            DicLog[studentID] += "努力程度：「" + each.Cells["ColumnUseEffort"].Value + "」\n";
                        }
                    }
                    if (_AEIncludeRecord.UseText) //文字描述
                    {
                        if ("" + each.Cells["ColumnUseText"].Value != "")
                        {
                            assnScore.Text = "" + each.Cells["ColumnUseText"].Value;
                            DicLog[studentID] += "文字描述：「" + each.Cells["ColumnUseText"].Value + "」\n";
                        }
                    }
                    InsertList.Add(assnScore.AsJHSCETakeRecord());
                    #endregion
                }
            }

            if (InsertList.Count > 0)
            {
                try
                {
                    JHSCETake.Insert(InsertList);

                    foreach (JHSCETakeRecord each in InsertList)
                    {
                        ApplicationLog.Log("社團外掛模組", "新增社團課程成績", "student", each.RefStudentID, DicLog[each.RefStudentID]);
                    }
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("新增資料產生錯誤" + ex.Message);
                    btnSave.Enabled = true;
                    return;
                }
            }
            if (UpdataList.Count > 0)
            {
                try
                {
                    JHSCETake.Update(UpdataList);
                    foreach (JHSCETakeRecord each in UpdataList)
                    {
                        ApplicationLog.Log("社團外掛模組", "更新社團課程成績", "student", each.RefStudentID, DicLog[each.RefStudentID]);
                    }
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("更新資料產生錯誤" + ex.Message);
                    btnSave.Enabled = true;
                    return;
                }
            }

            if (DeleteList.Count > 0)
            {
                try
                {
                    JHSCETake.Delete(DeleteList);
                    foreach (JHSCETakeRecord each in DeleteList)
                    {
                        ApplicationLog.Log("社團外掛模組", "刪除社團課程成績", "student", each.RefStudentID, "學生「" + each.Student.Name + "」刪除「" + each.Course.Name + "」社團成績");
                    }
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("更新資料產生錯誤" + ex.Message);
                    btnSave.Enabled = true;
                    return;
                }
            }

            FISCA.Presentation.Controls.MsgBox.Show("儲存成功");
            this.Close();
            #endregion
        }

        private bool DataGridViewErrodCheck()
        {
            foreach (DataGridViewRow row in dgvStudents.Rows)
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

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dgvStudents_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //整理出更新清單
            string StudID = (string)dgvStudents.CurrentRow.Cells[0].Value;
            if (!DicEditRow.ContainsKey(StudID))
            {
                DicEditRow.Add(StudID, dgvStudents.CurrentRow);
            }
            else
            {
                DicEditRow[StudID] = dgvStudents.CurrentRow;
            }

            if (dgvStudents.CurrentCell.OwningColumn.Name == "ColumnUseScore")
            {
                #region 成績檢查
                string CurrText = "" + dgvStudents.CurrentCell.Value;

                //清空資料的話
                if (CurrText == "")
                {
                    dgvStudents.CurrentCell.ErrorText = "";
                    return;
                }

                decimal CurrInt = 101;
                if (decimal.TryParse(CurrText, out CurrInt))
                {
                    dgvStudents.CurrentCell.ErrorText = "";
                }
                else
                {
                    dgvStudents.CurrentCell.ErrorText = "輸入資料必須為數字";
                    return;
                }

                if (CurrInt >= 0 && CurrInt <= 100)
                {
                    dgvStudents.CurrentCell.ErrorText = "";
                }
                else
                {
                    dgvStudents.CurrentCell.ErrorText = "輸入資料必須是0~100之數字";
                    return;
                }
                #endregion

                #region 努力程度替換

                //如果有努力程度
                int Effortindex = 0;
                foreach (DataGridViewColumn each in dgvStudents.Columns)
                {
                    if (each.Name == "ColumnUseEffort")
                    {
                        Effortindex = each.Index;
                        break;
                    }
                }
                if (Effortindex != 0)
                {
                    dgvStudents.CurrentRow.Cells[Effortindex].Value = EDT.GetCodeByScore(CurrInt);
                }


                #endregion
            }
            else if (dgvStudents.CurrentCell.OwningColumn.Name == "ColumnUseEffort")
            {
                #region 努力程度檢查
                //替換成文字
                string CurrText = "" + dgvStudents.CurrentCell.Value;

                if (CurrText == "") //未輸入資料(正確)
                {
                    dgvStudents.CurrentCell.ErrorText = "";
                    return;
                }

                int CurrInt = 101;
                if (int.TryParse(CurrText, out CurrInt)) //輸入數字(正確)
                {
                    if (EDT.CheckCode(CurrInt)) //輸入努力程度範圍(正確)
                    {
                        dgvStudents.CurrentCell.ErrorText = "";
                        dgvStudents.CurrentCell.Value = CurrInt;
                    }
                    else //輸入範圍外(錯誤)
                    {
                        dgvStudents.CurrentCell.ErrorText = "輸入努力程度以外範圍";
                    }
                }
                else //未輸入數字(錯誤)
                {
                    dgvStudents.CurrentCell.ErrorText = "努力程度目前僅提供輸入數字型態";
                }

                //EDT
                #endregion
            }
            else if (dgvStudents.CurrentCell.OwningColumn.Name == "ColumnUseText")
            {
                //不動作
            }
        }

        private void dgvStudents_CurrentCellDirtyStateChanged(object sender, EventArgs e)
        {
            if (dgvStudents.CurrentCell.OwningColumn.Name == "ColumnUseEffort") //努力程度欄位
            {
                if (dgvStudents.CurrentCell.RowIndex + 1 < dgvStudents.Rows.Count) //下方還有Row
                {
                    //CurrentCell移動為下一行
                    dgvStudents.CurrentCell = dgvStudents.Rows[dgvStudents.CurrentCell.RowIndex + 1].Cells[dgvStudents.CurrentCell.ColumnIndex];
                }
            }
        }

    }
}
