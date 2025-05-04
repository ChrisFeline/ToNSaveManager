namespace ToNSaveManager.Utils.JSPlugins {
    internal class OSC {
        internal static OSC Instance = new();

        public void SendInt(string parameterName, int value) => LilOSC.SendParam(parameterName, value);
        public void SendFloat(string parameterName, float value) => LilOSC.SendParam(parameterName, value);
        public void SendBool(string parameterName, bool value) => LilOSC.SendParam(parameterName, value);
        public void SendAvatar(string avatarId) => LilOSC.SendAvatar(avatarId);
        public void SendChatbox(string chatbox, bool direct = true, bool complete = false) => LilOSC.SendChatbox(chatbox, direct, complete);
    }
}
