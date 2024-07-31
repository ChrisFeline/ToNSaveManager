using System.Diagnostics;
using System.Reflection;
using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using Windows.ApplicationModel.Contacts;

namespace ToNSaveManager
{
    public partial class StatsWindow : Form {
        internal readonly static StatsData Stats = StatsData.Import();
        internal static void SetDirty() => Stats.SetDirty();
        internal static void Export() => Stats.Export();

        internal static StatsWindow? Instance { get; private set; }

        PropertyInfo[] TableProperties;

        public StatsWindow() {
            InitializeComponent();

            TableProperties = Stats.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.Name.StartsWith("_")).ToArray();
        }

        public static void Open(Form parent) {
            if (Instance == null || Instance.IsDisposed) Instance = new StatsWindow();

            if (Instance.Visible) {
                Instance.BringToFront();
                return;
            }

            Instance.StartPosition = FormStartPosition.Manual;
            Instance.Location = new Point(
                parent.Location.X + (parent.Width - Instance.Width) / 2,
                Math.Max(parent.Location.Y + (parent.Height - Instance.Height) / 2, 0)
            );
            Instance.Show(); // Don't parent
        }

        internal void LocalizeContent() {
            LANG.C(this, "STATS.TITLE");
            RightToLeft = LANG.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }

        internal void UpdateTable() {
            PropertyInfo property;

            if (TableProperties.Length != statsTable.RowCount) {
                Debug.WriteLine("Setting Row Count");
                statsTable.RowCount = TableProperties.Length;

                for (int i = 0; i < TableProperties.Length; i++) {
                    property = TableProperties[i];

                    for (int j = 0; j < statsTable.ColumnCount; j++) {
                        statsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
                        Control control = new Label() { Text = j == 0 ? NormalizeLabelText(property.Name) : "...", TextAlign = j == 0 ? ContentAlignment.BottomLeft : ContentAlignment.BottomRight, Anchor = AnchorStyles.Left | AnchorStyles.Right };
                        control.DataContext = property;
                        statsTable.Controls.Add(control, j, i);

                        control.MouseClick += Control_MouseClick;
                    }
                }
            }

            for (int i = 0; i < TableProperties.Length; i++) {
                property = TableProperties[i];

                Control? control = statsTable.GetControlFromPosition(1, i);
                if (control == null) continue;

                control.Text = property.GetValue(Stats)?.ToString();
            }
        }

        PropertyInfo? ContextField;
        private void Control_MouseClick(object? sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Right || sender == null) return;

            Control control = (Control)sender;

            if (control != null) {
                ContextField = (PropertyInfo?)control.DataContext;
                if (ContextField != null && ContextField.CanWrite) contextMenu.Show(control, e.Location);
            }
        }

        private void ctxTypeInValue_Click(object sender, EventArgs e) {
            if (ContextField == null) return;

            string value = ContextField.GetValue(Stats)?.ToString() ?? "";
            EditResult show = EditWindow.Show(value, "Type in custom value", this);

            if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result)) {
                ContextField.SetValue(Stats, result);
                Stats.SetDirty();
            }
        }

        private void StatsWindow_Load(object sender, EventArgs e) {
            LocalizeContent();
            UpdateTable();
        }

        private void StatsWindow_ResizeEnd(object sender, EventArgs e) {
            Refresh();
            Update();
        }

        public static string NormalizeLabelText(string text) {
            for (int i = 0; i < text.Length; i++) {
                if (i > 0 && char.IsUpper(text[i])) {
                    string a = text.Substring(0, i);
                    Console.WriteLine(a);
                    text = a + ' ' + text.Substring(i);
                    i++;
                } else if (i == 0 && !char.IsUpper(text[i])) {
                    text = char.ToUpperInvariant(text[i]) + text.Substring(1);
                }
            }

            return text;
        }
    }
}
