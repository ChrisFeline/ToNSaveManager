using System.Reflection;
using System.Windows.Forms;
using ToNSaveManager.Models;

namespace ToNSaveManager.Extensions
{
    internal static class FormHelperExtensions
    {
        public static void BindSettings(this CheckBox listBox, string memberName)
        {
            PropertyInfo? property = typeof(Settings).GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance);
            if (property == null)
                throw new NullReferenceException("Member name not found: " + memberName);

            object? value = property.GetValue(Settings.Get);
            
            listBox.Checked = value != null && (bool)value;
            listBox.CheckedChanged += (o, e) => {
                property.SetValue(Settings.Get, listBox.Checked);
                Settings.Export();
            };
        }

        public static void FixItemHeight(this ListBox listBox)
        {
            const string c = " ";
            listBox.ItemHeight = TextRenderer.MeasureText(c, listBox.Font).Height + 2;
        }
        public static void FixItemHeight(this ComboBox comboBox1, bool changeFont) {
            const string c = " ";
            // Get current DPI scale factor
            // Adjust ComboBox width and height accordingly
            if (changeFont) {
                // comboBox1.Height = (int)(comboBox1.Height * dpiScaleFactor);
                float dpiScaleFactor = comboBox1.DeviceDpi / 96f;
                comboBox1.Font = new Font(comboBox1.Font.FontFamily, comboBox1.Font.Size * dpiScaleFactor);
            } else {
                comboBox1.ItemHeight = TextRenderer.MeasureText(c, comboBox1.Font).Height + 2;
            }
        }

        /// <summary>
        /// Sets up children of a FlowLayoutPanel to auto-resize with the panel in the non-flow dimension.
        /// (This is a workaround for the lack of support for anchoring both sides of a child inside FlowLayoutPanel.)
        /// Optionally also resizes one control to fill remaining space in the flow dimension.
        /// </summary>
        /// <param name="fillControl">Optional child control to fill remaining space in the flow direction.</param>
        public static void AutoSizeChildren(this FlowLayoutPanel panel, Control? fillControl = null)
        {
            // wrapping does not make sense with auto-resizing
            panel.WrapContents = false;

            var isVertical = panel.FlowDirection == FlowDirection.TopDown;
            int dim(Control c, bool flowDir = false) => isVertical ? c.Width : c.Height;
            void setDim(Control c, int size, bool flowDir = false)
            {
                if (isVertical ^ flowDir)
                    c.Width = size;
                else
                    c.Height = size;
            }
            var children = panel.Controls.Cast<Control>().ToList();
            var relSizes = children.ToDictionary(c => c, c => dim(c) - dim(panel));

            // update relative size when controls are resized
            var isPanelResizing = false;
            foreach (var child in children)
            {
                child.Resize += (s, e) => {
                    if (!isPanelResizing)
                        relSizes[child] = dim(child) - dim(panel);
                };
            }

            // resize children when panel is resized
            panel.Resize += (s, e) => {
                isPanelResizing = true;
                foreach (var child in children)
                    setDim(child, dim(panel) + relSizes[child]);
                isPanelResizing = false;
            };

            if (fillControl != null)
            {
                // add the size difference between the panel and its children to the fillControl
                void sizeFill()
                {
                    var childrenSize = children.Sum(c => dim(c, true) + (isVertical ? c.Margin.Vertical : c.Margin.Horizontal));
                    var diff = dim(panel, true) - childrenSize - (isVertical ? panel.Padding.Vertical : panel.Padding.Horizontal);
                    if (diff != 0)
                        setDim(fillControl, dim(fillControl, true) + diff, true);
                }
                panel.Resize += (s, e) => sizeFill();
                foreach (var child in children)
                    child.Resize += (s, e) => sizeFill();
            }
        }
    }
}
