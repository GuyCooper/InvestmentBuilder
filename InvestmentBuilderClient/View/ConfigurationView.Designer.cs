namespace InvestmentBuilderClient.View
{
    partial class ConfigurationView
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
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.label2 = new System.Windows.Forms.Label();
            this.txtDataSource = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOutputFolder = new System.Windows.Forms.TextBox();
            this.btnSelectOutputFolder = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.label1 = new System.Windows.Forms.Label();
            this.listIndexes = new System.Windows.Forms.ListView();
            this.colName = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.colSymbol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnAddIndex = new System.Windows.Forms.Button();
            this.btnRemoveIndex = new System.Windows.Forms.Button();
            this.btnSaveChanges = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Data Source Name";
            // 
            // txtDataSource
            // 
            this.txtDataSource.Location = new System.Drawing.Point(137, 31);
            this.txtDataSource.Name = "txtDataSource";
            this.txtDataSource.Size = new System.Drawing.Size(474, 20);
            this.txtDataSource.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 73);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(71, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Output Folder";
            // 
            // txtOutputFolder
            // 
            this.txtOutputFolder.Location = new System.Drawing.Point(137, 66);
            this.txtOutputFolder.Name = "txtOutputFolder";
            this.txtOutputFolder.Size = new System.Drawing.Size(426, 20);
            this.txtOutputFolder.TabIndex = 5;
            // 
            // btnSelectOutputFolder
            // 
            this.btnSelectOutputFolder.Location = new System.Drawing.Point(581, 68);
            this.btnSelectOutputFolder.Name = "btnSelectOutputFolder";
            this.btnSelectOutputFolder.Size = new System.Drawing.Size(30, 23);
            this.btnSelectOutputFolder.TabIndex = 7;
            this.btnSelectOutputFolder.Text = "...";
            this.btnSelectOutputFolder.UseVisualStyleBackColor = true;
            this.btnSelectOutputFolder.Click += new System.EventHandler(this.btnSelectOutputFolder_Click);
            // 
            // button3
            // 
            this.button3.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.button3.Location = new System.Drawing.Point(536, 268);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 8;
            this.button3.Text = "OK";
            this.button3.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 104);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(44, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Indexes";
            // 
            // listIndexes
            // 
            this.listIndexes.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.colName,
            this.colSymbol});
            this.listIndexes.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listIndexes.Location = new System.Drawing.Point(6, 19);
            this.listIndexes.Name = "listIndexes";
            this.listIndexes.Size = new System.Drawing.Size(236, 120);
            this.listIndexes.TabIndex = 10;
            this.listIndexes.UseCompatibleStateImageBehavior = false;
            this.listIndexes.View = System.Windows.Forms.View.Details;
            // 
            // colName
            // 
            this.colName.Text = "Name";
            // 
            // colSymbol
            // 
            this.colSymbol.Text = "Symbol";
            // 
            // groupBox1
            // 
            this.groupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.groupBox1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.groupBox1.Controls.Add(this.btnRemoveIndex);
            this.groupBox1.Controls.Add(this.btnAddIndex);
            this.groupBox1.Controls.Add(this.listIndexes);
            this.groupBox1.Location = new System.Drawing.Point(137, 104);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(372, 164);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Historical Chart Indexes";
            // 
            // btnAddIndex
            // 
            this.btnAddIndex.Location = new System.Drawing.Point(269, 38);
            this.btnAddIndex.Name = "btnAddIndex";
            this.btnAddIndex.Size = new System.Drawing.Size(75, 23);
            this.btnAddIndex.TabIndex = 11;
            this.btnAddIndex.Text = "Add";
            this.btnAddIndex.UseVisualStyleBackColor = true;
            // 
            // btnRemoveIndex
            // 
            this.btnRemoveIndex.Location = new System.Drawing.Point(269, 89);
            this.btnRemoveIndex.Name = "btnRemoveIndex";
            this.btnRemoveIndex.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveIndex.TabIndex = 12;
            this.btnRemoveIndex.Text = "Remove Index";
            this.btnRemoveIndex.UseVisualStyleBackColor = true;
            // 
            // btnSaveChanges
            // 
            this.btnSaveChanges.Location = new System.Drawing.Point(26, 268);
            this.btnSaveChanges.Name = "btnSaveChanges";
            this.btnSaveChanges.Size = new System.Drawing.Size(75, 23);
            this.btnSaveChanges.TabIndex = 13;
            this.btnSaveChanges.Text = "Save to File";
            this.btnSaveChanges.UseVisualStyleBackColor = true;
            // 
            // ConfigurationView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(623, 303);
            this.Controls.Add(this.btnSaveChanges);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnSelectOutputFolder);
            this.Controls.Add(this.txtOutputFolder);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDataSource);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConfigurationView";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Configure Investment Builder";
            this.groupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtDataSource;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtOutputFolder;
        private System.Windows.Forms.Button btnSelectOutputFolder;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ListView listIndexes;
        private System.Windows.Forms.ColumnHeader colName;
        private System.Windows.Forms.ColumnHeader colSymbol;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnRemoveIndex;
        private System.Windows.Forms.Button btnAddIndex;
        private System.Windows.Forms.Button btnSaveChanges;
    }
}