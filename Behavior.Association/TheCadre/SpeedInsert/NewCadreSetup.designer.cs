﻿namespace JHSchool.Association
{
    partial class NewCadreSetup
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewX1 = new DevComponents.DotNetBar.Controls.DataGridViewX();
            this.colCadreType = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.colCadreIndex = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colCadreName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colNumber = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMeritA = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMeritB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMeritC = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colMeritReason = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.colRatioOrder = new DevComponents.DotNetBar.Controls.DataGridViewCheckBoxXColumn();
            this.btnSave = new DevComponents.DotNetBar.ButtonX();
            this.btnExit = new DevComponents.DotNetBar.ButtonX();
            this.labelX1 = new DevComponents.DotNetBar.LabelX();
            this.btnPrintOut = new DevComponents.DotNetBar.ButtonX();
            this.btnPrintIn = new DevComponents.DotNetBar.ButtonX();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.KeyInMerit = new DevComponents.DotNetBar.Controls.CheckBoxX();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewX1
            // 
            this.dataGridViewX1.AllowUserToResizeColumns = false;
            this.dataGridViewX1.AllowUserToResizeRows = false;
            this.dataGridViewX1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridViewX1.BackgroundColor = System.Drawing.Color.White;
            this.dataGridViewX1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewX1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colCadreType,
            this.colCadreIndex,
            this.colCadreName,
            this.colNumber,
            this.colMeritA,
            this.colMeritB,
            this.colMeritC,
            this.colMeritReason,
            this.colRatioOrder});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("微軟正黑體", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(136)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewX1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewX1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(208)))), ((int)(((byte)(215)))), ((int)(((byte)(229)))));
            this.dataGridViewX1.Location = new System.Drawing.Point(12, 39);
            this.dataGridViewX1.Name = "dataGridViewX1";
            this.dataGridViewX1.RowTemplate.Height = 24;
            this.dataGridViewX1.Size = new System.Drawing.Size(988, 383);
            this.dataGridViewX1.TabIndex = 0;
            this.dataGridViewX1.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewX1_CellEndEdit);
            this.dataGridViewX1.UserAddedRow += new System.Windows.Forms.DataGridViewRowEventHandler(this.dataGridViewX1_UserAddedRow);
            // 
            // colCadreType
            // 
            this.colCadreType.HeaderText = "幹部類型";
            this.colCadreType.Items.AddRange(new object[] {
            "班級幹部",
            "社團幹部",
            "學校幹部"});
            this.colCadreType.Name = "colCadreType";
            // 
            // colCadreIndex
            // 
            this.colCadreIndex.HeaderText = "排序";
            this.colCadreIndex.Name = "colCadreIndex";
            this.colCadreIndex.Width = 65;
            // 
            // colCadreName
            // 
            this.colCadreName.HeaderText = "幹部名稱";
            this.colCadreName.Name = "colCadreName";
            // 
            // colNumber
            // 
            this.colNumber.HeaderText = "擔任人數";
            this.colNumber.Name = "colNumber";
            // 
            // colMeritA
            // 
            this.colMeritA.HeaderText = "大功";
            this.colMeritA.Name = "colMeritA";
            this.colMeritA.Width = 60;
            // 
            // colMeritB
            // 
            this.colMeritB.HeaderText = "小功";
            this.colMeritB.Name = "colMeritB";
            this.colMeritB.Width = 60;
            // 
            // colMeritC
            // 
            this.colMeritC.HeaderText = "嘉獎";
            this.colMeritC.Name = "colMeritC";
            this.colMeritC.Width = 60;
            // 
            // colMeritReason
            // 
            this.colMeritReason.HeaderText = "獎勵事由";
            this.colMeritReason.Name = "colMeritReason";
            this.colMeritReason.Width = 300;
            // 
            // colRatioOrder
            // 
            this.colRatioOrder.Checked = true;
            this.colRatioOrder.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.colRatioOrder.CheckValue = null;
            this.colRatioOrder.HeaderText = "參與比序";
            this.colRatioOrder.Name = "colRatioOrder";
            this.colRatioOrder.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.colRatioOrder.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.colRatioOrder.Width = 85;
            // 
            // btnSave
            // 
            this.btnSave.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.BackColor = System.Drawing.Color.Transparent;
            this.btnSave.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnSave.Location = new System.Drawing.Point(838, 437);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 1;
            this.btnSave.Text = "儲存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnExit
            // 
            this.btnExit.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnExit.BackColor = System.Drawing.Color.Transparent;
            this.btnExit.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnExit.Location = new System.Drawing.Point(925, 437);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 2;
            this.btnExit.Text = "離開";
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
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
            this.labelX1.Location = new System.Drawing.Point(12, 12);
            this.labelX1.Name = "labelX1";
            this.labelX1.Size = new System.Drawing.Size(291, 21);
            this.labelX1.TabIndex = 3;
            this.labelX1.Text = "說明：幹部比序欄位,為免試入學超額比序使用。";
            // 
            // btnPrintOut
            // 
            this.btnPrintOut.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrintOut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintOut.BackColor = System.Drawing.Color.Transparent;
            this.btnPrintOut.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrintOut.Location = new System.Drawing.Point(12, 437);
            this.btnPrintOut.Name = "btnPrintOut";
            this.btnPrintOut.Size = new System.Drawing.Size(75, 23);
            this.btnPrintOut.TabIndex = 4;
            this.btnPrintOut.Text = "匯出";
            this.btnPrintOut.Click += new System.EventHandler(this.btnPrintOut_Click);
            // 
            // btnPrintIn
            // 
            this.btnPrintIn.AccessibleRole = System.Windows.Forms.AccessibleRole.PushButton;
            this.btnPrintIn.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPrintIn.BackColor = System.Drawing.Color.Transparent;
            this.btnPrintIn.ColorTable = DevComponents.DotNetBar.eButtonColor.OrangeWithBackground;
            this.btnPrintIn.Location = new System.Drawing.Point(93, 437);
            this.btnPrintIn.Name = "btnPrintIn";
            this.btnPrintIn.Size = new System.Drawing.Size(75, 23);
            this.btnPrintIn.TabIndex = 5;
            this.btnPrintIn.Text = "匯入";
            this.btnPrintIn.Click += new System.EventHandler(this.btnPrintIn_Click);
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileName = "匯出幹部名稱管理";
            this.saveFileDialog1.Filter = "Excel (*.xls)|*.xls";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 8000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // KeyInMerit
            // 
            this.KeyInMerit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.KeyInMerit.AutoSize = true;
            this.KeyInMerit.BackColor = System.Drawing.Color.Transparent;
            // 
            // 
            // 
            this.KeyInMerit.BackgroundStyle.Class = "";
            this.KeyInMerit.BackgroundStyle.CornerType = DevComponents.DotNetBar.eCornerType.Square;
            this.KeyInMerit.Location = new System.Drawing.Point(893, 12);
            this.KeyInMerit.Name = "KeyInMerit";
            this.KeyInMerit.Size = new System.Drawing.Size(107, 21);
            this.KeyInMerit.TabIndex = 6;
            this.KeyInMerit.Text = "輸入敘獎資料";
            this.KeyInMerit.CheckedChanged += new System.EventHandler(this.KeyInMerit_CheckedChanged);
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "幹部名稱";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.Width = 65;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "大功";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Width = 60;
            // 
            // dataGridViewTextBoxColumn3
            // 
            this.dataGridViewTextBoxColumn3.HeaderText = "小功";
            this.dataGridViewTextBoxColumn3.Name = "dataGridViewTextBoxColumn3";
            this.dataGridViewTextBoxColumn3.Width = 60;
            // 
            // dataGridViewTextBoxColumn4
            // 
            this.dataGridViewTextBoxColumn4.HeaderText = "嘉獎";
            this.dataGridViewTextBoxColumn4.Name = "dataGridViewTextBoxColumn4";
            this.dataGridViewTextBoxColumn4.Width = 60;
            // 
            // dataGridViewTextBoxColumn5
            // 
            this.dataGridViewTextBoxColumn5.HeaderText = "獎勵事由";
            this.dataGridViewTextBoxColumn5.Name = "dataGridViewTextBoxColumn5";
            this.dataGridViewTextBoxColumn5.Width = 200;
            // 
            // dataGridViewTextBoxColumn6
            // 
            this.dataGridViewTextBoxColumn6.HeaderText = "獎勵事由";
            this.dataGridViewTextBoxColumn6.Name = "dataGridViewTextBoxColumn6";
            this.dataGridViewTextBoxColumn6.Width = 300;
            // 
            // dataGridViewTextBoxColumn7
            // 
            this.dataGridViewTextBoxColumn7.HeaderText = "獎勵事由";
            this.dataGridViewTextBoxColumn7.Name = "dataGridViewTextBoxColumn7";
            this.dataGridViewTextBoxColumn7.Width = 300;
            // 
            // dataGridViewTextBoxColumn8
            // 
            this.dataGridViewTextBoxColumn8.HeaderText = "參與比序";
            this.dataGridViewTextBoxColumn8.Name = "dataGridViewTextBoxColumn8";
            // 
            // NewCadreSetup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1010, 470);
            this.Controls.Add(this.KeyInMerit);
            this.Controls.Add(this.btnPrintIn);
            this.Controls.Add(this.btnPrintOut);
            this.Controls.Add(this.labelX1);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.dataGridViewX1);
            this.DoubleBuffered = true;
            this.Name = "NewCadreSetup";
            this.Text = "幹部名稱管理(班級幹部 / 學校幹部 / 社團幹部)";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewX1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevComponents.DotNetBar.Controls.DataGridViewX dataGridViewX1;
        private DevComponents.DotNetBar.ButtonX btnSave;
        private DevComponents.DotNetBar.ButtonX btnExit;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn3;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn4;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn5;
        private DevComponents.DotNetBar.LabelX labelX1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn6;
        private DevComponents.DotNetBar.ButtonX btnPrintOut;
        private DevComponents.DotNetBar.ButtonX btnPrintIn;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.Timer timer1;
        private DevComponents.DotNetBar.Controls.CheckBoxX KeyInMerit;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn7;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn8;
        private System.Windows.Forms.DataGridViewComboBoxColumn colCadreType;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCadreIndex;
        private System.Windows.Forms.DataGridViewTextBoxColumn colCadreName;
        private System.Windows.Forms.DataGridViewTextBoxColumn colNumber;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMeritA;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMeritB;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMeritC;
        private System.Windows.Forms.DataGridViewTextBoxColumn colMeritReason;
        private DevComponents.DotNetBar.Controls.DataGridViewCheckBoxXColumn colRatioOrder;
    }
}