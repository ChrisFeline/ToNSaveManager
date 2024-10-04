using OpenRGB.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.Extensions {
    internal static class OpenRGBExtensions {
        internal static bool TryConnect(this OpenRgbClient client) {
			try {
                client.Connect();
			} catch {
                return false;
			}

            return client.Connected;
        }
    }
}
