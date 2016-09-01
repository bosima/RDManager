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
using System.Xml.Linq;

namespace RDManager
{
    public partial class MainForm : Form
    {
        private RDSDataNode currentTreeNode;
        private Panel currentRDPanel;
        private Dictionary<string, Panel> rdPanelDictionary;
        private ContextMenuStrip rightButtonMenu;

        public MainForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitUser();

            InitForm();

            InitRightButtonMemnu();

            InitTreeView();

            InitRemoteDesktop();
        }

        private void InitUser()
        {

        }

        private void InitForm()
        {
            this.WindowState = FormWindowState.Maximized;
        }

        #region 远程桌面操作
        private void InitRemoteDesktop()
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
            else {
                rdPanel = new Panel();
                rdPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                rdPanel.Location = new System.Drawing.Point(0, 0);
                rdPanel.Name = panelID;
                rdPanel.TabIndex = 0;
            }

            this.splitContainer1.Panel2.Controls.Add(rdPanel);

            if (!rdPanelDictionary.ContainsKey(rdPanel.Name))
            {
                rdPanelDictionary.Add(rdPanel.Name, rdPanel);
            }

            currentRDPanel = rdPanel;
        }

        /// <summary>
        /// 连接远程桌面
        /// </summary>
        /// <param name="server"></param>
        /// <param name="connectedAction"></param>
        private void ConnectRemoteDesktop(RDSDataNode node)
        {
            ChangeCurrentRDPanel(node);

            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = null;
            var server = (RDSServer)node.RDSData;

            // 如果Panel中包含子控件，则让远程桌面控件启动连接
            if (currentRDPanel.HasChildren)
            {
                rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];

                // About Connected https://msdn.microsoft.com/en-us/library/aa382835(v=vs.85).aspx
                if (rdp.Connected == 0)
                {
                    // 防止服务器相关参数变更
                    rdp.Tag = node;
                    rdp.Name = "rdp_" + server.ServerID.ToString();
                    rdp.Server = server.ServerAddress;
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

                rdp.Tag = node;
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
            var node = (RDSDataNode)rdp.Tag;

            node.ImageIndex = 1;
            node.SelectedImageIndex = 1;
        }

        /// <summary>
        /// 远程桌面连接成功事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Rdp_OnConnected(object sender, EventArgs e)
        {
            var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)sender;
            var node = (RDSDataNode)rdp.Tag;

            node.ImageIndex = 2;
            node.SelectedImageIndex = 2;
        }

        /// <summary>
        /// 断开远程桌面连接
        /// </summary>
        /// <param name="server"></param>
        private void DisConRemoteDesktop(RDSDataNode node)
        {
            var panelName = "panel_" + node.Name.Replace("node_", "");
            if (rdPanelDictionary.ContainsKey(panelName))
            {
                var disConRDanel = rdPanelDictionary[panelName];
                var disConRD = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)disConRDanel.Controls[0];
                disConRD.Disconnect();
            }
        }

        /// <summary>
        /// 全屏显示远程桌面
        /// </summary>
        /// <param name="node"></param>
        private void ShowFullScreenRemoteDesktop(RDSDataNode node)
        {
            ChangeCurrentRDPanel(node);

            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = null;

            // 如果Panel中包含子控件，则让远程桌面控件启动连接
            if (currentRDPanel.HasChildren)
            {
                rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];
                rdp.FullScreenTitle = rdp.Server;
                rdp.FullScreen = true;
            }
        }

        /// <summary>
        /// 发送Ctrl-Alt-Del到远程桌面
        /// </summary>
        /// <param name="node"></param>
        private void SendCtrlAltDelToRemoteDesktop(RDSDataNode node)
        {
            ChangeCurrentRDPanel(node);

            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = null;

            // 如果Panel中包含子控件，则让远程桌面控件启动连接
            if (currentRDPanel.HasChildren)
            {
                rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];

                // About Connected https://msdn.microsoft.com/en-us/library/aa382835(v=vs.85).aspx
                if (rdp.Connected == 1)
                {
                    //try
                    //{
                    //    var ocxWrapper = new MsRdpClientNonScriptableWrapper(rdp.GetOcx());
                    //    bool[] bools = { false, false, false, true, true, true, };
                    //    int[] ints = { 0x1d, 0x38, 0x53, 0x53, 0x38, 0x1d };

                    //    rdp.Focus();
                    //    ocxWrapper.SendKeys(ints, bools);
                    //}
                    //catch (Exception ex)
                    //{
                    //    MessageBox.Show(ex.Message);
                    //}

                    var ocx = (MSTSCLib.IMsRdpClientNonScriptable5)rdp.GetOcx();
                    bool[] bools = { false, false, false, true, true, true, };
                    int[] ints = { 0x1d, 0x38, 0x53, 0x53, 0x38, 0x1d };

                    rdp.Focus();
                    ocx.SendKeys(ints.Length, ref bools[0], ref ints[0]);
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
                Password = password
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
                    ConnectRemoteDesktop(node);
                }
            }
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
            var nodeID = (Guid)currentTreeNode.Tag;

            if (nodeID != Guid.Empty)
            {
                // 数据中移除
                RDSDataManager dataManager = new RDSDataManager();
                dataManager.Remove(nodeID);

                // 树形菜单中移除
                currentTreeNode.Remove();

                // Panel移除
                var panelID = "panel_" + nodeID;
                if (rdPanelDictionary.ContainsKey(panelID))
                {
                    rdPanelDictionary.Remove(panelID);
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
                    ConnectRemoteDesktop(currentTreeNode);
                }
            }
        }

        private void DisconServer_Click(object sender, EventArgs e)
        {
            if (currentTreeNode != null)
            {
                if (currentTreeNode.NodeType == RDSDataNodeType.Server)
                {
                    DisConRemoteDesktop(currentTreeNode);
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
                    var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];
                    rdp.Width = this.splitContainer1.Panel2.Width;
                    rdp.Height = this.splitContainer1.Panel2.Height;
                }
            }
        }
        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            bool isConnected = false;

            if (rdPanelDictionary.Count > 0)
            {
                foreach (var key in rdPanelDictionary.Keys)
                {
                    var rdpPanel = rdPanelDictionary[key];
                    if (rdpPanel != null && rdpPanel.Controls != null && rdpPanel.Controls.Count > 0)
                    {
                        var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)rdpPanel.Controls[0];
                        if (rdp.Connected > 0)
                        {
                            isConnected = true;
                        }
                    }
                }
            }

            if (isConnected)
            {
                var result = MessageBox.Show("还有活动的服务器连接，确定关闭此工具吗？", "关闭提醒", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                if (result == DialogResult.OK)
                {
                    if (rdPanelDictionary.Count > 0)
                    {
                        foreach (var key in rdPanelDictionary.Keys)
                        {
                            var rdpPanel = rdPanelDictionary[key];
                            if (rdpPanel != null && rdpPanel.Controls != null && rdpPanel.Controls.Count > 0)
                            {
                                var rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)rdpPanel.Controls[0];
                                if (rdp.Connected > 0)
                                {
                                    rdp.Disconnect();
                                }
                            }
                        }
                    }
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
