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
    public partial class DirectoryNameEditForm : Form
    {
        public DirectoryNameEditForm()
        {
            InitializeComponent();
        }

        public string DirectoryName { get; set; }

        private void btnSave_Click(object sender, EventArgs e)
        {
            DirectoryName = txtDirectoryName.Text;
            DialogResult = DialogResult.OK;
        }
    }
}
