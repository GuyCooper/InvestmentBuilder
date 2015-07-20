namespace InvestmentBuilderClient.View
{
    partial class PortfolioView
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
            this.gridPortfolio = new System.Windows.Forms.DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.gridPortfolio)).BeginInit();
            this.SuspendLayout();
            // 
            // gridPortfolio
            // 
            this.gridPortfolio.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gridPortfolio.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridPortfolio.Location = new System.Drawing.Point(0, 0);
            this.gridPortfolio.Name = "gridPortfolio";
            this.gridPortfolio.Size = new System.Drawing.Size(787, 261);
            this.gridPortfolio.TabIndex = 0;
            // 
            // PortfolioView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(787, 261);
            this.Controls.Add(this.gridPortfolio);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PortfolioView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "PortfolioView";
            ((System.ComponentModel.ISupportInitialize)(this.gridPortfolio)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gridPortfolio;
    }
}