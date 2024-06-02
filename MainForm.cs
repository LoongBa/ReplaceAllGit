using System.Collections;
using System.Diagnostics;
using Microsoft.Win32;

namespace CoffeeScholar.ReplaceAllGit;

public partial class MainForm : Form
{
    private DarkModeCS? _Dm = null;
    public MainForm()
    {
        InitializeComponent();
        _Dm = new DarkModeCS(this);
        // 订阅系统首选项更改事件
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
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
            lsvResult.Items.Add(item);
        }
    }

    private int _SortColumn = -1;

    private void ClearControls()
    {
        lsvResult.Items.Clear();
        lblNumber.Text = string.Empty;
        lblVersion.Text = string.Empty;
        linkPath.Text = string.Empty;
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

        // 使用正则表达式查找所有 git.exe
        var searchHelper = new SearchHelper(["MinGW64", "Git"], "bin\\git.exe", @"\d+\.\d+\.\d+", "--version", 3 * 1024 * 1024);
        var lastVersion = new Version("2.45.1"); // 最新版本：2.45.1
        var versionList = searchHelper.SearchFiles(lastVersion, chkIgnoreSmaller.Checked, "^git.exe$", true);

        // 刷新 ListView
        if (chkCombineSameFolder.Checked)
            RefreshListView(searchHelper.GroupByMainFolder(versionList));
        else
            RefreshListView(versionList.Values);

        btnRefresh.Enabled = true;
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

    private void lsvResult_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (lsvResult.SelectedItems.Count > 0)
        {
            var item = lsvResult.SelectedItems[0];
            lblNumber.Text = item.Text;
            linkPath.Text = item.SubItems[6].Text;
            lblVersion.Text = item.SubItems[1].Text;
        }
    }

    private void linkPath_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        // 打开文件夹
        var path = linkPath.Text;
        if (!File.Exists(path)) return;
        var folder = Path.GetDirectoryName(path);
        if (folder != null)
            Process.Start("explorer.exe", folder);
        linkPath.LinkVisited = false;
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        // 全选，或全不选
        foreach (ListViewItem item in lsvResult.Items)
            item.Checked = chkSelectAll.Checked;
    }

    private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
    {
        linkLabel1.LinkVisited = false;

        // Open the link in a browser.
        Process.Start("https://gitforwindows.org/");
    }
}

internal class ListViewItemComparer(int column, SortOrder order) : IComparer
{
    public int Compare(object x, object y)
    {
        var result = string.Compare(((ListViewItem)x).SubItems[column].Text, ((ListViewItem)y).SubItems[column].Text);
        return order == SortOrder.Ascending ? result : -result;
    }
}
