namespace CoffeeScholar.ReplaceAllGit
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
            lblLatestVersion = new LinkLabel();
            linkLabel1 = new LinkLabel();
            btnUpdateByBash = new Button();
            groupBox1 = new GroupBox();
            label1 = new Label();
            linkWhere = new LinkLabel();
            lblNumber = new Label();
            btnSetAsDefault = new Button();
            lblVersion = new Label();
            linkPath = new LinkLabel();
            chkCombineSameFolder = new CheckBox();
            chkIgnoreSmaller = new CheckBox();
            btnSearchAll = new Button();
            chkSelectAll = new CheckBox();
            lblDescription = new Label();
            lsvResult = new MyListView();
            columnHeader1 = new ColumnHeader();
            columnHeader5 = new ColumnHeader();
            columnHeader6 = new ColumnHeader();
            GitBash = new ColumnHeader();
            columnHeader3 = new ColumnHeader();
            columnHeader4 = new ColumnHeader();
            columnHeader7 = new ColumnHeader();
            FullPath = new ColumnHeader();
            groupBox2.SuspendLayout();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // btnUpdate
            // 
            btnUpdate.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnUpdate.Enabled = false;
            btnUpdate.Location = new Point(1166, 836);
            btnUpdate.Name = "btnUpdate";
            btnUpdate.Size = new Size(181, 34);
            btnUpdate.TabIndex = 9;
            btnUpdate.Text = "&U 更新复选框选中";
            btnUpdate.UseVisualStyleBackColor = true;
            btnUpdate.Click += btnUpdate_Click;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(lblLatestVersion);
            groupBox2.Controls.Add(linkLabel1);
            groupBox2.Controls.Add(btnUpdateByBash);
            groupBox2.Controls.Add(groupBox1);
            groupBox2.Controls.Add(chkCombineSameFolder);
            groupBox2.Controls.Add(chkIgnoreSmaller);
            groupBox2.Controls.Add(btnSearchAll);
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
            // lblLatestVersion
            // 
            lblLatestVersion.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblLatestVersion.AutoSize = true;
            lblLatestVersion.Location = new Point(1290, 26);
            lblLatestVersion.Name = "lblLatestVersion";
            lblLatestVersion.Size = new Size(30, 24);
            lblLatestVersion.TabIndex = 12;
            lblLatestVersion.TabStop = true;
            lblLatestVersion.Text = "    ";
            lblLatestVersion.Visible = false;
            lblLatestVersion.LinkClicked += lblLatestVersion_LinkClicked;
            // 
            // linkLabel1
            // 
            linkLabel1.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            linkLabel1.AutoSize = true;
            linkLabel1.Location = new Point(1111, 26);
            linkLabel1.Name = "linkLabel1";
            linkLabel1.Size = new Size(173, 24);
            linkLabel1.TabIndex = 11;
            linkLabel1.TabStop = true;
            linkLabel1.Text = "🔗Git for Windows";
            linkLabel1.LinkClicked += linkLabel1_LinkClicked;
            // 
            // btnUpdateByBash
            // 
            btnUpdateByBash.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnUpdateByBash.Enabled = false;
            btnUpdateByBash.Location = new Point(644, 836);
            btnUpdateByBash.Name = "btnUpdateByBash";
            btnUpdateByBash.Size = new Size(195, 34);
            btnUpdateByBash.TabIndex = 10;
            btnUpdateByBash.Text = "&B 基于 GitBash 升级";
            btnUpdateByBash.UseVisualStyleBackColor = true;
            btnUpdateByBash.Visible = false;
            btnUpdateByBash.Click += btnUpdateByBash_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(linkWhere);
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
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 36);
            label1.Name = "label1";
            label1.Size = new Size(107, 24);
            label1.TabIndex = 5;
            label1.Text = "环境 Where";
            // 
            // linkWhere
            // 
            linkWhere.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linkWhere.Location = new Point(136, 36);
            linkWhere.Name = "linkWhere";
            linkWhere.Size = new Size(1025, 34);
            linkWhere.TabIndex = 4;
            linkWhere.LinkClicked += LinkWhereLinkClicked;
            // 
            // lblNumber
            // 
            lblNumber.AutoSize = true;
            lblNumber.Location = new Point(13, 74);
            lblNumber.Name = "lblNumber";
            lblNumber.Size = new Size(21, 24);
            lblNumber.TabIndex = 0;
            lblNumber.Text = "0";
            // 
            // btnSetAsDefault
            // 
            btnSetAsDefault.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnSetAsDefault.Enabled = false;
            btnSetAsDefault.Location = new Point(1185, 70);
            btnSetAsDefault.Name = "btnSetAsDefault";
            btnSetAsDefault.Size = new Size(112, 33);
            btnSetAsDefault.TabIndex = 3;
            btnSetAsDefault.Text = "&D 设为默认";
            btnSetAsDefault.UseVisualStyleBackColor = true;
            btnSetAsDefault.Click += btnSetAsDefault_Click;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(58, 74);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(62, 24);
            lblVersion.TabIndex = 1;
            lblVersion.Text = "2.45.1";
            // 
            // linkPath
            // 
            linkPath.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            linkPath.Location = new Point(136, 74);
            linkPath.Name = "linkPath";
            linkPath.Size = new Size(1025, 34);
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
            chkCombineSameFolder.Location = new Point(302, 840);
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
            chkIgnoreSmaller.Location = new Point(116, 840);
            chkIgnoreSmaller.Name = "chkIgnoreSmaller";
            chkIgnoreSmaller.Size = new Size(162, 28);
            chkIgnoreSmaller.TabIndex = 6;
            chkIgnoreSmaller.Text = "忽略较小的文件";
            chkIgnoreSmaller.UseVisualStyleBackColor = true;
            // 
            // btnSearchAll
            // 
            btnSearchAll.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnSearchAll.Enabled = false;
            btnSearchAll.Location = new Point(1030, 836);
            btnSearchAll.Name = "btnSearchAll";
            btnSearchAll.Size = new Size(112, 34);
            btnSearchAll.TabIndex = 8;
            btnSearchAll.Text = "&S 搜索全盘";
            btnSearchAll.UseVisualStyleBackColor = true;
            btnSearchAll.Click += btnSearchAll_Click;
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
            lblDescription.Size = new Size(581, 24);
            lblDescription.TabIndex = 2;
            lblDescription.Text = "当前系统中的 git.exe 分别来自：环境变量（系统、用户）、程序自带。";
            // 
            // lsvResult
            // 
            lsvResult.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            lsvResult.CheckBoxes = true;
            lsvResult.Columns.AddRange(new ColumnHeader[] { columnHeader1, columnHeader5, columnHeader6, GitBash, columnHeader3, columnHeader4, columnHeader7, FullPath });
            lsvResult.FullRowSelect = true;
            lsvResult.Location = new Point(22, 200);
            lsvResult.MultiSelect = false;
            lsvResult.Name = "lsvResult";
            lsvResult.OwnerDraw = true;
            lsvResult.Size = new Size(1325, 621);
            lsvResult.TabIndex = 4;
            lsvResult.UseCompatibleStateImageBehavior = false;
            lsvResult.View = View.Details;
            lsvResult.ColumnClick += lsvResult_ColumnClick;
            lsvResult.ItemCheck += lsvResult_ItemCheck;
            lsvResult.ItemChecked += lsvResult_ItemChecked;
            lsvResult.SelectedIndexChanged += lsvResult_SelectedIndexChanged;
            lsvResult.SizeChanged += lsvResult_SizeChanged;
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
            // GitBash
            // 
            GitBash.Text = "Bash";
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
            columnHeader4.Width = 120;
            // 
            // columnHeader7
            // 
            columnHeader7.Text = "最近访问";
            columnHeader7.TextAlign = HorizontalAlignment.Center;
            columnHeader7.Width = 200;
            // 
            // FullPath
            // 
            FullPath.Text = "完整路径";
            FullPath.Width = 491;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1395, 914);
            Controls.Add(groupBox2);
            MinimumSize = new Size(1000, 800);
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
        private MyListView lsvResult;
        private ColumnHeader columnHeader1;
        private ColumnHeader columnHeader3;
        private ColumnHeader columnHeader6;
        private ColumnHeader columnHeader5;
        private ColumnHeader FullPath;
        private CheckBox chkSelectAll;
        private Button btnSearchAll;
        private ColumnHeader columnHeader4;
        private ColumnHeader columnHeader7;
        private CheckBox chkIgnoreSmaller;
        private CheckBox chkCombineSameFolder;
        private GroupBox groupBox1;
        private Button btnSetAsDefault;
        private Label lblVersion;
        private LinkLabel linkPath;
        private Button btnUpdateByBash;
        private Label lblNumber;
        private LinkLabel linkLabel1;
        private LinkLabel lblLatestVersion;
        private ColumnHeader GitBash;
        private Label label1;
        private LinkLabel linkWhere;
    }
}
