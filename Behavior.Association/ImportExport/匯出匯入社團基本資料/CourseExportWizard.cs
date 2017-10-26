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
            //���o�ץX����ƥN��
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
            _collection.Add(SetField("���Ψt�νs��", "ID"));
            _collection.Add(SetField("���ΦW��", "CourseName"));
            _collection.Add(SetField("�Ǧ~��", "SchoolYear"));
            _collection.Add(SetField("�Ǵ�", "Semester"));
            _collection.Add(SetField("���ɦѮv", "Teacher1Name"));
            //_collection.Add(SetField("�W�Ҧa�I", "AssnAddress"));
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
                FISCA.Presentation.Controls.MsgBox.Show("�����ܤֿ�ܤ@���ץX���!", "���ť�", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // 2017/10/26 �����ק�A��s�s��Aspose�A�䴩.xlsx�ɮת��פJ�ץX�C
            saveFileDialog1.Filter = "Excel (*.xlsx)|*.xlsx|Excel (*.xls)|*.xls|�Ҧ��ɮ� (*.*)|*.*";
            saveFileDialog1.FileName = "�ץX���ΰ򥻸��";
            if (saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                //���o�W�Ҧa�I
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



                List<JHCourseRecord> CourseList = JHCourse.SelectByIDs(AssnAdmin.Instance.SelectedSource); //���o���βM��
                CourseList.Sort(new Comparison<JHCourseRecord>(SortCourse));

                Workbook book = new Workbook();
                int ColumnExport = 0;
                Dictionary<string, int> ColumnPutValue = new Dictionary<string, int>();

                if (CourseExportName.Contains("���Ψt�νs��"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("���Ψt�νs��");
                    ColumnExport++;
                }

                if (CourseExportName.Contains("���ΦW��"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("���ΦW��");
                    ColumnExport++;
                }

                if (CourseExportName.Contains("�Ǧ~��"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("�Ǧ~��");
                    ColumnExport++;
                }
                if (CourseExportName.Contains("�Ǵ�"))
                {

                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("�Ǵ�");
                    ColumnExport++;
                }
                if (CourseExportName.Contains("���ɦѮv"))
                {
                    book.Worksheets[0].Cells[0, ColumnExport].PutValue("���ɦѮv");
                    ColumnExport++;
                }
                //if (CourseExportName.Contains("�W�Ҧa�I"))
                //{
                //    book.Worksheets[0].Cells[0, ColumnExport].PutValue("�W�Ҧa�I");
                //    ColumnExport++;
                //}

                int RowIndex = 1;
                int ColumnIndex = 0;
                foreach (JHCourseRecord course in CourseList)
                {
                    if (CourseExportName.Contains("���Ψt�νs��"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.ID);
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("���ΦW��"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.Name);
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("�Ǧ~��"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.SchoolYear.HasValue ? course.SchoolYear.Value.ToString() : "");
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("�Ǵ�"))
                    {
                        book.Worksheets[0].Cells[RowIndex, ColumnIndex].PutValue(course.Semester.HasValue ? course.Semester.Value.ToString() : "");
                        ColumnIndex++;
                    }
                    if (CourseExportName.Contains("���ɦѮv"))
                    {
                        //�B�z�ץX���ɦѮv�ɥ��]�t�ΦW
                        //�L�k�P����ƶi��פJ�����D
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
                    //if (CourseExportName.Contains("�W�Ҧa�I"))
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
                    FISCA.Presentation.Controls.MsgBox.Show("�}���ɮ׵o�ͥ���:" + ex.Message, "���~", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                this.Close();
            }
        }

        private FieldCollection GetSelectedFields() //�̿��������,�إ߲M��
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

        private void chkSelect_CheckedChanged(object sender, EventArgs e) //��Ŀ� ������� ��
        {
            foreach (ListViewItem item in listView.Items)
            {
                item.Checked = chkSelect.Checked;
            }
        }
    }
}