namespace Portal_Nacional_x_DES
{
    partial class Form1
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            folderBrowserDialog1 = new FolderBrowserDialog();
            textBox1 = new TextBox();
            button1 = new Button();
            listBox1 = new ListBox();
            registros1 = new Label();
            processButton1 = new Button();
            showInscrição = new Label();
            logButton = new Button();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.ButtonHighlight;
            textBox1.Location = new Point(12, 12);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(336, 23);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(354, 12);
            button1.Name = "button1";
            button1.Size = new Size(28, 23);
            button1.TabIndex = 2;
            button1.Text = "...";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click;
            // 
            // listBox1
            // 
            listBox1.FormattingEnabled = true;
            listBox1.ItemHeight = 15;
            listBox1.Location = new Point(5, 52);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(522, 109);
            listBox1.TabIndex = 3;
            listBox1.SelectedIndexChanged += listBox1_SelectedIndexChanged;
            // 
            // registros1
            // 
            registros1.AutoSize = true;
            registros1.Location = new Point(5, 164);
            registros1.Name = "registros1";
            registros1.Size = new Size(45, 15);
            registros1.TabIndex = 4;
            registros1.Text = "Total: 0";
            // 
            // processButton1
            // 
            processButton1.Location = new Point(388, 10);
            processButton1.Name = "processButton1";
            processButton1.Size = new Size(139, 36);
            processButton1.TabIndex = 5;
            processButton1.Text = "Processar";
            processButton1.UseVisualStyleBackColor = true;
            processButton1.Click += processButton1_Click;
            // 
            // showInscrição
            // 
            showInscrição.AutoSize = true;
            showInscrição.Location = new Point(318, 164);
            showInscrição.Name = "showInscrição";
            showInscrição.RightToLeft = RightToLeft.No;
            showInscrição.Size = new Size(209, 15);
            showInscrição.TabIndex = 6;
            showInscrição.Text = "Inscrição Municipal: X.XXX.XXX/XXX-X";
            showInscrição.Click += label1_Click_1;
            // 
            // logButton
            // 
            logButton.Location = new Point(449, 205);
            logButton.Name = "logButton";
            logButton.Size = new Size(86, 23);
            logButton.TabIndex = 7;
            logButton.Text = "Relatórios";
            logButton.UseVisualStyleBackColor = true;
            logButton.Click += logButton_Click;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(541, 232);
            Controls.Add(logButton);
            Controls.Add(showInscrição);
            Controls.Add(processButton1);
            Controls.Add(registros1);
            Controls.Add(listBox1);
            Controls.Add(button1);
            Controls.Add(textBox1);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "Portal Nacional X DES";
            Load += Form1_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private FolderBrowserDialog folderBrowserDialog1;
        private TextBox textBox1;
        private Button button1;
        private ListBox listBox1;
        private Label registros1;
        private Button processButton1;
        private Label showInscrição;
        private Button logButton;
    }
}
