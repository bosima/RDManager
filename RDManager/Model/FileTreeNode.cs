using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDManager
{
    /// <summary>
    /// 文件树节点
    /// </summary>
    internal class FileTreeNode
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Type { get; set; }
        public ulong Size { get; set; }
        public List<FileTreeNode> Children { get; set; }
    }
}
