using ToNSaveManager.Extensions;
using ToNSaveManager.Models;

namespace ToNSaveManager
{
    public partial class ObjectivesWindow : Form
    {
        static ObjectivesWindow? Instance;
        static List<Objective> Objectives = Objective.ImportFromMemory();

        public ObjectivesWindow()
        {
            InitializeComponent();
            listBox1.FixItemHeight();
        }

        public static void Open(Form parent)
        {
            if (Instance == null || Instance.IsDisposed) Instance = new ObjectivesWindow();

            if (Instance.Visible)
            {
                Instance.BringToFront();
                return;
            }

            Instance.StartPosition = FormStartPosition.Manual;
            Instance.Location = new Point(
                parent.Location.X + (parent.Width - Instance.Width) / 2,
                parent.Location.Y + (parent.Height - Instance.Height) / 2
            );
            Instance.Show(); // Don't parent
        }

        internal static void RefreshLists()
        {
            Instance?.listBox1.Refresh();
        }

        private void ObjectivesWindow_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            foreach (Objective ob in Objectives)
                listBox1.Items.Add(ob);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            if (e.Index < 0)
                return;

            ListBox listBox = (ListBox)sender;
            Objective objective = (Objective)listBox.Items[e.Index];

            string itemText = listBox.Items[e.Index].ToString() ?? string.Empty;
            Font itemFont = new Font(listBox.Font, objective.IsCompleted ? FontStyle.Strikeout : FontStyle.Regular);

            using (var backColorBrush = new SolidBrush(objective.IsSeparator ? this.BackColor : listBox.BackColor))
                e.Graphics.FillRectangle(backColorBrush, e.Bounds);

            Color itemColor;

            if (objective.IsSeparator) itemColor = e.ForeColor;
            else if (e.Index == MouseDownIndex) itemColor = MouseRightClick ? Color.Cyan : Color.Red;
            else itemColor = objective.IsCompleted ? Color.FromArgb(122, 122, 122) : (Settings.Get.ColorfulObjectives ? objective.DisplayColor : e.ForeColor);

            int maxWidth = e.Bounds.Width;
            TextRenderer.DrawText(e.Graphics, MainWindow.GetTruncatedText(itemText, itemFont, maxWidth), itemFont, e.Bounds, itemColor,
                TextFormatFlags.VerticalCenter);
        }

        private int MouseDownIndex = -1;
        private bool MouseRightClick;
        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            MouseRightClick = e.Button == MouseButtons.Right;
            if (MouseRightClick || e.Button == MouseButtons.Left)
            {
                MouseDownIndex = listBox1.IndexFromPoint(e.Location);
                if (MouseDownIndex > 0) InvalidateItem(MouseDownIndex);
                else MouseDownIndex = -1;
            }
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            int index = listBox1.IndexFromPoint(e.Location);
            if (index < 0 || index >= listBox1.Items.Count || index != MouseDownIndex)
            {
                index = MouseDownIndex;
                MouseDownIndex = -1;
                if (index > -1) InvalidateItem(index);
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
                    objective.Toggle();
                    // MainWindow.SaveData.Export();
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
