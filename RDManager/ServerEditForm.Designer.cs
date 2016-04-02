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
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtServerName = new System.Windows.Forms.TextBox();
            this.txtServerAddress = new System.Windows.Forms.TextBox();
            this.txtServerPort = new System.Windows.Forms.TextBox();
            this.txtUserName = new System.Windows.Forms.TextBox();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(39, 56);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(130, 24);
            this.label1.TabIndex = 0;
            this.label1.Text = "服务器名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(63, 117);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "访问地址";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(111, 179);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 24);
            this.label3.TabIndex = 2;
            this.label3.Text = "端口";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(87, 240);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 24);
            this.label4.TabIndex = 3;
            this.label4.Text = "用户名";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(111, 297);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 24);
            this.label5.TabIndex = 4;
            this.label5.Text = "密码";
            // 
            // txtServerName
            // 
            this.txtServerName.Location = new System.Drawing.Point(202, 50);
            this.txtServerName.Name = "txtServerName";
            this.txtServerName.Size = new System.Drawing.Size(432, 35);
            this.txtServerName.TabIndex = 5;
            // 
            // txtServerAddress
            // 
            this.txtServerAddress.Location = new System.Drawing.Point(202, 112);
            this.txtServerAddress.Name = "txtServerAddress";
            this.txtServerAddress.Size = new System.Drawing.Size(432, 35);
            this.txtServerAddress.TabIndex = 6;
            // 
            // txtServerPort
            // 
            this.txtServerPort.Location = new System.Drawing.Point(202, 174);
            this.txtServerPort.Name = "txtServerPort";
            this.txtServerPort.Size = new System.Drawing.Size(146, 35);
            this.txtServerPort.TabIndex = 7;
            this.txtServerPort.Text = "3389";
            // 
            // txtUserName
            // 
            this.txtUserName.Location = new System.Drawing.Point(202, 233);
            this.txtUserName.Name = "txtUserName";
            this.txtUserName.Size = new System.Drawing.Size(298, 35);
            this.txtUserName.TabIndex = 8;
            this.txtUserName.Text = "administrator";
            // 
            // txtPassword
            // 
            this.txtPassword.Location = new System.Drawing.Point(202, 292);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(298, 35);
            this.txtPassword.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(494, 370);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(140, 43);
            this.button1.TabIndex = 10;
            this.button1.Text = "保 存";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ServerEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(12F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(690, 451);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.txtUserName);
            this.Controls.Add(this.txtServerPort);
            this.Controls.Add(this.txtServerAddress);
            this.Controls.Add(this.txtServerName);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ServerEditForm";
            this.Text = "服务器编辑";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtServerName;
        private System.Windows.Forms.TextBox txtServerAddress;
        private System.Windows.Forms.TextBox txtServerPort;
        private System.Windows.Forms.TextBox txtUserName;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button button1;
    }
}