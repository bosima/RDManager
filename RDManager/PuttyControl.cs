using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RDManager
{
    public delegate void SSHExited();
    public delegate void SSHConnected(object sender, EventArgs e);
    public delegate void SSHDisconnected(object sender, EventArgs e);

    /// <summary>
    /// Putty SSH 控件
    /// </summary>
    public partial class PuttyControl : UserControl
    {
        Process puttyProcess;
        bool isSSHExited = false;

        public event SSHExited OnExited;
        public event SSHConnected OnConnected;
        public event SSHDisconnected OnDisconnected;

        public PuttyControl()
        {
            InitializeComponent();
        }

        // Reference:https://www.cnblogs.com/hackpig/p/5783604.html

        #region Windows API
        [DllImport("user32.dll")]
        private static extern int SetParent(IntPtr hWndChild, IntPtr hWndParent);

        [DllImport("user32.dll")]
        private static extern bool ShowWindowAsync(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        private static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        [DllImport("user32.dll")]
        private static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern bool ShowWindow(IntPtr hWnd, short State);

        private const int HWND_TOP = 0x0;
        private const int WM_COMMAND = 0x0112;
        private const int WM_QT_PAINT = 0xC2DC;
        private const int WM_PAINT = 0x000F;
        private const int WM_SIZE = 0x0005;
        private const int SWP_FRAMECHANGED = 0x0020;
        public enum ShowWindowStyles : short
        {
            SW_HIDE = 0,
            SW_SHOWNORMAL = 1,
            SW_NORMAL = 1,
            SW_SHOWMINIMIZED = 2,
            SW_SHOWMAXIMIZED = 3,
            SW_MAXIMIZE = 3,
            SW_SHOWNOACTIVATE = 4,
            SW_SHOW = 5,
            SW_MINIMIZE = 6,
            SW_SHOWMINNOACTIVE = 7,
            SW_SHOWNA = 8,
            SW_RESTORE = 9,
            SW_SHOWDEFAULT = 10,
            SW_FORCEMINIMIZE = 11,
            SW_MAX = 11
        }
        #endregion

        public void Connect(string ip, int port, string userName, string password, string privateKey,string keyPassPhrase)
        {
            // putty command line: https://the.earth.li/~sgtatham/putty/0.76/htmldoc/Chapter3.html#using-cmdline
            puttyProcess = new Process();
            puttyProcess.StartInfo.WorkingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "tools");
            puttyProcess.StartInfo.FileName = @"putty.exe";
            if (!string.IsNullOrWhiteSpace(privateKey))
            {
                var ppkFileName = PrivateKeyFileUtils.GetPpkFileName(privateKey, keyPassPhrase);
                puttyProcess.StartInfo.Arguments = $"-ssh -P {port} -i ../keys/{ppkFileName} {userName}@{ip}";
            }
            else
            {
                puttyProcess.StartInfo.Arguments = $"-ssh -P {port} -pw {password} {userName}@{ip}";
            }
           ;
            puttyProcess.StartInfo.WindowStyle = ProcessWindowStyle.Minimized;
            puttyProcess.EnableRaisingEvents = true;
            puttyProcess.Exited += PuttyProcess_Exited;
            puttyProcess.Start();

            while (true)
            {
                if (puttyProcess.MainWindowHandle == IntPtr.Zero)
                {
                    Thread.Sleep(100);
                    continue;
                }

                if (isSSHExited)
                {
                    break;
                }

                ShowWindow(puttyProcess.MainWindowHandle, (short)ShowWindowStyles.SW_MAXIMIZE);
                SetParent(puttyProcess.MainWindowHandle, this.Handle);
                isSSHExited = false;
                IsConnected = true;
                OnConnected?.Invoke(this, EventArgs.Empty);
                break;
            }

            ResizeControl();
            Thread.Sleep(100);
            foreach (char c in keyPassPhrase)
            {
                SendMessage(puttyProcess.MainWindowHandle, 0x0102, c, 0);
            }
            SendMessage(puttyProcess.MainWindowHandle, 0x0102, (int)Keys.Enter, 0);
        }

        public bool IsConnected
        {
            get; set;
        }

        public void Disconnect()
        {
            Close();
        }

        private void ResizeControl()
        {
            if (puttyProcess == null)
            {
                return;
            }

            SendMessage(puttyProcess.MainWindowHandle, WM_COMMAND, WM_PAINT, 0);
            PostMessage(puttyProcess.MainWindowHandle, WM_QT_PAINT, 0, 0);

            SetWindowPos(
            puttyProcess.MainWindowHandle,
              HWND_TOP,
              0 - 8,
               0 - 30,
              (int)this.Width + 30,
              (int)this.Height + 38,
              SWP_FRAMECHANGED);

            SendMessage(puttyProcess.MainWindowHandle, WM_COMMAND, WM_SIZE, 0);
        }

        private void PuttyProcess_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            throw new Exception("Error");
        }

        private void PuttyControl_SizeChanged(object sender, EventArgs e)
        {
            ResizeControl();
        }

        private void PuttyProcess_Exited(object sender, EventArgs e)
        {
            isSSHExited = true;
            IsConnected = false;
            puttyProcess = null;
            OnDisconnected(this, EventArgs.Empty);
            OnExited?.Invoke();
        }

        private void Close()
        {
            if (!puttyProcess.HasExited)
            {
                puttyProcess.Kill();
                puttyProcess.Dispose();
                puttyProcess = null;
                IsConnected = false;
                OnDisconnected(this, EventArgs.Empty);
            }
        }
    }
}
