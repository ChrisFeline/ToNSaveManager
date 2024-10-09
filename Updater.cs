using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ToNSaveManager.Models;

namespace ToNSaveManager {
    internal static class Updater {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        internal class UpdateProgressBar : IProgress<float> {
            string start = "[";
            string end = "]";
            char fill = '#';
            char back = '-';

            int top = 0;
            int left = 0;
            int length = 0;

            int full_len = 0;

            string final = "Done.";

            internal UpdateProgressBar (int length = 20) {
                this.top = Console.CursorTop;
                this.left = Console.CursorLeft;
                this.length = length;
            }

            public void Start() {
                Console.CursorVisible = false;
            }

            public void Report(float value) {
                lock (start) {
                    Console.SetCursorPosition(left, top);


                    int c = (int)Math.Round(value * length);
                    Console.Write(start);
                    Console.Write(new string(fill, c).PadRight(length, back));
                    Console.Write(end);
                    Console.Write(' ');

                    float percent = value * 100;
                    Console.Write(percent.ToString("0.00").PadLeft(6, ' '));
                    Console.Write('%');

                    full_len = Console.CursorLeft - left;
                }
            }

            public void Done() {
                Report(1);
                Console.SetCursorPosition(left, top);
                Console.WriteLine((start + final + end).PadRight(full_len));
                Console.CursorVisible = true;
            }
        }

        internal static void Start(GitHubRelease release, GitHubRelease.Asset asset) {
            AllocConsole();

            Console.Title = "ToNSaveManager - Updating " + release.name;

            string TempFileName = release.tag_name + ".temp.zip";
            string TempFileLocation = Path.Combine(Program.ProgramDirectory, TempFileName);

            try {
                if (File.Exists(TempFileLocation))
                    File.Delete(TempFileLocation);

                Console.Write($"Downloading '{asset.name}' . . . ");

                UpdateProgressBar progress = new UpdateProgressBar();
                progress.Start();
                string downloadUrl = asset.browser_download_url;

                using (HttpClient client = new HttpClient()) {
                    client.Timeout = TimeSpan.FromMinutes(5);

                    using (var file = new FileStream(TempFileLocation, FileMode.Create, FileAccess.Write, FileShare.None)) {
                        client.Download(downloadUrl, file, progress);
                    }
                }

                progress.Done();

                // Move current executable
                File.Move(Program.ProgramLocation, Program.ProgramLocationTemporary, true);

                Console.WriteLine("Extracting update files . . .");
                ZipFile.ExtractToDirectory(TempFileLocation, Program.ProgramDirectory, true);

                Console.WriteLine("Finishing update . . .");
                File.Delete(TempFileLocation); // .zip file cleanup

                Console.WriteLine("Update complete, restarting . . .");

                Program.ReleaseMutex(); // Release mutex so downloaded app opens properly
                                        // Start new process with --post-update
                ProcessStartInfo processInfo = new ProcessStartInfo("ToNSaveManager.exe", "--post-update");
                Process.Start(processInfo);
                // Exit this app
                Application.Exit();
                return;
            } catch (Exception ex) {
                Logger.Error("Automatic update failed.");
                Logger.Error(ex);


                MessageBox.Show("Automatic update has failed. Try using the file 'update.bat' instead.\nPlease report this error to on the GitHub page.\n\n" + ex, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if (File.Exists(Program.ProgramLocationTemporary)) {
                File.Move(Program.ProgramLocationTemporary, Program.ProgramLocation, true);
            }
        }

        internal static void PostUpdate(string[] args) {
            Logger.Info("Running post-update cleanup.");

            try {
                using (Process currentProcess = Process.GetCurrentProcess()) {
                    Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
                    foreach (Process process in processes) {
                        using (process) {
                            if (process.Id != currentProcess.Id) {
                                Logger.Info("Killing old running process: " + process.Id);
                                process.Kill();
                                process.WaitForExit();
                            }
                        }
                    }
                }

                if (File.Exists(Program.ProgramLocationTemporary)) {
                    Logger.Info("Deleting old program files.");
                    File.Delete(Program.ProgramLocationTemporary);
                }

                Logger.Info("Post-update success.");
                MessageBox.Show("Successfully updated to version " + Program.GetVersion(), Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                Logger.Error("Failed to run post-update.");
                Logger.Error(ex);

                MessageBox.Show("Failed to run post-update.\n\n" + ex, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region Extensions
        static void CopyTo(this Stream source, Stream destination, int bufferSize, IProgress<long>? progress = null) {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(destination);
            ArgumentOutOfRangeException.ThrowIfNegative(bufferSize);

            if (!source.CanRead)
                throw new ArgumentException("Has to be readable", nameof(source));
            if (!destination.CanWrite)
                throw new ArgumentException("Has to be writable", nameof(destination));

            var buffer = new byte[bufferSize];
            long totalBytesRead = 0;
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, buffer.Length)) != 0) {
                destination.Write(buffer, 0, bytesRead);
                totalBytesRead += bytesRead;
                progress?.Report(totalBytesRead);
            }
        }

        static void Download(this HttpClient client, string requestUri, Stream destination, IProgress<float>? progress = null) {
            // Get the http headers first to examine the content length
            using (var response = client.GetAsync(requestUri, HttpCompletionOption.ResponseHeadersRead).Result) {
                var contentLength = response.Content.Headers.ContentLength;

                using (var download = response.Content.ReadAsStream()) {

                    // Ignore progress reporting when no progress reporter was 
                    // passed or when the content length is unknown
                    if (progress == null || !contentLength.HasValue) {
                        download.CopyTo(destination);
                        return;
                    }

                    // Convert absolute progress (bytes downloaded) into relative progress (0% - 100%)
                    var relativeProgress = new Progress<long>(totalBytes => progress.Report((float)totalBytes / contentLength.Value));
                    // Use extension method to report progress while downloading
                    download.CopyTo(destination, 81920, relativeProgress);
                    progress.Report(1);
                }
            }
        }
        #endregion
    }
}
