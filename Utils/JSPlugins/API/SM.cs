using Jint;
using Jint.Runtime.Interop;
using ToNSaveManager.Models;

namespace ToNSaveManager.Utils.JSPlugins.API {
    [JSEngineAPI("SM")]
    internal static class SM {
        private static string Source => JSEngine.GetLastSyntaxSource();

        internal static void Register(Engine engine) {
            engine.SetValue("AlertButtons", TypeReference.CreateTypeReference<MessageBoxButtons>(engine));
            engine.SetValue("AlertIcon", TypeReference.CreateTypeReference<MessageBoxIcon>(engine));
            engine.SetValue("AlertResult", TypeReference.CreateTypeReference<DialogResult>(engine));
            engine.SetValue("alert", Alert);
        }

        internal static DialogResult Alert(string message, MessageBoxButtons buttons = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.None) {
            DialogResult result = MessageBox.Show(message, Source, buttons, icon);
            return result;
        }

        public static Entry? LatestSave => MainWindow.SaveData.FindRecentEntry();
    }
}
