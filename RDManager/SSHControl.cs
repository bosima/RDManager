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
using System.Threading;
using System.Collections.Concurrent;

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
            AsyncConnectSFTP();
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
            if (_sftpClient != null && CheckSFTPConnected())
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
            InitRemoteFileList();
            InitProcessList();
        }

        /// <summary>
        /// 获取文件图标索引
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        private int GetFileIconIndex(string fileExtension)
        {
            // TODO:和系统图标有差别，还需再看看

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

            this.processListView.Columns.Add("源文件", 300, HorizontalAlignment.Left);
            this.processListView.Columns.Add("方向", 80, HorizontalAlignment.Center);
            this.processListView.Columns.Add("目标文件", 300, HorizontalAlignment.Left);
            this.processListView.Columns.Add("大小", 100, HorizontalAlignment.Left);
            this.processListView.Columns.Add("状态", 100, HorizontalAlignment.Left);

            this.localListView.SmallImageList = _fileIconList;
            this.remoteListView.SmallImageList = _fileIconList;
        }

        /// <summary>
        /// 修正拆分器的位置
        /// </summary>
        private void FixSplitContainer()
        {
            lrSplitContainer.SplitterDistance = lrSplitContainer.Width / 2;
            fsSplitContainer.SplitterDistance = fsSplitContainer.Height - 100;
            leftSplitContainer.SplitterDistance = 32;
            leftSplitContainer.FixedPanel = FixedPanel.Panel1;
            rightSplitContainer.SplitterDistance = 32;
            rightSplitContainer.FixedPanel = FixedPanel.Panel1;
        }

        /// <summary>
        /// 检查SFTP连接状态
        /// </summary>
        /// <returns></returns>
        private bool CheckSFTPConnected()
        {
            try
            {
                if (_sftpClient != null)
                {
                    return _sftpClient.IsConnected;
                }
                else
                {
                    return false;
                }
            }
            catch (ObjectDisposedException)
            {
            }

            return false;
        }

        /// <summary>
        /// 异步连接SFTP
        /// </summary>
        private void AsyncConnectSFTP()
        {
            Task.Factory.StartNew(() =>
            {
                ConnectSFTP();
                SFTP_OnConnected(this, new EventArgs());
            });
        }

        /// <summary>
        /// 连接SFTP
        /// </summary>
        private void ConnectSFTP()
        {
            // connecting
            _sFTPConnectStatus = 0;

            var connectionInfo = new ConnectionInfo(_rdsServer.ServerAddress,
                                   _rdsServer.UserName,
                                   new PasswordAuthenticationMethod(_rdsServer.UserName, EncryptUtils.DecryptServerPassword(_rdsServer))
                                   );

            _sftpClient = new SftpClient(connectionInfo);
            _sftpClient.KeepAliveInterval = new TimeSpan(0, 0, 10);
            _sftpClient.ErrorOccurred += SFTP_OnErrorOccurred;
            _sftpClient.Connect();
        }

        /// <summary>
        /// 确定SFTP连接状态，如果已经断开，则重新连接
        /// </summary>
        private void FixSFTPConnect()
        {
            if (_sftpClient == null || !CheckSFTPConnected())
            {
                AsyncConnectSFTP();
            }
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
            if (e != null && e.Exception != null)
            {
                ShowErrorMessage(e.Exception.Message);
            }
            else
            {
                ShowErrorMessage("SFTP发生错误！");
            }
        }

        /// <summary>
        /// 显示错误消息
        /// </summary>
        /// <param name="message"></param>
        private void ShowErrorMessage(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() =>
                {
                    MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
            else
            {
                MessageBox.Show(message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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
            InitLocalListRightButtonMemnu();

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
            txtCurrentLocalPath.Text = _sftpLocalPath;

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
                    lvi.Tag = "directory|" + f.Name;
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
            var items = localListView.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    DeleteLocalListItem(item);
                }
            }
        }

        /// <summary>
        /// 删除指定本地项目
        /// </summary>
        /// <param name="listItem"></param>
        private void DeleteLocalListItem(ListViewItem listItem)
        {
            if (listItem != null)
            {
                Task.Factory.StartNew(() =>
                {
                    var itemArray = listItem.Tag.ToString().Split('|');
                    var itemType = itemArray[0];
                    var itemPath = itemArray[1];

                    try
                    {
                        if (itemType == "directory")
                        {
                            DeleteLocalDirectory(listItem.Text, itemPath);
                        }
                        else if (itemType == "file")
                        {
                            AddLocalFileDeleteTask(itemPath, listItem.Text, ulong.Parse(listItem.SubItems[1].Text));
                        }

                        RefreshLocalListView();
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage("删除失败：" + ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// 删除本地目录
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="fullPath"></param>
        private void DeleteLocalDirectory(string directoryName, string fullPath)
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
                MakeLocalDirectoryTree(rootNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本地目录结构失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RecursionDeleteLocalDirectory(rootNode, _sftpLocalPath);
        }

        /// <summary>
        /// 递归删除本地目录
        /// </summary>
        /// <param name="treeNode"></param>
        private void RecursionDeleteLocalDirectory(FileTreeNode treeNode, string localFilePath)
        {
            // 如果目录存在，则遍历下级处理
            if (treeNode.Type == "directory")
            {
                var currentDirectoryPath = Path.Combine(localFilePath, treeNode.Name);

                if (treeNode.Children == null || treeNode.Children.Count <= 0)
                {
                    AddLocalDirectoryDeleteTask(currentDirectoryPath, localFilePath, treeNode.Name);
                }
                else
                {
                    foreach (var subNode in treeNode.Children)
                    {
                        RecursionDeleteLocalDirectory(subNode, currentDirectoryPath);
                    }

                    AddLocalDirectoryDeleteTask(currentDirectoryPath, localFilePath, treeNode.Name);
                }
            }

            if (treeNode.Type == "file")
            {
                AddLocalFileDeleteTask(treeNode.Path, treeNode.Name, treeNode.Size);
            }
        }

        /// <summary>
        /// 添加删除本地文件任务
        /// </summary>
        /// <param name="remoteFilePath"></param>
        private void AddLocalFileDeleteTask(string localFilePath, string localFileName, ulong fileSize)
        {
            var localPath = localFilePath.TrimEnd(localFileName.ToCharArray());
            if (localPath.EndsWith(Path.DirectorySeparatorChar.ToString()) && localPath.Length > 1)
            {
                localPath = localPath.Substring(0, localPath.Length - 1);
            }

            SFTPProcessTask task = new SFTPProcessTask()
            {
                ID = Guid.NewGuid(),
                PathType = "file",
                Direct = "本地删除",
                SourceFilePath = localFilePath,
                SourcePath = localPath,
                DestPath = localPath,
                TotalSize = fileSize,
                ProcessSize = fileSize,
                StartTime = DateTime.Now,
                ProcessStatus = 0
            };

            AddSFTPTask(task);
        }

        /// <summary>
        /// 添加删除本地文件夹任务
        /// </summary>
        /// <param name="localFilePath"></param>
        private void AddLocalDirectoryDeleteTask(string localFilePath, string localPath, string fileName)
        {
            try
            {
                SFTPProcessTask task = new SFTPProcessTask()
                {
                    ID = Guid.NewGuid(),
                    PathType = "directory",
                    Direct = "本地删除",
                    SourceFilePath = localFilePath,
                    SourcePath = localPath,
                    DestPath = localPath,
                    FileName = fileName,
                    TotalSize = 0,
                    ProcessSize = 0,
                    StartTime = DateTime.Now,
                    ProcessStatus = 0
                };

                AddSFTPTask(task);
            }
            catch (Exception ex)
            {
                ShowErrorMessage("添加删除本地文件夹任务异常：" + ex.Message);
            }
        }

        /// <summary>
        /// 右键上传处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLocalListUploadItem_Click(object sender, EventArgs e)
        {
            var items = localListView.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    UploadLocalListItem(item);
                }
            }
        }

        /// <summary>
        /// 上传本地文件列表项目
        /// </summary>
        /// <param name="listItem"></param>
        private void UploadLocalListItem(ListViewItem listItem)
        {
            Task.Factory.StartNew(() =>
            {
                var itemArray = listItem.Tag.ToString().Split('|');
                var itemType = itemArray[0];
                var itemPath = itemArray[1];

                if (itemType == "file")
                {
                    var localFilePath = itemPath;
                    var fileName = listItem.Text;
                    var fileSize = ulong.Parse(listItem.SubItems[1].Text);
                    AddUploadFileTask(localFilePath, fileName, fileSize);
                }

                if (itemType == "directory")
                {
                    AddUploadLocalDirectoryTask(listItem.Text, itemPath);
                }
            });
        }

        /// <summary>
        /// 上传本地文件夹
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="fullPath"></param>
        private void AddUploadLocalDirectoryTask(string directoryName, string fullPath)
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
                MakeLocalDirectoryTree(rootNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本地目录结构失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 递归创建文件夹和上传文件
            RecursionAddUploadLocalDirectoryTask(rootNode, _sftpRemotePath);
        }

        /// <summary>
        /// 递归文件树上传本地目录
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="remoteUploadPath"></param>
        private void RecursionAddUploadLocalDirectoryTask(FileTreeNode treeNode, string remoteUploadPath)
        {
            // 如果目录不存在，则首先创建
            if (treeNode.Type == "directory")
            {
                var currentUploadPath = remoteUploadPath == _sFTPPathSeparator ? remoteUploadPath + treeNode.Name : remoteUploadPath + _sFTPPathSeparator + treeNode.Name;
                AddCreateRemoteDirectoryTask(treeNode.Path, currentUploadPath, treeNode.Name);

                if (treeNode.Children != null && treeNode.Children.Count > 0)
                {
                    foreach (var subNode in treeNode.Children)
                    {
                        RecursionAddUploadLocalDirectoryTask(subNode, currentUploadPath);
                    }
                }
            }

            if (treeNode.Type == "file")
            {
                AddUploadFileTask(treeNode.Path, treeNode.Name, remoteUploadPath, treeNode.Size);
            }
        }

        /// <summary>
        /// 根据根节点生成一个目录树
        /// </summary>
        /// <param name="tree"></param>
        private void MakeLocalDirectoryTree(FileTreeNode tree)
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
                        MakeLocalDirectoryTree(node);
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
                    AddUploadFileTask(path, item.Text, fSize);
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
        /// 本地目录选中项变更处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (localListView.SelectedItems == null || localListView.SelectedItems.Count <= 0)
            {
                _currentLocalSelectedItem = null;
            }
        }

        /// <summary>
        /// 异步上传文件到指定远程文件夹
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="localFileName"></param>
        /// <param name="remoteDirectoryPath"></param>
        /// <param name="fileSize"></param>
        private void AddUploadFileTask(string localFilePath, string localFileName, string remoteDirectoryPath, ulong fileSize)
        {
            Task.Factory.StartNew(() =>
            {
                var remoteFilePath = remoteDirectoryPath == _sFTPPathSeparator ? remoteDirectoryPath + localFileName : remoteDirectoryPath + _sFTPPathSeparator + localFileName;

                SFTPProcessTask task = new SFTPProcessTask()
                {
                    ID = Guid.NewGuid(),
                    FileName = localFileName,
                    DestPath = remoteDirectoryPath,
                    DestFilePath = remoteFilePath,
                    PathType = "file",
                    Direct = "上传",
                    SourceFilePath = localFilePath,
                    SourcePath = localFilePath.TrimEnd(localFileName.ToCharArray()),
                    TotalSize = fileSize,
                    StartTime = DateTime.Now,
                    ProcessStatus = 0
                };

                AddSFTPTask(task);
            });
        }

        /// <summary>
        /// 异步上传文件到当前远程文件夹
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="remoteFileName"></param>
        private void AddUploadFileTask(string localFilePath, string localFileName, ulong fileSize)
        {
            AddUploadFileTask(localFilePath, localFileName, _sftpRemotePath, fileSize);
        }

        /// <summary>
        /// 添加创建远程文件夹的任务
        /// </summary>
        /// <param name="localFilePath"></param>
        /// <param name="remoteFilePath"></param>
        /// <param name="pathName"></param>
        private void AddCreateRemoteDirectoryTask(string localFilePath, string remoteFilePath, string pathName)
        {
            var remotePath = remoteFilePath.TrimEnd(pathName.ToCharArray());
            var localPath = localFilePath.TrimEnd(pathName.ToCharArray());

            if (remotePath.EndsWith(_sFTPPathSeparator) && remotePath.LastIndexOf(_sFTPPathSeparator) > 0)
            {
                remotePath = remotePath.Substring(0, remotePath.Length - 1);
            }

            SFTPProcessTask task = new SFTPProcessTask()
            {
                ID = Guid.NewGuid(),
                PathType = "directory",
                FileName = pathName,
                SourcePath = localPath,
                SourceFilePath = localFilePath,
                DestFilePath = remoteFilePath,
                DestPath = remotePath,
                Direct = "上传",
                TotalSize = 0,
                StartTime = DateTime.Now,
                ProcessStatus = 0
            };

            AddSFTPTask(task);
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

        /// <summary>
        /// 自动改变本地路径输入框的宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnlLocalPath_SizeChanged(object sender, EventArgs e)
        {
            var localPathWidth = pnlLocalPath.Width - lblLocalDirectory.Width - 15;
            if (localPathWidth < 20)
            {
                localPathWidth = 20;
            }

            txtCurrentLocalPath.Width = localPathWidth;
        }

        /// <summary>
        /// 处理本地目录输入框按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCurrentLocalPath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessTxtCurrentLocalPathEnter();
            }
        }

        /// <summary>
        /// 处理本地目录输入框Enter事件
        /// </summary>
        private void ProcessTxtCurrentLocalPathEnter()
        {
            var path = txtCurrentLocalPath.Text;
            if (path.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                path = path.TrimEnd(Path.DirectorySeparatorChar);
                txtCurrentLocalPath.Text = path;
            }

            if (Directory.Exists(path))
            {
                SetLocalSubFileList(path);
            }
            else
            {
                ShowErrorMessage("本地文件夹不存在：" + path);
            }
        }

        /// <summary>
        /// 元素被拖动时触发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void localListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (localListView.SelectedItems.Count <= 0)
                {
                    return;
                }

                DataObject data = new DataObject("localListView", localListView.SelectedItems);
                DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        private void localListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("remoteListView"))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void localListView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var data = e.Data.GetData("remoteListView", false);

                if (data != null)
                {
                    ListView.SelectedListViewItemCollection items = data as ListView.SelectedListViewItemCollection;

                    foreach (ListViewItem item in items)
                    {
                        // TODO:如果有文件夹，则需要指定本地目录
                        DownloadLocalListItem(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("无法处理拖动的数据：" + ex.Message);
            }
        }
        #endregion

        #region SFTP Remote List
        /// <summary>
        /// 远程文件列表右键菜单
        /// </summary>
        private ContextMenuStrip _remoteContextMenuStrip;

        /// <summary>
        /// 当前远程目录选项
        /// </summary>
        private ListViewItem _currentRemoteSelectedItem;

        /// <summary>
        /// SFTP远程文件路径
        /// </summary>
        private string _sftpRemotePath = string.Empty;

        /// <summary>
        /// 初始化远程文件列表
        /// </summary>
        private void InitRemoteFileList()
        {
            InitRemoteListRightButtonMemnu();

            if (CheckSFTPConnected())
            {
                if (string.IsNullOrWhiteSpace(_sftpRemotePath))
                {
                    _sftpRemotePath = _sFTPPathSeparator;
                }

                SetRemoteSubFileList(_sftpRemotePath);
            }
        }

        /// <summary>
        /// 设置远程下级文件目录
        /// </summary>
        /// <param name="currentPath"></param>
        private void SetRemoteSubFileList(string currentPath)
        {
            _sftpRemotePath = currentPath;
            txtCurrentRemotePath.Text = _sftpRemotePath;

            if (!CheckSFTPConnected())
            {
                // TODO:Disable remote listview
                ShowErrorMessage("连接已断开！");
                return;
            }

            try
            {
                var sftpFiles = _sftpClient.ListDirectory(currentPath);
                if (sftpFiles.Any())
                {
                    var sftpFileList = sftpFiles.OrderBy(d => d.Name).ToList();
                    SetRemoteFileList(sftpFileList, currentPath);
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("操作失败：" + ex.Message);
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
                            ext = f.Name.Substring(extIndex);
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

            if (currentPath == _sFTPPathSeparator) // 根目录
            {
                parentPath = "";
            }
            else
            {
                parentPath = currentPath.Substring(0, currentPath.LastIndexOf('/'));
                if (string.IsNullOrWhiteSpace(parentPath))
                {
                    parentPath = _sFTPPathSeparator;
                }
            }

            return parentPath;
        }

        /// <summary>
        /// 初始化本地右键按钮
        /// </summary>
        private void InitRemoteListRightButtonMemnu()
        {
            ToolStripMenuItem btnRemoteListDownloadItem = new ToolStripMenuItem();
            btnRemoteListDownloadItem.Name = "btnRemoteListUploadItem";
            btnRemoteListDownloadItem.Text = "下载";
            btnRemoteListDownloadItem.Click += BtnRemoteListDownloadItem_Click;

            ToolStripMenuItem btnRemoteListRefreshItem = new ToolStripMenuItem();
            btnRemoteListRefreshItem.Name = "btnRemoteListRefreshItem";
            btnRemoteListRefreshItem.Text = "刷新";
            btnRemoteListRefreshItem.Click += BtnRemoteListRefreshItem_Click;

            ToolStripMenuItem btnRemoteListNewFolderItem = new ToolStripMenuItem();
            btnRemoteListNewFolderItem.Name = "btnRemoteListNewFolderItem";
            btnRemoteListNewFolderItem.Text = "新建文件夹";
            btnRemoteListNewFolderItem.Click += BtnRemoteListNewFolderItem_Click;

            ToolStripMenuItem btnRemoteListDeleteItem = new ToolStripMenuItem();
            btnRemoteListDeleteItem.Name = "btnRemoteListDeleteItem";
            btnRemoteListDeleteItem.Text = "删除";
            btnRemoteListDeleteItem.Click += BtnRemoteListDeleteItem_Click;


            _remoteContextMenuStrip = new ContextMenuStrip();
            _remoteContextMenuStrip.Items.Add(btnRemoteListDownloadItem);
            _remoteContextMenuStrip.Items.Add(btnRemoteListRefreshItem);
            _remoteContextMenuStrip.Items.Add(btnRemoteListNewFolderItem);
            _remoteContextMenuStrip.Items.Add(btnRemoteListDeleteItem);

            remoteListView.ContextMenuStrip = _remoteContextMenuStrip;
        }

        /// <summary>
        /// 远程删除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoteListDeleteItem_Click(object sender, EventArgs e)
        {
            var items = remoteListView.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    DeleteRemoteListItem(item);
                }
            }
        }

        /// <summary>
        /// 删除指定远程项目
        /// </summary>
        /// <param name="listItem"></param>
        private void DeleteRemoteListItem(ListViewItem listItem)
        {
            if (listItem != null)
            {
                Task.Factory.StartNew(() =>
                {
                    var itemArray = listItem.Tag.ToString().Split('|');
                    var itemType = itemArray[0];
                    var itemPath = itemArray[1];

                    try
                    {
                        if (itemType == "directory")
                        {
                            AddRemoteDirectoryDeleteTaskWithSub(listItem.Text, itemPath);
                        }
                        else if (itemType == "file")
                        {
                            AddRemoteFileDeleteTask(itemPath, listItem.Text, ulong.Parse(listItem.SubItems[1].Text));
                        }
                    }
                    catch (Exception ex)
                    {
                        ShowErrorMessage("删除失败：" + ex.Message);
                    }
                });
            }
        }

        /// <summary>
        /// 添加远程文件删除任务
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="fileSize"></param>
        private void AddRemoteFileDeleteTask(string remoteFilePath, string remoteFileName, ulong fileSize)
        {
            var remotePath = remoteFilePath.TrimEnd(remoteFileName.ToCharArray());
            if (remotePath.EndsWith(_sFTPPathSeparator) && remotePath.Length > 1)
            {
                remotePath = remotePath.Substring(0, remotePath.Length - 1);
            }

            SFTPProcessTask task = new SFTPProcessTask()
            {
                ID = Guid.NewGuid(),
                Direct = "远程删除",
                PathType = "file",
                DestPath = remotePath,
                SourceFilePath = remoteFilePath,
                SourcePath = remotePath,
                FileName = remoteFileName,
                TotalSize = fileSize,
                ProcessSize = fileSize,
                StartTime = DateTime.Now,
                ProcessStatus = 0
            };

            AddSFTPTask(task);
        }

        /// <summary>
        /// 添加远程删除文件夹任务
        /// </summary>
        /// <param name="remoteFilePath"></param>
        private void AddRemoteDirectoryDeleteTask(string remoteFilePath, string remoteFileName)
        {
            var remotePath = remoteFilePath.TrimEnd(remoteFileName.ToCharArray());
            if (remotePath.EndsWith(_sFTPPathSeparator) && remotePath.Length > 1)
            {
                remotePath = remotePath.Substring(0, remotePath.Length - 1);
            }

            SFTPProcessTask task = new SFTPProcessTask()
            {
                ID = Guid.NewGuid(),
                PathType = "directory",
                Direct = "远程删除",
                SourceFilePath = remoteFilePath,
                SourcePath = remotePath,
                DestPath = remotePath,
                TotalSize = 0,
                ProcessSize = 0,
                StartTime = DateTime.Now,
                ProcessStatus = 0
            };

            AddSFTPTask(task);
        }

        /// <summary>
        /// 添加远程删除文件夹任务
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="fullPath"></param>
        private void AddRemoteDirectoryDeleteTaskWithSub(string directoryName, string fullPath)
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
                MakeRemoteDirectoryTree(rootNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取本地目录结构失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            RecursionAddRemoteDirectoryDeleteTask(rootNode, _sftpRemotePath);
        }

        /// <summary>
        /// 递归删除远程目录
        /// </summary>
        /// <param name="treeNode"></param>
        private void RecursionAddRemoteDirectoryDeleteTask(FileTreeNode treeNode, string remoteFilePath)
        {
            // 如果目录存在，则遍历下级处理
            if (treeNode.Type == "directory")
            {
                var currentDirectoryPath = remoteFilePath == _sFTPPathSeparator ? remoteFilePath + treeNode.Name : remoteFilePath + _sFTPPathSeparator + treeNode.Name;

                if (treeNode.Children == null || treeNode.Children.Count <= 0)
                {
                    AddRemoteDirectoryDeleteTask(currentDirectoryPath, treeNode.Name);
                }
                else
                {
                    foreach (var subNode in treeNode.Children)
                    {
                        RecursionAddRemoteDirectoryDeleteTask(subNode, currentDirectoryPath);
                    }

                    AddRemoteDirectoryDeleteTask(currentDirectoryPath, treeNode.Name);
                }
            }

            if (treeNode.Type == "file")
            {
                AddRemoteFileDeleteTask(treeNode.Path, treeNode.Name, treeNode.Size);
            }
        }

        /// <summary>
        /// 新建文件夹
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoteListNewFolderItem_Click(object sender, EventArgs e)
        {
            DirectoryNameEditForm form = new DirectoryNameEditForm();
            if (form.ShowDialog() == DialogResult.OK)
            {
                var directoryName = form.DirectoryName;
                var directory = _sftpRemotePath + _sFTPPathSeparator + directoryName;

                try
                {
                    if (!_sftpClient.Exists(directory))
                    {
                        _sftpClient.CreateDirectory(directory);
                    }

                    RefreshRemoteListView();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("新建文件夹失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// 远程刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoteListRefreshItem_Click(object sender, EventArgs e)
        {
            RefreshRemoteListView();
        }

        /// <summary>
        /// 远程下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRemoteListDownloadItem_Click(object sender, EventArgs e)
        {
            var items = remoteListView.SelectedItems;
            if (items.Count > 0)
            {
                foreach (ListViewItem item in items)
                {
                    DownloadLocalListItem(item);
                }
            }
        }

        /// <summary>
        /// 下载远程文件列表项目
        /// </summary>
        /// <param name="listItem"></param>
        private void DownloadLocalListItem(ListViewItem listItem)
        {
            Task.Factory.StartNew(() =>
            {
                var itemArray = listItem.Tag.ToString().Split('|');
                var itemType = itemArray[0];
                var itemPath = itemArray[1];

                if (itemType == "file")
                {
                    var fileName = listItem.Text;
                    var fileSize = ulong.Parse(listItem.SubItems[1].Text);
                    AddDownloadFileTask(itemPath, fileName, fileSize);
                }

                if (itemType == "directory")
                {
                    AddDownloadRemoteDirectoryTask(listItem.Text, itemPath);
                }

                RefreshLocalListView();
            });
        }

        /// <summary>
        /// 下载远程文件夹
        /// </summary>
        /// <param name="directoryName"></param>
        /// <param name="fullPath"></param>
        private void AddDownloadRemoteDirectoryTask(string directoryName, string fullPath)
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
                MakeRemoteDirectoryTree(rootNode);
            }
            catch (Exception ex)
            {
                MessageBox.Show("获取远程目录结构失败：" + ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 递归创建文件夹和上传文件
            RecursionAddDownloadRemoteDirectoryTask(rootNode, _sftpLocalPath);
        }

        /// <summary>
        /// 递归文件树下载远程目录
        /// </summary>
        /// <param name="treeNode"></param>
        /// <param name="remoteUploadPath"></param>
        private void RecursionAddDownloadRemoteDirectoryTask(FileTreeNode treeNode, string localDownloadPath)
        {
            // 如果目录不存在，则首先创建
            if (treeNode.Type == "directory")
            {
                var currentDownloadPath = Path.Combine(localDownloadPath, treeNode.Name);

                AddCreateDownloadDirectoryTask(currentDownloadPath, treeNode.Path, treeNode.Name);

                if (treeNode.Children != null && treeNode.Children.Count > 0)
                {
                    foreach (var subNode in treeNode.Children)
                    {
                        RecursionAddDownloadRemoteDirectoryTask(subNode, currentDownloadPath);
                    }
                }
            }

            if (treeNode.Type == "file")
            {
                AddDownloadFileTask(treeNode.Path, treeNode.Name, localDownloadPath, treeNode.Size);
            }
        }

        /// <summary>
        /// 异步下载文件
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="remoteFileName"></param>
        /// <param name="localDirectoryPath"></param>
        /// <param name="fileSize"></param>
        private void AddDownloadFileTask(string remoteFilePath, string remoteFileName, string localDirectoryPath, ulong fileSize)
        {
            Task.Factory.StartNew(() =>
            {
                var localPath = Path.Combine(localDirectoryPath, remoteFileName);
                SFTPProcessTask task = new SFTPProcessTask()
                {
                    ID = Guid.NewGuid(),
                    FileName = remoteFileName,
                    DestPath = localDirectoryPath,
                    DestFilePath = localPath,
                    PathType = "file",
                    Direct = "下载",
                    SourcePath = _sftpRemotePath,
                    SourceFilePath = remoteFilePath,
                    TotalSize = fileSize,
                    StartTime = DateTime.Now,
                    ProcessStatus = 0
                };

                AddSFTPTask(task);
            });
        }

        /// <summary>
        /// 异步下载文件到当前本地文件夹
        /// </summary>
        /// <param name="remoteFilePath"></param>
        /// <param name="remoteFileName"></param>
        private void AddDownloadFileTask(string remoteFilePath, string remoteFileName, ulong fileSize)
        {
            AddDownloadFileTask(remoteFilePath, remoteFileName, _sftpLocalPath, fileSize);
        }

        /// <summary>
        /// 添加创建下载文件夹的任务
        /// </summary>
        /// <param name="localPath"></param>
        /// <param name="remoteFilePath"></param>
        /// <param name="pathName"></param>
        private void AddCreateDownloadDirectoryTask(string localPath, string remoteFilePath, string pathName)
        {
            var remotePath = remoteFilePath.TrimEnd(pathName.ToCharArray());
            if (remotePath.EndsWith(_sFTPPathSeparator) && remotePath.Length > 1)
            {
                remotePath = remotePath.Substring(0, remotePath.Length - 1);
            }

            SFTPProcessTask task = new SFTPProcessTask()
            {
                ID = Guid.NewGuid(),
                PathType = "directory",
                FileName = pathName,
                SourceFilePath = remoteFilePath,
                SourcePath = remotePath,
                DestPath = localPath,
                Direct = "下载",
                TotalSize = 0,
                StartTime = DateTime.Now,
                ProcessStatus = 0
            };

            AddSFTPTask(task);
        }

        /// <summary>
        /// 根据根节点生成一个远程目录树
        /// </summary>
        /// <param name="tree"></param>
        private void MakeRemoteDirectoryTree(FileTreeNode tree)
        {
            var fsArray = _sftpClient.ListDirectory(tree.Path).ToList();
            if (fsArray.Any())
            {
                tree.Children = new List<FileTreeNode>();

                foreach (var fs in fsArray)
                {
                    if (fs.IsDirectory)
                    {
                        if (fs.Name == "." || fs.Name == "..")
                        {
                            continue;
                        }

                        var node = new FileTreeNode()
                        {
                            Name = fs.Name,
                            Path = fs.FullName,
                            Type = "directory"
                        };

                        tree.Children.Add(node);
                        MakeRemoteDirectoryTree(node);
                    }

                    if (fs.IsRegularFile)
                    {
                        tree.Children.Add(new FileTreeNode()
                        {
                            Name = fs.Name,
                            Path = fs.FullName,
                            Size = (ulong)fs.Length,
                            Type = "file"
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 远程目录右键处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void remoteListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                _currentRemoteSelectedItem = remoteListView.GetItemAt(e.X, e.Y);
            }
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
                    AddDownloadFileTask(path, item.Text, fSize);
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
        /// 刷新远程文件列表
        /// </summary>
        /// <param name="remotePath"></param>
        private void RefreshRemoteListView()
        {
            if (remoteListView.InvokeRequired)
            {
                remoteListView.Invoke(new Action(() =>
                {
                    SetRemoteSubFileList(_sftpRemotePath);
                }));
            }
            else
            {
                SetRemoteSubFileList(_sftpRemotePath);
            }
        }

        /// <summary>
        /// 执行下载文件完成
        /// </summary>
        /// <param name="status"></param>
        private void DoDownloadFileCompleted(SFTPProcessTask status)
        {
            if (_sftpLocalPath == status.DestPath)
            {
                RefreshLocalListView();
            }
        }

        /// <summary>
        /// 自动改变远程路径输入框的宽度
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void pnRemotePath_SizeChanged(object sender, EventArgs e)
        {
            var remotePathWidth = pnRemotePath.Width - lblRemoteDirectory.Width - 65;
            if (remotePathWidth < 20)
            {
                remotePathWidth = 20;
            }

            txtCurrentRemotePath.Width = remotePathWidth;
            btnReconnect.Left = txtCurrentRemotePath.Width + lblRemoteDirectory.Width + 12;
        }

        /// <summary>
        /// 处理远程目录输入框按键事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtCurrentRemotePath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ProcessTxtCurrentRemotePathEnter();
            }
        }

        /// <summary>
        /// 处理远程目录输入框Enter事件
        /// </summary>
        private void ProcessTxtCurrentRemotePathEnter()
        {
            FixSFTPConnect();

            var path = txtCurrentRemotePath.Text;
            if (path.EndsWith("/") && path.IndexOf('/') != path.LastIndexOf('/'))
            {
                path = path.TrimEnd('/');
                txtCurrentRemotePath.Text = path;
            }

            if (_sftpClient.Exists(path))
            {
                SetRemoteSubFileList(path);
            }
            else
            {
                ShowErrorMessage("远程文件夹不存在：" + path);
            }
        }

        /// <summary>
        /// 重新连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReconnect_Click(object sender, EventArgs e)
        {
            FixSFTPConnect();
        }

        private void remoteListView_ItemDrag(object sender, ItemDragEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                if (remoteListView.SelectedItems.Count <= 0)
                {
                    return;
                }

                DataObject data = new DataObject("remoteListView", remoteListView.SelectedItems);
                DoDragDrop(data, DragDropEffects.Copy);
            }
        }

        private void remoteListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("localListView"))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        private void remoteListView_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                var data = e.Data.GetData("localListView", false);

                if (data != null)
                {
                    ListView.SelectedListViewItemCollection items = data as ListView.SelectedListViewItemCollection;

                    foreach (ListViewItem item in items)
                    {
                        // TODO:如果有文件夹，需要指定远程文件夹
                        UploadLocalListItem(item);
                    }
                }
            }
            catch (Exception ex)
            {
                ShowErrorMessage("无法处理拖动的数据：" + ex.Message);
            }
        }
        #endregion

        #region SFTP Process List
        /// <summary>
        /// SFTP任务队列处理锁
        /// </summary>
        private static readonly object _sftpProcessQueueLocker = new object();

        /// <summary>
        /// 状态文件列表右键菜单
        /// </summary>
        private ContextMenuStrip _statusContextMenuStrip;

        /// <summary>
        /// 处理任务队列
        /// </summary>
        private List<SFTPProcessTask> _sftpProcessList;

        /// <summary>
        /// 初始化处理列表
        /// </summary>
        private void InitProcessList()
        {
            _sftpProcessList = new List<SFTPProcessTask>();
            InitStatusListRightButtonMemnu();
            AsyncProcessSFTPTask();
        }

        /// <summary>
        /// 添加SFTP任务
        /// </summary>
        /// <param name="task"></param>
        private void AddSFTPTask(SFTPProcessTask task)
        {
            // 添加到列表
            UpdateProcessStatus(task);

            AddProcessTask(task);
        }

        /// <summary>
        /// 添加处理任务
        /// </summary>
        /// <param name="task"></param>
        private void AddProcessTask(SFTPProcessTask task)
        {
            lock (_sftpProcessQueueLocker)
            {
                // 添加到队列
                _sftpProcessList.Add(task);
            }
        }

        /// <summary>
        /// 返回一个处理任务
        /// </summary>
        /// <returns></returns>
        private SFTPProcessTask PopProcessTask()
        {
            SFTPProcessTask task = null;
            lock (_sftpProcessQueueLocker)
            {
                if (_sftpProcessList.Count > 0)
                {
                    task = _sftpProcessList[0];
                    _sftpProcessList.Remove(task);
                }
            }

            return task;
        }

        /// <summary>
        /// 异步处理SFTP任务
        /// </summary>
        private void AsyncProcessSFTPTask()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    var task = PopProcessTask();

                    if (task == null)
                    {
                        Thread.Sleep(1000);
                    }
                    else
                    {
                        if (task.ProcessStatus == -2)
                        {
                            continue;
                        }

                        if (task.Direct == "本地删除")
                        {
                            DeleteLocal(task);
                        }

                        if (task.Direct == "上传")
                        {
                            Upload(task);
                        }

                        if (task.Direct == "远程删除")
                        {
                            DeleteRemote(task);
                        }

                        if (task.Direct == "下载")
                        {
                            Download(task);
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 执行下载
        /// </summary>
        /// <param name="task"></param>
        private void Download(SFTPProcessTask task)
        {
            // 文件
            if (task.PathType == "file")
            {
                DownloadFile(task);
            }

            // 文件夹
            if (task.PathType == "directory")
            {
                CreateLocalDirectory(task);
            }

            UploadLocalListView(task);
        }

        /// <summary>
        /// 执行远程删除
        /// </summary>
        /// <param name="task"></param>
        private void DeleteRemote(SFTPProcessTask task)
        {
            // 文件
            if (task.PathType == "file")
            {
                DeleteRemoteFile(task);
            }

            // 文件夹
            if (task.PathType == "directory")
            {
                DeleteRemoteDirectory(task);
            }

            UpdateRemoteListView(task);
        }

        /// <summary>
        /// 执行上传
        /// </summary>
        /// <param name="task"></param>
        private void Upload(SFTPProcessTask task)
        {
            // 文件
            if (task.PathType == "file")
            {
                UploadFile(task);
            }

            // 文件夹
            if (task.PathType == "directory")
            {
                CreateRemoteDirectory(task);
            }

            UpdateRemoteListView(task);
        }

        /// <summary>
        /// 执行本地删除
        /// </summary>
        /// <param name="task"></param>
        private void DeleteLocal(SFTPProcessTask task)
        {
            // 文件
            if (task.PathType == "file")
            {
                DeleteLocalFile(task);
            }

            // 文件夹
            if (task.PathType == "directory")
            {
                DeleteLocalDirectory(task);
            }

            UploadLocalListView(task);
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="task"></param>
        private void UploadFile(SFTPProcessTask task)
        {
            int i = 0;
            DoUpload:
            try
            {
                using (var file = File.Open(task.SourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    FixSFTPConnect();

                    _sftpClient.UploadFile(file, task.DestFilePath, (size) =>
                    {
                        if (task.ProcessStatus == -2)
                        {
                            file.Close();
                        }

                        // 上传进度
                        task.ProcessSize = size;

                        // 上传完毕
                        if (size == task.TotalSize)
                        {
                            task.ProcessStatus = 1;
                            task.EndTime = DateTime.Now;
                        }

                        UpdateProcessStatus(task);

                        if (task.ProcessStatus == 1)
                        {
                            DoUploadFileCompleted(task);
                        }
                    });
                }

                if (task.TotalSize == 0)
                {
                    task.ProcessStatus = 1;
                    task.EndTime = DateTime.Now;
                    DoUploadFileCompleted(task);

                    UpdateProcessStatus(task);
                }
            }
            catch (Exception ex)
            {
                if (i < 3 && task.ProcessStatus != -2)
                {
                    Thread.Sleep((i + 1) * 1000);
                    goto DoUpload;
                }
                else
                {
                    task.ErrorMessage = ex.Message;
                    task.ProcessStatus = -1;
                    UpdateProcessStatus(task);
                }
            }
        }

        /// <summary>
        /// 创建远程文件
        /// </summary>
        /// <param name="remotePath"></param>
        private void CreateRemoteDirectory(SFTPProcessTask task)
        {
            try
            {
                if (!_sftpClient.Exists(task.DestFilePath))
                {
                    _sftpClient.CreateDirectory(task.DestFilePath);
                }

                task.ProcessStatus = 1;
                task.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                task.ErrorMessage = ex.Message;
                task.ProcessStatus = -1;
            }

            UpdateProcessStatus(task);
        }

        /// <summary>
        /// 执行上传文件完成
        /// </summary>
        /// <param name="task"></param>
        private void DoUploadFileCompleted(SFTPProcessTask task)
        {
            UpdateRemoteListView(task);
        }

        /// <summary>
        /// 更新远程文件列表：操作当前目录时才更新
        /// </summary>
        /// <param name="task"></param>
        private void UpdateRemoteListView(SFTPProcessTask task)
        {
            if (_sftpRemotePath == task.DestPath)
            {
                RefreshRemoteListView();
            }
        }

        /// <summary>
        /// 更新本地文件列表：操作当前目录时才更新
        /// </summary>
        /// <param name="task"></param>
        private void UploadLocalListView(SFTPProcessTask task)
        {
            if (_sftpLocalPath == task.DestPath)
            {
                RefreshLocalListView();
            }
        }

        /// <summary>
        /// 删除本地文件
        /// </summary>
        /// <param name="localFilePath"></param>
        private void DeleteLocalFile(SFTPProcessTask task)
        {
            try
            {
                if (File.Exists(task.SourceFilePath))
                {
                    File.Delete(task.SourceFilePath);
                }

                task.ProcessStatus = 1;
                task.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                task.ErrorMessage = ex.Message;
                task.ProcessStatus = -1;
            }

            UpdateProcessStatus(task);
        }

        /// <summary>
        /// 删除本地文件夹
        /// </summary>
        /// <param name="localDirectoryPath"></param>
        private void DeleteLocalDirectory(SFTPProcessTask task)
        {
            try
            {
                if (Directory.Exists(task.SourceFilePath))
                {
                    Directory.Delete(task.SourceFilePath);
                }

                task.ProcessStatus = 1;
                task.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                task.ErrorMessage = ex.Message;
                task.ProcessStatus = -1;
            }

            UpdateProcessStatus(task);
        }

        /// <summary>
        /// 删除远程文件
        /// </summary>
        /// <param name="remoteFilePath"></param>
        private void DeleteRemoteFile(SFTPProcessTask task)
        {
            try
            {
                if (_sftpClient.Exists(task.SourceFilePath))
                {
                    _sftpClient.DeleteFile(task.SourceFilePath);
                }

                task.ProcessStatus = 1;
                task.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                task.ErrorMessage = ex.Message;
                task.ProcessStatus = -1;
            }

            UpdateProcessStatus(task);
        }

        /// <summary>
        /// 删除远程文件夹
        /// </summary>
        /// <param name="remoteDirectoryPath"></param>
        private void DeleteRemoteDirectory(SFTPProcessTask task)
        {
            try
            {
                if (_sftpClient.Exists(task.SourceFilePath))
                {
                    _sftpClient.DeleteDirectory(task.SourceFilePath);
                }

                task.ProcessStatus = 1;
                task.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                task.ErrorMessage = ex.Message;
                task.ProcessStatus = -1;
            }

            UpdateProcessStatus(task);
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="task"></param>
        private void DownloadFile(SFTPProcessTask task)
        {
            int i = 0;
            DoDownload:
            try
            {
                using (var file = File.Open(task.DestFilePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None))
                {
                    FixSFTPConnect();

                    _sftpClient.DownloadFile(task.SourceFilePath, file, (size) =>
                    {
                        if (task.ProcessStatus == -2)
                        {
                            file.Close();
                        }

                        // 下载进度
                        task.ProcessSize = size;

                        // 下载完毕
                        if (size == task.TotalSize)
                        {
                            task.ProcessStatus = 1;
                            task.EndTime = DateTime.Now;
                        }

                        UpdateProcessStatus(task);

                        if (task.ProcessStatus == 1)
                        {
                            DoDownloadFileCompleted(task);
                        }
                    });
                }

                if (task.TotalSize == 0)
                {
                    task.ProcessStatus = 1;
                    task.EndTime = DateTime.Now;
                    DoDownloadFileCompleted(task);

                    UpdateProcessStatus(task);
                }

            }
            catch (Exception ex)
            {
                if (i < 3 && task.ProcessStatus != -2)
                {
                    Thread.Sleep((i + 1) * 1000);
                    goto DoDownload;
                }
                else
                {
                    task.ErrorMessage = ex.Message;
                    task.ProcessStatus = -1;
                    UpdateProcessStatus(task);
                }
            }
        }

        /// <summary>
        /// 创建本地文件夹
        /// </summary>
        /// <param name="remotePath"></param>
        private void CreateLocalDirectory(SFTPProcessTask task)
        {
            try
            {
                if (!Directory.Exists(task.DestPath))
                {
                    Directory.CreateDirectory(task.DestPath);
                }

                task.ProcessStatus = 1;
                task.EndTime = DateTime.Now;
            }
            catch (Exception ex)
            {
                task.ErrorMessage = ex.Message;
                task.ProcessStatus = -1;
            }

            UpdateProcessStatus(task);
        }

        /// <summary>
        /// 更新SFTP处理状态
        /// </summary>
        /// <param name="status"></param>
        private void UpdateProcessStatus(SFTPProcessTask task)
        {
            if (processListView.InvokeRequired)
            {
                processListView.Invoke(new Action(() =>
                {
                    SetProcessListViewItem(task);
                }));
            }
            else
            {
                SetProcessListViewItem(task);
            }
        }

        /// <summary>
        /// 设置远程文件列表项目
        /// </summary>
        /// <param name="fsArray"></param>
        private void SetProcessListViewItem(SFTPProcessTask task)
        {
            this.processListView.BeginUpdate();

            var processStatus = string.Empty;
            if (task.ProcessStatus == 1)
            {
                processStatus = "完成";
            }
            else if (task.ProcessStatus == 0)
            {
                processStatus = "处理中";
            }
            else if (task.ProcessStatus == -1)
            {
                processStatus = task.ErrorMessage;
            }

            if (task.ViewItem == null)
            {
                ListViewItem lvi = new ListViewItem();
                lvi.Tag = task;
                lvi.Text = task.SourceFilePath;
                lvi.SubItems.Add(task.Direct);
                lvi.SubItems.Add(task.DestFilePath);
                lvi.SubItems.Add(task.ProcessSize + "/" + task.TotalSize);
                lvi.SubItems.Add(processStatus);

                task.ViewItem = lvi;
                this.processListView.Items.Insert(0, lvi);
            }
            else
            {
                task.ViewItem.SubItems[3].Text = task.ProcessSize + "/" + task.TotalSize;
                task.ViewItem.SubItems[4].Text = processStatus;
            }

            this.processListView.EndUpdate();
        }

        /// <summary>
        /// 初始化状态列表右键按钮
        /// </summary>
        private void InitStatusListRightButtonMemnu()
        {
            ToolStripMenuItem btnStatusListRemoveFinishedItem = new ToolStripMenuItem();
            btnStatusListRemoveFinishedItem.Name = "btnStatusListRemoveFinishedItem";
            btnStatusListRemoveFinishedItem.Text = "删除全部完成";
            btnStatusListRemoveFinishedItem.Click += BtnStatusListRemoveFinishedItem_Click;

            ToolStripMenuItem btnStatusListRemoveItem = new ToolStripMenuItem();
            btnStatusListRemoveItem.Name = "btnStatusListRemoveItem";
            btnStatusListRemoveItem.Text = "删除选定";
            btnStatusListRemoveItem.Click += BtnStatusListRemoveItem_Click;

            ToolStripMenuItem btnStatusListRemoveAllItem = new ToolStripMenuItem();
            btnStatusListRemoveAllItem.Name = "btnStatusListRemoveAllItem";
            btnStatusListRemoveAllItem.Text = "停止并删除所有";
            btnStatusListRemoveAllItem.Click += BtnStatusListRemoveAllItem_Click;

            ToolStripMenuItem btnStatusListRedoItem = new ToolStripMenuItem();
            btnStatusListRedoItem.Name = "btnStatusListRedoItem";
            btnStatusListRedoItem.Text = "重新处理选定";
            btnStatusListRedoItem.Click += BtnStatusListRedoItem_Click;

            ToolStripMenuItem btnStatusListRedoAllFailedItem = new ToolStripMenuItem();
            btnStatusListRedoAllFailedItem.Name = "btnStatusListRedoAllFailedItem";
            btnStatusListRedoAllFailedItem.Text = "重新处理全部失败";
            btnStatusListRedoAllFailedItem.Click += BtnStatusListRedoAllFailedItem_Click;

            _statusContextMenuStrip = new ContextMenuStrip();
            _statusContextMenuStrip.Items.Add(btnStatusListRemoveFinishedItem);
            _statusContextMenuStrip.Items.Add(btnStatusListRemoveItem);
            _statusContextMenuStrip.Items.Add(btnStatusListRemoveAllItem);
            _statusContextMenuStrip.Items.Add(btnStatusListRedoItem);
            _statusContextMenuStrip.Items.Add(btnStatusListRedoAllFailedItem);

            processListView.ContextMenuStrip = _statusContextMenuStrip;
        }

        /// <summary>
        /// 重新处理选中项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStatusListRedoItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                processListView.Invoke(new Action(() =>
                {
                    var items = processListView.SelectedItems;

                    for (int i = items.Count - 1; i >= 0; i--)
                    {
                        var item = items[i];
                        var status = (SFTPProcessTask)item.Tag;

                        if (status.Direct == "上传")
                        {
                            AddUploadFileTask(status.SourceFilePath, status.FileName, status.DestPath, status.TotalSize);
                        }
                        else if (status.Direct == "下载")
                        {
                            AddDownloadFileTask(status.SourceFilePath, status.FileName, status.DestPath, status.TotalSize);
                        }
                    }
                }));
            });
        }

        /// <summary>
        /// 重新处理所有失败
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStatusListRedoAllFailedItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                processListView.Invoke(new Action(() =>
                {
                    processListView.BeginUpdate();

                    var items = processListView.Items;

                    for (int i = items.Count - 1; i >= 0; i--)
                    {
                        var item = processListView.Items[i];
                        var status = (SFTPProcessTask)item.Tag;

                        if (status.ProcessStatus == -1)
                        {
                            if (status.Direct == "上传")
                            {
                                AddUploadFileTask(status.SourceFilePath, status.FileName, status.DestFilePath, status.TotalSize);
                            }
                            else if (status.Direct == "下载")
                            {
                                AddDownloadFileTask(status.SourceFilePath, status.FileName, status.DestFilePath, status.TotalSize);
                            }
                        }
                    }

                    processListView.EndUpdate();
                }));
            });
        }

        /// <summary>
        /// 删除全部已完成
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStatusListRemoveFinishedItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                processListView.Invoke(new Action(() =>
                {
                    processListView.BeginUpdate();

                    for (int i = processListView.Items.Count - 1; i >= 0; i--)
                    {
                        var item = processListView.Items[i];
                        var status = (SFTPProcessTask)item.Tag;
                        if (status.ProcessStatus == 1)
                        {
                            processListView.Items.RemoveAt(i);
                            item = null;
                        }
                    }

                    processListView.EndUpdate();
                }));
            });
        }

        /// <summary>
        /// 停止并删除所有
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStatusListRemoveAllItem_Click(object sender, EventArgs e)
        {
            Task.Factory.StartNew(() =>
            {
                processListView.Invoke(new Action(() =>
                {
                    processListView.BeginUpdate();

                    for (int i = processListView.Items.Count - 1; i >= 0; i--)
                    {
                        var item = processListView.Items[i];
                        var task = (SFTPProcessTask)item.Tag;
                        if (task.ProcessStatus == 0) task.ProcessStatus = -2;
                        processListView.Items.Remove(item);
                        item = null;
                    }

                    processListView.EndUpdate();
                }));

            });
        }

        /// <summary>
        /// 删除选定项
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnStatusListRemoveItem_Click(object sender, EventArgs e)
        {
            var t = Task.Factory.StartNew(() =>
            {
                processListView.Invoke(new Action(() =>
                {
                    var items = processListView.SelectedItems;

                    processListView.BeginUpdate();

                    for (int i = items.Count - 1; i >= 0; i--)
                    {
                        var item = items[i];
                        var task = (SFTPProcessTask)item.Tag;
                        if (task.ProcessStatus == 0) task.ProcessStatus = -2;
                        processListView.Items.Remove(item);
                        item = null;
                    }

                    processListView.EndUpdate();
                }));
            });
        }
        #endregion
    }
}
