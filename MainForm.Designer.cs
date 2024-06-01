namespace ReplaceAllGit
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            btnUpdate = new Button();
            groupBox2 = new GroupBox();
            button1 = new Button();
            groupBox1 = new GroupBox();
            lblNumber = new Label();
            btnSetAsDefault = new Button();
            lblVersion = new Label();
            linkPath = new LinkLabel();
            chkCombineSameFolder = new CheckBox();
            chkIgnoreSmaller = new CheckBox();
            btnRefresh = new Button();
            chkSelectAll = new CheckBox();
            lblDescription = new Label();
            lsvResult = new ListView();
            columnHeader1 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            columnHeader2 = new ColumnHeader();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnUpdate
            // 
            btnUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnUpdate.Location = new Point(1072, 836);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(112, 34);
            btnUpdate.TabIndex = 9;
            btnUpdate.Text = "&U 更新选中";
            btnUpdate.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(button1);
            groupBox2.Controls.Add(groupBox1);
            groupBox2.Controls.Add(chkCombineSameFolder);
            groupBox2.Controls.Add(chkIgnoreSmaller);
            groupBox2.Controls.Add(btnRefresh);
            groupBox2.Controls.Add(chkSelectAll);
            groupBox2.Controls.Add(lblDescription);
            groupBox2.Controls.Add(btnUpdate);
            groupBox2.Controls.Add(lsvResult);
            groupBox2.Location = new Point(12, 12);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(1371, 883);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button1.Location = new Point(1214, 836);
            button1.Name = "button1";
            button1.Size = new Size(112, 34);
            button1.TabIndex = 10;
            button1.Text = "&R 恢复选中";
            button1.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(lblNumber);
            groupBox1.Controls.Add(btnSetAsDefault);
            groupBox1.Controls.Add(lblVersion);
            groupBox1.Controls.Add(linkPath);
            groupBox1.Location = new Point(23, 66);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(1324, 118);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "选中项详情";
            // 
            // lblNumber
            // 
            lblNumber.AutoSize = true;
            lblNumber.Location = new Point(13, 30);
            lblNumber.Name = "lblNumber";
            lblNumber.Size = new Size(21, 24);
            lblNumber.TabIndex = 0;
            lblNumber.Text = "0";
            // 
            // btnSetAsDefault
            // 
            btnSetAsDefault.Location = new Point(1203, 21);
            btnSetAsDefault.Name = "btnSetAsDefault";
            btnSetAsDefault.Size = new Size(100, 33);
            btnSetAsDefault.TabIndex = 3;
            btnSetAsDefault.Text = "设为默认";
            btnSetAsDefault.UseVisualStyleBackColor = true;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(73, 30);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(62, 24);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "2.45.1";
            // 
            // linkPath
            // 
            linkPath.AutoSize = true;
            linkPath.Location = new Point(158, 30);
            linkPath.Name = "linkPath";
            linkPath.Size = new Size(486, 24);
            linkPath.TabIndex = 2;
            linkPath.TabStop = true;
            linkPath.Text = "D:\\_Dev_\\_Scoop_\\apps\\git\\2.45.1\\mingw64\\bin\\git.exe";
            linkPath.LinkClicked += linkPath_LinkClicked;
            // 
            // chkCombineSameFolder
            // 
            chkCombineSameFolder.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkCombineSameFolder.AutoSize = true;
            chkCombineSameFolder.Checked = true;
            chkCombineSameFolder.CheckState = CheckState.Checked;
            chkCombineSameFolder.Location = new Point(741, 840);
            chkCombineSameFolder.Name = "chkCombineSameFolder";
            chkCombineSameFolder.Size = new Size(144, 28);
            chkCombineSameFolder.TabIndex = 7;
            chkCombineSameFolder.Text = "合并相同目录";
            chkCombineSameFolder.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreSmaller
            // 
            chkIgnoreSmaller.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkIgnoreSmaller.AutoSize = true;
            chkIgnoreSmaller.Checked = true;
            chkIgnoreSmaller.CheckState = CheckState.Checked;
            chkIgnoreSmaller.Location = new Point(555, 840);
            chkIgnoreSmaller.Name = "chkIgnoreSmaller";
            chkIgnoreSmaller.Size = new Size(162, 28);
            chkIgnoreSmaller.TabIndex = 6;
            chkIgnoreSmaller.Text = "忽略较小的文件";
            chkIgnoreSmaller.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            btnRefresh.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnRefresh.Location = new Point(932, 836);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(112, 34);
            btnRefresh.TabIndex = 8;
            btnRefresh.Text = "&R 刷新";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            // 
            // chkSelectAll
            // 
            chkSelectAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            chkSelectAll.AutoSize = true;
            chkSelectAll.Location = new Point(26, 840);
            chkSelectAll.Name = "chkSelectAll";
            chkSelectAll.Size = new Size(72, 28);
            chkSelectAll.TabIndex = 5;
            chkSelectAll.Text = "全选";
            chkSelectAll.UseVisualStyleBackColor = true;
            chkSelectAll.CheckedChanged += chkSelectAll_CheckedChanged;
            // 
            // lblDescription
            // 
            lblDescription.AutoSize = true;
            lblDescription.Location = new Point(23, 26);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(951, 24);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "当前系统中发现的 Git.exe 分别来自：环境变量（系统、用户）、各硬盘不同位置（往往是其它程序自带的 Git.exe）。";
            // 
            // lsvResult
            // 
            lsvResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lsvResult.CheckBoxes = true;
            lsvResult.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader5, columnHeader6, columnHeader3, columnHeader4, columnHeader7, columnHeader2 });
            lsvResult.FullRowSelect = true;
            lsvResult.Location = new Point(22, 200);
            lsvResult.MultiSelect = false;
            lsvResult.Name = "lsvResult";
            lsvResult.Size = new Size(1325, 621);
            lsvResult.TabIndex = 4;
            lsvResult.UseCompatibleStateImageBehavior = false;
            lsvResult.View = View.Details;
            lsvResult.ColumnClick += lsvResult_ColumnClick;
            lsvResult.SelectedIndexChanged += lsvResult_SelectedIndexChanged;
            // 
            // columnHeader1
            // 
            columnHeader1.Text = "序号";
            columnHeader1.Width = 80;
            // 
            // columnHeader5
            // 
            columnHeader5.Text = "版本";
            columnHeader5.TextAlign = HorizontalAlignment.Center;
            columnHeader5.Width = 120;
            // 
            // columnHeader6
            // 
            columnHeader6.Text = "更新";
            columnHeader6.TextAlign = HorizontalAlignment.Center;
            columnHeader6.Width = 100;
            // 
            // columnHeader3
            // 
            columnHeader3.Text = "环境变量";
            columnHeader3.TextAlign = HorizontalAlignment.Center;
            columnHeader3.Width = 150;
            // 
            // columnHeader4
            // 
            columnHeader4.Text = "文件大小";
            columnHeader4.TextAlign = HorizontalAlignment.Right;
            columnHeader4.Width = 150;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "更新时间";
            columnHeader7.TextAlign = HorizontalAlignment.Center;
            columnHeader7.Width = 200;
            // 
            // columnHeader2
            // 
            columnHeader2.Text = "完整路径";
            columnHeader2.Width = 700;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1395, 914);
            Controls.Add(groupBox2);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "查找 Git 并升级到最新版本";
            Load += MainForm_Load;
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Button btnUpdate;
        private GroupBox groupBox2;
        private Label lblDescription;
        private ListView lsvResult;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader5;
        private ColumnHeader columnHeader2;
        private CheckBox chkSelectAll;
        private Button btnRefresh;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader7;
        private CheckBox chkIgnoreSmaller;
        private CheckBox chkCombineSameFolder;
        private GroupBox groupBox1;
        private Button btnSetAsDefault;
        private Label lblVersion;
        private LinkLabel linkPath;
        private Button button1;
        private Label lblNumber;
    }
}
