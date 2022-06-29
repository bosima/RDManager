namespace RDManager
{
    partial class ServerEditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbtnLinux = new System.Windows.Forms.RadioButton();
            this.rbtnWindows = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rbtnSSH = new System.Windows.Forms.RadioButton();
            this.rbtnRD = new System.Windows.Forms.RadioButton();
            this.button2 = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.btnSelectPPK = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.lblPPKFile = new System.Windows.Forms.Label();
            this.btnRemoveKey = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtKeyPassPhrase = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(22, 109);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 145);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "访问地址";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(46, 183);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 12);
            this.label4.TabIndex = 3;
            this.label4.Text = "用户名";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(58, 218);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(29, 12);
            this.label5.TabIndex = 4;
            this.label5.Text = "密码";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(103, 106);
            this.txtServerName.Margin = new System.Windows.Forms.Padding(2);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(218, 21);
            this.txtServerName.TabIndex = 5;
            this.txtServerName.Leave += new System.EventHandler(this.txtServerName_Leave);
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Location = new System.Drawing.Point(103, 142);
            this.txtServerAddress.Margin = new System.Windows.Forms.Padding(2);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(151, 21);
            this.txtServerAddress.TabIndex = 6;
            this.txtServerAddress.Leave += new System.EventHandler(this.txtServerAddress_Leave);
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(261, 142);
            this.txtServerPort.Margin = new System.Windows.Forms.Padding(2);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(60, 21);
            this.txtServerPort.TabIndex = 7;
            this.txtServerPort.Text = "3389";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(103, 180);
            this.txtUserName.Margin = new System.Windows.Forms.Padding(2);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(151, 21);
            this.txtUserName.TabIndex = 8;
            this.txtUserName.Text = "administrator";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(103, 215);
            this.txtPassword.Margin = new System.Windows.Forms.Padding(2);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(151, 21);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(162, 406);
            this.button1.Margin = new System.Windows.Forms.Padding(2);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 22);
            this.button1.TabIndex = 10;
            this.button1.Text = "保 存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(34, 26);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 11;
            this.label6.Text = "操作系统";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtnLinux);
            this.panel1.Controls.Add(this.rbtnWindows);
            this.panel1.Location = new System.Drawing.Point(103, 12);
            this.panel1.Margin = new System.Windows.Forms.Padding(2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(149, 36);
            this.panel1.TabIndex = 12;
            // 
            // rbtnLinux
            // 
            this.rbtnLinux.AutoSize = true;
            this.rbtnLinux.Location = new System.Drawing.Point(76, 12);
            this.rbtnLinux.Margin = new System.Windows.Forms.Padding(2);
            this.rbtnLinux.Name = "rbtnLinux";
            this.rbtnLinux.Size = new System.Drawing.Size(53, 16);
            this.rbtnLinux.TabIndex = 1;
            this.rbtnLinux.TabStop = true;
            this.rbtnLinux.Text = "Linux";
            this.rbtnLinux.UseVisualStyleBackColor = true;
            this.rbtnLinux.Click += new System.EventHandler(this.rbtnLinux_Click);
            // 
            // rbtnWindows
            // 
            this.rbtnWindows.AutoSize = true;
            this.rbtnWindows.Location = new System.Drawing.Point(2, 12);
            this.rbtnWindows.Margin = new System.Windows.Forms.Padding(2);
            this.rbtnWindows.Name = "rbtnWindows";
            this.rbtnWindows.Size = new System.Drawing.Size(65, 16);
            this.rbtnWindows.TabIndex = 0;
            this.rbtnWindows.TabStop = true;
            this.rbtnWindows.Text = "Windows";
            this.rbtnWindows.UseVisualStyleBackColor = true;
            this.rbtnWindows.Click += new System.EventHandler(this.rbtnWindows_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(34, 68);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 12);
            this.label7.TabIndex = 13;
            this.label7.Text = "连接方式";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtnSSH);
            this.panel2.Controls.Add(this.rbtnRD);
            this.panel2.Location = new System.Drawing.Point(103, 55);
            this.panel2.Margin = new System.Windows.Forms.Padding(2);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(149, 36);
            this.panel2.TabIndex = 13;
            // 
            // rbtnSSH
            // 
            this.rbtnSSH.AutoSize = true;
            this.rbtnSSH.Location = new System.Drawing.Point(76, 12);
            this.rbtnSSH.Margin = new System.Windows.Forms.Padding(2);
            this.rbtnSSH.Name = "rbtnSSH";
            this.rbtnSSH.Size = new System.Drawing.Size(47, 16);
            this.rbtnSSH.TabIndex = 1;
            this.rbtnSSH.TabStop = true;
            this.rbtnSSH.Text = "SSH2";
            this.rbtnSSH.UseVisualStyleBackColor = true;
            // 
            // rbtnRD
            // 
            this.rbtnRD.AutoSize = true;
            this.rbtnRD.Location = new System.Drawing.Point(2, 12);
            this.rbtnRD.Margin = new System.Windows.Forms.Padding(2);
            this.rbtnRD.Name = "rbtnRD";
            this.rbtnRD.Size = new System.Drawing.Size(71, 16);
            this.rbtnRD.TabIndex = 0;
            this.rbtnRD.TabStop = true;
            this.rbtnRD.Text = "远程桌面";
            this.rbtnRD.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(251, 406);
            this.button2.Margin = new System.Windows.Forms.Padding(2);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 22);
            this.button2.TabIndex = 14;
            this.button2.Text = "取 消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(45, 29);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 15;
            this.label3.Text = "密钥";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            // 
            // btnSelectPPK
            // 
            this.btnSelectPPK.Location = new System.Drawing.Point(91, 24);
            this.btnSelectPPK.Name = "btnSelectPPK";
            this.btnSelectPPK.Size = new System.Drawing.Size(91, 23);
            this.btnSelectPPK.TabIndex = 16;
            this.btnSelectPPK.Text = "选择pem文件";
            this.btnSelectPPK.UseVisualStyleBackColor = true;
            this.btnSelectPPK.Click += new System.EventHandler(this.btnSelectPPK_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(187, 29);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 12);
            this.label8.TabIndex = 17;
            this.label8.Text = "（优先）";
            // 
            // lblPPKFile
            // 
            this.lblPPKFile.AutoSize = true;
            this.lblPPKFile.Location = new System.Drawing.Point(91, 62);
            this.lblPPKFile.Name = "lblPPKFile";
            this.lblPPKFile.Size = new System.Drawing.Size(41, 12);
            this.lblPPKFile.TabIndex = 18;
            this.lblPPKFile.Text = "未设置";
            // 
            // btnRemoveKey
            // 
            this.btnRemoveKey.Location = new System.Drawing.Point(235, 57);
            this.btnRemoveKey.Name = "btnRemoveKey";
            this.btnRemoveKey.Size = new System.Drawing.Size(44, 23);
            this.btnRemoveKey.TabIndex = 20;
            this.btnRemoveKey.Text = "移除";
            this.btnRemoveKey.UseVisualStyleBackColor = true;
            this.btnRemoveKey.Visible = false;
            this.btnRemoveKey.Click += new System.EventHandler(this.btnRemoveKey_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtKeyPassPhrase);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.btnRemoveKey);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.lblPPKFile);
            this.groupBox1.Controls.Add(this.btnSelectPPK);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Location = new System.Drawing.Point(12, 255);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(321, 134);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "密钥对";
            // 
            // txtKeyPassPhrase
            // 
            this.txtKeyPassPhrase.Location = new System.Drawing.Point(91, 92);
            this.txtKeyPassPhrase.Name = "txtKeyPassPhrase";
            this.txtKeyPassPhrase.Size = new System.Drawing.Size(151, 21);
            this.txtKeyPassPhrase.TabIndex = 22;
            this.txtKeyPassPhrase.UseSystemPasswordChar = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(10, 95);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(65, 12);
            this.label9.TabIndex = 21;
            this.label9.Text = "Passphrase";
            // 
            // ServerEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(345, 448);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.txtServerAddress);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerEditForm";
            this.Text = "服务器编辑";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.RadioButton rbtnWindows;
        private System.Windows.Forms.RadioButton rbtnLinux;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.RadioButton rbtnSSH;
        private System.Windows.Forms.RadioButton rbtnRD;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private System.Windows.Forms.Button btnSelectPPK;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label lblPPKFile;
        private System.Windows.Forms.Button btnRemoveKey;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtKeyPassPhrase;
        private System.Windows.Forms.Label label9;
    }
}