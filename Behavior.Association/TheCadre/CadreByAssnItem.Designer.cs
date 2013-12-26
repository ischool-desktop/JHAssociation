namespace JHSchool.Association
{
    partial class CadreByAssnItem
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewX1 = new JHSchool.Association.CustomDataGridView();
            this.ColStudentID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColCadreName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColSeatNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStudNum = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColStudentName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToAddRows = false;
            this.dataGridViewX1.AllowUserToDeleteRows = false;
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColStudentID,
            this.ColCadreName,
            this.ColClassName,
            this.ColSeatNo,
            this.ColStudNum,
            this.ColStudentName});
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(13, 8);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(524, 169);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellEndEdit);
            // 
            // ColStudentID
            // 
            this.ColStudentID.HeaderText = "學生ID";
            this.ColStudentID.Name = "ColStudentID";
            this.ColStudentID.ReadOnly = true;
            this.ColStudentID.Visible = false;
            // 
            // ColCadreName
            // 
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.LightCyan;
            this.ColCadreName.DefaultCellStyle = dataGridViewCellStyle1;
            this.ColCadreName.HeaderText = "幹部名稱";
            this.ColCadreName.Name = "ColCadreName";
            this.ColCadreName.ReadOnly = true;
            // 
            // ColClassName
            // 
            this.ColClassName.HeaderText = "班級";
            this.ColClassName.Name = "ColClassName";
            // 
            // ColSeatNo
            // 
            this.ColSeatNo.HeaderText = "座號";
            this.ColSeatNo.Name = "ColSeatNo";
            this.ColSeatNo.Width = 65;
            // 
            // ColStudNum
            // 
            this.ColStudNum.HeaderText = "學號";
            this.ColStudNum.Name = "ColStudNum";
            this.ColStudNum.Width = 90;
            // 
            // ColStudentName
            // 
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.LightCyan;
            this.ColStudentName.DefaultCellStyle = dataGridViewCellStyle2;
            this.ColStudentName.HeaderText = "學生姓名";
            this.ColStudentName.Name = "ColStudentName";
            this.ColStudentName.ReadOnly = true;
            // 
            // CadreByAssnItem
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewX1);
            this.Name = "CadreByAssnItem";
            this.Size = new System.Drawing.Size(550, 185);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private CustomDataGridView dataGridViewX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStudentID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColCadreName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColClassName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColSeatNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStudNum;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColStudentName;
    }
}
