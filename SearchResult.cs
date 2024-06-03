using EveryThingSharpCore.Fluent;

namespace CoffeeScholar.ReplaceAllGit;

public class SearchResult(EverythingEntry entry)
{
    public bool IsInUserPath { get; set; }
    public bool IsInSysPath { get; set; }
    public string FullPath { get; set; } = entry.FullPath;
    public string Version { get; set; } = string.Empty;
    public long Size { get; set; } = entry.Size;
    public uint RunCount { get; set; } = entry.RunCount;
    public DateTime? LastAccessTime { get; set; } = entry.DateRun;
    public string ErrorMessage { get; set; } = string.Empty;

    public bool IsCheckingVersionSucceed { get; set; }

    public bool IsUpdatingSucceed { get; set; }

    public bool IsNeedUpdating { get; set; }
    public DateTime? CreationTime { get; set; }
    public DateTime? LastWriteTime { get; set; }
    public string MainFolder { get; set; } = string.Empty;
}