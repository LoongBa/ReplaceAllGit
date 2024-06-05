using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using CoffeeScholar.ReplaceAllGit.DarkMode;
using Microsoft.Win32;

namespace CoffeeScholar.ReplaceAllGit;

public partial class MainForm : Form
{
    private readonly DarkModeCS? _DarkMode;
    private int _SortColumn = -1;

    private bool _IsDefaultSet;
    private bool _IsItemSelected;
    private bool _IsItemChecked;

    private const string SearchPattern = "^git.exe$";
    private readonly SearchHelper _SearchHelper;
    private Version _LastVersion = new();
    private Dictionary<string, GitSearchResult> _VersionList = new();
    private GitSearchResult? _DefaultSearchResult;
    private GitSearchResult? _SelectedSearchResult;
    private string[] _WhereIsGit_Paths_ = [];

    public MainForm()
    {
        InitializeComponent();
        _DarkMode = new DarkModeCS(this);
        // ����ϵͳ��ѡ������¼�
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;

        var winDir = Environment.GetEnvironmentVariable("windir");
        if (winDir != null)
        {
            var filePath = winDir + @"\explorer.exe";
            var icon = Icon.ExtractAssociatedIcon(filePath);
            var bitmap = icon?.ToBitmap();
            var scaledImage = bitmap?.GetThumbnailImage(16, 16, null, IntPtr.Zero);
            linkPath.ImageAlign = ContentAlignment.TopLeft;
            linkPath.Image = scaledImage;
            linkWhere.ImageAlign = ContentAlignment.TopLeft;
            linkWhere.Image = scaledImage;
        }

        // ��ʼ��
        _SearchHelper = new SearchHelper(["MinGW64", "Git"], "bin\\git.exe", @"\d+\.\d+\.\d+", "--version", 3 * 1024 * 1024);
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        switch (e.Category)
        {
            case UserPreferenceCategory.General:
                // �û�������һ����ѡ������
                break;
            case UserPreferenceCategory.Desktop:
                // �û�������������ص���ѡ������
                break;
                // �����������...
        }

        // ������Ҫˢ��Ӧ�ó��������
        //_DarkMode?.TryDarkModeTheme();

    }

    #region ��������

    private void RefreshListView(IEnumerable<GitSearchResult> results)
    {
        var index = 0;
        // �г����
        var gitPaths = _WhereIsGit_Paths_;
        foreach (var result in results.OrderByDescending(r => r.Version))
        {
            index++;
            /*
            var type = result.IsInSysPath ? "ϵͳ" : string.Empty;
            if (type.Length > 0) type += " | ";
            type += result.IsInUserPath ? "�û�" : string.Empty;
            */
            result.IsInWherePath = _WhereIsGit_Paths_.Contains(result.FullPath);
            var type = result.IsInWherePath ? "��" : "";
            ListViewItem item = new(result.Index = index.ToString("D2"));
            item.SubItems.Add(result.Version);
            item.SubItems.Add(result.IsNeedUpdating ? "������" : "");
            item.SubItems.Add(result.HasGitBash ? "��" : "");
            item.SubItems.Add(type);
            item.SubItems.Add(SearchHelper.FormatFileSizeForDisplay(result.Size));
            item.SubItems.Add(result.LastAccessTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            item.SubItems.Add(result.FullPath);
            item.Tag = result;
            lsvResult.Items.Add(item);
        }
    }

    private static void OpenWithExplorer(string fullPath)
    {
        if (!File.Exists(fullPath)) return;
        var folder = Path.GetDirectoryName(fullPath);
        if (folder != null)
            Process.Start("explorer.exe", folder);
    }

    private void ClearControls()
    {
        lsvResult.Items.Clear();
        lblNumber.Text = string.Empty;
        lblVersion.Text = string.Empty;
        linkPath.Text = string.Empty;
    }

    private void CheckButtons()
    {
        btnSetAsDefault.Enabled = _IsItemSelected;
        btnUpdate.Enabled = _IsDefaultSet && _IsItemChecked;
        btnUpdateByBash.Enabled = _IsItemSelected && (_SelectedSearchResult?.HasGitBash ?? false);

        if (_SelectedSearchResult != null && !_IsDefaultSet)
        {
            var item = _SelectedSearchResult;
            lblNumber.Text = item.Index;
            linkPath.Text = @"    " + item.FullPath;
            lblVersion.Text = item.Version;
        }
    }

    #endregion

    #region ҵ�񷽷�

    private async Task<Version> CheckLatestVersionsAsync()
    {
        var replacementPattern = "${version}";

        // ͨ�� Git-Scm ��ȡ���°汾�� 
        var url = "https://git-scm.com/";
        var searchPattern = @"<span class=""version"">\s*?(?<version>\d+\.\d+\.\d+)\s*?</span>";
        var task1 = SearchHelper.CheckLatestVersionAsync(url, searchPattern, replacementPattern, RegexOptions.Singleline);

        // ͨ�� gitforWindows.org ��ȡ���°汾��
        url = "https://gitforwindows.org/";
        searchPattern = @"<div class=""version""><a .*?>Version\s?(?<version>\d+\.\d+\.\d+)\s?</a></div>";
        var task2 = SearchHelper.CheckLatestVersionAsync(url, searchPattern, replacementPattern, RegexOptions.Singleline);

        // �ȴ������������
        var versions = await Task.WhenAll(task1, task2);
        // �ж����µİ汾��
        var latestVersion = versions.Max();

        // �������°汾��
        return new Version(latestVersion ?? "");
    }
    private async Task<(string githubUrl, string huaweiUrl, string huaweiHomeUrl)> GetDownloadUrlAsync()
    {
        var replacementPattern = "${url}";

        /*
        // ͨ�� gitforwindows.org ��ȡ���°汾��
        var url = "https://gitforwindows.org";
        //<a class="button featurebutton" href="https://github.com/git-for-windows/git/releases/download/v2.45.2.windows.1/Git-2.45.2-64-bit.exe" target="_blank">Download</a>
        var searchPattern = @"<a class=""button featurebutton"" href=""(?<url>.*?)"" target=""_blank"">Download</a>";
        */

        // ͨ�� git-scm.com ��ȡ���°汾��
        var url = "https://git-scm.com/download/win";
        //<a href="https://github.com/git-for-windows/git/releases/download/v2.45.2.windows.1/Git-2.45.2-64-bit.exe">64-bit Git for Windows Setup</a>
        var searchPattern = @"<a href=\""(?<url>[^\""]*)\""[^<]*>64-bit Git for Windows Setup</a>";
        var githubUrl = await SearchHelper.CheckLatestVersionAsync(url, searchPattern, replacementPattern, RegexOptions.Singleline);

        // �滻Ϊ��Ϊ�����ַ
        // https://github.com/git-for-windows/git/releases/download/v2.45.2.windows.1/Git-2.45.2-64-bit.exe
        // https://mirrors.huaweicloud.com/git-for-windows/v2.45.2.windows.1/Git-2.45.2-64-bit.exe
        var huaweiUrl = githubUrl.Replace("https://github.com/git-for-windows/git/releases/download/", "https://mirrors.huaweicloud.com/git-for-windows/");

        // ������ʽƥ��URLֱ�����һ��б��
        var match = Regex.Match(huaweiUrl, @"(.*/).*");
        // ��ȡƥ��ĵ�һ�飬Ҳ����URLֱ�����һ��б�ܵĲ���
        var huaweiHomeUrl = match.Groups[1].Value;

        // �������°汾��
        return (githubUrl, huaweiUrl, huaweiHomeUrl);
    }

    /// <summary>
    /// ���� git-bash.exe
    /// </summary>
    private void SearchGitBash(Dictionary<string, GitSearchResult> versionList)
    {
        foreach (var pair in versionList)
        {
            // ���ϼ�Ŀ¼���� git-bash.exe
            var targetFolder = Path.GetDirectoryName(pair.Value.MainFolder);
            if (targetFolder != null)
            {
                var result = _SearchHelper.SearchFilesWith<GitSearchResult>("git-bash.exe", targetFolder);
                if (result.Count > 0)
                {
                    var gitBash = result.First();
                    pair.Value.GitBashFullPath = gitBash;
                }
            }
        }
    }

    /// <summary>
    /// ���� git-bash.exe ���� Git
    /// </summary>
    /// <param name="selectedSearchResult"></param>
    private bool RunGitBash(GitSearchResult? selectedSearchResult)
    {
        if (selectedSearchResult == null) return false;

        var gitBashPath = selectedSearchResult.GitBashFullPath;
        if (string.IsNullOrEmpty(gitBashPath) || !File.Exists(gitBashPath))
            return false;

        // ���� git-bash.exe
        // var arguments = "-c \"git update-git-for-windows\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = gitBashPath,
            // Arguments = arguments,
            UseShellExecute = false,
            // RedirectStandardOutput = true,
            // RedirectStandardError = true,  // �ض��������
            CreateNoWindow = false
        };

        try
        {
            using var process = Process.Start(startInfo);
            /*
            if (process != null)
            {
                // // ��ȡ���
                // var versionOutput = process.StandardOutput.ReadToEnd();
                // // ��ȡ�������
                // var error = process.StandardError.ReadToEnd();

                //process.WaitForExitAsync();

                /*
                Debug.WriteLine("Git Bash version:");
                Debug.WriteLine(versionOutput);
                Debug.WriteLine("Git Bash error:");
                Debug.WriteLine(error);
            #1#
            }
        */
        }
        catch (Exception ex)
        {
            Debug.WriteLine("An error occurred while trying to get the Git Bash version: " + ex.Message);
        }

        return true;
    }
    #endregion

    private async void MainForm_Load(object sender, EventArgs e)
    {
        Text = $@"Replace All Git - ���� Git �����������°汾 - {Application.ProductVersion}";
        ClearControls();

        #region ������°汾
        var version = await CheckLatestVersionsAsync();
        _LastVersion = version;
        lblLatestVersion.Text = _LastVersion.ToString();
        lblLatestVersion.Visible = true;
        #endregion

        // ��ȡ where �������Ƿ����ָ���ĳ���
        var exeName = "git.exe";
        await Task.Run(async () =>
        {
            // ��ȡ git.exe ��·��
            var gitPaths = SearchHelper.GetPathsByCmdWhere(exeName);
            _WhereIsGit_Paths_ = gitPaths;

            // �� UI �߳��и��� linkWhere.Text
            Invoke(() => linkWhere.Text = @"    " + string.Join(" | ", gitPaths));

            // SearchAll
            BeginInvoke(() => btnSearchAll_Click(this, EventArgs.Empty));
            //btnSearchAll.Enabled = true;
        });

    }

    private void btnSearchAll_Click(object sender, EventArgs e)
    {
        ClearControls();
        btnSearchAll.Enabled = false;

        // �������� git.exe
        _VersionList = _SearchHelper.SearchFiles<GitSearchResult>(_LastVersion, chkIgnoreSmaller.Checked, SearchPattern, true);
        // ���� git-bash.exe
        SearchGitBash(_VersionList);

        // ˢ�� ListView
        if (chkCombineSameFolder.Checked)
            RefreshListView(_SearchHelper.GroupByMainFolder(_VersionList));
        else
            RefreshListView(_VersionList.Values);

        btnSearchAll.Enabled = true;
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        // ����ѡ����
        var selectedItems = lsvResult.Items.Cast<ListViewItem>().Where(i => i.Checked).ToList();
        if (selectedItems.Count == 0)
        {
            MessageBox.Show(@"����ѡ��Ҫ������ Git �汾��", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var needRefresh = false;
        foreach (var item in selectedItems)
        {
            if (item.Tag is not SearchResult searchResult
                || _DefaultSearchResult is not { MainFolder: not null }
                || searchResult.MainFolder == null) continue;

            var fromMainFolder = _DefaultSearchResult.MainFolder;
            var toMainFolder = searchResult.MainFolder;
            // MessageBox ȷ��
            if (MessageBox.Show($"""
                                 �Ƿ�� 
                                    {fromMainFolder}
                                 �����ļ�����
                                 	{toMainFolder}
                                 """,
                    @"ȷ�ϲ���", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                needRefresh = true;
                var logs = _SearchHelper.CopyFilesFrom(fromMainFolder, toMainFolder);
                {
                    var logText = new StringBuilder();
                    foreach (var log in logs)
                        logText.AppendLine($"\t\t{log}");
                    MessageBox.Show(
                        $"""
                         �� 
                            {fromMainFolder} 
                         �����ļ�����
                         	{toMainFolder}��
                         �����ɹ���
                            {logText}
                         """,
                        @"ȷ�ϲ���", MessageBoxButtons.OK);
                }
                Debug.WriteLine(logs);
            }
        }
        if (needRefresh)
            btnSearchAll_Click(this, e);  // ˢ�� ListView
    }

    private void btnSetAsDefault_Click(object sender, EventArgs e)
    {
        if (_IsDefaultSet)
        {
            // ȡ��Ĭ��
            _DefaultSearchResult = null;

            _IsDefaultSet = false;
            btnSetAsDefault.Text = @"&D ��ΪĬ��";
            lsvResult_SelectedIndexChanged(sender, e);
        }
        else
        {
            // ����ΪĬ��
            if (lsvResult.SelectedItems.Count != 0)
            {
                var item = lsvResult.SelectedItems[0];
                item.Checked = false;
                _DefaultSearchResult = item.Tag as GitSearchResult;

                _IsDefaultSet = true;
                btnSetAsDefault.Text = @"&C ȡ��Ĭ��";
            }
        }
        CheckButtons();
    }

    private void btnUpdateByBash_Click(object sender, EventArgs e)
    {
        RunGitBash(_SelectedSearchResult);
        /*
        var result = RunGitBash(_SelectedSearchResult);
        if (!result)
            MessageBox.Show(@"Git-Bash ����ʧ�ܡ�", @"��ʾ", MessageBoxButtons.OK, MessageBoxIcon.Information);
    */
    }

    private void lsvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        _IsItemChecked = lsvResult.Items.Cast<ListViewItem>().Any(i => i.Checked);
        _IsItemSelected = lsvResult.SelectedItems.Count > 0;
        if (_IsItemSelected)
            _SelectedSearchResult = lsvResult.SelectedItems[0].Tag as GitSearchResult;
        else
            _SelectedSearchResult = null;

        CheckButtons();
    }

    private void lsvResult_ItemChecked(object sender, ItemCheckedEventArgs e)
    {
        CheckButtons();
    }

    private void lsvResult_ColumnClick(object sender, ColumnClickEventArgs e)
    {
        if (e.Column != _SortColumn)
        {
            // ���������в��ǵ�ǰ������У���ôʹ����������
            _SortColumn = e.Column;
            lsvResult.Sorting = SortOrder.Ascending;
        }
        else
        {
            // �����������ǵ�ǰ������У���ô��ת����˳��
            lsvResult.Sorting = lsvResult.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }

        // ʹ�� ListViewItemSorter �����������
        lsvResult.ListViewItemSorter = new ListViewItemComparer(e.Column, lsvResult.Sorting);
    }

    private void lsvResult_ItemCheck(object sender, ItemCheckEventArgs e)
    {
        var item = lsvResult.Items[e.Index];
        if (_DefaultSearchResult != null && item is { Tag: SearchResult })
        {
            var searchResult = (SearchResult)item.Tag;
            var mainFolder = searchResult.MainFolder;
            if (mainFolder.Equals(_DefaultSearchResult.MainFolder, StringComparison.OrdinalIgnoreCase))
            {
                // ���ı� Check 
                e.NewValue = e.CurrentValue;
            }

        }

    }

    private void lsvResult_SizeChanged(object sender, EventArgs e)
    {
        // �Զ������п�
        lsvResult.Columns[7].Width = -2;
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        // ȫѡ����ȫ��ѡ
        foreach (ListViewItem item in lsvResult.Items)
            item.Checked = chkSelectAll.Checked;
    }

    private async void lblLatestVersion_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        var (githubUrl, huaweiUrl, huaweiHomeUrl) = await GetDownloadUrlAsync();
        var url = githubUrl;
        var homeUrl = "https://gitforwindows.org/";
        var dialogResult = MessageBox.Show(@"�Ƿ��û�Ϊ������� Github.com ���ص�ַ��",
                                        @"����������ַ",
                                            MessageBoxButtons.YesNoCancel);

        switch (dialogResult)
        {
            case DialogResult.Yes:
                url = huaweiUrl;
                homeUrl = huaweiHomeUrl;
                break;
            case DialogResult.No:
                break;
            case DialogResult.Cancel:
            default:
                return;
        }
        // ����������������ص�ַ
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);

        // ����������������ص�ַ
        psi = new ProcessStartInfo
        {
            FileName = homeUrl,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        //\u1F310 ���� \u1F517 ����
        linkLabel1.LinkVisited = false;

        var psi = new ProcessStartInfo
        {
            FileName = "https://gitforwindows.org/",
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void linkPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // ���ļ���
        var path = linkPath.Text.Trim();
        linkPath.LinkVisited = false;
        OpenWithExplorer(path);
    }

    private void LinkWhereLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // ���ļ���
        foreach (var path in _WhereIsGit_Paths_)
            OpenWithExplorer(path);
        linkPath.LinkVisited = false;
    }
}

internal class ListViewItemComparer(int column, SortOrder order) : IComparer
{
    public int Compare(object? x, object? y)
    {
        var result = String.CompareOrdinal(((ListViewItem)x!).SubItems[column].Text, ((ListViewItem)y!).SubItems[column].Text);
        return order == SortOrder.Ascending ? result : -result;
    }
}
