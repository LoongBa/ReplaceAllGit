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
        // 使用 Everything 搜索
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

        // 验证版本
        var versionsDict = CheckVersions<TSearchResult>(searchResults, lastVersion, isIgnoreSmaller);
        return versionsDict;
    }

    public List<string> SearchFilesWith<TSearchResult>(string searchPattern, string targetFolder)
    where TSearchResult : SearchResult, new()
    {
        // 使用 Everything 搜索
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
            throw new DirectoryNotFoundException($"文件夹不存在：{fromMainFolder}。");
        if (!Directory.Exists(toMainFolder))
            throw new DirectoryNotFoundException($"文件夹不存在：{toMainFolder}。");

        // 从 fromMainFolder 复制到 toMainFolder，包括子目录、覆盖已存在的文件
        var copiedFiles = new List<string>();
        var result = CopyFilesRecursive(fromMainFolder, toMainFolder, copiedFiles);
        return result;
    }

    /// <summary>
    /// 递归复制文件夹中的所有文件
    /// </summary>
    /// <param name="sourceFolder">源文件夹路径</param>
    /// <param name="destinationFolder">目标文件夹路径</param>
    /// <param name="copiedFiles">已复制文件列表</param>
    /// <returns>复制成功的文件列表</returns>
    private List<string> CopyFilesRecursive(string sourceFolder, string destinationFolder, ICollection<string> copiedFiles)
    {
        var result = new List<string>();
        // 获取源文件夹中的所有文件
        var files = Directory.GetFiles(sourceFolder);

        foreach (var file in files)
        {
            // 获取文件名
            var fileName = Path.GetFileName(file);
            // 构建目标文件路径
            var destinationPath = Path.Combine(destinationFolder, fileName);
            // 确保目标文件夹存在
            Directory.CreateDirectory(destinationFolder);
            // 复制文件
            File.Copy(file, destinationPath, true);
            // 添加到已复制文件列表
            copiedFiles.Add(destinationPath);
        }

        // 获取源文件夹中的所有子文件夹
        var subfolders = Directory.GetDirectories(sourceFolder);

        foreach (var subfolder in subfolders)
        {
            // 构建目标子文件夹路径
            var destinationSubfolder = Path.Combine(destinationFolder, Path.GetFileName(subfolder));
            // 递归复制子文件夹中的文件
            result = CopyFilesRecursive(subfolder, destinationSubfolder, copiedFiles);
        }
        // 返回复制成功的文件列表，应该包含所有文件（包括子文件夹中的文件）
        return result;
    }

    public List<TSearchResult> GroupByMainFolder<TSearchResult>(Dictionary<string, TSearchResult> searchResults)
    where TSearchResult : SearchResult, new()
    {
        // 合并结果中 FullPath 的上两级目录相同的项
        var grouped = searchResults.GroupBy(r => r.Value.MainFolder);
        // 选取合并后的主程序
        var groupedResult = new List<TSearchResult>();
        foreach (var group in grouped)
        {
            // 选取每组中 bin 目录下的 git.exe 项
            var mainExe = group.FirstOrDefault(r => r.Value.FullPath.EndsWith(mainExeEndsWith)).Value;
            if (mainExe != null)
                groupedResult.Add(mainExe);
        }
        return groupedResult;
    }

    /// <summary>
    /// 判断是否是最新版本
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
        #region 获取环境变量中 Path 的值
        /*
        var variable = "Path";
        var sysPathString = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.Machine);
        var userPathString = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
        */
        #endregion

        var dictionary = new Dictionary<string, TSearchResult>();
        foreach (var entry in results)
        {
            // 获取详细信息
            var searchResult = new TSearchResult().CopyValuesFrom<TSearchResult>(entry);
            var result = CheckVersion(searchResult);
            if (isIgnoreSmaller && ignoreSmallerFileSize > 0 && result.Size < ignoreSmallerFileSize)
                continue;

            // 检查是否在环境变量 Path 中
            var fileFolder = Path.GetDirectoryName((string?)result.FullPath);
            Debug.Assert(fileFolder != null, nameof(fileFolder) + " != null");

            // 判断是否在环境变量 Path 中
            // result.IsInSysPath = sysPathString?.Contains(fileFolder) ?? false;
            // result.IsInUserPath = userPathString?.Contains(fileFolder) ?? false;
            // result.IsInWherePath = gitPaths?.Any(p => p.Contains(fileFolder)) ?? false;

            // 判断是否需要更新
            result.IsNeedUpdating = !IsLatestVersion(result.Version, lastVersion);

            // 主目录：向上取若干级，直到符合指定名字
            var parentFolder = result.FullPath;
            string? folderName;
            do
            {
                // 获取路径的最后一级目录名
                parentFolder = Path.GetDirectoryName(parentFolder);
                folderName = Path.GetFileName(parentFolder);
            } while (folderName != null && !mainFolderNames.Contains(folderName, StringComparer.OrdinalIgnoreCase));
            result.MainFolder = parentFolder ?? string.Empty;
            Debug.WriteLine($"{result.Version}\t{result.MainFolder}");

            // 添加到字典
            dictionary.Add(result.FullPath, result);

            if (!result.IsCheckingVersionSucceed)
                Debug.WriteLine(@$"ERROR: {entry}\r\n\t{result.Version}");
        }
        return dictionary;
    }

    public static string[] GetPathsByCmdWhere(string exeName)
    {
        // 运行 CMD "where git.exe" 查看所有 git.exe 的路径
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
            // 调用 Console 执行命令 git --version
            using Process process = new();
            process.StartInfo.FileName = result.FullPath;
            process.StartInfo.Arguments = getVersionInfoArguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.Start();

            // 提取版本号 "2.45.1"
            var versionString = process.StandardOutput.ReadToEnd();
            versionString = _ExtractVersionInfoRegex.Match(versionString).Value;
            result.Version = versionString;
            // 退出进程
            process.WaitForExitAsync();

            // 检查版本成功
            result.IsCheckingVersionSucceed = true;

            // 获取文件信息
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