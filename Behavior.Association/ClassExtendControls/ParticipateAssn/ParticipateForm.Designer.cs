namespace JHSchool.Association
{
    partial class ParticipateForm
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
            this.IntSchoolYear = new DevComponents.Editors.IntegerInput();
            this.txtSchoolYear = new DevComponents.DotNetBar.LabelX();
            this.txtSemester = new DevComponents.DotNetBar.LabelX();
            this.IntSemester = new DevComponents.Editors.IntegerInput();
            this.txtHelp = new DevComponents.DotNetBar.LabelX();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.cbAssnAddress = new DevComponents.DotNetBar.Controls.CheckBoxX();
            ((System.ComponentModel.ISupportInitialize)(this.IntSchoolYear)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntSemester)).BeginInit();
            this.SuspendLayout();
            // 
            // IntSchoolYear
            // 
            this.IntSchoolYear.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.IntSchoolYear.BackgroundStyle.Class = "DateTimeInputBackground";
            this.IntSchoolYear.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.IntSchoolYear.Location = new System.Drawing.Point(107, 62);
            this.IntSchoolYear.MaxValue = 999;
            this.IntSchoolYear.MinValue = 90;
            this.IntSchoolYear.Name = "IntSchoolYear";
            this.IntSchoolYear.ShowUpDown = true;
            this.IntSchoolYear.Size = new System.Drawing.Size(49, 25);
            this.IntSchoolYear.TabIndex = 0;
            this.IntSchoolYear.Value = 90;
            // 
            // txtSchoolYear
            // 
            this.txtSchoolYear.AutoSize = true;
            this.txtSchoolYear.BackColor = System.Drawing.Color.Transparent;
            this.txtSchoolYear.Location = new System.Drawing.Point(48, 64);
            this.txtSchoolYear.Name = "txtSchoolYear";
            this.txtSchoolYear.Size = new System.Drawing.Size(47, 21);
            this.txtSchoolYear.TabIndex = 1;
            this.txtSchoolYear.Text = "學年度";
            // 
            // txtSemester
            // 
            this.txtSemester.AutoSize = true;
            this.txtSemester.BackColor = System.Drawing.Color.Transparent;
            this.txtSemester.Location = new System.Drawing.Point(168, 64);
            this.txtSemester.Name = "txtSemester";
            this.txtSemester.Size = new System.Drawing.Size(34, 21);
            this.txtSemester.TabIndex = 2;
            this.txtSemester.Text = "學期";
            // 
            // IntSemester
            // 
            this.IntSemester.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.IntSemester.BackgroundStyle.Class = "DateTimeInputBackground";
            this.IntSemester.ButtonFreeText.Shortcut = DevComponents.DotNetBar.eShortcut.F2;
            this.IntSemester.Location = new System.Drawing.Point(214, 62);
            this.IntSemester.MaxValue = 2;
            this.IntSemester.MinValue = 1;
            this.IntSemester.Name = "IntSemester";
            this.IntSemester.ShowUpDown = true;
            this.IntSemester.Size = new System.Drawing.Size(49, 25);
            this.IntSemester.TabIndex = 3;
            this.IntSemester.Value = 1;
            // 
            // txtHelp
            // 
            this.txtHelp.AutoSize = true;
            this.txtHelp.BackColor = System.Drawing.Color.Transparent;
            this.txtHelp.Location = new System.Drawing.Point(11, 14);
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.Size = new System.Drawing.Size(133, 21);
            this.txtHelp.TabIndex = 4;
            this.txtHelp.Text = "請指定學年度/學期：";
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(177, 117);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(58, 23);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = "列印";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(244, 117);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(58, 23);
            this.btnExit.TabIndex = 6;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // cbAssnAddress
            // 
            this.cbAssnAddress.AutoSize = true;
            this.cbAssnAddress.BackColor = System.Drawing.Color.Transparent;
            this.cbAssnAddress.Checked = true;
            this.cbAssnAddress.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAssnAddress.CheckValue = "Y";
            this.cbAssnAddress.Location = new System.Drawing.Point(11, 119);
            this.cbAssnAddress.Name = "cbAssnAddress";
            this.cbAssnAddress.Size = new System.Drawing.Size(156, 21);
            this.cbAssnAddress.TabIndex = 7;
            this.cbAssnAddress.Text = "列印上課地點(新功能)";
            // 
            // ParticipateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 152);
            this.Controls.Add(this.cbAssnAddress);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.txtHelp);
            this.Controls.Add(this.IntSemester);
            this.Controls.Add(this.txtSemester);
            this.Controls.Add(this.txtSchoolYear);
            this.Controls.Add(this.IntSchoolYear);
            this.Name = "ParticipateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "班級社團分組名單";
            this.Load += new System.EventHandler(this.ParticipateForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.IntSchoolYear)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.IntSemester)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.Editors.IntegerInput IntSchoolYear;
        private DevComponents.DotNetBar.LabelX txtSchoolYear;
        private DevComponents.DotNetBar.LabelX txtSemester;
        private DevComponents.Editors.IntegerInput IntSemester;
        private DevComponents.DotNetBar.LabelX txtHelp;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbAssnAddress;
    }
}