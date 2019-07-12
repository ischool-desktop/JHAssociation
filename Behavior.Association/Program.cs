using FISCA;
using FISCA.LogAgent;
using FISCA.Presentation;
using Framework;
using JHSchool.Association.CadreReport;
using JHSchool.Association.Properties;
using JHSchool.Association.SpecifyByName_2;
using System.Collections.Generic;
using System.Windows.Forms;

namespace JHSchool.Association
{
    public class Program
    {
        [MainMethod("JHSchool.Association")]

        static public void Main()
        {
            #region 主要

            //增加一個頁籤
            MotherForm.AddPanel(AssnAdmin.Instance);

            //UDT Setup
            FISCA.UDT.SchemaManager sch = new FISCA.UDT.SchemaManager(FISCA.Authentication.DSAServices.DefaultConnection);
            sch.SyncSchema(new ClubSchedule());
            sch.SyncSchema(new ClubSetting());

            //增加一個ListView
            AssnAdmin.Instance.AddView(new ExtracurricularActivitiesView());

            //基本資料
            FISCA.Permission.FeatureAce UserPermission;
            UserPermission = FISCA.Permission.UserAcl.Current["JHSchool.Association.Detail0010"];
            if (UserPermission.Editable || UserPermission.Viewable)
                AssnAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<AssnCourseItem>());

            //學生資訊
            UserPermission = FISCA.Permission.UserAcl.Current["JHSchool.Association.Detail0020"];
            if (UserPermission.Editable || UserPermission.Viewable)
                AssnAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<StudentDetailItem>());

            //社團幹部
            //AssnAdmin.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<CadreByAssnItem>());

            //學生結算資料項目
            UserPermission = FISCA.Permission.UserAcl.Current["JHSchool.Association.Detail0030"];
            if (UserPermission.Editable || UserPermission.Viewable)
                Student.Instance.AddDetailBulider(new FISCA.Presentation.DetailBulider<StudentSettleAccountsAssnItem>());

            AssnAdmin.Instance.ListPaneContexMenu["更新畫面資料"].Image = Resources.trainning_refresh_64;
            AssnAdmin.Instance.ListPaneContexMenu["更新畫面資料"].Click += delegate
            {
                AssnEvents.RaiseAssnChanged();
            };

            #endregion

            #region 社團Ribbon

            RibbonBarItem edit = AssnAdmin.Instance.RibbonBarItems["編輯"];
            #region 編輯
            edit["新增社團"].Size = RibbonBarButton.MenuButtonSize.Large;
            edit["新增社團"].Image = Resources.recreation_add_64;
            edit["新增社團"].Enable = User.Acl["JHSchool.Association.Ribbon0010"].Executable;
            edit["新增社團"].Click += delegate
            {
                AddCourse insert = new AddCourse();
                insert.ShowDialog();
            };

            edit["刪除社團"].Size = RibbonBarButton.MenuButtonSize.Large;
            edit["刪除社團"].Image = Resources.recreation_close_64;
            edit["刪除社團"].Enable = User.Acl["JHSchool.Association.Ribbon0010.5"].Executable;
            edit["刪除社團"].Click += delegate
            {
                //當你有選擇一個社團
                if (AssnAdmin.Instance.SelectedSource.Count == 1)
                {
                    //取得社團課程資料
                    JHSchool.Data.JHCourseRecord record = JHSchool.Data.JHCourse.SelectByID(AssnAdmin.Instance.SelectedSource[0]);
                    //取得學生
                    List<JHSchool.Data.JHSCAttendRecord> scattendList = JHSchool.Data.JHSCAttend.SelectByStudentIDAndCourseID(new List<string>() { }, new List<string>() { record.ID });
                    int attendStudentCount = 0;
                    //將一般狀態的學生計算出來
                    foreach (JHSchool.Data.JHSCAttendRecord scattend in scattendList)
                    {
                        if (scattend.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)
                            attendStudentCount++;
                    }

                    //如果大於0,則警告
                    if (attendStudentCount > 0)
                        MsgBox.Show(record.Name + " 有" + attendStudentCount.ToString() + "位社團參與學生，請先移除社團參與學生後再刪除社團.");
                    else
                    {
                        string msg = string.Format("確定要刪除「{0}」？", record.Name);
                        if (MsgBox.Show(msg, "刪除社團", MessageBoxButtons.YesNo) == DialogResult.Yes)
                        {
                            #region 自動刪除非一般學生的修課記錄
                            List<JHSchool.Data.JHSCAttendRecord> deleteSCAttendList = new List<JHSchool.Data.JHSCAttendRecord>();
                            foreach (JHSchool.Data.JHSCAttendRecord scattend in scattendList)
                            {
                                JHSchool.Data.JHStudentRecord stuRecord = JHSchool.Data.JHStudent.SelectByID(scattend.RefStudentID);
                                if (stuRecord == null) continue;
                                if (stuRecord.Status != K12.Data.StudentRecord.StudentStatus.一般)
                                    deleteSCAttendList.Add(scattend);
                            }

                            //List<string> studentIDs = new List<string>();
                            //foreach (JHSchool.Data.JHSCAttendRecord scattend in deleteSCAttendList)
                            //    studentIDs.Add(scattend.RefStudentID);
                            //學生成績也移除
                            //List<JHSchool.Data.JHSCETakeRecord> sceList = JHSchool.Data.JHSCETake.SelectByStudentAndCourse(studentIDs, new List<string>() { record.ID });
                            //JHSchool.Data.JHSCETake.Delete(sceList);

                            //移除學生課程關連
                            JHSchool.Data.JHSCAttend.Delete(deleteSCAttendList);
                            #endregion

                            JHSchool.Data.JHCourse.Delete(record);
                            //更新課程畫面
                            Course.Instance.SyncDataBackground(record.ID);

                            AssnEvents.RaiseAssnChanged();

                            ApplicationLog.Log("社團外掛模組", "刪除社團", "course", record.ID, "社團「" + record.Name + "」已被刪除!!");
                        }
                        else
                            return;
                    }
                }
                else if (AssnAdmin.Instance.SelectedSource.Count == 0)
                {
                    MsgBox.Show("僅能一次刪除一個社團!!");
                }
                else
                {
                    MsgBox.Show("您必須選擇一個社團!!");
                }
            };
            #endregion

            RibbonBarItem Print = AssnAdmin.Instance.RibbonBarItems["資料統計"];

            #region 匯出及匯入

            //社團基本資料
            Print["匯出"].Size = RibbonBarButton.MenuButtonSize.Large;
            Print["匯出"].Image = Properties.Resources.Export_Image;
            Print["匯出"]["匯出社團基本資料"].Enable = User.Acl["JHSchool.Association.Ribbon0080"].Executable;
            Print["匯出"]["匯出社團基本資料"].Click += delegate
            {
                new CourseExportWizard().ShowDialog();
            };

            Print["匯入"].Size = RibbonBarButton.MenuButtonSize.Large;
            Print["匯入"].Image = Properties.Resources.Import_Image;
            Print["匯入"]["匯入社團基本資料"].Enable = User.Acl["JHSchool.Association.Ribbon0090"].Executable;
            Print["匯入"]["匯入社團基本資料"].Click += delegate
            {
                //MsgBox.Show("功能未完成");
                new AssociationImportWizard().ShowDialog();
            };

            //社團參與學生
            RibbonBarItem rbItemCourseImportExport = AssnAdmin.Instance.RibbonBarItems["資料統計"];
            //RibbonBarItem rbItemCourseImportExport = FISCA.Presentation.MotherForm.RibbonBarItems["社團作業", "資料統計"];
            rbItemCourseImportExport["匯出"]["匯出社團參與學生"].Enable = User.Acl["JHSchool.Association.Ribbon0100"].Executable;
            rbItemCourseImportExport["匯出"]["匯出社團參與學生"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Evaluation.ImportExport.Course.ExportCourseStudents("社團");
                JHSchool.Evaluation.ImportExport.Course.ExportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.Course.ExportStudentV2(exporter.Text, exporter.Image, "社團", AssnAdmin.Instance.SelectedSource);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };
            rbItemCourseImportExport["匯入"]["匯入社團參與學生"].Enable = User.Acl["JHSchool.Association.Ribbon0110"].Executable;
            rbItemCourseImportExport["匯入"]["匯入社團參與學生"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Evaluation.ImportExport.Course.ImportCourseStudents("社團");
                JHSchool.Evaluation.ImportExport.Course.ImportStudentV2 wizard = new JHSchool.Evaluation.ImportExport.Course.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            RibbonBarButton rbItemExport = Student.Instance.RibbonBarItems["資料統計"]["匯出"];
            RibbonBarButton rbItemImport = Student.Instance.RibbonBarItems["資料統計"]["匯入"];

            //社團活動表現(成績)
            rbItemExport["學務相關匯出"]["匯出社團活動表現"].Enable = User.Acl["JHSchool.Student.Ribbon0165"].Executable;
            rbItemExport["學務相關匯出"]["匯出社團活動表現"].Click += delegate
            {
                SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Association.ImportExport.ExportAssnCode();
                JHSchool.Behavior.ImportExport.ExportStudentV2 wizard = new JHSchool.Behavior.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
                exporter.InitializeExport(wizard);
                wizard.ShowDialog();
            };

            rbItemImport["學務相關匯入"]["匯入社團活動表現"].Enable = User.Acl["JHSchool.Student.Ribbon0166"].Executable;
            rbItemImport["學務相關匯入"]["匯入社團活動表現"].Click += delegate
            {
                SmartSchool.API.PlugIn.Import.Importer importer = new JHSchool.Association.ImportExport.ImportAssnCode();
                JHSchool.Behavior.ImportExport.ImportStudentV2 wizard = new JHSchool.Behavior.ImportExport.ImportStudentV2(importer.Text, importer.Image);
                importer.InitializeImport(wizard);
                wizard.ShowDialog();
            };

            //rbItemExport["匯出活動表現紀錄(健文)"].Click += delegate
            //{
            //    SmartSchool.API.PlugIn.Export.Exporter exporter = new JHSchool.Association.ImportExport.匯出表現資料();
            //    JHSchool.Behavior.ImportExport.ExportStudentV2 wizard = new JHSchool.Behavior.ImportExport.ExportStudentV2(exporter.Text, exporter.Image);
            //    exporter.InitializeExport(wizard);
            //    wizard.ShowDialog();
            //};
            //rbItemExport["匯出活動表現紀錄對照表(健文)"].Click += (sender, e) =>
            //{
            //    FISCA.UDT.AccessHelper helper = new FISCA.UDT.AccessHelper();
            //    List<JHSchool.Association.UDT.活動表現紀錄對照表> records = helper.Select<JHSchool.Association.UDT.活動表現紀錄對照表>();
            //    Aspose.Cells.Workbook work = new Aspose.Cells.Workbook();
            //    work.Worksheets[0].Cells[0,0].PutValue("細項");
            //    work.Worksheets[0].Cells[0,1].PutValue("類別");
            //    work.Worksheets[0].Cells[0,2].PutValue("數量");
            //    for (int i = 0; i < records.Count; i++)
            //    {
            //        work.Worksheets[0].Cells[i+1,0].PutValue(records[i].細項);
            //        work.Worksheets[0].Cells[i+1,1].PutValue(records[i].類別);
            //        work.Worksheets[0].Cells[i+1,2].PutValue(records[i].數量);
            //    }
            //    JHSchool.Association.Report.Utility.Completed("活動表現紀錄對照表", work);
            //};
            #endregion

            #region 報表
            //Print["報表"]["社團點名表"].Enable = (AssnAdmin.Instance.SelectedSource.Count > 0 && User.Acl["JHBehavior.Course.Ribbon0210"].Executable);
            Print["報表"].Size = RibbonBarButton.MenuButtonSize.Large;
            Print["報表"].Image = Resources.paste_64;
            //Print["報表"]["社團點名表"].Enable = User.Acl["JHSchool.Association.Report0010"].Executable;
            //Print["報表"]["社團點名表"].Click += delegate
            //{
            //    ClubActivitiesForm ClubActivities = new ClubActivitiesForm();
            //    ClubActivities.ShowDialog();
            //};

            Print["報表"]["社團點名表(編號)"].Enable = User.Acl["JHSchool.Association.Report0010"].Executable;
            Print["報表"]["社團點名表(編號)"].Click += delegate
            {
                ClubActivitiesForm_2 ClubActivities = new ClubActivitiesForm_2();
                ClubActivities.ShowDialog();
            };

            Print["報表"]["社團修課清單"].Enable = User.Acl["JHSchool.Association.Report0020"].Executable;
            Print["報表"]["社團修課清單"].Click += delegate
            {
                //直接列印沒有ShowDialog
                StudentAsson AssonList = new StudentAsson();
            };

            Print["報表"]["社團成績校對單"].Enable = User.Acl["JHSchool.Association.Report0030"].Executable;
            Print["報表"]["社團成績校對單"].Click += delegate
            {
                //直接列印沒有ShowDialog
                CollationClass AssonList = new CollationClass();
            };

            Print["報表"]["社團幹部總表"].Enable = User.Acl["K12.Societies.Report00060.9"].Executable;
            Print["報表"]["社團幹部總表"].Click += delegate
            {
                //直接列印沒有ShowDialog
                SocietiesLeaders SummaryTable = new SocietiesLeaders();
            };

            #endregion

            #region 成績

            RibbonBarItem Results = AssnAdmin.Instance.RibbonBarItems["成績"];
            Results["社團評量設定"].Size = RibbonBarButton.MenuButtonSize.Small;
            Results["社團評量設定"].Image = Resources.physical_science_lock_64;
            Results["社團評量設定"].Enable = User.Acl["JHSchool.Association.Ribbon0030"].Executable;
            Results["社團評量設定"].Click += delegate
            {
                AssnEvaluationConfig editor = new AssnEvaluationConfig();
                editor.ShowDialog();
            };

            Results["社團成績輸入"].Size = RibbonBarButton.MenuButtonSize.Small;
            Results["社團成績輸入"].Image = Resources.photoshop_write_64;
            Results["社團成績輸入"].Enable = User.Acl["JHSchool.Association.Ribbon0020"].Executable;
            Results["社團成績輸入"].Click += delegate
            {
                if (AssnAdmin.Instance.SelectedSource.Count == 1)
                {
                    AssnCourseResults editor = new AssnCourseResults(AssnAdmin.Instance.SelectedSource[0]);
                    editor.ShowDialog();
                }
            };

            //RibbonBarItem SetResults = AssnAdmin.Instance.RibbonBarItems["結算"];
            Results["社團成績期末結算"].Size = RibbonBarButton.MenuButtonSize.Small;
            Results["社團成績期末結算"].Image = Resources.conver_to_text_reload_128;
            Results["社團成績期末結算"].Enable = User.Acl["JHSchool.Association.Ribbon0060"].Executable;
            Results["社團成績期末結算"].Click += delegate
            {
                SettleAccountsForm settle = new SettleAccountsForm();
                settle.ShowDialog();
            };
            #endregion

            #region 資料檢查與管理
            RibbonBarItem Check = AssnAdmin.Instance.RibbonBarItems["批次作業/檢查"];

            Check["變更學生參與社團"].Size = RibbonBarButton.MenuButtonSize.Medium;
            Check["變更學生參與社團"].Image = Resources.add_row_128;
            Check["變更學生參與社團"].Enable = User.Acl["JHSchool.Association.Ribbon00120"].Executable;
            Check["變更學生參與社團"].Click += delegate
            {

                ChangeForm changeF = new ChangeForm();
                changeF.ShowDialog();
            };

            Check["社團幹部登錄"].Image = Resources.stamp_paper_fav_128;
            Check["社團幹部登錄"].Size = RibbonBarButton.MenuButtonSize.Medium;
            Check["社團幹部登錄"].Enable = User.Acl["JHSchool.Association.Ribbon00145.1"].Executable;
            Check["社團幹部登錄"].Click += delegate
            {
                new AssociationByClassSeanNo().ShowDialog();
            };

            Check["社團評量輸入檢查"].Image = Resources.corel_info_128;
            Check["社團評量輸入檢查"].Size = RibbonBarButton.MenuButtonSize.Small;
            Check["社團評量輸入檢查"].Enable = User.Acl["JHSchool.Association.Ribbon0070"].Executable;
            Check["社團評量輸入檢查"].Click += delegate
            {
                AssonResultsCheck AssnRC = new AssonResultsCheck();
                AssnRC.ShowDialog();
            };

            Check["未參與社團檢查"].Size = RibbonBarButton.MenuButtonSize.Small;
            Check["未參與社團檢查"].Image = Resources.blur_info_128;
            Check["未參與社團檢查"].Enable = User.Acl["JHSchool.Association.Ribbon0040"].Executable;
            Check["未參與社團檢查"].Click += delegate
            {
                NotHaveToParticipateForm NotParticipateForm = new NotHaveToParticipateForm();
                NotParticipateForm.ShowDialog();
            };

            Check["重覆參與社團檢查"].Size = RibbonBarButton.MenuButtonSize.Small;
            Check["重覆參與社團檢查"].Image = Resources.animation_info_128;
            Check["重覆參與社團檢查"].Enable = User.Acl["JHSchool.Association.Ribbon0050"].Executable;
            Check["重覆參與社團檢查"].Click += delegate
            {
                ReiterateAssnForm ReiterateForm = new ReiterateAssnForm();
                ReiterateForm.ShowDialog();
            };

            RibbonBarItem Orda = AssnAdmin.Instance.RibbonBarItems["其它"];

            Orda["上課地點管理"].Size = RibbonBarButton.MenuButtonSize.Medium;
            Orda["上課地點管理"].Image = Resources.architecture_64;
            Orda["上課地點管理"].Enable = User.Acl["JHSchool.Association.Ribbon00130"].Executable;
            Orda["上課地點管理"].Click += delegate
            {
                AssnAddressForm AAressF = new AssnAddressForm();
                DialogResult dr = AAressF.ShowDialog();
                if (dr == DialogResult.Yes)
                {
                    AssnEvents.RaiseAssnChanged();
                }

            };

            Orda["社團歷程"].Size = RibbonBarButton.MenuButtonSize.Medium;
            Orda["社團歷程"].Image = Resources.folder_zoom_64;
            Orda["社團歷程"].Enable = User.Acl["JHSchool.Association.Ribbon00140"].Executable;
            Orda["社團歷程"].Click += delegate
            {
                new ViewForm("course", AssnAdmin.Instance.SelectedSource).ShowDialog();
            };

            #endregion

            //2019/7/10 - 社團時間表
            RibbonBarItem setup = AssnAdmin.Instance.RibbonBarItems["設定"];
            setup["社團時間表"].Size = RibbonBarButton.MenuButtonSize.Large;
            setup["社團時間表"].Enable = true; //權限
            setup["社團時間表"].Click += delegate
            {
                new ClubSectionDetail().ShowDialog();
            };

            Results["社團成績輸入"].Enable = false;
            Results["社團成績期末結算"].Enable = false;
            Check["社團評量輸入檢查"].Enable = false;
            Check["變更學生參與社團"].Enable = false;
            Orda["社團歷程"].Enable = false;
            Check["社團幹部登錄"].Enable = false;

            AssnAdmin.Instance.SelectedSourceChanged += delegate
            {
                Results["社團成績輸入"].Enable = (AssnAdmin.Instance.SelectedSource.Count == 1 && User.Acl["JHSchool.Association.Ribbon0020"].Executable);
                Results["社團成績期末結算"].Enable = (AssnAdmin.Instance.SelectedSource.Count >= 1 && User.Acl["JHSchool.Association.Ribbon0060"].Executable);
                Check["社團評量輸入檢查"].Enable = (AssnAdmin.Instance.SelectedSource.Count >= 1 && User.Acl["JHSchool.Association.Ribbon0070"].Executable);
                Check["變更學生參與社團"].Enable = (AssnAdmin.Instance.SelectedSource.Count == 1 && User.Acl["JHSchool.Association.Ribbon00120"].Executable);
                Orda["社團歷程"].Enable = (AssnAdmin.Instance.SelectedSource.Count >= 1 && User.Acl["JHSchool.Association.Ribbon00140"].Executable);
                Check["社團幹部登錄"].Enable = (AssnAdmin.Instance.SelectedSource.Count == 1 && User.Acl["JHSchool.Association.Ribbon00145.1"].Executable);
            };

            #endregion

            #region 班級RibbonBar

            RibbonBarItem ClassPort = Class.Instance.RibbonBarItems["資料統計"];
            ClassPort["報表"]["社團相關報表"]["班級社團分組名單"].Enable = User.Acl["JHSchool.Association.Report0040"].Executable;
            ClassPort["報表"]["社團相關報表"]["班級社團分組名單"].Click += delegate
            {
                ParticipateForm ClassPart = new ParticipateForm();
                ClassPart.ShowDialog();
            };

            RibbonBarItem ClassBarBtn = Class.Instance.RibbonBarItems["學務"];
            ClassBarBtn["社團快速分組"].Image = Properties.Resources.distibute_vertical_center_refresh_64;
            ClassBarBtn["社團快速分組"].Enable = User.Acl["JHSchool.Association.ClassAssnDivide"].Executable && Class.Instance.SelectedList.Count == 1;
            ClassBarBtn["社團快速分組"].Click += delegate
            {
                ClassAssnDivide Part1 = new ClassAssnDivide();
                Part1.ShowDialog();
            };

            //班級選擇變更時
            Class.Instance.SelectedListChanged += delegate
            {
                ClassBarBtn["社團快速分組"].Enable = User.Acl["JHSchool.Association.ClassAssnDivide"].Executable && Class.Instance.SelectedList.Count == 1;
            };

            #endregion

            #region 權限

            //班級報表
            Framework.Security.Catalog ClassreportRibbon = Framework.Security.RoleAclSource.Instance["班級"]["報表"];
            ClassreportRibbon.Add(new Framework.Security.ReportFeature("JHSchool.Association.Report0040", "班級社團分組名單"));

            Framework.Security.Catalog ClassRibbon = Framework.Security.RoleAclSource.Instance["班級"]["功能按鈕"];
            ClassRibbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.ClassAssnDivide", "社團快速分組"));

            Framework.Security.Catalog ribbon = Framework.Security.RoleAclSource.Instance["社團作業"]["功能按鈕"];
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0010", "新增社團"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0010.5", "刪除社團"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0020", "社團成績輸入"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0030", "社團評量設定"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0040", "未參與社團檢查"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0050", "重覆參與社團檢查"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0060", "社團成績期末結算"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0070", "社團評量輸入檢查"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon00120", "變更學生參與社團"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon00130", "上課地點管理"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon00140", "社團歷程"));
            ribbon.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon00145.1", "社團幹部登錄"));

            Framework.Security.Catalog detail = Framework.Security.RoleAclSource.Instance["社團作業"]["資料項目"];
            detail.Add(new Framework.Security.DetailItemFeature("JHSchool.Association.Detail0010", "社團基本資料"));
            detail.Add(new Framework.Security.DetailItemFeature("JHSchool.Association.Detail0020", "社員清單"));
            //detail.Add(new Framework.Security.DetailItemFeature("Behavior.TheCadre.Detail00050", "社團幹部"));

            Framework.Security.Catalog reportRibbon = Framework.Security.RoleAclSource.Instance["社團作業"]["報表"];
            reportRibbon.Add(new Framework.Security.ReportFeature("JHSchool.Association.Report0010", "社團點名表"));
            reportRibbon.Add(new Framework.Security.ReportFeature("JHSchool.Association.Report0020", "社團修課清單"));
            reportRibbon.Add(new Framework.Security.ReportFeature("JHSchool.Association.Report0030", "社團成績校對單"));
            reportRibbon.Add(new Framework.Security.ReportFeature("K12.Societies.Report00060.9", "社團幹部總表"));

            //new
            Framework.Security.Catalog ribbon2 = Framework.Security.RoleAclSource.Instance["社團作業"]["功能按鈕"];
            ribbon2.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0080", "匯出社團基本資料"));
            ribbon2.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0100", "匯出社團參與學生"));

            Framework.Security.Catalog ribbon3 = Framework.Security.RoleAclSource.Instance["社團作業"]["功能按鈕"];
            ribbon3.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0090", "匯入社團基本資料"));
            ribbon3.Add(new Framework.Security.RibbonFeature("JHSchool.Association.Ribbon0110", "匯入社團參與學生"));

            Framework.Security.Catalog Studentdetail = Framework.Security.RoleAclSource.Instance["學生"]["資料項目"];
            Studentdetail.Add(new Framework.Security.DetailItemFeature("JHSchool.Association.Detail0030", "社團成績_高雄"));
            Framework.Security.Catalog sribbon1 = Framework.Security.RoleAclSource.Instance["學生"]["功能按鈕"];
            sribbon1.Add(new Framework.Security.RibbonFeature("JHSchool.Student.Ribbon0165", "匯出社團活動表現"));
            Framework.Security.Catalog sribbon2 = Framework.Security.RoleAclSource.Instance["學生"]["功能按鈕"];
            sribbon2.Add(new Framework.Security.RibbonFeature("JHSchool.Student.Ribbon0166", "匯入社團活動表現"));
            #endregion
        }
    }
}