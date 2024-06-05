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
        // 订阅系统首选项更改事件
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

        // 初始化
        _SearchHelper = new SearchHelper(["MinGW64", "Git"], "bin\\git.exe", @"\d+\.\d+\.\d+", "--version", 3 * 1024 * 1024);
    }

    private void OnUserPreferenceChanged(object sender, UserPreferenceChangedEventArgs e)
    {
        switch (e.Category)
        {
            case UserPreferenceCategory.General:
                // 用户更改了一般首选项设置
                break;
            case UserPreferenceCategory.Desktop:
                // 用户更改了桌面相关的首选项设置
                break;
                // 处理其他类别...
        }

        // 可能需要刷新应用程序的设置
        //_DarkMode?.TryDarkModeTheme();

    }

    #region 交互方法

    private void RefreshListView(IEnumerable<GitSearchResult> results)
    {
        var index = 0;
        // 列出结果
        var gitPaths = _WhereIsGit_Paths_;
        foreach (var result in results.OrderByDescending(r => r.Version))
        {
            index++;
            /*
            var type = result.IsInSysPath ? "系统" : string.Empty;
            if (type.Length > 0) type += " | ";
            type += result.IsInUserPath ? "用户" : string.Empty;
            */
            result.IsInWherePath = _WhereIsGit_Paths_.Contains(result.FullPath);
            var type = result.IsInWherePath ? "在" : "";
            ListViewItem item = new(result.Index = index.ToString("D2"));
            item.SubItems.Add(result.Version);
            item.SubItems.Add(result.IsNeedUpdating ? "需升级" : "");
            item.SubItems.Add(result.HasGitBash ? "有" : "");
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

    #region 业务方法

    private async Task<Version> CheckLatestVersionsAsync()
    {
        var replacementPattern = "${version}";

        // 通过 Git-Scm 获取最新版本号 
        var url = "https://git-scm.com/";
        var searchPattern = @"<span class=""version"">\s*?(?<version>\d+\.\d+\.\d+)\s*?</span>";
        var task1 = SearchHelper.CheckLatestVersionAsync(url, searchPattern, replacementPattern, RegexOptions.Singleline);

        // 通过 gitforWindows.org 获取最新版本号
        url = "https://gitforwindows.org/";
        searchPattern = @"<div class=""version""><a .*?>Version\s?(?<version>\d+\.\d+\.\d+)\s?</a></div>";
        var task2 = SearchHelper.CheckLatestVersionAsync(url, searchPattern, replacementPattern, RegexOptions.Singleline);

        // 等待两个任务完成
        var versions = await Task.WhenAll(task1, task2);
        // 判断最新的版本号
        var latestVersion = versions.Max();

        // 返回最新版本号
        return new Version(latestVersion ?? "");
    }
    private async Task<(string githubUrl, string huaweiUrl, string huaweiHomeUrl)> GetDownloadUrlAsync()
    {
        var replacementPattern = "${url}";

        /*
        // 通过 gitforwindows.org 获取最新版本号
        var url = "https://gitforwindows.org";
        //<a class="button featurebutton" href="https://github.com/git-for-windows/git/releases/download/v2.45.2.windows.1/Git-2.45.2-64-bit.exe" target="_blank">Download</a>
        var searchPattern = @"<a class=""button featurebutton"" href=""(?<url>.*?)"" target=""_blank"">Download</a>";
        */

        // 通过 git-scm.com 获取最新版本号
        var url = "https://git-scm.com/download/win";
        //<a href="https://github.com/git-for-windows/git/releases/download/v2.45.2.windows.1/Git-2.45.2-64-bit.exe">64-bit Git for Windows Setup</a>
        var searchPattern = @"<a href=\""(?<url>[^\""]*)\""[^<]*>64-bit Git for Windows Setup</a>";
        var githubUrl = await SearchHelper.CheckLatestVersionAsync(url, searchPattern, replacementPattern, RegexOptions.Singleline);

        // 替换为华为镜像地址
        // https://github.com/git-for-windows/git/releases/download/v2.45.2.windows.1/Git-2.45.2-64-bit.exe
        // https://mirrors.huaweicloud.com/git-for-windows/v2.45.2.windows.1/Git-2.45.2-64-bit.exe
        var huaweiUrl = githubUrl.Replace("https://github.com/git-for-windows/git/releases/download/", "https://mirrors.huaweicloud.com/git-for-windows/");

        // 正则表达式匹配URL直到最后一个斜杠
        var match = Regex.Match(huaweiUrl, @"(.*/).*");
        // 获取匹配的第一组，也就是URL直到最后一个斜杠的部分
        var huaweiHomeUrl = match.Groups[1].Value;

        // 返回最新版本号
        return (githubUrl, huaweiUrl, huaweiHomeUrl);
    }

    /// <summary>
    /// 查找 git-bash.exe
    /// </summary>
    private void SearchGitBash(Dictionary<string, GitSearchResult> versionList)
    {
        foreach (var pair in versionList)
        {
            // 在上级目录搜索 git-bash.exe
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
    /// 基于 git-bash.exe 升级 Git
    /// </summary>
    /// <param name="selectedSearchResult"></param>
    private bool RunGitBash(GitSearchResult? selectedSearchResult)
    {
        if (selectedSearchResult == null) return false;

        var gitBashPath = selectedSearchResult.GitBashFullPath;
        if (string.IsNullOrEmpty(gitBashPath) || !File.Exists(gitBashPath))
            return false;

        // 运行 git-bash.exe
        // var arguments = "-c \"git update-git-for-windows\"";

        var startInfo = new ProcessStartInfo
        {
            FileName = gitBashPath,
            // Arguments = arguments,
            UseShellExecute = false,
            // RedirectStandardOutput = true,
            // RedirectStandardError = true,  // 重定向错误流
            CreateNoWindow = false
        };

        try
        {
            using var process = Process.Start(startInfo);
            /*
            if (process != null)
            {
                // // 读取输出
                // var versionOutput = process.StandardOutput.ReadToEnd();
                // // 读取错误输出
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
        Text = $@"Replace All Git - 查找 Git 并升级到最新版本 - {Application.ProductVersion}";
        ClearControls();

        #region 检查最新版本
        var version = await CheckLatestVersionsAsync();
        _LastVersion = version;
        lblLatestVersion.Text = _LastVersion.ToString();
        lblLatestVersion.Visible = true;
        #endregion

        // 获取 where 命令中是否包含指定的程序
        var exeName = "git.exe";
        await Task.Run(async () =>
        {
            // 获取 git.exe 的路径
            var gitPaths = SearchHelper.GetPathsByCmdWhere(exeName);
            _WhereIsGit_Paths_ = gitPaths;

            // 在 UI 线程中更新 linkWhere.Text
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

        // 搜索所有 git.exe
        _VersionList = _SearchHelper.SearchFiles<GitSearchResult>(_LastVersion, chkIgnoreSmaller.Checked, SearchPattern, true);
        // 搜索 git-bash.exe
        SearchGitBash(_VersionList);

        // 刷新 ListView
        if (chkCombineSameFolder.Checked)
            RefreshListView(_SearchHelper.GroupByMainFolder(_VersionList));
        else
            RefreshListView(_VersionList.Values);

        btnSearchAll.Enabled = true;
    }

    private void btnUpdate_Click(object sender, EventArgs e)
    {
        // 升级选中项
        var selectedItems = lsvResult.Items.Cast<ListViewItem>().Where(i => i.Checked).ToList();
        if (selectedItems.Count == 0)
        {
            MessageBox.Show(@"请先选择要升级的 Git 版本。", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // MessageBox 确认
            if (MessageBox.Show($"""
                                 是否从 
                                    {fromMainFolder}
                                 复制文件到：
                                 	{toMainFolder}
                                 """,
                    @"确认操作", MessageBoxButtons.OKCancel) == DialogResult.OK)
            {
                needRefresh = true;
                var logs = _SearchHelper.CopyFilesFrom(fromMainFolder, toMainFolder);
                {
                    var logText = new StringBuilder();
                    foreach (var log in logs)
                        logText.AppendLine($"\t\t{log}");
                    MessageBox.Show(
                        $"""
                         从 
                            {fromMainFolder} 
                         复制文件到：
                         	{toMainFolder}，
                         操作成功。
                            {logText}
                         """,
                        @"确认操作", MessageBoxButtons.OK);
                }
                Debug.WriteLine(logs);
            }
        }
        if (needRefresh)
            btnSearchAll_Click(this, e);  // 刷新 ListView
    }

    private void btnSetAsDefault_Click(object sender, EventArgs e)
    {
        if (_IsDefaultSet)
        {
            // 取消默认
            _DefaultSearchResult = null;

            _IsDefaultSet = false;
            btnSetAsDefault.Text = @"&D 设为默认";
            lsvResult_SelectedIndexChanged(sender, e);
        }
        else
        {
            // 设置为默认
            if (lsvResult.SelectedItems.Count != 0)
            {
                var item = lsvResult.SelectedItems[0];
                item.Checked = false;
                _DefaultSearchResult = item.Tag as GitSearchResult;

                _IsDefaultSet = true;
                btnSetAsDefault.Text = @"&C 取消默认";
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
            MessageBox.Show(@"Git-Bash 升级失败。", @"提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            // 如果点击的列不是当前排序的列，那么使用升序排序
            _SortColumn = e.Column;
            lsvResult.Sorting = SortOrder.Ascending;
        }
        else
        {
            // 如果点击的列是当前排序的列，那么反转排序顺序
            lsvResult.Sorting = lsvResult.Sorting == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
        }

        // 使用 ListViewItemSorter 对项进行排序
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
                // 不改变 Check 
                e.NewValue = e.CurrentValue;
            }

        }

    }

    private void lsvResult_SizeChanged(object sender, EventArgs e)
    {
        // 自动调整列宽
        lsvResult.Columns[7].Width = -2;
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        // 全选，或全不选
        foreach (ListViewItem item in lsvResult.Items)
            item.Checked = chkSelectAll.Checked;
    }

    private async void lblLatestVersion_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
    {
        var (githubUrl, huaweiUrl, huaweiHomeUrl) = await GetDownloadUrlAsync();
        var url = githubUrl;
        var homeUrl = "https://gitforwindows.org/";
        var dialogResult = MessageBox.Show(@"是否用华为镜像代替 Github.com 下载地址？",
                                        @"访问下载网址",
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
        // 打开浏览器，访问下载地址
        var psi = new ProcessStartInfo
        {
            FileName = url,
            UseShellExecute = true
        };
        Process.Start(psi);

        // 打开浏览器，访问下载地址
        psi = new ProcessStartInfo
        {
            FileName = homeUrl,
            UseShellExecute = true
        };
        Process.Start(psi);
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        //\u1F310 地球 \u1F517 链接
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
        // 打开文件夹
        var path = linkPath.Text.Trim();
        linkPath.LinkVisited = false;
        OpenWithExplorer(path);
    }

    private void LinkWhereLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // 打开文件夹
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
