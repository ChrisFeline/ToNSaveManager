using ToNSaveManager.Models;

namespace ToNSaveManager
{
    public partial class ObjectivesWindow : Form
    {
        static ObjectivesWindow? Instance;

        public ObjectivesWindow()
        {
            InitializeComponent();
        }

        public static void Open(Form parent)
        {
            if (Instance == null || Instance.IsDisposed) Instance = new ObjectivesWindow();

            if (Instance.Visible)
            {
                Instance.Close();
                return;
            }

            Instance.StartPosition = FormStartPosition.Manual;
            Instance.Location = new Point(
                parent.Location.X + (parent.Width - Instance.Width) / 2,
                parent.Location.Y + (parent.Height - Instance.Height) / 2
            );
            Instance.Show(parent);
        }

        private void ObjectivesWindow_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (Objective ob in MainWindow.SaveData.Objectives)
                listBox1.Items.Add(ob);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            ListBox listBox = (ListBox)sender;
            Objective objective = (Objective)listBox.Items[e.Index];

            using (var backColorBrush = new SolidBrush(objective.IsSeparator ? this.BackColor : listBox.BackColor))
                e.Graphics.FillRectangle(backColorBrush, e.Bounds);

            string itemText = listBox.Items[e.Index].ToString() ?? string.Empty;

            Color itemColor;

            if (objective.IsSeparator) itemColor = e.ForeColor;
            else if (e.Index == MouseDownIndex) itemColor = MouseRightClick ? Color.Cyan : Color.Red;
            else itemColor = objective.IsCompleted ? Color.Gray : e.ForeColor;

            Font itemFont = new Font(listBox.Font, objective.IsCompleted ? FontStyle.Strikeout : FontStyle.Regular);

            int maxWidth = e.Bounds.Width;
            TextRenderer.DrawText(e.Graphics, MainWindow.GetTruncatedText(itemText, itemFont, maxWidth), itemFont, e.Bounds, itemColor, TextFormatFlags.Left);
        }

        private int MouseDownIndex = -1;
        private bool MouseRightClick;
        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseRightClick = e.Button == MouseButtons.Right;
            if (MouseRightClick || e.Button == MouseButtons.Left)
            {
                MouseDownIndex = listBox1.IndexFromPoint(e.Location);
                InvalidateItem(MouseDownIndex);
            }
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index < 0 || index >= listBox1.Items.Count || index != MouseDownIndex)
            {
                index = MouseDownIndex;
                MouseDownIndex = -1;
                InvalidateItem(index);
                return;
            }

            Objective objective = (Objective)listBox1.Items[index];
            if (objective.IsSeparator) return;

            switch (e.Button)
            {
                case MouseButtons.Right:
                    MainWindow.OpenExternalLink(objective.Reference);
                    break;
                case MouseButtons.Left:
                    objective.IsCompleted = !objective.IsCompleted;
                    MainWindow.SaveData.Export(true);
                    break;

                default: break;
            }

            MouseDownIndex = -1;
            InvalidateItem(index);
        }

        private void InvalidateItem(int index)
        {
            Rectangle itemRect = listBox1.GetItemRectangle(index);
            listBox1.Invalidate(itemRect);
        }

        // Tooltips
        int PreviousTooltipIndex = -1;
        private void listBox1_MouseMove(object sender, MouseEventArgs e)
        {
            // Get the index of the item under the mouse pointer
            int index = listBox1.IndexFromPoint(e.Location);

            if (PreviousTooltipIndex != index)
            {
                PreviousTooltipIndex = index;

                if (index < 0)
                {
                    TooltipUtil.Set(listBox1, null);
                    return;
                }

                Objective objective = (Objective)listBox1.Items[index];
                TooltipUtil.Set(listBox1, objective.Description);
            }
        }
    }
}
