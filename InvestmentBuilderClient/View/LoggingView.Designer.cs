namespace InvestmentBuilderClient.View
{
    partial class LoggingView
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
            this.listLogging = new System.Windows.Forms.ListView();
            this.LogLevel = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.Message = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.SuspendLayout();
            // 
            // listLogging
            // 
            this.listLogging.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listLogging.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.LogLevel,
            this.Message});
            this.listLogging.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listLogging.Location = new System.Drawing.Point(0, 0);
            this.listLogging.Name = "listLogging";
            this.listLogging.Size = new System.Drawing.Size(468, 260);
            this.listLogging.TabIndex = 0;
            this.listLogging.UseCompatibleStateImageBehavior = false;
            this.listLogging.View = System.Windows.Forms.View.Details;
            // 
            // LogLevel
            // 
            this.LogLevel.Text = "Log Level";
            this.LogLevel.Width = 100;
            // 
            // Message
            // 
            this.Message.Text = "Message";
            this.Message.Width = 400;
            // 
            // LoggingView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(470, 261);
            this.Controls.Add(this.listLogging);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoggingView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Logging";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView listLogging;
        private System.Windows.Forms.ColumnHeader LogLevel;
        private System.Windows.Forms.ColumnHeader Message;
    }
}