using RDManager.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RDManager
{
    public partial class InitForm : Form
    {
        private DateTime openTime = DateTime.Now;

        public InitForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text.Trim();
            string secrectKey = txtSecrectKey.Text.Trim();

            if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(secrectKey))
            {
                MessageBox.Show("请输入登录密码和加密密钥！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DateTime now = DateTime.Now;
            var initTime = now.Ticks - openTime.Ticks;
            var initTimeString = initTime.ToString();
            var startChar = initTimeString[0];
            var middleChar = initTimeString[(initTimeString.Length - 1) / 2];
            if (startChar < middleChar)
            {
                startChar = middleChar;
            }

            var startIndex = int.Parse(startChar.ToString());
            startIndex = startIndex == 9 ? 8 : startIndex;
            secrectKey = EncryptUtils.MD5Encrypt(secrectKey);
            var passSecrectKey = secrectKey.Substring(startIndex, 24);
            var encryptPassword = EncryptUtils.DES3Encrypt(password, passSecrectKey);

            RDSDataManager manager = new RDSDataManager();
            manager.SetSecrectKey(secrectKey);
            manager.SetInitTime(initTimeString);
            manager.SetPassword(encryptPassword);

            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
            this.Close();
        }
    }
}
