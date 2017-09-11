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
    public partial class SFTPForm : Form
    {
        public SFTPForm()
        {
            InitializeComponent();

            FixSplitContainer();
        }

        public RDSServer Server
        {
            get;
            set;
        }

        private void FixSplitContainer()
        {
            LRSplitContainer.SplitterDistance = LRSplitContainer.Width / 2;
        }
    }
}
