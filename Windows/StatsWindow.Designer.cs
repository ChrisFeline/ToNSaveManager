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
            toolTip = new ToolTip(components);
            statsTable = new TableLayoutPanel();
            contextMenu = new ContextMenuStrip(components);
            ctxTypeInValue = new ToolStripMenuItem();
            ctxCopyStatName = new ToolStripMenuItem();
            contextMenu.SuspendLayout();
            SuspendLayout();
            // 
            // statsTable
            // 
            statsTable.AutoSize = true;
            statsTable.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            statsTable.CellBorderStyle = TableLayoutPanelCellBorderStyle.Single;
            statsTable.ColumnCount = 2;
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statsTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            statsTable.Dock = DockStyle.Top;
            statsTable.ForeColor = Color.White;
            statsTable.Location = new Point(4, 4);
            statsTable.Margin = new Padding(3, 10, 3, 3);
            statsTable.Name = "statsTable";
            statsTable.RowCount = 1;
            statsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            statsTable.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            statsTable.Size = new Size(316, 22);
            statsTable.TabIndex = 0;
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
            // StatsWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(324, 120);
            Controls.Add(statsTable);
            MaximizeBox = false;
            MinimizeBox = false;
            MinimumSize = new Size(340, 0);
            Name = "StatsWindow";
            Padding = new Padding(4);
            ShowIcon = false;
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ToN Stats Tracker";
            Load += StatsWindow_Load;
            ResizeEnd += StatsWindow_ResizeEnd;
            contextMenu.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ToolTip toolTip;
        private TableLayoutPanel statsTable;
        private ContextMenuStrip contextMenu;
        private ToolStripMenuItem ctxTypeInValue;
        private ToolStripMenuItem ctxCopyStatName;
    }
}