using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.Extensions {
    internal static class ArrayExtensions {
        public static void FillRange<T>(this T[] array, T value, int start, int end, bool safe = true) {
            int s = Math.Min(start, end);
            int e = Math.Max(end, start) + 1;

            for (int i = s; i < e; i++) {
                if (safe) SetSafe(array, value, i);
                else array[i] = value;
            }
        }
        public static void SetSafe<T>(this T[] array, T value, int i) {
            if (i < 0 || i >= array.Length) return;
            array[i] = value;
        }
    }
}
