using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RDManager.Model
{
    /// <summary>
    /// SFTP传输处理状态
    /// </summary>
    public class SFTPProcessStatus
    {
        public Guid ID { get; set; }
        public string SourcePath { get; set; }
        public string SourceFilePath { get; set; }
        public string DestPath { get; set; }
        public string DestFilePath { get; set; }
        public string Direct { get; set; }
        public ulong TotalSize { get; set; }
        public ulong ProcessSize { get; set; }
        public int ProcessStatus { get; set; }
        public string ErrorMessage { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ListViewItem ViewItem { get; set; }
    }
}
