using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models.Stats;

namespace ToNSaveManager
{
    public struct EditResult
    {
        public string Text;
        public bool Accept;
    }

    public partial class EditWindow : Form {
        internal static bool IsActive;

        internal static EditWindow Instance = new EditWindow();
        public static Size GetSize() => Instance.Size;

        bool ShowDelete { get; set; }
        bool HideEmpty { get; set; }
        bool HandleNewLine { get; set; }
        string Content {
            get => textBox1.Text;
            set => textBox1.Text = value;
        }

        private EditWindow() {
            InitializeComponent();
            textBox1.FixItemHeight();
        }

        public static EditResult Show(string content, string title, Form parent, bool showDelete = false, bool hideEmpty = false, bool handleNewLine = false, bool insertKeyTemplate = false) {
            IsActive = true;
            if (Instance.IsDisposed) Instance = new EditWindow();

            if (insertKeyTemplate) Instance.FillContextMenuItems();
            Instance.buttonInsert.Enabled = insertKeyTemplate;

            if (handleNewLine) content = content.Replace("\n", "\\n");

            Instance.ShowDelete = showDelete;
            Instance.HideEmpty = hideEmpty;
            Instance.Text = title;
            Instance.Owner = parent;
            Instance.StartPosition = FormStartPosition.CenterParent;
            Instance.Content = content;
            Instance.RightToLeft = LANG.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;

            DialogResult res = Instance.ShowDialog();
            IsActive = false;

            if (handleNewLine) Instance.Content = Instance.Content.Replace("\\n", "\n");

            return new EditResult() { Text = Instance.Content, Accept = (res == DialogResult.OK) };
        }

        // Save Button
        private void button1_Click(object sender, EventArgs e) {
            Content = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        // Cancel Button
        private void button2_Click(object sender, EventArgs e) {
            DialogResult = DialogResult.Cancel;
        }

        private void EditWindow_Shown(object sender, EventArgs e) {
            // Focus the text input please!!!
            Instance.textBox1.Focus();
            LocalizeContent();
            textBox1_TextChanged(textBox1, EventArgs.Empty);
        }

        internal void LocalizeContent() {
            button1.Text = LANG.S("EDIT.SAVE") ?? "Save";
            button2.Text = LANG.S("EDIT.CANCEL") ?? "Cancel";
        }

        private bool IsEmpty;
        private void textBox1_TextChanged(object sender, EventArgs e) {
            if (HideEmpty) button1.Visible = !string.IsNullOrEmpty(Content);

            if (ShowDelete) {
                bool isEmpty = string.IsNullOrEmpty(Content);

                if (IsEmpty != isEmpty) {
                    IsEmpty = isEmpty;

                    button1.Text = (IsEmpty ? (LANG.S("EDIT.DELETE") ?? "Delete") : (LANG.S("EDIT.SAVE") ?? "Save"));
                }
            }
        }

        private void FillContextMenuItems() {
            contextMenuStrip1.Items.Clear();

            ToolStripMenuItem[] items = ToNStats.PropertyGroups.Select(kvp => {
                var tt = new ToolStripMenuItem() {
                    Text = kvp.Key
                };

                tt.DropDownItems.AddRange(
                    kvp.Value.Select(v => {
                        var t2 = new ToolStripMenuItem();
                        (string? tx, string? tt) = LANG.T(v.KeyLang);

                        t2.Text = tx ?? v.Key;

                        if (string.IsNullOrEmpty(tt)) tt = string.Empty;
                        else tt += "\n\n";
                        tt += LANG.S("STATS.TEMPLATE_KEY", v.KeyTemplate);

                        t2.ToolTipText = tt;

                        t2.Tag = v.KeyTemplate;
                        t2.Click += TemplateItemClicked;
                        return t2;
                    }).ToArray()
                );

                return tt;
            }).ToArray();

            contextMenuStrip1.Items.AddRange(items);

            contextMenuStrip1.Items.Add("Close");
        }

        private void TemplateItemClicked(object? sender, EventArgs e) {
            if (sender == null) return;

            var _sender = (ToolStripMenuItem)sender;
            textBox1.Paste(_sender.Tag?.ToString());
        }

        private void buttonInsert_MouseClick(object sender, MouseEventArgs e) {
            // FillContextMenuItems();
            contextMenuStrip1.Show(buttonInsert, e.Location);
        }
    }
}
