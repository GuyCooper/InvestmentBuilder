namespace InvestmentBuilderClient.View
{
    partial class AddTradeView
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
            this.dteTransactionDate = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmboType = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.nmrcNumber = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.txtSymbol = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCcy = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.nmrcScaling = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.txtTotalCost = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.nmrcNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrcScaling)).BeginInit();
            this.SuspendLayout();
            // 
            // dteTransactionDate
            // 
            this.dteTransactionDate.Location = new System.Drawing.Point(176, 24);
            this.dteTransactionDate.Name = "dteTransactionDate";
            this.dteTransactionDate.Size = new System.Drawing.Size(200, 20);
            this.dteTransactionDate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Transaction Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 68);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(90, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Investment Name";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(176, 61);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(200, 20);
            this.txtName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(28, 101);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Action";
            // 
            // cmboType
            // 
            this.cmboType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmboType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmboType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmboType.FormattingEnabled = true;
            this.cmboType.Location = new System.Drawing.Point(176, 101);
            this.cmboType.Name = "cmboType";
            this.cmboType.Size = new System.Drawing.Size(121, 21);
            this.cmboType.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(28, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(80, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Amount Traded";
            // 
            // nmrcNumber
            // 
            this.nmrcNumber.Location = new System.Drawing.Point(177, 136);
            this.nmrcNumber.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nmrcNumber.Name = "nmrcNumber";
            this.nmrcNumber.Size = new System.Drawing.Size(120, 20);
            this.nmrcNumber.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(28, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Symbol";
            // 
            // txtSymbol
            // 
            this.txtSymbol.Location = new System.Drawing.Point(177, 171);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(199, 20);
            this.txtSymbol.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(28, 206);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(49, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Currency";
            // 
            // txtCcy
            // 
            this.txtCcy.Location = new System.Drawing.Point(176, 203);
            this.txtCcy.Name = "txtCcy";
            this.txtCcy.Size = new System.Drawing.Size(100, 20);
            this.txtCcy.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(28, 244);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(75, 13);
            this.label7.TabIndex = 12;
            this.label7.Text = "Scaling Factor";
            // 
            // nmrcScaling
            // 
            this.nmrcScaling.Location = new System.Drawing.Point(176, 237);
            this.nmrcScaling.Name = "nmrcScaling";
            this.nmrcScaling.Size = new System.Drawing.Size(120, 20);
            this.nmrcScaling.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(28, 276);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 13);
            this.label8.TabIndex = 14;
            this.label8.Text = "Total Cost";
            // 
            // txtTotalCost
            // 
            this.txtTotalCost.Location = new System.Drawing.Point(177, 269);
            this.txtTotalCost.Name = "txtTotalCost";
            this.txtTotalCost.Size = new System.Drawing.Size(100, 20);
            this.txtTotalCost.TabIndex = 15;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(360, 291);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(279, 291);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 17;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // AddTradeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(447, 321);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.txtTotalCost);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.nmrcScaling);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.txtCcy);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtSymbol);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.nmrcNumber);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cmboType);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dteTransactionDate);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddTradeView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Add Trade";
            ((System.ComponentModel.ISupportInitialize)(this.nmrcNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrcScaling)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dteTransactionDate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmboType;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown nmrcNumber;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtSymbol;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCcy;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown nmrcScaling;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtTotalCost;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button button2;
    }
}