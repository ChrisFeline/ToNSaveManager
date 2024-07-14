﻿namespace ToNSaveManager
{
    public struct EditResult
    {
        public string Text;
        public bool Accept;
    }

    public partial class EditWindow : Form
    {
        static EditWindow Instance = new EditWindow();
        public static Size GetSize() => Instance.Size;

        string Content
        {
            get => textBox1.Text;
            set => textBox1.Text = value;
        }

        private EditWindow()
        {
            InitializeComponent();
        }

        public static EditResult Show(string content, string title, Form parent)
        {
            if (Instance.IsDisposed) Instance = new EditWindow();

            Instance.Text = title;
            Instance.Owner = parent;
            Instance.StartPosition = FormStartPosition.CenterParent;
            Instance.Content = content;

            DialogResult res = Instance.ShowDialog();

            return new EditResult() { Text = Instance.Content, Accept = (res == DialogResult.OK) };
        }

        // Save Button
        private void button1_Click(object sender, EventArgs e)
        {
            Content = textBox1.Text;
            DialogResult = DialogResult.OK;
        }

        // Cancel Button
        private void button2_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void EditWindow_Shown(object sender, EventArgs e)
        {
            // Focus the text input please!!!
            Instance.textBox1.Focus();
        }
    }
}
