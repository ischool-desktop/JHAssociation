namespace JHSchool.Association
{
    partial class AssnEvaluationConfig
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
            this.cbxPercentage = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbxDegree = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.cbxDescribe = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.gpAssn = new DevComponents.DotNetBar.Controls.GroupPanel();
            this.txtEnd = new DevComponents.DotNetBar.LabelX();
            this.txtStart = new DevComponents.DotNetBar.LabelX();
            this.txtHelp1 = new DevComponents.DotNetBar.LabelX();
            this.dtiEnd = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.dtiStart = new DevComponents.Editors.DateTimeAdv.DateTimeInput();
            this.txtHelp2 = new DevComponents.DotNetBar.LabelX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.gpAssn.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtiEnd)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtiStart)).BeginInit();
            this.SuspendLayout();
            // 
            // cbxPercentage
            // 
            this.cbxPercentage.AutoSize = true;
            this.cbxPercentage.BackColor = System.Drawing.Color.Transparent;
            this.cbxPercentage.Location = new System.Drawing.Point(15, 49);
            this.cbxPercentage.Name = "cbxPercentage";
            this.cbxPercentage.Size = new System.Drawing.Size(67, 21);
            this.cbxPercentage.TabIndex = 0;
            this.cbxPercentage.Text = "成績";
            // 
            // cbxDegree
            // 
            this.cbxDegree.AutoSize = true;
            this.cbxDegree.BackColor = System.Drawing.Color.Transparent;
            this.cbxDegree.Location = new System.Drawing.Point(92, 49);
            this.cbxDegree.Name = "cbxDegree";
            this.cbxDegree.Size = new System.Drawing.Size(80, 21);
            this.cbxDegree.TabIndex = 1;
            this.cbxDegree.Text = "努力程度";
            // 
            // cbxDescribe
            // 
            this.cbxDescribe.AutoSize = true;
            this.cbxDescribe.BackColor = System.Drawing.Color.Transparent;
            this.cbxDescribe.Location = new System.Drawing.Point(182, 49);
            this.cbxDescribe.Name = "cbxDescribe";
            this.cbxDescribe.Size = new System.Drawing.Size(80, 21);
            this.cbxDescribe.TabIndex = 2;
            this.cbxDescribe.Text = "文字描述";
            // 
            // gpAssn
            // 
            this.gpAssn.BackColor = System.Drawing.Color.Transparent;
            this.gpAssn.CanvasColor = System.Drawing.SystemColors.Control;
            this.gpAssn.ColorSchemeStyle = DevComponents.DotNetBar.eDotNetBarStyle.Office2007;
            this.gpAssn.Controls.Add(this.txtEnd);
            this.gpAssn.Controls.Add(this.txtStart);
            this.gpAssn.Controls.Add(this.txtHelp1);
            this.gpAssn.Controls.Add(this.dtiEnd);
            this.gpAssn.Controls.Add(this.dtiStart);
            this.gpAssn.Controls.Add(this.txtHelp2);
            this.gpAssn.Controls.Add(this.cbxPercentage);
            this.gpAssn.Controls.Add(this.cbxDescribe);
            this.gpAssn.Controls.Add(this.cbxDegree);
            this.gpAssn.Location = new System.Drawing.Point(12, 12);
            this.gpAssn.Name = "gpAssn";
            this.gpAssn.Size = new System.Drawing.Size(282, 226);
            // 
            // 
            // 
            this.gpAssn.Style.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.gpAssn.Style.BackColorGradientAngle = 90;
            this.gpAssn.Style.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.gpAssn.Style.BorderBottom = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAssn.Style.BorderBottomWidth = 1;
            this.gpAssn.Style.BorderColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBorder;
            this.gpAssn.Style.BorderLeft = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAssn.Style.BorderLeftWidth = 1;
            this.gpAssn.Style.BorderRight = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAssn.Style.BorderRightWidth = 1;
            this.gpAssn.Style.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.gpAssn.Style.BorderTopWidth = 1;
            this.gpAssn.Style.CornerDiameter = 4;
            this.gpAssn.Style.CornerType = DevComponents.DotNetBar.eCornerType.Rounded;
            this.gpAssn.Style.TextColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelText;
            this.gpAssn.Style.TextLineAlignment = DevComponents.DotNetBar.eStyleTextAlignment.Near;
            this.gpAssn.TabIndex = 3;
            this.gpAssn.Text = "評量項目";
            // 
            // txtEnd
            // 
            this.txtEnd.AutoSize = true;
            this.txtEnd.Location = new System.Drawing.Point(15, 154);
            this.txtEnd.Name = "txtEnd";
            this.txtEnd.Size = new System.Drawing.Size(60, 21);
            this.txtEnd.TabIndex = 8;
            this.txtEnd.Text = "結束時間";
            // 
            // txtStart
            // 
            this.txtStart.AutoSize = true;
            this.txtStart.Location = new System.Drawing.Point(15, 125);
            this.txtStart.Name = "txtStart";
            this.txtStart.Size = new System.Drawing.Size(60, 21);
            this.txtStart.TabIndex = 7;
            this.txtStart.Text = "開始時間";
            // 
            // txtHelp1
            // 
            this.txtHelp1.AutoSize = true;
            this.txtHelp1.Location = new System.Drawing.Point(15, 21);
            this.txtHelp1.Name = "txtHelp1";
            this.txtHelp1.Size = new System.Drawing.Size(203, 21);
            this.txtHelp1.TabIndex = 6;
            this.txtHelp1.Text = "設定評量種類：(可複選評量方式)";
            // 
            // dtiEnd
            // 
            // 
            // 
            // 
            this.dtiEnd.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtiEnd.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtiEnd.ButtonDropDown.Visible = true;
            this.dtiEnd.Location = new System.Drawing.Point(87, 152);
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtiEnd.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtiEnd.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtiEnd.MonthCalendar.DayNames = new string[] {
        "日",
        "一",
        "二",
        "三",
        "四",
        "五",
        "六"};
            this.dtiEnd.MonthCalendar.DisplayMonth = new System.DateTime(2009, 11, 1, 0, 0, 0, 0);
            this.dtiEnd.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtiEnd.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiEnd.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtiEnd.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiEnd.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtiEnd.MonthCalendar.TodayButtonVisible = true;
            this.dtiEnd.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtiEnd.Name = "dtiEnd";
            this.dtiEnd.Size = new System.Drawing.Size(167, 25);
            this.dtiEnd.TabIndex = 5;
            // 
            // dtiStart
            // 
            // 
            // 
            // 
            this.dtiStart.BackgroundStyle.Class = "DateTimeInputBackground";
            this.dtiStart.ButtonDropDown.Shortcut = DevComponents.DotNetBar.eShortcut.AltDown;
            this.dtiStart.ButtonDropDown.Visible = true;
            this.dtiStart.Location = new System.Drawing.Point(87, 123);
            // 
            // 
            // 
            this.dtiStart.MonthCalendar.AnnuallyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiStart.MonthCalendar.BackgroundStyle.BackColor = System.Drawing.SystemColors.Window;
            this.dtiStart.MonthCalendar.ClearButtonVisible = true;
            // 
            // 
            // 
            this.dtiStart.MonthCalendar.CommandsBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground2;
            this.dtiStart.MonthCalendar.CommandsBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiStart.MonthCalendar.CommandsBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarBackground;
            this.dtiStart.MonthCalendar.CommandsBackgroundStyle.BorderTop = DevComponents.DotNetBar.eStyleBorderType.Solid;
            this.dtiStart.MonthCalendar.CommandsBackgroundStyle.BorderTopColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.BarDockedBorder;
            this.dtiStart.MonthCalendar.CommandsBackgroundStyle.BorderTopWidth = 1;
            this.dtiStart.MonthCalendar.DayNames = new string[] {
        "日",
        "一",
        "二",
        "三",
        "四",
        "五",
        "六"};
            this.dtiStart.MonthCalendar.DisplayMonth = new System.DateTime(2009, 11, 1, 0, 0, 0, 0);
            this.dtiStart.MonthCalendar.MarkedDates = new System.DateTime[0];
            this.dtiStart.MonthCalendar.MonthlyMarkedDates = new System.DateTime[0];
            // 
            // 
            // 
            this.dtiStart.MonthCalendar.NavigationBackgroundStyle.BackColor2SchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground2;
            this.dtiStart.MonthCalendar.NavigationBackgroundStyle.BackColorGradientAngle = 90;
            this.dtiStart.MonthCalendar.NavigationBackgroundStyle.BackColorSchemePart = DevComponents.DotNetBar.eColorSchemePart.PanelBackground;
            this.dtiStart.MonthCalendar.TodayButtonVisible = true;
            this.dtiStart.MonthCalendar.WeeklyMarkedDays = new System.DayOfWeek[0];
            this.dtiStart.Name = "dtiStart";
            this.dtiStart.Size = new System.Drawing.Size(167, 25);
            this.dtiStart.TabIndex = 4;
            // 
            // txtHelp2
            // 
            this.txtHelp2.AutoSize = true;
            this.txtHelp2.BackColor = System.Drawing.Color.Transparent;
            this.txtHelp2.Location = new System.Drawing.Point(15, 96);
            this.txtHelp2.Name = "txtHelp2";
            this.txtHelp2.Size = new System.Drawing.Size(127, 21);
            this.txtHelp2.TabIndex = 3;
            this.txtHelp2.Text = "設定評量輸入日期：";
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(138, 249);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(219, 249);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 5;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // AssnEvaluationConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(311, 284);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.gpAssn);
            this.Name = "AssnEvaluationConfig";
            this.Text = "社團評量設定";
            this.Load += new System.EventHandler(this.AssnEvaluationConfig_Load);
            this.gpAssn.ResumeLayout(false);
            this.gpAssn.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dtiEnd)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dtiStart)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevComponents.DotNetBar.Controls.CheckBoxX cbxPercentage;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbxDegree;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbxDescribe;
        private DevComponents.DotNetBar.Controls.GroupPanel gpAssn;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.LabelX txtHelp2;
        private DevComponents.DotNetBar.LabelX txtHelp1;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtiEnd;
        private DevComponents.Editors.DateTimeAdv.DateTimeInput dtiStart;
        private DevComponents.DotNetBar.LabelX txtEnd;
        private DevComponents.DotNetBar.LabelX txtStart;
    }
}