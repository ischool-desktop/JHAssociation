using Aspose.Words;
using Aspose.Words.Drawing;
using FISCA.Presentation;
using FISCA.Presentation.Controls;
using K12.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ClubProofReport
{
    public partial class MainForm : BaseForm
    {
        BackgroundWorker BGW = new BackgroundWorker();
        private string CadreConfig = "ClubProofReport.cs";
        //主文件
        private Document _doc;
        //單頁範本
        private Document _template;
        // 入學照片
        Dictionary<string, string> _PhotoPDict = new Dictionary<string, string>();
        // 畢業照片
        Dictionary<string, string> _PhotoGDict = new Dictionary<string, string>();
        Dictionary<String,ClassRecord> _ClassDic; //classRecord字典,減少查詢次數

        public MainForm()
        {
            InitializeComponent();
            BGW.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);
        }

        private void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _doc.MailMerge.RemoveEmptyParagraphs = true;
            _doc.MailMerge.DeleteFields(); //刪除未使用的功能變數
            buttonX1.Enabled = true;

            try
            {
                SaveFileDialog SaveFileDialog1 = new SaveFileDialog();

                SaveFileDialog1.Filter = "Word (*.doc)|*.doc|所有檔案 (*.*)|*.*";
                SaveFileDialog1.FileName = "社團參與證明單";

                if (SaveFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    _doc.Save(SaveFileDialog1.FileName);
                    Process.Start(SaveFileDialog1.FileName);
                    MotherForm.SetStatusBarMessage("社團參與證明單,列印完成!!");
                }
                else
                {
                    FISCA.Presentation.Controls.MsgBox.Show("檔案未儲存");
                    return;
                }
            }
            catch
            {
                FISCA.Presentation.Controls.MsgBox.Show("檔案儲存錯誤,請檢查檔案是否開啟中!!");
                MotherForm.SetStatusBarMessage("檔案儲存錯誤,請檢查檔案是否開啟中!!");
            }
        }

        private void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            _doc = new Document();
            _doc.Sections.Clear(); //清空此Document

            #region 資料建立
            //學校名稱及校長
            String schoolName = K12.Data.School.ChineseName;
            string schoolMaster = K12.Data.School.Configuration["學校資訊"].PreviousData.SelectSingleNode("ChancellorChineseName").InnerText;
            //請先參考 FISCA.UDT.dll
            FISCA.Data.QueryHelper _Q = new FISCA.Data.QueryHelper();

            //畫面選取的學生ID清單
            List<string> StudentIDList = K12.Presentation.NLDPanels.Student.SelectedSource;

            //SQL
            StringBuilder sb = new StringBuilder();
            string StudentIDs = string.Join("','", StudentIDList);
            sb.Append(string.Format("StudentID in ('{0}')", StudentIDs));
            String tableName = "$jhschool.association.udt";
            //取得學生社團成績
            DataTable dt = _Q.Select(String.Format("select * from {0} where {1}", tableName, sb.ToString()));

            //建立字典存放各學生的社團record
            Dictionary<String, List<RecordObj>> StudentDic = new Dictionary<string, List<RecordObj>>();
            foreach (DataRow row in dt.Rows)
            {
                String studentid = row["studentid"].ToString();
                String schoolyear = row["schoolyear"].ToString();
                String semester = row["semester"].ToString();
                String content = row["scores"].ToString(); //此欄位為XML,需透過RecordObj去解析屬性

                if (!StudentDic.ContainsKey(studentid))
                {
                    StudentDic.Add(studentid, new List<RecordObj>());
                }
                StudentDic[studentid].Add(new RecordObj(studentid, schoolyear, semester, content));
            }

            //排序各學生的社團record
            foreach(String id in StudentDic.Keys)
            {
                StudentDic[id].Sort(SortRecordObj);
            }

            //取得StudentRecord清單,為個人資料用
            List<StudentRecord> studentRecords = K12.Data.Student.SelectByIDs(StudentDic.Keys.ToArray());
            
            //建立學生record字典方便以後查詢
            Dictionary<String, StudentRecord> studentRecordsDic = new Dictionary<string, StudentRecord>();
            //建立班級record字典方便以後查詢
            _ClassDic = new Dictionary<string, ClassRecord>();
            foreach (StudentRecord each in studentRecords) //依據被選取的學生所得到的StudentRecord清單建立各字典
            {
                if (!studentRecordsDic.ContainsKey(each.ID)) //沒有該學生ID就新增
                {
                    studentRecordsDic.Add(each.ID, each);  //用學生ID可取得StudentRecord
                }

                if (!_ClassDic.ContainsKey(each.RefClassID)) //沒有該班級ID就新增
                {
                    _ClassDic.Add(each.RefClassID, new ClassRecord()); //用班級ID可取得ClassRecord
                }
            }

            //以班級字典的key取得各自的ClassRecord
            List<ClassRecord> classList = K12.Data.Class.SelectByIDs(_ClassDic.Keys.ToList()); 
            foreach (ClassRecord classRecord in classList)
            {
                _ClassDic[classRecord.ID] = classRecord; //設定個別班級ID的ClassRecord
            }

            //排序StudentRecord清單
            studentRecords.Sort(studentRecordsSort);

            if (StudentDic.Count != 0)
            {
                // 入學照片
                _PhotoPDict.Clear();
                _PhotoPDict = K12.Data.Photo.SelectFreshmanPhoto(StudentDic.Keys.ToList());

                // 畢業照片
                _PhotoGDict.Clear();
                _PhotoGDict = K12.Data.Photo.SelectGraduatePhoto(StudentDic.Keys.ToList());
            }
            #endregion

            #region 資料列印
            //取得設定檔
            Campus.Report.ReportConfiguration ConfigurationInCadre = new Campus.Report.ReportConfiguration(CadreConfig);
            if (ConfigurationInCadre.Template == null)
            {
                //如果範本為空,則建立一個預設範本
                ConfigurationInCadre.Template = new Campus.Report.ReportTemplate(Properties.Resources.社團參與證明單, Campus.Report.TemplateType.Word);
            }
            _template = ConfigurationInCadre.Template.ToDocument();

            //開始列印
            foreach(StudentRecord each in studentRecords)
            {
                String id = each.ID; //學生ID
                Document PageOne = (Document)_template.Clone(true);
                PageOne.MailMerge.MergeField += new Aspose.Words.Reporting.MergeFieldEventHandler(MailMerge_MergeField);

                //合併欄位的參照字典,key=fieldName,value=fieldValue
                Dictionary<String, String> fieldDic = new Dictionary<string, string>();
                if (_PhotoPDict.ContainsKey(id))
                {
                    fieldDic.Add("新生照片1", _PhotoPDict[id]);
                    fieldDic.Add("新生照片2", _PhotoPDict[id]);
                }
                if (_PhotoGDict.ContainsKey(id))
                {
                    fieldDic.Add("畢業照片1", _PhotoGDict[id]);
                    fieldDic.Add("畢業照片2", _PhotoGDict[id]);
                }

                fieldDic.Add("班級", _ClassDic[each.RefClassID].Name);
                fieldDic.Add("座號", studentRecordsDic[id].SeatNo.ToString());
                fieldDic.Add("學號", studentRecordsDic[id].StudentNumber);
                fieldDic.Add("姓名", studentRecordsDic[id].Name);
                fieldDic.Add("學校名稱", schoolName);
                fieldDic.Add("校長", schoolMaster);

                int fieldindex = 0;
                foreach (RecordObj record in StudentDic[id])
                {
                    fieldindex++;
                    fieldDic.Add("學年度" + fieldindex, record.SchoolYear);
                    fieldDic.Add("學期" + fieldindex, record.Semester);
                    fieldDic.Add("社團" + fieldindex, record.ClubName);
                    fieldDic.Add("成績" + fieldindex, record.Score);
                    fieldDic.Add("評量" + fieldindex, record.Effort);
                    fieldDic.Add("文字描述" + fieldindex, record.Text);
                    PageOne.MailMerge.Execute(fieldDic.Keys.ToArray(), fieldDic.Values.ToArray());
                }
                _doc.Sections.Add(_doc.ImportNode(PageOne.FirstSection, true));
            }
            #endregion
        }

        //MailMerge事件
        private void MailMerge_MergeField(object sender, Aspose.Words.Reporting.MergeFieldEventArgs e)
        {
            if (e.FieldName == "新生照片1" || e.FieldName == "新生照片2")
            {
                if (!string.IsNullOrEmpty(e.FieldValue.ToString()))
                {
                    byte[] photo = Convert.FromBase64String(e.FieldValue.ToString()); //e.FieldValue as byte[];

                    if (photo != null && photo.Length > 0)
                    {
                        DocumentBuilder photoBuilder = new DocumentBuilder(e.Document);
                        photoBuilder.MoveToField(e.Field, true);
                        e.Field.Remove();
                        Shape photoShape = new Shape(e.Document, ShapeType.Image);
                        photoShape.ImageData.SetImage(photo);
                        photoShape.WrapType = WrapType.Inline;
                        if (e.FieldName == "新生照片1")
                        {
                            // 1吋
                            photoShape.Width = ConvertUtil.MillimeterToPoint(25);
                            photoShape.Height = ConvertUtil.MillimeterToPoint(35);
                        }
                        else
                        {
                            //2吋
                            photoShape.Width = ConvertUtil.MillimeterToPoint(35);
                            photoShape.Height = ConvertUtil.MillimeterToPoint(45);
                        }
                        photoBuilder.InsertNode(photoShape);
                    }
                }
            }
            else if (e.FieldName == "畢業照片1" || e.FieldName == "畢業照片2")
            {
                if (!string.IsNullOrEmpty(e.FieldValue.ToString()))
                {
                    byte[] photo = Convert.FromBase64String(e.FieldValue.ToString()); //e.FieldValue as byte[];

                    if (photo != null && photo.Length > 0)
                    {
                        DocumentBuilder photoBuilder = new DocumentBuilder(e.Document);
                        photoBuilder.MoveToField(e.Field, true);
                        e.Field.Remove();
                        Shape photoShape = new Shape(e.Document, ShapeType.Image);
                        photoShape.ImageData.SetImage(photo);
                        photoShape.WrapType = WrapType.Inline;
                        if (e.FieldName == "畢業照片1")
                        {
                            // 1吋
                            photoShape.Width = ConvertUtil.MillimeterToPoint(25);
                            photoShape.Height = ConvertUtil.MillimeterToPoint(35);
                        }
                        else
                        {
                            //2吋
                            photoShape.Width = ConvertUtil.MillimeterToPoint(35);
                            photoShape.Height = ConvertUtil.MillimeterToPoint(45);
                        }
                        photoBuilder.InsertNode(photoShape);
                    }
                }
            }
        }

        //RecordObj排序
        private int SortRecordObj(RecordObj x, RecordObj y)
        {
            String xx = x.SchoolYear.PadLeft(3, '0');
            xx += x.Semester.PadLeft(1, '0');

            String yy = y.SchoolYear.PadLeft(3, '0');
            yy += y.Semester.PadLeft(1, '0');

            return xx.CompareTo(yy);
        }

        //studentRecords資料排序
        private int studentRecordsSort(StudentRecord x, StudentRecord y)
        {
            string xx = _ClassDic[x.RefClassID].GradeYear.ToString().PadLeft(1, '0');   
            xx += _ClassDic[x.RefClassID].DisplayOrder.PadLeft(3, '0');
            xx += _ClassDic[x.RefClassID].Name.PadLeft(20, '0');            
            xx += x.SeatNo.ToString().PadLeft(3, '0');          
            xx += x.StudentNumber.PadLeft(20, '0');         
            xx += x.Name.PadLeft(10, '0');

            string yy = _ClassDic[y.RefClassID].GradeYear.ToString().PadLeft(1, '0');
            yy += _ClassDic[y.RefClassID].DisplayOrder.PadLeft(3, '0');
            yy += _ClassDic[y.RefClassID].Name.PadLeft(20, '0');
            yy += y.SeatNo.ToString().PadLeft(3, '0');
            yy += y.StudentNumber.PadLeft(20, '0');
            yy += y.Name.PadLeft(10, '0');

            return xx.CompareTo(yy);
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            buttonX1.Enabled = false;

            if (!BGW.IsBusy)
            {
                BGW.RunWorkerAsync();
            }
            else
            {
                MessageBox.Show("系統忙碌中...");
            }
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            //取得設定檔
            Campus.Report.ReportConfiguration ConfigurationInCadre = new Campus.Report.ReportConfiguration(CadreConfig);
            //畫面內容(範本內容,預設樣式
            Campus.Report.TemplateSettingForm TemplateForm;
            if (ConfigurationInCadre.Template != null)
            {
                TemplateForm = new Campus.Report.TemplateSettingForm(ConfigurationInCadre.Template, new Campus.Report.ReportTemplate(Properties.Resources.社團參與證明單, Campus.Report.TemplateType.Word));
            }
            else
            {
                ConfigurationInCadre.Template = new Campus.Report.ReportTemplate(Properties.Resources.社團參與證明單, Campus.Report.TemplateType.Word);
                TemplateForm = new Campus.Report.TemplateSettingForm(ConfigurationInCadre.Template, new Campus.Report.ReportTemplate(Properties.Resources.社團參與證明單, Campus.Report.TemplateType.Word));
            }

            //預設名稱
            TemplateForm.DefaultFileName = "社團參與證明單(範本)";
            //如果回傳為OK
            if (TemplateForm.ShowDialog() == DialogResult.OK)
            {
                //設定後樣試,回傳
                ConfigurationInCadre.Template = TemplateForm.Template;
                //儲存
                ConfigurationInCadre.Save();
            }
        }

        private void linkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "另存新檔";
            sfd.FileName = "社團參與證明_合併欄位總表.doc";
            sfd.Filter = "Word檔案 (*.doc)|*.doc|所有檔案 (*.*)|*.*";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    FileStream fs = new FileStream(sfd.FileName, FileMode.Create);
                    fs.Write(Properties.Resources.合併欄位總表, 0, Properties.Resources.合併欄位總表.Length);
                    fs.Close();
                    System.Diagnostics.Process.Start(sfd.FileName);
                }
                catch
                {
                    FISCA.Presentation.Controls.MsgBox.Show("指定路徑無法存取。", "另存檔案失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
        }
    }
}
