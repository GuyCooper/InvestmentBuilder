namespace InvestmentBuilderClient.View
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
            this.toolStripLabel2 = new System.Windows.Forms.ToolStripLabel();
            this.cmboAccountName = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.cmboValuationDate = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnConfig = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCommitData = new System.Windows.Forms.ToolStripButton();
            this.btnRunBuilder = new System.Windows.Forms.ToolStripButton();
            this.btnPerformance = new System.Windows.Forms.ToolStripButton();
            this.btnViewReport = new System.Windows.Forms.ToolStripButton();
            this.btnManageUsers = new System.Windows.Forms.ToolStripButton();
            this.btnOpenOutputFolder = new System.Windows.Forms.ToolStripButton();
            this.btnAddTrade = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.cmboAccountName,
            this.toolStripLabel1,
            this.cmboValuationDate,
            this.toolStripSeparator1,
            this.btnConfig,
            this.toolStripSeparator2,
            this.btnCommitData,
            this.btnRunBuilder,
            this.btnPerformance,
            this.btnViewReport,
            this.btnManageUsers,
            this.btnOpenOutputFolder,
            this.btnAddTrade,
            this.btnRefresh});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(994, 39);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(102, 36);
            this.toolStripLabel2.Text = "Account Name";
            // 
            // cmboAccountName
            // 
            this.cmboAccountName.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.cmboAccountName.Name = "cmboAccountName";
            this.cmboAccountName.Size = new System.Drawing.Size(121, 36);
            this.cmboAccountName.SelectedIndexChanged += new System.EventHandler(this.cmboAccountName_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(103, 36);
            this.toolStripLabel1.Text = "Valuation Date";
            // 
            // cmboValuationDate
            // 
            this.cmboValuationDate.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.cmboValuationDate.Name = "cmboValuationDate";
            this.cmboValuationDate.Size = new System.Drawing.Size(140, 36);
            this.cmboValuationDate.SelectedIndexChanged += new System.EventHandler(this.OnValuationDateChanged);
            this.cmboValuationDate.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.OnValuationDateEnter);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 36);
            // 
            // btnConfig
            // 
            this.btnConfig.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnConfig.Image = ((System.Drawing.Image)(resources.GetObject("btnConfig.Image")));
            this.btnConfig.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnConfig.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnConfig.Name = "btnConfig";
            this.btnConfig.Size = new System.Drawing.Size(23, 36);
            this.btnConfig.Text = "toolStripButton1";
            this.btnConfig.ToolTipText = "Configure";
            this.btnConfig.Click += new System.EventHandler(this.btnConfig_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 36);
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
            // btnPerformance
            // 
            this.btnPerformance.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPerformance.Image = ((System.Drawing.Image)(resources.GetObject("btnPerformance.Image")));
            this.btnPerformance.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnPerformance.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPerformance.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnPerformance.Name = "btnPerformance";
            this.btnPerformance.Size = new System.Drawing.Size(36, 36);
            this.btnPerformance.Text = "toolStripButton1";
            this.btnPerformance.ToolTipText = "Build Performance Charts";
            this.btnPerformance.Click += new System.EventHandler(this.btnPerformance_Click);
            // 
            // btnViewReport
            // 
            this.btnViewReport.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnViewReport.Image = ((System.Drawing.Image)(resources.GetObject("btnViewReport.Image")));
            this.btnViewReport.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnViewReport.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnViewReport.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnViewReport.Name = "btnViewReport";
            this.btnViewReport.Size = new System.Drawing.Size(36, 36);
            this.btnViewReport.Text = "toolStripButton1";
            this.btnViewReport.ToolTipText = "view report";
            this.btnViewReport.Click += new System.EventHandler(this.btnViewReport_Click);
            // 
            // btnManageUsers
            // 
            this.btnManageUsers.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnManageUsers.Image = ((System.Drawing.Image)(resources.GetObject("btnManageUsers.Image")));
            this.btnManageUsers.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnManageUsers.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnManageUsers.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnManageUsers.Name = "btnManageUsers";
            this.btnManageUsers.Size = new System.Drawing.Size(36, 36);
            this.btnManageUsers.ToolTipText = "Manage Users";
            this.btnManageUsers.Click += new System.EventHandler(this.btnManageUsers_Click);
            // 
            // btnOpenOutputFolder
            // 
            this.btnOpenOutputFolder.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpenOutputFolder.Image = ((System.Drawing.Image)(resources.GetObject("btnOpenOutputFolder.Image")));
            this.btnOpenOutputFolder.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnOpenOutputFolder.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpenOutputFolder.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnOpenOutputFolder.Name = "btnOpenOutputFolder";
            this.btnOpenOutputFolder.Size = new System.Drawing.Size(36, 36);
            this.btnOpenOutputFolder.Text = "Open Output Folder";
            this.btnOpenOutputFolder.Click += new System.EventHandler(this.btnOpenOutputFolder_Click);
            // 
            // btnAddTrade
            // 
            this.btnAddTrade.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAddTrade.Image = ((System.Drawing.Image)(resources.GetObject("btnAddTrade.Image")));
            this.btnAddTrade.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnAddTrade.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAddTrade.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.btnAddTrade.Name = "btnAddTrade";
            this.btnAddTrade.Size = new System.Drawing.Size(36, 36);
            this.btnAddTrade.Text = "toolStripButton1";
            this.btnAddTrade.ToolTipText = "Add Trade";
            this.btnAddTrade.Click += new System.EventHandler(this.btnAddTrade_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(36, 36);
            this.btnRefresh.Text = "toolStripButton1";
            this.btnRefresh.ToolTipText = "refresh";
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(994, 350);
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
        private System.Windows.Forms.ToolStripButton btnConfig;
        private System.Windows.Forms.ToolStripButton btnPerformance;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox cmboAccountName;
        private System.Windows.Forms.ToolStripButton btnViewReport;
        private System.Windows.Forms.ToolStripButton btnManageUsers;
        private System.Windows.Forms.ToolStripButton btnOpenOutputFolder;
        private System.Windows.Forms.ToolStripButton btnAddTrade;
        private System.Windows.Forms.ToolStripButton btnRefresh;



    }
}

