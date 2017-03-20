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
            this.btnRunBuilder = new System.Windows.Forms.ToolStripButton();
            this.btnViewReport = new System.Windows.Forms.ToolStripButton();
            this.btnManageUsers = new System.Windows.Forms.ToolStripButton();
            this.btnOpenOutputFolder = new System.Windows.Forms.ToolStripButton();
            this.btnAddTrade = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnUndo = new System.Windows.Forms.ToolStripButton();
            this.btnRedemption = new System.Windows.Forms.ToolStripButton();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Font = new System.Drawing.Font("Arial Rounded MT Bold", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.toolStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel2,
            this.cmboAccountName,
            this.toolStripLabel1,
            this.cmboValuationDate,
            this.toolStripSeparator1,
            this.btnConfig,
            this.toolStripSeparator2,
            this.btnRunBuilder,
            this.btnViewReport,
            this.btnManageUsers,
            this.btnOpenOutputFolder,
            this.btnAddTrade,
            this.btnRefresh,
            this.btnUndo,
            this.btnRedemption});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Padding = new System.Windows.Forms.Padding(0, 0, 2, 0);
            this.toolStrip1.Size = new System.Drawing.Size(1558, 39);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // toolStripLabel2
            // 
            this.toolStripLabel2.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.toolStripLabel2.Name = "toolStripLabel2";
            this.toolStripLabel2.Size = new System.Drawing.Size(153, 36);
            this.toolStripLabel2.Text = "Account Name";
            // 
            // cmboAccountName
            // 
            this.cmboAccountName.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.cmboAccountName.Name = "cmboAccountName";
            this.cmboAccountName.Size = new System.Drawing.Size(180, 36);
            this.cmboAccountName.SelectedIndexChanged += new System.EventHandler(this.cmboAccountName_SelectedIndexChanged);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(152, 36);
            this.toolStripLabel1.Text = "Valuation Date";
            // 
            // cmboValuationDate
            // 
            this.cmboValuationDate.Margin = new System.Windows.Forms.Padding(5, 1, 5, 2);
            this.cmboValuationDate.Name = "cmboValuationDate";
            this.cmboValuationDate.Size = new System.Drawing.Size(208, 36);
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
            this.btnConfig.Size = new System.Drawing.Size(28, 36);
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
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnUndo
            // 
            this.btnUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnUndo.Enabled = false;
            this.btnUndo.Image = ((System.Drawing.Image)(resources.GetObject("btnUndo.Image")));
            this.btnUndo.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnUndo.Name = "btnUndo";
            this.btnUndo.Size = new System.Drawing.Size(36, 36);
            this.btnUndo.Text = "toolStripButton1";
            this.btnUndo.ToolTipText = "undo previous transaction";
            this.btnUndo.Click += new System.EventHandler(this.btnUndo_Click);
            // 
            // btnRedemption
            // 
            this.btnRedemption.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRedemption.Image = ((System.Drawing.Image)(resources.GetObject("btnRedemption.Image")));
            this.btnRedemption.ImageScaling = System.Windows.Forms.ToolStripItemImageScaling.None;
            this.btnRedemption.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRedemption.Name = "btnRedemption";
            this.btnRedemption.Size = new System.Drawing.Size(36, 36);
            this.btnRedemption.Text = "Redemptions";
            this.btnRedemption.ToolTipText = "Redemptions";
            this.btnRedemption.Click += new System.EventHandler(this.btnRedemption_Click);
            // 
            // MainView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1558, 538);
            this.Controls.Add(this.toolStrip1);
            this.IsMdiContainer = true;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
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
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnRunBuilder;
        private System.Windows.Forms.ToolStripButton btnConfig;
        private System.Windows.Forms.ToolStripLabel toolStripLabel2;
        private System.Windows.Forms.ToolStripComboBox cmboAccountName;
        private System.Windows.Forms.ToolStripButton btnViewReport;
        private System.Windows.Forms.ToolStripButton btnManageUsers;
        private System.Windows.Forms.ToolStripButton btnOpenOutputFolder;
        private System.Windows.Forms.ToolStripButton btnAddTrade;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnUndo;
        private System.Windows.Forms.ToolStripButton btnRedemption;



    }
}

