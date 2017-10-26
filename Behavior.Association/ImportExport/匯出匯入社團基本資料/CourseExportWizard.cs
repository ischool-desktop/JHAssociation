using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Xml;
//using SmartSchool.SmartPlugIn.Student.Export.RequestHandler.Formater;
//using SmartSchool.SmartPlugIn.Student.Export.RequestHandler;
//using SmartSchool.SmartPlugIn.Student.Export.ResponseHandler;
//using SmartSchool.SmartPlugIn.Student.Export.ResponseHandler.Output;
//using SmartSchool.SmartPlugIn.Student.Export.ResponseHandler.Connector;
using DevComponents.DotNetBar;
using System.Diagnostics;
using FISCA.Presentation.Controls;
using JHSchool.Legacy.Export.RequestHandler;
using JHSchool.Legacy.Export.ResponseHandler;
using JHSchool.Legacy.Export.ResponseHandler.Connector;
using JHSchool.Legacy.Export.RequestHandler.Formater;
using Framework;
using JHSchool.Legacy.Export.ResponseHandler.Output;
using JHSchool.Data;
using Aspose.Cells;
using FISCA.UDT;

namespace JHSchool.Association
{
    public partial class CourseExportWizard : BaseForm
    {
        List<string> CourseExportName = new List<string>();
        //Dictionary<string, AssnAddress> AssnAddressDic = new Dictionary<string, AssnAddress>();

        public CourseExportWizard()
        {
            InitializeComponent();
        }

        private void ExportForm_Load(object sender, EventArgs e)
        {
            //取得匯出之資料代號
            FieldCollection collection = Format();

            foreach (Field field in collection)
            {
                ListViewItem item = listView.Items.Add(field.DisplayText);
                item.Tag = field;
                item.Checked = true;
            }


        }

        private FieldCollection Format()
        {
            FieldCollection _collection = new FieldCollection();
            _collection.Add(SetField("社團系統編號", "ID"));
            _collection.Add(SetField("社團名稱", "CourseName"));
            _collection.Add(SetField("學年度", "SchoolYear"));
            _collection.Add(SetField("學期", "Semester"));
            _collection.Add(SetField("指導老師", "Teacher1Name"));
            //_collection.Add(SetField("上課地點", "AssnAddress"));
            return _collection;
        }

        private Field SetField(string x, string y)
        {
            Field field = new Field();
            field.FieldName = y;
            field.DisplayText = x;
            return field;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (GetSelectedFields().Count == 0)
            {
                FISCA.Presentation.Controls.MsgBox.Show("必須至少選擇一項匯出欄位!", "欄位空白", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 2017/10/26 羿均修改，更新新版Aspose，支援.xlsx檔案的匯入匯出。
            saveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls|所有檔案 (*.*)|*.*";
            saveFileDialog1.FileName = "匯出社團基本資料";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //取得上課地點
                //AssnAddressDic.Clear();
                //List<AssnAddress> AddressList = new List<AssnAddress>();
                //AccessHelper _a = new AccessHelper();
                //AddressList = _a.Select<AssnAddress>(string.Format("AssociationID in ('{0}')", string.Join("','", AssnAdmin.Instance.SelectedSource)));
                //foreach (AssnAddress each in AddressList)
                //{
                //    if (!AssnAddressDic.ContainsKey(each.AssociationID))
                //    {
                //        AssnAddressDic.Add(each.AssociationID, each);
                //    }
                //}



                List<JHCourseRecord> CourseList = JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource); //取得社團清單
                CourseList.Sort(new Comparison<JHCourseRecord>(SortCourse));

                Workbook book = new Workbook();
                int ColumnExport = 0;
                Dictionary<string, int> ColumnPutValue = new Dictionary<string, int>();

                if (CourseExportName.Contains("社團系統編號"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("社團系統編號");
                    ColumnExport++;
                }

                if (CourseExportName.Contains("社團名稱"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("社團名稱");
                    ColumnExport++;
                }

                if (CourseExportName.Contains("學年度"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("學年度");
                    ColumnExport++;
                }
                if (CourseExportName.Contains("學期"))
                {

                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("學期");
                    ColumnExport++;
                }
                if (CourseExportName.Contains("指導老師"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("指導老師");
                    ColumnExport++;
                }
                //if (CourseExportName.Contains("上課地點"))
                //{
                //    book.Worksheets[0].Cells[0, ColumnExport].PutValue("上課地點");
                //    ColumnExport++;
                //}

                int RowIndex = 1;
                int ColumnIndex = 0;
                foreach (JHCourseRecord course in CourseList)
                {
                    if (CourseExportName.Contains("社團系統編號"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.ID);
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("社團名稱"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.Name);
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("學年度"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.SchoolYear.HasValue ? course.SchoolYear.Value.ToString() : "");
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("學期"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.Semester.HasValue ? course.Semester.Value.ToString() : "");
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("指導老師"))
                    {
                        //處理匯出指導老師時未包含匿名
                        //無法同筆資料進行匯入的問題
                        //2011/2/18 by dylan
                        string TeacherName = "";
                        if (course.Teachers.Count != 0)
                        {
                            if (course.Teachers[0].TeacherNickname != "")
                                TeacherName = course.Teachers[0].TeacherName + "(" + course.Teachers[0].TeacherNickname + ")";
                            else
                                TeacherName = course.Teachers[0].TeacherName;
                        }
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(TeacherName);
                        ColumnIndex++;
                    }
                    //if (CourseExportName.Contains("上課地點"))
                    //{
                    //    if (AssnAddressDic.ContainsKey(course.ID))
                    //    {
                    //        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(AssnAddressDic[course.ID].Address);
                    //        ColumnIndex++;
                    //    }
                    //}
                    ColumnIndex = 0;
                    RowIndex++;
                }

                try
                {
                    book.Save(saveFileDialog1.FileName);
                    Process.Start(saveFileDialog1.FileName);
                }
                catch (Exception ex)
                {
                    FISCA.Presentation.Controls.MsgBox.Show("開啟檔案發生失敗:" + ex.Message, "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Close();
            }
        }

        private FieldCollection GetSelectedFields() //依選取的項目,建立清單
        {
            FieldCollection collection = new FieldCollection();
            foreach (ListViewItem item in listView.CheckedItems)
            {
                CourseExportName.Add(item.Text);
                Field field = item.Tag as Field;
                collection.Add(field);
            }
            return collection;
        }

        private int SortCourse(JHCourseRecord x, JHCourseRecord y)
        {
            string CourseNameX = x.Name;
            string CourseNameY = y.Name;
            return CourseNameX.CompareTo(CourseNameY);
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void chkSelect_CheckedChanged(object sender, EventArgs e) //當勾選 全部選取 時
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = chkSelect.Checked;
            }
        }
    }
}