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
        // ����ϵͳ��ѡ������¼�
        SystemEvents.UserPreferenceChanged += OnUserPreferenceChanged;
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
    }

    private void RefreshListView(IEnumerable<SearchResult> results)
    {
        var index = 0;
        // �г����
        foreach (var result in results.OrderByDescending(r => r.Version))
        {
            index++;
            var inPath = result.IsInSysPath ? "ϵͳ" : string.Empty;
            if (inPath.Length > 0) inPath += " | ";
            inPath += result.IsInUserPath ? "�û�" : string.Empty;

            ListViewItem item = new(index.ToString("D2"));
            item.SubItems.Add(result.Version);
            item.SubItems.Add(result.IsNeedUpdating ? "������" : "������");
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
        Text = $@"Replace All Git - ���� Git �����������°汾 - {Application.ProductVersion}";
        ClearControls();
    }

    private void btnRefresh_Click(object sender, EventArgs e)
    {
        ClearControls();
        btnRefresh.Enabled = false;

        /*
        // ����ע����Ѱ�װ�� Git
        var gitInstalled = CheckRegistryForGit();
        if (gitInstalled.isInstalled)
        {
            Console.WriteLine($@"�Ѱ�װ Git for Windows��{gitInstalled.fileFullPath}");
        }
        */

        // ʹ��������ʽ�������� git.exe
        var searchHelper = new SearchHelper(["MinGW64", "Git"], "bin\\git.exe", @"\d+\.\d+\.\d+", "--version", 3 * 1024 * 1024);
        var lastVersion = new Version("2.45.1"); // ���°汾��2.45.1
        var versionList = searchHelper.SearchFiles(lastVersion, chkIgnoreSmaller.Checked, "^git.exe$", true);

        // ˢ�� ListView
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
        // ���ļ���
        var path = linkPath.Text;
        if (!File.Exists(path)) return;
        var folder = Path.GetDirectoryName(path);
        if (folder != null)
            Process.Start("explorer.exe", folder);
        linkPath.LinkVisited = false;
    }

    private void chkSelectAll_CheckedChanged(object sender, EventArgs e)
    {
        // ȫѡ����ȫ��ѡ
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
