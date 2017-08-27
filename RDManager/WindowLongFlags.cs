using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDManager
{
    public abstract class WindowLongFlags
    {
        public const int GWL_EXSTYLE = -20;
        public const int GWLP_HINSTANCE = -6;
        public const int GWLP_HWNDPARENT = -8;
        public const int GWL_ID = -12;
        public const int GWL_STYLE = -16;
        public const int GWL_USERDATA = -21;
        public const int GWL_WNDPROC = -4;
        public const int DWLP_USER = 0x8;
        public const int DWLP_MSGRESULT = 0x0;
        public const int DWLP_DLGPROC = 0x4;
    }
}
