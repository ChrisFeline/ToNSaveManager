using Newtonsoft.Json;
using System.Diagnostics;
using System.Reflection;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Tab;

namespace ToNSaveManager
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            if (!StartCheckForUpdate())
                Application.Run(new MainWindow());
        }

        /// <summary>
        /// Check for updates on the GitHub repo.
        /// </summary>
        /// <param name="showUpToDate">Shows a message if there's no updates available.</param>
        internal static bool StartCheckForUpdate(bool showUpToDate = false)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version? currentVersion = assembly.GetName().Version;
            if (currentVersion == null) return false; // No current version?

            GitHubRelease? release = GitHubRelease.GetLatest();
            if (release == null || release.tag_name == MainWindow.Settings.IgnoreRelease || release.assets.Length == 0) return false;
            GitHubRelease.Asset? asset = release.assets.FirstOrDefault(v => v.name == "ToNSaveManager.zip" && v.content_type == "application/zip" && v.state == "uploaded");
            if (asset == null) return false;

            if (Version.TryParse(release.tag_name, out Version? releaseVersion) && releaseVersion > currentVersion)
            {
                int index = release.body.IndexOf("**NOTE:**");
                string body = index > 0 ? "\n\n" + release.body.Substring(0, index).Trim() : string.Empty;
                DialogResult result = MessageBox.Show($"A new update have been released on GitHub.\n\nWould you like to automatically update to the new version?" + body, "New update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    StartUpdate(asset.browser_download_url);
                    return true;
                } else if (!showUpToDate)
                {
                    MainWindow.Settings.IgnoreRelease = release.tag_name;
                    MainWindow.Settings.Export();
                }
            } else if (showUpToDate)
            {
                MessageBox.Show($"No updates are currently available.", "No updates available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return false;
        }

        /// <summary>
        /// Oversimplified, but horrible method of updating from github.
        /// </summary>
        /// <param name="browser_download_url">The asset download url.</param>
        private static void StartUpdate(string browser_download_url, string temp_file_name = "update.temp.zip")
        {
            var processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "powershell.exe";

            string[] commands = new string[]
            {
                "$Host.UI.RawUI.WindowTitle = 'Downloading Update...'",
                "$ProgressPreference = 'SilentlyContinue'",
                "Write-Host \"Terminating app process\"",
                "taskkill /IM ToNSaveManager.exe",
                "Clear-Host",
                "Write-Host \"Downloading Release Asset\"",
                $"Invoke-WebRequest -Uri \"{browser_download_url}\" -OutFile \"{temp_file_name}\"",
                "Clear-Host",
                "Write-Host \"Expanding Release Asset\"",
                "$ProgressPreference = 'Continue'",
                $"Expand-Archive -Path '{temp_file_name}' -DestinationPath './' -Force",
                "Clear-Host",
                "Write-Host \"Opening ToNSaveManager.exe\"",
                "Start-Process \"ToNSaveManager.exe\"",
                $"Remove-Item '{temp_file_name}'"
            };

            processStartInfo.Arguments = $"-NoLogo -NonInteractive -Command \"{string.Join("; ", commands)}\"";
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = false;

            using (Process.Start(processStartInfo))
            {
                Debug.WriteLine("Updating");
                Application.Exit();
            }
        }

        /// <summary>
        /// GitHub Release JSON Object
        /// </summary>
        public class GitHubRelease
        {
            private const string GitHubApiBaseUrl = "https://api.github.com";
            private const string RepoOwner = "ChrisFeline";
            private const string RepoName = "ToNSaveManager";

            public int id { get; set; }
            public string name { get; set; } = string.Empty;
            public string tag_name { get; set; } = string.Empty;
            public string body { get; set; } = string.Empty;
            public DateTime created_at { get; set; }
            public DateTime published_at { get; set; }
            public Asset[] assets { get; set; } = new Asset[0];

            public class Asset
            {
                public int id { get; set; }
                public string name { get; set; } = string.Empty;
                public string content_type { get; set; } = string.Empty;
                public string state { get; set; } = string.Empty;
                public string browser_download_url { get; set; } = string.Empty;
            }

            public static GitHubRelease? GetLatest()
            {
                try
                {
                    using (var httpClient = new HttpClient())
                    {
                        httpClient.DefaultRequestHeaders.Add("User-Agent", "ToNSaveManager");
                        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

                        string response = httpClient.GetStringAsync($"{GitHubApiBaseUrl}/repos/{RepoOwner}/{RepoName}/releases/latest").Result;
                        GitHubRelease? latestRelease = JsonConvert.DeserializeObject<GitHubRelease>(response);

                        return latestRelease;
                    }
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}