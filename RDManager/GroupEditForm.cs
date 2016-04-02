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
    public partial class GroupEditForm : Form
    {
        public GroupEditForm()
        {
            InitializeComponent();

            if (Model == null)
            {
                Model = new RDSGroup();
            }
        }

        public RDSGroup Model
        {
            get;
            set;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Model.GroupID = Guid.NewGuid();
            Model.GroupName = txtGroupName.Text;

            if (string.IsNullOrWhiteSpace(Model.GroupName))
            {
                MessageBox.Show("请首先填写组名称！", "警告", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RDSDataManager dataManager = new RDSDataManager();
            dataManager.AddGroup(Model);

            this.DialogResult = DialogResult.OK;
        }
    }
}
