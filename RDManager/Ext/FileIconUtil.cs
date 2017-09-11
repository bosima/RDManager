﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;
using Microsoft.Win32;

namespace RDManager
{
    /// <summary>
    /// 文件图标工具
    /// From http://blog.csdn.net/onlybymyself/article/details/51764452
    /// </summary>
    public class FileIconUtil
    {
        /// <summary>           
        /// 当读取磁盘驱动器图标失败的默认图标索引号           
        /// </summary>           
        public static readonly long ErrorDriverIndex = -8;

        //在shell32.dll导入函数SHGetFileInfo  
        [DllImport("shell32.dll", EntryPoint = "SHGetFileInfo")]

        public static extern int GetFileInfo(string pszPath, int dwFileAttributes, ref FileInfomation psfi, int cbFileInfo, int uFlags);

        [DllImport("shell32.dll")]
        private static extern int ExtractIconEx(string lpszFile, int niconIndex, IntPtr[] phiconLarge, IntPtr[] phiconSmall, int nIcons);

        //定义SHFILEINFO结构  
        [StructLayout(LayoutKind.Sequential)]
        public struct FileInfomation
        {
            public IntPtr hIcon;
            public int iIcon;
            public int dwAttributes;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        [DllImport("Shell32.dll", EntryPoint = "SHGetFileInfo", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern IntPtr SHGetFileInfo(string pszPath, uint dwFileAttributes, ref FileInfomation psfi, uint cbFileInfo, uint uFlags);

        [DllImport("User32.dll", EntryPoint = "DestroyIcon")]
        public static extern int DestroyIcon(IntPtr hIcon);

        //定义文件属性标识  
        public enum FileAttributeFlags : uint
        {
            FILE_ATTRIBUTE_READONLY = 0x00000001,
            FILE_ATTRIBUTE_HIDDEN = 0x00000002,
            FILE_ATTRIBUTE_SYSTEM = 0x00000004,
            FILE_ATTRIBUTE_DIRECTORY = 0x00000010,
            FILE_ATTRIBUTE_ARCHIVE = 0x00000020,
            FILE_ATTRIBUTE_DEVICE = 0x00000040,
            FILE_ATTRIBUTE_NORMAL = 0x00000080,
            FILE_ATTRIBUTE_TEMPORARY = 0x00000100,
            FILE_ATTRIBUTE_SPARSE_FILE = 0x00000200,
            FILE_ATTRIBUTE_REPARSE_POINT = 0x00000400,
            FILE_ATTRIBUTE_COMPRESSED = 0x00000800,
            FILE_ATTRIBUTE_OFFLINE = 0x00001000,
            FILE_ATTRIBUTE_NOT_CONTENT_INDEXED = 0x00002000,
            FILE_ATTRIBUTE_ENCRYPTED = 0x00004000
        }

        //定义获取资源标识  
        public enum FileInfoFlags : uint
        {
            SHGFI_ICON = 0x000000100, // get icon  
            SHGFI_DISPLAYNAME = 0x000000200, // get display name  
            SHGFI_TYPENAME = 0x000000400, // get type name  
            SHGFI_ATTRIBUTES = 0x000000800, // get attributes  
            SHGFI_ICONLOCATION = 0x000001000, // get icon location  
            SHGFI_EXETYPE = 0x000002000, // return exe type  
            SHGFI_SYSICONINDEX = 0x000004000, // get system icon index  
            SHGFI_LINKOVERLAY = 0x000008000, // put a link overlay on icon  
            SHGFI_SELECTED = 0x000010000, // show icon in selected state  
            SHGFI_ATTR_SPECIFIED = 0x000020000, // get only specified attributes  
            SHGFI_LARGEICON = 0x000000000, // get large icon  
            SHGFI_SMALLICON = 0x000000001, // get small icon  
            SHGFI_OPENICON = 0x000000002, // get open icon  
            SHGFI_SHELLICONSIZE = 0x000000004, // get shell size icon  
            SHGFI_PIDL = 0x000000008, // pszPath is a pidl  
            SHGFI_USEFILEATTRIBUTES = 0x000000010, // use passed dwFileAttribute  
            SHGFI_ADDOVERLAYS = 0x000000020, // apply the appropriate overlays  
            SHGFI_OVERLAYINDEX = 0x000000040 // Get the index of the overlay  
        }

        /// <summary>  
        /// 获取文件类型的关联图标  
        /// </summary>  
        /// <param name="fileExt">文件类型的扩展名或文件的绝对路径</param>  
        /// <param name="isLargeIcon">是否返回大图标</param>  
        /// <returns>获取到的图标</returns>  
        public static Icon GetIcon(string fileExt, bool isLargeIcon)
        {
            FileInfomation shfi = new FileInfomation();
            IntPtr hI;
            if (isLargeIcon)
            {
                hI = SHGetFileInfo(fileExt, 0, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (uint)FileInfoFlags.SHGFI_LARGEICON);
            }
            else
            {
                hI = SHGetFileInfo(fileExt, 0, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_USEFILEATTRIBUTES | (uint)FileInfoFlags.SHGFI_SMALLICON);
            }
            Icon icon = Icon.FromHandle(shfi.hIcon).Clone() as Icon;
            DestroyIcon(shfi.hIcon); //释放资源  
            return icon;
        }

        /// <summary>   
        /// 获取文件夹图标  
        /// </summary>   
        /// <returns>图标</returns>   
        public static Icon GetDirectoryIcon(bool isLargeIcon)
        {
            FileInfomation _FileInfomation = new FileInfomation();
            IntPtr _IconIntPtr;
            if (isLargeIcon)
            {
                _IconIntPtr = SHGetFileInfo(@"", 0, ref _FileInfomation, (uint)Marshal.SizeOf(_FileInfomation), ((uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_LARGEICON));
            }
            else
            {
                _IconIntPtr = SHGetFileInfo(@"", 0, ref _FileInfomation, (uint)Marshal.SizeOf(_FileInfomation), ((uint)FileInfoFlags.SHGFI_ICON | (uint)FileInfoFlags.SHGFI_SMALLICON));
            }
            if (_IconIntPtr.Equals(IntPtr.Zero))
                return null;
            Icon _Icon = System.Drawing.Icon.FromHandle(_FileInfomation.hIcon);
            //DestroyIcon(_FileInfomation.hIcon); //释放资源  
            return _Icon;
        }

        /// <summary>           
        /// 获取磁盘驱动器图标  
        /// </summary>           
        /// <param name="driverMark">有效的磁盘标号，如C、D、I等等，不区分大小写</param>           
        /// <param name="isSmallIcon">标识是获取小图标还是获取大图标</param>           
        /// <param name="imageIndex">输出与返回图标对应的系统图标索引号</param>           
        /// <returns>返回一个Icon类型的磁盘驱动器图标对象</returns>           
        public static Icon GetDriverIcon(char driverMark, bool isSmallIcon)
        {
            long imageIndex;
            return GetDriverIcon(driverMark, isSmallIcon, out imageIndex);
        }

        /// <summary>           
        /// 获取磁盘驱动器图标  
        /// </summary>           
        /// <param name="driverMark">有效的磁盘标号，如C、D、I等等，不区分大小写</param>           
        /// <param name="isSmallIcon">标识是获取小图标还是获取大图标</param>           
        /// <param name="imageIndex">输出与返回图标对应的系统图标索引号</param>           
        /// <returns>返回一个Icon类型的磁盘驱动器图标对象</returns>           
        public static Icon GetDriverIcon(char driverMark, bool isSmallIcon, out long imageIndex)
        {
            imageIndex = ErrorDriverIndex;
            //非有效盘符，返回封装的磁盘图标               
            if (driverMark < 'a' && driverMark > 'z' && driverMark < 'A' && driverMark > 'Z')
            {
                return null;
            }
            string driverName = driverMark.ToString().ToUpper() + ":\\";

            FileInfomation shfi = new FileInfomation();
            FileInfoFlags uFlags = FileInfoFlags.SHGFI_ICON | FileInfoFlags.SHGFI_SHELLICONSIZE | FileInfoFlags.SHGFI_USEFILEATTRIBUTES;

            if (isSmallIcon)
                uFlags |= FileInfoFlags.SHGFI_SMALLICON;
            else
                uFlags |= FileInfoFlags.SHGFI_LARGEICON;

            int iTotal = (int)SHGetFileInfo(driverName, (uint)FileAttributeFlags.FILE_ATTRIBUTE_NORMAL, ref shfi, (uint)Marshal.SizeOf(shfi), (uint)uFlags);

            Icon icon = null;
            if (iTotal > 0)
            {
                icon = Icon.FromHandle(shfi.hIcon).Clone() as Icon;
                imageIndex = shfi.iIcon;
            }

            DestroyIcon(shfi.hIcon); //释放资源  
            return icon;
        }
    }
}