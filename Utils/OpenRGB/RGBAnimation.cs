using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Utils.OpenRGB {
    internal class RGBAnimation {
        static readonly RGBTemp Black = new RGBTemp(Color.Black);

        public RGBTemp Current;
        public RGBTemp Target;

        public RGBTemp Value;
        public RGBTemp Flashy => RGBTemp.Lerp(Target, Black, Progress);

        public float Progress;

        public bool Completed { get; private set; }
        public bool Refresh { get; private set; }

        public void SetTarget(Color color) {
            Current = Value;
            Target = new RGBTemp(color);
            Progress = 0;
            Completed = false;
        }

        public bool Update(float speed, float deltaTime) {
            if (Progress > 1f) Progress = 0f;
            Progress += speed * deltaTime;

            if (Completed) {
                Refresh = false;
            } else {
                Refresh = true;
                Value = RGBTemp.Lerp(Current, Target, Progress);
                Completed = Progress > 1f;
            }

            return Refresh;
        }
    }
}
