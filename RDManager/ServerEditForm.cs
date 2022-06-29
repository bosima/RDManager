using RDManager.DAL;
using RDManager.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RDManager
{
    public partial class ServerEditForm : Form
    {
        private RDSServer model;

        public ServerEditForm()
        {
            InitializeComponent();

            if (model == null)
            {
                model = new RDSServer();
            }
        }

        public RDSServer Model
        {
            get
            {
                return model;
            }

            set
            {
                model = value;

                if (value != null)
                {
                    txtServerName.Text = Model.ServerName;
                    txtServerAddress.Text = Model.ServerAddress;
                    txtServerPort.Text = Model.ServerPort.ToString();
                    txtUserName.Text = Model.UserName;
                    txtPassword.Text = EncryptUtils.DecryptServerPassword(Model);
                    rbtnLinux.Checked = Model.OpType == "Linux" ? true : false;
                    rbtnWindows.Checked = (Model.OpType == "" || Model.OpType == "Windows") ? true : false;
                    rbtnSSH.Checked = Model.LinkType == "SSH2" ? true : false;
                    rbtnRD.Checked = (Model.LinkType == "" || Model.LinkType == "远程桌面") ? true : false;
                    lblPPKFile.Text = string.IsNullOrWhiteSpace(Model.PrivateKey) ? "未设置" : Model.PrivateKey;
                    txtKeyPassPhrase.Text = Model.KeyPassPhrase;

                    if (!string.IsNullOrWhiteSpace(Model.PrivateKey))
                    {
                        btnRemoveKey.Visible = true;
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Model.ServerID = Model.ServerID == Guid.Empty ? Guid.NewGuid() : Model.ServerID;
            Model.ServerName = txtServerName.Text.Trim();
            Model.ServerAddress = txtServerAddress.Text.Trim();
            Model.UserName = txtUserName.Text.Trim();
            Model.Password = txtPassword.Text.Trim();
            Model.OpType = rbtnWindows.Checked ? rbtnWindows.Text : rbtnLinux.Text;
            Model.LinkType = rbtnRD.Checked ? rbtnRD.Text : rbtnSSH.Text;
            Model.PrivateKey = lblPPKFile.Text == "未设置" ? string.Empty : lblPPKFile.Text;
            Model.KeyPassPhrase = txtKeyPassPhrase.Text;

            int serverPort = 0;
            if (!int.TryParse(txtServerPort.Text.Trim(), out serverPort))
            {
                MessageBox.Show("端口号应该是一个数字！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Model.ServerPort = serverPort;

            if (string.IsNullOrWhiteSpace(Model.ServerName) ||
                string.IsNullOrWhiteSpace(Model.ServerAddress) ||
                string.IsNullOrWhiteSpace(Model.UserName) ||
                (string.IsNullOrWhiteSpace(Model.Password) && string.IsNullOrWhiteSpace(Model.PrivateKey)) ||
                string.IsNullOrWhiteSpace(Model.OpType) ||
                string.IsNullOrWhiteSpace(Model.LinkType))
            {
                MessageBox.Show("请首先填写所有项目！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RDSDataManager dataManager = new RDSDataManager();
            if (!string.IsNullOrWhiteSpace(Model.Password))
            {
                Model.Password = EncryptUtils.EncryptServerPassword(Model);
            }
            if (!string.IsNullOrWhiteSpace(Model.KeyPassPhrase))
            {
                Model.KeyPassPhrase = EncryptUtils.EncryptServerKeyPassPhrase(Model);
            }
            dataManager.SaveServer(Model);

            this.DialogResult = DialogResult.OK;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void rbtnWindows_Click(object sender, EventArgs e)
        {
            rbtnRD.Checked = true;
            txtServerPort.Text = "3389";
            txtUserName.Text = "administrator";
        }

        private void rbtnLinux_Click(object sender, EventArgs e)
        {
            rbtnSSH.Checked = true;
            txtServerPort.Text = "22";
            txtUserName.Text = "root";
        }

        private void txtServerAddress_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerName.Text))
            {
                txtServerName.Text = txtServerAddress.Text;
            }
        }

        private void txtServerName_Leave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtServerAddress.Text))
            {
                txtServerAddress.Text = txtServerName.Text;
            }
        }

        private void btnSelectPPK_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "请选择";
            dialog.Filter = "Privacy Enhanced Mail(*.pem)|*.pem";
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                var ppkFileName = PrivateKeyFileUtils.UploadFile(dialog.FileName);
                lblPPKFile.Text = ppkFileName;
                btnRemoveKey.Visible = true;
            }
        }

        private void btnRemoveKey_Click(object sender, EventArgs e)
        {
            string pemFileName = lblPPKFile.Text;
            if (!string.IsNullOrWhiteSpace(pemFileName))
            {
                PrivateKeyFileUtils.RemoveFile(pemFileName);
            }

            txtKeyPassPhrase.Text = string.Empty;
            lblPPKFile.Text = "未设置";
            btnRemoveKey.Visible = false;
        }
    }
}
