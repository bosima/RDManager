using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RDManager
{
    public class FileOperationUtil
    {
        public static List<DriveInfo> GetDrives()
        {
            List<DriveInfo> list = new List<DriveInfo>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();

            //检索计算机上的所有逻辑驱动器名称
            foreach (DriveInfo item in allDrives)
            {
                if (item.IsReady)
                {
                    list.Add(item);
                }
            }

            return list;
        }
    }
}
