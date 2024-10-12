using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager
{
    internal class TooltipUtil : ToolTip
    {
        static readonly TooltipUtil Instance = new TooltipUtil();

        TooltipUtil()
        {
            this.UseAnimation = true;
            this.UseFading = true;
            this.AutomaticDelay = 0;
            this.AutoPopDelay = 10000;
        }

        public static void Set(Control control, string? text)
        {
            Instance.Active = false;
            Instance.SetToolTip(control, text);
            Instance.Active = true;
        }

        public static void Show(Control control, string? text) {
            Set(control, text);
            Instance.Show(text, control);
        }
    }
}
