namespace InvestmentBuilderClient
{
    partial class MainView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainView));
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmboValuationDate = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCommitData = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRunBuilder = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.cmboValuationDate,
            this.toolStripSeparator1,
            this.btnCommitData,
            this.toolStripSeparator2,
            this.btnRunBuilder});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(694, 39);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(84, 36);
            this.toolStripLabel1.Text = "Valuation Date";
            // 
            // cmboValuationDate
            // 
            this.cmboValuationDate.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.cmboValuationDate.Name = "cmboValuationDate";
            this.cmboValuationDate.Size = new System.Drawing.Size(140, 39);
            this.cmboValuationDate.SelectedIndexChanged += new System.EventHandler(this.OnValuationDateChanged);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // btnCommitData
            // 
            this.btnCommitData.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCommitData.Image = ((System.Drawing.Image)(resources.GetObject("btnCommitData.Image")));
            this.btnCommitData.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnCommitData.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCommitData.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnCommitData.Name = "btnCommitData";
            this.btnCommitData.Size = new System.Drawing.Size(36, 36);
            this.btnCommitData.Text = "toolStripButton1";
            this.btnCommitData.ToolTipText = "Commit Data";
            this.btnCommitData.Click += new System.EventHandler(this.btnCommitData_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // btnRunBuilder
            // 
            this.btnRunBuilder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRunBuilder.Image = ((System.Drawing.Image)(resources.GetObject("btnRunBuilder.Image")));
            this.btnRunBuilder.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRunBuilder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRunBuilder.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnRunBuilder.Name = "btnRunBuilder";
            this.btnRunBuilder.Size = new System.Drawing.Size(36, 36);
            this.btnRunBuilder.Text = "toolStripButton2";
            this.btnRunBuilder.ToolTipText = "Run Builder";
            this.btnRunBuilder.Click += new System.EventHandler(this.btnRunBuilder_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 350);
            this.Controls.Add(this.toolStrip1);
            this.IsMdiContainer = true;
            this.Name = "MainView";
            this.Text = "Investment Builder";
            this.Load += new System.EventHandler(this.MainView_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cmboValuationDate;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnCommitData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnRunBuilder;



    }
}

