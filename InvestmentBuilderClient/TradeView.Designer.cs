﻿namespace InvestmentBuilderClient
{
    partial class TradeView
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
            this.gridTrades = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.tradeViewBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.btnRemoveTrade = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridTrades)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeViewBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // gridTrades
            // 
            this.gridTrades.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridTrades.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridTrades.Location = new System.Drawing.Point(-1, -1);
            this.gridTrades.Name = "gridTrades";
            this.gridTrades.Size = new System.Drawing.Size(760, 271);
            this.gridTrades.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(357, 280);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Add Trade";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnRemoveTrade
            // 
            this.btnRemoveTrade.Location = new System.Drawing.Point(263, 280);
            this.btnRemoveTrade.Name = "btnRemoveTrade";
            this.btnRemoveTrade.Size = new System.Drawing.Size(88, 23);
            this.btnRemoveTrade.TabIndex = 2;
            this.btnRemoveTrade.Text = "Remove Trade";
            this.btnRemoveTrade.UseVisualStyleBackColor = true;
            this.btnRemoveTrade.Click += new System.EventHandler(this.btnRemoveTrade_Click);
            // 
            // TradeView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(757, 315);
            this.Controls.Add(this.btnRemoveTrade);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.gridTrades);
            this.Name = "TradeView";
            this.Text = "TradeView";
            ((System.ComponentModel.ISupportInitialize)(this.gridTrades)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tradeViewBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridTrades;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.BindingSource tradeViewBindingSource;
        private System.Windows.Forms.Button btnRemoveTrade;
    }
}