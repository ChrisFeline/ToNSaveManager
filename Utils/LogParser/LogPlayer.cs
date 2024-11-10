namespace ToNSaveManager.Utils.LogParser {
    public struct LogPlayer : IEquatable<LogPlayer> {
        public string Name;
        public string GUID;

        public bool Equals(LogPlayer other) {
            return Name.Equals(other.Name);
        }

        public override string ToString() {
            return Name;
        }

        public override int GetHashCode() {
            return Name.GetHashCode();
        }
    }
}
