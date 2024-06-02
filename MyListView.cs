namespace CoffeeScholar.ReplaceAllGit;

public class MyListView : ListView
{
    public MyListView()
    {
        // 开启 OwnerDraw，这样我们可以自己绘制项
        //OwnerDraw = true;

        // 添加一个项
        //Items.Add("This is some text. This part is red.");

        // DrawItem += OnDrawItem;

        SizeChanged += (_, _) =>
        {
            if (Columns.Count >= 1) 
                Columns[^1].Width = -2;
        };
    }

    private void OnDrawItem(object sender, DrawListViewItemEventArgs e)
    {
        // 获取要绘制的文字
        var text = e.Item.Text;
        if (string.IsNullOrEmpty(text)) return;

        text = "test1 \\033[31m test2 \\033[0m test3";
        AnsiStringHelper.DrawString(e.Graphics, text, Font, e.Bounds);

        // 使用默认的字体和红色绘制文字的后半部分
        // var size = e.Graphics.MeasureString(text, Font);
        // e.Graphics.DrawString(text.Substring(startIndex), Font, Brushes.Red, e.Bounds.Left + size.Width, e.Bounds.Top);
    }
}