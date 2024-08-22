namespace ToNSaveManager.Windows {
    partial class EmulatorWindow
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            comboRoundType = new ComboBox();
            panel1 = new Panel();
            label1 = new Label();
            panel2 = new Panel();
            panel7 = new Panel();
            comboMonster3 = new ComboBox();
            panel6 = new Panel();
            comboMonster2 = new ComboBox();
            panel3 = new Panel();
            label2 = new Label();
            comboMonster = new ComboBox();
            panel8 = new Panel();
            panel4 = new Panel();
            label3 = new Label();
            comboLocation = new ComboBox();
            panel9 = new Panel();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            SuspendLayout();
            // 
            // comboRoundType
            // 
            comboRoundType.Dock = DockStyle.Right;
            comboRoundType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboRoundType.FormattingEnabled = true;
            comboRoundType.Location = new Point(134, 0);
            comboRoundType.Name = "comboRoundType";
            comboRoundType.Size = new Size(180, 23);
            comboRoundType.TabIndex = 1;
            comboRoundType.SelectedIndexChanged += comboRoundType_SelectedIndexChanged;
            // 
            // panel1
            // 
            panel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel1.Controls.Add(label1);
            panel1.Controls.Add(comboRoundType);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(314, 23);
            panel1.TabIndex = 2;
            // 
            // label1
            // 
            label1.BackColor = Color.Transparent;
            label1.Dock = DockStyle.Left;
            label1.ForeColor = Color.White;
            label1.Location = new Point(0, 0);
            label1.Name = "label1";
            label1.Size = new Size(82, 23);
            label1.TabIndex = 2;
            label1.Text = "Round Type";
            label1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panel2
            // 
            panel2.AutoSize = true;
            panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel2.Controls.Add(panel7);
            panel2.Controls.Add(panel6);
            panel2.Controls.Add(panel3);
            panel2.Controls.Add(panel8);
            panel2.Controls.Add(panel4);
            panel2.Controls.Add(panel9);
            panel2.Controls.Add(panel1);
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(10, 10);
            panel2.Name = "panel2";
            panel2.Size = new Size(314, 161);
            panel2.TabIndex = 3;
            // 
            // panel7
            // 
            panel7.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel7.Controls.Add(comboMonster3);
            panel7.Dock = DockStyle.Top;
            panel7.Location = new Point(0, 138);
            panel7.Name = "panel7";
            panel7.Size = new Size(314, 23);
            panel7.TabIndex = 7;
            // 
            // comboMonster3
            // 
            comboMonster3.BackColor = Color.Black;
            comboMonster3.Dock = DockStyle.Right;
            comboMonster3.DrawMode = DrawMode.OwnerDrawFixed;
            comboMonster3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMonster3.ForeColor = Color.White;
            comboMonster3.FormattingEnabled = true;
            comboMonster3.Location = new Point(134, 0);
            comboMonster3.Name = "comboMonster3";
            comboMonster3.Size = new Size(180, 24);
            comboMonster3.TabIndex = 6;
            comboMonster3.DrawItem += comboMonster_DrawItem;
            comboMonster3.SelectedIndexChanged += comboMonster3_SelectedIndexChanged;
            comboMonster3.EnabledChanged += comboMonster_EnabledChanged;
            // 
            // panel6
            // 
            panel6.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel6.Controls.Add(comboMonster2);
            panel6.Dock = DockStyle.Top;
            panel6.Location = new Point(0, 115);
            panel6.Name = "panel6";
            panel6.Size = new Size(314, 23);
            panel6.TabIndex = 6;
            // 
            // comboMonster2
            // 
            comboMonster2.BackColor = Color.Black;
            comboMonster2.Dock = DockStyle.Right;
            comboMonster2.DrawMode = DrawMode.OwnerDrawFixed;
            comboMonster2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMonster2.ForeColor = Color.White;
            comboMonster2.FormattingEnabled = true;
            comboMonster2.Location = new Point(134, 0);
            comboMonster2.Name = "comboMonster2";
            comboMonster2.Size = new Size(180, 24);
            comboMonster2.TabIndex = 5;
            comboMonster2.DrawItem += comboMonster_DrawItem;
            comboMonster2.SelectedIndexChanged += comboMonster2_SelectedIndexChanged;
            comboMonster2.EnabledChanged += comboMonster_EnabledChanged;
            // 
            // panel3
            // 
            panel3.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel3.Controls.Add(label2);
            panel3.Controls.Add(comboMonster);
            panel3.Dock = DockStyle.Top;
            panel3.Location = new Point(0, 92);
            panel3.Name = "panel3";
            panel3.Size = new Size(314, 23);
            panel3.TabIndex = 3;
            // 
            // label2
            // 
            label2.BackColor = Color.Transparent;
            label2.Dock = DockStyle.Left;
            label2.ForeColor = Color.White;
            label2.Location = new Point(0, 0);
            label2.Name = "label2";
            label2.Size = new Size(82, 23);
            label2.TabIndex = 2;
            label2.Text = "Monster";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // comboMonster
            // 
            comboMonster.BackColor = Color.Black;
            comboMonster.Dock = DockStyle.Right;
            comboMonster.DrawMode = DrawMode.OwnerDrawFixed;
            comboMonster.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMonster.ForeColor = Color.White;
            comboMonster.FormattingEnabled = true;
            comboMonster.Location = new Point(134, 0);
            comboMonster.Name = "comboMonster";
            comboMonster.Size = new Size(180, 24);
            comboMonster.TabIndex = 4;
            comboMonster.DrawItem += comboMonster_DrawItem;
            comboMonster.SelectedIndexChanged += comboMonster_SelectedIndexChanged;
            comboMonster.EnabledChanged += comboMonster_EnabledChanged;
            // 
            // panel8
            // 
            panel8.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel8.Dock = DockStyle.Top;
            panel8.Location = new Point(0, 69);
            panel8.Name = "panel8";
            panel8.Size = new Size(314, 23);
            panel8.TabIndex = 8;
            // 
            // panel4
            // 
            panel4.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel4.Controls.Add(label3);
            panel4.Controls.Add(comboLocation);
            panel4.Dock = DockStyle.Top;
            panel4.Location = new Point(0, 46);
            panel4.Name = "panel4";
            panel4.Size = new Size(314, 23);
            panel4.TabIndex = 4;
            // 
            // label3
            // 
            label3.BackColor = Color.Transparent;
            label3.Dock = DockStyle.Left;
            label3.ForeColor = Color.White;
            label3.Location = new Point(0, 0);
            label3.Name = "label3";
            label3.Size = new Size(82, 23);
            label3.TabIndex = 2;
            label3.Text = "Location";
            label3.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // comboLocation
            // 
            comboLocation.BackColor = Color.Black;
            comboLocation.Dock = DockStyle.Right;
            comboLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            comboLocation.ForeColor = Color.White;
            comboLocation.FormattingEnabled = true;
            comboLocation.Location = new Point(134, 0);
            comboLocation.Name = "comboLocation";
            comboLocation.Size = new Size(180, 23);
            comboLocation.TabIndex = 2;
            comboLocation.SelectedIndexChanged += comboLocation_SelectedIndexChanged;
            // 
            // panel9
            // 
            panel9.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel9.Dock = DockStyle.Top;
            panel9.Location = new Point(0, 23);
            panel9.Name = "panel9";
            panel9.Size = new Size(314, 23);
            panel9.TabIndex = 9;
            // 
            // EmulatorWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(334, 340);
            Controls.Add(panel2);
            MinimumSize = new Size(350, 100);
            Name = "EmulatorWindow";
            Padding = new Padding(10);
            ShowIcon = false;
            Text = "ToN Parameter Emulator";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            panel2.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ComboBox comboRoundType;
        private Panel panel1;
        private Label label1;
        private Panel panel2;
        private Panel panel3;
        private Label label2;
        private ComboBox comboMonster;
        private Panel panel4;
        private Label label3;
        private ComboBox comboLocation;
        private Panel panel7;
        private ComboBox comboMonster3;
        private Panel panel6;
        private ComboBox comboMonster2;
        private Panel panel8;
        private Panel panel9;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
    }
}