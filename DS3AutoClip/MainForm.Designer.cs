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
            this.startObsButton = new System.Windows.Forms.Button();
            this.logToggleLabel = new System.Windows.Forms.Label();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // mainTimer
            // 
            this.mainTimer.Enabled = true;
            this.mainTimer.Tick += new System.EventHandler(this.mainTimer_Tick);
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(13, 170);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(424, 106);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            this.richTextBox1.Visible = false;
            // 
            // button1
            // 
            this.button1.ForeColor = System.Drawing.Color.Black;
            this.button1.Location = new System.Drawing.Point(3, 2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "Test";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
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
            this.label2.Location = new System.Drawing.Point(87, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 17);
            this.label2.TabIndex = 4;
            this.label2.Text = "Start";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(271, 9);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(35, 17);
            this.label3.TabIndex = 5;
            this.label3.Text = "Stop";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(13, 32);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(39, 17);
            this.label4.TabIndex = 6;
            this.label4.Text = "Event";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(13, 67);
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
            this.startEventComboBox.Location = new System.Drawing.Point(90, 29);
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
            this.stopEventComboBox.Location = new System.Drawing.Point(274, 29);
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
            this.startActionComboBox.Location = new System.Drawing.Point(90, 64);
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
            this.stopActionComboBox.Location = new System.Drawing.Point(274, 64);
            this.stopActionComboBox.Name = "stopActionComboBox";
            this.stopActionComboBox.Size = new System.Drawing.Size(160, 25);
            this.stopActionComboBox.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(13, 110);
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
            this.targetProcessComboBox.Location = new System.Drawing.Point(90, 107);
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
            // startObsButton
            // 
            this.startObsButton.Enabled = false;
            this.startObsButton.ForeColor = System.Drawing.SystemColors.WindowText;
            this.startObsButton.Location = new System.Drawing.Point(90, 138);
            this.startObsButton.Name = "startObsButton";
            this.startObsButton.Size = new System.Drawing.Size(109, 25);
            this.startObsButton.TabIndex = 14;
            this.startObsButton.Text = "Start OBS";
            this.startObsButton.UseVisualStyleBackColor = true;
            this.startObsButton.Click += new System.EventHandler(this.startObsButton_Click);
            // 
            // logToggleLabel
            // 
            this.logToggleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logToggleLabel.ForeColor = System.Drawing.Color.Gray;
            this.logToggleLabel.Location = new System.Drawing.Point(362, 138);
            this.logToggleLabel.Name = "logToggleLabel";
            this.logToggleLabel.Size = new System.Drawing.Size(72, 25);
            this.logToggleLabel.TabIndex = 15;
            this.logToggleLabel.Text = "Show logs";
            this.logToggleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.logToggleLabel.Click += new System.EventHandler(this.logToggleLabel_Click);
            // 
            // linkLabel1
            // 
            this.linkLabel1.AutoSize = true;
            this.linkLabel1.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(150)))), ((int)(((byte)(255)))));
            this.linkLabel1.Location = new System.Drawing.Point(12, 142);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(35, 17);
            this.linkLabel1.TabIndex = 16;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "Help";
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(24)))), ((int)(((byte)(24)))), ((int)(((byte)(24)))));
            this.ClientSize = new System.Drawing.Size(450, 174);
            this.Controls.Add(this.linkLabel1);
            this.Controls.Add(this.logToggleLabel);
            this.Controls.Add(this.startObsButton);
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
        private System.Windows.Forms.Button startObsButton;
        private System.Windows.Forms.Label logToggleLabel;
        private System.Windows.Forms.LinkLabel linkLabel1;
    }
}

