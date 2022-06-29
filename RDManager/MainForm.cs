using RDManager.DAL;
using RDManager.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Xml.Linq;

namespace RDManager
{
    public partial class MainForm : Form
    {
        private RDSDataNode currentTreeNode;
        private Panel currentRDPanel;
        private Dictionary<string, Panel> rdPanelDictionary;
        private ContextMenuStrip rightButtonMenu;
        const int scrollRegion = 25;
        private System.Threading.Timer noOperatorCountTimer;

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int wMsg, int wParam, int lParam);

        static int iOperCount = 0;

        public MainForm()
        {
            InitializeComponent();

            OperateMessageFilter msg = new OperateMessageFilter();
            Application.AddMessageFilter(msg);
            noOperatorCountTimer = new System.Threading.Timer(new System.Threading.TimerCallback(state =>
              {
                  iOperCount++;
                  if (iOperCount > 450)
                  {
                      Application.Exit();
                  }
              }), null, 10000, 2000);
        }

        private class OperateMessageFilter : IMessageFilter
        {
            public bool PreFilterMessage(ref Message m)
            {
                if (m.Msg == 0x0200 || m.Msg == 0x0201 || m.Msg == 0x0204 || m.Msg == 0x0207)
                {
                    iOperCount = 0;
                }

                return false;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitUser();

            InitForm();

            InitRightButtonMemnu();

            InitTreeView();

            InitRemoteTerminal();
        }

        private void InitUser()
        {

        }

        private void InitForm()
        {
            this.WindowState = FormWindowState.Maximized;
        }

        #region 远程终端操作
        private void InitRemoteTerminal()
        {
            rdPanelDictionary = new Dictionary<string, Panel>();
            currentRDPanel = defaultRDPanel;
        }

        /// <summary>
        /// 更换当前Panel
        /// </summary>
        /// <param name="node"></param>
        private void ChangeCurrentRDPanel(RDSDataNode node)
        {
            // 移除当前Panel
            if (currentRDPanel != null)
            {
                this.splitContainer1.Panel2.Controls.Remove(currentRDPanel);
            }

            Panel rdPanel = null;
            var panelID = "panel_" + node.Tag.ToString();

            // 如果Panel已经存在，直接加载Panel，否则创建新的Panel
            if (rdPanelDictionary.ContainsKey(panelID))
            {
                rdPanel = rdPanelDictionary[panelID];
            }

            if (rdPanel == null)
            {
                rdPanel = new Panel();
                rdPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                rdPanel.Location = new System.Drawing.Point(0, 0);
                rdPanel.Name = panelID;
                rdPanel.TabIndex = 0;
                rdPanel.GotFocus += RdPanel_GotFocus;
            }

            this.splitContainer1.Panel2.Controls.Add(rdPanel);

            if (!rdPanelDictionary.ContainsKey(rdPanel.Name))
            {
                rdPanelDictionary.Add(rdPanel.Name, rdPanel);
            }

            currentRDPanel = rdPanel;
            currentRDPanel.Focus();
        }

        private void RdPanel_GotFocus(object sender, EventArgs e)
        {
            var panel = (Panel)sender;
            if (panel != null && panel.Controls != null && panel.Controls.Count > 0)
            {
                if (panel.Controls[0] is SSHControl)
                {
                    panel.Controls[0].Focus();
                }
            }
        }

        /// <summary>
        /// 连接远程终端
        /// </summary>
        /// <param name="server"></param>
        /// <param name="connectedAction"></param>
        private void ConnectRemoteTerminal(RDSDataNode node)
        {
            ChangeCurrentRDPanel(node);

            var server = (RDSServer)node.RDSData;
            if (server.OpType == "Linux")
            {
                ConnectSSH(server);
            }
            else
            {
                if (server.LinkType == "SSH2")
                {
                    ConnectSSH(server);
                }
                else
                {
                    ConnectWindowsDesktop(server);
                }
            }
        }

        /// <summary>
        /// 连接SSH
        /// </summary>
        /// <param name="server"></param>
        private void ConnectSSH(RDSServer server)
        {
            var parent = currentRDPanel;
            SSHControl ssh = null;

            // todo 先试试看看TCP连接能不能建立

            if (parent.HasChildren)
            {
                ssh = (SSHControl)currentRDPanel.Controls[0];
                ssh.Tag = server.ServerID.ToString();

                if (!ssh.IsConnected)
                {
                    ssh.Connect();
                    ssh.Focus();
                }
            }
            else
            {
                ssh = new SSHControl(server);
                ssh.Tag = server.ServerID.ToString();
                ssh.Dock = System.Windows.Forms.DockStyle.Fill;
                ssh.OnConnected += Ssh_OnConnected;
                ssh.OnDisconnected += Ssh_OnDisconnected;
                parent.Controls.Add(ssh);

                ssh.Connect();
                ssh.Focus();
            }
        }

        /// <summary>
        /// 连接Windows远程桌面
        /// </summary>
        /// <param name="server"></param>
        private void ConnectWindowsDesktop(RDSServer server)
        {
            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = null;

            // 如果Panel中包含子控件，则让远程桌面控件启动连接
            if (currentRDPanel.HasChildren)
            {
                rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];

                // About Connected https://msdn.microsoft.com/en-us/library/aa382835(v=vs.85).aspx
                if (rdp.Connected == 0)
                {
                    // 防止服务器相关参数变更
                    rdp.Tag = server.ServerID.ToString();
                    rdp.Name = "rdp_" + server.ServerID.ToString();
                    rdp.Server = server.ServerAddress;
                    rdp.AdvancedSettings9.RDPPort = server.ServerPort;
                    rdp.UserName = server.UserName;
                    rdp.AdvancedSettings9.ClearTextPassword = EncryptUtils.DecryptServerPassword(server);

                    rdp.Connect();
                }
            }

            // 如果远程桌面控件不存在，则创建
            if (rdp == null)
            {
                rdp = new AxMSTSCLib.AxMsRdpClient9NotSafeForScripting();
                rdp.Width = this.splitContainer1.Panel2.Width;
                rdp.Height = this.splitContainer1.Panel2.Height;
                rdp.Dock = System.Windows.Forms.DockStyle.None;

                currentRDPanel.Controls.Add(rdp);

                rdp.Tag = server.ServerID.ToString();
                rdp.Name = "rdp_" + server.ServerID.ToString();
                rdp.Server = server.ServerAddress;
                rdp.UserName = server.UserName;
                rdp.CausesValidation = false;
                rdp.AdvancedSettings9.EnableCredSspSupport = true;
                rdp.AdvancedSettings9.RDPPort = server.ServerPort;
                rdp.AdvancedSettings9.ClearTextPassword = EncryptUtils.DecryptServerPassword(server);
                rdp.AdvancedSettings9.BandwidthDetection = true;
                rdp.AdvancedSettings.allowBackgroundInput = 1;
                rdp.ColorDepth = 32;
                rdp.ConnectingText = "正在连接";
                rdp.DisconnectedText = "连接已断开";
                rdp.OnConnected += Rdp_OnConnected;
                rdp.OnDisconnected += Rdp_OnDisconnected;
                rdp.OnFatalError += Rdp_OnFatalError;
                rdp.Connect();
            }
        }

        private void Rdp_OnFatalError(object sender, AxMSTSCLib.IMsTscAxEvents_OnFatalErrorEvent e)
        {
            //0
            //An unknown error has occurred.
            //1
            //Internal error code 1.
            //2
            //An out-of - memory error has occurred.
            //3
            //A window-creation error has occurred.
            //4
            //Internal error code 2.
            //5
            //Internal error code 3.This is not a valid state.
            //6
            //Internal error code 4.
            //7
            //An unrecoverable error has occurred during client connection.
            //100
            //Winsock initialization error.

            var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)sender;
            rdp.DisconnectedText = "连接已断开，错误码: " + e.errorCode.ToString();
        }

        /// <summary>
        /// 远程桌面连接断开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rdp_OnDisconnected(object sender, AxMSTSCLib.IMsTscAxEvents_OnDisconnectedEvent e)
        {
            var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)sender;
            var nodeId = rdp.Tag.ToString();
            var node = (RDSDataNode)FindNode(nodeId, serverTree.Nodes[0]);

            node.ImageIndex = 1;
            node.SelectedImageIndex = 1;

            if (rdp.Parent != null)
            {
                string panelName = rdp.Parent.Name;
                rdp.Parent.Dispose();
                rdp.Parent = null;
                rdPanelDictionary.Remove(panelName);
            }
        }

        /// <summary>
        /// 远程桌面连接成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rdp_OnConnected(object sender, EventArgs e)
        {
            var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)sender;
            var nodeId = rdp.Tag.ToString();
            var node = (RDSDataNode)FindNode(nodeId, serverTree.Nodes[0]);

            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
        }

        /// <summary>
        /// SSH连接成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ssh_OnConnected(object sender, EventArgs e)
        {
            var rdp = (SSHControl)sender;
            var nodeId = rdp.Tag.ToString();
            var node = (RDSDataNode)FindNode(nodeId, serverTree.Nodes[0]);

            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
        }

        /// <summary>
        /// SSH连接断开事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ssh_OnDisconnected(object sender, EventArgs e)
        {
            var rdp = (SSHControl)sender;
            var nodeId = rdp.Tag.ToString();
            var node = (RDSDataNode)FindNode(nodeId, serverTree.Nodes[0]);

            if (rdp.InvokeRequired)
            {
                rdp.Invoke(new Action(() =>
                {
                    DoAfterDisconnect(node, rdp);
                }));
            }
            else
            {
                DoAfterDisconnect(node, rdp);
            }
        }

        private void DoAfterDisconnect(RDSDataNode node, SSHControl rdp)
        {
            node.ImageIndex = 1;
            node.SelectedImageIndex = 1;

            if (rdp.Parent != null)
            {
                string panelName = rdp.Parent.Name;
                rdp.Parent.Dispose();
                rdp.Parent = null;
                rdPanelDictionary.Remove(panelName);
            }
        }

        /// <summary>
        /// 断开远程终端连接
        /// </summary>
        /// <param name="server"></param>
        private void DisConRemoteTerminal(RDSDataNode node)
        {
            var panelName = "panel_" + node.Name.Replace("node_", "");
            if (rdPanelDictionary.ContainsKey(panelName))
            {
                var disConRDanel = rdPanelDictionary[panelName];
                DisconnectPanel(disConRDanel);
            }
        }

        /// <summary>
        /// 全屏显示远程桌面
        /// </summary>
        /// <param name="node"></param>
        private void ShowFullScreenRemoteDesktop(RDSDataNode node)
        {
            ChangeCurrentRDPanel(node);

            if (node.NodeType == RDSDataNodeType.Server)
            {
                var server = (RDSServer)node.RDSData;

                if (server.LinkType == "远程桌面")
                {
                    AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = null;

                    // 如果Panel中包含子控件，则让远程桌面控件启动连接
                    if (currentRDPanel.HasChildren)
                    {
                        rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];
                        rdp.FullScreenTitle = rdp.Server;
                        rdp.FullScreen = true;
                    }
                }
            }
        }

        /// <summary>
        /// 发送Ctrl-Alt-Del到远程桌面
        /// </summary>
        /// <param name="node"></param>
        private void SendCtrlAltDelToRemoteDesktop(RDSDataNode node)
        {
            ChangeCurrentRDPanel(node);

            if (node.NodeType == RDSDataNodeType.Server)
            {
                var server = (RDSServer)node.RDSData;

                if (server.LinkType == "远程桌面")
                {
                    AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = null;

                    // 如果Panel中包含子控件，则让远程桌面控件启动连接
                    if (currentRDPanel.HasChildren)
                    {
                        rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];

                        // About Connected https://msdn.microsoft.com/en-us/library/aa382835(v=vs.85).aspx
                        if (rdp.Connected == 1)
                        {
                            var ocx = (MSTSCLib.IMsRdpClientNonScriptable5)rdp.GetOcx();
                            bool[] bools = { false, false, false, true, true, true, };
                            int[] ints = { 0x1d, 0x38, 0x53, 0x53, 0x38, 0x1d };

                            rdp.Focus();
                            ocx.SendKeys(ints.Length, ref bools[0], ref ints[0]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 断开Panel连接
        /// </summary>
        /// <param name="rdpPanel"></param>
        private void DisconnectPanel(Panel rdpPanel)
        {
            if (rdpPanel != null && rdpPanel.Controls != null && rdpPanel.Controls.Count > 0)
            {
                if (rdpPanel.Controls[0] is SSHControl)
                {
                    var console = (SSHControl)rdpPanel.Controls[0];
                    if (console.IsConnected)
                    {
                        console.Disconnect();
                    }
                }
                else
                {
                    var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)rdpPanel.Controls[0];
                    if (rdp.Connected > 0)
                    {
                        rdp.Disconnect();
                    }
                }
            }
        }
        #endregion

        #region TreeView操作
        private void InitTreeView()
        {
            RDSDataManager dataManager = new RDSDataManager();
            var doc = dataManager.GetData();
            var root = doc.Root;

            RDSDataNode rootNode = new RDSDataNode();
            rootNode.Text = "远程桌面";
            rootNode.Tag = Guid.Empty;
            rootNode.NodeType = RDSDataNodeType.Group;
            rootNode.ContextMenuStrip = rightButtonMenu;
            rootNode.ImageIndex = 0;

            InitTreeNodes(root, rootNode);

            serverTree.Nodes.Add(rootNode);
            serverTree.ImageList = imageList1;

            rootNode.Expand();

            currentTreeNode = rootNode;
        }

        private void InitTreeNodes(XElement root, TreeNode parentNode)
        {
            var elements = root.Elements().Where(d => d.Attribute("name") != null).OrderBy(d => d.Attribute("name").Value);
            foreach (XElement element in elements)
            {
                if (element.Name == "group")
                {
                    RDSDataNode item = MakeGroupTreeNode(parentNode, element);
                    item.ImageIndex = 0;
                    item.SelectedImageIndex = 0;

                    parentNode.Nodes.Add(item);
                    InitTreeNodes(element, item);
                }
                else if (element.Name == "server")
                {
                    RDSDataNode item = MakeServerTreeNode(parentNode, element);
                    item.ImageIndex = 1;
                    item.SelectedImageIndex = 1;

                    parentNode.Nodes.Add(item);
                }
            }
        }

        private RDSDataNode MakeServerTreeNode(TreeNode parentItem, XElement element)
        {
            Guid id = new Guid(element.Attribute("id").Value);
            string name = element.Attribute("name").Value;
            string address = element.Attribute("address").Value;
            string port = element.Attribute("port").Value;
            string username = element.Attribute("username").Value;
            string password = element.Attribute("password").Value;
            string privateKey = element.Attribute("privatekey")?.Value;
            string keyPassPhrase = element.Attribute("keypassphrase")?.Value;
            string optype = element.Attributes().Any(d => d.Name == "optype") ? element.Attribute("optype").Value : string.Empty;
            string linktype = element.Attributes().Any(d => d.Name == "linktype") ? element.Attribute("linktype").Value : string.Empty;

            RDSDataNode item = new RDSDataNode();
            item.Name = "node_" + id;
            item.Text = name;
            item.Tag = id;
            item.ContextMenuStrip = rightButtonMenu;

            item.NodeType = RDSDataNodeType.Server;
            item.RDSData = new RDSServer()
            {
                ServerID = id,
                GroupID = (Guid)parentItem.Tag,
                ServerName = name,
                ServerAddress = address,
                ServerPort = int.Parse(port),
                UserName = username,
                Password = password,
                PrivateKey = privateKey,
                KeyPassPhrase = keyPassPhrase,
                OpType = optype,
                LinkType = linktype
            };
            return item;
        }

        private RDSDataNode MakeGroupTreeNode(TreeNode parentItem, XElement element)
        {
            Guid id = new Guid(element.Attribute("id").Value);
            string name = element.Attribute("name").Value;

            RDSDataNode item = new RDSDataNode();
            item.Name = "node_" + id;
            item.Text = name;
            item.Tag = id;
            item.ContextMenuStrip = rightButtonMenu;
            item.NodeType = RDSDataNodeType.Group;
            item.RDSData = new RDSGroup()
            {
                GroupID = id,
                GroupName = name,
                ParentGroupID = (Guid)parentItem.Tag
            };
            return item;
        }

        private void serverTree_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            bool isDoConnect = false;
            var treeNode = e.Node;

            if (treeNode != null)
            {
                treeNode.Checked = true;

                var node = (RDSDataNode)treeNode;
                currentTreeNode = node;

                if (node.NodeType == RDSDataNodeType.Server)
                {
                    isDoConnect = true;
                }

                if (isDoConnect)
                {
                    ConnectRemoteTerminal(node);
                }
            }
        }

        /// <summary>
        /// 拖动释放
        /// 参考：http://www.cnblogs.com/jiewei915/archive/2012/01/11/2318951.html
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverTree_DragDrop(object sender, DragEventArgs e)
        {
            // 得到拖放中的节点
            var moveDataNode = (RDSDataNode)e.Data.GetData("RDManager.Model.RDSDataNode");

            // 根据鼠标坐标确定要移动到的目标节点
            TreeNode targetNode = new TreeNode();
            Point pt = ((TreeView)(sender)).PointToClient(new Point(e.X, e.Y));
            targetNode = this.serverTree.GetNodeAt(pt);
            var targetDataNode = (RDSDataNode)targetNode;

            // 如果目标为自己，返回
            if (targetDataNode.Text == moveDataNode.Text)
            {
                return;
            }

            // 如果目标包含在自己的节点树中，返回            
            if (moveDataNode.Nodes.Contains(targetDataNode))
            {
                return;
            }
            else if (targetDataNode.FullPath.Contains(moveDataNode.FullPath))
            {
                return;
            }

            // 如果目标节点是Server，返回
            if (targetDataNode.NodeType == RDSDataNodeType.Server)
            {
                return;
            }

            // 如果目标为子节点，返回
            if (targetDataNode.Parent == moveDataNode)
            {
                return;
            }

            // 如果目标为父节点，返回
            if (targetDataNode == moveDataNode.Parent)
            {
                return;
            }

            // 移除拖放的节点
            RemoveServerTreeNode(moveDataNode, false);

            // 添加节点
            var newMoveNode = (RDSDataNode)moveDataNode.Clone();
            newMoveNode.RDSData = moveDataNode.RDSData;
            newMoveNode.NodeType = moveDataNode.NodeType;

            // 保存节点
            if (newMoveNode.NodeType == RDSDataNodeType.Group)
            {
                var group = (RDSGroup)newMoveNode.RDSData;
                group.ParentGroupID = (Guid)targetDataNode.Tag;
                RDSDataManager dataManager = new RDSDataManager();
                dataManager.AddGroup(group);
            }
            else
            {
                var server = (RDSServer)newMoveNode.RDSData;
                server.GroupID = (Guid)targetDataNode.Tag;
                RDSDataManager dataManager = new RDSDataManager();
                dataManager.SaveServer(server);
            }


            // 将节点插入到分组中
            targetDataNode.Nodes.Insert(targetDataNode.Nodes.Count, newMoveNode);

            // 更新当前拖动的节点选择
            serverTree.SelectedNode = newMoveNode;

            // 展开当前节点的父节点
            targetDataNode.Expand();
        }

        /// <summary>
        /// 拖动鼠标移动进入某个元素
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverTree_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent("RDManager.Model.RDSDataNode"))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// 开始拖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverTree_ItemDrag(object sender, ItemDragEventArgs e)
        {
            TreeNode tn = e.Item as TreeNode;

            if (e.Button == MouseButtons.Left && tn != null && tn.Parent != null)
            {
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        /// <summary>
        /// 拖动悬停
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void serverTree_DragOver(object sender, DragEventArgs e)
        {
            // 拖动过程中高亮鼠标滑过的节点
            TreeViewHitTestInfo hti = this.serverTree.HitTest(this.serverTree.PointToClient(new Point(e.X, e.Y)));
            this.serverTree.SelectedNode = hti.Node;

            //拖动过程中鼠标达到上下边界时滚动条自动调整            
            if (e.Y > (serverTree.Height + scrollRegion * 8))
            {
                //向下滚动
                SendMessage(serverTree.Handle, (int)277, (int)1, 0);
            }
            else if (e.Y < (serverTree.Top + scrollRegion))
            {
                //向上滚动
                SendMessage(serverTree.Handle, (int)277, (int)0, 0);
            }
        }

        /// <summary>
        /// 根据节点ID递归查找结点
        /// </summary>
        /// <param name="nodeId"></param>
        /// <param name="parentNode"></param>
        /// <returns></returns>
        private TreeNode FindNode(string nodeId, TreeNode parentNode)
        {
            TreeNodeCollection nodes = parentNode.Nodes;
            foreach (TreeNode n in nodes)
            {
                if (n.Tag.ToString() == nodeId)
                {
                    return n;
                }

                TreeNode temp = FindNode(nodeId, n);
                if (temp != null)
                {
                    return temp;
                }
            }

            return null;
        }

        #endregion

        #region 右键菜单
        private void InitRightButtonMemnu()
        {
            ToolStripMenuItem btnAddGroupItem = new ToolStripMenuItem();
            btnAddGroupItem.Name = "btnAddGroupItem";
            btnAddGroupItem.Text = "添加组";
            btnAddGroupItem.Click += AddGroup_Click;

            ToolStripMenuItem btnAddServerItem = new ToolStripMenuItem();
            btnAddServerItem.Name = "btnAddServerItem";
            btnAddServerItem.Text = "添加服务器";
            btnAddServerItem.Click += AddServer_Click;

            ToolStripMenuItem btnConnectServerItem = new ToolStripMenuItem();
            btnConnectServerItem.Name = "btnConnectServerItem";
            btnConnectServerItem.Text = "连接服务器";
            btnConnectServerItem.Click += ConnectServer_Click;

            ToolStripMenuItem btnDisconServerItem = new ToolStripMenuItem();
            btnDisconServerItem.Name = "btnDisconServerItem";
            btnDisconServerItem.Text = "断开服务器";
            btnDisconServerItem.Click += DisconServer_Click;

            ToolStripMenuItem btnFullScreenServerItem = new ToolStripMenuItem();
            btnFullScreenServerItem.Name = "btnFullScreenServerItem";
            btnFullScreenServerItem.Text = "全屏显示";
            btnFullScreenServerItem.Click += FullScreen_Click;

            ToolStripMenuItem btnCADServerItem = new ToolStripMenuItem();
            btnCADServerItem.Name = "btnCADServerItem";
            btnCADServerItem.Text = "发送Ctrl+Alt+Del";
            btnCADServerItem.Click += CAD_Click;

            ToolStripMenuItem btnEditServerItem = new ToolStripMenuItem();
            btnEditServerItem.Name = "btnEditServerItem";
            btnEditServerItem.Text = "编辑";
            btnEditServerItem.Click += Edit_Click;

            ToolStripMenuItem btnDeleteServerItem = new ToolStripMenuItem();
            btnDeleteServerItem.Name = "btnDeleteServerItem";
            btnDeleteServerItem.Text = "删除";
            btnDeleteServerItem.Click += Delete_Click;

            rightButtonMenu = new ContextMenuStrip();
            rightButtonMenu.Items.Add(btnAddGroupItem);
            rightButtonMenu.Items.Add(btnAddServerItem);
            rightButtonMenu.Items.Add(btnConnectServerItem);
            rightButtonMenu.Items.Add(btnDisconServerItem);
            rightButtonMenu.Items.Add(btnFullScreenServerItem);
            rightButtonMenu.Items.Add(btnCADServerItem);
            rightButtonMenu.Items.Add(btnEditServerItem);
            rightButtonMenu.Items.Add(btnDeleteServerItem);
        }

        private void serverTree_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 左键切换节点
            if (e.Button == MouseButtons.Left)
            {
                var treeNode = e.Node;
                if (treeNode != null)
                {
                    var node = (RDSDataNode)treeNode;
                    if (node.NodeType == RDSDataNodeType.Server)
                    {
                        ChangeCurrentRDPanel(node);
                    }
                }
            }

            // 右键处理节点右键菜单
            if (e.Button == MouseButtons.Right)
            {
                var treeNode = e.Node;
                if (treeNode != null)
                {
                    serverTree.SelectedNode = serverTree.GetNodeAt(e.X, e.Y);

                    var node = (RDSDataNode)treeNode;
                    currentTreeNode = node;

                    if (node.NodeType == RDSDataNodeType.Server)
                    {
                        ((ToolStripMenuItem)rightButtonMenu.Items[0]).Enabled = false;
                        ((ToolStripMenuItem)rightButtonMenu.Items[1]).Enabled = false;

                        // TODO:判断节点连接状态，如果未连接，则不能断开连接
                    }
                    else
                    {
                        ((ToolStripMenuItem)rightButtonMenu.Items[0]).Enabled = true;
                        ((ToolStripMenuItem)rightButtonMenu.Items[1]).Enabled = true;
                    }
                }
            }
        }

        private void FullScreen_Click(object sender, EventArgs e)
        {
            if (currentTreeNode != null)
            {
                if (currentTreeNode.NodeType == RDSDataNodeType.Server)
                {
                    ShowFullScreenRemoteDesktop(currentTreeNode);
                }
            }
        }

        private void CAD_Click(object sender, EventArgs e)
        {
            if (currentTreeNode != null)
            {
                if (currentTreeNode.NodeType == RDSDataNodeType.Server)
                {
                    SendCtrlAltDelToRemoteDesktop(currentTreeNode);
                }
            }
        }

        private void Delete_Click(object sender, EventArgs e)
        {
            RemoveServerTreeNode(currentTreeNode);
        }

        /// <summary>
        /// 移除节点
        /// </summary>
        /// <param name="node"></param>
        private void RemoveServerTreeNode(RDSDataNode node, bool removePanel = true)
        {
            var nodeID = (Guid)node.Tag;

            if (nodeID != Guid.Empty)
            {
                // 数据中移除
                RDSDataManager dataManager = new RDSDataManager();
                dataManager.Remove(nodeID);

                // 树形菜单中移除
                node.Remove();

                // Panel移除
                if (removePanel)
                {
                    var panelID = "panel_" + nodeID;
                    if (rdPanelDictionary.ContainsKey(panelID))
                    {
                        rdPanelDictionary.Remove(panelID);
                    }
                }
            }
        }

        private void Edit_Click(object sender, EventArgs e)
        {
            if (currentTreeNode == null)
            {
                return;
            }

            if (currentTreeNode.NodeType == RDSDataNodeType.Group)
            {
                var rdsData = (RDSGroup)currentTreeNode.RDSData;

                GroupEditForm groupEditWindow = new GroupEditForm();
                groupEditWindow.Model = rdsData;
                var result = groupEditWindow.ShowDialog();
                var model = groupEditWindow.Model.Clone();

                if (result == DialogResult.OK)
                {
                    currentTreeNode.Text = model.GroupName;
                    currentTreeNode.RDSData = model;
                }
            }
            else
            {
                var rdsData = (RDSServer)currentTreeNode.RDSData;

                ServerEditForm serverEditWindow = new ServerEditForm();
                serverEditWindow.Model = rdsData;
                var result = serverEditWindow.ShowDialog();
                var model = serverEditWindow.Model.Clone();

                if (result == DialogResult.OK)
                {
                    currentTreeNode.Text = model.ServerName;
                    currentTreeNode.RDSData = model;
                }
            }
        }

        private void AddGroup_Click(object sender, EventArgs e)
        {
            GroupEditForm groupEditWindow = new GroupEditForm();
            groupEditWindow.Model.ParentGroupID = (Guid)currentTreeNode.Tag;
            var result = groupEditWindow.ShowDialog();
            var model = groupEditWindow.Model.Clone();

            if (result == DialogResult.OK)
            {
                var newNode = new RDSDataNode()
                {
                    Name = "node_" + model.GroupID,
                    Text = model.GroupName,
                    Tag = model.GroupID,
                    RDSData = model,
                    ContextMenuStrip = rightButtonMenu,
                    NodeType = RDSDataNodeType.Group,
                    ImageIndex = 0,
                    SelectedImageIndex = 0
                };

                currentTreeNode.Nodes.Add(newNode);
                currentTreeNode.Expand();
            }
        }

        private void AddServer_Click(object sender, EventArgs e)
        {
            ServerEditForm serverEditWindow = new ServerEditForm();
            serverEditWindow.Model.GroupID = (Guid)currentTreeNode.Tag;
            var result = serverEditWindow.ShowDialog();
            var model = serverEditWindow.Model.Clone();

            if (result == DialogResult.OK)
            {
                var newNode = new RDSDataNode()
                {
                    Name = "node_" + model.ServerID,
                    Text = model.ServerName,
                    Tag = model.ServerID,
                    RDSData = model,
                    ContextMenuStrip = rightButtonMenu,
                    NodeType = RDSDataNodeType.Server,
                    ImageIndex = 1,
                    SelectedImageIndex = 1
                };

                currentTreeNode.Nodes.Add(newNode);
                currentTreeNode.Expand();
            }
        }

        private void ConnectServer_Click(object sender, EventArgs e)
        {
            if (currentTreeNode != null)
            {
                if (currentTreeNode.NodeType == RDSDataNodeType.Server)
                {
                    ConnectRemoteTerminal(currentTreeNode);
                }
            }
        }

        private void DisconServer_Click(object sender, EventArgs e)
        {
            if (currentTreeNode != null)
            {
                if (currentTreeNode.NodeType == RDSDataNodeType.Server)
                {
                    DisConRemoteTerminal(currentTreeNode);
                }
            }
        }
        #endregion

        #region Split操作
        private void splitContainer1_SplitterMoved(object sender, SplitterEventArgs e)
        {
            if (currentRDPanel != null)
            {
                if (currentRDPanel.HasChildren)
                {
                    if (currentRDPanel.Controls[0] is SSHControl)
                    {
                        var console = (SSHControl)currentRDPanel.Controls[0];
                        console.Width = this.splitContainer1.Panel2.Width;
                        console.Height = this.splitContainer1.Panel2.Height;
                    }
                    else
                    {
                        var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];
                        rdp.Width = this.splitContainer1.Panel2.Width;
                        rdp.Height = this.splitContainer1.Panel2.Height;
                    }
                }
            }
        }
        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DoCloseForm(e);
        }

        /// <summary>
        /// 执行关闭窗口
        /// </summary>
        /// <param name="e"></param>
        private void DoCloseForm(FormClosingEventArgs e)
        {
            if (rdPanelDictionary.Count > 0)
            {
                for (int i = 0; i < rdPanelDictionary.Count; i++)
                {
                    var rdpPanel = rdPanelDictionary.ElementAt(i).Value;
                    DisconnectPanel(rdpPanel);
                }
            }
        }

        /// <summary>
        /// 关闭退出窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// 关于此工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("萤火虫远程终端管理工具 v2.0", "关于", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 解密加密的全部密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void decryptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 查询全部数据
            RDSDataManager dataManager = new RDSDataManager();
            var doc = dataManager.GetData();
            var root = doc.Root;

            RDSDataNode rootNode = new RDSDataNode();
            rootNode.Text = "远程桌面";
            rootNode.Tag = Guid.Empty;
            rootNode.NodeType = RDSDataNodeType.Group;
            rootNode.ContextMenuStrip = rightButtonMenu;
            rootNode.ImageIndex = 0;

            InitTreeNodes(root, rootNode);

            DecryptPassword(dataManager, rootNode);

            // 清除加密Key和密钥，下次登录需
            dataManager.SetSecrectKey(string.Empty);
            dataManager.SetInitTime(string.Empty);
            dataManager.SetPassword(string.Empty);

            MessageBox.Show("处理成功，请重新启动。", "处理成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        /// <summary>
        /// 递归解密全部密码
        /// </summary>
        /// <param name="dataManager"></param>
        /// <param name="parentNode"></param>
        private void DecryptPassword(RDSDataManager dataManager, RDSDataNode parentNode)
        {
            if (parentNode.Nodes == null && parentNode.Nodes.Count <= 0)
            {
                return;
            }

            // 恢复密码为明文
            foreach (RDSDataNode node in parentNode.Nodes)
            {
                if (node.NodeType == RDSDataNodeType.Server)
                {
                    var server = (RDSServer)node.RDSData;
                    server.Password = EncryptUtils.DecryptServerPassword(server);
                    server.KeyPassPhrase = EncryptUtils.DecryptServerKeyPassPhrase(server);
                    dataManager.SaveServer(server);
                }
                else
                {
                    DecryptPassword(dataManager, node);
                }
            }
        }
    }
}
