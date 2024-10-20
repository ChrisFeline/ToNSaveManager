using System.Net.Sockets;
using System.Net;
using System.Text;
using ToNSaveManager.Models;
using ToNSaveManager.Models.Index;
using System.Numerics;
using Timer = System.Windows.Forms.Timer;
using ToNSaveManager.Utils.LogParser;
using ToNSaveManager.Utils.API;

namespace ToNSaveManager.Utils
{
    internal static class LilOSC {
        static readonly LoggerSource Logger = new LoggerSource("OSC");

        static UdpClient? UdpClient;
        const string ParamRoundType = "ToN_RoundType";

        const string ParamTerror1 = "ToN_Terror1";
        const string ParamTerror2 = "ToN_Terror2";
        const string ParamTerror3 = "ToN_Terror3";

        const string ParamTPhase1 = "ToN_TPhase1";
        const string ParamTPhase2 = "ToN_TPhase2";
        const string ParamTPhase3 = "ToN_TPhase3";

        const string ParamOptedIn = "ToN_OptedIn";
        const string ParamSaboteur = "ToN_Saboteur";
        const string ParamMap = "ToN_Map";
        const string ParamItem = "ToN_Item";
        const string ParamEncounter = "ToN_Encounter";

        internal const string ParamTerrorColorH = "ToN_ColorH";
        internal const string ParamTerrorColorS = "ToN_ColorS";
        internal const string ParamTerrorColorV = "ToN_ColorV";
        internal const string ParamTerrorColorL = "ToN_ColorL";
        internal const string ParamTerrorColorR = "ToN_ColorR";
        internal const string ParamTerrorColorG = "ToN_ColorG";
        internal const string ParamTerrorColorB = "ToN_ColorB";

        const string ParamAlive = "ToN_IsAlive";
        const string ParamReborn = "ToN_Reborn";
        const string ParamStarted = "ToN_IsStarted";
        const string ParamDamaged = "ToN_Damaged";
        const string ParamDeath = "ToN_DeathID";
        const string ParamPages = "ToN_Pages";
        const string ParamItemStatus = "ToN_ItemStatus";
        const string ParamMaster = "ToN_MasterChange";

        static readonly string[] ParamAll = [
            ParamRoundType, ParamTerror1, ParamTerror2, ParamTerror3, ParamTPhase1, ParamTPhase2, ParamTPhase3,
            ParamOptedIn, ParamSaboteur, ParamMap, ParamEncounter,
            ParamTerrorColorH, ParamTerrorColorS, ParamTerrorColorV, ParamTerrorColorL,
            ParamTerrorColorR, ParamTerrorColorG, ParamTerrorColorB,
            ParamAlive, ParamReborn, ParamDamaged, ParamMaster, ParamDeath, ParamPages, ParamItemStatus
        ];
        const string ParameterFileName = "osc_parameters.txt";
        internal static void Initialize() {
            SendData(true);

            try {
                if (!Settings.Get.OSCEnabled || File.Exists(ParameterFileName)) return;
                File.WriteAllText(ParameterFileName, string.Join(',', ParamAll));
            } catch { }
        }

        internal static bool IsDirty = false;

        static int LastRoundType = -1;
        static int LastTerror1 = -1;
        static int LastTerror2 = -1;
        static int LastTerror3 = -1;
        static int LastTPhase1 = -1;
        static int LastTPhase2 = -1;
        static int LastTPhase3 = -1;
        static bool LastOptedIn = false;
        static bool LastSaboteur = false;
        static bool LastAlive = true;
        static bool LastReborn = false;
        static bool LastStarted = false;
        static int LastMapID = -1;
        static int LastItemID = -1;
        static Color LastTerrorColor = Color.Black;

        static int LastPageCount = 0;

        static string[]? m_EncounterKeys { get; set; }
        static string[] EncounterKeys {
            get {
                if (m_EncounterKeys == null) {
                    var keys = new List<string>();

                    foreach (var item in ToNIndex.Instance.AllTerrors.Where(t => t.Encounters != null && t.Encounters.Length > 0)) {
#pragma warning disable CS8604 // Possible null reference argument.
                        keys.AddRange(item.Encounters.Select(e => e.Suffix));
#pragma warning restore CS8604 // Possible null reference argument.
                    }

                    m_EncounterKeys = keys.ToArray();
                }

                return m_EncounterKeys;
            }
        }

        static Dictionary<int, bool> LastEncounters = new Dictionary<int, bool>();

        static bool IsOptedIn => ToNGameState.IsOptedIn;
        static TerrorMatrix TMatrix => ToNGameState.Terrors;
        static ToNIndex.Map RMap => ToNGameState.Location;
        static ToNIndex.Item RItem => ToNGameState.Item;
        static int PageCount => ToNGameState.PageCount;
        static bool IsAlive => ToNGameState.IsAlive;
        static bool IsRoundActive => ToNGameState.IsRoundActive;
        static bool IsReborn => ToNGameState.IsReborn;

        static string ChatboxMessage = string.Empty;
        static bool ChatboxClear = false;

        static int ChatboxInterval;
        static int ChatboxCountdown;

        internal static void SetChatboxMessage(string message, int interval = 5000) {
            ChatboxInterval = Math.Max(interval, 3000);
            if (message.Length > 144) message = message.Substring(0, 144);
            ChatboxMessage = message;

            ChatboxCountdown = Math.Max(3000 - (ChatboxInterval - ChatboxCountdown), 0);
            ChatboxClear = string.IsNullOrEmpty(message);
        }


        static bool ItemStatus = true;
        internal static void SetItemStatus(bool status) {
            if (ItemStatus != status) {
                ItemStatus = status;
                Logger.Debug("Setting Item Status: " + status);
                SendParam(ParamItemStatus, status);
            }
        }

        #region Death ID Ticker
        private static Timer? DeathTimer;
        private static Queue<int> DeathQueue = new Queue<int>();
        private static int DeathSendID = -1;

        internal static void SetDeathID(int id) {
            if (DeathTimer == null) {
                DeathTimer = new Timer();
                DeathTimer.Tick += DeathTimer_Tick;
            }

            if (DeathSendID > -1) {
                DeathQueue.Enqueue(id);
                return;
            }

            DeathTimer.Stop();
            DeathSendID = id;
            DeathTimer.Interval = 100;
            DeathTimer.Start();
        }

        private static void SendDeathID(int id) {
            SendParam(ParamDeath, id);
        }

        private static void DeathTimer_Tick(object? sender, EventArgs e) {
            if (DeathTimer == null) return;

            SendDeathID(DeathSendID);
            DeathTimer.Stop();

            if (DeathSendID > 0) {
                DeathSendID = 0;
                DeathTimer.Interval = Settings.Get.OSCDeathDecay;
                DeathTimer.Start();
            } else if (DeathQueue.Count > 0) {
                DeathSendID = DeathQueue.Dequeue();
                DeathTimer.Interval = Settings.Get.OSCDeathCooldown;
                DeathTimer.Start();
            } else {
                DeathSendID = -1;
            }

            Logger.Debug("Next Death ID Status: " + DeathSendID);
        }
        #endregion

        #region Host Change
        private static Timer? HostTimer;
        private static bool LastHost = false;
        internal static void SendHostChange() {
            if (LastHost || !MainWindow.Started || !Settings.Get.OSCMasterChange || !Settings.Get.OSCEnabled) return;

            if (HostTimer == null) {
                HostTimer = new Timer();
                HostTimer.Tick += HostTimer_Tick;
                HostTimer.Interval = Settings.Get.OSCMasterChangeInterval;
            }

            LastHost = true;
            SendParam(ParamMaster, LastHost);

            HostTimer.Stop();
            if (Settings.Get.OSCMasterChangeInterval > 0) {
                HostTimer.Interval = Settings.Get.OSCMasterChangeInterval;
                HostTimer.Start();
            } else LastHost = false;
        }

        private static void HostTimer_Tick(object? sender, EventArgs e) {
            HostTimer?.Stop();

            LastHost = false;
            SendParam(ParamMaster, LastHost);
        }
        #endregion

        private static Timer? DamageTimer;
        private static int LastDamage = 0;
        internal static void SetDamage(int damage) {
            if (!MainWindow.Started || !Settings.Get.OSCDamagedEvent || !Settings.Get.OSCEnabled) return;

            if (DamageTimer == null) {
                DamageTimer = new Timer();
                DamageTimer.Tick += DamageTimer_Tick;
                DamageTimer.Interval = Settings.Get.OSCDamagedInterval;
            }

            if (LastDamage != damage) {
                LastDamage = damage;
                SendParam(ParamDamaged, damage); // change param to a const

                DamageTimer.Stop();
                if (Settings.Get.OSCDamagedInterval > 0) {
                    DamageTimer.Interval = Settings.Get.OSCDamagedInterval;
                    DamageTimer.Start();
                } else {
                    LastDamage = 0;
                }
            }
        }

        private static void DamageTimer_Tick(object? sender, EventArgs e) {
            DamageTimer?.Stop();

            LastDamage = 0;
            SendParam(ParamDamaged, LastDamage);
        }

        internal static void SetDirty() {
            IsDirty = true;
        }

        internal static void SendData(bool force = false) {
            if (Settings.Get.OSCEnabled && ((MainWindow.Started && IsDirty) || force)) {
                IsDirty = false;

                if (LastOptedIn != IsOptedIn || force) SendParam(ParamOptedIn, LastOptedIn = IsOptedIn);
                if (LastMapID != RMap.Id || force) SendParam(ParamMap, LastMapID = RMap.Id);
                if (LastItemID != RItem.Id || force) SendParam(ParamItem, LastItemID = RItem.Id);

                int value = (int)TMatrix.RoundType;
                if (LastRoundType != value && value == 0) SendParam(ParamRoundType, LastRoundType = value);

                ToNIndex.TerrorInfo info1 = TMatrix.Terror1;
                ToNIndex.TerrorInfo info2 = TMatrix.Terror2;
                ToNIndex.TerrorInfo info3 = TMatrix.Terror3;

                int value1 = info1.Index, phase1 = info1.Phase;
                int value2 = info2.Index, phase2 = info2.Phase;
                int value3 = info3.Index, phase3 = info3.Phase;

                switch (TMatrix.RoundType) {
                    case ToNRoundType.Fog_Alternate:
                    case ToNRoundType.Fog:
                    case ToNRoundType.Eight_Pages:
                        if (TMatrix.RoundType == ToNRoundType.Eight_Pages && value1 < 255) {
                            ToNIndex.Terror terror = ToNIndex.Instance.GetTerror(info1);
                            value1 = terror.Id;
                            value2 = (int)terror.Group;
                            value3 = info1.Index;
                        }
                        break;

                    default:
                        break;
                }

                if (Settings.Get.OSCSendColor) {
                    Color terrorColor = TMatrix.DisplayColor;

                    if (LastTerrorColor != terrorColor || force) {
                        LastTerrorColor = terrorColor;
                        Vector3 col;
                        switch (Settings.Get.OSCSendColorFormat) {
                            default: // HSV
                                col = Color2HSV(terrorColor);
                                SendParam(ParamTerrorColorH, col.X);
                                SendParam(ParamTerrorColorS, col.Y);
                                SendParam(ParamTerrorColorV, col.Z);
                                break;
                            case 1: // RGB
                                const float f_byte_max = byte.MaxValue;
                                SendParam(ParamTerrorColorR, terrorColor.R / f_byte_max);
                                SendParam(ParamTerrorColorG, terrorColor.G / f_byte_max);
                                SendParam(ParamTerrorColorB, terrorColor.B / f_byte_max);
                                break;
                            case 2: // HSL
                                SendParam(ParamTerrorColorH, terrorColor.GetHue());
                                SendParam(ParamTerrorColorS, terrorColor.GetSaturation());
                                SendParam(ParamTerrorColorL, terrorColor.GetBrightness());
                                break;
                            case 3: // RGB32
                                SendParam(ParamTerrorColorR, terrorColor.R);
                                SendParam(ParamTerrorColorG, terrorColor.G);
                                SendParam(ParamTerrorColorB, terrorColor.B);
                                break;
                        }
                    }
                }

                if (LastTerror1 != value1 || force) SendParam(ParamTerror1, LastTerror1 = value1);
                if (LastTerror2 != value2 || force) SendParam(ParamTerror2, LastTerror2 = value2);
                if (LastTerror3 != value3 || force) SendParam(ParamTerror3, LastTerror3 = value3);

                if (LastTPhase1 != phase1 || force) SendParam(ParamTPhase1, LastTPhase1 = phase1);
                if (LastTPhase2 != phase2 || force) SendParam(ParamTPhase2, LastTPhase2 = phase2);
                if (LastTPhase3 != phase3 || force) SendParam(ParamTPhase3, LastTPhase3 = phase3);

                // Encounters
                for (int i = 0; i < EncounterKeys.Length; i++) {
                    string key = EncounterKeys[i];
                    bool value0 = false;

                    for (int j = TMatrix.StartIndex; j < TMatrix.Length; j++) {
                        var info = TMatrix[j];
                        if (info.Encounter > -1 && info.Value.Encounters != null &&
                            info.Value.Encounters.Length > 0 && info.Value.Encounters[info.Encounter].Suffix == key) {
                            value0 = true;
                        }
                    }

                    if (force || !LastEncounters.ContainsKey(i) || LastEncounters[i] != value0) {
                        SendParam(ParamEncounter + key, LastEncounters[i] = value0);
                    }
                }

                if (LastRoundType != value || force) SendParam(ParamRoundType, LastRoundType = value);

                if (LastSaboteur != TMatrix.IsSaboteur || force) SendParam(ParamSaboteur, LastSaboteur = TMatrix.IsSaboteur);
                if (LastPageCount != PageCount || force) SendParam(ParamPages, LastPageCount = PageCount);
                if (LastAlive != IsAlive || force) SendParam(ParamAlive, LastAlive = IsAlive);
                if (LastReborn != IsReborn || force) SendParam(ParamReborn, LastReborn = IsReborn);
                if (LastStarted != IsRoundActive || force) SendParam(ParamStarted, LastStarted = IsRoundActive);
            }

            if (ChatboxClear || (ToNLogContext.CanSendChatbox && MainWindow.Started && !force && !string.IsNullOrEmpty(ChatboxMessage))) {
                ChatboxCountdown -= MainWindow.LogWatcher.Interval;
                if (ChatboxCountdown < 0) {
                    ChatboxCountdown = ChatboxInterval;
                    SendChatbox(ChatboxMessage);
                    if (ChatboxClear) ChatboxClear = false;
                }
            }
        }

        static Vector3 Color2HSV(Color color) {
            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            float hue = color.GetHue() / 360f; // Hue is always the same for HSL & HSV
            float sat = (max == 0) ? 0 : 1f - (1f * min / max);
            float val = max / 255f;

            return new Vector3(hue, sat, val);
        }

        #region Encoding & Buffers
        static readonly byte[] temp_buffer = new byte[2048];

        private static void EncodeInto(byte[] data, ref int offset, string path, int value) {
            byte[] tmp = Encoding.UTF8.GetBytes(path);
            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }

            tmp = new byte[] { 44, 105 }; // ",i"

            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }

            Array.Copy(BitConverter.GetBytes(IPAddress.HostToNetworkOrder(value)), 0, data, offset, 4);
            offset += 4;
        }

        private static void EncodeInto(byte[] data, ref int offset, string path, float value) {
            byte[] tmp = Encoding.UTF8.GetBytes(path);
            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }

            tmp = new byte[] { 44, 102 }; // ",f"

            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }

            tmp = BitConverter.GetBytes(value);
            if (BitConverter.IsLittleEndian) {
                Array.Reverse(tmp);
            }
            Array.Copy(tmp, 0, data, offset, 4);
            offset += 4;
        }

        private static void EncodeInto(byte[] data, ref int offset, string path, bool value) {
            byte[] tmp = Encoding.UTF8.GetBytes(path);
            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }

            tmp = new byte[] { 44, (byte)(value ? 'T' : 'F') }; // ",T"

            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }
        }

        private static void EncodeInto(byte[] data, ref int offset, string path, string value, bool direct, bool complete = false) {
            byte[] tmp = Encoding.UTF8.GetBytes(path);
            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }

            tmp = new byte[] { 44, 115, (byte)(direct ? 'T' : 'F'), (byte)(complete ? 'T' : 'F') }; // ",sTT"

            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }

            tmp = Encoding.UTF8.GetBytes(value);
            Array.Copy(tmp, 0, data, offset, tmp.Length);
            data[tmp.Length + offset] = 0;
            offset += tmp.Length;
            for (int endOffset = (offset + 4) & ~3; offset < endOffset; offset++) {
                data[offset] = 0;
            }
        }

        private static void SendParam(string name, int value) {
            int encodedLength = 0;
            EncodeInto(temp_buffer, ref encodedLength, "/avatar/parameters/" + name, value);
            SendBuffer(temp_buffer, encodedLength);

            Logger.Debug("Sending Param: " + name + " = " + value);
        }

        private static void SendParam(string name, float value) {
            int encodedLength = 0;
            EncodeInto(temp_buffer, ref encodedLength, "/avatar/parameters/" + name, value);
            SendBuffer(temp_buffer, encodedLength);

            Logger.Debug("Sending Param: " + name + " = " + value);
        }

        private static void SendParam(string name, bool value) {
            int encodedLength = 0;
            EncodeInto(temp_buffer, ref encodedLength, "/avatar/parameters/" + name, value);
            SendBuffer(temp_buffer, encodedLength);

            Logger.Debug("Sending Param: " + name + " = " + value);
        }

        private static void SendChatbox(string message, bool direct = true, bool complete = false) {
            int encodedLength = 0;
            EncodeInto(temp_buffer, ref encodedLength, "/chatbox/input", message, direct, complete);
            SendBuffer(temp_buffer, encodedLength);
        }

        private static void SendBuffer(byte[] buffer, int encodedLength, IPAddress? ipAddress = null, int port = 9000) {
            if (UdpClient == null) UdpClient = new UdpClient();
            UdpClient.Send(buffer, encodedLength, (ipAddress ?? IPAddress.Loopback).ToString(), port);
        }
        #endregion
    }
}
