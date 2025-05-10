using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Models.Stats;
using ToNSaveManager.Utils.API;

namespace ToNSaveManager.Utils.JSPlugins.API {
    [JSEngineAPI("WS")]
    internal static class WS {
        private static string Source => JSEngine.GetLastSyntaxSource();

        public static bool Enabled => Settings.Get.WebSocketEnabled;
        public static void SendEvent(string name, object? value = null) {
            if (!string.IsNullOrEmpty(name))
                WebSocketAPI.EventCustom.Send(Source, name, value);
        }
    }

    [JSEngineAPI("TON")]
    internal static class API {
        #region ToN Stats Data
        public static StatsData Stats => ToNStats.Local;
        public static StatsLobby StatsLobby => ToNStats.Lobby;
        public static StatsRound StatsRound => ToNStats.Round;
        #endregion

        // General Game State
        #region ToNGameState
        public static bool IsEmulated => ToNGameState.IsEmulated;
        public static bool IsAlive => ToNGameState.IsAlive;
        public static bool IsReborn => ToNGameState.IsReborn;
        public static bool IsRoundActive => ToNGameState.IsRoundActive;
        public static bool IsSaboteour => ToNGameState.IsSaboteour;
        public static bool IsOptedIn => ToNGameState.IsOptedIn;
        public static int PageCount => ToNGameState.PageCount;

        public static ToNRoundType RoundType => ToNGameState.RoundType;
        public static Color RoundColor => ToNGameState.Terrors.RoundColor;
        public static ToNIndex.Map Location => ToNGameState.Location;
        public static ToNIndex.Item Item => ToNGameState.Item;

        public static ToNIndex.TerrorInfo[] Terrors => ToNGameState.Terrors.Terrors;
        public static Color DisplayColor => ToNGameState.Terrors.DisplayColor;

        // Instance Information
        public static int PlayerCount => ToNGameState.PlayerCount;
        public static string DisplayName => ToNGameState.DisplayName;
        public static string DiscordName => ToNGameState.DiscordName;
        public static string InstanceURL => ToNGameState.InstanceURL;
        #endregion
    }
}
