namespace ReplaceAllGit;

public class MyListView : ListView
{
    public MyListView()
    {
        // 开启 OwnerDraw，这样我们可以自己绘制项
        this.OwnerDraw = true;

        // 添加一个项
        this.Items.Add("This is some text. This part is red.");

        this.DrawItem += this.OnDrawItem;
    }

    private void OnDrawItem(object sender, DrawListViewItemEventArgs e)
    {
        // 获取要绘制的文字
        string text = e.Item.Text;

        // 找到红色部分的开始和结束位置
        int startIndex = text.IndexOf("This part is red");
        int endIndex = startIndex + "This part is red".Length;

        // 使用默认的字体和颜色绘制文字的前半部分
        var coloredText = text.Substring(0, startIndex);
        e.Graphics.DrawString(coloredText, this.Font, Brushes.Black, e.Bounds);

        // 使用默认的字体和红色绘制文字的后半部分
        SizeF size = e.Graphics.MeasureString(text, this.Font);
        e.Graphics.DrawString(text.Substring(startIndex), this.Font, Brushes.Red, e.Bounds.Left + size.Width, e.Bounds.Top);
    }
}