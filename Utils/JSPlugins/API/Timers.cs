using System.Timers;
using Jint;
using Jint.Native;
using Jint.Native.Function;
using Timer = System.Timers.Timer;

namespace ToNSaveManager.Utils.JSPlugins.API {
    [JSEngineAPI]
    internal static class Timers {
        private static Dictionary<int, TimerInstance> Instances = new Dictionary<int, TimerInstance>();
        private static int CurrentTimer = 0;

        internal static int OpenTimer(Function callback, int time, bool loop, params JsValue[] args) {
            int id = CurrentTimer++;

            Instances[id] = new TimerInstance(callback, args, time, id, loop);

            return id;
        }

        internal static void CloseTimer(int id) {
            if (Instances.ContainsKey(id)) {
                Instances[id].Dispose();
                Instances.Remove(id);
            }
        }

        internal static void Register(Engine engine) {
            engine.SetValue("setTimeout", SetTimeout);
            engine.SetValue("setInterval", SetInterval);
            engine.SetValue("clearTimeout", ClearTimeout);
            engine.SetValue("clearInterval", ClearInterval);
        }

        internal static int SetTimeout(JsValue callback, int time, params JsValue[] args) {
            if (callback is Function function) {
                return OpenTimer(function, time, false, args);
            } else throw new ArgumentException("Callback must be a native function.");
        }
        internal static int SetInterval(JsValue callback, int time, params JsValue[] args) {
            if (callback is Function function) {
                return OpenTimer(function, time, true, args);
            } else throw new ArgumentException("Callback must be a native function.");
        }

        internal static void ClearTimeout(int id) => CloseTimer(id);
        internal static void ClearInterval(int id) => CloseTimer(id);

        private class TimerInstance : IDisposable {
            private Timer _Timer;
            private Function Callback;
            private int ID;
            private bool Loop;
            private JsValue[]? Args;
            private bool Disposed;

            internal TimerInstance(Function callback, JsValue[]? args, int interval, int id, bool loop) {
                _Timer = new Timer();
                _Timer.Elapsed += OnTick;
                _Timer.Interval = interval;
                _Timer.AutoReset = loop;
                _Timer.Enabled = true;
                Callback = callback;
                ID = id;
                Args = args;
                Loop = loop;
            }

            private void OnTick(object? state, ElapsedEventArgs args) {
                if (!Loop) {
                    Dispose();
                    CloseTimer(ID);
                }

                if (Args !=   null)
                    JSEngine.Enqueue(Callback, Args);
                else
                    JSEngine.Enqueue(Callback);
            }

            public void Dispose() {
                if (Disposed) return;
                Disposed = true;
                _Timer.Stop();
                _Timer.Dispose();
            }
        }
    }
}
