namespace InvestmentBuilderClient.View
{
    partial class RedemptionsView
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
            this.gridRedemptions = new System.Windows.Forms.DataGridView();
            this.btnAddRedemption = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gridRedemptions)).BeginInit();
            this.SuspendLayout();
            // 
            // gridRedemptions
            // 
            this.gridRedemptions.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.gridRedemptions.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridRedemptions.Location = new System.Drawing.Point(-3, 1);
            this.gridRedemptions.Name = "gridRedemptions";
            this.gridRedemptions.Size = new System.Drawing.Size(599, 207);
            this.gridRedemptions.TabIndex = 0;
            // 
            // btnAddRedemption
            // 
            this.btnAddRedemption.Location = new System.Drawing.Point(474, 226);
            this.btnAddRedemption.Name = "btnAddRedemption";
            this.btnAddRedemption.Size = new System.Drawing.Size(108, 23);
            this.btnAddRedemption.TabIndex = 1;
            this.btnAddRedemption.Text = "Add Redemption";
            this.btnAddRedemption.UseVisualStyleBackColor = true;
            this.btnAddRedemption.Click += new System.EventHandler(this.btnAddRedemption_Click);
            // 
            // RedemptionsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(594, 261);
            this.Controls.Add(this.btnAddRedemption);
            this.Controls.Add(this.gridRedemptions);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "RedemptionsView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "RedemptionsView";
            ((System.ComponentModel.ISupportInitialize)(this.gridRedemptions)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridRedemptions;
        private System.Windows.Forms.Button btnAddRedemption;
    }
}