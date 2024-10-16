using System.Diagnostics;
using System.Runtime.InteropServices;
using ToNSaveManager.Models;
using ICSharpCode.SharpZipLib.Zip;

namespace ToNSaveManager {
    internal static class Updater {
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        const string POST_UPDATE_ARG = "--clean-update";
        internal static void Start(GitHubRelease release, GitHubRelease.Asset asset) {
            /*
            AllocConsole();

            Console.Title = "ToNSaveManager - Updating " + release.name;

            string TempFileName = release.tag_name + ".temp.zip";
            string TempFileLocation = Path.Combine(Program.ProgramDirectory, TempFileName);

            try {
                if (File.Exists(TempFileLocation))
                    File.Delete(TempFileLocation);

                Console.Write($"Downloading '{asset.name}' . . . ");

                string downloadUrl = asset.browser_download_url;
                using (HttpClient client = new HttpClient()) {
                    using (var s = client.GetStreamAsync(downloadUrl).Result) {
                        using (var fs = new FileStream(TempFileLocation, FileMode.CreateNew)) {
                            s.CopyTo(fs);
                        }
                    }
                }

                // Move current executable
                Logger.Info("Moving from: " + Program.ProgramLocation);
                Logger.Info("Moving to: " + Program.ProgramLocationTemporary);
                File.Move(Program.ProgramLocation, Program.ProgramLocationTemporary, true);

                Console.WriteLine("Extracting update files . . .");
                Logger.Info("Extracting: " + TempFileLocation);
                FastZip zip = new FastZip();
                zip.ExtractZip(TempFileLocation, Program.ProgramDirectory, null);

                Console.WriteLine("Finishing update . . .");
                File.Delete(TempFileLocation); // .zip file cleanup

                Console.WriteLine("Update complete, restarting . . .");

                Program.ReleaseMutex(); // Release mutex so downloaded app opens properly
                                        // Start new process with --post-update
                ProcessStartInfo processInfo = new ProcessStartInfo(Program.ProgramFile, POST_UPDATE_ARG);
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
            */
        }

        const string LEGACY_POST_UPDATE_ARG = "--post-update";
        internal static void CheckPostUpdate(string[] args) {
            bool updateLegacy = Program.ContainsArg(LEGACY_POST_UPDATE_ARG);
            if (!updateLegacy && !Program.ContainsArg(POST_UPDATE_ARG)) return;
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

                if (updateLegacy) {
                    // Run legacy cleanup, old to new transition
                    Logger.Info("Updated from legacy version, running legacy cleanup...");

                    try {
                        string legacyTempFiles = Path.Combine(Program.ProgramDirectory, ".temp_files");
                        if (Directory.Exists(legacyTempFiles)) {
                            Logger.Info("Deleting legacy temp files: " + legacyTempFiles);
                            Directory.Delete(legacyTempFiles, true);
                        }
                    } catch (Exception ex) {
                        Logger.Error(ex);
                    }

                    try {
                        Logger.Info("Deleting unused legacy files.");
                        var unusedFiles = Directory.GetFiles(Program.ProgramDirectory)
                            .Where(f => f.EndsWith(".pdb") || f.EndsWith(".dll"));

                        foreach (string file in unusedFiles) {
                            try {
                                Logger.Info("Deleting unused file: " + file);
                                File.Delete(file);
                            } catch { }
                        }
                    } catch (Exception ex) {
                        Logger.Error(ex);
                    }
                }

                if (File.Exists(Program.ProgramLocationTemporary)) {
                    Logger.Info("Deleting old program files.");
                    File.Delete(Program.ProgramLocationTemporary);
                }

                Logger.Info("Post-update success.");
                // MessageBox.Show("Successfully updated to version " + Program.GetVersion(), Program.ProgramName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            } catch (Exception ex) {
                Logger.Error("Failed to run post-update.");
                Logger.Error(ex);

                MessageBox.Show("Failed to run post-update.\nPlease report this issue on the GitHub page.\n\n" + ex, "Update Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
