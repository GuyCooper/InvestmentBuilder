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
            ((System.ComponentModel.ISupportInitialize)(this.receiptsGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.receiptsBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // receiptsGrid
            // 
            this.receiptsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.receiptsGrid.Location = new System.Drawing.Point(21, 17);
            this.receiptsGrid.Name = "receiptsGrid";
            this.receiptsGrid.Size = new System.Drawing.Size(326, 232);
            this.receiptsGrid.TabIndex = 0;
            // 
            // cmboDate
            // 
            this.cmboDate.FormattingEnabled = true;
            this.cmboDate.Location = new System.Drawing.Point(399, 43);
            this.cmboDate.Name = "cmboDate";
            this.cmboDate.Size = new System.Drawing.Size(121, 21);
            this.cmboDate.TabIndex = 1;
            this.cmboDate.SelectedValueChanged += new System.EventHandler(this.OnSelectedDateChange);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(526, 43);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "ValuationDate";
            // 
            // CashAccountView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(660, 261);
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
    }
}