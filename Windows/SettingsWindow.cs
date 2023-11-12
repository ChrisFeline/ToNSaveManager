using System.Diagnostics;
using ToNSaveManager.Extensions;
using ToNSaveManager.Models;
using Timer = System.Windows.Forms.Timer;

namespace ToNSaveManager.Windows
{
    public partial class SettingsWindow : Form
    {
        #region Initialization
        static SettingsWindow? Instance;

        readonly Timer ClickTimer = new Timer() { Interval = 200 };
        readonly Stopwatch Stopwatch = new Stopwatch();

        public SettingsWindow()
        {
            InitializeComponent();
            ClickTimer.Tick += ClickTimer_Tick;
        }

        public static void Open(Form parent)
        {
            if (Instance == null || Instance.IsDisposed) Instance = new();

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
            Instance.Show();
        }
        #endregion

        #region Form Events
        // Subscribe to events on load
        private void SettingsWindow_Load(object sender, EventArgs e)
        {
            BindControlsRecursive(Controls);
            // Custom audio handling
            PostAudioLocationSet();
            checkPlayAudio.CheckedChanged += CheckPlayAudio_CheckedChanged;
            checkXSOverlay.CheckedChanged += CheckXSOverlay_CheckedChanged;
            // Refresh lists when time format changes
            check24Hour.CheckedChanged += TimeFormat_CheckedChanged;
            checkInvertMD.CheckedChanged += TimeFormat_CheckedChanged;
            checkShowSeconds.CheckedChanged += TimeFormat_CheckedChanged;
            // Refresh list when style is changed
            checkColorObjectives.CheckedChanged += CheckColorObjectives_CheckedChanged;

            // Tooltips
            toolTip.SetToolTip(btnCheckForUpdates, "Current Version: " + Program.GetVersion());
        }

        private void SettingsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            ClickTimer.Dispose();

            MainWindow.RefreshLists();
            MainWindow.ResetNotification();
        }

        private void TimeFormat_CheckedChanged(object? sender, EventArgs e) => MainWindow.RefreshLists();
        private void CheckXSOverlay_CheckedChanged(object? sender, EventArgs e) => MainWindow.SendXSNotification(true);
        private void CheckPlayAudio_CheckedChanged(object? sender, EventArgs e)
        {
            MainWindow.PlayNotification();
            if (!checkPlayAudio.Checked) MainWindow.ResetNotification();
        }

        private void btnCheckForUpdates_Click(object sender, EventArgs e)
        {
            if (Program.StartCheckForUpdate(true)) this.Close();
        }

        private void btnOpenData_Click(object sender, EventArgs e)
        {
            MainWindow.OpenExternalLink(Program.DataLocation);
        }

        private void checkPlayAudio_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Left) return;
            Stopwatch.Start();
            CancelNext = false;
        }

        private void checkPlayAudio_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && !string.IsNullOrEmpty(Settings.Get.AudioLocation))
            {
                Settings.Get.AudioLocation = null;
                Settings.Export();
                PostAudioLocationSet();
                return;
            }

            if (e.Button != MouseButtons.Left) return;

            Stopwatch.Stop();
            long elapsed = Stopwatch.ElapsedMilliseconds;
            Stopwatch.Reset();

            if (CancelNext)
            {
                CancelNext = false;
                return;
            }

            if (elapsed > 210)
            {
                TogglePlayAudio();
                return;
            }

            if (!DoubleClickCheck)
            {
                DoubleClickCheck = true;
                ClickTimer.Stop();
                ClickTimer.Start();
                return;
            }

            DoubleClickCheck = false;
            ClickTimer.Stop();

            using (OpenFileDialog fileDialog = new OpenFileDialog())
            {
                fileDialog.InitialDirectory = "./";
                fileDialog.Title = "Select Custom Sound";
                fileDialog.Filter = "Waveform Audio (*.wav)|*.wav";

                if (fileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(fileDialog.FileName))
                {
                    Settings.Get.AudioLocation = fileDialog.FileName;
                    Settings.Export();
                    PostAudioLocationSet();
                }
            }
        }

        private void CheckColorObjectives_CheckedChanged(object? sender, EventArgs e)
        {
            ObjectivesWindow.RefreshLists();
        }

        // Double click
        private bool DoubleClickCheck = false;
        private bool CancelNext = false;
        private void ClickTimer_Tick(object? sender, EventArgs e)
        {
            ClickTimer.Stop();
            if (DoubleClickCheck)
            {
                DoubleClickCheck = false;
                CancelNext = true;
                TogglePlayAudio();
            }
        }
        #endregion

        #region Utils
        private void TogglePlayAudio()
        {
            checkPlayAudio.Checked = !checkPlayAudio.Checked;
        }

        private void BindControlsRecursive(Control.ControlCollection controls)
        {
            foreach (Control c in controls)
            {
                string? tag = c.Tag?.ToString();
                if (!string.IsNullOrEmpty(tag))
                {
                    int index = tag.IndexOf('|', StringComparison.InvariantCulture);
                    if (index > -1)
                    {
                        string tooltip = tag.Substring(index + 1).Replace("\\n", Environment.NewLine, StringComparison.Ordinal);
                        tag = tag.Substring(0, index);
                        toolTip.SetToolTip(c, tooltip);
                    }
                }

                switch (c)
                {
                    case GroupBox g:
                        BindControlsRecursive(g.Controls);
                        break;
                    case CheckBox b:
                        if (!string.IsNullOrEmpty(tag))
                            b.BindSettings(tag);
                        break;
                    default: break;
                }
            }
        }

        private void PostAudioLocationSet()
        {
            bool hasLocation = string.IsNullOrEmpty(Settings.Get.AudioLocation);
            checkPlayAudio.Text = "Play Audio (" + (hasLocation ? "default.wav" : Path.GetFileName(Settings.Get.AudioLocation)) + ")";
        }
        #endregion
    }
}
