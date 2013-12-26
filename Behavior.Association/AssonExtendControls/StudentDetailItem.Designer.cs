using DevComponents.DotNetBar.Controls;
using SmartSchool.Common;

namespace JHSchool.Association
{
    partial class StudentDetailItem
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
            this.lbCourseCount = new DevComponents.DotNetBar.LabelX();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.ClearStudentInAssn = new System.Windows.Forms.ToolStripMenuItem();
            this.清空學生待處理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnInserStudent = new DevComponents.DotNetBar.ButtonX();
            this.buttonItem2 = new DevComponents.DotNetBar.ButtonItem();
            this.buttonItem1 = new DevComponents.DotNetBar.ButtonItem();
            this.lvStudents = new ListViewEX();
            this.chClass_ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSeatNo_ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chName_ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chGender_ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.chSNum_ = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnClearStudent = new DevComponents.DotNetBar.ButtonX();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbCourseCount
            // 
            this.lbCourseCount.AutoSize = true;
            // 
            // 
            // 
            this.lbCourseCount.BackgroundStyle.Class = "";
            this.lbCourseCount.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lbCourseCount.Location = new System.Drawing.Point(424, 205);
            this.lbCourseCount.Name = "lbCourseCount";
            this.lbCourseCount.Size = new System.Drawing.Size(74, 21);
            this.lbCourseCount.TabIndex = 1;
            this.lbCourseCount.Text = "社員人數：";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.ClearStudentInAssn,
            this.清空學生待處理ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(191, 92);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(190, 22);
            this.toolStripMenuItem2.Text = "將選擇學生加入待處理";
            this.toolStripMenuItem2.Click += new System.EventHandler(this.toolStripMenuItem2_Click);
            // 
            // ClearStudentInAssn
            // 
            this.ClearStudentInAssn.Name = "ClearStudentInAssn";
            this.ClearStudentInAssn.Size = new System.Drawing.Size(199, 22);
            this.ClearStudentInAssn.Text = "移除選擇社員";
            this.ClearStudentInAssn.Click += new System.EventHandler(this.ClearStudentInAssn_Click);
            // 
            // 清空學生待處理ToolStripMenuItem
            // 
            this.清空學生待處理ToolStripMenuItem.Name = "清空學生待處理ToolStripMenuItem";
            this.清空學生待處理ToolStripMenuItem.Size = new System.Drawing.Size(190, 22);
            this.清空學生待處理ToolStripMenuItem.Text = "清空待處理";
            this.清空學生待處理ToolStripMenuItem.Click += new System.EventHandler(this.清空學生待處理ToolStripMenuItem_Click);
            // 
            // btnInserStudent
            // 
            this.btnInserStudent.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnInserStudent.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnInserStudent.Location = new System.Drawing.Point(128, 204);
            this.btnInserStudent.Name = "btnInserStudent";
            this.btnInserStudent.Size = new System.Drawing.Size(195, 23);
            this.btnInserStudent.SubItems.AddRange(new DevComponents.DotNetBar.BaseItem[] {
            this.buttonItem2});
            this.btnInserStudent.TabIndex = 5;
            this.btnInserStudent.Text = "由待處理(學生)加入社員";
            this.btnInserStudent.PopupOpen += new System.EventHandler(this.btnInserStudent_PopupOpen);
            this.btnInserStudent.Click += new System.EventHandler(this.btnInserStudent_Click);
            // 
            // buttonItem2
            // 
            this.buttonItem2.GlobalItem = false;
            this.buttonItem2.Name = "buttonItem2";
            this.buttonItem2.Text = "buttonItem2";
            // 
            // buttonItem1
            // 
            this.buttonItem1.GlobalItem = false;
            this.buttonItem1.Name = "buttonItem1";
            this.buttonItem1.Text = "buttonItem1";
            // 
            // lvStudents
            // 
            // 
            // 
            // 
            this.lvStudents.Border.Class = "ListViewBorder";
            this.lvStudents.Border.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.lvStudents.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.chClass_,
            this.chSeatNo_,
            this.chName_,
            this.chGender_,
            this.chSNum_});
            this.lvStudents.ContextMenuStrip = this.contextMenuStrip1;
            this.lvStudents.FullRowSelect = true;
            this.lvStudents.Location = new System.Drawing.Point(19, 8);
            this.lvStudents.Name = "lvStudents";
            this.lvStudents.Size = new System.Drawing.Size(512, 188);
            this.lvStudents.TabIndex = 6;
            this.lvStudents.UseCompatibleStateImageBehavior = false;
            this.lvStudents.View = System.Windows.Forms.View.Details;
            // 
            // chClass_
            // 
            this.chClass_.Text = "班級";
            this.chClass_.Width = 90;
            // 
            // chSeatNo_
            // 
            this.chSeatNo_.Text = "座號";
            this.chSeatNo_.Width = 90;
            // 
            // chName_
            // 
            this.chName_.Text = "姓名";
            this.chName_.Width = 120;
            // 
            // chGender_
            // 
            this.chGender_.Text = "性別";
            // 
            // chSNum_
            // 
            this.chSNum_.Text = "學號";
            this.chSNum_.Width = 110;
            // 
            // btnClearStudent
            // 
            this.btnClearStudent.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnClearStudent.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnClearStudent.Location = new System.Drawing.Point(19, 204);
            this.btnClearStudent.Name = "btnClearStudent";
            this.btnClearStudent.Size = new System.Drawing.Size(100, 23);
            this.btnClearStudent.TabIndex = 7;
            this.btnClearStudent.Text = "移除選擇學生";
            this.btnClearStudent.Click += new System.EventHandler(this.btnClearStudent_Click);
            // 
            // StudentDetailItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnClearStudent);
            this.Controls.Add(this.lvStudents);
            this.Controls.Add(this.btnInserStudent);
            this.Controls.Add(this.lbCourseCount);
            this.Name = "StudentDetailItem";
            this.Size = new System.Drawing.Size(550, 240);
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.LabelX lbCourseCount;
        private DevComponents.DotNetBar.ButtonX btnInserStudent;
        private DevComponents.DotNetBar.ButtonItem buttonItem1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem ClearStudentInAssn;
        private DevComponents.DotNetBar.ButtonItem buttonItem2;
        private ListViewEX lvStudents;
        private System.Windows.Forms.ColumnHeader chClass_;
        private System.Windows.Forms.ColumnHeader chSeatNo_;
        private System.Windows.Forms.ColumnHeader chSNum_;
        private System.Windows.Forms.ColumnHeader chName_;
        private System.Windows.Forms.ColumnHeader chGender_;
        private DevComponents.DotNetBar.ButtonX btnClearStudent;
        private System.Windows.Forms.ToolStripMenuItem 清空學生待處理ToolStripMenuItem;
    }
}
