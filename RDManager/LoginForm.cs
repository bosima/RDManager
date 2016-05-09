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

            var secrectPassword = new RDSDataManager().GetPassword();
            var encryptPassword = EncryptUtils.EncryptPassword(password);

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

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.button2_Click(sender, e);
            }
        }
    }
}
