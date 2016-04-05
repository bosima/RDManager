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
                    txtUserName.Text = Model.UserName;
                    txtPassword.Text = Model.Password;
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
                string.IsNullOrWhiteSpace(Model.Password))
            {
                MessageBox.Show("请首先填写所有项目！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RDSDataManager dataManager = new RDSDataManager();
            dataManager.AddServer(Model);

            this.DialogResult = DialogResult.OK;
        }
    }
}
