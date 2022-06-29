using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.OpenSsl;

namespace RDManager
{
    internal class PasswordFinder: IPasswordFinder
    {
        readonly string _password;
        public PasswordFinder(string password)
        {
            _password = password;
        }

        public char[] GetPassword()
        {
            return _password.ToCharArray();
        }
    }
}
