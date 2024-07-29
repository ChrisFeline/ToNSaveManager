using System.Diagnostics;
using ToNSaveManager.Extensions;
using ToNSaveManager.Localization;
using ToNSaveManager.Models;

namespace ToNSaveManager
{
    public partial class StatsWindow : Form
    {
        internal static StatsWindow? Instance;

        public StatsWindow() {
            InitializeComponent();
        }

        public static void Open(Form parent) {
            if (Instance == null || Instance.IsDisposed) Instance = new StatsWindow();

            if (Instance.Visible)
            {
                Instance.BringToFront();
                return;
            }

            Instance.StartPosition = FormStartPosition.Manual;
            Instance.Location = new Point(
                parent.Location.X + (parent.Width - Instance.Width) / 2,
                Math.Max(parent.Location.Y + (parent.Height - Instance.Height) / 2, 0)
            );
            Instance.Show(); // Don't parent
        }

        internal void LocalizeContent() {
            LANG.C(this, "STATS.TITLE");
            RightToLeft = LANG.IsRightToLeft ? RightToLeft.Yes : RightToLeft.No;
        }

        private void StatsWindow_Load(object sender, EventArgs e)
        {
            LocalizeContent();
        }
    }
}
