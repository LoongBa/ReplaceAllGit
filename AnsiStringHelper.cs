using System.Text.RegularExpressions;

namespace CoffeeScholar.ReplaceAllGit;

public class AnsiStringHelper
{
    // 创建一个正则表达式来匹配 ANSI 转义序列
    private static readonly Regex AnsiRegex = new(@"\033\[(\d+)m");

    public static void DrawString(Graphics graphics, string text, Font font, Rectangle rectangle)
    {

        return;
        // 初始化颜色为黑色
        var color = Color.Red;

        var startIndex = 0;
        foreach (Match match in AnsiRegex.Matches(text))
        {
            // 绘制前一个颜色的文本
            var substring = text.Substring(startIndex, match.Index - startIndex);
            var size = graphics.MeasureString(substring, font);
            using (Brush brush = new SolidBrush(color))
            {
                graphics.DrawString(substring, font, brush, rectangle);
            }

            // 更新位置
            rectangle.X += (int)size.Width;

            // 更新颜色
            var code = int.Parse(match.Groups[1].Value);
            color = GetColorByAnsi(code);
            startIndex = match.Index + match.Length;
        }

        // 绘制最后一个颜色的文本
        var remaining = text.Substring(startIndex);
        using (Brush brush = new SolidBrush(color))
        {
            graphics.DrawString(remaining, font, brush, rectangle);
        }
    }

    private static readonly Dictionary<int, Color> ColorMap = new()
    {
        { 0, Color.Black },
        { 30, Color.Black },
        { 31, Color.Red },
        { 32, Color.Green },
        { 33, Color.Yellow },
        { 34, Color.Blue },
        { 35, Color.Magenta },
        { 36, Color.Cyan },
        { 37, Color.White },
        { 90, Color.Gray },
        { 91, Color.LightCoral },
        { 92, Color.LightGreen },
        { 93, Color.LightYellow },
        { 94, Color.LightBlue },
        { 95, Color.LightPink },
        { 96, Color.LightCyan },
        { 97, Color.White }
    };
    public static Color GetColorByAnsi(int code)
    {
        var color = ColorMap.TryGetValue(code, out Color mappedColor)
            ? mappedColor
            : Color.Black; // 如果找不到对应的颜色，使用默认颜色

        return color;
    }
}