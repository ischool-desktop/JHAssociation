using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using FISCA.Presentation;
using JHSchool.Data;

namespace JHSchool.Association
{
    public partial class ExtracurricularActivitiesView :NavView
    {
        public ExtracurricularActivitiesView()
        {
            InitializeComponent();

            NavText = "聯課活動檢視";
 
            SourceChanged += new EventHandler(ExtracurricularActivitiesView_SourceChanged);
        }

        //private Dictionary<DevComponents.AdvTree.Node, List<string>> items = new Dictionary<DevComponents.AdvTree.Node, List<string>>();

        void ExtracurricularActivitiesView_SourceChanged(object sender, EventArgs e)
        {                        
            advTree1.Nodes.Clear();

            DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            Node1.Text = "所有社團(" + Source.Count.ToString() + ")";
           
            advTree1.Nodes.Add(Node1);
            advTree1.SelectedNode = Node1;

            SetListPaneSource(Source, false, false);
            
            #region 舊的
            //DevComponents.AdvTree.Node Node1 = new DevComponents.AdvTree.Node();
            //Node1.Text = "所有社團(" + list.Count + ")";

            //Dictionary<string, List<DevComponents.AdvTree.Node>> dic = new Dictionary<string, List<DevComponents.AdvTree.Node>>();
            //dic.Add("未分學年度學期",new List<DevComponents.AdvTree.Node>());

            //foreach (JHCourseRecord each in list)
            //{                                    
            //    DevComponents.AdvTree.Node Node3 = new DevComponents.AdvTree.Node();
            //    Node3.Text = each.Name;

            //    if (each.SchoolYear.HasValue && each.Semester.HasValue)
            //    {
            //        string school = each.SchoolYear.Value + "學年度 第" + each.Semester.Value + "學期";

            //        if (!dic.ContainsKey(school))
            //        {
            //            dic.Add(school, new List<DevComponents.AdvTree.Node>());
            //        }

            //        Node3.Text = each.Name;
            //        dic[school].Add(Node3);
            //    }
            //    else
            //    {
            //        dic["未分學年度學期"].Add(Node3);
            //    }

            //}

            //foreach (string each1 in dic.Keys)
            //{
            //    DevComponents.AdvTree.Node Node4 = new DevComponents.AdvTree.Node();
            //    Node4.Text = each1;

            //    foreach (DevComponents.AdvTree.Node each2 in dic[each1])
            //    {
            //        Node4.Nodes.Add(each2);
            //    }

            //    advTree1.Nodes.Add(Node4);

            //} 
            #endregion
        }

        private void advTree1_NodeClick(object sender, DevComponents.AdvTree.TreeNodeMouseEventArgs e)
        {
            //判斷是否有按Control
            bool SelectedAll = (Control.ModifierKeys & Keys.Control) == Keys.Control;
            //判斷是否有按Shift
            bool AddToTemp = (Control.ModifierKeys & Keys.Shift) == Keys.Shift;
            //Source - 傳入ID
            //SelectedAll - 是否全選
            //AddToTemp - 是否加入待處理
            SetListPaneSource(Source, SelectedAll, AddToTemp);
        }
    }
}
