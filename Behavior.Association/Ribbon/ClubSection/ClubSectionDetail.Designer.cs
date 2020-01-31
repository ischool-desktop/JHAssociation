namespace JHSchool.Association
{
    partial class ClubSectionDetail
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.colDate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colWeek = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colPer = new DevComponents.DotNetBar.Controls.DataGridViewComboBoxExColumn();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.cbSingClub = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.lbHelp2 = new DevComponents.DotNetBar.LabelX();
            this.cbDoubleClub = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.lbHelp1 = new DevComponents.DotNetBar.LabelX();
            this.cbGradeYear = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.lbSchoolYear = new DevComponents.DotNetBar.LabelX();
            this.lbSemester = new DevComponents.DotNetBar.LabelX();
            this.intSchoolYear = new DevComponents.Editors.IntegerInput();
            this.cbSemester = new DevComponents.DotNetBar.Controls.ComboBoxEx();
            this.comboItem1 = new DevComponents.Editors.ComboItem();
            this.comboItem2 = new DevComponents.Editors.ComboItem();
            this.btnImport = new DevComponents.DotNetBar.ButtonX();
            this.lbHelp3 = new DevComponents.DotNetBar.LabelX();
            this.buttonX1 = new DevComponents.DotNetBar.ButtonX();
            this.buttonItem1 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem2 = new DevComponents.DotNetBar.ButtonItem();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToOrderColumns = true;
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colDate,
            this.colWeek,
            this.colPer});
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle4;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(12, 89);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(528, 412);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellEndEdit);
            // 
            // colDate
            // 
            this.colDate.HeaderText = "日期";
            this.colDate.Name = "colDate";
            this.colDate.Width = 200;
            // 
            // colWeek
            // 
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.colWeek.DefaultCellStyle = dataGridViewCellStyle3;
            this.colWeek.HeaderText = "星期";
            this.colWeek.Name = "colWeek";
            this.colWeek.ReadOnly = true;
            // 
            // colPer
            // 
            this.colPer.DisplayMember = "Text";
            this.colPer.DropDownHeight = 106;
            this.colPer.DropDownWidth = 121;
            this.colPer.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.colPer.HeaderText = "節次";
            this.colPer.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.colPer.IntegralHeight = false;
            this.colPer.ItemHeight = 17;
            this.colPer.Name = "colPer";
            this.colPer.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colPer.RightToLeft = System.Windows.Forms.RightToLeft.No;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.AutoSize = true;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(383, 569);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 25);
            this.btnSave.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.AutoSize = true;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(465, 569);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cbSingClub
            // 
            this.cbSingClub.AutoSize = true;
            this.cbSingClub.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cbSingClub.BackgroundStyle.Class = "";
            this.cbSingClub.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbSingClub.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.cbSingClub.Checked = true;
            this.cbSingClub.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbSingClub.CheckValue = "Y";
            this.cbSingClub.Location = new System.Drawing.Point(354, 62);
            this.cbSingClub.Name = "cbSingClub";
            this.cbSingClub.Size = new System.Drawing.Size(80, 21);
            this.cbSingClub.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbSingClub.TabIndex = 3;
            this.cbSingClub.Text = "每週上課";
            // 
            // lbHelp2
            // 
            this.lbHelp2.AutoSize = true;
            this.lbHelp2.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbHelp1.BackgroundStyle.Class = "";
            this.lbHelp1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbHelp2.Location = new System.Drawing.Point(247, 62);
            this.lbHelp2.Name = "lbHelp2";
            this.lbHelp2.Size = new System.Drawing.Size(101, 21);
            this.lbHelp2.TabIndex = 4;
            this.lbHelp2.Text = "本年級上課周次";
            // 
            // cbDoubleClub
            // 
            this.cbDoubleClub.AutoSize = true;
            this.cbDoubleClub.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.cbDoubleClub.BackgroundStyle.Class = "";
            this.cbDoubleClub.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.cbDoubleClub.CheckBoxStyle = DevComponents.DotNetBar.eCheckBoxStyle.RadioButton;
            this.cbDoubleClub.Location = new System.Drawing.Point(440, 62);
            this.cbDoubleClub.Name = "cbDoubleClub";
            this.cbDoubleClub.Size = new System.Drawing.Size(80, 21);
            this.cbDoubleClub.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbDoubleClub.TabIndex = 5;
            this.cbDoubleClub.Text = "隔週上課";
            // 
            // lbHelp1
            // 
            this.lbHelp1.AutoSize = true;
            this.lbHelp1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbHelp1.BackgroundStyle.Class = "";
            this.lbHelp1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbHelp1.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.lbHelp1.Location = new System.Drawing.Point(22, 62);
            this.lbHelp1.Name = "lbHelp1";
            this.lbHelp1.Size = new System.Drawing.Size(215, 21);
            this.lbHelp1.TabIndex = 11;
            this.lbHelp1.Text = "日期可輸入1/10 替換為 2020/1/10";
            // 
            // cbGradeYear
            // 
            this.cbGradeYear.DisplayMember = "Text";
            this.cbGradeYear.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbGradeYear.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbGradeYear.FormattingEnabled = true;
            this.cbGradeYear.ItemHeight = 19;
            this.cbGradeYear.Location = new System.Drawing.Point(354, 22);
            this.cbGradeYear.Name = "cbGradeYear";
            this.cbGradeYear.Size = new System.Drawing.Size(80, 25);
            this.cbGradeYear.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbGradeYear.TabIndex = 12;
            this.cbGradeYear.SelectedIndexChanged += new System.EventHandler(this.cbGreadYear_SelectedIndexChanged);
            // 
            // labelX1
            // 
            this.labelX1.AutoSize = true;
            this.labelX1.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.labelX1.BackgroundStyle.Class = "";
            this.labelX1.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.labelX1.Location = new System.Drawing.Point(310, 24);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(34, 21);
            this.labelX1.TabIndex = 13;
            this.labelX1.Text = "年級";
            // 
            // lbSchoolYear
            // 
            this.lbSchoolYear.AutoSize = true;
            this.lbSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbSchoolYear.BackgroundStyle.Class = "";
            this.lbSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbSchoolYear.Location = new System.Drawing.Point(21, 24);
            this.lbSchoolYear.Name = "lbSchoolYear";
            this.lbSchoolYear.Size = new System.Drawing.Size(47, 21);
            this.lbSchoolYear.TabIndex = 14;
            this.lbSchoolYear.Text = "學年度";
            // 
            // lbSemester
            // 
            this.lbSemester.AutoSize = true;
            this.lbSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbSemester.BackgroundStyle.Class = "";
            this.lbSemester.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbSemester.Location = new System.Drawing.Point(168, 24);
            this.lbSemester.Name = "lbSemester";
            this.lbSemester.Size = new System.Drawing.Size(34, 21);
            this.lbSemester.TabIndex = 15;
            this.lbSemester.Text = "學期";
            // 
            // intSchoolYear
            // 
            this.intSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.intSchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.intSchoolYear.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.intSchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.intSchoolYear.Location = new System.Drawing.Point(78, 22);
            this.intSchoolYear.MaxValue = 999;
            this.intSchoolYear.MinValue = 100;
            this.intSchoolYear.Name = "intSchoolYear";
            this.intSchoolYear.ShowUpDown = true;
            this.intSchoolYear.Size = new System.Drawing.Size(80, 25);
            this.intSchoolYear.TabIndex = 16;
            this.intSchoolYear.Value = 100;
            this.intSchoolYear.ValueChanged += new System.EventHandler(this.intSchoolYear_ValueChanged);
            // 
            // cbSemester
            // 
            this.cbSemester.DisplayMember = "Text";
            this.cbSemester.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cbSemester.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cbSemester.FormattingEnabled = true;
            this.cbSemester.ItemHeight = 19;
            this.cbSemester.Items.AddRange(new object[] {
            this.comboItem1,
            this.comboItem2});
            this.cbSemester.Location = new System.Drawing.Point(212, 22);
            this.cbSemester.Name = "cbSemester";
            this.cbSemester.Size = new System.Drawing.Size(68, 25);
            this.cbSemester.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.cbSemester.TabIndex = 17;
            this.cbSemester.SelectedIndexChanged += new System.EventHandler(this.cbSemester_SelectedIndexChanged);
            // 
            // comboItem1
            // 
            this.comboItem1.Text = "1";
            // 
            // comboItem2
            // 
            this.comboItem2.Text = "2";
            // 
            // btnImport
            // 
            this.btnImport.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnImport.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnImport.AutoSize = true;
            this.btnImport.BackColor = System.Drawing.Color.Transparent;
            this.btnImport.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnImport.Location = new System.Drawing.Point(103, 569);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 25);
            this.btnImport.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.btnImport.TabIndex = 19;
            this.btnImport.Text = "匯入";
            this.btnImport.Click += new System.EventHandler(this.btnImport_Click);
            // 
            // lbHelp3
            // 
            this.lbHelp3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lbHelp3.AutoSize = true;
            this.lbHelp3.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.lbHelp3.BackgroundStyle.Class = "";
            this.lbHelp3.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbHelp3.Location = new System.Drawing.Point(22, 507);
            this.lbHelp3.Name = "lbHelp3";
            this.lbHelp3.Size = new System.Drawing.Size(500, 56);
            this.lbHelp3.TabIndex = 20;
            this.lbHelp3.Text = "匯出條件:1.所有資料 2.匯出所選學年期 3.匯出所選學年期+年級\r\n匯入說明:您匯入一年級資料則所有一年級日期資料將被清空取代(以您匯入內容為準)\r\n請注意若" +
    "此份資料內,包含一筆二年級之日期資料,則二年級日期資料也會被清空";
            // 
            // buttonX1
            // 
            this.buttonX1.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.buttonX1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonX1.BackColor = System.Drawing.Color.Transparent;
            this.buttonX1.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.buttonX1.Location = new System.Drawing.Point(22, 569);
            this.buttonX1.Name = "buttonX1";
            this.buttonX1.Size = new System.Drawing.Size(75, 25);
            this.buttonX1.Style = DevComponents.DotNetBar.eDotNetBarStyle.StyleManagerControlled;
            this.buttonX1.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem1,
            this.buttonItem2});
            this.buttonX1.TabIndex = 21;
            this.buttonX1.Text = "匯出";
            this.buttonX1.Click += new System.EventHandler(this.buttonX1_Click);
            // 
            // buttonItem1
            // 
            this.buttonItem1.GlobalItem = false;
            this.buttonItem1.Name = "buttonItem1";
            this.buttonItem1.Text = "依學年度學期";
            this.buttonItem1.Click += new System.EventHandler(this.buttonItem1_Click);
            // 
            // buttonItem2
            // 
            this.buttonItem2.GlobalItem = false;
            this.buttonItem2.Name = "buttonItem2";
            this.buttonItem2.Text = "依學年度學期年級";
            this.buttonItem2.Click += new System.EventHandler(this.buttonItem2_Click);
            // 
            // ClubSectionDetail
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(552, 602);
            this.Controls.Add(this.buttonX1);
            this.Controls.Add(this.lbHelp3);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.cbSemester);
            this.Controls.Add(this.intSchoolYear);
            this.Controls.Add(this.lbSemester);
            this.Controls.Add(this.lbSchoolYear);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.cbGradeYear);
            this.Controls.Add(this.lbHelp1);
            this.Controls.Add(this.cbDoubleClub);
            this.Controls.Add(this.lbHelp2);
            this.Controls.Add(this.cbSingClub);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dataGridViewX1);
            this.DoubleBuffered = true;
            this.Name = "ClubSectionDetail";
            this.Text = "社團上課時間表";
            this.Load += new System.EventHandler(this.ClubSectionDetail_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.intSchoolYear)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbSingClub;
        private DevComponents.DotNetBar.LabelX lbHelp2;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbDoubleClub;
        private DevComponents.DotNetBar.LabelX lbHelp1;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbGradeYear;
        private DevComponents.DotNetBar.LabelX labelX1;
        private DevComponents.DotNetBar.LabelX lbSchoolYear;
        private DevComponents.DotNetBar.LabelX lbSemester;
        private DevComponents.Editors.IntegerInput intSchoolYear;
        private DevComponents.DotNetBar.Controls.ComboBoxEx cbSemester;
        private DevComponents.Editors.ComboItem comboItem1;
        private DevComponents.Editors.ComboItem comboItem2;
        private DevComponents.DotNetBar.ButtonX btnImport;
        private DevComponents.DotNetBar.LabelX lbHelp3;
        private System.Windows.Forms.DataGridViewTextBoxColumn colDate;
        private System.Windows.Forms.DataGridViewTextBoxColumn colWeek;
        private DevComponents.DotNetBar.Controls.DataGridViewComboBoxExColumn colPer;
        private DevComponents.DotNetBar.ButtonX buttonX1;
        private DevComponents.DotNetBar.ButtonItem buttonItem1;
        private DevComponents.DotNetBar.ButtonItem buttonItem2;
    }
}