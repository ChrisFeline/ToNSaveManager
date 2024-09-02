using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
using System.Text;
using ToNSaveManager.Models;
using Newtonsoft.Json.Linq;
using System.Xml.Linq;
using ToNSaveManager.Models.Index;
using System.Numerics;

namespace ToNSaveManager.Utils
{
    internal static class LilOSC {
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
        const string ParamEncounter = "ToN_Encounter";

        const string ParamTerrorColorH = "ToN_ColorH";
        const string ParamTerrorColorS = "ToN_ColorS";
        const string ParamTerrorColorV = "ToN_ColorV";

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
        static int LastMapID = -1;
        static Color LastTerrorColor = Color.Black;

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

        static bool IsOptedIn = false;
        public static TerrorMatrix TMatrix = TerrorMatrix.Empty;

        static readonly ToNIndex.Map EmptyMap = new ToNIndex.Map() { IsEmpty = true, Id = byte.MaxValue, Name = "Empty" };
        public static ToNIndex.Map RMap = EmptyMap;

        static string ChatboxMessage = string.Empty;
        static bool ChatboxClear = false;
        static int ChatboxInterval;
        static int ChatboxCountdown = 0;

        internal static void SetChatboxMessage(string message, int interval = 5) {
            ChatboxInterval = Math.Max(interval, 3);
            if (message.Length > 144) message = message.Substring(0, 144);
            ChatboxMessage = message;

            ChatboxCountdown = Math.Max(3 - (ChatboxInterval - ChatboxCountdown), 0);
            ChatboxClear = string.IsNullOrEmpty(message);
        }

        internal static void SetTerrorMatrix(TerrorMatrix terrorMatrix) {
            TMatrix = terrorMatrix;
            IsDirty = true;

            // Reusing LilOSC.SetTerrorMatrix method because it is already properly handled everywhere.
            StatsWindow.SetTerrorMatrix(terrorMatrix);
        }

        internal static void SetMap(ToNIndex.Map? map = null) {
            RMap = map == null || map.IsEmpty ? EmptyMap : map;
            IsDirty = true;

            // Reusing LilOSC.SetMap method because it is already properly handled everywhere.
            StatsWindow.SetLocation(RMap);
        }

        internal static void SetOptInStatus(bool optedIn) {
            IsOptedIn = optedIn;
            IsDirty = true;
        }

        internal static void SendData(bool force = false) {
            if (Settings.Get.OSCEnabled && ((MainWindow.Started && IsDirty) || force)) {
                IsDirty = false;

                if (LastOptedIn != IsOptedIn || force) SendParam(ParamOptedIn, LastOptedIn = IsOptedIn);
                if (LastMapID != RMap.Id || force) SendParam(ParamMap, LastMapID = RMap.Id);

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

                // Color Testing
                if (Settings.Get.OSCSendColor) {
                    Color terrorColor;
                    if (TMatrix.TerrorCount > 0 &&
                        TMatrix.RoundType != ToNRoundType.Eight_Pages &&
                        TMatrix.RoundType != ToNRoundType.Fog &&
                        TMatrix.RoundType != ToNRoundType.Fog_Alternate) {
                        Color color1 = ToNIndex.Instance.GetTerror(info1).Color;
                        Color color2 = ToNIndex.Instance.GetTerror(info2).Color;
                        Color color3 = ToNIndex.Instance.GetTerror(info3).Color;

                        Color c;
                        int R = 0, G = 0, B = 0;
                        for (int i = 0; i < TMatrix.TerrorCount; i++) {
                            if (i > 2) break;

                            switch (i) {
                                case 0: c = color1; break;
                                case 1: c = color2; break;
                                case 2: c = color3; break;
                                default: c = Color.White; break;
                            }

                            R += c.R;
                            G += c.G;
                            B += c.B;
                        }

                        terrorColor = Color.FromArgb(R / TMatrix.TerrorCount, G / TMatrix.TerrorCount, B / TMatrix.TerrorCount);
                    } else terrorColor = Color.White;

                    if (LastTerrorColor != terrorColor || force) {
                        Vector3 hsv = Color2HSV(LastTerrorColor = terrorColor);
                        SendParam(ParamTerrorColorH, hsv.X);
                        SendParam(ParamTerrorColorS, hsv.Y);
                        SendParam(ParamTerrorColorV, hsv.Z);
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

                    for (int j = 0; j < TMatrix.Length; j++) {
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
            }

            if (ChatboxClear || (Settings.Get.OSCSendChatbox && MainWindow.Started && !force && !string.IsNullOrEmpty(ChatboxMessage))) {
                ChatboxCountdown--;
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
    }
}
