using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace CoffeeScholar.ReplaceAllGit
{
    public partial class AboutForm : Form
    {
        public AboutForm()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void AboutForm_Load(object sender, EventArgs e)
        {
            lblVersion.Text = $@"Ver: {Application.ProductVersion}";
            richText.Text = $@"Replace All Git - 查找 Git 并升级到最新版本";
            cboxLanguage.SelectedIndex = 0;

            var filePath = "Readme.rtf";
            // 判断文件是否存在
            if (File.Exists(filePath))
            {
                var fileContent = File.ReadAllText(filePath, Encoding.UTF8);
                richText.Text = fileContent;
            }
            else
                richText.Text = @"Readme.rtf 文件不存在";

            filePath = "License.txt";
            if (File.Exists(filePath))
            {
                var fileContent = File.ReadAllText(filePath, Encoding.UTF8);
                richText.Text += "\r\n\r\n" + fileContent;
            }
            else
                richText.Text += "\r\nLicense.txt 文件不存在";
        }

        private void linkGithub_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkGithub.LinkVisited = false;

            var psi = new ProcessStartInfo
            {
                FileName = linkGithub.Text.Trim(),
                UseShellExecute = true
            };
            Process.Start(psi);
        }
    }
}
