using Newtonsoft.Json;
using static ToNSaveManager.Models.GitHubRelease;

namespace ToNSaveManager.Models
{
    /// <summary>
    /// GitHub Release JSON Object
    /// </summary>
    internal class GitHubRelease
    {
        private const string GitHubApiBaseUrl = "https://api.github.com";
        private const string RepoOwner = "nomlasvrc";
        private const string RepoName = "ToNSaveManager";

        public int id { get; set; }
        public string name { get; set; } = string.Empty;
        public string tag_name { get; set; } = string.Empty;
        public string body { get; set; } = string.Empty;
        public DateTime created_at { get; set; }
        public DateTime published_at { get; set; }
        public Asset[] assets { get; set; } = Array.Empty<Asset>();

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

    internal class GitHubUpdate {
        internal static GitHubRelease? Release;
        internal static Asset? Asset;

        internal static bool Update { get; set; }

        internal static void Set(GitHubRelease release, GitHubRelease.Asset asset) {
            Release = release;
            Asset = asset;
            Update = true;

            List<Form> openForms = new List<Form>();

            foreach (Form f in Application.OpenForms) {
                openForms.Add(f);
            }

            foreach (Form f in openForms) {
                f.Close();
            }
        }

        internal static void Start() {
            if (!Update || Release == null || Asset == null) return;
            Updater.Start(Release, Asset);
        }
    }
}
