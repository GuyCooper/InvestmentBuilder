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
            this.label9 = new System.Windows.Forms.Label();
            this.txtExchange = new System.Windows.Forms.TextBox();
            this.btnCheck = new System.Windows.Forms.Button();
            this.lblCheckResult = new System.Windows.Forms.Label();
            this.chkSellAll = new System.Windows.Forms.CheckBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtManualPrice = new System.Windows.Forms.TextBox();
            this.cmboName = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtSource = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.nmrcNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nmrcScaling)).BeginInit();
            this.SuspendLayout();
            // 
            // dteTransactionDate
            // 
            this.dteTransactionDate.Location = new System.Drawing.Point(469, 57);
            this.dteTransactionDate.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.dteTransactionDate.Name = "dteTransactionDate";
            this.dteTransactionDate.Size = new System.Drawing.Size(527, 38);
            this.dteTransactionDate.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(75, 57);
            this.label1.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(231, 32);
            this.label1.TabIndex = 1;
            this.label1.Text = "Transaction Date";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(75, 162);
            this.label2.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(235, 32);
            this.label2.TabIndex = 2;
            this.label2.Text = "Investment Name";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(75, 241);
            this.label3.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(95, 32);
            this.label3.TabIndex = 4;
            this.label3.Text = "Action";
            // 
            // cmboType
            // 
            this.cmboType.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.cmboType.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmboType.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.cmboType.FormattingEnabled = true;
            this.cmboType.Location = new System.Drawing.Point(469, 241);
            this.cmboType.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.cmboType.Name = "cmboType";
            this.cmboType.Size = new System.Drawing.Size(316, 39);
            this.cmboType.TabIndex = 5;
            this.cmboType.SelectedIndexChanged += new System.EventHandler(this.cmboType_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(75, 324);
            this.label4.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(210, 32);
            this.label4.TabIndex = 6;
            this.label4.Text = "Amount Traded";
            // 
            // nmrcNumber
            // 
            this.nmrcNumber.Location = new System.Drawing.Point(472, 324);
            this.nmrcNumber.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.nmrcNumber.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.nmrcNumber.Name = "nmrcNumber";
            this.nmrcNumber.Size = new System.Drawing.Size(320, 38);
            this.nmrcNumber.TabIndex = 7;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(75, 408);
            this.label5.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(110, 32);
            this.label5.TabIndex = 8;
            this.label5.Text = "Symbol";
            // 
            // txtSymbol
            // 
            this.txtSymbol.Location = new System.Drawing.Point(472, 408);
            this.txtSymbol.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(524, 38);
            this.txtSymbol.TabIndex = 9;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(75, 568);
            this.label6.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(129, 32);
            this.label6.TabIndex = 10;
            this.label6.Text = "Currency";
            // 
            // txtCcy
            // 
            this.txtCcy.Location = new System.Drawing.Point(469, 560);
            this.txtCcy.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.txtCcy.Name = "txtCcy";
            this.txtCcy.Size = new System.Drawing.Size(260, 38);
            this.txtCcy.TabIndex = 11;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(75, 644);
            this.label7.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(197, 32);
            this.label7.TabIndex = 12;
            this.label7.Text = "Scaling Factor";
            // 
            // nmrcScaling
            // 
            this.nmrcScaling.Location = new System.Drawing.Point(469, 639);
            this.nmrcScaling.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.nmrcScaling.Name = "nmrcScaling";
            this.nmrcScaling.Size = new System.Drawing.Size(320, 38);
            this.nmrcScaling.TabIndex = 13;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(75, 723);
            this.label8.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(144, 32);
            this.label8.TabIndex = 14;
            this.label8.Text = "Total Cost";
            // 
            // txtTotalCost
            // 
            this.txtTotalCost.Location = new System.Drawing.Point(469, 715);
            this.txtTotalCost.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.txtTotalCost.Name = "txtTotalCost";
            this.txtTotalCost.Size = new System.Drawing.Size(260, 38);
            this.txtTotalCost.TabIndex = 15;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(975, 958);
            this.btnOK.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(200, 55);
            this.btnOK.TabIndex = 16;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button2.Location = new System.Drawing.Point(746, 958);
            this.button2.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(200, 55);
            this.button2.TabIndex = 17;
            this.button2.Text = "Cancel";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(75, 486);
            this.label9.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(269, 32);
            this.label9.TabIndex = 18;
            this.label9.Text = "Exchange (optional)";
            // 
            // txtExchange
            // 
            this.txtExchange.Location = new System.Drawing.Point(472, 479);
            this.txtExchange.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.txtExchange.Name = "txtExchange";
            this.txtExchange.Size = new System.Drawing.Size(420, 38);
            this.txtExchange.TabIndex = 19;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(17, 958);
            this.btnCheck.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(173, 55);
            this.btnCheck.TabIndex = 20;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // lblCheckResult
            // 
            this.lblCheckResult.AutoSize = true;
            this.lblCheckResult.Location = new System.Drawing.Point(280, 827);
            this.lblCheckResult.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.lblCheckResult.Name = "lblCheckResult";
            this.lblCheckResult.Size = new System.Drawing.Size(0, 32);
            this.lblCheckResult.TabIndex = 21;
            // 
            // chkSellAll
            // 
            this.chkSellAll.AutoSize = true;
            this.chkSellAll.Enabled = false;
            this.chkSellAll.Location = new System.Drawing.Point(832, 246);
            this.chkSellAll.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.chkSellAll.Name = "chkSellAll";
            this.chkSellAll.Size = new System.Drawing.Size(142, 36);
            this.chkSellAll.TabIndex = 22;
            this.chkSellAll.Text = "Sell All";
            this.chkSellAll.UseVisualStyleBackColor = true;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(75, 811);
            this.label10.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(181, 32);
            this.label10.TabIndex = 23;
            this.label10.Text = "Manual Price";
            // 
            // txtManualPrice
            // 
            this.txtManualPrice.Location = new System.Drawing.Point(469, 794);
            this.txtManualPrice.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.txtManualPrice.Name = "txtManualPrice";
            this.txtManualPrice.Size = new System.Drawing.Size(260, 38);
            this.txtManualPrice.TabIndex = 24;
            // 
            // cmboName
            // 
            this.cmboName.FormattingEnabled = true;
            this.cmboName.Location = new System.Drawing.Point(467, 155);
            this.cmboName.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.cmboName.Name = "cmboName";
            this.cmboName.Size = new System.Drawing.Size(529, 39);
            this.cmboName.TabIndex = 25;
            this.cmboName.SelectedIndexChanged += new System.EventHandler(this.cmboName_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(75, 881);
            this.label11.Margin = new System.Windows.Forms.Padding(8, 0, 8, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(238, 32);
            this.label11.TabIndex = 26;
            this.label11.Text = "Source (Optional)";
            // 
            // txtSource
            // 
            this.txtSource.Location = new System.Drawing.Point(467, 875);
            this.txtSource.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
            this.txtSource.Name = "txtSource";
            this.txtSource.Size = new System.Drawing.Size(420, 38);
            this.txtSource.TabIndex = 27;
            // 
            // AddTradeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(16F, 31F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1192, 1068);
            this.Controls.Add(this.txtSource);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.cmboName);
            this.Controls.Add(this.txtManualPrice);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.chkSellAll);
            this.Controls.Add(this.lblCheckResult);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.txtExchange);
            this.Controls.Add(this.label9);
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
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dteTransactionDate);
            this.Margin = new System.Windows.Forms.Padding(8, 7, 8, 7);
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
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtExchange;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label lblCheckResult;
        private System.Windows.Forms.CheckBox chkSellAll;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtManualPrice;
        private System.Windows.Forms.ComboBox cmboName;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtSource;
    }
}