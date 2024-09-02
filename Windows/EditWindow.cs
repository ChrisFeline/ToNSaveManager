using ToNSaveManager.Localization;

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
        }

        public static EditResult Show(string content, string title, Form parent, bool showDelete = false, bool hideEmpty = false, bool handleNewLine = false) {
            IsActive = true;
            if (Instance.IsDisposed) Instance = new EditWindow();

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
    }
}
