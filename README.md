# RDManager
A tool for remote computer manager, surport windows remote desktop and ssh.

这是一个远程计算机管理工具，支持Windows远程桌面和SSH连接。

1）
微软的远程桌面团队提供过一个称为Remote Desktop Connection Manager的工具，管理远程桌面那叫一个爽。其最新版本为2.7，可惜在高DPI的环境下，远程桌面的字会比较小，可能是基于WPF，WPF会原样展示远程桌面的分辨率。这个工具还有一个2.2版本，是WinForm开发的，远程桌面连接时不存在字太小的问题，可惜我使用的时候经常莫名的断开连接。

因此，有了这个新的远程桌面管理工具。

2）
针对Linux服务器管理的需要，增加了SSH Teminal和SFTP的功能。

SSH Teminal功能来源于poderosa的早期版本， https://github.com/DavidFlamini/terminalcontrol

poderosa已经有新的版本，功能更完善强大，如需了解，看这里：https://github.com/poderosaproject/poderosa

SFTP功能来源于SSH.NET，https://github.com/sshnet/SSH.NET

3）
对于SSH连接后续还有一些完善计划，比如提升SFTP连接稳定性、远程移动文件、修改文件权限、增加操作提示、直接拖动到文件夹内进行传输等。


如果你喜欢也可以拿去改，保留许可证及作者信息即可。

