namespace CoffeeScholar.ReplaceAllGit;

public class MyListView : ListView
{
    public MyListView()
    {
        // ���� OwnerDraw���������ǿ����Լ�������
        //OwnerDraw = true;

        // ���һ����
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
        // ��ȡҪ���Ƶ�����
        var text = e.Item.Text;
        if (string.IsNullOrEmpty(text)) return;

        text = "test1 \\033[31m test2 \\033[0m test3";
        AnsiStringHelper.DrawString(e.Graphics, text, Font, e.Bounds);

        // ʹ��Ĭ�ϵ�����ͺ�ɫ�������ֵĺ�벿��
        // var size = e.Graphics.MeasureString(text, Font);
        // e.Graphics.DrawString(text.Substring(startIndex), Font, Brushes.Red, e.Bounds.Left + size.Width, e.Bounds.Top);
    }
}