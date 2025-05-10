using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ToNSaveManager.Models;
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
}
