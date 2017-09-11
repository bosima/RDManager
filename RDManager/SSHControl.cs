using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using RDManager.Model;
using WalburySoftware;
using System.IO;
using Renci.SshNet;
using System.Threading.Tasks;
using Renci.SshNet.Sftp;

namespace RDManager
{
    public partial class SSHControl : UserControl
    {
        /// <summary>
        /// 服务器信息
        /// </summary>
        private RDSServer _rdsServer;

        /// <summary>
        /// 初始化SSHControl的一个新实例
        /// </summary>
        /// <param name="server"></param>
        public SSHControl(RDSServer server)
        {
            this._rdsServer = server;
            this._driveList = FileOperationUtil.GetDrives();

            InitializeComponent();
            InitSFTPView();
        }

        #region Event
        public delegate void ConnectedEventHandle(object sneder, EventArgs args);
        public delegate void DisonnectedEventHandle(object sneder, EventArgs args);

        public event ConnectedEventHandle OnConnected;
        public event DisonnectedEventHandle OnDisconnected;
        #endregion

        /// <summary>
        /// 是否已建立连接
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (this._shell != null)
                {
                    return this._shell.IsConnected;
                }

                return false;
            }
        }

        /// <summary>
        /// 连接
        /// </summary>
        public void Connect()
        {
            ConnectShell();
            ConnectSFTP();
        }

        /// <summary>
        /// 断开连接
        /// </summary>
        public void Disconnect()
        {
            DisconnectShell();
            DisconnectSFTP();
        }

        /// <summary>
        /// 执行连接后处理
        /// </summary>
        /// <param name="linkType"></param>
        private void DoAfterConnected(int linkType)
        {
            OnConnected.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// 执行连接断开后处理
        /// </summary>
        /// <param name="linkType"></param>
        private void DoAfterDisonnected(int linkType)
        {
            // 只要一个断开，全部断开
            if (linkType == 1)
            {
                DisconnectSFTP();
            }
            else
            {
                DisconnectShell();
            }

            OnDisconnected.Invoke(this, new EventArgs());
        }

        #region Shell
        /// <summary>
        /// Shell控件
        /// </summary>
        private TerminalControl _shell;

        /// <summary>
        /// Shell是否已经连接
        /// </summary>
        public bool IsShellConnected
        {
            get
            {
                if (_shell != null)
                {
                    return _shell.IsConnected;
                }

                return false;
            }
        }

        /// <summary>
        /// 连接Shell
        /// </summary>
        /// <param name="server"></param>
        private void ConnectShell()
        {
            if (_shell != null)
            {
                _shell.Tag = _rdsServer.ServerID.ToString();
                _shell.UserName = _rdsServer.UserName;
                _shell.Password = EncryptUtils.DecryptServerPassword(_rdsServer);
                _shell.Host = _rdsServer.ServerAddress;
                _shell.Port = _rdsServer.ServerPort;

                if (!_shell.IsConnected)
                {
                    _shell.Connect();
                    _shell.Focus();
                }
            }
            else
            {
                _shell = new TerminalControl(_rdsServer.UserName, EncryptUtils.DecryptServerPassword(_rdsServer), _rdsServer.ServerAddress, _rdsServer.ServerPort);
                _shell.Tag = _rdsServer.ServerID.ToString();
                _shell.BackColor = System.Drawing.Color.Teal;
                _shell.ForeColor = System.Drawing.Color.Snow;
                _shell.Dock = System.Windows.Forms.DockStyle.Fill;
                _shell.Font = new Font("Courier New", 10);
                _shell.OnConnected += Shell_OnConnected;
                _shell.OnDisconnected += Shell_OnDisconnected;
                shellTab.Controls.Add(_shell);

                _shell.Connect();
                _shell.Focus();
            }
        }

        /// <summary>
        /// 断开Shell
        /// </summary>
        private void DisconnectShell()
        {
            if (_shell != null && _shell.IsConnected)
            {
                _shell.Disconnect();
            }
        }

        /// <summary>
        /// Shell连接成功触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shell_OnConnected(object sender, EventArgs e)
        {
            DoAfterConnected(1);
        }

        /// <summary>
        /// Shell连接断开触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shell_OnDisconnected(object sender, EventArgs e)
        {
            if (IsSFTPConnected || IsSFTPConnecting)
            {
                DisconnectSFTP();
            }

            DoAfterDisonnected(1);
        }
        #endregion

        #region SFTP
        /// <summary>
        /// 文件图标列表
        /// </summary>
        private ImageList _fileIconList = new ImageList();

        /// <summary>
        /// SFTP路径分隔符
        /// </summary>
        private string _sFTPPathSeparator = "/";

        /// <summary>
        /// 文件图标字典：后缀名>图标在_fileIconList中的索引
        /// </summary>
        private Dictionary<string, int> _fileIconIndexDic = new Dictionary<string, int>();

        /// <summary>
        /// SFTP客户端
        /// </summary>
        private SftpClient _sftpClient;

        /// <summary>
        /// SFTP连接状态
        /// </summary>
        private int _sFTPConnectStatus = -1;

        /// <summary>
        /// SFTP是否正在连接
        /// </summary>
        public bool IsSFTPConnecting
        {
            get
            {
                if (_sFTPConnectStatus == 0)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// SFTP是否已经连接
        /// </summary>
        public bool IsSFTPConnected
        {
            get
            {
                if (_sFTPConnectStatus == 1)
                {
                    return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 初始化SFTP视图
        /// </summary>
        private void InitSFTPView()
        {
            FixSplitContainer();
            InitListView();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void InitListView()
        {
            InitListViewHeader();
            InitLocalFileList();
            InitLocalListRightButtonMemnu();
        }

        /// <summary>
        /// 获取文件图标索引
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        private int GetFileIconIndex(string fileExtension)
        {
            if (!_fileIconIndexDic.ContainsKey(fileExtension))
            {
                Icon icon;
                if (fileExtension == "directory")
                {
                    icon = FileIconUtil.GetDirectoryIcon(false);
                }
                else if (fileExtension == "drive")
                {
                    icon = FileIconUtil.GetDriverIcon(_driveList[0].Name[0], true);
                }
                else
                {
                    icon = FileIconUtil.GetIcon(fileExtension, false);
                }

                _fileIconList.Images.Add(icon);
                _fileIconIndexDic.Add(fileExtension, _fileIconList.Images.Count - 1);
            }

            return _fileIconIndexDic[fileExtension];
        }

        /// <summary>
        /// 初始化ListView表头
        /// </summary>
        private void InitListViewHeader()
        {
            this.localListView.Columns.Add("文件名", 200, HorizontalAlignment.Left);
            this.localListView.Columns.Add("文件大小", 100, HorizontalAlignment.Right);
            this.localListView.Columns.Add("文件类型", 80, HorizontalAlignment.Left);
            this.localListView.Columns.Add("修改时间", 130, HorizontalAlignment.Left);

            this.remoteListView.Columns.Add("文件名", 200, HorizontalAlignment.Left);
            this.remoteListView.Columns.Add("文件大小", 100, HorizontalAlignment.Right);
            this.remoteListView.Columns.Add("文件类型", 80, HorizontalAlignment.Left);
            this.remoteListView.Columns.Add("修改时间", 130, HorizontalAlignment.Left);

            this.statusListView.Columns.Add("源文件", 300, HorizontalAlignment.Left);
            this.statusListView.Columns.Add("方向", 80, HorizontalAlignment.Center);
            this.statusListView.Columns.Add("目标文件", 300, HorizontalAlignment.Left);
            this.statusListView.Columns.Add("大小", 100, HorizontalAlignment.Left);
            this.statusListView.Columns.Add("状态", 100, HorizontalAlignment.Left);

            this.localListView.SmallImageList = _fileIconList;
            this.remoteListView.SmallImageList = _fileIconList;
        }

        /// <summary>
        /// 修正拆分器的位置
        /// </summary>
        private void FixSplitContainer()
        {
            LRSplitContainer.SplitterDistance = LRSplitContainer.Width / 2;
            FSSplitContainer.SplitterDistance = FSSplitContainer.Height - 100;
        }

        /// <summary>
        /// 检查SFTP连接状态
        /// </summary>
        /// <returns></returns>
        private bool CheckSFTPConnected()
        {
            try
            {
                return _sftpClient.IsConnected;
            }
            catch (ObjectDisposedException)
            {
            }

            return false;
        }

        /// <summary>
        /// 连接SFTP
        /// </summary>
        private void ConnectSFTP()
        {
            Task.Factory.StartNew(() =>
            {
                // connecting
                _sFTPConnectStatus = 0;

                var connectionInfo = new ConnectionInfo(_rdsServer.ServerAddress,
                                       _rdsServer.UserName,
                                       new PasswordAuthenticationMethod(_rdsServer.UserName, EncryptUtils.DecryptServerPassword(_rdsServer))
                                       );

                _sftpClient = new SftpClient(connectionInfo);
                _sftpClient.KeepAliveInterval = new TimeSpan(0, 1, 0);
                _sftpClient.ErrorOccurred += SFTP_OnErrorOccurred;
                _sftpClient.Connect();


                SFTP_OnConnected(this, new EventArgs());
            });
        }

        /// <summary>
        /// 断开SFTP
        /// </summary>
        private void DisconnectSFTP()
        {
            _sFTPConnectStatus = -2; // disconnecting

            if (_sftpClient != null)
            {
                try
                {
                    if (_sftpClient.IsConnected)
                    {
                        _sftpClient.Disconnect();
                    }
                }
                catch (ObjectDisposedException)
                {
                }

                _sftpClient.Dispose();
            }

            SFTP_OnDisconnected(this, new EventArgs());
        }

        /// <summary>
        /// SFTP连接成功触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SFTP_OnConnected(object sender, EventArgs e)
        {
            // connected
            _sFTPConnectStatus = 1;

            this.Invoke(new Action(() =>
            {
                InitRemoteFileList();
            }));

            // do nothing
        }

        /// <summary>
        /// SFTP连接断开触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SFTP_OnDisconnected(object sender, EventArgs e)
        {
            _sFTPConnectStatus = -1; // disconnected

            DoAfterDisonnected(2);
        }

        /// <summary>
        /// SFTP错误发生时
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SFTP_OnErrorOccurred(object sender, Renci.SshNet.Common.ExceptionEventArgs e)
        {
        }
        #endregion

        #region SFTP Local List
        /// <summary>
        /// 本地文件列表右键菜单
        /// </summary>
        private ContextMenuStrip _localContextMenuStrip;

        /// <summary>
        /// 当前本地目录选项
        /// </summary>
        private ListViewItem _currentLocalSelectedItem;

        /// <summary>
        /// SFTP本地文件路径
        /// </summary>
        private string _sftpLocalPath = string.Empty;

        /// <summary>
        /// 逻辑磁盘列表
        /// </summary>
        private List<DriveInfo> _driveList;

        /// <summary>
        /// 初始化本地文件列表
        /// </summary>
        private void InitLocalFileList()
        {
            var systemPath = System.Environment.SystemDirectory;
            var defaultDisk = systemPath.Substring(0, 3);
            SetLocalSubFileList(defaultDisk);
        }

        /// <summary>
        /// 设置本地下级文件目录
        /// </summary>
        /// <param name="currentPath"></param>
        private void SetLocalSubFileList(string currentPath)
        {
            _sftpLocalPath = currentPath;

            // TODO:获取系统管理员权限
            DirectoryInfo dir = new DirectoryInfo(currentPath);
            var fsArray = dir.GetFileSystemInfos();
            SetLocalFileList(fsArray, currentPath);
        }

        /// <summary>
        /// 设置本地文件列表
        /// </summary>
        /// <param name="fsArray"></param>
        private void SetLocalFileList(FileSystemInfo[] fsArray, string currentPath)
        {
            string parentPath = GetWindowsParentPath(currentPath);

            this.localListView.BeginUpdate();

            this.localListView.Items.Clear();

            if (!string.IsNullOrWhiteSpace(parentPath))
            {
                ListViewItem lviParent = new ListViewItem();
                lviParent.Tag = "directory|" + parentPath;
                lviParent.Text = "..";
                lviParent.ImageIndex = GetFileIconIndex("directory");
                this.localListView.Items.Add(lviParent);
            }

            if (fsArray.Length > 0)
            {
                foreach (var f in fsArray)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Tag = (f is DirectoryInfo ? "directory" : "file") + "|" + f.FullName;

                    if (f is DirectoryInfo)
                    {
                        lvi.Text = f.Name;
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("文件夹");
                        lvi.SubItems.Add(f.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"));
                        lvi.ImageIndex = GetFileIconIndex("directory");
                    }
                    else if (f is FileInfo)
                    {
                        var dInfo = (FileInfo)f;

                        var ext = f.Extension;
                        if (string.IsNullOrWhiteSpace(ext))
                        {
                            ext = "文件";
                        }

                        lvi.Text = dInfo.Name;
                        lvi.SubItems.Add(dInfo.Length.ToString());
                        lvi.SubItems.Add(ext);
                        lvi.SubItems.Add(f.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"));
                        lvi.ImageIndex = GetFileIconIndex(ext);
                    }

                    this.localListView.Items.Add(lvi);
                }
            }

            this.localListView.EndUpdate();
        }

        /// <summary>
        /// 设置本地磁盘列表
        /// </summary>
        /// <param name="fsArray"></param>
        private void SetLocalDriveList()
        {
            this.localListView.BeginUpdate();
            this.localListView.Items.Clear();

            if (_driveList.Count > 0)
            {
                foreach (var f in _driveList)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Tag = f.Name;
                    lvi.Text = f.Name;
                    lvi.SubItems.Add("");
                    lvi.SubItems.Add("逻辑磁盘");
                    lvi.ImageIndex = GetFileIconIndex("drive");

                    this.localListView.Items.Add(lvi);
                }
            }

            this.localListView.EndUpdate();
        }

        /// <summary>
        /// 获取Windows上级目录
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        private string GetWindowsParentPath(string currentPath)
        {
            var parentPath = string.Empty;

            if (currentPath.IndexOf("\\") < currentPath.Length - 2)  // 不是磁盘根目录，则截取
            {
                parentPath = currentPath.Substring(0, currentPath.LastIndexOf("\\"));

                if (parentPath.IndexOf("\\") == -1)
                {
                    parentPath += "\\";
                }
            }
            else if (_driveList.Where(d => d.Name == currentPath).Any())  // currentPath为磁盘根目录，则上级目录设置为drive
            {
                parentPath = "drive";
            }
            else if (currentPath == "drive")  // currentPath为磁盘列表，则上级目录设置为空
            {
                parentPath = "";
            }

            return parentPath;
        }

        /// <summary>
        /// 初始化本地右键按钮
        /// </summary>
        private void InitLocalListRightButtonMemnu()
        {
            ToolStripMenuItem btnLocalListUploadItem = new ToolStripMenuItem();
            btnLocalListUploadItem.Name = "btnLocalListUploadItem";
            btnLocalListUploadItem.Text = "上传";
            btnLocalListUploadItem.Click += BtnLocalListUploadItem_Click;

            ToolStripMenuItem btnLocalListRefreshItem = new ToolStripMenuItem();
            btnLocalListRefreshItem.Name = "btnLocalListRefreshItem";
            btnLocalListRefreshItem.Text = "刷新";
            btnLocalListRefreshItem.Click += BtnLocalListRefreshItem_Click;

            ToolStripMenuItem btnLocalListNewFolderItem = new ToolStripMenuItem();
            btnLocalListNewFolderItem.Name = "btnLocalListNewFolderItem";
            btnLocalListNewFolderItem.Text = "新建文件夹";
            btnLocalListNewFolderItem.Click += BtnLocalListNewFolderItem_Click;

            ToolStripMenuItem btnLocalListDeleteItem = new ToolStripMenuItem();
            btnLocalListDeleteItem.Name = "btnLocalListDeleteItem";
            btnLocalListDeleteItem.Text = "删除";
            btnLocalListDeleteItem.Click += BtnLocalListDeleteItem_Click;


            _localContextMenuStrip = new ContextMenuStrip();
            _localContextMenuStrip.Items.Add(btnLocalListUploadItem);
            _localContextMenuStrip.Items.Add(btnLocalListRefreshItem);
            _localContextMenuStrip.Items.Add(btnLocalListNewFolderItem);
            _localContextMenuStrip.Items.Add(btnLocalListDeleteItem);

            localListView.ContextMenuStrip = _localContextMenuStrip;
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLocalListNewFolderItem_Click(object sender, EventArgs e)
        {
            DirectoryNameEditForm form = new DirectoryNameEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var directoryName = form.DirectoryName;
                var directory = Path.Combine(_sftpLocalPath, directoryName);

                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    RefreshLocalListView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("新建文件夹失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLocalListDeleteItem_Click(object sender, EventArgs e)
        {
            if (_currentLocalSelectedItem != null)
            {
                var itemArray = _currentLocalSelectedItem.Tag.ToString().Split('|');
                var itemType = itemArray[0];
                var itemPath = itemArray[1];

                try
                {
                    if (itemType == "directory")
                    {
                        // TODO:递归删除

                        if (Directory.Exists(itemPath))
                        {
                            Directory.Delete(itemPath);
                        }
                    }
                    else if (itemType == "file")
                    {
                        if (File.Exists(itemPath))
                        {
                            File.Delete(itemPath);
                        }
                    }

                    RefreshLocalListView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("删除失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 右键上传处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLocalListUploadItem_Click(object sender, EventArgs e)
        {
            if (_currentLocalSelectedItem != null)
            {
                UploadLocalListItem(_currentLocalSelectedItem);
            }
        }

        /// <summary>
        /// 上传本地文件列表项目
        /// </summary>
        /// <param name="listItem"></param>
        private void UploadLocalListItem(ListViewItem listItem)
        {
            var itemArray = listItem.Tag.ToString().Split('|');
            var itemType = itemArray[0];
            var itemPath = itemArray[1];

            if (itemType == "file")
            {
                var localFilePath = itemPath;
                var fileName = listItem.Text;
                var fileSize = ulong.Parse(listItem.SubItems[1].Text);
                AsyncUploadFile(localFilePath, fileName, fileSize);
            }

            if (itemType == "directory")
            {
                UploadLocalDirectory(listItem.Text, itemPath);
            }
        }

        /// <summary>
        /// 上传本地文件夹
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="fullPath"></param>
        private void UploadLocalDirectory(string directoryName, string fullPath)
        {
            // 递归获取一个树形结构
            var rootNode = new FileTreeNode()
            {
                Name = directoryName,
                Path = fullPath,
                Type = "directory"
            };

            try
            {
                MakeDirectoryTree(rootNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本地目录结构失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 递归创建文件夹和上传文件
            RecursionUploadLocalDirectory(rootNode, _sftpRemotePath);
        }

        /// <summary>
        /// 递归文件树上传本地目录
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="remoteUploadPath"></param>
        private void RecursionUploadLocalDirectory(FileTreeNode treeNode, string remoteUploadPath)
        {
            // 如果目录不存在，则首先创建
            if (treeNode.Type == "directory")
            {
                var currentUploadPath = remoteUploadPath + _sFTPPathSeparator + treeNode.Name;

                if (!_sftpClient.Exists(currentUploadPath))
                {
                    _sftpClient.CreateDirectory(currentUploadPath);
                }

                if (treeNode.Children != null && treeNode.Children.Count > 0)
                {
                    foreach (var subNode in treeNode.Children)
                    {
                        RecursionUploadLocalDirectory(subNode, currentUploadPath);
                    }
                }
            }

            if (treeNode.Type == "file")
            {
                AsyncUploadFile(treeNode.Path, treeNode.Name, remoteUploadPath, treeNode.Size);
            }
        }

        /// <summary>
        /// 根据根节点生成一个目录树
        /// </summary>
        /// <param name="tree"></param>
        private void MakeDirectoryTree(FileTreeNode tree)
        {
            DirectoryInfo dir = new DirectoryInfo(tree.Path);
            var fsArray = dir.GetFileSystemInfos();
            if (fsArray.Length > 0)
            {
                tree.Children = new List<FileTreeNode>();

                foreach (var fs in fsArray)
                {
                    if (fs is DirectoryInfo)
                    {
                        var node = new FileTreeNode()
                        {
                            Name = fs.Name,
                            Path = fs.FullName,
                            Type = "directory"
                        };

                        tree.Children.Add(node);
                        MakeDirectoryTree(node);
                    }

                    if (fs is FileInfo)
                    {
                        var f = (FileInfo)fs;
                        tree.Children.Add(new FileTreeNode()
                        {
                            Name = fs.Name,
                            Path = fs.FullName,
                            Size = (ulong)f.Length,
                            Type = "file"
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLocalListRefreshItem_Click(object sender, EventArgs e)
        {
            RefreshLocalListView();
        }

        /// <summary>
        /// 本地目录右键处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _currentLocalSelectedItem = localListView.GetItemAt(e.X, e.Y);
            }
        }

        /// <summary>
        /// 本地目录鼠标双击处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = localListView.HitTest(e.X, e.Y);
            if (info.Item != null)
            {
                var item = info.Item as ListViewItem;
                var pathInfo = item.Tag.ToString().Split('|');
                var pathType = pathInfo[0];
                var path = pathInfo[1];

                // 磁盘目录
                if (path == "drive")
                {
                    SetLocalDriveList();
                    return;
                }

                // 打开下级目录
                if (pathType == "directory")
                {
                    SetLocalSubFileList(path);
                    return;
                }

                // 如果连接已经建立，上传文件
                if (pathType == "file")
                {
                    var fSize = ulong.Parse(item.SubItems[1].Text);
                    AsyncUploadFile(path, item.Text, fSize);
                }
            }
        }

        /// <summary>
        /// 本地目录排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 默认非倒序
            if (localListView.Columns[e.Column].Tag == null)
            {
                localListView.Columns[e.Column].Tag = false;
            }
            var tabK = (bool)localListView.Columns[e.Column].Tag;
            localListView.Columns[e.Column].Tag = !tabK;
            localListView.ListViewItemSorter = new ListViewSort(e.Column, localListView.Columns[e.Column].Tag);

            localListView.Sort();
        }

        /// <summary>
        /// 异步上传文件到指定远程文件夹
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="localFileName"></param>
        /// <param name="remoteDirectoryPath"></param>
        /// <param name="fileSize"></param>
        private void AsyncUploadFile(string localFilePath, string localFileName, string remoteDirectoryPath, ulong fileSize)
        {
            var remoteFilePath = remoteDirectoryPath + _sFTPPathSeparator + localFileName;

            Task.Factory.StartNew(() =>
            {
                SFTPProcessStatus status = new SFTPProcessStatus()
                {
                    ID = Guid.NewGuid(),
                    DestPath = remoteDirectoryPath,
                    DestFilePath = remoteFilePath,
                    Direct = "upload",
                    SourceFilePath = localFilePath,
                    TotalSize = fileSize,
                    StartTime = DateTime.Now,
                    ProcessStatus = 0
                };

                try
                {
                    using (var file = File.Open(localFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        _sftpClient.UploadFile(file, remoteFilePath, (size) =>
                        {
                            // 上传进度
                            status.ProcessSize = size;

                            // 上传完毕
                            if (size == fileSize)
                            {
                                status.ProcessStatus = 1;
                                status.EndTime = DateTime.Now;
                                DoUploadFileCompleted(status);
                            }

                            UpdateSFTPStatus(status);
                        });
                    }
                }
                catch (Exception ex)
                {
                    status.ErrorMessage = ex.Message;
                    status.ProcessStatus = -1;
                    UpdateSFTPStatus(status);
                }
            });
        }

        /// <summary>
        /// 异步上传文件到当前远程文件夹
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="remoteFileName"></param>
        private void AsyncUploadFile(string localFilePath, string localFileName, ulong fileSize)
        {
            AsyncUploadFile(localFilePath, localFileName, _sftpRemotePath, fileSize);
        }

        /// <summary>
        /// 执行上传文件完成
        /// </summary>
        /// <param name="status"></param>
        private void DoUploadFileCompleted(SFTPProcessStatus status)
        {
            if (_sftpRemotePath == status.DestPath)
            {
                UpdateRemoteList(status.DestPath);
            }
        }

        /// <summary>
        /// 刷新本地文件列表
        /// </summary>
        /// <param name="remotePath"></param>
        private void RefreshLocalListView()
        {
            if (localListView.InvokeRequired)
            {
                localListView.Invoke(new Action(() =>
                {
                    SetLocalSubFileList(_sftpLocalPath);
                }));
            }
            else
            {
                SetLocalSubFileList(_sftpLocalPath);
            }
        }
        #endregion

        #region SFTP Remote List
        /// <summary>
        /// SFTP远程文件路径
        /// </summary>
        private string _sftpRemotePath = string.Empty;

        /// <summary>
        /// 初始化远程文件列表
        /// </summary>
        private void InitRemoteFileList()
        {
            if (IsSFTPConnected)
            {
                var rootPath = "/";
                SetRemoteSubFileList(rootPath);
            }
        }

        /// <summary>
        /// 设置远程下级文件目录
        /// </summary>
        /// <param name="currentPath"></param>
        private void SetRemoteSubFileList(string currentPath)
        {
            _sftpRemotePath = currentPath;

            if (!CheckSFTPConnected())
            {
                // todo:disable remote listview
                MessageBox.Show("连接已断开！");
                return;
            }

            var sftpFiles = _sftpClient.ListDirectory(currentPath);
            if (sftpFiles.Any())
            {
                var sftpFileList = sftpFiles.OrderBy(d => d.Name).ToList();
                SetRemoteFileList(sftpFileList, currentPath);
            }
        }

        /// <summary>
        /// 设置远程文件列表
        /// </summary>
        /// <param name="fsArray"></param>
        private void SetRemoteFileList(List<SftpFile> fsArray, string currentPath)
        {
            string parentPath = GetLinuxParentPath(currentPath);

            this.remoteListView.BeginUpdate();

            this.remoteListView.Items.Clear();

            if (!string.IsNullOrWhiteSpace(parentPath))
            {
                ListViewItem lviParent = new ListViewItem();
                lviParent.Tag = "directory|" + parentPath;
                lviParent.Text = "..";
                lviParent.ImageIndex = GetFileIconIndex("directory");
                this.remoteListView.Items.Add(lviParent);
            }

            if (fsArray.Any())
            {
                foreach (var f in fsArray)
                {
                    ListViewItem lvi = new ListViewItem();
                    lvi.Tag = f.FullName;
                    lvi.Tag = (f.IsDirectory ? "directory" : "file") + "|" + f.FullName;

                    if (f.IsDirectory)
                    {
                        if (f.Name == "." || f.Name == "..")
                        {
                            continue;
                        }

                        lvi.Text = f.Name;
                        lvi.SubItems.Add("");
                        lvi.SubItems.Add("文件夹");
                        lvi.SubItems.Add(f.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"));
                        lvi.ImageIndex = GetFileIconIndex("directory");
                    }
                    else if (f.IsRegularFile)
                    {
                        var ext = string.Empty;
                        var extIndex = f.Name.LastIndexOf('.');
                        if (extIndex >= 0)
                        {
                            ext = f.Name.Substring(extIndex + 1);
                        }

                        if (string.IsNullOrWhiteSpace(ext))
                        {
                            ext = "文件";
                        }

                        lvi.Text = f.Name;
                        lvi.SubItems.Add(f.Length.ToString());
                        lvi.SubItems.Add(ext);
                        lvi.SubItems.Add(f.LastWriteTime.ToString("yyyy/MM/dd HH:mm:ss"));
                        lvi.ImageIndex = GetFileIconIndex(ext);
                    }

                    this.remoteListView.Items.Add(lvi);
                }
            }

            this.remoteListView.EndUpdate();
        }

        /// <summary>
        /// 获取Linux上级目录
        /// </summary>
        /// <param name="currentPath"></param>
        /// <returns></returns>
        private string GetLinuxParentPath(string currentPath)
        {
            var parentPath = string.Empty;

            if (currentPath == "/") // 根目录
            {
                parentPath = "";
            }
            else
            {
                parentPath = currentPath.Substring(0, currentPath.LastIndexOf('/'));
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    parentPath = "/";
                }
            }

            return parentPath;
        }

        /// <summary>
        /// 远程目录鼠标双击处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remoteListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            ListViewHitTestInfo info = remoteListView.HitTest(e.X, e.Y);
            if (info.Item != null)
            {
                var item = info.Item as ListViewItem;
                var pathInfo = item.Tag.ToString().Split('|');
                var pathType = pathInfo[0];
                var path = pathInfo[1];

                // 打开下级目录
                if (pathType == "directory")
                {
                    SetRemoteSubFileList(path);
                    return;
                }

                // 如果连接已经建立，下载文件
                if (pathType == "file")
                {
                    var fSize = ulong.Parse(item.SubItems[1].Text);
                    AsyncDownloadFile(path, item.Text, fSize);
                }
            }
        }

        /// <summary>
        /// 远程目录排序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remoteListView_ColumnClick(object sender, ColumnClickEventArgs e)
        {
            // 默认非倒序
            if (remoteListView.Columns[e.Column].Tag == null)
            {
                remoteListView.Columns[e.Column].Tag = false;
            }
            var tabK = (bool)remoteListView.Columns[e.Column].Tag;
            remoteListView.Columns[e.Column].Tag = !tabK;
            remoteListView.ListViewItemSorter = new ListViewSort(e.Column, remoteListView.Columns[e.Column].Tag);

            remoteListView.Sort();
        }

        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="remoteFileName"></param>
        private void AsyncDownloadFile(string remoteFilePath, string remoteFileName, ulong fileSize)
        {
            Task.Factory.StartNew(() =>
            {
                var localPath = Path.Combine(_sftpLocalPath, remoteFileName);
                SFTPProcessStatus status = new SFTPProcessStatus()
                {
                    ID = Guid.NewGuid(),
                    DestPath = _sftpLocalPath,
                    DestFilePath = localPath,
                    Direct = "download",
                    SourcePath = _sftpRemotePath,
                    SourceFilePath = remoteFilePath,
                    TotalSize = fileSize,
                    StartTime = DateTime.Now,
                    ProcessStatus = 0
                };

                try
                {
                    using (var file = File.Open(localPath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                    {
                        _sftpClient.DownloadFile(remoteFilePath, file, (size) =>
                        {
                            // 下载进度
                            status.ProcessSize = size;

                            // 下载完毕
                            if (size == fileSize)
                            {
                                status.ProcessStatus = 1;
                                status.EndTime = DateTime.Now;
                                DoDownloadFileCompleted(status);
                            }

                            UpdateSFTPStatus(status);
                        });
                    }
                }
                catch (Exception ex)
                {
                    status.ErrorMessage = ex.Message;
                    status.ProcessStatus = -1;
                    UpdateSFTPStatus(status);
                }
            });
        }

        /// <summary>
        /// 更新远程文件列表
        /// </summary>
        /// <param name="remotePath"></param>
        private void UpdateRemoteList(string remotePath)
        {
            if (remoteListView.InvokeRequired)
            {
                remoteListView.Invoke(new Action(() =>
                {
                    SetRemoteSubFileList(remotePath);
                }));
            }
            else
            {
                SetRemoteSubFileList(remotePath);
            }
        }

        /// <summary>
        /// 执行下载文件完成
        /// </summary>
        /// <param name="status"></param>
        private void DoDownloadFileCompleted(SFTPProcessStatus status)
        {
            if (_sftpLocalPath == status.DestPath)
            {
                RefreshLocalListView();
            }
        }
        #endregion

        #region SFTP Status List
        /// <summary>
        /// 更新SFTP状态
        /// </summary>
        /// <param name="status"></param>
        private void UpdateSFTPStatus(SFTPProcessStatus status)
        {
            if (statusListView.InvokeRequired)
            {
                statusListView.Invoke(new Action(() =>
                {
                    SetStatusListView(status);
                }));
            }
        }

        /// <summary>
        /// 设置远程文件列表
        /// </summary>
        /// <param name="fsArray"></param>
        private void SetStatusListView(SFTPProcessStatus status)
        {
            this.statusListView.BeginUpdate();

            var processStatus = string.Empty;
            if (status.ProcessStatus == 1)
            {
                processStatus = "完成";
            }
            else if (status.ProcessStatus == 0)
            {
                processStatus = "处理中";
            }
            else if (status.ProcessStatus == -1)
            {
                processStatus = status.ErrorMessage;
            }

            if (status.ViewItem == null)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = status.ID.ToString();
                lvi.Text = status.SourceFilePath;
                lvi.SubItems.Add(status.Direct);
                lvi.SubItems.Add(status.DestFilePath);
                lvi.SubItems.Add(status.ProcessSize + "/" + status.TotalSize);
                lvi.SubItems.Add(processStatus);

                status.ViewItem = lvi;
                this.statusListView.Items.Insert(0, lvi);
            }
            else
            {
                status.ViewItem.SubItems[3].Text = status.ProcessSize + "/" + status.TotalSize;
                status.ViewItem.SubItems[4].Text = processStatus;
            }

            // TODO:显示完成时间

            this.statusListView.EndUpdate();
        }
        #endregion
    }
}
