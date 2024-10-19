using System;
using System.Diagnostics;
using System.Drawing.Text;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;
using ToNSaveManager.Utils.API;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.LogParser;

namespace ToNSaveManager
{
    internal static class Program
    {
        internal const string ProgramName = "ToNSaveManager";

        internal static readonly string ProgramDirectory = AppContext.BaseDirectory ?? string.Empty;
        internal static readonly string ProgramLocation = Path.Combine(ProgramDirectory, ProgramFile);
        internal static readonly string ProgramLocationTemporary = Path.Combine(ProgramDirectory, "_" + ProgramFile.ToLowerInvariant() + ".old");
        internal const string ProgramFile = ProgramName + ".exe";

        internal static readonly string DataLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ProgramName);
        internal static readonly string LegacyDataLocation = Path.Combine(LogWatcher<ToNLogContext>.GetVRChatDataLocation(), ProgramName);

        internal static Mutex? AppMutex = new Mutex(true, ProgramName);
        internal static void ReleaseMutex() {
            if (AppMutex != null) {
                AppMutex.ReleaseMutex();
                AppMutex.Dispose();
                AppMutex = null;
            }
        }
        internal static bool CheckMutex()
        {
            return AppMutex != null && !AppMutex.WaitOne(TimeSpan.Zero, true);
        }

        static string[] Arguments = Array.Empty<string>();
        internal static bool ContainsArg(string arg) {
            return Array.IndexOf(Arguments, arg) != -1;
        }

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Arguments = args;
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;
            CultureInfo.DefaultThreadCurrentUICulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;

            if (!Directory.Exists(DataLocation)) Directory.CreateDirectory(DataLocation);

            LANG.Initialize();
            Updater.CheckPostUpdate(args);

            if (CheckMutex())
            {
                // Don't run program if it's already running, instead we focus the already existing window
                NativeMethods.PostMessage((IntPtr)NativeMethods.HWND_BROADCAST, NativeMethods.WM_FOCUSINST, IntPtr.Zero, IntPtr.Zero);
                return;
            }

            ApplicationConfiguration.Initialize();
            Application.SetCompatibleTextRenderingDefault(true);
            InitializeFont();

            MainWindow.Started = false;

            if (!StartCheckForUpdate()) {
                Application.ApplicationExit += OnApplicationExit;
                Application.Run(new MainWindow());
            }

            // Check when all forms close
            Logger.Info("All windows are closed...");
            GitHubUpdate.Start();
        }

        private static void OnApplicationExit(object? sender, EventArgs e) {
            Logger.Log("Disposing used resources...");
            FontCollection.Dispose();
            DefaultFont?.Dispose();
            ReleaseMutex();
            Logger.Log("Saving before app exit...");
            MainWindow.SaveData.Export();
            StatsWindow.WriteChanges();
            Logger.Log("Done.");
        }

        static readonly PrivateFontCollection FontCollection = new PrivateFontCollection();
        static Font? DefaultFont;

        static void InitializeFont()
        {
            using (Stream? fontStream = GetEmbededResource("FiraCode.ttf"))
            {
                if (fontStream != null) try
                    {
                        IntPtr data = Marshal.AllocCoTaskMem((int)fontStream.Length);
                        byte[] fontdata = new byte[fontStream.Length];
                        fontStream.Read(fontdata, 0, (int)fontStream.Length);
                        Marshal.Copy(fontdata, 0, data, (int)fontStream.Length);
                        uint cFonts = 0;
                        NativeMethods.AddFontMemResourceEx(data, (uint)fontdata.Length, IntPtr.Zero, ref cFonts);
                        FontCollection.AddMemoryFont(data, (int)fontStream.Length);
                        fontStream.Close();
                        Marshal.FreeCoTaskMem(data);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error(ex.ToString());
                    }
            }

            DefaultFont = new Font(FontCollection.Families[0], 8.999999f, GraphicsUnit.Point);
            Application.SetDefaultFont(DefaultFont);
        }

        internal static Stream? GetEmbededResource(string name)
        {
            return Assembly.GetExecutingAssembly()
                .GetManifestResourceStream($"ToNSaveManager.Resources.{name}");
        }

        internal static Version? GetVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            return assembly.GetName().Version;
        }

        /// <summary>
        /// Check for updates on the GitHub repo.
        /// </summary>
        /// <param name="showUpToDate">Shows a message if there's no updates available.</param>
        internal static bool StartCheckForUpdate(bool showUpToDate = false)
        {
            Version? currentVersion = GetVersion();
            if (currentVersion == null) return false; // No current version?

            GitHubRelease? release = GitHubRelease.GetLatest();
            if (release == null || release.assets.Length == 0 || (!showUpToDate && release.tag_name == Settings.Get.IgnoreRelease)) return false;
            GitHubRelease.Asset? asset = release.assets.FirstOrDefault(v => v.name == "ToNSaveManager.zip" && v.content_type == "application/zip" && v.state == "uploaded");
            if (asset == null) return false;

            if (Version.TryParse(release.tag_name, out Version? releaseVersion) && releaseVersion > currentVersion)
            {
                const string log_start = "[changelog]: <> (START)";
                const string log_end = "[changelog]: <> (END)";

                int start = release.body.IndexOf(log_start, StringComparison.InvariantCulture);
                int end = release.body.IndexOf(log_end, StringComparison.InvariantCulture);
                string body = string.Empty;

                if (start > -1 && end > (start + log_start.Length) && end > start)
                {
                    start += log_start.Length;
                    body = "\n\n" + release.body.Substring(start, end - start).Trim();
                }

                DialogResult result = MessageBox.Show((LANG.S("MESSAGE.UPDATE_AVAILABLE") ?? "A new update have been released on GitHub.\n\nWould you like to automatically download and update to the new version?") + body, LANG.S("MESSAGE.UPDATE_AVAILABLE.TITLE") ?? "New update available", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
                if (result == DialogResult.Yes)
                {
                    GitHubUpdate.Set(release, asset);
                    return true;
                } else if (!showUpToDate)
                {
                    Settings.Get.IgnoreRelease = release.tag_name;
                    Settings.Export();
                }
            } else if (showUpToDate)
            {
                MessageBox.Show(LANG.S("MESSAGE.UPDATE_UNAVAILABLE") ?? "No updates are currently available.", LANG.S("MESSAGE.UPDATE_UNAVAILABLE.TITLE") ?? "No updates available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            return false;
        }

        internal static bool CreateFileBackup(string filePath)
        {
            Logger.Info("Creating Backup For: " + filePath);

            try
            {
                if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
                    File.Copy(filePath, filePath + ".backup_" + DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                return true;
            }
            catch
            {
                return false;
            }
        }
    }

    public partial class MainWindow : Form
    {
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == NativeMethods.WM_FOCUSINST) FocusInstance();
            base.WndProc(ref m);
        }

        private void FocusInstance()
        {
            if (WindowState == FormWindowState.Minimized)
                WindowState = FormWindowState.Normal;

            NativeMethods.SetForegroundWindow(this.Handle);
        }
    }

    internal class NativeMethods
    {
        public const int HWND_BROADCAST = 0xffff;
        public static readonly int WM_FOCUSINST = RegisterWindowMessage("WM_FOCUSINST");
        [DllImport("user32")]
        public static extern bool PostMessage(IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam);
        [DllImport("user32")]
        public static extern int RegisterWindowMessage(string message);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("gdi32.dll")]
        public static extern IntPtr AddFontMemResourceEx(IntPtr pbFont, uint cbFont, IntPtr pdv, [In] ref uint pcFonts);
    }
}