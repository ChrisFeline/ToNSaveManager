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

        public static void Set(Control control, string text)
        {
            Instance.Active = false;
            Instance.SetToolTip(control, text);
            Instance.Active = true;
        }

        public static void Show(Control control, string text, Point point)
        {
            Instance.Active = false;
            Instance.Show(text, control, point);
            Instance.Active = true;
        }
    }
}
