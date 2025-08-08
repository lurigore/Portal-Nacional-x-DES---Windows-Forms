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
            guias = new TabControl();
            tabPage1 = new TabPage();
            textBox3 = new TextBox();
            tomadorSP_IM = new Label();
            processButton2 = new Button();
            button2 = new Button();
            textBox2 = new TextBox();
            tabPage2 = new TabPage();
            openFileDialog1 = new OpenFileDialog();
            versao_DES = new Label();
            versaoSistema = new Label();
            guias.SuspendLayout();
            tabPage1.SuspendLayout();
            tabPage2.SuspendLayout();
            SuspendLayout();
            // 
            // textBox1
            // 
            textBox1.BackColor = SystemColors.ButtonHighlight;
            textBox1.Location = new Point(16, 8);
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(336, 23);
            textBox1.TabIndex = 1;
            // 
            // button1
            // 
            button1.Location = new Point(358, 8);
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
            listBox1.Location = new Point(9, 48);
            listBox1.Name = "listBox1";
            listBox1.Size = new Size(522, 109);
            listBox1.TabIndex = 3;
            // 
            // registros1
            // 
            registros1.AutoSize = true;
            registros1.Location = new Point(9, 160);
            registros1.Name = "registros1";
            registros1.Size = new Size(44, 15);
            registros1.TabIndex = 4;
            registros1.Text = "Total: 0";
            // 
            // processButton1
            // 
            processButton1.Location = new Point(392, 6);
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
            showInscrição.Location = new Point(322, 160);
            showInscrição.Name = "showInscrição";
            showInscrição.RightToLeft = RightToLeft.No;
            showInscrição.Size = new Size(209, 15);
            showInscrição.TabIndex = 6;
            showInscrição.Text = "Inscrição Municipal: X.XXX.XXX/XXX-X";
            // 
            // logButton
            // 
            logButton.Location = new Point(439, 266);
            logButton.Name = "logButton";
            logButton.Size = new Size(86, 23);
            logButton.TabIndex = 7;
            logButton.Text = "Relatórios";
            logButton.UseVisualStyleBackColor = true;
            logButton.Click += logButton_Click;
            // 
            // guias
            // 
            guias.Controls.Add(tabPage1);
            guias.Controls.Add(tabPage2);
            guias.Location = new Point(2, -1);
            guias.Name = "guias";
            guias.SelectedIndex = 0;
            guias.Size = new Size(584, 266);
            guias.TabIndex = 8;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(textBox3);
            tabPage1.Controls.Add(tomadorSP_IM);
            tabPage1.Controls.Add(processButton2);
            tabPage1.Controls.Add(button2);
            tabPage1.Controls.Add(textBox2);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(576, 238);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "SP NFSE";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(9, 66);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(174, 23);
            textBox3.TabIndex = 8;
            // 
            // tomadorSP_IM
            // 
            tomadorSP_IM.AutoSize = true;
            tomadorSP_IM.Location = new Point(6, 48);
            tomadorSP_IM.Name = "tomadorSP_IM";
            tomadorSP_IM.Size = new Size(177, 15);
            tomadorSP_IM.TabIndex = 7;
            tomadorSP_IM.Text = "Inscrição Municipal do Tomador";
            // 
            // processButton2
            // 
            processButton2.Location = new Point(402, 6);
            processButton2.Name = "processButton2";
            processButton2.Size = new Size(139, 36);
            processButton2.TabIndex = 6;
            processButton2.Text = "Processar";
            processButton2.UseVisualStyleBackColor = true;
            processButton2.Click += processButton2_Click;
            // 
            // button2
            // 
            button2.Location = new Point(348, 6);
            button2.Name = "button2";
            button2.Size = new Size(28, 23);
            button2.TabIndex = 3;
            button2.Text = "...";
            button2.UseVisualStyleBackColor = true;
            button2.Click += button2_Click;
            // 
            // textBox2
            // 
            textBox2.BackColor = SystemColors.ButtonHighlight;
            textBox2.Location = new Point(6, 6);
            textBox2.Name = "textBox2";
            textBox2.ReadOnly = true;
            textBox2.Size = new Size(336, 23);
            textBox2.TabIndex = 2;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(processButton1);
            tabPage2.Controls.Add(showInscrição);
            tabPage2.Controls.Add(textBox1);
            tabPage2.Controls.Add(button1);
            tabPage2.Controls.Add(registros1);
            tabPage2.Controls.Add(listBox1);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(576, 238);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Portal Nacional";
            tabPage2.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            openFileDialog1.FileName = "openFileDialog1";
            // 
            // versao_DES
            // 
            versao_DES.AutoSize = true;
            versao_DES.Location = new Point(12, 270);
            versao_DES.Name = "versao_DES";
            versao_DES.Size = new Size(67, 15);
            versao_DES.TabIndex = 10;
            versao_DES.Text = "Versão DES:";
            // 
            // versaoSistema
            // 
            versaoSistema.AutoSize = true;
            versaoSistema.Location = new Point(531, 270);
            versaoSistema.Name = "versaoSistema";
            versaoSistema.Size = new Size(41, 15);
            versaoSistema.TabIndex = 11;
            versaoSistema.Text = "V.0.1.2";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(581, 294);
            Controls.Add(logButton);
            Controls.Add(versaoSistema);
            Controls.Add(versao_DES);
            Controls.Add(guias);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            MaximizeBox = false;
            Name = "Form1";
            Text = "Portal Nacional X DES";
            FormClosing += Form1_FormClosing;
            Load += Form1_Load;
            guias.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            tabPage2.ResumeLayout(false);
            tabPage2.PerformLayout();
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
        private TabControl guias;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button processButton2;
        private Button button2;
        private TextBox textBox2;
        private OpenFileDialog openFileDialog1;
        private Label tomadorSP_IM;
        private TextBox textBox3;
        private Label versao_DES;
        private Label versaoSistema;
    }
}
