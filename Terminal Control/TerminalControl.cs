using System;
using System.Resources;
using System.Text;
using System.Drawing;
using System.Collections;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Win32;

using Poderosa;
using Poderosa.Toolkit;
using Poderosa.Connection;
using Poderosa.ConnectionParam;
using Poderosa.Terminal;
using Poderosa.Forms;
using Poderosa.Communication;
using Poderosa.Config;
using Poderosa.MacroEnv;
using Poderosa.Text;
using Poderosa.UI;
using Granados.SSHC;

namespace WalburySoftware
{
    public class TerminalControl : Control
    {
        #region Event
        public delegate void ConnectedEventHandle(object sneder, EventArgs args);
        public delegate void DisonnectedEventHandle(object sneder, EventArgs args);

        public event ConnectedEventHandle OnConnected;
        public event DisonnectedEventHandle OnDisconnected;
        #endregion

        #region fields
        private string _username = "";
        private string _password = "";
        private string _hostname = "";
        private int _port = 22;
        private TerminalPane _terminalPane;
        private Label statusText;
        #endregion

        #region Constructors
        public TerminalControl(string userName, string password, string hostname, int port)
        {
            this._hostname = hostname;
            this._port = port;
            this._password = password;
            this._username = userName;
        }

        public TerminalControl()
        {

        }
        #endregion

        #region methods
        public void ReConnect()
        {
            Poderosa.ConnectionParam.LogType logType = Poderosa.ConnectionParam.LogType.Default;

            if (this._terminalPane.Connection != null)
            {
                logType = this._terminalPane.Connection.LogType;
                this._terminalPane.Connection.Close();
                this._terminalPane.Detach();
            }

            Connect();
        }

        public void Connect()
        {
            // TODO:先判断连接状态

            if (statusText == null)
            {
                statusText = new Label();
                statusText.Dock = DockStyle.Fill;
                statusText.BackColor = Color.White;
                statusText.ForeColor = Color.Black;
                statusText.Font = new Font("Microsoft YaHei", 10);
                statusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            }

            statusText.Text = "正在连接";
            statusText.Visible = true;
            this.Invalidate();

            this.Controls.Add(statusText);

            if (GApp._frame == null)
            {
                GApp.Run(new string[0]);
                GApp.Options.BGColor = this.BackColor;
                GApp.Options.TextColor = this.ForeColor;
                GApp.Options.RightButtonAction = RightButtonAction.Paste;
                GApp.Options.AutoCopyByLeftButton = true;
                GApp.Options.Font = this.Font;
                GApp._frame._multiPaneControl.InitUI(null, GApp.Options);
                GEnv.InterThreadUIService.MainFrameHandle = GApp._frame.Handle;
            }

            try
            {
                //------------------------------------------------------------------------
                SSHTerminalParam sshp = new SSHTerminalParam(Poderosa.ConnectionParam.ConnectionMethod.SSH2, this.Host, this.UserName, this.Password);
                sshp.AuthType = AuthType.Password;
                sshp.IdentityFile = string.Empty;
                sshp.Encoding = EncodingType.UTF8;
                sshp.Port = this._port;
                sshp.RenderProfile = new RenderProfile();
                sshp.TerminalType = TerminalType.XTerm;

                CommunicationUtil.SilentClient s = new CommunicationUtil.SilentClient();
                Size sz = this.Size;
                SocketWithTimeout swt = new SSHConnector(sshp, sz, sshp.Passphrase, null);
                swt.AsyncConnect(s, sshp.Host, sshp.Port);
                ConnectionTag ct = s.Wait(swt);

                if (ct == null)
                {
                    var errMsg = s.GetErrorMessage();
                    statusText.Text = "连接异常：" + errMsg;
                    return;
                }

                if (this._terminalPane == null)
                {
                    this._terminalPane = new TerminalPane();
                }
                else
                {
                    this._terminalPane.Detach();
                }

                this._terminalPane.Dock = DockStyle.Fill;
                this._terminalPane.ForeColor = this.ForeColor;
                this._terminalPane.BackColor = this.BackColor;
                this._terminalPane.FakeVisible = true;
                this._terminalPane.Attach(ct);
                ct.Receiver.Listen();

                if (this._terminalPane.Connection != null)
                {
                    statusText.Visible = false;
                    this.Controls.Add(this._terminalPane);

                    if (this._terminalPane.Connection != null)
                    {
                        this._terminalPane.Connection.ClosedEvent += Connection_ClosedEvent;
                    }

                    OnConnected?.Invoke(this, new EventArgs());
                }
                else
                {
                    statusText.Visible = true;
                    statusText.Text = "连接已断开";

                    OnDisconnected?.Invoke(this, new EventArgs());
                }
            }
            catch (Exception ex)
            {
                return;
            }
        }

        private void Connection_ClosedEvent(object sender, EventArgs args)
        {
            statusText.Invoke(new Action(() =>
                    {
                        statusText.Text = "连接已断开";
                        statusText.Visible = true;

                        OnDisconnected?.Invoke(this, args);
                    }
                ));
        }

        public void Disconnect()
        {
            if (this._terminalPane.Connection != null)
            {
                this._terminalPane.Connection.Close();
                this._terminalPane.Detach();
            }
        }

        public void SendText(string command)
        {
            if (this._terminalPane.Connection != null)
            {
                this._terminalPane.Connection.WriteChars(command.ToCharArray());
            }
        }

        public string GetLastLine()
        {
            return new string(this._terminalPane.Document.LastLine.Text);
        }

        /// <summary>
        /// Create New Log
        /// </summary>
        /// <param name="logType">I guess just use Default all the time here</param>
        /// <param name="File">This should be a full path. Example: @"C:\Temp\logfilename.txt"</param>
        /// <param name="append">Set this to true</param>
        public void SetLog(LogType logType, string File, bool append)
        {
            // make sure directory exists
            string dir = File.Substring(0, File.LastIndexOf(@"\"));
            if (!System.IO.Directory.Exists(dir))
                System.IO.Directory.CreateDirectory(dir);

            this._terminalPane.Connection.ResetLog((Poderosa.ConnectionParam.LogType)logType, File, append);
        }

        public void SetPaneColors(Color TextColor, Color BackColor)
        {
            RenderProfile prof = this._terminalPane.ConnectionTag.RenderProfile;
            prof.BackColor = BackColor;
            prof.ForeColor = TextColor;

            this._terminalPane.ApplyRenderProfile(prof);
        }

        public void CopySelectedTextToClipboard()
        {
            if (GEnv.TextSelection.IsEmpty) return;

            string t = GEnv.TextSelection.GetSelectedText();
            if (t.Length > 0)
                Clipboard.SetDataObject(t, false);
        }

        public void PasteTextFromClipboard()
        {
            string value = (string)Clipboard.GetDataObject().GetData("Text");
            if (value == null || value.Length == 0 || this._terminalPane == null || this._terminalPane.ConnectionTag == null) return;

            PasteProcessor p = new PasteProcessor(this._terminalPane.ConnectionTag, value);
            p.Perform();
        }
        #endregion

        #region Properties
        public TerminalPane TerminalPane
        {
            get
            {
                return this._terminalPane;
            }
        }

        public string UserName
        {
            get
            {
                return this._username;
            }
            set
            {
                this._username = value;
            }
        }

        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                this._password = value;
            }
        }

        public string Host
        {
            get
            {
                return this._hostname;
            }
            set
            {
                this._hostname = value;
            }
        }

        public int Port
        {
            get
            {
                return this._port;
            }
            set
            {
                this._port = value;
            }
        }

        public int ScrollBackBuffer
        {
            get
            {
                return GApp.Options.TerminalBufferSize;
            }
            set
            {
                GApp.Options.TerminalBufferSize = value;
            }
        }

        public bool IsConnected
        {
            get
            {
                if (this._terminalPane != null && this._terminalPane.Connection != null)
                {
                    return !this._terminalPane.Connection.IsClosed;
                }

                return false;
            }
        }
        #endregion

        #region overrides
        protected override void OnGotFocus(EventArgs e)
        {
            base.OnGotFocus(e);

            if (this._terminalPane != null)
            {
                this._terminalPane.Focus();
            }
        }
        #endregion
    }

    #region enums
    public enum ConnectionMethod
    {
        /// <summary>
        /// Telnet
        /// </summary>
        Telnet,
        /// <summary>
        /// SSH1
        /// </summary>
        SSH1,
        /// <summary>
        /// SSH2
        /// </summary>
        SSH2
    }

    public enum LogType
    {
        /// <summary>
        /// The log is not recorded.
        /// </summary>
        [EnumValue(Description = "Enum.LogType.None")]
        None,

        /// <summary>
        /// The log is a plain text file. This is standard.
        /// </summary>
        [EnumValue(Description = "Enum.LogType.Default")]
        Default,

        /// <summary>
        /// The log is a binary file.
        /// </summary>
        [EnumValue(Description = "Enum.LogType.Binary")]
        Binary,

        /// <summary>
        /// The log is an XML file. We may ask you to record the log in this type for debugging.
        /// </summary>
        [EnumValue(Description = "Enum.LogType.Xml")]
        Xml
    }
    #endregion
}
