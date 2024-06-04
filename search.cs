using System.Text.RegularExpressions;

public static class search
{
    public static string CheckLatestVersionAsync(string url, string pattern, string replacementPattern)
    {
        var html = new HttpClient().GetStringAsync(url).Result;
        var match = Regex.Match(html, pattern, RegexOptions.Singleline);
        return match.Success ? match.Result(replacementPattern) : string.Empty;
    }
}