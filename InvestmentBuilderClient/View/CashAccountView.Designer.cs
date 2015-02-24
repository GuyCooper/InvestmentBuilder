namespace InvestmentBuilderClient.View
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.cashAccountGrid = new System.Windows.Forms.DataGridView();
            this.cashAccountBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnAddTransaction = new System.Windows.Forms.Button();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnDeleteTransaction = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.cashAccountGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashAccountBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // cashAccountGrid
            // 
            this.cashAccountGrid.AllowUserToAddRows = false;
            this.cashAccountGrid.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.cashAccountGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.cashAccountGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cashAccountGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.ColumnHeader;
            this.cashAccountGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.cashAccountGrid.Location = new System.Drawing.Point(-4, 1);
            this.cashAccountGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cashAccountGrid.Name = "cashAccountGrid";
            this.cashAccountGrid.ReadOnly = true;
            this.cashAccountGrid.RowTemplate.DefaultCellStyle.BackColor = System.Drawing.SystemColors.Control;
            this.cashAccountGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.cashAccountGrid.Size = new System.Drawing.Size(654, 270);
            this.cashAccountGrid.TabIndex = 0;
            this.cashAccountGrid.SelectionChanged += new System.EventHandler(this.OnSelectedTransactionChanged);
            // 
            // btnAddTransaction
            // 
            this.btnAddTransaction.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnAddTransaction.Location = new System.Drawing.Point(96, 301);
            this.btnAddTransaction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnAddTransaction.Name = "btnAddTransaction";
            this.btnAddTransaction.Size = new System.Drawing.Size(88, 27);
            this.btnAddTransaction.TabIndex = 5;
            this.btnAddTransaction.Text = "Add Transaction";
            this.btnAddTransaction.UseVisualStyleBackColor = true;
            this.btnAddTransaction.Click += new System.EventHandler(this.btnAddTransaction_Click);
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotal.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.txtTotal.Font = new System.Drawing.Font("Arial Rounded MT Bold", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtTotal.Location = new System.Drawing.Point(427, 302);
            this.txtTotal.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.ReadOnly = true;
            this.txtTotal.Size = new System.Drawing.Size(207, 26);
            this.txtTotal.TabIndex = 6;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label3.Location = new System.Drawing.Point(362, 307);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 16);
            this.label3.TabIndex = 7;
            this.label3.Text = "TOTAL";
            // 
            // btnDeleteTransaction
            // 
            this.btnDeleteTransaction.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.btnDeleteTransaction.Enabled = false;
            this.btnDeleteTransaction.Location = new System.Drawing.Point(215, 301);
            this.btnDeleteTransaction.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.btnDeleteTransaction.Name = "btnDeleteTransaction";
            this.btnDeleteTransaction.Size = new System.Drawing.Size(104, 27);
            this.btnDeleteTransaction.TabIndex = 8;
            this.btnDeleteTransaction.Text = "Delete Transaction";
            this.btnDeleteTransaction.UseVisualStyleBackColor = true;
            this.btnDeleteTransaction.Click += new System.EventHandler(this.btnDeleteTransaction_Click);
            // 
            // CashAccountView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 338);
            this.Controls.Add(this.btnDeleteTransaction);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtTotal);
            this.Controls.Add(this.btnAddTransaction);
            this.Controls.Add(this.cashAccountGrid);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "CashAccountView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "CashAccountView";
            ((System.ComponentModel.ISupportInitialize)(this.cashAccountGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cashAccountBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        protected System.Windows.Forms.DataGridView cashAccountGrid;
        protected System.Windows.Forms.BindingSource cashAccountBindingSource;
        private System.Windows.Forms.Button btnAddTransaction;
        private System.Windows.Forms.TextBox txtTotal;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnDeleteTransaction;
    }
}