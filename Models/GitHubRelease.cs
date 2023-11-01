using Newtonsoft.Json;

namespace ToNSaveManager.Models
{
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
