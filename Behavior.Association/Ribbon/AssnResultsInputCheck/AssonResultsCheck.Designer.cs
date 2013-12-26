namespace JHSchool.Association
{
    partial class AssonResultsCheck
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
            this.components = new System.ComponentModel.Container();
            this.listViewEx1 = new SmartSchool.Common.ListViewEX();
            this.ColID = new System.Windows.Forms.ColumnHeader();
            this.ColName = new System.Windows.Forms.ColumnHeader();
            this.ColTeacherName = new System.Windows.Forms.ColumnHeader();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ToolStripMenuItemAdd = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripMenuItemClear = new System.Windows.Forms.ToolStripMenuItem();
            this.txtTempCount = new DevComponents.DotNetBar.LabelX();
            this.cbViewNotClass = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.btnPrint = new DevComponents.DotNetBar.ButtonX();
            this.txtHelp = new DevComponents.DotNetBar.LabelX();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // listViewEx1
            // 
            this.listViewEx1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            // 
            // 
            // 
            this.listViewEx1.Border.Class = "ListViewBorder";
            this.listViewEx1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColID,
            this.ColName,
            this.ColTeacherName});
            this.listViewEx1.ContextMenuStrip = this.contextMenuStrip1;
            this.listViewEx1.FullRowSelect = true;
            this.listViewEx1.Location = new System.Drawing.Point(13, 41);
            this.listViewEx1.Name = "listViewEx1";
            this.listViewEx1.Size = new System.Drawing.Size(618, 331);
            this.listViewEx1.TabIndex = 0;
            this.listViewEx1.UseCompatibleStateImageBehavior = false;
            this.listViewEx1.View = System.Windows.Forms.View.Details;
            // 
            // ColID
            // 
            this.ColID.Text = "社團ID";
            this.ColID.Width = 0;
            // 
            // ColName
            // 
            this.ColName.Text = "社團名稱";
            this.ColName.Width = 200;
            // 
            // ColTeacherName
            // 
            this.ColTeacherName.Text = "指導老師";
            this.ColTeacherName.Width = 100;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ToolStripMenuItemAdd,
            this.ToolStripMenuItemClear});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(163, 48);
            // 
            // ToolStripMenuItemAdd
            // 
            this.ToolStripMenuItemAdd.Name = "ToolStripMenuItemAdd";
            this.ToolStripMenuItemAdd.Size = new System.Drawing.Size(162, 22);
            this.ToolStripMenuItemAdd.Text = "加入[社團]待處理";
            this.ToolStripMenuItemAdd.Click += new System.EventHandler(this.ToolStripMenuItemAdd_Click);
            // 
            // ToolStripMenuItemClear
            // 
            this.ToolStripMenuItemClear.Name = "ToolStripMenuItemClear";
            this.ToolStripMenuItemClear.Size = new System.Drawing.Size(162, 22);
            this.ToolStripMenuItemClear.Text = "清空[社團]待處理";
            this.ToolStripMenuItemClear.Click += new System.EventHandler(this.ToolStripMenuItemClear_Click);
            // 
            // txtTempCount
            // 
            this.txtTempCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTempCount.AutoSize = true;
            this.txtTempCount.BackColor = System.Drawing.Color.Transparent;
            this.txtTempCount.Location = new System.Drawing.Point(523, 12);
            this.txtTempCount.Name = "txtTempCount";
            this.txtTempCount.Size = new System.Drawing.Size(101, 21);
            this.txtTempCount.TabIndex = 20;
            this.txtTempCount.Text = "待處理社團數: 0";
            // 
            // cbViewNotClass
            // 
            this.cbViewNotClass.AutoSize = true;
            this.cbViewNotClass.BackColor = System.Drawing.Color.Transparent;
            this.cbViewNotClass.Location = new System.Drawing.Point(13, 12);
            this.cbViewNotClass.Name = "cbViewNotClass";
            this.cbViewNotClass.Size = new System.Drawing.Size(174, 21);
            this.cbViewNotClass.TabIndex = 19;
            this.cbViewNotClass.Text = "僅顯示未完成輸入之社團";
            this.cbViewNotClass.CheckedChanged += new System.EventHandler(this.cbViewNotClass_CheckedChanged);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(556, 379);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 25);
            this.btnExit.TabIndex = 18;
            this.btnExit.Text = "關閉";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnPrint
            // 
            this.btnPrint.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrint.AutoSize = true;
            this.btnPrint.BackColor = System.Drawing.Color.Transparent;
            this.btnPrint.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrint.Location = new System.Drawing.Point(13, 378);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(83, 25);
            this.btnPrint.TabIndex = 16;
            this.btnPrint.Text = "匯出到Excel";
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click_1);
            // 
            // txtHelp
            // 
            this.txtHelp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtHelp.AutoSize = true;
            this.txtHelp.BackColor = System.Drawing.Color.Transparent;
            this.txtHelp.Location = new System.Drawing.Point(102, 380);
            this.txtHelp.Name = "txtHelp";
            this.txtHelp.Size = new System.Drawing.Size(230, 21);
            this.txtHelp.TabIndex = 21;
            this.txtHelp.Text = "說明：本功能僅檢查(一般)狀態之學生";
            // 
            // AssonResultsCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(642, 416);
            this.Controls.Add(this.txtTempCount);
            this.Controls.Add(this.txtHelp);
            this.Controls.Add(this.cbViewNotClass);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.listViewEx1);
            this.MaximizeBox = true;
            this.Name = "AssonResultsCheck";
            this.Text = "社團評量輸入狀況檢查";
            this.Load += new System.EventHandler(this.AssonResultsCheck_Load);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX txtTempCount;
        private DevComponents.DotNetBar.Controls.CheckBoxX cbViewNotClass;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private DevComponents.DotNetBar.ButtonX btnPrint;
        private SmartSchool.Common.ListViewEX listViewEx1;
        private System.Windows.Forms.ColumnHeader ColName;
        private System.Windows.Forms.ColumnHeader ColTeacherName;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemAdd;
        private System.Windows.Forms.ToolStripMenuItem ToolStripMenuItemClear;
        private System.Windows.Forms.ColumnHeader ColID;
        private DevComponents.DotNetBar.LabelX txtHelp;

    }
}