using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDManager.Model
{
    public class SFTPProcessException : Exception
    {
        public SFTPProcessException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
