using System.Diagnostics;
using System.Text.RegularExpressions;
using EveryThingSharpCore.Enums;
using EveryThingSharpCore.Fluent;
using Microsoft.Win32;

namespace CoffeeScholar.ReplaceAllGit;

public class SearchHelper(
    string[] mainFolderNames,   //["MinGW64", "Git"]
    string mainExeEndsWith = "bin\\git.exe",
    string extractVersionInfoRegexPattern = @"\d+\.\d+\.\d+",
    string getVersionInfoArguments = "--version",
    uint ignoreSmallerFileSize = 3 * 1024 * 1024)
{
    private readonly Regex _ExtractVersionInfoRegex = new(extractVersionInfoRegexPattern);

    public Dictionary<string, SearchResult> SearchFiles(Version lastVersion, bool isIgnoreSmaller, string searchPattern,
        bool isRegex = false)
    {
        // ʹ�� Everything ����
        using EverythingSearcher everything = new();
        var searchResults = everything
            .SearchFor(searchPattern)
            .SetRegex(isRegex)
            .OrderBy(Sort.NameAscending)
            .WithResultLimit(50)
            .WithOffset(0)
            .GetFields(RequestFlags.FullPathAndFileName | RequestFlags.RunCount)
            .Execute().ToArray();

        // ��֤�汾
        var versionsDict = CheckVersions(searchResults, lastVersion, isIgnoreSmaller);
        return versionsDict;
    }

    public List<SearchResult> GroupByMainFolder(Dictionary<string, SearchResult> searchResults)
    {
        // �ϲ������ FullPath ��������Ŀ¼��ͬ����
        var grouped = searchResults.GroupBy(r => r.Value.MainFolder);
        // ѡȡ�ϲ����������
        var groupedResult = new List<SearchResult>();
        foreach (var group in grouped)
        {
            // ѡȡÿ���� bin Ŀ¼�µ� git.exe ��
            var mainExe = group.FirstOrDefault(r => r.Value.FullPath.EndsWith(mainExeEndsWith)).Value;
            if (mainExe != null)
                groupedResult.Add(mainExe);
        }
        return groupedResult;
    }

    private Dictionary<string, SearchResult> CheckVersions(EverythingEntry[] results, Version lastVersion, bool isIgnoreSmaller)
    {
        var variable = "Path";
        var sysPathString = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
        var userPathString = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);

        var dictionary = new Dictionary<string, SearchResult>();
        foreach (var entry in results)
        {
            // ��ȡ��ϸ��Ϣ
            var result = CheckVersion(new SearchResult(entry));
            if (isIgnoreSmaller && ignoreSmallerFileSize > 0 && result.Size < ignoreSmallerFileSize)
                continue;

            // ����Ƿ��ڻ������� Path ��
            var fileFolder = Path.GetDirectoryName((string?)result.FullPath);
            Debug.Assert(fileFolder != null, nameof(fileFolder) + " != null");

            result.IsInSysPath = sysPathString?.Contains(fileFolder) ?? false;
            result.IsInUserPath = userPathString?.Contains(fileFolder) ?? false;
            result.IsNeedUpdating = !IsLatestVersion(result.Version, lastVersion);

            // ��Ŀ¼������ȡ���ɼ���ֱ������ָ������
            string? parentFolder = result.FullPath;
            string? folderName;
            do
            {
                // ��ȡ·�������һ��Ŀ¼��
                parentFolder = Path.GetDirectoryName(parentFolder);
                folderName = Path.GetFileName(parentFolder);
            } while (folderName != null && !mainFolderNames.Contains(folderName, StringComparer.OrdinalIgnoreCase));
            result.MainFolder = parentFolder ?? string.Empty;
            Debug.WriteLine($"{result.Version}\t{result.MainFolder}");

            // ��ӵ��ֵ�
            dictionary.Add(result.FullPath, result);

            if (!result.IsCheckingVersionSucceed)
                Debug.WriteLine(@$"ERROR: {entry}\r\n\t{result.Version}");
        }
        return dictionary;
    }

    private SearchResult CheckVersion(SearchResult result)
    {
        try
        {
            // ���� Console ִ������ git --version
            using Process process = new();
            process.StartInfo.FileName = result.FullPath;
            process.StartInfo.Arguments = getVersionInfoArguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // ��ȡ�汾�� "2.45.1"
            var versionString = process.StandardOutput.ReadToEnd();
            versionString = _ExtractVersionInfoRegex.Match(versionString).Value;
            result.Version = versionString;
            // �˳�����
            process.WaitForExitAsync();

            // ���汾�ɹ�
            result.IsCheckingVersionSucceed = true;

            // ��ȡ�ļ���Ϣ
            var fileInfo = new FileInfo(result.FullPath);
            result.Size = fileInfo.Length;
            result.LastAccessTime = fileInfo.LastAccessTime;
            result.CreationTime = fileInfo.CreationTime;
            result.LastWriteTime = fileInfo.LastWriteTime;
        }
        catch (Exception e)
        {
            result.IsCheckingVersionSucceed = false;
            result.Version = string.Empty;
            result.ErrorMessage = e.Message;
        }
        return result;
    }

    /// <summary>
    /// �ж��Ƿ������°汾
    /// </summary>
    public static bool IsLatestVersion(string nowVersionString, Version lastVersion)
    {
        // lastVersion = "2.45.1";
        // nowVersionString = "2.44.11";
        var nowVersion = new Version(nowVersionString);
        return nowVersion >= lastVersion;
    }

    private (bool isInstalled, string path) CheckRegistryForGit()
    {
        const string gitRegistryPath = @"SOFTWARE\GitForWindows";
        var key = Registry.LocalMachine.OpenSubKey(gitRegistryPath);
        if (key == null) return (false, string.Empty);

        var installPath = (string)key.GetValue("InstallPath");
        if (string.IsNullOrEmpty(installPath))
            return (false, string.Empty);
        return (true, installPath);

    }

    public static string ComposeDisplayFileSize(long fileSize)
    {
        var sizeString = fileSize switch
        {
            < 1024 => $"{fileSize} B",
            < 1024 * 1024 => $"{fileSize / 1024} KB",
            < 1024 * 1024 * 1024 => $"{fileSize / 1024 / 1024} MB",
            < 1024L * 1024 * 1024 * 1024 => $"{fileSize / 1024 / 1024 / 1024} GB",
            _ => string.Empty
        };
        return sizeString.Length > 0 ? sizeString : string.Empty;
    }
}