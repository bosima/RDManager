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

            InitForm();

            InitRightButtonMemnu();

            InitTreeView();

            InitRemoteDesktop();
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
        /// <param name="server"></param>
        private void ChangeCurrentRDPanel(RDSServer server)
        {
            // 移除当前Panel
            if (currentRDPanel != null)
            {
                this.splitContainer1.Panel2.Controls.Remove(currentRDPanel);
            }

            Panel rdPanel = null;

            // 如果Panel已经存在，直接加载Panel，否则创建新的Panel
            if (rdPanelDictionary.ContainsKey(server.ServerAddress))
            {
                rdPanel = rdPanelDictionary[server.ServerAddress];
            }
            else {
                rdPanel = new Panel();
                rdPanel.Dock = System.Windows.Forms.DockStyle.Fill;
                rdPanel.Location = new System.Drawing.Point(0, 0);
                rdPanel.Name = server.ServerAddress;
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
        private void ConnectRemoteDesktop(RDSServer server)
        {
            ChangeCurrentRDPanel(server);

            AxMSTSCLib.AxMsRdpClient9NotSafeForScripting rdp = null;

            // 如果Panel中包含子控件，则让远程桌面控件启动连接
            if (currentRDPanel.HasChildren)
            {
                rdp = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)currentRDPanel.Controls[0];

                // About Connected https://msdn.microsoft.com/en-us/library/aa382835(v=vs.85).aspx
                if (rdp.Connected == 0)
                {
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

                rdp.Server = server.ServerAddress;
                rdp.UserName = server.UserName;
                rdp.CausesValidation = false;
                rdp.AdvancedSettings9.EnableCredSspSupport = true;
                rdp.AdvancedSettings9.RDPPort = server.ServerPort;
                rdp.AdvancedSettings9.ClearTextPassword = server.Password;
                rdp.AdvancedSettings9.BandwidthDetection = true;
                rdp.ColorDepth = 32;
                rdp.ConnectingText = "正在连接";
                rdp.DisconnectedText = "连接已断开";
                rdp.Connect();
            }
        }

        /// <summary>
        /// 断开远程桌面连接
        /// </summary>
        /// <param name="server"></param>
        private void DisConRemoteDesktop(RDSServer server)
        {
            if (rdPanelDictionary.ContainsKey(server.ServerAddress))
            {
                var disConRDanel = rdPanelDictionary[server.ServerAddress];
                var disConRD = (AxMSTSCLib.AxMsRdpClient9NotSafeForScripting)disConRDanel.Controls[0];
                disConRD.Disconnect();
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

            InitTreeNodes(root, rootNode);

            serverTree.Nodes.Add(rootNode);
            rootNode.Expand();
            currentTreeNode = rootNode;
        }

        private void InitTreeNodes(XElement root, TreeNode parentNode)
        {
            foreach (XElement element in root.Elements())
            {
                if (element.Name == "group")
                {
                    RDSDataNode item = MakeGroupTreeNode(parentNode, element);

                    parentNode.Nodes.Add(item);
                    InitTreeNodes(element, item);
                }
                else
                {
                    // TODO:给节点增加一个连接状态图标
                    RDSDataNode item = MakeServerTreeNode(parentNode, element);
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
                    RDSServer server = (RDSServer)node.RDSData;
                    ConnectRemoteDesktop(server);
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

            // TODO:添加一个全屏按钮

            // TODO:添加一个编辑按钮

            ToolStripMenuItem btnDeleteServerItem = new ToolStripMenuItem();
            btnDeleteServerItem.Name = "btnDeleteServerItem";
            btnDeleteServerItem.Text = "删除";
            btnDeleteServerItem.Click += Delete_Click;

            rightButtonMenu = new ContextMenuStrip();
            rightButtonMenu.Items.Add(btnAddGroupItem);
            rightButtonMenu.Items.Add(btnAddServerItem);
            rightButtonMenu.Items.Add(btnConnectServerItem);
            rightButtonMenu.Items.Add(btnDisconServerItem);
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
                    RDSServer server = (RDSServer)node.RDSData;
                    ChangeCurrentRDPanel(server);
                }
            }

            // 右键处理节点右键菜单
            if (e.Button == MouseButtons.Right)
            {
                var treeNode = e.Node;
                if (treeNode != null)
                {
                    treeNode.Checked = true;

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

        private void Delete_Click(object sender, EventArgs e)
        {
            // TODO:删除节点及下级节点

            MessageBox.Show("暂未实现");
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
                    Text = model.GroupName,
                    Tag = model.GroupID,
                    RDSData = model,
                    ContextMenuStrip = rightButtonMenu,
                    NodeType = RDSDataNodeType.Group
                };

                var selectNode = (RDSDataNode)serverTree.SelectedNode;
                selectNode.Nodes.Add(newNode);
                selectNode.Expand();
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
                    Text = model.ServerName,
                    Tag = model.ServerID,
                    RDSData = model,
                    ContextMenuStrip = rightButtonMenu,
                    NodeType = RDSDataNodeType.Server
                };

                var selectNode = (RDSDataNode)serverTree.SelectedNode;
                selectNode.Nodes.Add(newNode);
                selectNode.Expand();
            }
        }

        private void ConnectServer_Click(object sender, EventArgs e)
        {
            if (currentTreeNode != null)
            {
                if (currentTreeNode.NodeType == RDSDataNodeType.Server)
                {
                    RDSServer server = (RDSServer)currentTreeNode.RDSData;
                    ConnectRemoteDesktop(server);
                }

                // TODO:设置节点选中状态
            }
        }

        private void DisconServer_Click(object sender, EventArgs e)
        {
            if (currentTreeNode != null)
            {
                if (currentTreeNode.NodeType == RDSDataNodeType.Server)
                {
                    RDSServer server = (RDSServer)currentTreeNode.RDSData;
                    DisConRemoteDesktop(server);
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
    }
}
