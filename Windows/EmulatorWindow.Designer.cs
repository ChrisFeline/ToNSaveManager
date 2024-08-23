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
            mainPanel = new Panel();
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
            checkBoxIsKiller = new CheckBox();
            panel5 = new Panel();
            buttonStepEndRound = new Button();
            panel2 = new Panel();
            buttonStepReveal = new Button();
            panel11 = new Panel();
            buttonStepKillerSet = new Button();
            panel10 = new Panel();
            buttonStepStart = new Button();
            panel1.SuspendLayout();
            mainPanel.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            panel3.SuspendLayout();
            panel4.SuspendLayout();
            panel9.SuspendLayout();
            panel5.SuspendLayout();
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
            // mainPanel
            // 
            mainPanel.AutoSize = true;
            mainPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            mainPanel.Controls.Add(panel7);
            mainPanel.Controls.Add(panel6);
            mainPanel.Controls.Add(panel3);
            mainPanel.Controls.Add(panel8);
            mainPanel.Controls.Add(panel4);
            mainPanel.Controls.Add(panel9);
            mainPanel.Controls.Add(panel1);
            mainPanel.Dock = DockStyle.Top;
            mainPanel.Location = new Point(10, 10);
            mainPanel.Name = "mainPanel";
            mainPanel.Size = new Size(314, 161);
            mainPanel.TabIndex = 3;
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
            panel9.Controls.Add(checkBoxIsKiller);
            panel9.Dock = DockStyle.Top;
            panel9.Location = new Point(0, 23);
            panel9.Name = "panel9";
            panel9.Size = new Size(314, 23);
            panel9.TabIndex = 9;
            // 
            // checkBoxIsKiller
            // 
            checkBoxIsKiller.AutoSize = true;
            checkBoxIsKiller.CheckAlign = ContentAlignment.TopRight;
            checkBoxIsKiller.Dock = DockStyle.Right;
            checkBoxIsKiller.ForeColor = Color.White;
            checkBoxIsKiller.Location = new Point(251, 0);
            checkBoxIsKiller.Name = "checkBoxIsKiller";
            checkBoxIsKiller.Size = new Size(63, 23);
            checkBoxIsKiller.TabIndex = 0;
            checkBoxIsKiller.Text = "Is Killer";
            checkBoxIsKiller.UseVisualStyleBackColor = true;
            // 
            // panel5
            // 
            panel5.AutoSize = true;
            panel5.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel5.Controls.Add(buttonStepEndRound);
            panel5.Controls.Add(panel2);
            panel5.Controls.Add(buttonStepReveal);
            panel5.Controls.Add(panel11);
            panel5.Controls.Add(buttonStepKillerSet);
            panel5.Controls.Add(panel10);
            panel5.Controls.Add(buttonStepStart);
            panel5.Dock = DockStyle.Top;
            panel5.Location = new Point(10, 171);
            panel5.MinimumSize = new Size(0, 10);
            panel5.Name = "panel5";
            panel5.Padding = new Padding(0, 10, 0, 0);
            panel5.Size = new Size(314, 145);
            panel5.TabIndex = 4;
            // 
            // buttonStepEndRound
            // 
            buttonStepEndRound.BackColor = Color.FromArgb(46, 52, 64);
            buttonStepEndRound.Dock = DockStyle.Top;
            buttonStepEndRound.FlatStyle = FlatStyle.Flat;
            buttonStepEndRound.ForeColor = Color.White;
            buttonStepEndRound.Location = new Point(0, 115);
            buttonStepEndRound.Name = "buttonStepEndRound";
            buttonStepEndRound.Size = new Size(314, 30);
            buttonStepEndRound.TabIndex = 3;
            buttonStepEndRound.Text = "End Round";
            buttonStepEndRound.UseVisualStyleBackColor = false;
            buttonStepEndRound.Click += buttonStep_Click;
            // 
            // panel2
            // 
            panel2.AutoSize = true;
            panel2.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel2.Dock = DockStyle.Top;
            panel2.Location = new Point(0, 110);
            panel2.MinimumSize = new Size(0, 5);
            panel2.Name = "panel2";
            panel2.Size = new Size(314, 5);
            panel2.TabIndex = 4;
            // 
            // buttonStepReveal
            // 
            buttonStepReveal.BackColor = Color.FromArgb(46, 52, 64);
            buttonStepReveal.Dock = DockStyle.Top;
            buttonStepReveal.FlatStyle = FlatStyle.Flat;
            buttonStepReveal.ForeColor = Color.White;
            buttonStepReveal.Location = new Point(0, 80);
            buttonStepReveal.Name = "buttonStepReveal";
            buttonStepReveal.Size = new Size(314, 30);
            buttonStepReveal.TabIndex = 2;
            buttonStepReveal.Text = "Reveal";
            buttonStepReveal.UseVisualStyleBackColor = false;
            buttonStepReveal.Click += buttonStep_Click;
            // 
            // panel11
            // 
            panel11.AutoSize = true;
            panel11.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel11.Dock = DockStyle.Top;
            panel11.Location = new Point(0, 75);
            panel11.MinimumSize = new Size(0, 5);
            panel11.Name = "panel11";
            panel11.Size = new Size(314, 5);
            panel11.TabIndex = 6;
            // 
            // buttonStepKillerSet
            // 
            buttonStepKillerSet.BackColor = Color.FromArgb(46, 52, 64);
            buttonStepKillerSet.Dock = DockStyle.Top;
            buttonStepKillerSet.FlatStyle = FlatStyle.Flat;
            buttonStepKillerSet.ForeColor = Color.White;
            buttonStepKillerSet.Location = new Point(0, 45);
            buttonStepKillerSet.Name = "buttonStepKillerSet";
            buttonStepKillerSet.Size = new Size(314, 30);
            buttonStepKillerSet.TabIndex = 1;
            buttonStepKillerSet.Text = "Killer Set";
            buttonStepKillerSet.UseVisualStyleBackColor = false;
            buttonStepKillerSet.Click += buttonStep_Click;
            // 
            // panel10
            // 
            panel10.AutoSize = true;
            panel10.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            panel10.Dock = DockStyle.Top;
            panel10.Location = new Point(0, 40);
            panel10.MinimumSize = new Size(0, 5);
            panel10.Name = "panel10";
            panel10.Size = new Size(314, 5);
            panel10.TabIndex = 5;
            // 
            // buttonStepStart
            // 
            buttonStepStart.BackColor = Color.FromArgb(46, 52, 64);
            buttonStepStart.Dock = DockStyle.Top;
            buttonStepStart.FlatStyle = FlatStyle.Flat;
            buttonStepStart.ForeColor = Color.White;
            buttonStepStart.Location = new Point(0, 10);
            buttonStepStart.Name = "buttonStepStart";
            buttonStepStart.Size = new Size(314, 30);
            buttonStepStart.TabIndex = 0;
            buttonStepStart.Text = "Start";
            buttonStepStart.UseVisualStyleBackColor = false;
            buttonStepStart.Click += buttonStep_Click;
            // 
            // EmulatorWindow
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.FromArgb(46, 52, 64);
            ClientSize = new Size(334, 340);
            Controls.Add(panel5);
            Controls.Add(mainPanel);
            MinimumSize = new Size(350, 100);
            Name = "EmulatorWindow";
            Padding = new Padding(10);
            ShowIcon = false;
            Text = "ToN Parameter Emulator";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            mainPanel.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel4.ResumeLayout(false);
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            panel5.ResumeLayout(false);
            panel5.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private ComboBox comboRoundType;
        private Panel panel1;
        private Label label1;
        private Panel mainPanel;
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
        private Panel panel5;
        private Button buttonStepStart;
        private Button buttonStepKillerSet;
        private Button buttonStepReveal;
        private Button buttonStepEndRound;
        private Panel panel2;
        private Panel panel11;
        private Panel panel10;
        private CheckBox checkBoxIsKiller;
    }
}