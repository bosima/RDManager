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
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string password = txtPassword.Text.Trim();

            RDSDataManager manager = new RDSDataManager();
            var initTimeString = manager.GetInitTime();
            var startChar = initTimeString[0];
            var middleChar = initTimeString[(initTimeString.Length - 1) / 2];
            if (startChar < middleChar)
            {
                startChar = middleChar;
            }

            var startIndex = int.Parse(startChar.ToString());
            startIndex = startIndex == 9 ? 8 : startIndex;
            var secrectKey = manager.GetSecrectKey();
            var secrectPassword = manager.GetPassword();
            var passSecrectKey = secrectKey.Substring(startIndex, 24);
            var encryptPassword = EncryptUtils.DES3Encrypt(password, passSecrectKey);

            if (encryptPassword != secrectPassword)
            {
                MessageBox.Show("密码不正确，请重新输入！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.Hide();
            MainForm mainForm = new MainForm();
            mainForm.ShowDialog();
            this.Close();
        }
    }
}
