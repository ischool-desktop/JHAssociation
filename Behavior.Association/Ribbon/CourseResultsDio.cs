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

namespace JHSchool.Association
{
    public partial class CourseResultsDio : BaseForm
    {
        public JHAEIncludeRecord Include { get; set; }

        private List<JHAEIncludeRecord> _list { get; set; }

        public CourseResultsDio(List<JHAEIncludeRecord> list)
        {
            InitializeComponent();

            _list = list;
            foreach (JHAEIncludeRecord each in _list)
            {
                comboBoxEx1.Items.Add(each.ExamName);
            }
        }

        private void buttonX1_Click(object sender, EventArgs e)
        {
            if (comboBoxEx1.SelectedItem != null)
            {
                foreach (JHAEIncludeRecord each in _list)
                {
                    if (each.ExamName == comboBoxEx1.Text)
                    {
                        Include = each;
                    }
                }
            }
            this.Close();
        }

        private void buttonX2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
