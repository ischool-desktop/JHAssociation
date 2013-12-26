using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using K12.Presentation;
using JHSchool.Data;
using FISCA.Presentation.Controls;
using FISCA.UDT;
using K12.Data.Configuration;
using Framework;
using System.Text.RegularExpressions;

namespace JHSchool.Association
{
    public partial class AssnAdmin : NLDPanel
    {
        //List<JHCourseRecord> AllAssn = new List<JHCourseRecord>();
        private BackgroundWorker BGW1 = new BackgroundWorker();

        private Dictionary<string, List<string>> AssociationCourseList = new Dictionary<string, List<string>>();

        private string FiltedSemester = School.DefaultSchoolYear + "學年度 第" + School.DefaultSemester + "學期";

        private Dictionary<string, JHCourseRecord> AssociationDic = new Dictionary<string, JHCourseRecord>();

        private Dictionary<string, JHCourseRecord> TempAssociationDic = new Dictionary<string, JHCourseRecord>();

        private List<JHCourseTagRecord> JHCourseTaglist = new List<JHCourseTagRecord>();

        //UDT物件
        private AccessHelper _accessHelper = new AccessHelper();
        private Dictionary<string, AssnAddress> AddressDic = new Dictionary<string, AssnAddress>();

        private bool isbusy = false;

        ListPaneField Field1; //名稱
        ListPaneField Field2; //老師 
        ListPaneField Field3; //上課地點 
        ListPaneField Field4; //評量名稱 
        ListPaneField Field6; //學年度
        ListPaneField Field7; //學期

        ListPaneField Field5; //人數

        Dictionary<string, int> CatchSCAttend = new Dictionary<string, int>();

        Dictionary<string, JHAssessmentSetupRecord> AssessmentDic = new Dictionary<string, JHAssessmentSetupRecord>();

        private MenuButton SearchAssociationName, SearchAssociationTeacher, SearchStudentName, SearchAddress;
        private SearchEventArgs SearEvArgs = null;

        public AssnAdmin()
        {
            Group = "高雄社團";

            #region 註冊事件
            BGW1.DoWork += new DoWorkEventHandler(BGW_DoWork);
            BGW1.RunWorkerCompleted += new RunWorkerCompletedEventHandler(BGW_RunWorkerCompleted);

            //課程與TAG關聯更新
            JHSchool.CourseExtendControls.Ribbon.CourseTagEvents.TagChanged += new EventHandler(CourseTagEvents_TagChanged);

            //Tag更新
            JHCourseTag.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(CourseTagEvents_TagChanged);
            JHCourseTag.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(CourseTagEvents_TagChanged);
            JHCourseTag.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(CourseTagEvents_TagChanged);

            //CourseTag.Instance.ItemUpdated += new EventHandler<Framework.ItemUpdatedEventArgs>(Instance_ItemUpdated);

            //課程更新
            JHCourse.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHCourse_AfterInsert); //Insert&Delete影響數量,所以整體更新
            JHCourse.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHCourse_AfterInsert);
            JHCourse.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHCourse_AfterInsert); //Update只影響課程資訊,所以只做畫面資訊更新
            Course.Instance.ItemUpdated += new EventHandler<Framework.ItemUpdatedEventArgs>(CourseInstance_ItemUpdated); //課程頁籤的更新就全部更新

            //修課記錄更新
            JHSCAttend.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHSCAttend_AfterUpdate);
            JHSCAttend.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHSCAttend_AfterUpdate);
            JHSCAttend.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHSCAttend_AfterUpdate);

            //課程評量更新
            JHAssessmentSetup.AfterDelete += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterDelete);
            JHAssessmentSetup.AfterInsert += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterDelete);
            JHAssessmentSetup.AfterUpdate += new EventHandler<K12.Data.DataChangedEventArgs>(JHAssessmentSetup_AfterDelete);

            //老師更新
            JHTeacher.AfterChange += new EventHandler<K12.Data.DataChangedEventArgs>(JHTeacher_AfterChange);
            //Teacher.Instance.ItemUpdated += new EventHandler<Framework.ItemUpdatedEventArgs>(TeacherInstance_ItemUpdated);

            //社團自我更新事件
            AssnEvents.AssnChanged += new EventHandler(AssnEvents_AssnChanged);

            //待處理更新
            this.TempSourceChanged += new EventHandler(Instance_TempSourceChanged);
            #endregion

            //=============================

            #region 社團名稱
            Field1 = new ListPaneField("社團名稱");
            Field1.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (AssociationDic.ContainsKey(e.Key))
                {
                    e.Value = AssociationDic[e.Key].Name;
                }
                else if (TempAssociationDic.ContainsKey(e.Key))
                {
                    e.Value = TempAssociationDic[e.Key].Name;
                }
            };
            this.AddListPaneField(Field1);
            #endregion

            #region 人數
            Field5 = new ListPaneField("人數");
            Field5.PreloadVariableBackground += delegate(object sender, PreloadVariableEventArgs e)
            {
                //收集人數資料
                CatchSCAttend.Clear();
                JHStudent.SelectAll();
                List<JHSCAttendRecord> list = new List<JHSCAttendRecord>();
                list = JHSCAttend.SelectByCourseIDs(e.Keys);
                foreach (JHSCAttendRecord each in list)
                {
                    if (each.Student.Status == K12.Data.StudentRecord.StudentStatus.一般)
                    {
                        if (!CatchSCAttend.ContainsKey(each.RefCourseID))
                        {
                            CatchSCAttend.Add(each.RefCourseID, 0);
                        }
                        CatchSCAttend[each.RefCourseID] += 1;
                    }
                }

            };
            Field5.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                //免得爆了...

                if (CatchSCAttend.ContainsKey(e.Key))
                {
                    e.Value = CatchSCAttend[e.Key];
                }
                else
                {
                    e.Value = 0;
                }

            };
            this.AddListPaneField(Field5);
            #endregion

            #region 上課地點
            Field3 = new ListPaneField("上課地點");
            Field3.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (AddressDic.ContainsKey(e.Key))
                {
                    e.Value = AddressDic[e.Key].Address;
                }
            };
            this.AddListPaneField(Field3);
            #endregion

            #region 指導老師
            Field2 = new ListPaneField("指導老師");
            Field2.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (AssociationDic.ContainsKey(e.Key))
                {
                    if (AssociationDic[e.Key].MajorTeacherNickname == "")
                    {
                        e.Value = AssociationDic[e.Key].MajorTeacherName;
                    }
                    else
                    {
                        e.Value = AssociationDic[e.Key].MajorTeacherName + "(" + AssociationDic[e.Key].MajorTeacherNickname + ")";
                    }
                }
                else if (TempAssociationDic.ContainsKey(e.Key))
                {
                    if (TempAssociationDic[e.Key].MajorTeacherNickname == "")
                    {
                        e.Value = TempAssociationDic[e.Key].MajorTeacherName;
                    }
                    else
                    {
                        e.Value = TempAssociationDic[e.Key].MajorTeacherName + "(" + TempAssociationDic[e.Key].MajorTeacherNickname + ")";
                    }
                }
            };
            this.AddListPaneField(Field2);
            #endregion

            #region 評量設定
            Field4 = new ListPaneField("評量設定");
            Field4.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (AssociationDic.ContainsKey(e.Key))
                {
                    if (AssessmentDic.ContainsKey(AssociationDic[e.Key].RefAssessmentSetupID))
                    {
                        e.Value = AssessmentDic[AssociationDic[e.Key].RefAssessmentSetupID].Name;
                    }
                    else
                    {
                        e.Value = "";
                    }
                }
                else if (TempAssociationDic.ContainsKey(e.Key))
                {
                    if (AssessmentDic.ContainsKey(TempAssociationDic[e.Key].RefAssessmentSetupID))
                    {
                        e.Value = AssessmentDic[TempAssociationDic[e.Key].RefAssessmentSetupID].Name;
                    }
                    else
                    {
                        e.Value = "";
                    }
                }
            };
            this.AddListPaneField(Field4);
            #endregion

            #region 學年度
            Field6 = new ListPaneField("學年度");
            Field6.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (AssociationDic.ContainsKey(e.Key))
                {
                    if (AssociationDic[e.Key].SchoolYear.HasValue)
                    {
                        e.Value = AssociationDic[e.Key].SchoolYear.Value.ToString();
                    }
                    else
                    {
                        e.Value = "";
                    }
                }
                else if (TempAssociationDic.ContainsKey(e.Key))
                {
                    if (TempAssociationDic[e.Key].SchoolYear.HasValue)
                    {
                        e.Value = TempAssociationDic[e.Key].SchoolYear.Value.ToString();
                    }
                    else
                    {
                        e.Value = "";
                    }
                }
            };
            this.AddListPaneField(Field6);
            #endregion

            #region 學期
            Field7 = new ListPaneField("學期");
            Field7.GetVariable += delegate(object sender, GetVariableEventArgs e)
            {
                if (AssociationDic.ContainsKey(e.Key))
                {
                    if (AssociationDic[e.Key].Semester.HasValue)
                    {
                        e.Value = AssociationDic[e.Key].Semester.Value.ToString();
                    }
                    else
                    {
                        e.Value = "";
                    }
                }
                else if (TempAssociationDic.ContainsKey(e.Key))
                {
                    if (TempAssociationDic[e.Key].Semester.HasValue)
                    {
                        e.Value = TempAssociationDic[e.Key].Semester.Value.ToString();
                    }
                    else
                    {
                        e.Value = "";
                    }
                }
            };
            this.AddListPaneField(Field7);
            #endregion


            //當篩選被PopupOpen時
            FilterMenu.SupposeHasChildern = true;
            FilterMenu.PopupOpen += new EventHandler<PopupOpenEventArgs>(FilterMenu_PopupOpen);
            
            //預設顯示目前學年度學期
            FilterMenu.Text = FiltedSemester;

            //0706 -暫時移除
            //SetFilterSource(FiltedSemester);        

            //Search
            Framework.ConfigData cd = User.Configuration["AssociationSearchOptionPreference"];
            SearchAssociationName = SetSearchButton("社團名稱", "SearchAssociationName", cd);
            SearchAssociationTeacher = SetSearchButton("指導老師", "SearchTeacherName", cd);
            SearchStudentName = SetSearchButton("參與學生", "SearchStudentName", cd);
            SearchAddress = SetSearchButton("上課地點", "SearchAddress", cd);
            this.Search += new EventHandler<SearchEventArgs>(AssnAdmin_Search);

            BGW1.RunWorkerAsync(); //取得 社團 & 聯課活動 的課程
        }

        /// <summary>
        /// 設定可搜尋欄位
        /// </summary>
        private MenuButton SetSearchButton(string MenuName, string BoolMenuName, Framework.ConfigData cd)
        {
            MenuButton SearchName = SearchConditionMenu[MenuName];
            SearchName.AutoCheckOnClick = true;
            SearchName.AutoCollapseOnClick = false;
            SearchName.Checked = cd.GetBoolean(BoolMenuName, true);
            SearchName.Click += delegate
            {
                cd.SetBoolean(BoolMenuName, SearchName.Checked);
                BackgroundWorker async = new BackgroundWorker();
                async.DoWork += delegate(object sender, DoWorkEventArgs e) { (e.Argument as Framework.ConfigData).Save(); };
                async.RunWorkerAsync(cd);
            };
            return SearchName;
        }

        /// <summary>
        /// 當Filter,被Popup時的事件
        /// </summary>
        void FilterMenu_PopupOpen(object sender, PopupOpenEventArgs e)
        {
            if (!BGW1.IsBusy)
            {
                foreach (string item in AssociationCourseList.Keys)
                {
                    MenuButton mb = e.VirtualButtons[item];
                    mb.AutoCheckOnClick = true;
                    mb.AutoCollapseOnClick = true;
                    mb.Checked = (item == FiltedSemester);
                    mb.Tag = item;
                    mb.CheckedChanged += delegate(object sender1, EventArgs e1)
                    {
                        MenuButton mb1 = sender1 as MenuButton;
                        AssociationInsert(mb1.Text);
                        FiltedSemester = FilterMenu.Text = mb1.Text;
                        mb1.Checked = true;
                    };
                }
            }
            else //如果忙碌中則提醒使用者
            {
                e.Cancel = true;
                e.VirtualButtons.Text = "資料下載中...";
                FISCA.Presentation.Controls.MsgBox.Show("資料下載中...\n請稍後再試");
            }
        }

        void AssnAdmin_Search(object sender, SearchEventArgs e)
        {
            SearEvArgs = e;
            Campus.Windows.BlockMessage.Display("資料搜尋中,請稍候....", new Campus.Windows.ProcessInvoker(ProcessSearch));
        }

        private void ProcessSearch(Campus.Windows.MessageArgs args)
        {
            List<string> results = new List<string>();
            Regex rx = new Regex(SearEvArgs.Condition, RegexOptions.IgnoreCase);

            if (SearchAssociationName.Checked)
            {
                foreach (JHCourseRecord each in JHCourse.SelectByIDs(AssociationCourseList[FiltedSemester]))
                {
                    string name = each.Name;
                    if (rx.Match(name).Success)
                    {
                        if (!results.Contains(each.ID))
                            results.Add(each.ID);
                    }
                }
            }
            if (SearchAssociationTeacher.Checked)
            {
                foreach (JHCourseRecord each in JHCourse.SelectByIDs(AssociationCourseList[FiltedSemester]))
                {
                    if (each.Teachers.Count != 0)
                    {
                        string name = each.Teachers[0].TeacherName;
                        if (rx.Match(name).Success)
                        {
                            if (!results.Contains(each.ID))
                                results.Add(each.ID);
                        }
                    }
                }
            }
            if (SearchStudentName.Checked)
            {
                foreach (JHSCAttendRecord SCA in JHSCAttend.SelectByCourseIDs(AssociationCourseList[FiltedSemester]))
                {
                    string name = SCA.Student.Name;
                    if (rx.Match(name).Success)
                    {
                        if (!results.Contains(SCA.RefCourseID))
                            results.Add(SCA.RefCourseID);
                    }
                }
            }
            if (SearchAddress.Checked)
            {
                //UDT物件
                AccessHelper _accessHelper = new AccessHelper();
                foreach (AssnAddress each in _accessHelper.Select<AssnAddress>())
                {
                    if (!AssociationCourseList[FiltedSemester].Contains(each.AssociationID))
                        continue;

                    string name = each.Address;
                    if (rx.Match(name).Success)
                    {
                        if (!results.Contains(each.AssociationID))
                            results.Add(each.AssociationID);
                    }
                }
            }

            SearEvArgs.Result.AddRange(results);
        }

        void AssnEvents_AssnChanged(object sender, EventArgs e)
        {
            if (BGW1.IsBusy)
            {
                isbusy = true;
            }
            else
            {
                BGW1.RunWorkerAsync();
            }
        }

        void Instance_TempSourceChanged(object sender, EventArgs e)
        {
            TempAssociationDic.Clear();

            foreach (string courseID in AssnAdmin.Instance.TempSource)
            {
                if (AssociationDic.ContainsKey(courseID))
                    TempAssociationDic.Add(courseID, AssociationDic[courseID]);
                else
                    TempAssociationDic.Add(courseID, JHCourse.SelectByID(courseID));
            }
        }

        #region 更新引發事件

        /// <summary>
        /// 教師更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void JHTeacher_AfterChange(object sender, K12.Data.DataChangedEventArgs e)
        {
            JHCourse.RemoveAll();
            ChengCourseDic(this.DisplaySource); //更新字典
            Field2.Reload();
        }

        /// <summary>
        /// 教師更新事件(new)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TeacherInstance_ItemUpdated(object sender, Framework.ItemUpdatedEventArgs e)
        {
            JHCourse.RemoveAll();
            ChengCourseDic(this.DisplaySource); //更新字典
            Field2.Reload();
        }

        /// <summary>
        /// 課程更新事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CourseInstance_ItemUpdated(object sender, Framework.ItemUpdatedEventArgs e)
        {
            List<JHCourseTagRecord> crList = JHCourseTag.SelectByCourseIDs(e.PrimaryKeys);
            foreach (JHCourseTagRecord each in crList)
            {
                if (each.Name == "社團" || each.Name == "聯課活動")
                {
                    if (BGW1.IsBusy)
                    {
                        isbusy = true;
                    }
                    else
                    {
                        BGW1.RunWorkerAsync();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// 課程更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //void JHCourse_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        //{
        //    JHCourse.RemoveAll();
        //    ChengCourseDic(this.DisplaySource);
        //    Field1.Reload();
        //    Field2.Reload();
        //    Field4.Reload();
        //    Field6.Reload();
        //    Field7.Reload();
        //}

        /// <summary>
        /// 課程新增/刪除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void JHCourse_AfterInsert(object sender, K12.Data.DataChangedEventArgs e)
        {

            List<JHCourseTagRecord> crList = JHCourseTag.SelectByCourseIDs(e.PrimaryKeys);
            foreach (JHCourseTagRecord each in crList)
            {
                if (each.Name == "社團" || each.Name == "聯課活動")
                {
                    if (BGW1.IsBusy)
                    {
                        isbusy = true;
                    }
                    else
                    {
                        BGW1.RunWorkerAsync();
                    }
                    break;
                }
            }
        }

        /// <summary>
        /// Tag更新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void CourseTagEvents_TagChanged(object sender, EventArgs e)
        {
            if (BGW1.IsBusy)
            {
                isbusy = true;
            }
            else
            {
                BGW1.RunWorkerAsync();
            }
        }

        void Instance_ItemUpdated(object sender, Framework.ItemUpdatedEventArgs e)
        {
            if (BGW1.IsBusy)
            {
                isbusy = true;
            }
            else
            {
                BGW1.RunWorkerAsync();
            }
        }

        /// <summary>
        /// 課程修課人數變更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void JHSCAttend_AfterUpdate(object sender, K12.Data.DataChangedEventArgs e)
        {
            Field5.Reload();
        }
        #endregion

        private void ChengCourseDic(List<string> list)
        {
            AddressDic.Clear();
            foreach (AssnAddress each in _accessHelper.Select<AssnAddress>())
            {
                if (list.Contains(each.AssociationID))
                {
                    if (!AddressDic.ContainsKey(each.AssociationID)) //避免上課地點重覆而產生錯誤
                    {
                        AddressDic.Add(each.AssociationID, each);
                    }
                    else
                    {

                    }
                }
            }
            #region 更新字典
            AssociationDic.Clear();
            foreach (JHCourseRecord each in JHCourse.SelectByIDs(list))
            {
                if (!AssociationDic.ContainsKey(each.ID))
                {
                    AssociationDic.Add(each.ID, each);
                }
            }
            #endregion
        }

        #region 課程評量更新
        void JHAssessmentSetup_AfterDelete(object sender, K12.Data.DataChangedEventArgs e)
        {
            ChengAssessment();
            Field4.Reload();
        }

        private void ChengAssessment()
        {
            AssessmentDic.Clear();
            List<JHAssessmentSetupRecord> list = JHAssessmentSetup.SelectAll();
            foreach (JHAssessmentSetupRecord each in list)
            {
                if (!AssessmentDic.ContainsKey(each.ID))
                {
                    AssessmentDic.Add(each.ID, each);
                }
            }
        }
        #endregion

        #region BGW

        void BGW_DoWork(object sender, DoWorkEventArgs e)
        {
            //取得社團評量清單
            ChengAssessment();

            #region 取得社團課程

            FISCA.Data.QueryHelper _queryHelper = new FISCA.Data.QueryHelper();
            StringBuilder sb = new StringBuilder();
            sb.Append("select tag_course.ref_course_id,course.course_name from tag_course ");
            sb.Append("join tag on tag_course.ref_tag_id=tag.id ");
            sb.Append("join course on course.id=tag_course.ref_course_id ");
            sb.Append("where tag.prefix LIKE '聯課活動' ");
            sb.Append("or tag.name LIKE '社團' ");
            DataTable dt = _queryHelper.Select(sb.ToString());
            List<string> courseIDList = new List<string>();
            foreach (DataRow row in dt.Rows)
            {
                string courseid = "" + row["ref_course_id"];
                if (!courseIDList.Contains(courseid))
                {
                    courseIDList.Add(courseid);
                }
            }
            #endregion

            #region 更新



            List<JHCourseRecord> AllAssn = JHCourse.SelectByIDs(courseIDList);

            AssociationCourseList.Clear();
            AssociationCourseList.Add("未分學年度學期", new List<string>());

            List<string> SortList = new List<string>();

            foreach (JHCourseRecord each in AllAssn)
            {
                if (each.SchoolYear.HasValue && each.Semester.HasValue)
                {
                    string save = each.SchoolYear.Value.ToString() + "學年度 第" + each.Semester.Value.ToString() + "學期";

                    if (!AssociationCourseList.ContainsKey(save))
                    {
                        //學年度學期名稱/當學期的社團課程ID清單
                        AssociationCourseList.Add(save, new List<string>());
                        //排序用清單
                        SortList.Add(save);
                    }
                    AssociationCourseList[save].Add(each.ID);
                }
                else //沒有學年度/學期
                {
                    AssociationCourseList["未分學年度學期"].Add(each.ID);
                }
            }

            SortList.Sort();

            Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();
            dic.Add("未分學年度學期", AssociationCourseList["未分學年度學期"]);
            foreach (string each in SortList)
            {
                dic.Add(each, AssociationCourseList[each]);
            }

            AssociationCourseList = dic;
            isbusy = false;
            #endregion
        }

        void BGW_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                FISCA.Presentation.Controls.MsgBox.Show("取得社團課程作業已被取消!!");
                return;
            }

            if (e.Error != null)
            {
                SmartSchool.ErrorReporting.ReportingService.ReportException(e.Error);
                FISCA.Presentation.Controls.MsgBox.Show("取得社團發生例外狀況!!");
                return;
            }


            if (isbusy)
            {
                BGW1.RunWorkerAsync();
            }
            else
            {
                AssociationInsert(FilterMenu.Text);

                //依社團名稱排一下
                Field1.Column.DataGridView.Sort(Field1.Column, ListSortDirection.Ascending);
            }
        }

        #endregion

        private void AssociationInsert(string NowSemester)
        {
            if (AssociationCourseList.ContainsKey(NowSemester))
            {
                ChengCourseDic(AssociationCourseList[NowSemester]); //更新字典

                //List<JHCourseRecord> listRecord = JHCourse.SelectByIDs(SemesterCourse[NowSemester]);
                //listRecord.Sort(new Comparison<JHCourseRecord>(SortCourse));
                //List<string> listString = new List<string>();
                //foreach (JHCourseRecord each in listRecord)
                //{
                //    listString.Add(each.ID);
                //}

                SetFilteredSource(AssociationCourseList[NowSemester]);
            }
            else
            {
                SetFilteredSource(new List<string>());
            }
        }

        private static AssnAdmin _AssnAdmin;

        public static AssnAdmin Instance
        {
            get
            {
                if (_AssnAdmin == null)
                {
                    _AssnAdmin = new AssnAdmin();
                }
                return _AssnAdmin;
            }
        }
    }
}
