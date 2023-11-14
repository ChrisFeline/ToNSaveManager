using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;
using ToNSaveManager.Models;

namespace ToNSaveManager
{
    public partial class UpdateWindow : Form
    {
        private void Print(string message)
        {
            textBox1.AppendText(message + Environment.NewLine);
            textBox1.SelectionStart = textBox1.Text.Length;
            textBox1.ScrollToCaret();
        }

        string OldTempDir;
        string TempFileName;
        string TempFileLocation;

        GitHubRelease Release;
        GitHubRelease.Asset Asset;

        internal UpdateWindow(GitHubRelease release, GitHubRelease.Asset asset)
        {
            Release = release;
            Asset = asset;

            TempFileName = release.tag_name + ".temp.zip";
            TempFileLocation = Path.Combine(Program.ProgramDirectory, TempFileName);
            OldTempDir = Path.Combine(Program.ProgramDirectory, ".temp_files");

            InitializeComponent();
        }

        public static void RunPostUpdateCheck(string[] args)
        {
            int index = Array.IndexOf(args, "--post-update") + 1;
            if (index == 0 || index >= args.Length) return;
            string temp = args[index];

            try
            {
                if (Directory.Exists(temp))
                    Directory.Delete(temp, true);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to do post update.\n\n" + ex, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BackgroundWr_DoWork(object? sender, DoWorkEventArgs e)
        {
            if (sender == null) return;
            BackgroundWorker worker = (BackgroundWorker)sender;

            try
            {
                if (File.Exists(TempFileLocation))
                    File.Delete(TempFileLocation);

                worker.ReportProgress(0, $"Downloading asset '{Asset.name}'");
                string downloadUrl = Asset.browser_download_url;
                using (HttpClient client = new HttpClient())
                {
                    using (var s = client.GetStreamAsync(downloadUrl).Result)
                    {
                        using (var fs = new FileStream(TempFileLocation, FileMode.CreateNew))
                        {
                            s.CopyTo(fs);
                        }
                    }
                }

                worker.ReportProgress(50, $"Asset downloaded, decompressing...");
                string outputDir = Path.Combine(Program.ProgramDirectory, Path.GetFileNameWithoutExtension(TempFileLocation));
                ZipFile.ExtractToDirectory(TempFileLocation, outputDir, true);
                File.Delete(TempFileLocation); // .zip cleanup

                if (!Directory.Exists(OldTempDir)) Directory.CreateDirectory(OldTempDir);

                string[] files = Directory.GetFiles(outputDir);
                worker.ReportProgress(90, $"Decompressed '{files.Length}' files...");

                foreach (string file in files)
                {
                    worker.ReportProgress(95, ($"  > {file}"));
                    string fileName = Path.GetFileName(file);
                    string f = Path.Combine(Program.ProgramDirectory, fileName);
                    string n = Path.Combine(OldTempDir, fileName);
                    if (File.Exists(f))
                        File.Move(f, n, true);

                    File.Move(file, f);
                }

                worker.ReportProgress(100, ($"Cleaning up..."));
                Directory.Delete(outputDir, true);
                e.Result = null;
            }
            catch (Exception ex)
            {
                e.Result = ex;
            }
        }

        private void UpdateWindow_Load(object sender, EventArgs e)
        {
            Print($"Updating to '{Release.tag_name}'");
            BackgroundWorker worker = new BackgroundWorker()
            {
                WorkerReportsProgress = true,
            };
            worker.DoWork += BackgroundWr_DoWork;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerAsync();
        }

        private void Worker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
        {
            if (e.UserState == null) return;
            Print(" * " + e.UserState);
        }

        private void Worker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Result != null)
            {
                MessageBox.Show("Failed to download update package.\n\n" + e.Result, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
                return;
            }

            Print(string.Empty);
            Print("Running post update... please wait...");
            Program.ReleaseMutex(); // Release mutex so downloaded app opens properly
            // Start new process with --post-update
            ProcessStartInfo processInfo = new ProcessStartInfo("ToNSaveManager.exe", $"--post-update \"{OldTempDir}\"");
            Process.Start(processInfo);
            // Exit this app
            Application.Exit();
        }
    }
}
