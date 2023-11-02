using System.Diagnostics;
using System.Drawing.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using ToNSaveManager.Models;
using ToNSaveManager.Utils;

namespace ToNSaveManager
{
    internal static class Program
    {
        internal static readonly string DataLocation = Path.Combine(LogWatcher.GetVRChatDataLocation(), "ToNSaveManager");

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
            Application.SetCompatibleTextRenderingDefault(true);
            InitializeFont();

            Application.ApplicationExit += delegate {
                Debug.WriteLine("Disposing on exit");
                FontCollection.Dispose();
                DefaultFont?.Dispose();
            };

            UpdateWindow.RunPostUpdateCheck(args);
            if (!StartCheckForUpdate())
                Application.Run(new MainWindow());
        }

        static readonly PrivateFontCollection FontCollection = new PrivateFontCollection();
        static Font? DefaultFont;
        static void InitializeFont()
        {
            using (Stream? fontStream = GetEmbededResource("FiraCode.ttf"))
            {
                if (fontStream != null)
                {
                    Debug.WriteLine("Reading default font stream.");

                    byte[] fontBytes = new byte[fontStream.Length];
                    fontStream.Read(fontBytes, 0, (int)fontStream.Length);
                    IntPtr fontPtr = Marshal.AllocCoTaskMem(fontBytes.Length);
                    Marshal.Copy(fontBytes, 0, fontPtr, fontBytes.Length);
                    FontCollection.AddMemoryFont(fontPtr, fontBytes.Length);
                    Marshal.FreeCoTaskMem(fontPtr);
                }
            }

            Debug.WriteLine("Applying default font.");
            DefaultFont = new Font(FontCollection.Families[0], 9f);
            Application.SetDefaultFont(DefaultFont);
        }

        internal static Stream? GetEmbededResource(string name)
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"ToNSaveManager.Resources.{name}");
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
}