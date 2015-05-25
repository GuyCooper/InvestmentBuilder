namespace InvestmentBuilderClient.View
{
    partial class AddIndexView
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtSymbol = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.lblCheckResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(18, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Symbol";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(76, 14);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(232, 20);
            this.txtName.TabIndex = 2;
            // 
            // txtSymbol
            // 
            this.txtSymbol.Location = new System.Drawing.Point(76, 54);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(230, 20);
            this.txtSymbol.TabIndex = 3;
            // 
            // button1
            // 
            this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.button1.Location = new System.Drawing.Point(188, 108);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(57, 23);
            this.button1.TabIndex = 4;
            this.button1.Text = "Cancel";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button2.Location = new System.Drawing.Point(251, 108);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(57, 23);
            this.button2.TabIndex = 5;
            this.button2.Text = "OK";
            this.button2.UseVisualStyleBackColor = true;
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(76, 108);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(57, 23);
            this.btnCheck.TabIndex = 6;
            this.btnCheck.Text = "Check";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // lblCheckResult
            // 
            this.lblCheckResult.AutoSize = true;
            this.lblCheckResult.Location = new System.Drawing.Point(75, 88);
            this.lblCheckResult.Name = "lblCheckResult";
            this.lblCheckResult.Size = new System.Drawing.Size(0, 13);
            this.lblCheckResult.TabIndex = 7;
            // 
            // AddIndexView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(335, 139);
            this.ControlBox = false;
            this.Controls.Add(this.lblCheckResult);
            this.Controls.Add(this.btnCheck);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtSymbol);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "AddIndexView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Add Index";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtSymbol;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.Label lblCheckResult;
    }
}