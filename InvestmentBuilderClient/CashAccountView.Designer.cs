namespace InvestmentBuilderClient
{
    partial class CashAccountView
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
            this.receiptsGrid = new System.Windows.Forms.DataGridView();
            this.receiptsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.cmboDate = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnAddReceipt = new System.Windows.Forms.Button();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDeleteReceipt = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.receiptsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.receiptsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // receiptsGrid
            // 
            this.receiptsGrid.AllowUserToAddRows = false;
            this.receiptsGrid.AllowUserToDeleteRows = false;
            this.receiptsGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.receiptsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.receiptsGrid.Location = new System.Drawing.Point(32, 46);
            this.receiptsGrid.Name = "receiptsGrid";
            this.receiptsGrid.ReadOnly = true;
            this.receiptsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.receiptsGrid.Size = new System.Drawing.Size(478, 289);
            this.receiptsGrid.TabIndex = 0;
            this.receiptsGrid.SelectionChanged += new System.EventHandler(this.OnSelectedTransactionChanged);
            // 
            // cmboDate
            // 
            this.cmboDate.FormattingEnabled = true;
            this.cmboDate.Location = new System.Drawing.Point(383, 15);
            this.cmboDate.Name = "cmboDate";
            this.cmboDate.Size = new System.Drawing.Size(121, 21);
            this.cmboDate.TabIndex = 1;
            this.cmboDate.SelectedIndexChanged += new System.EventHandler(this.OnValuationDateChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(306, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "ValuationDate";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(29, 18);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Receipts";
            // 
            // btnAddReceipt
            // 
            this.btnAddReceipt.Location = new System.Drawing.Point(98, 14);
            this.btnAddReceipt.Name = "btnAddReceipt";
            this.btnAddReceipt.Size = new System.Drawing.Size(75, 23);
            this.btnAddReceipt.TabIndex = 5;
            this.btnAddReceipt.Text = "Add Receipt";
            this.btnAddReceipt.UseVisualStyleBackColor = true;
            this.btnAddReceipt.Click += new System.EventHandler(this.btnAddReceipt_Click);
            // 
            // txtTotal
            // 
            this.txtTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtTotal.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(187, 350);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(178, 26);
            this.txtTotal.TabIndex = 6;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(104, 357);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "TOTAL";
            // 
            // btnDeleteReceipt
            // 
            this.btnDeleteReceipt.Enabled = false;
            this.btnDeleteReceipt.Location = new System.Drawing.Point(181, 14);
            this.btnDeleteReceipt.Name = "btnDeleteReceipt";
            this.btnDeleteReceipt.Size = new System.Drawing.Size(89, 23);
            this.btnDeleteReceipt.TabIndex = 8;
            this.btnDeleteReceipt.Text = "Delete Receipt";
            this.btnDeleteReceipt.UseVisualStyleBackColor = true;
            this.btnDeleteReceipt.Click += new System.EventHandler(this.btnDeleteReceipt_Click);
            // 
            // CashAccountView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(554, 391);
            this.Controls.Add(this.btnDeleteReceipt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.btnAddReceipt);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.cmboDate);
            this.Controls.Add(this.receiptsGrid);
            this.Name = "CashAccountView";
            this.Text = "CashAccountView";
            ((System.ComponentModel.ISupportInitialize)(this.receiptsGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.receiptsBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView receiptsGrid;
        private System.Windows.Forms.BindingSource receiptsBindingSource;
        private System.Windows.Forms.ComboBox cmboDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnAddReceipt;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDeleteReceipt;
    }
}