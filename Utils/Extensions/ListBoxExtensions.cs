using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToNSaveManager.Extensions
{
    internal static class ListBoxExtensions
    {
        public static void FixItemHeight(this ListBox listBox)
        {
            const string c = " ";
            listBox.ItemHeight = TextRenderer.MeasureText(c, listBox.Font).Height + 2;
        }
    }
}
