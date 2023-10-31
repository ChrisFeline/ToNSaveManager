using System.ComponentModel;
using System.Diagnostics;
using System.IO.Compression;

namespace ToNSaveManager
{
    public partial class UpdateWindow : Form
    {
        private void Print(string message) => textBox1.Text += message + Environment.NewLine;

        string TempFileName;
        GitHubRelease Release;
        GitHubRelease.Asset Asset;

        internal UpdateWindow(GitHubRelease release, GitHubRelease.Asset asset)
        {
            Release = release;
            Asset = asset;
            TempFileName = release.tag_name + ".temp.zip";
            InitializeComponent();
        }

        public static void RunPostUpdate(string[] args)
        {
            int index = Array.IndexOf(args, "--post-update") + 1;
            if (index == 0 || index >= args.Length) return;
            string temp = args[index];

            try
            {
                if (Directory.Exists(temp))
                    Directory.Delete(temp, true);
            } catch (Exception ex)
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
                if (File.Exists(TempFileName))
                    File.Delete(TempFileName);

                worker.ReportProgress(0, $"Downloading asset '{Asset.name}'");
                string downloadUrl = Asset.browser_download_url;
                using (HttpClient client = new HttpClient())
                {
                    using (var s = client.GetStreamAsync(downloadUrl).Result)
                    {
                        using (var fs = new FileStream(TempFileName, FileMode.CreateNew))
                        {
                            s.CopyTo(fs);
                        }
                    }
                }

                worker.ReportProgress(50, $"Asset downloaded, decompressing...");
                string outputDir = Path.GetFileNameWithoutExtension(TempFileName);
                ZipFile.ExtractToDirectory(TempFileName, outputDir, true);
                File.Delete(TempFileName); // .zip cleanup

                const string tempDir = ".temp";
                if (!Directory.Exists(tempDir)) Directory.CreateDirectory(tempDir);

                string[] files = Directory.GetFiles(outputDir);
                worker.ReportProgress(90, $"Decompressed '{files.Length}' files...");

                foreach (string file in files)
                {
                    worker.ReportProgress(95, ($"  > {file}"));
                    string f = Path.GetFileName(file);
                    if (File.Exists(f))
                        File.Move(f, Path.Combine(tempDir, f), true);

                    File.Move(file, f);
                }

                worker.ReportProgress(100, ($"Cleaning up."));
                Directory.Delete(outputDir, true);

                ProcessStartInfo processInfo = new ProcessStartInfo("ToNSaveManager.exe", $"--post-update \"{tempDir}\"");
                Process.Start(processInfo);

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

            Application.Exit();
        }
    }
}
