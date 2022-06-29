﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDManager.Model
{
    public class RDSServer
    {
        public Guid GroupID { get; set; }
        public Guid ServerID { get; set; }
        public string ServerName { get; set; }
        public string ServerAddress { get; set; }
        public int ServerPort { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string PrivateKey { get; set; }
        public string KeyPassPhrase { get; set; }
        public string OpType { get; set; }
        public string LinkType { get; set; }

        public RDSServer Clone()
        {
            var newServer = new RDSServer();
            newServer.GroupID = GroupID;
            newServer.ServerID = ServerID;
            newServer.ServerName = ServerName;
            newServer.ServerAddress = ServerAddress;
            newServer.ServerPort = ServerPort;
            newServer.UserName = UserName;
            newServer.Password = Password;
            newServer.PrivateKey = PrivateKey;
            newServer.KeyPassPhrase = KeyPassPhrase;
            newServer.OpType = OpType;
            newServer.LinkType = LinkType;

            return newServer;
        }
    }
}
