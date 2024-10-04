using RGBColor = OpenRGB.NET.Color;

namespace ToNSaveManager.Utils.OpenRGB {
    internal struct RGBTemp {
        public float R;
        public float G;
        public float B;

        public RGBTemp(Color color) {
            R = color.R;
            G = color.G;
            B = color.B;
        }

        public void Lerp(RGBTemp other, float value) {
            R = _lerp(R, other.R, value);
            G = _lerp(G, other.G, value);
            B = _lerp(B, other.B, value);
        }
        public static RGBTemp Lerp(RGBTemp a, RGBTemp b, float t) {
            a.Lerp(b, t);
            return a;
        }

        private static float _lerp(float a, float b, float t) {
            return a + (b - a) * Math.Clamp(t, 0f, 1f);
        }

        public RGBColor ToRGBColor() {
            return new RGBColor((byte)Math.Round(R), (byte)Math.Round(G), (byte)Math.Round(B));
        }
    }
}
