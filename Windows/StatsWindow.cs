using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Models.Stats;
using ToNSaveManager.Utils;
using static System.Windows.Forms.AxHost;

namespace ToNSaveManager
{
    public partial class StatsWindow : Form {
        internal static StatsWindow? Instance { get; private set; }
        internal static void RefreshTable() => Instance?.UpdateTable();

        internal static void ClearLobby() {
            ToNStats.ClearLobby();
            ToNStats.ClearRound();
            RefreshTable();
            UpdateChatboxContent();
        }

        internal static void WriteChanges() {
            if (Settings.Get.OSCMessageInfoTemplate.IsModified) {
                UpdateChatboxContent();
            }

            if (Settings.Get.RoundInfoToFile) {
                foreach (var template in Settings.Get.RoundInfoTemplates) template.WriteToFile();
            }

            ToNStats.Export(false);
        }

        internal static void AddRound(bool survived) {
            ToNStats.AddRound(survived, MainWindow.Started);
            RefreshTable();
        }
        internal static void AddStun(bool isLocal) {
            ToNStats.AddStun(isLocal, MainWindow.Started);
            RefreshTable();
        }
        internal static void AddDamage(int damage) {
            ToNStats.AddDamage(damage, MainWindow.Started);
            RefreshTable();
        }

        internal static void SetDisplayName(string displayName, bool isDiscord) {
            ToNStats.SetDisplayName(displayName, isDiscord);
            RefreshTable();
        }

        internal static void SetInstanceURL(string instanceURL) {
            ToNStats.SetInstanceURL(instanceURL);
            RefreshTable();
        }

        internal static void SetTerrorMatrix(TerrorMatrix terrorMatrix) {
            ToNStats.AddTerrors(terrorMatrix);
            RefreshTable();
        }
        internal static void SetLocation(ToNIndex.Map map, bool killersSet) {
            ToNStats.AddLocation(map, killersSet);
            RefreshTable();
        }
        internal static void SetIsKiller(bool isKiller) {
            ToNStats.AddIsKiller(isKiller);
            RefreshTable();
        }
        internal static void SetPageCount(int pages) {
            ToNStats.AddPageCount(pages);
            RefreshTable();
        }
        internal static void SetPlayerCount(int playerCount) {
            ToNStats.AddPlayerCount(playerCount);
            RefreshTable();
        }
        internal static void SetIsAlive(bool isAlive) {
            ToNStats.AddIsAlive(isAlive);
            RefreshTable();

            if (!isAlive)
                SetActiveInRound(false);
        }
        internal static void SetIsStarted(bool started) {
            ToNStats.AddIsStarted(started);
            RefreshTable();

            SetActiveInRound(started);
        }

        static bool IsRoundActive;
        static void SetActiveInRound(bool active) {
            if (IsRoundActive != active) {
                IsRoundActive = active;
                if (IsRoundActive) {
                    LilOSC.SetChatboxMessage(string.Empty);
                    ToNStats.ClearRound();
                    RefreshTable();
                } else UpdateChatboxContent();
            }
        }

        static StatPropertyContainer[] TableProperties => ToNStats.PropertyValues;
        static string[,]? DisplayNames;

        static Dictionary<string, StatPropertyContainer> TableDictionary => ToNStats.PropertyDictionary;

        public StatsWindow() {
            InitializeComponent();
            listBox1.FixItemHeight();
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

            UpdateTable();

            // string key = "STATS.LABEL_" + property.KeyUpper;
            // strring tt += LANG.S("STATS.TEMPLATE_KEY", '{' + property.Key + '}');

            if (DisplayNames == null) DisplayNames = new string[TableProperties.Length, 2];

            for (int i = 0; i < TableProperties.Length; i++) {
                var property = TableProperties[i];

                (string? tx, string? tt) = LANG.T(property.KeyLang);

                DisplayNames[i, 0] = string.IsNullOrEmpty(tx) ? property.Key : tx;

                if (string.IsNullOrEmpty(tt)) tt = string.Empty;
                else tt += "\n\n";
                tt += LANG.S("STATS.TEMPLATE_KEY", property.KeyTemplate);

                DisplayNames[i, 1] = tt;
            }

            LANG.C(ctxTypeInValue, "STATS.CTX_TYPE_IN_VALUE");
            LANG.C(ctxCopyStatName, "STATS.CTX_COPY_TEMPLATE_KEY");

            RightToLeft = LANG.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }

        internal void UpdateTable() {
            if (InvokeRequired) {
                this.Invoke(new System.Windows.Forms.MethodInvoker(listBox1.Refresh));
                return;
            }
            listBox1.Refresh();
            // listBox1.Update();
        }

        StatPropertyContainer? ContextField;

        private void ctxTypeInValue_Click(object sender, EventArgs e) {
            if (ContextField == null) return;

            string value = ContextField.GetValue()?.ToString() ?? "";
            EditResult show = EditWindow.Show(value, LANG.S("STATS.CTX_TYPE_IN_VALUE.TITLE") ?? "Type in custom value", this);

            if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result)) {
                ToNStats.Set(ContextField.Key, result);
                UpdateTable();
            }
        }

        private void ctxCopyStatName_Click(object sender, EventArgs e) {
            if (ContextField == null) return;

            string content = '{' + ContextField.Key + '}';
            Clipboard.SetDataObject(content, false, 4, 200);
            MessageBox.Show(LANG.S("STATS.CTX_COPY_TEMPLATE_KEY.MESSAGE", content) ?? ("Copied to clipboard: " + content));
        }

        private void StatsWindow_Load(object sender, EventArgs e) {
            listBox1.DataSource = TableProperties;
            LocalizeContent();
            UpdateTable();
        }

        private void StatsWindow_ResizeEnd(object sender, EventArgs e) {
            Refresh();
            Update();
        }

        /*
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
        */

        internal static void UpdateChatboxContent() {
            if (IsRoundActive || !MainWindow.Started || !Settings.Get.OSCSendChatbox || string.IsNullOrEmpty(Settings.Get.OSCMessageInfoTemplate.Template)) return;
            string template = TemplateManager.ReplaceTemplate(Settings.Get.OSCMessageInfoTemplate.Template);
            LilOSC.SetChatboxMessage(template);
        }

        private void listBoxEntries_DrawItem(object sender, DrawItemEventArgs e) {
            if (e.Index < 0)
                return;

            using (Brush brush = (e.Index % 2 == 0 ? new SolidBrush(e.BackColor) : new SolidBrush(Color.FromArgb(e.BackColor.R / 2, e.BackColor.G / 2, e.BackColor.B / 2)))) {
                e.Graphics.FillRectangle(brush, e.Bounds);
            }

            ListBox listBox = (ListBox)sender;

            var item = (StatPropertyContainer)listBox.Items[e.Index];
            string itemText = DisplayNames == null ? item.Key : DisplayNames[e.Index, 0];

            int maxWidth = e.Bounds.Width;
            TextRenderer.DrawText(e.Graphics, itemText, listBox.Font, e.Bounds, e.ForeColor, TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);
            TextRenderer.DrawText(e.Graphics, item.Value?.ToString() ?? "NULL", listBox.Font, e.Bounds, e.ForeColor, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPrefix);

            e.DrawFocusRectangle();
        }

        // Tooltips
        int PreviousTooltipIndex = -1;
        private void listBoxEntries_MouseMove(object sender, MouseEventArgs e) {
            // Get the index of the item under the mouse pointer
            int index = listBox1.IndexFromPoint(e.Location);

            if (PreviousTooltipIndex != index) {
                PreviousTooltipIndex = index;

                if (index < 0) {
                    toolTip.SetToolTip(listBox1, null);
                    return;
                }

                toolTip.SetToolTip(listBox1, DisplayNames == null ? null : DisplayNames[index, 1]);
            }
        }

        private void listBoxEntries_SelectedValueChanged(object sender, EventArgs e) {
            if (listBox1.SelectedIndex < 0) return;
            listBox1.SelectedIndex = -1;
        }

        private void listBoxEntries_MouseClick(object sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Right || sender == null) return;

            int index = listBox1.IndexFromPoint(e.Location);
            if (index < 0 || index >= listBox1.Items.Count) return;

            ContextField = TableProperties[index];
            ctxTypeInValue.Enabled = ContextField != null && ContextField.CanWrite;
            contextMenu.Show(listBox1, e.Location);
        }
    }
}
