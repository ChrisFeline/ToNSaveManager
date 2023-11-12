using System.Diagnostics;
using ToNSaveManager.Extensions;
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
            // Tooltips
            toolTip.SetToolTip(checkPlayAudio, "Double click to select custom audio file.\nRight click to reset back to 'default.wav'");
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
            if (e.Button == MouseButtons.Right && !string.IsNullOrEmpty(MainWindow.Settings.AudioLocation))
            {
                MainWindow.Settings.AudioLocation = null;
                MainWindow.Settings.Export();
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
                    MainWindow.Settings.AudioLocation = fileDialog.FileName;
                    MainWindow.Settings.Export();
                    PostAudioLocationSet();
                }
            }
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
                switch (c)
                {
                    case GroupBox g:
                        BindControlsRecursive(g.Controls);
                        break;
                    case CheckBox b:
                        string? tag = b.Tag?.ToString();
                        if (!string.IsNullOrEmpty(tag))
                            b.BindSettings(tag);
                        break;
                    default: break;
                }
            }
        }

        private void PostAudioLocationSet()
        {
            bool hasLocation = string.IsNullOrEmpty(MainWindow.Settings.AudioLocation);
            checkPlayAudio.Text = "Play Audio (" + (hasLocation ? "default.wav" : Path.GetFileName(MainWindow.Settings.AudioLocation)) + ")";
        }
        #endregion
    }
}
