namespace InvestmentBuilderClient.View
{
    partial class ManageUserView
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
            this.txtAccountName = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkEnableAccount = new System.Windows.Forms.CheckBox();
            this.btnAddAccount = new System.Windows.Forms.Button();
            this.btnAddMember = new System.Windows.Forms.Button();
            this.btnRemove = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.txtCurrency = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.cmboType = new System.Windows.Forms.ComboBox();
            this.lstVwMembers = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colRole = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnEdit = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.cmboBroker = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Account Name";
            // 
            // txtAccountName
            // 
            this.txtAccountName.Location = new System.Drawing.Point(129, 33);
            this.txtAccountName.Name = "txtAccountName";
            this.txtAccountName.Size = new System.Drawing.Size(244, 20);
            this.txtAccountName.TabIndex = 1;
            this.txtAccountName.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtAccountName_KeyPress);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 299);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(50, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Members";
            // 
            // chkEnableAccount
            // 
            this.chkEnableAccount.AutoSize = true;
            this.chkEnableAccount.Location = new System.Drawing.Point(129, 267);
            this.chkEnableAccount.Name = "chkEnableAccount";
            this.chkEnableAccount.Size = new System.Drawing.Size(102, 17);
            this.chkEnableAccount.TabIndex = 4;
            this.chkEnableAccount.Text = "Enable Account";
            this.chkEnableAccount.UseVisualStyleBackColor = true;
            // 
            // btnAddAccount
            // 
            this.btnAddAccount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddAccount.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnAddAccount.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAddAccount.Location = new System.Drawing.Point(231, 437);
            this.btnAddAccount.Name = "btnAddAccount";
            this.btnAddAccount.Size = new System.Drawing.Size(139, 44);
            this.btnAddAccount.TabIndex = 5;
            this.btnAddAccount.Text = "Add / Update Account";
            this.btnAddAccount.UseVisualStyleBackColor = true;
            // 
            // btnAddMember
            // 
            this.btnAddMember.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.btnAddMember.Location = new System.Drawing.Point(28, 335);
            this.btnAddMember.Name = "btnAddMember";
            this.btnAddMember.Size = new System.Drawing.Size(75, 23);
            this.btnAddMember.TabIndex = 6;
            this.btnAddMember.Text = "Add";
            this.btnAddMember.UseVisualStyleBackColor = true;
            this.btnAddMember.Click += new System.EventHandler(this.btnAddMember_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.Location = new System.Drawing.Point(28, 364);
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(75, 23);
            this.btnRemove.TabIndex = 7;
            this.btnRemove.Text = "Remove";
            this.btnRemove.UseVisualStyleBackColor = true;
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(22, 78);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 8;
            this.label3.Text = "Description";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(22, 139);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Password";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(22, 207);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Reporting Currency";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(129, 71);
            this.txtDescription.Multiline = true;
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(241, 56);
            this.txtDescription.TabIndex = 11;
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(129, 139);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(241, 20);
            this.txtPassword.TabIndex = 12;
            // 
            // txtCurrency
            // 
            this.txtCurrency.Location = new System.Drawing.Point(129, 204);
            this.txtCurrency.Name = "txtCurrency";
            this.txtCurrency.Size = new System.Drawing.Size(100, 20);
            this.txtCurrency.TabIndex = 13;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(22, 175);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 14;
            this.label6.Text = "Account Type";
            // 
            // cmboType
            // 
            this.cmboType.FormattingEnabled = true;
            this.cmboType.Location = new System.Drawing.Point(129, 172);
            this.cmboType.Name = "cmboType";
            this.cmboType.Size = new System.Drawing.Size(121, 21);
            this.cmboType.TabIndex = 15;
            // 
            // lstVwMembers
            // 
            this.lstVwMembers.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colRole});
            this.lstVwMembers.Location = new System.Drawing.Point(129, 299);
            this.lstVwMembers.Name = "lstVwMembers";
            this.lstVwMembers.Size = new System.Drawing.Size(244, 132);
            this.lstVwMembers.TabIndex = 16;
            this.lstVwMembers.UseCompatibleStateImageBehavior = false;
            this.lstVwMembers.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            // 
            // colRole
            // 
            this.colRole.Text = "Role";
            // 
            // btnEdit
            // 
            this.btnEdit.Location = new System.Drawing.Point(28, 393);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(75, 23);
            this.btnEdit.TabIndex = 17;
            this.btnEdit.Text = "Edit";
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(22, 238);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(38, 13);
            this.label7.TabIndex = 18;
            this.label7.Text = "Broker";
            // 
            // cmboBroker
            // 
            this.cmboBroker.FormattingEnabled = true;
            this.cmboBroker.Location = new System.Drawing.Point(129, 235);
            this.cmboBroker.Name = "cmboBroker";
            this.cmboBroker.Size = new System.Drawing.Size(241, 21);
            this.cmboBroker.TabIndex = 19;
            // 
            // ManageUserView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(382, 492);
            this.Controls.Add(this.cmboBroker);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.lstVwMembers);
            this.Controls.Add(this.cmboType);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtCurrency);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtDescription);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnRemove);
            this.Controls.Add(this.btnAddMember);
            this.Controls.Add(this.btnAddAccount);
            this.Controls.Add(this.chkEnableAccount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtAccountName);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ManageUserView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Manage Accounts";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtAccountName;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkEnableAccount;
        private System.Windows.Forms.Button btnAddAccount;
        private System.Windows.Forms.Button btnAddMember;
        private System.Windows.Forms.Button btnRemove;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtDescription;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.TextBox txtCurrency;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox cmboType;
        private System.Windows.Forms.ListView lstVwMembers;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colRole;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmboBroker;
    }
}