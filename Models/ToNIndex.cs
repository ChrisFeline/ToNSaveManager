namespace ToNSaveManager.Models
{
    internal class ToNIndex
    {
        public Dictionary<int, string> Terrors = new Dictionary<int, string>();
        public Dictionary<int, string> Alternates = new Dictionary<int, string>();

        public static ToNIndex Import()
        {
            using (Stream? stream = Program.GetEmbededResource("index.json"))
            {
                if (stream == null) return new ToNIndex();

                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<ToNIndex>(json) ?? new ToNIndex();
                }
            }
        }

        public string? this[int index, bool alternate]
            => alternate ? GetAlternate(index) : GetTerror(index);

        public string? GetTerror(int index)
        {
            return Terrors.ContainsKey(index) ? Terrors[index] : null;
        }
        public string? GetAlternate(int index)
        {
            return Alternates.ContainsKey(index) ? Alternates[index] : null;
        }
    }
}
