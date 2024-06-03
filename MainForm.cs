using System.Collections;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using CoffeeScholar.ReplaceAllGit.DarkMode;
using Microsoft.Win32;

namespace CoffeeScholar.ReplaceAllGit;

public partial class MainForm : Form
{
    private readonly DarkModeCS? _DarkMode;
    private int _SortColumn = -1;

    private bool _IsDefaultSet;
    private bool _ItemSelected;

    private const string SearchPattern = "^git.exe$";
    private readonly SearchHelper _SearchHelper;
    private Version _LastVersion;
    private Dictionary<string, SearchResult> _VersionList = new();
    private SearchResult? _DefaultSearchResult;
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
        }

        var version = Task.Run(() =>
        {
            var url = "https://git-scm.com/";
            var pattern = @"<span class=""version"">\s*?(?<version>\d+\.\d+\.\d+)\s*?</span>";
            var version1 = CheckLatestVersionAsync(url, pattern);

            url = "https://gitforwindows.org/";
            pattern = @"<div class=""version""><a .*?>Version\s?(?<version>\d+\.\d+\.\d+)\s?</a></div>";
            var version2 = CheckLatestVersionAsync(url, pattern);

            var version = string.IsNullOrEmpty(version1)
                ? new Version(version2)
                : new Version(version1);

            return version;
        });

        _LastVersion = version.Result;
        lblLatestVersion.Text = $@"最新版本：{_LastVersion}";

        // 使用正则表达式查找所有 git.exe
        _SearchHelper = new SearchHelper(["MinGW64", "Git"], "bin\\git.exe", @"\d+\.\d+\.\d+", "--version", 3 * 1024 * 1024);
    }

    private static string CheckLatestVersionAsync(string url, string pattern)
    {
        var html = new HttpClient().GetStringAsync(url).Result;
        var match = Regex.Match(html, pattern, RegexOptions.Singleline);
        var replacement = "${version}";
        return match.Success ? match.Result(replacement) : string.Empty;
    }
    private void RefreshListView(IEnumerable<SearchResult> results)
    {
        var index = 0;
        // 列出结果
        foreach (var result in results.OrderByDescending(r => r.Version))
        {
            index++;
            var inPath = result.IsInSysPath ? "系统" : string.Empty;
            if (inPath.Length > 0) inPath += " | ";
            inPath += result.IsInUserPath ? "用户" : string.Empty;

            ListViewItem item = new(index.ToString("D2"));
            item.SubItems.Add(result.Version);
            item.SubItems.Add(result.IsNeedUpdating ? "需升级" : "已最新");
            item.SubItems.Add(inPath);
            item.SubItems.Add(SearchHelper.ComposeDisplayFileSize(result.Size));
            item.SubItems.Add(result.LastWriteTime?.ToString("yyyy-MM-dd HH:mm:ss"));
            item.SubItems.Add(result.FullPath);
            item.Tag = result;
            lsvResult.Items.Add(item);
        }
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

    private void ClearControls()
    {
        lsvResult.Items.Clear();
        lblNumber.Text = string.Empty;
        lblVersion.Text = string.Empty;
        linkPath.Text = string.Empty;
    }

    private void CheckButtons()
    {
        btnUpdate.Enabled = btnRestore.Enabled =
            _IsDefaultSet && lsvResult.Items.Cast<ListViewItem>().Any(i => i.Checked);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        Text = $@"Replace All Git - 查找 Git 并升级到最新版本 - {Application.ProductVersion}";
        ClearControls();
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        ClearControls();
        btnRefresh.Enabled = false;

        /*
        // 查找注册表，已安装的 Git
        var gitInstalled = CheckRegistryForGit();
        if (gitInstalled.isInstalled)
        {
            Console.WriteLine($@"已安装 Git for Windows：{gitInstalled.fileFullPath}");
        }
        */

        _VersionList = _SearchHelper.SearchFiles(_LastVersion, chkIgnoreSmaller.Checked, SearchPattern, true);
        // 刷新 ListView
        if (chkCombineSameFolder.Checked)
            RefreshListView(_SearchHelper.GroupByMainFolder(_VersionList));
        else
            RefreshListView(_VersionList.Values);

        btnRefresh.Enabled = true;
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
            btnRefresh_Click(this, e);  // 刷新 ListView
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
                _DefaultSearchResult = item.Tag as SearchResult;

                _IsDefaultSet = true;
                btnSetAsDefault.Text = @"&C 取消默认";
            }
        }
        CheckButtons();
    }

    private void lsvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        _ItemSelected = lsvResult.SelectedItems.Count > 0;

        btnSetAsDefault.Enabled = _ItemSelected;
        CheckButtons();

        if (_ItemSelected && !_IsDefaultSet)
        {
            var item = lsvResult.SelectedItems[0];
            lblNumber.Text = item.Text;
            linkPath.Text = @"    " + item.SubItems[6].Text;
            lblVersion.Text = item.SubItems[1].Text;
        }
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

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        // 全选，或全不选
        foreach (ListViewItem item in lsvResult.Items)
            item.Checked = chkSelectAll.Checked;
    }

    private void linkPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // 打开文件夹
        var path = linkPath.Text.Trim();
        if (!File.Exists(path)) return;
        var folder = Path.GetDirectoryName(path);
        if (folder != null)
            Process.Start("explorer.exe", folder);
        linkPath.LinkVisited = false;
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
}

internal class ListViewItemComparer(int column, SortOrder order) : IComparer
{
    public int Compare(object? x, object? y)
    {
        var result = String.CompareOrdinal(((ListViewItem)x!).SubItems[column].Text, ((ListViewItem)y!).SubItems[column].Text);
        return order == SortOrder.Ascending ? result : -result;
    }
}
