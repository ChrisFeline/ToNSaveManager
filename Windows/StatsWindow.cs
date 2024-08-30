using System.Diagnostics;
using System.Reflection;
using System.Text.RegularExpressions;
using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;
using Windows.ApplicationModel.Contacts;

namespace ToNSaveManager
{
    public partial class StatsWindow : Form {
        internal readonly static StatsData Stats = StatsData.Import();
        internal readonly static StatsData Lobby = new StatsData();
        internal static StatsWindow? Instance { get; private set; }
        internal static void RefreshTable() => Instance?.UpdateTable();

        internal static void ClearLobby() {
            Lobby.Clear();
            RefreshTable();
            UpdateChatboxContent();
        }

        internal static void SetDirty() => Stats.SetDirty();
        internal static void Export() {
            if (Stats.IsDirty) UpdateChatboxContent();
            Stats.Export();
        }

        internal static void AddRound(bool survived) {
            if (MainWindow.Started) Stats.AddRound(survived);
            Lobby.AddRound(survived);
            RefreshTable();
        }
        internal static void AddStun(bool isLocal) {
            if (MainWindow.Started) Stats.AddStun(isLocal);
            Lobby.AddStun(isLocal);
            RefreshTable();
        }
        internal static void AddDamage(int damage) {
            if (MainWindow.Started) Stats.AddDamage(damage);
            Lobby.AddDamage(damage);
            RefreshTable();
        }

        internal static bool IsRoundActive;
        internal static void SetRoundActive(bool active) {
            if (IsRoundActive != active) {
                Logger.Debug("Setting Round Active: " + active);

                IsRoundActive = active;
                if (IsRoundActive) LilOSC.SetChatboxMessage(string.Empty);
                else UpdateChatboxContent();
            }
        }

        static readonly PropertyInfo[] TableProperties;
        static readonly Dictionary<string, PropertyInfo> TableDictionary;
        static StatsWindow () {
            TableProperties = Stats.GetType().GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.Public)
                .Where(p => !p.Name.StartsWith("_")).ToArray();

            TableDictionary = new Dictionary<string, PropertyInfo>();
            foreach (PropertyInfo info in TableProperties) {
                TableDictionary.Add(info.Name.ToUpperInvariant(), info);
            }
        }
        public StatsWindow() {
            InitializeComponent();
        }

        bool ShowLobbyStats;

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

            for (int i = 0; i < TableProperties.Length; i++) {
                Control? control = statsTable.GetControlFromPosition(0, i);
                if (control == null || control.DataContext == null) continue;

                PropertyInfo property = (PropertyInfo)control.DataContext;

                string key = "STATS.LABEL_" + property.Name.ToUpperInvariant();
                (string? tx, string? tt) = LANG.T(key);

                if (string.IsNullOrEmpty(tx)) control.Text = property.Name;
                else control.Text = tx;

                if (string.IsNullOrEmpty(tt)) tt = string.Empty;
                else tt += "\n\n";
                tt += LANG.S("STATS.CHATBOX_KEY_TOTAL", '{' + property.Name + '}') + '\n' +
                      LANG.S("STATS.CHATBOX_KEY_LOBBY", "{Lobby" + property.Name + '}');

                toolTip.SetToolTip(control, tt);
            }

            LANG.C(ctxTypeInValue, "STATS.CTX_TYPE_IN_VALUE");
            LANG.C(ctxCopyStatName, "STATS.CTX_COPY_CHATBOX_KEY");

            RightToLeft = LANG.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }

        internal void UpdateTable() {
            PropertyInfo property;

            if (TableProperties.Length != statsTable.RowCount) {
                statsTable.RowCount = TableProperties.Length;

                for (int i = 0; i < TableProperties.Length; i++) {
                    property = TableProperties[i];

                    for (int j = 0; j < statsTable.ColumnCount; j++) {
                        statsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
                        Control control = new Label() {
                            Text = j == 0 ? NormalizeLabelText(property.Name) : "...",
                            TextAlign = ContentAlignment.BottomLeft,
                            Anchor = AnchorStyles.Left | AnchorStyles.Right
                        };
                        control.DataContext = property;
                        control.Tag = control.Text;
                        statsTable.Controls.Add(control, j, i);

                        control.MouseClick += Control_MouseClick;
                    }
                }
            }

            for (int i = 0; i < TableProperties.Length; i++) {
                property = TableProperties[i];

                Control? control = statsTable.GetControlFromPosition(1, i);
                if (control == null) continue;

                control.Text = property.GetValue(ShowLobbyStats ? Lobby : Stats)?.ToString();
            }

            if (ShowLobbyStats) {
                LANG.C(btnSwitch, "STATS.SHOW_TOTAL", toolTip, "Show Total Stats");
            } else {
                LANG.C(btnSwitch, "STATS.SHOW_LOBBY", toolTip, "Show Lobby Stats");
            }
        }

        PropertyInfo? ContextField;
        private void Control_MouseClick(object? sender, MouseEventArgs e) {
            if (e.Button != MouseButtons.Right || sender == null) return;

            Control control = (Control)sender;

            if (control != null) {
                ContextField = (PropertyInfo?)control.DataContext;
                ctxTypeInValue.Enabled = !ShowLobbyStats && ContextField != null && ContextField.CanWrite;
                contextMenu.Show(control, e.Location);
            }
        }

        private void ctxTypeInValue_Click(object sender, EventArgs e) {
            if (ContextField == null) return;

            string value = ContextField.GetValue(Stats)?.ToString() ?? "";
            EditResult show = EditWindow.Show(value, LANG.S("STATS.CTX_TYPE_IN_VALUE.TITLE") ?? "Type in custom value", this);

            if (show.Accept && !string.IsNullOrEmpty(show.Text) && int.TryParse(show.Text.Trim(), out int result)) {
                ContextField.SetValue(Stats, result);
                Stats.SetDirty();
                UpdateTable();
            }
        }

        private void ctxCopyStatName_Click(object sender, EventArgs e) {
            if (ContextField == null) return;

            string content = ContextField.Name;
            if (ShowLobbyStats) content = "Lobby" + content;
            content = '{' + content + '}';

            Clipboard.SetDataObject(content, false, 4, 200);
            MessageBox.Show(LANG.S("STATS.CTX_COPY_CHATBOX_KEY.MESSAGE", content) ?? ("Copied to clipboard: " + content));
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

        private void btnSwitch_Click(object sender, EventArgs e) {
            ShowLobbyStats = !ShowLobbyStats;
            UpdateTable();
        }

        static readonly Regex MessageTemplatePattern = new Regex(@"{\w+}", RegexOptions.Compiled);
        const string LOBBY_PREFIX = "LOBBY";

        internal static void UpdateChatboxContent() {
            if (IsRoundActive || !MainWindow.Started || !Settings.Get.OSCSendChatbox || string.IsNullOrEmpty(Settings.Get.OSCMessageTemplate)) return;
            string template = MessageTemplatePattern.Replace(Settings.Get.OSCMessageTemplate, UpdateChatboxEvaluator);
            LilOSC.SetChatboxMessage(template);
        }

        static string UpdateChatboxEvaluator(Match m) {
            string key = m.Value.Substring(1, m.Length - 2).ToUpperInvariant();

            bool isLobby = key.StartsWith(LOBBY_PREFIX, StringComparison.OrdinalIgnoreCase);
            if (isLobby) key = key.Substring(LOBBY_PREFIX.Length);

            if (!string.IsNullOrEmpty(key) && TableDictionary.ContainsKey(key))
                return TableDictionary[key].GetValue(isLobby ? Lobby : Stats)?.ToString() ?? m.Value;

            return m.Value;
        }
    }
}
