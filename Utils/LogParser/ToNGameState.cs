using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Utils.API;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.OpenRGB;

namespace ToNSaveManager.Utils {
    internal static class ToNGameState {
        public static bool IsEmulated { get; private set; }
        public static bool IsAlive { get; private set; } = true;
        public static bool IsReborn { get; private set; } = false;
        public static bool IsRoundActive { get; private set; } = false;
        public static bool IsSaboteour { get; private set; }
        public static bool IsOptedIn { get; private set; }
        public static int PageCount { get; private set; }

        public static ToNRoundType RoundType { get; private set; } = ToNRoundType.Intermission;
        public static TerrorMatrix Terrors { get; private set; } = TerrorMatrix.Empty;
        public static ToNIndex.Map Location { get; private set; } = ToNIndex.Map.Empty;
        public static ToNIndex.Item Item { get; private set; } = ToNIndex.Item.Empty;

        // Instance Information
        public static int PlayerCount { get; private set; }
        public static string DisplayName { get; private set; } = "Unknown";
        public static string DiscordName { get; private set; } = "Unknown";
        public static string InstanceURL { get; private set; } = string.Empty;

        public static void ClearStates() {
            WebSocketAPI.ClearBuffer();
            SetEmulated(false);
            SetPlayerCount(0);
            SetAlive(true);
            SetRoundActive(false);
            SetOptedIn(false);
            SetKiller(false);
            SetTerrorMatrix(TerrorMatrix.Empty);
            SetLocation(ToNIndex.Map.Empty);
            SetItem(ToNIndex.Item.Empty);
            SetRoundType(ToNRoundType.Intermission);
            SetPageCount(0);
        }

        public static void SetDisplayName(string displayName, bool isDiscord) {
            if (isDiscord) DiscordName = displayName;
            else DisplayName = displayName;

            StatsWindow.SetDisplayName(displayName, isDiscord);
        }

        public static void SetInstanceURL(string instanceURL) {
            InstanceURL = instanceURL;
            StatsWindow.SetInstanceURL(instanceURL);
        }

        public static void SetPlayerCount(int playerCount) {
            PlayerCount = playerCount;
            StatsWindow.SetPlayerCount(PlayerCount);
            DSRichPresence.SetPlayerCount(PlayerCount);
        }

        public static void SetEmulated(bool isEmulated) {
            IsEmulated = isEmulated;
            DSRichPresence.SetDirty();
        }

        public static void SetRoundActive(bool roundActive) {
            IsRoundActive = roundActive;

            StatsWindow.SetIsStarted(roundActive);
            SetPageCount(0);

            LilOSC.SetDirty();
            DSRichPresence.SetDirty();

            WebSocketAPI.SendValue("ROUND_ACTIVE", roundActive);
        }

        public static void SetAlive(bool isAlive) {
            IsAlive = isAlive;

            StatsWindow.SetIsAlive(isAlive);
            OpenRGBControl.SetIsAlive();
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();

            WebSocketAPI.SendValue("ALIVE", IsAlive);
        }

        public static void SetReborn(bool reborn) {
            IsReborn = reborn;

            StatsWindow.SetIsReborn(reborn);
            LilOSC.SetDirty();

            WebSocketAPI.SendValue("REBORN", reborn);
        }

        public static void SetOptedIn(bool isOptedIn) {
            IsOptedIn = isOptedIn;

            StatsWindow.SetOptedIn(IsOptedIn);
            LilOSC.SetDirty();

            WebSocketAPI.SendValue("OPTED_IN", IsOptedIn);
        }

        public static void SetKiller(bool isKiller) {
            IsSaboteour = isKiller;
            StatsWindow.SetIsKiller(isKiller);

            WebSocketAPI.SendValue("IS_SABOTEUR", IsAlive);
        }

        public static void SetTerrorMatrix(TerrorMatrix matrix) {
            Terrors = matrix;
            SetRoundType(Terrors.RoundType);

            StatsWindow.SetTerrorMatrix(Terrors);
            OpenRGBControl.SetTerrorMatrix(Terrors);
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();

            WebSocketAPI.EventTerror.Send(Terrors);
        }

        public static void SetRoundType(ToNRoundType roundType) {
            if (RoundType == roundType) return;
            RoundType = roundType;
            DSRichPresence.SetRoundType(roundType);

            WebSocketAPI.EventRoundType.Send(roundType);
        }

        public static void SetItem(ToNIndex.Item item) {
            Item = item;

            StatsWindow.SetItem(item);
            LilOSC.SetDirty();

            WebSocketAPI.EventItem.Send(item);
        }

        public static void SetLocation(ToNIndex.Map location) {
            if (Location.IsEmpty != location.IsEmpty)
                DSRichPresence.UpdateTimestamp();

            Location = location;
            StatsWindow.SetLocation(Location, !Terrors.IsEmpty);
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();

            WebSocketAPI.EventLocation.Send(location);
        }

        public static void SetPageCount(int pages) {
            if (PageCount == pages) return;

            PageCount = pages;
            StatsWindow.SetPageCount(pages);
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();

            WebSocketAPI.SendValue("PAGE_COUNT", PageCount);
        }

        // Unified damage event
        public static void AddDamage(int damage) {
            if (!IsEmulated) StatsWindow.AddDamage(damage);
            LilOSC.SetDamage(damage);
            OpenRGBControl.SetDamaged();

            WebSocketAPI.SendValue("DAMAGED", damage);
        }
    }
}
