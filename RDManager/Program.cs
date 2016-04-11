using RDManager.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace RDManager
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            RDSDataManager dataManager = new RDSDataManager();
            var secKey = dataManager.GetSecrectKey();
            var initTime = dataManager.GetInitTime();

            if (string.IsNullOrWhiteSpace(secKey) || string.IsNullOrWhiteSpace(initTime))
            {
                Application.Run(new InitForm());
            }
            else
            {
                Application.Run(new LoginForm());
            }
        }
    }
}
