using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Utils.Discord;

namespace ToNSaveManager.Utils.LogParser
{
    internal class ToNLogContext : LogContext {
        public bool SaveCodeCreated { get; set; }

        public bool IsAlive { get; private set; }

        public bool IsSaboteour { get; private set; }
        public bool IsOptedIn { get; private set; }
        public TerrorMatrix Terrors = TerrorMatrix.Empty;
        public ToNIndex.Map Location = ToNIndex.Map.Empty;
        public ToNRoundResult Result = ToNRoundResult.R;

        public override void Exit() {
            base.Exit();
            OnAwake();
        }

        public override void OnAwake() {
            ClearSummary();
            SetOptedIn(false);
            SetTerrorMatrix(TerrorMatrix.Empty);
            SetLocation(ToNIndex.Map.Empty);
            SetRoundResult(IsLeavingRoom ? ToNRoundResult.D : ToNRoundResult.R);
        }

        public void SetIsKiller(bool isKiller) {
            if (IsSaboteour == isKiller) return;

            IsSaboteour = isKiller;
            if (IsRecent) StatsWindow.SetIsKiller(isKiller);
        }

        public void SetIsAlive(bool alive) {
            if (IsAlive == alive) return;

            IsAlive = alive;
            Logger.Debug("SET PLAYER ALIVE: " + IsAlive);

            if (IsRecent) {
                StatsWindow.SetActiveInRound(alive);
                DSRichPresence.SetIsAlive(alive);
                if (IsAlive) LilOSC.SetPageCount();
            }
        }

        public void SetOptedIn(bool isOptedIn) {
            IsOptedIn = isOptedIn;
            Logger.Debug("SET OPTED IN: " + isOptedIn);

            if (IsRecent) {
                LilOSC.SetOptInStatus(isOptedIn);
            }

            SetIsAlive(false);
            if (!IsOptedIn && Summary.Result != ToNRoundResult.R) ClearSummary();
        }
        public void SetTerrorMatrix(TerrorMatrix matrix) {
            if (Terrors.IsEmpty != matrix.IsEmpty)
                SetIsAlive(Terrors.IsEmpty); // Is living

            Terrors = matrix;
            matrix.IsSaboteur = IsSaboteour && !matrix.IsEmpty;

            if (IsRecent) {
                LilOSC.SetTerrorMatrix(matrix);
            }
        }
        public void SetLocation(ToNIndex.Map map) {
            Location = map;

            if (IsRecent) {
                LilOSC.SetMap(map);
            }
        }
        public void SetRoundResult(ToNRoundResult result) {
            Result = result;
        }

        public RoundSummary Summary { get; private set; } = new RoundSummary(ToNRoundResult.R, TerrorMatrix.Empty, ToNIndex.Map.Empty, true);
        // Triggered at end of round
        public void SaveSummary() {
            Summary = new RoundSummary(Result, Terrors, Location);
        }
        // Triggered when consumed by the save code
        public void ClearSummary() {
            Summary = new RoundSummary(ToNRoundResult.R, TerrorMatrix.Empty, ToNIndex.Map.Empty, true);
        }

        public struct RoundSummary {
            public ToNRoundResult Result;
            public TerrorMatrix Terrors;
            public ToNIndex.Map Map;
            public bool IsEmpty;

            public RoundSummary(ToNRoundResult result, TerrorMatrix terrors, ToNIndex.Map map, bool isEmpty = false) {
                Result = result;
                Terrors = terrors;
                Map = map;
                IsEmpty = isEmpty;
            }
        }
    }
}
