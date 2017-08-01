namespace DeepRemove
{
    partial class deepRemoveUIfrm
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
            this.selectFolderRoot = new System.Windows.Forms.Button();
            this.rootFolder = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.enableDeepRemove = new System.Windows.Forms.CheckBox();
            this.doDeepRemove = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.filesDeleted = new System.Windows.Forms.TextBox();
            this.foldersRemoved = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.foldersScanned = new System.Windows.Forms.TextBox();
            this.logLevelGroup = new System.Windows.Forms.GroupBox();
            this.loglevelMaximum = new System.Windows.Forms.RadioButton();
            this.loglevelMedium = new System.Windows.Forms.RadioButton();
            this.loglevelMinimum = new System.Windows.Forms.RadioButton();
            this.logLocationGroup = new System.Windows.Forms.GroupBox();
            this.logFileAtUserDocs = new System.Windows.Forms.RadioButton();
            this.logFileAtRootFolder = new System.Windows.Forms.RadioButton();
            this.logLevelGroup.SuspendLayout();
            this.logLocationGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectFolderRoot
            // 
            this.selectFolderRoot.Location = new System.Drawing.Point(21, 20);
            this.selectFolderRoot.Margin = new System.Windows.Forms.Padding(4);
            this.selectFolderRoot.Name = "selectFolderRoot";
            this.selectFolderRoot.Size = new System.Drawing.Size(741, 36);
            this.selectFolderRoot.TabIndex = 0;
            this.selectFolderRoot.Text = "Select Folder to Remove";
            this.selectFolderRoot.UseVisualStyleBackColor = true;
            this.selectFolderRoot.Click += new System.EventHandler(this.selectFolderRoot_Click);
            // 
            // rootFolder
            // 
            this.rootFolder.Location = new System.Drawing.Point(21, 92);
            this.rootFolder.Margin = new System.Windows.Forms.Padding(4);
            this.rootFolder.Name = "rootFolder";
            this.rootFolder.ReadOnly = true;
            this.rootFolder.Size = new System.Drawing.Size(738, 27);
            this.rootFolder.TabIndex = 1;
            // 
            // enableDeepRemove
            // 
            this.enableDeepRemove.AutoSize = true;
            this.enableDeepRemove.Checked = true;
            this.enableDeepRemove.CheckState = System.Windows.Forms.CheckState.Indeterminate;
            this.enableDeepRemove.Location = new System.Drawing.Point(18, 287);
            this.enableDeepRemove.Margin = new System.Windows.Forms.Padding(4);
            this.enableDeepRemove.Name = "enableDeepRemove";
            this.enableDeepRemove.Size = new System.Drawing.Size(503, 24);
            this.enableDeepRemove.TabIndex = 2;
            this.enableDeepRemove.Text = "Check this box to enable Deep Remove button, no more warnings";
            this.enableDeepRemove.UseVisualStyleBackColor = true;
            this.enableDeepRemove.CheckedChanged += new System.EventHandler(this.enableDeepRemove_CheckedChanged);
            this.enableDeepRemove.CheckStateChanged += new System.EventHandler(this.enableDeepRemove_CheckStateChanged);
            // 
            // doDeepRemove
            // 
            this.doDeepRemove.Enabled = false;
            this.doDeepRemove.Font = new System.Drawing.Font("Century Schoolbook", 16F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.doDeepRemove.Location = new System.Drawing.Point(18, 329);
            this.doDeepRemove.Margin = new System.Windows.Forms.Padding(4);
            this.doDeepRemove.Name = "doDeepRemove";
            this.doDeepRemove.Size = new System.Drawing.Size(741, 84);
            this.doDeepRemove.TabIndex = 3;
            this.doDeepRemove.Text = "Permanently Deep Remove Selected Folder";
            this.doDeepRemove.UseVisualStyleBackColor = true;
            this.doDeepRemove.Click += new System.EventHandler(this.doDeepRemove_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 439);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(105, 20);
            this.label1.TabIndex = 4;
            this.label1.Text = "Files Deleted";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(22, 475);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(136, 20);
            this.label2.TabIndex = 5;
            this.label2.Text = "Folders Removed";
            // 
            // filesDeleted
            // 
            this.filesDeleted.Location = new System.Drawing.Point(241, 436);
            this.filesDeleted.Name = "filesDeleted";
            this.filesDeleted.ReadOnly = true;
            this.filesDeleted.Size = new System.Drawing.Size(158, 27);
            this.filesDeleted.TabIndex = 6;
            this.filesDeleted.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // foldersRemoved
            // 
            this.foldersRemoved.Location = new System.Drawing.Point(241, 472);
            this.foldersRemoved.Name = "foldersRemoved";
            this.foldersRemoved.ReadOnly = true;
            this.foldersRemoved.Size = new System.Drawing.Size(158, 27);
            this.foldersRemoved.TabIndex = 7;
            this.foldersRemoved.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(417, 475);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(131, 20);
            this.label3.TabIndex = 8;
            this.label3.Text = "Folders Scanned";
            // 
            // foldersScanned
            // 
            this.foldersScanned.Location = new System.Drawing.Point(592, 472);
            this.foldersScanned.Name = "foldersScanned";
            this.foldersScanned.ReadOnly = true;
            this.foldersScanned.Size = new System.Drawing.Size(164, 27);
            this.foldersScanned.TabIndex = 9;
            this.foldersScanned.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // logLevelGroup
            // 
            this.logLevelGroup.Controls.Add(this.loglevelMaximum);
            this.logLevelGroup.Controls.Add(this.loglevelMedium);
            this.logLevelGroup.Controls.Add(this.loglevelMinimum);
            this.logLevelGroup.Location = new System.Drawing.Point(21, 137);
            this.logLevelGroup.Name = "logLevelGroup";
            this.logLevelGroup.Size = new System.Drawing.Size(210, 126);
            this.logLevelGroup.TabIndex = 10;
            this.logLevelGroup.TabStop = false;
            this.logLevelGroup.Text = "Log Level";
            // 
            // loglevelMaximum
            // 
            this.loglevelMaximum.AutoSize = true;
            this.loglevelMaximum.Location = new System.Drawing.Point(7, 87);
            this.loglevelMaximum.Name = "loglevelMaximum";
            this.loglevelMaximum.Size = new System.Drawing.Size(83, 24);
            this.loglevelMaximum.TabIndex = 2;
            this.loglevelMaximum.Text = "verbose";
            this.loglevelMaximum.UseVisualStyleBackColor = true;
            this.loglevelMaximum.CheckedChanged += new System.EventHandler(this.loglevelMaximum_CheckedChanged);
            // 
            // loglevelMedium
            // 
            this.loglevelMedium.AutoSize = true;
            this.loglevelMedium.Location = new System.Drawing.Point(6, 56);
            this.loglevelMedium.Name = "loglevelMedium";
            this.loglevelMedium.Size = new System.Drawing.Size(199, 24);
            this.loglevelMedium.TabIndex = 1;
            this.loglevelMedium.Text = "removed/deleted names";
            this.loglevelMedium.UseVisualStyleBackColor = true;
            this.loglevelMedium.CheckedChanged += new System.EventHandler(this.loglevelMedium_CheckedChanged);
            // 
            // loglevelMinimum
            // 
            this.loglevelMinimum.AutoSize = true;
            this.loglevelMinimum.Checked = true;
            this.loglevelMinimum.Location = new System.Drawing.Point(7, 25);
            this.loglevelMinimum.Name = "loglevelMinimum";
            this.loglevelMinimum.Size = new System.Drawing.Size(99, 24);
            this.loglevelMinimum.TabIndex = 0;
            this.loglevelMinimum.TabStop = true;
            this.loglevelMinimum.Text = "minimum";
            this.loglevelMinimum.UseVisualStyleBackColor = true;
            this.loglevelMinimum.CheckedChanged += new System.EventHandler(this.loglevelMinimum_CheckedChanged);
            // 
            // logLocationGroup
            // 
            this.logLocationGroup.Controls.Add(this.logFileAtRootFolder);
            this.logLocationGroup.Controls.Add(this.logFileAtUserDocs);
            this.logLocationGroup.Location = new System.Drawing.Point(471, 137);
            this.logLocationGroup.Name = "logLocationGroup";
            this.logLocationGroup.Size = new System.Drawing.Size(200, 126);
            this.logLocationGroup.TabIndex = 11;
            this.logLocationGroup.TabStop = false;
            this.logLocationGroup.Text = "Log Files Location";
            // 
            // logFileAtUserDocs
            // 
            this.logFileAtUserDocs.AutoSize = true;
            this.logFileAtUserDocs.Checked = true;
            this.logFileAtUserDocs.Location = new System.Drawing.Point(7, 25);
            this.logFileAtUserDocs.Name = "logFileAtUserDocs";
            this.logFileAtUserDocs.Size = new System.Drawing.Size(148, 24);
            this.logFileAtUserDocs.TabIndex = 0;
            this.logFileAtUserDocs.TabStop = true;
            this.logFileAtUserDocs.Text = "User Documents";
            this.logFileAtUserDocs.UseVisualStyleBackColor = true;
            this.logFileAtUserDocs.CheckedChanged += new System.EventHandler(this.logFileAtUserDocs_CheckedChanged);
            // 
            // logFileAtRootFolder
            // 
            this.logFileAtRootFolder.AutoSize = true;
            this.logFileAtRootFolder.Location = new System.Drawing.Point(7, 87);
            this.logFileAtRootFolder.Name = "logFileAtRootFolder";
            this.logFileAtRootFolder.Size = new System.Drawing.Size(127, 24);
            this.logFileAtRootFolder.TabIndex = 1;
            this.logFileAtRootFolder.TabStop = true;
            this.logFileAtRootFolder.Text = "Root folder \'\\\'";
            this.logFileAtRootFolder.UseVisualStyleBackColor = true;
            this.logFileAtRootFolder.CheckedChanged += new System.EventHandler(this.logFileAtRootFolder_CheckedChanged);
            // 
            // deepRemoveUIfrm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(783, 521);
            this.Controls.Add(this.logLocationGroup);
            this.Controls.Add(this.logLevelGroup);
            this.Controls.Add(this.foldersScanned);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.foldersRemoved);
            this.Controls.Add(this.filesDeleted);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.doDeepRemove);
            this.Controls.Add(this.enableDeepRemove);
            this.Controls.Add(this.rootFolder);
            this.Controls.Add(this.selectFolderRoot);
            this.Font = new System.Drawing.Font("Century Schoolbook", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "deepRemoveUIfrm";
            this.Text = "Deep Remove";
            this.logLevelGroup.ResumeLayout(false);
            this.logLevelGroup.PerformLayout();
            this.logLocationGroup.ResumeLayout(false);
            this.logLocationGroup.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectFolderRoot;
        private System.Windows.Forms.TextBox rootFolder;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.CheckBox enableDeepRemove;
        private System.Windows.Forms.Button doDeepRemove;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox filesDeleted;
        private System.Windows.Forms.TextBox foldersRemoved;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox foldersScanned;
        private System.Windows.Forms.GroupBox logLevelGroup;
        private System.Windows.Forms.RadioButton loglevelMaximum;
        private System.Windows.Forms.RadioButton loglevelMedium;
        private System.Windows.Forms.RadioButton loglevelMinimum;
        private System.Windows.Forms.GroupBox logLocationGroup;
        private System.Windows.Forms.RadioButton logFileAtRootFolder;
        private System.Windows.Forms.RadioButton logFileAtUserDocs;
    }
}

