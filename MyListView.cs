namespace ReplaceAllGit;

public class MyListView : ListView
{
    public MyListView()
    {
        // ���� OwnerDraw���������ǿ����Լ�������
        this.OwnerDraw = true;

        // ���һ����
        this.Items.Add("This is some text. This part is red.");

        this.DrawItem += this.OnDrawItem;
    }

    private void OnDrawItem(object sender, DrawListViewItemEventArgs e)
    {
        // ��ȡҪ���Ƶ�����
        string text = e.Item.Text;

        // �ҵ���ɫ���ֵĿ�ʼ�ͽ���λ��
        int startIndex = text.IndexOf("This part is red");
        int endIndex = startIndex + "This part is red".Length;

        // ʹ��Ĭ�ϵ��������ɫ�������ֵ�ǰ�벿��
        var coloredText = text.Substring(0, startIndex);
        e.Graphics.DrawString(coloredText, this.Font, Brushes.Black, e.Bounds);

        // ʹ��Ĭ�ϵ�����ͺ�ɫ�������ֵĺ�벿��
        SizeF size = e.Graphics.MeasureString(text, this.Font);
        e.Graphics.DrawString(text.Substring(startIndex), this.Font, Brushes.Red, e.Bounds.Left + size.Width, e.Bounds.Top);
    }
}