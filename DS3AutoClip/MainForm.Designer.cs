namespace DS3AutoClip
{
    partial class MainForm
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
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.mainTimer = new System.Windows.Forms.Timer(this.components);
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.stateLabel = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.logTimer = new System.Windows.Forms.Timer(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.startEventComboBox = new System.Windows.Forms.ComboBox();
            this.stopEventComboBox = new System.Windows.Forms.ComboBox();
            this.startActionComboBox = new System.Windows.Forms.ComboBox();
            this.stopActionComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.targetProcessComboBox = new System.Windows.Forms.ComboBox();
            this.processTimer = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // mainTimer
            // 
            this.mainTimer.Enabled = true;
            this.mainTimer.Tick += new System.EventHandler(this.mainTimer_Tick);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(14, 220);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(447, 60);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Current State:";
            // 
            // stateLabel
            // 
            this.stateLabel.AutoSize = true;
            this.stateLabel.Location = new System.Drawing.Point(131, 9);
            this.stateLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stateLabel.Name = "stateLabel";
            this.stateLabel.Size = new System.Drawing.Size(17, 17);
            this.stateLabel.TabIndex = 2;
            this.stateLabel.Text = "...";
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(295, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "button1";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // logTimer
            // 
            this.logTimer.Enabled = true;
            this.logTimer.Interval = 50;
            this.logTimer.Tick += new System.EventHandler(this.logTimer_Tick);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(95, 61);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Start";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(279, 61);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Stop";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 84);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(60, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "On Event";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 119);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(44, 17);
            this.label5.TabIndex = 7;
            this.label5.Text = "Action";
            // 
            // startEventComboBox
            // 
            this.startEventComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startEventComboBox.FormattingEnabled = true;
            this.startEventComboBox.Items.AddRange(new object[] {
            "A",
            "b",
            "c",
            "d"});
            this.startEventComboBox.Location = new System.Drawing.Point(98, 81);
            this.startEventComboBox.Name = "startEventComboBox";
            this.startEventComboBox.Size = new System.Drawing.Size(160, 25);
            this.startEventComboBox.TabIndex = 8;
            // 
            // stopEventComboBox
            // 
            this.stopEventComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stopEventComboBox.FormattingEnabled = true;
            this.stopEventComboBox.Items.AddRange(new object[] {
            "A",
            "b",
            "c",
            "d"});
            this.stopEventComboBox.Location = new System.Drawing.Point(282, 81);
            this.stopEventComboBox.Name = "stopEventComboBox";
            this.stopEventComboBox.Size = new System.Drawing.Size(160, 25);
            this.stopEventComboBox.TabIndex = 9;
            // 
            // startActionComboBox
            // 
            this.startActionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.startActionComboBox.FormattingEnabled = true;
            this.startActionComboBox.Items.AddRange(new object[] {
            "A",
            "b",
            "c",
            "d"});
            this.startActionComboBox.Location = new System.Drawing.Point(98, 116);
            this.startActionComboBox.Name = "startActionComboBox";
            this.startActionComboBox.Size = new System.Drawing.Size(160, 25);
            this.startActionComboBox.TabIndex = 10;
            // 
            // stopActionComboBox
            // 
            this.stopActionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.stopActionComboBox.FormattingEnabled = true;
            this.stopActionComboBox.Items.AddRange(new object[] {
            "A",
            "b",
            "c",
            "d"});
            this.stopActionComboBox.Location = new System.Drawing.Point(282, 116);
            this.stopActionComboBox.Name = "stopActionComboBox";
            this.stopActionComboBox.Size = new System.Drawing.Size(160, 25);
            this.stopActionComboBox.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 162);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(70, 17);
            this.label6.TabIndex = 12;
            this.label6.Text = "Key Target";
            // 
            // targetProcessComboBox
            // 
            this.targetProcessComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.targetProcessComboBox.FormattingEnabled = true;
            this.targetProcessComboBox.Items.AddRange(new object[] {
            "None"});
            this.targetProcessComboBox.Location = new System.Drawing.Point(98, 159);
            this.targetProcessComboBox.Name = "targetProcessComboBox";
            this.targetProcessComboBox.Size = new System.Drawing.Size(344, 25);
            this.targetProcessComboBox.TabIndex = 13;
            this.targetProcessComboBox.DropDown += new System.EventHandler(this.targetProcessComboBox_DropDown);
            this.targetProcessComboBox.SelectedIndexChanged += new System.EventHandler(this.targetProcessComboBox_SelectedIndexChanged);
            this.targetProcessComboBox.DropDownClosed += new System.EventHandler(this.targetProcessComboBox_DropDownClosed);
            // 
            // processTimer
            // 
            this.processTimer.Enabled = true;
            this.processTimer.Interval = 3000;
            this.processTimer.Tick += new System.EventHandler(this.processTimer_Tick);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(475, 296);
            this.Controls.Add(this.targetProcessComboBox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.stopActionComboBox);
            this.Controls.Add(this.startActionComboBox);
            this.Controls.Add(this.stopEventComboBox);
            this.Controls.Add(this.startEventComboBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.stateLabel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.richTextBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.White;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "DS3 Auto Clip";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Timer mainTimer;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label stateLabel;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Timer logTimer;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox startEventComboBox;
        private System.Windows.Forms.ComboBox stopEventComboBox;
        private System.Windows.Forms.ComboBox startActionComboBox;
        private System.Windows.Forms.ComboBox stopActionComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox targetProcessComboBox;
        private System.Windows.Forms.Timer processTimer;
    }
}

