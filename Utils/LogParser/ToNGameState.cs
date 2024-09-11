using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.OpenRGB;

namespace ToNSaveManager.Utils {
    internal static class ToNGameState {
        public static bool IsEmulated { get; private set; }
        public static bool IsAlive { get; private set; }
        public static bool IsSaboteour { get; private set; }
        public static bool IsOptedIn { get; private set; }
        public static int PageCount { get; private set; }

        public static ToNRoundType RoundType { get; private set; } = ToNRoundType.Intermission;
        public static TerrorMatrix Terrors { get; private set; } = TerrorMatrix.Empty;
        public static ToNIndex.Map Location { get; private set; } = ToNIndex.Map.Empty;

        public static void SetEmulated(bool isEmulated) {
            IsEmulated = isEmulated;
        }

        public static void SetAlive(bool isAlive) {
            IsAlive = isAlive;

            OpenRGBControl.SetIsAlive(isAlive);
            StatsWindow.SetActiveInRound(isAlive);
            if (IsAlive) SetPageCount(0);
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();
        }

        public static void SetKiller(bool isKiller) {
            IsSaboteour = isKiller;
            StatsWindow.SetIsKiller(isKiller);
        }

        public static void SetOptedIn(bool isOptedIn) {
            IsOptedIn = isOptedIn;
            LilOSC.SetDirty();
        }

        public static void SetTerrorMatrix(TerrorMatrix matrix) {
            Terrors = matrix;
            SetRoundType(Terrors.RoundType);

            StatsWindow.SetTerrorMatrix(Terrors);
            OpenRGBControl.SetTerrorMatrix(Terrors);
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();
        }

        public static void SetRoundType(ToNRoundType roundType) {
            if (RoundType == roundType) return;
            RoundType = roundType;
            DSRichPresence.SetRoundType(roundType);
        }

        public static void SetLocation(ToNIndex.Map location) {
            if (Location.IsEmpty != location.IsEmpty)
                DSRichPresence.UpdateTimestamp();

            Location = location;
            StatsWindow.SetLocation(Location, !Terrors.IsEmpty);
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();
        }

        public static void SetPageCount(int pages) {
            if (PageCount == pages) return;

            PageCount = pages;
            StatsWindow.SetPageCount(pages);
            LilOSC.SetDirty();
            DSRichPresence.SetDirty();
        }

        // Unified damage event
        public static void AddDamage(int damage) {
            StatsWindow.AddDamage(damage);
            LilOSC.SetDamage(damage);
            OpenRGBControl.SetDamaged();
        }
    }
}
