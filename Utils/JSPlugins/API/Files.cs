namespace ToNSaveManager.Utils.JSPlugins.API {
    [JSEngineAPI("FS")]
    internal class Files {
        static string[] ExecExtensions = ["ACTION", "APK", "APP", "BAT", "BIN", "CMD", "COM", "COMMAND", "CPL", "CSH", "EXE", "GADGET", "INF1", "INS", "INX", "IPA", "ISU", "JOB", "JSE", "KSH", "LNK", "MSC", "MSI", "MSP", "MST", "OSX", "OUT", "PAF", "PIF", "PRG", "PS1", "REG", "RGS", "RUN", "SCT", "SHB", "SHS", "U3P", "VB", "VBE", "VBS", "VBSCRIPT", "WORKFLOW", "WS", "WSF"];

        static string SourceDir => Path.Combine(JSEngine.scriptsPath, Path.GetDirectoryName(JSEngine.GetLastSyntaxSource()) ?? string.Empty);
        internal static string? ResolvePath(string filePath, bool toScript) {
            if (string.IsNullOrEmpty(filePath)) return null;

            string source = toScript ? Path.GetDirectoryName(JSEngine.GetLastSyntaxSource()) ?? string.Empty : string.Empty;
            string fullPath = Path.Combine(JSEngine.scriptsPath, source, filePath);

            if (!Path.GetFullPath(fullPath).StartsWith(Path.GetFullPath(JSEngine.scriptsPath))) {
                Console.Error($"Invalid path: {fullPath}\nPaths resolved outside of the 'scripts' folder are not allowed...");
                return null;
            }

            string extension = Path.GetExtension(fullPath).ToUpperInvariant();
            if (extension.Length > 0) {
                extension = extension.Substring(1);
            }
            
            if (extension.Equals("JS", StringComparison.InvariantCultureIgnoreCase)) {
                Console.Error($"Invalid path: {fullPath}\nDo not resolve paths to other scripts.");
                return null;
            } else if (Array.IndexOf(ExecExtensions, extension) > -1) {
                Console.Error($"Invalid path: {fullPath}\nDo not resolve paths to executables.");
                return null;
            }

            return fullPath;
        }

        public static string[]? GetFiles(string dirPath = "./", bool recursive = false) {
            string? fullPath = ResolvePath(dirPath, true);
            if (string.IsNullOrEmpty(fullPath)) return null;

            if (!Directory.Exists(fullPath)) {
                Console.Error($"Directory does not exist: {dirPath} ({fullPath})");
                return Array.Empty<string>();
            }

            return Directory.GetFiles(fullPath, "*.*", recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
                .Select(p => Path.GetRelativePath(SourceDir, p))
                .Where(p => !p.EndsWith(".js", StringComparison.InvariantCultureIgnoreCase)).ToArray();
        }

        public static string[]? GetDirs(string dirPath = "./") {
            string? fullPath = ResolvePath(dirPath, true);
            if (string.IsNullOrEmpty(fullPath)) return null;

            if (!Directory.Exists(fullPath)) {
                Console.Error($"Directory does not exist: {dirPath} ({fullPath})");
                return Array.Empty<string>();
            }

            return Directory.GetDirectories(fullPath).Select(p => Path.GetRelativePath(SourceDir, p)).ToArray();
        }

        public static string? Read(string filePath) {
            string? fullPath = ResolvePath(filePath, true);

            if (string.IsNullOrEmpty(fullPath) || !File.Exists(fullPath)) {
                Console.Error($"Could not find file: {fullPath}");
                return null;
            }

            return File.ReadAllText(fullPath);
        }

        public static void Write(string filePath, string value) {
            string? fullPath = ResolvePath(filePath, true);
            if (string.IsNullOrEmpty(fullPath)) return;

            File.WriteAllText(fullPath, value);
        }

        public static bool Exists(string filePath) {
            string? fullPath = ResolvePath(filePath, true);
            return !string.IsNullOrEmpty(fullPath) && (File.Exists(fullPath) || Directory.Exists(fullPath));
        }
    }
}
