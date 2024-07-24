using EveryThingSharpCore.Fluent;

namespace LoongBa.ReplaceAllGit;

public class SearchResult
{
    // 静态工厂方法
    public virtual TSearchResult CopyValuesFrom<TSearchResult>(EverythingEntry entry)
        where TSearchResult : SearchResult, new()
    {
        FullPath = entry.FullPath;
        Size = entry.Size;
        RunCount = entry.RunCount;
        LastAccessTime = entry.DateRun;

        return (this as TSearchResult)!;
    }
    public string Index { get; set; } = string.Empty;
    public bool IsInUserPath { get; set; }
    public bool IsInSysPath { get; set; }
    /// <summary>
    /// 是否在 where 能找到的路径中
    /// </summary>
    public bool IsInWherePath { get; set; }
    public string FullPath { get; set; } = string.Empty;
    public string Version { get; set; } = string.Empty;
    public long Size { get; set; }
    public uint RunCount { get; set; }
    public DateTime? LastAccessTime { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;

    public bool IsCheckingVersionSucceed { get; set; }

    public bool IsUpdatingSucceed { get; set; }

    public bool IsNeedUpdating { get; set; }
    public DateTime? CreationTime { get; set; }
    public DateTime? LastWriteTime { get; set; }
    public string MainFolder { get; set; } = string.Empty;
}

public class GitSearchResult : SearchResult
{
    public GitSearchResult()
    {
    }

    public string GitBashFullPath { get; set; } = string.Empty;
    public bool HasGitBash => GitBashFullPath.Length > 0;
}