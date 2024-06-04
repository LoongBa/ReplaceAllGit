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

    public Dictionary<string, TSearchResult> SearchFiles<TSearchResult>(Version lastVersion, bool isIgnoreSmaller, string searchPattern,
        bool isRegex = false)
    where TSearchResult : SearchResult, new()
    {
        // ʹ�� Everything ����
        var noRexPattern = @$"wholefilename:{searchPattern}";
        using EverythingSearcher everything = new();
        var searchResults = everything
            .SearchFor(isRegex ? searchPattern : noRexPattern)
            .SetRegex(isRegex)
            .OrderBy(Sort.NameAscending)
            .WithResultLimit(50)
            .WithOffset(0)
            .GetFields(RequestFlags.FullPathAndFileName | RequestFlags.RunCount)
            .Execute().ToArray();

        // ��֤�汾
        var versionsDict = CheckVersions<TSearchResult>(searchResults, lastVersion, isIgnoreSmaller);
        return versionsDict;
    }

    public List<string> SearchFilesWith<TSearchResult>(string searchPattern, string targetFolder)
    where TSearchResult : SearchResult, new()
    {
        // ʹ�� Everything ����
        var pattern = @$"wholefilename:{searchPattern} {targetFolder}";
        using EverythingSearcher everything = new();
        var searchResults = everything
            .SearchFor(pattern)
            .SetRegex(false)
            .OrderBy(Sort.NameAscending)
            .WithResultLimit(100)
            .WithOffset(0)
            .GetFields(RequestFlags.FullPathAndFileName)
            .Execute().ToArray();

        var searchedFiles = new List<string>();
        foreach (var entry in searchResults)
        {
            var result = new TSearchResult().CopyValuesFrom<TSearchResult>(entry);
            searchedFiles.Add(result.FullPath);
        }

        return searchedFiles;
    }

    public List<string> CopyFilesFrom(string fromMainFolder, string toMainFolder)
    {
        if (!Directory.Exists(fromMainFolder))
            throw new DirectoryNotFoundException($"�ļ��в����ڣ�{fromMainFolder}��");
        if (!Directory.Exists(toMainFolder))
            throw new DirectoryNotFoundException($"�ļ��в����ڣ�{toMainFolder}��");

        // �� fromMainFolder ���Ƶ� toMainFolder��������Ŀ¼�������Ѵ��ڵ��ļ�
        var copiedFiles = new List<string>();
        var result = CopyFilesRecursive(fromMainFolder, toMainFolder, copiedFiles);
        return result;
    }

    /// <summary>
    /// �ݹ鸴���ļ����е������ļ�
    /// </summary>
    /// <param name="sourceFolder">Դ�ļ���·��</param>
    /// <param name="destinationFolder">Ŀ���ļ���·��</param>
    /// <param name="copiedFiles">�Ѹ����ļ��б�</param>
    /// <returns>���Ƴɹ����ļ��б�</returns>
    private List<string> CopyFilesRecursive(string sourceFolder, string destinationFolder, ICollection<string> copiedFiles)
    {
        var result = new List<string>();
        // ��ȡԴ�ļ����е������ļ�
        var files = Directory.GetFiles(sourceFolder);

        foreach (var file in files)
        {
            // ��ȡ�ļ���
            var fileName = Path.GetFileName(file);
            // ����Ŀ���ļ�·��
            var destinationPath = Path.Combine(destinationFolder, fileName);
            // ȷ��Ŀ���ļ��д���
            Directory.CreateDirectory(destinationFolder);
            // �����ļ�
            File.Copy(file, destinationPath, true);
            // ��ӵ��Ѹ����ļ��б�
            copiedFiles.Add(destinationPath);
        }

        // ��ȡԴ�ļ����е��������ļ���
        var subfolders = Directory.GetDirectories(sourceFolder);

        foreach (var subfolder in subfolders)
        {
            // ����Ŀ�����ļ���·��
            var destinationSubfolder = Path.Combine(destinationFolder, Path.GetFileName(subfolder));
            // �ݹ鸴�����ļ����е��ļ�
            result = CopyFilesRecursive(subfolder, destinationSubfolder, copiedFiles);
        }
        // ���ظ��Ƴɹ����ļ��б�Ӧ�ð��������ļ����������ļ����е��ļ���
        return result;
    }

    public List<TSearchResult> GroupByMainFolder<TSearchResult>(Dictionary<string, TSearchResult> searchResults)
    where TSearchResult : SearchResult, new()
    {
        // �ϲ������ FullPath ��������Ŀ¼��ͬ����
        var grouped = searchResults.GroupBy(r => r.Value.MainFolder);
        // ѡȡ�ϲ����������
        var groupedResult = new List<TSearchResult>();
        foreach (var group in grouped)
        {
            // ѡȡÿ���� bin Ŀ¼�µ� git.exe ��
            var mainExe = group.FirstOrDefault(r => r.Value.FullPath.EndsWith(mainExeEndsWith)).Value;
            if (mainExe != null)
                groupedResult.Add(mainExe);
        }
        return groupedResult;
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

    private Dictionary<string, TSearchResult> CheckVersions<TSearchResult>(EverythingEntry[] results, Version lastVersion, bool isIgnoreSmaller)
        where TSearchResult : SearchResult, new()

    {
        #region ��ȡ���������� Path ��ֵ
        /*
        var variable = "Path";
        var sysPathString = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
        var userPathString = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
        */
        #endregion

        var dictionary = new Dictionary<string, TSearchResult>();
        foreach (var entry in results)
        {
            // ��ȡ��ϸ��Ϣ
            var searchResult = new TSearchResult().CopyValuesFrom<TSearchResult>(entry);
            var result = CheckVersion(searchResult);
            if (isIgnoreSmaller && ignoreSmallerFileSize > 0 && result.Size < ignoreSmallerFileSize)
                continue;

            // ����Ƿ��ڻ������� Path ��
            var fileFolder = Path.GetDirectoryName((string?)result.FullPath);
            Debug.Assert(fileFolder != null, nameof(fileFolder) + " != null");

            // �ж��Ƿ��ڻ������� Path ��
            // result.IsInSysPath = sysPathString?.Contains(fileFolder) ?? false;
            // result.IsInUserPath = userPathString?.Contains(fileFolder) ?? false;
            // result.IsInWherePath = gitPaths?.Any(p => p.Contains(fileFolder)) ?? false;

            // �ж��Ƿ���Ҫ����
            result.IsNeedUpdating = !IsLatestVersion(result.Version, lastVersion);

            // ��Ŀ¼������ȡ���ɼ���ֱ������ָ������
            var parentFolder = result.FullPath;
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

    public static string[] GetPathsByCmdWhere(string exeName)
    {
        // ���� CMD "where git.exe" �鿴���� git.exe ��·��
        var psi = new ProcessStartInfo("where", exeName)
        {
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using var process = Process.Start(psi);
        var output = process?.StandardOutput.ReadToEnd();
        process?.WaitForExit();
        var gitPaths = output?.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        return gitPaths ?? [];
    }

    private TSearchResult CheckVersion<TSearchResult>(TSearchResult result)
    where TSearchResult : SearchResult, new()
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

    public (bool isInstalled, string path) CheckRegistryForGit()
    {
        const string gitRegistryPath = @"SOFTWARE\GitForWindows";
        var key = Registry.LocalMachine.OpenSubKey(gitRegistryPath);
        if (key == null) return (false, string.Empty);

        var installPath = (string)key.GetValue("InstallPath")!;
        if (string.IsNullOrEmpty(installPath))
            return (false, string.Empty);
        return (true, installPath);

    }

    public static string FormatFileSizeForDisplay(long fileSize)
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

    public static async Task<string> CheckLatestVersionAsync(string url, string pattern, string replacementPattern, RegexOptions? regexOptions = null)
    {
        var html = await new HttpClient().GetStringAsync(url);
        Match? match;
        if (regexOptions != null)
        {
            var options = regexOptions.Value;
            match = Regex.Match(html, pattern, options);
        }
        else
            match = Regex.Match(html, pattern);

        return match.Success ? match.Result(replacementPattern) : string.Empty;
    }
}