using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
using ToNSaveManager.Utils.API;
using ToNSaveManager.Utils.Discord;
using ToNSaveManager.Utils.OpenRGB;

namespace ToNSaveManager.Utils.LogParser
{
    internal class ToNLogContext : LogContext {
        internal static ToNLogContext? Instance { get; private set; }

        public bool SaveCodeCreated { get; set; }
        public bool HasLoadedSave { get; set; }

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
        public override void Enter(string name, DateTime date) {
            base.Enter(name, date);
            HasLoadedSave = false;

            if (Settings.Get.AutoCopy && Settings.Get.CopyOnJoin) MainWindow.Instance?.CopyRecent(true);
            if (IsRecent) ToNGameState.ClearStates();
        }
        public override void Enter(string instanceID, bool isHomeWorld) {
            base.Enter(instanceID, isHomeWorld);
            if (IsRecent) WebSocketAPI.SendValue("INSTANCE", instanceID);
        }

        public override void Join(string displayName) {
            base.Join(displayName);
            ToNGameState.SetPlayerCount(Players.Count);
        }
        public override void Leave(string displayName) {
            base.Leave(displayName);
            ToNGameState.SetPlayerCount(Players.Count);
        }

        public override void OnAwake() {
            if (IsRecent) {
                Instance = this;
                ToNGameState.ClearStates();
            }

            ClearSummary();
            SetOptedIn(false);
            SetTerrorMatrix(TerrorMatrix.Empty);
            SetLocation(ToNIndex.Map.Empty);
            SetRoundResult(IsLeavingRoom ? ToNRoundResult.D : ToNRoundResult.R);
        }

        public override void OnInit() {
            base.OnInit();

            if (IsRecent && Authenticated && !string.IsNullOrEmpty(DisplayName))
                ToNGameState.SetDisplayName(DisplayName, false);
        }

        public void SetIsKiller(bool isKiller) {
            if (IsSaboteour == isKiller) return;

            IsSaboteour = isKiller;
            if (IsRecent) ToNGameState.SetKiller(IsSaboteour);
        }

        public void SetIsAlive(bool alive) {
            if (IsAlive == alive) return;

            IsAlive = alive;
            if (IsRecent) ToNGameState.SetAlive(IsAlive);
        }

        public void SetOptedIn(bool isOptedIn) {
            IsOptedIn = isOptedIn;

            if (IsRecent) ToNGameState.SetOptedIn(IsOptedIn);

            if (!IsOptedIn) {
                SetIsAlive(false);
                ClearSummary();
            }
        }
        public void SetTerrorMatrix(TerrorMatrix matrix) {
            if (Terrors.IsEmpty != matrix.IsEmpty) {
                SetIsAlive(Terrors.IsEmpty); // Is living
                // Live Build Fallback
                if (Location.IsEmpty) DSRichPresence.UpdateTimestamp();
            }

            matrix.IsSaboteur = IsSaboteour && !matrix.IsEmpty;
            matrix.MapID = Location.IsEmpty ? -1 : Location.Id;
            Terrors = matrix;

            if (IsRecent) ToNGameState.SetTerrorMatrix(matrix);
        }
        public void SetLocation(ToNIndex.Map map) {
            Location = map;
            if (IsRecent) ToNGameState.SetLocation(map);
        }
        public void SetRoundResult(ToNRoundResult result) {
            Result = result;
        }

        public RoundSummary Summary { get; private set; } = new RoundSummary(ToNRoundResult.R, TerrorMatrix.Empty, ToNIndex.Map.Empty, null, true);
        // Triggered at end of round
        public void SaveSummary() {
            Summary = new RoundSummary(Result, Terrors, Location, Settings.Get.SaveRoundNote ? Settings.Get.RoundNoteTemplate.GetString() : null);
        }
        // Triggered when consumed by the save code
        public void ClearSummary() {
            Summary = new RoundSummary(ToNRoundResult.R, TerrorMatrix.Empty, ToNIndex.Map.Empty, null, true);
        }

        public struct RoundSummary {
            public ToNRoundResult Result;
            public TerrorMatrix Terrors;
            public ToNIndex.Map Map;
            public bool IsEmpty;
            public string? Note;

            public RoundSummary(ToNRoundResult result, TerrorMatrix terrors, ToNIndex.Map map, string? note, bool isEmpty = false) {
                Result = result;
                Terrors = terrors;
                Map = map;
                IsEmpty = isEmpty;
                Note = note;
            }
        }
    }
}
