using Newtonsoft.Json;
using System.Reflection;

namespace ToNSaveManager
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            if (!Directory.Exists(DataLocation)) Directory.CreateDirectory(DataLocation);

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            UpdateWindow.RunPostUpdateCheck(args);
            if (!StartCheckForUpdate())
                Application.Run(new MainWindow());
        }

        internal static readonly string DataLocation = Path.Combine(LogWatcher.GetVRChatDataLocation(), "ToNSaveManager");

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
            if (release == null || release.assets.Length == 0 || (!showUpToDate && release.tag_name == MainWindow.Settings.IgnoreRelease)) return false;
            GitHubRelease.Asset? asset = release.assets.FirstOrDefault(v => v.name == "ToNSaveManager.zip" && v.content_type == "application/zip" && v.state == "uploaded");
            if (asset == null) return false;

            if (Version.TryParse(release.tag_name, out Version? releaseVersion) && releaseVersion > currentVersion)
            {
                const string log_start = "[changelog]: <> (START)";
                const string log_end = "[changelog]: <> (END)";

                int start = release.body.IndexOf(log_start);
                int end = release.body.IndexOf(log_end);
                string body = string.Empty;

                if (start > -1 && end > (start + log_start.Length) && end > start)
                {
                    start += log_start.Length;
                    body = "\n\n" + release.body.Substring(start, end - start).Trim();
                }

                DialogResult result = MessageBox.Show($"A new update have been released on GitHub.\n\nWould you like to automatically download and update to the new version?" + body, "New update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    UpdateWindow updateWindow = new UpdateWindow(release, asset);
                    updateWindow.ShowDialog();
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
    }

    /// <summary>
    /// GitHub Release JSON Object
    /// </summary>
    internal class GitHubRelease
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

        internal class Asset
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