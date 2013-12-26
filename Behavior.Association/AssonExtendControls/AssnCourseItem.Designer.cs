namespace JHSchool.Association
{
    partial class AssnCourseItem
    {
        /// <summary> 
        /// 設計工具所需的變數。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 清除任何使用中的資源。
        /// </summary>
        /// <param name="disposing">如果應該處置 Managed 資源則為 true，否則為 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 元件設計工具產生的程式碼

        /// <summary> 
        /// 此為設計工具支援所需的方法 - 請勿使用程式碼編輯器修改這個方法的內容。
        ///
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.txtCourseName = new DevComponents.DotNetBar.Controls.TextBoxX();
            this.lbCourseName = new DevComponents.DotNetBar.LabelX();
            this.lbCourseTeacher1 = new DevComponents.DotNetBar.LabelX();
            this.cbxSchoolYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lbSchoolYear = new DevComponents.DotNetBar.LabelX();
            this.lbSemester = new DevComponents.DotNetBar.LabelX();
            this.cbxSemeter = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.errorProvider1 = new System.Windows.Forms.ErrorProvider(this.components);
            this.cbxTeacher = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lbCourseAddress = new DevComponents.DotNetBar.LabelX();
            this.cbxEAddress = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.lLink = new System.Windows.Forms.LinkLabel();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).BeginInit();
            this.SuspendLayout();
            // 
            // txtCourseName
            // 
            // 
            // 
            // 
            this.txtCourseName.Border.Class = "TextBoxBorder";
            this.txtCourseName.Location = new System.Drawing.Point(127, 20);
            this.txtCourseName.Name = "txtCourseName";
            this.txtCourseName.Size = new System.Drawing.Size(125, 25);
            this.txtCourseName.TabIndex = 0;
            // 
            // lbCourseName
            // 
            this.lbCourseName.AutoSize = true;
            this.lbCourseName.Location = new System.Drawing.Point(63, 22);
            this.lbCourseName.Name = "lbCourseName";
            this.lbCourseName.Size = new System.Drawing.Size(60, 21);
            this.lbCourseName.TabIndex = 1;
            this.lbCourseName.Text = "社團名稱";
            // 
            // lbCourseTeacher1
            // 
            this.lbCourseTeacher1.AutoSize = true;
            this.lbCourseTeacher1.Location = new System.Drawing.Point(63, 58);
            this.lbCourseTeacher1.Name = "lbCourseTeacher1";
            this.lbCourseTeacher1.Size = new System.Drawing.Size(60, 21);
            this.lbCourseTeacher1.TabIndex = 2;
            this.lbCourseTeacher1.Text = "指導老師";
            // 
            // cbxSchoolYear
            // 
            this.cbxSchoolYear.DisplayMember = "Text";
            this.cbxSchoolYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSchoolYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSchoolYear.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.cbxSchoolYear.FormattingEnabled = true;
            this.cbxSchoolYear.ItemHeight = 19;
            this.cbxSchoolYear.Location = new System.Drawing.Point(363, 20);
            this.cbxSchoolYear.Name = "cbxSchoolYear";
            this.cbxSchoolYear.Size = new System.Drawing.Size(125, 25);
            this.cbxSchoolYear.TabIndex = 4;
            // 
            // lbSchoolYear
            // 
            this.lbSchoolYear.AutoSize = true;
            this.lbSchoolYear.Location = new System.Drawing.Point(315, 22);
            this.lbSchoolYear.Name = "lbSchoolYear";
            this.lbSchoolYear.Size = new System.Drawing.Size(47, 21);
            this.lbSchoolYear.TabIndex = 5;
            this.lbSchoolYear.Text = "學年度";
            // 
            // lbSemester
            // 
            this.lbSemester.AutoSize = true;
            this.lbSemester.Location = new System.Drawing.Point(328, 58);
            this.lbSemester.Name = "lbSemester";
            this.lbSemester.Size = new System.Drawing.Size(34, 21);
            this.lbSemester.TabIndex = 6;
            this.lbSemester.Text = "學期";
            // 
            // cbxSemeter
            // 
            this.cbxSemeter.DisplayMember = "Text";
            this.cbxSemeter.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxSemeter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbxSemeter.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.cbxSemeter.FormattingEnabled = true;
            this.cbxSemeter.ItemHeight = 19;
            this.cbxSemeter.Location = new System.Drawing.Point(363, 56);
            this.cbxSemeter.Name = "cbxSemeter";
            this.cbxSemeter.Size = new System.Drawing.Size(125, 25);
            this.cbxSemeter.TabIndex = 7;
            // 
            // errorProvider1
            // 
            this.errorProvider1.ContainerControl = this;
            // 
            // cbxTeacher
            // 
            this.cbxTeacher.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cbxTeacher.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cbxTeacher.DisplayMember = "Text";
            this.cbxTeacher.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxTeacher.FormattingEnabled = true;
            this.cbxTeacher.ItemHeight = 19;
            this.cbxTeacher.Location = new System.Drawing.Point(127, 56);
            this.cbxTeacher.Name = "cbxTeacher";
            this.cbxTeacher.Size = new System.Drawing.Size(125, 25);
            this.cbxTeacher.TabIndex = 14;
            // 
            // lbCourseAddress
            // 
            this.lbCourseAddress.AutoSize = true;
            this.lbCourseAddress.Location = new System.Drawing.Point(299, 94);
            this.lbCourseAddress.Name = "lbCourseAddress";
            this.lbCourseAddress.Size = new System.Drawing.Size(60, 21);
            this.lbCourseAddress.TabIndex = 15;
            this.lbCourseAddress.Text = "上課地點";
            // 
            // cbxEAddress
            // 
            this.cbxEAddress.DisplayMember = "Text";
            this.cbxEAddress.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbxEAddress.FormattingEnabled = true;
            this.cbxEAddress.ItemHeight = 19;
            this.cbxEAddress.Location = new System.Drawing.Point(363, 92);
            this.cbxEAddress.Name = "cbxEAddress";
            this.cbxEAddress.Size = new System.Drawing.Size(125, 25);
            this.cbxEAddress.TabIndex = 16;
            // 
            // lLink
            // 
            this.lLink.AutoSize = true;
            this.lLink.BackColor = System.Drawing.Color.Transparent;
            this.lLink.Location = new System.Drawing.Point(376, 128);
            this.lLink.Name = "lLink";
            this.lLink.Size = new System.Drawing.Size(112, 17);
            this.lLink.TabIndex = 17;
            this.lLink.TabStop = true;
            this.lLink.Text = "管理上課地點清單";
            this.lLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lLink_LinkClicked);
            // 
            // AssnCourseItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lLink);
            this.Controls.Add(this.cbxEAddress);
            this.Controls.Add(this.lbCourseAddress);
            this.Controls.Add(this.cbxTeacher);
            this.Controls.Add(this.cbxSemeter);
            this.Controls.Add(this.lbSemester);
            this.Controls.Add(this.lbSchoolYear);
            this.Controls.Add(this.cbxSchoolYear);
            this.Controls.Add(this.lbCourseTeacher1);
            this.Controls.Add(this.lbCourseName);
            this.Controls.Add(this.txtCourseName);
            this.Name = "AssnCourseItem";
            this.Size = new System.Drawing.Size(550, 160);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.TextBoxX txtCourseName;
        private DevComponents.DotNetBar.LabelX lbCourseName;
        private DevComponents.DotNetBar.LabelX lbCourseTeacher1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSchoolYear;
        private DevComponents.DotNetBar.LabelX lbSchoolYear;
        private DevComponents.DotNetBar.LabelX lbSemester;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxSemeter;
        private System.Windows.Forms.ErrorProvider errorProvider1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxTeacher;
        private DevComponents.DotNetBar.LabelX lbCourseAddress;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbxEAddress;
        private System.Windows.Forms.LinkLabel lLink;
    }
}
