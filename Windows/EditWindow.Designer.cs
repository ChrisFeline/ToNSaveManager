namespace ToNSaveManager
{
    partial class EditWindow
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditWindow));
            textBox1 = new TextBox();
            button1 = new Button();
            button2 = new Button();
            contextMenuStrip1 = new ContextMenuStrip(components);
            insertToolStripMenuItem = new ToolStripMenuItem();
            buttonInsert = new Button();
            contextMenuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            textBox1.BackColor = Color.FromArgb(59, 66, 82);
            textBox1.ForeColor = Color.White;
            textBox1.Location = new Point(10, 10);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(406, 23);
            textBox1.TabIndex = 0;
            textBox1.TextChanged += textBox1_TextChanged;
            // 
            // button1
            // 
            button1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            button1.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            button1.FlatStyle = FlatStyle.Flat;
            button1.ForeColor = Color.White;
            button1.Location = new Point(13, 42);
            button1.Name = "button1";
            button1.Size = new Size(75, 29);
            button1.TabIndex = 1;
            button1.Text = "Save";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // button2
            // 
            button2.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            button2.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            button2.FlatStyle = FlatStyle.Flat;
            button2.ForeColor = Color.White;
            button2.Location = new Point(338, 42);
            button2.Name = "button2";
            button2.Size = new Size(75, 29);
            button2.TabIndex = 2;
            button2.Text = "Cancel";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // contextMenuStrip1
            // 
            contextMenuStrip1.Items.AddRange(new ToolStripItem[] { insertToolStripMenuItem });
            contextMenuStrip1.Name = "contextMenuStrip1";
            contextMenuStrip1.Size = new Size(104, 26);
            // 
            // insertToolStripMenuItem
            // 
            insertToolStripMenuItem.Name = "insertToolStripMenuItem";
            insertToolStripMenuItem.Size = new Size(103, 22);
            insertToolStripMenuItem.Text = "Insert";
            // 
            // buttonInsert
            // 
            buttonInsert.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            buttonInsert.FlatAppearance.BorderColor = Color.FromArgb(122, 122, 122);
            buttonInsert.FlatStyle = FlatStyle.Flat;
            buttonInsert.ForeColor = Color.White;
            buttonInsert.Location = new Point(94, 42);
            buttonInsert.Name = "buttonInsert";
            buttonInsert.Size = new Size(238, 29);
            buttonInsert.TabIndex = 3;
            buttonInsert.Text = "Insert Template Key";
            buttonInsert.UseVisualStyleBackColor = true;
            buttonInsert.MouseClick += buttonInsert_MouseClick;
            // 
            // EditWindow
            // 
            AcceptButton = button1;
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(46, 52, 64);
            CancelButton = button2;
            ClientSize = new Size(426, 79);
            ControlBox = false;
            Controls.Add(buttonInsert);
            Controls.Add(button2);
            Controls.Add(button1);
            Controls.Add(textBox1);
            ForeColor = SystemColors.ControlText;
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "EditWindow";
            Padding = new Padding(10);
            ShowInTaskbar = false;
            Text = "Note Editor";
            Shown += EditWindow_Shown;
            contextMenuStrip1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox textBox1;
        private Button button1;
        private Button button2;
        private ContextMenuStrip contextMenuStrip1;
        private ToolStripMenuItem insertToolStripMenuItem;
        private Button buttonInsert;
    }
}