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
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmulatorWindow));
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
            checkSpecial = new CheckBox();
            panel4 = new Panel();
            label3 = new Label();
            comboLocation = new ComboBox();
            panel9 = new Panel();
            checkBoxIsKiller = new CheckBox();
            panel5 = new Panel();
            buttonStepEndRound = new Button();
            buttonStepReveal = new Button();
            buttonStepKillerSet = new Button();
            buttonStepStart = new Button();
            flowLayoutPanel1 = new FlowLayoutPanel();
            buttonDamage = new Button();
            buttonDeath = new Button();
            panel1.SuspendLayout();
            mainPanel.SuspendLayout();
            panel7.SuspendLayout();
            panel6.SuspendLayout();
            panel3.SuspendLayout();
            panel8.SuspendLayout();
            panel4.SuspendLayout();
            panel9.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            SuspendLayout();
            // 
            // comboRoundType
            // 
            comboRoundType.Dock = DockStyle.Right;
            comboRoundType.DropDownStyle = ComboBoxStyle.DropDownList;
            comboRoundType.FormattingEnabled = true;
            comboRoundType.IntegralHeight = false;
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
            comboMonster3.BackColor = Color.FromArgb(40, 0, 0);
            comboMonster3.Dock = DockStyle.Right;
            comboMonster3.DrawMode = DrawMode.OwnerDrawFixed;
            comboMonster3.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMonster3.ForeColor = Color.White;
            comboMonster3.FormattingEnabled = true;
            comboMonster3.IntegralHeight = false;
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
            comboMonster2.BackColor = Color.FromArgb(40, 0, 0);
            comboMonster2.Dock = DockStyle.Right;
            comboMonster2.DrawMode = DrawMode.OwnerDrawFixed;
            comboMonster2.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMonster2.ForeColor = Color.White;
            comboMonster2.FormattingEnabled = true;
            comboMonster2.IntegralHeight = false;
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
            comboMonster.BackColor = Color.FromArgb(40, 0, 0);
            comboMonster.Dock = DockStyle.Right;
            comboMonster.DrawMode = DrawMode.OwnerDrawFixed;
            comboMonster.DropDownStyle = ComboBoxStyle.DropDownList;
            comboMonster.ForeColor = Color.White;
            comboMonster.FormattingEnabled = true;
            comboMonster.IntegralHeight = false;
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
            panel8.Controls.Add(checkSpecial);
            panel8.Dock = DockStyle.Top;
            panel8.Location = new Point(0, 69);
            panel8.Name = "panel8";
            panel8.Size = new Size(314, 23);
            panel8.TabIndex = 8;
            // 
            // checkSpecial
            // 
            checkSpecial.AutoSize = true;
            checkSpecial.CheckAlign = ContentAlignment.TopRight;
            checkSpecial.Dock = DockStyle.Right;
            checkSpecial.ForeColor = Color.White;
            checkSpecial.Location = new Point(138, 0);
            checkSpecial.Name = "checkSpecial";
            checkSpecial.Size = new Size(176, 23);
            checkSpecial.TabIndex = 0;
            checkSpecial.Text = "Special Encounter CheckBox";
            checkSpecial.UseVisualStyleBackColor = true;
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
            comboLocation.IntegralHeight = false;
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
            panel5.Dock = DockStyle.Top;
            panel5.Location = new Point(10, 171);
            panel5.MinimumSize = new Size(0, 10);
            panel5.Name = "panel5";
            panel5.Padding = new Padding(0, 10, 0, 0);
            panel5.Size = new Size(314, 10);
            panel5.TabIndex = 4;
            // 
            // buttonStepEndRound
            // 
            buttonStepEndRound.AutoSize = true;
            buttonStepEndRound.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonStepEndRound.BackColor = Color.FromArgb(40, 0, 0);
            buttonStepEndRound.FlatStyle = FlatStyle.Flat;
            buttonStepEndRound.ForeColor = Color.White;
            buttonStepEndRound.Location = new Point(128, 36);
            buttonStepEndRound.Name = "buttonStepEndRound";
            buttonStepEndRound.Size = new Size(77, 27);
            buttonStepEndRound.TabIndex = 3;
            buttonStepEndRound.Text = "End Round";
            buttonStepEndRound.UseVisualStyleBackColor = false;
            buttonStepEndRound.EnabledChanged += buttonStep_Enabled;
            buttonStepEndRound.Click += buttonStep_Click;
            // 
            // buttonStepReveal
            // 
            buttonStepReveal.AutoSize = true;
            buttonStepReveal.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonStepReveal.BackColor = Color.FromArgb(40, 0, 0);
            buttonStepReveal.FlatStyle = FlatStyle.Flat;
            flowLayoutPanel1.SetFlowBreak(buttonStepReveal, true);
            buttonStepReveal.ForeColor = Color.White;
            buttonStepReveal.Location = new Point(122, 3);
            buttonStepReveal.Name = "buttonStepReveal";
            buttonStepReveal.Size = new Size(53, 27);
            buttonStepReveal.TabIndex = 2;
            buttonStepReveal.Text = "Reveal";
            buttonStepReveal.UseVisualStyleBackColor = false;
            buttonStepReveal.EnabledChanged += buttonStep_Enabled;
            buttonStepReveal.Click += buttonStep_Click;
            // 
            // buttonStepKillerSet
            // 
            buttonStepKillerSet.AutoSize = true;
            buttonStepKillerSet.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonStepKillerSet.BackColor = Color.FromArgb(40, 0, 0);
            buttonStepKillerSet.FlatStyle = FlatStyle.Flat;
            buttonStepKillerSet.ForeColor = Color.White;
            buttonStepKillerSet.Location = new Point(52, 3);
            buttonStepKillerSet.Name = "buttonStepKillerSet";
            buttonStepKillerSet.Size = new Size(64, 27);
            buttonStepKillerSet.TabIndex = 1;
            buttonStepKillerSet.Text = "Killer Set";
            buttonStepKillerSet.UseVisualStyleBackColor = false;
            buttonStepKillerSet.EnabledChanged += buttonStep_Enabled;
            buttonStepKillerSet.Click += buttonStep_Click;
            // 
            // buttonStepStart
            // 
            buttonStepStart.AutoSize = true;
            buttonStepStart.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonStepStart.BackColor = Color.FromArgb(40, 0, 0);
            buttonStepStart.FlatStyle = FlatStyle.Flat;
            buttonStepStart.ForeColor = Color.White;
            buttonStepStart.Location = new Point(3, 3);
            buttonStepStart.Name = "buttonStepStart";
            buttonStepStart.Size = new Size(43, 27);
            buttonStepStart.TabIndex = 0;
            buttonStepStart.Text = "Start";
            buttonStepStart.UseVisualStyleBackColor = false;
            buttonStepStart.EnabledChanged += buttonStep_Enabled;
            buttonStepStart.Click += buttonStep_Click;
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.AutoSize = true;
            flowLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            flowLayoutPanel1.Controls.Add(buttonStepStart);
            flowLayoutPanel1.Controls.Add(buttonStepKillerSet);
            flowLayoutPanel1.Controls.Add(buttonStepReveal);
            flowLayoutPanel1.Controls.Add(buttonDamage);
            flowLayoutPanel1.Controls.Add(buttonDeath);
            flowLayoutPanel1.Controls.Add(buttonStepEndRound);
            flowLayoutPanel1.Dock = DockStyle.Top;
            flowLayoutPanel1.Location = new Point(10, 181);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(314, 66);
            flowLayoutPanel1.TabIndex = 5;
            // 
            // buttonDamage
            // 
            buttonDamage.AutoSize = true;
            buttonDamage.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonDamage.BackColor = Color.FromArgb(40, 0, 0);
            buttonDamage.FlatStyle = FlatStyle.Flat;
            buttonDamage.ForeColor = Color.Red;
            buttonDamage.Location = new Point(3, 36);
            buttonDamage.Name = "buttonDamage";
            buttonDamage.Size = new Size(63, 27);
            buttonDamage.TabIndex = 4;
            buttonDamage.Text = "Damage";
            buttonDamage.UseVisualStyleBackColor = false;
            buttonDamage.EnabledChanged += buttonStep_Enabled;
            buttonDamage.Click += buttonStep_Click;
            // 
            // buttonDeath
            // 
            buttonDeath.AutoSize = true;
            buttonDeath.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            buttonDeath.BackColor = Color.FromArgb(40, 0, 0);
            buttonDeath.FlatStyle = FlatStyle.Flat;
            buttonDeath.ForeColor = Color.Red;
            buttonDeath.Location = new Point(72, 36);
            buttonDeath.Name = "buttonDeath";
            buttonDeath.Size = new Size(50, 27);
            buttonDeath.TabIndex = 5;
            buttonDeath.Text = "Death";
            buttonDeath.UseVisualStyleBackColor = false;
            buttonDeath.EnabledChanged += buttonStep_Enabled;
            buttonDeath.Click += buttonStep_Click;
            // 
            // EmulatorWindow
            // 
            AutoScaleDimensions = new SizeF(96F, 96F);
            AutoScaleMode = AutoScaleMode.Dpi;
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            BackColor = Color.Black;
            ClientSize = new Size(334, 384);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(panel5);
            Controls.Add(mainPanel);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            MinimumSize = new Size(350, 100);
            Name = "EmulatorWindow";
            Padding = new Padding(10);
            Text = "ToN Parameter Emulator";
            Load += Form1_Load;
            panel1.ResumeLayout(false);
            mainPanel.ResumeLayout(false);
            panel7.ResumeLayout(false);
            panel6.ResumeLayout(false);
            panel3.ResumeLayout(false);
            panel8.ResumeLayout(false);
            panel8.PerformLayout();
            panel4.ResumeLayout(false);
            panel9.ResumeLayout(false);
            panel9.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
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
        private CheckBox checkBoxIsKiller;
        private CheckBox checkSpecial;
        private FlowLayoutPanel flowLayoutPanel1;
        private Button buttonDamage;
        private Button buttonDeath;
    }
}