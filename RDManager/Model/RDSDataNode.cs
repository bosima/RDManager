using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RDManager.Model
{
    public class RDSDataNode : TreeNode
    {
        public RDSDataNodeType NodeType { get; set; }

        public object RDSData { get; set; }
    }

    public enum RDSDataNodeType
    {
        Group = 1,
        Server = 2
    }
}
