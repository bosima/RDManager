using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RDManager.Model
{
    public class RDSGroup
    {
        public Guid ParentGroupID
        {
            get;
            set;
        }

        public string GroupName
        {
            get;
            set;
        }

        public Guid GroupID
        {
            get;
            set;
        }

        public RDSGroup Clone()
        {
            var newServer = new RDSGroup();
            newServer.GroupID = GroupID;
            newServer.ParentGroupID = ParentGroupID;
            newServer.GroupName = GroupName;

            return newServer;
        }
    }
}
