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
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(43, 217);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(67, 290);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "访问地址";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(91, 365);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 24);
            this.label4.TabIndex = 3;
            this.label4.Text = "用户名";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(115, 436);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 24);
            this.label5.TabIndex = 4;
            this.label5.Text = "密码";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(206, 212);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(432, 35);
            this.txtServerName.TabIndex = 5;
            this.txtServerName.Leave += new System.EventHandler(this.txtServerName_Leave);
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Location = new System.Drawing.Point(206, 285);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(298, 35);
            this.txtServerAddress.TabIndex = 6;
            this.txtServerAddress.Leave += new System.EventHandler(this.txtServerAddress_Leave);
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(522, 285);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(116, 35);
            this.txtServerPort.TabIndex = 7;
            this.txtServerPort.Text = "3389";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(206, 360);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(298, 35);
            this.txtUserName.TabIndex = 8;
            this.txtUserName.Text = "administrator";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(206, 431);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(298, 35);
            this.txtPassword.TabIndex = 9;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(306, 513);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 43);
            this.button1.TabIndex = 10;
            this.button1.Text = "保 存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(67, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(106, 24);
            this.label6.TabIndex = 11;
            this.label6.Text = "操作系统";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbtnLinux);
            this.panel1.Controls.Add(this.rbtnWindows);
            this.panel1.Location = new System.Drawing.Point(206, 25);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(298, 71);
            this.panel1.TabIndex = 12;
            // 
            // rbtnLinux
            // 
            this.rbtnLinux.AutoSize = true;
            this.rbtnLinux.Location = new System.Drawing.Point(151, 25);
            this.rbtnLinux.Name = "rbtnLinux";
            this.rbtnLinux.Size = new System.Drawing.Size(101, 28);
            this.rbtnLinux.TabIndex = 1;
            this.rbtnLinux.TabStop = true;
            this.rbtnLinux.Text = "Linux";
            this.rbtnLinux.UseVisualStyleBackColor = true;
            this.rbtnLinux.Click += new System.EventHandler(this.rbtnLinux_Click);
            // 
            // rbtnWindows
            // 
            this.rbtnWindows.AutoSize = true;
            this.rbtnWindows.Location = new System.Drawing.Point(4, 25);
            this.rbtnWindows.Name = "rbtnWindows";
            this.rbtnWindows.Size = new System.Drawing.Size(125, 28);
            this.rbtnWindows.TabIndex = 0;
            this.rbtnWindows.TabStop = true;
            this.rbtnWindows.Text = "Windows";
            this.rbtnWindows.UseVisualStyleBackColor = true;
            this.rbtnWindows.Click += new System.EventHandler(this.rbtnWindows_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(67, 137);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(106, 24);
            this.label7.TabIndex = 13;
            this.label7.Text = "连接方式";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.rbtnSSH);
            this.panel2.Controls.Add(this.rbtnRD);
            this.panel2.Location = new System.Drawing.Point(206, 110);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(298, 71);
            this.panel2.TabIndex = 13;
            // 
            // rbtnSSH
            // 
            this.rbtnSSH.AutoSize = true;
            this.rbtnSSH.Location = new System.Drawing.Point(151, 25);
            this.rbtnSSH.Name = "rbtnSSH";
            this.rbtnSSH.Size = new System.Drawing.Size(89, 28);
            this.rbtnSSH.TabIndex = 1;
            this.rbtnSSH.TabStop = true;
            this.rbtnSSH.Text = "SSH2";
            this.rbtnSSH.UseVisualStyleBackColor = true;
            // 
            // rbtnRD
            // 
            this.rbtnRD.AutoSize = true;
            this.rbtnRD.Location = new System.Drawing.Point(4, 25);
            this.rbtnRD.Name = "rbtnRD";
            this.rbtnRD.Size = new System.Drawing.Size(137, 28);
            this.rbtnRD.TabIndex = 0;
            this.rbtnRD.TabStop = true;
            this.rbtnRD.Text = "远程桌面";
            this.rbtnRD.UseVisualStyleBackColor = true;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(498, 513);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(140, 43);
            this.button2.TabIndex = 14;
            this.button2.Text = "取 消";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // ServerEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 595);
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
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerEditForm";
            this.Text = "服务器编辑";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
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
    }
}