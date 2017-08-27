using RDManager.DAL;
using RDManager.Model;
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
                    rbtnWindows.Checked = Model.OpType == "Windows" ? true : false;
                    rbtnLinux.Checked = Model.OpType == "Windows" ? false : true;
                    rbtnRD.Checked = Model.LinkType == "远程桌面" ? true : false;
                    rbtnSSH.Checked = Model.LinkType == "远程桌面" ? false : true;
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
                string.IsNullOrWhiteSpace(Model.Password) ||
                string.IsNullOrWhiteSpace(Model.OpType) ||
                string.IsNullOrWhiteSpace(Model.LinkType))
            {
                MessageBox.Show("请首先填写所有项目！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RDSDataManager dataManager = new RDSDataManager();
            Model.Password = EncryptUtils.EncryptServerPassword(Model);
            dataManager.AddServer(Model);

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
    }
}
