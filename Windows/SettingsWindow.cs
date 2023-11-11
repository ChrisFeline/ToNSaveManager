using ToNSaveManager.Extensions;

namespace ToNSaveManager.Windows
{
    public partial class SettingsWindow : Form
    {
        static SettingsWindow? Instance;

        public SettingsWindow()
        {
            InitializeComponent();
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
            toolTip.SetToolTip(checkPlayAudio, "Right click for custom audio file.");
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

        private void SettingsWindow_FormClosed(object sender, FormClosedEventArgs e)
        {
            MainWindow.RefreshLists();
            MainWindow.ResetNotification();
        }

        private void checkPlayAudio_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button != MouseButtons.Right) return;

            if (!string.IsNullOrEmpty(MainWindow.Settings.AudioLocation))
            {
                var result = MessageBox.Show($"Reset audio file back to 'default.wav'?\n\nCurrent Audio File:\n{MainWindow.Settings.AudioLocation}", "Clear Custom Sound", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (result == DialogResult.Yes)
                {
                    MainWindow.Settings.AudioLocation = null;
                    MainWindow.Settings.Export();
                    PostAudioLocationSet();
                }

                return;
            }

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
    }
}
