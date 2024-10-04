﻿namespace ToNSaveManager
{
    partial class ObjectivesWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ObjectivesWindow));
            listBox1 = new ListBox();
            toolTip = new ToolTip(components);
            SuspendLayout();
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
            listBox1.Location = new Point(12, 12);
            listBox1.Name = "listBox1";
            listBox1.SelectionMode = SelectionMode.None;
            listBox1.Size = new Size(254, 307);
            listBox1.TabIndex = 0;
            listBox1.TabStop = false;
            listBox1.UseTabStops = false;
            listBox1.DrawItem += listBox1_DrawItem;
            listBox1.MouseDown += listBox1_MouseDown;
            listBox1.MouseMove += listBox1_MouseMove;
            listBox1.MouseUp += listBox1_MouseUp;
            // 
            // ObjectivesWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(278, 331);
            Controls.Add(listBox1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ObjectivesWindow";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "ToN Objectives";
            Load += ObjectivesWindow_Load;
            ResumeLayout(false);
        }

        #endregion

        private ListBox listBox1;
        private ToolTip toolTip;
    }
}