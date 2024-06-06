namespace CoffeeScholar.ReplaceAllGit
{
    partial class AboutForm
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
            btnCancel = new Button();
            richText = new RichTextBox();
            cboxLanguage = new ComboBox();
            label1 = new Label();
            linkGithub = new LinkLabel();
            lblVersion = new Label();
            SuspendLayout();
            // 
            // btnCancel
            // 
            btnCancel.Location = new Point(808, 528);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(112, 34);
            btnCancel.TabIndex = 3;
            btnCancel.Text = "关闭";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            // 
            // richText
            // 
            richText.Location = new Point(13, 50);
            richText.Name = "richText";
            richText.ReadOnly = true;
            richText.Size = new Size(920, 460);
            richText.TabIndex = 1;
            richText.Text = "About ReplaceAllGit";
            // 
            // cboxLanguage
            // 
            cboxLanguage.DropDownStyle = ComboBoxStyle.DropDownList;
            cboxLanguage.Enabled = false;
            cboxLanguage.FormattingEnabled = true;
            cboxLanguage.Items.AddRange(new object[] { "简体中文", "English" });
            cboxLanguage.Location = new Point(751, 12);
            cboxLanguage.Name = "cboxLanguage";
            cboxLanguage.Size = new Size(182, 32);
            cboxLanguage.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(650, 15);
            label1.Name = "label1";
            label1.Size = new Size(95, 24);
            label1.TabIndex = 5;
            label1.Text = "language:";
            // 
            // linkGithub
            // 
            linkGithub.AutoSize = true;
            linkGithub.Location = new Point(13, 533);
            linkGithub.Name = "linkGithub";
            linkGithub.Size = new Size(419, 24);
            linkGithub.TabIndex = 2;
            linkGithub.TabStop = true;
            linkGithub.Text = "https://github.com/coffeescholar/ReplaceAllGit";
            linkGithub.LinkClicked += linkGithub_LinkClicked;
            // 
            // lblVersion
            // 
            lblVersion.AutoSize = true;
            lblVersion.Location = new Point(13, 15);
            lblVersion.Name = "lblVersion";
            lblVersion.Size = new Size(83, 24);
            lblVersion.TabIndex = 6;
            lblVersion.Text = "Version: ";
            // 
            // AboutForm
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            CancelButton = btnCancel;
            ClientSize = new Size(945, 588);
            ControlBox = false;
            Controls.Add(lblVersion);
            Controls.Add(linkGithub);
            Controls.Add(label1);
            Controls.Add(cboxLanguage);
            Controls.Add(richText);
            Controls.Add(btnCancel);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "AboutForm";
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "About";
            TopMost = true;
            Load += AboutForm_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btnCancel;
        private RichTextBox richText;
        private ComboBox cboxLanguage;
        private Label label1;
        private LinkLabel linkGithub;
        private Label lblVersion;
    }
}