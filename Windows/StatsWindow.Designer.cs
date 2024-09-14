namespace ToNSaveManager
{
    partial class StatsWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StatsWindow));
            toolTip = new ToolTip(components);
            contextMenu = new ContextMenuStrip(components);
            ctxTypeInValue = new ToolStripMenuItem();
            ctxCopyStatName = new ToolStripMenuItem();
            listBox1 = new ListBox();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // contextMenu
            // 
            contextMenu.Items.AddRange(new ToolStripItem[] { ctxTypeInValue, ctxCopyStatName });
            contextMenu.Name = "contextMenu";
            contextMenu.Size = new Size(158, 48);
            // 
            // ctxTypeInValue
            // 
            ctxTypeInValue.Name = "ctxTypeInValue";
            ctxTypeInValue.Size = new Size(157, 22);
            ctxTypeInValue.Text = "Type in value";
            ctxTypeInValue.Click += ctxTypeInValue_Click;
            // 
            // ctxCopyStatName
            // 
            ctxCopyStatName.Name = "ctxCopyStatName";
            ctxCopyStatName.Size = new Size(157, 22);
            ctxCopyStatName.Text = "Copy stat name";
            ctxCopyStatName.Click += ctxCopyStatName_Click;
            // 
            // listBox1
            // 
            listBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            listBox1.BackColor = Color.FromArgb(59, 66, 82);
            listBox1.BorderStyle = BorderStyle.FixedSingle;
            listBox1.DrawMode = DrawMode.OwnerDrawFixed;
            listBox1.ForeColor = Color.FromArgb(236, 239, 244);
            listBox1.FormattingEnabled = true;
            listBox1.IntegralHeight = false;
            listBox1.ItemHeight = 1;
            listBox1.Location = new Point(7, 7);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(360, 219);
            listBox1.TabIndex = 0;
            listBox1.TabStop = false;
            listBox1.UseTabStops = false;
            listBox1.DrawItem += listBoxEntries_DrawItem;
            listBox1.SelectedValueChanged += listBoxEntries_SelectedValueChanged;
            listBox1.MouseMove += listBoxEntries_MouseMove;
            listBox1.MouseUp += listBoxEntries_MouseClick;
            // 
            // StatsWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(374, 233);
            Controls.Add(listBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MinimizeBox = false;
            MinimumSize = new Size(340, 0);
            Name = "StatsWindow";
            Padding = new Padding(4);
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ToN Stats Tracker";
            Load += StatsWindow_Load;
            ResizeEnd += StatsWindow_ResizeEnd;
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ToolTip toolTip;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem ctxTypeInValue;
        private ToolStripMenuItem ctxCopyStatName;
        private ListBox listBox1;
    }
}