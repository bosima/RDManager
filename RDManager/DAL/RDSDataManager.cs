using RDManager.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace RDManager.DAL
{
    public class RDSDataManager
    {
        private static string dataPath = Path.Combine(System.AppDomain.CurrentDomain.BaseDirectory, "rds.xml");
        public XDocument GetData()
        {
            return XDocument.Load(dataPath);
        }

        /// <summary>
        /// 移除指定的组或服务器
        /// </summary>
        /// <param name="id"></param>
        public void Remove(Guid id)
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var elelment = root.Descendants().Where(d => d.Attribute("id").Value == id.ToString()).FirstOrDefault();
            if (elelment != null)
            {
                elelment.Remove();
            }

            doc.Save(dataPath);
        }

        /// <summary>
        /// 添加分组
        /// </summary>
        /// <param name="model"></param>
        public void AddGroup(RDSGroup model)
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var groupElement = new XElement("group");
            groupElement.SetAttributeValue("id", model.GroupID);
            groupElement.SetAttributeValue("name", model.GroupName);

            // TODO:检查重名

            if (model.ParentGroupID == Guid.Empty)
            {
                root.Add(groupElement);
            }
            else
            {
                var parentGroup = root.DescendantsAndSelf("group").Where(d => d.Attribute("id").Value == model.ParentGroupID.ToString()).FirstOrDefault();
                if (parentGroup == null)
                {
                    throw new ArgumentException("上级分组不存在！");
                }

                parentGroup.Add(groupElement);
            }

            doc.Save(dataPath);
        }

        /// <summary>
        /// 添加服务器
        /// </summary>
        /// <param name="model"></param>
        public void AddServer(RDSServer model)
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var element = new XElement("server");
            element.SetAttributeValue("id", model.ServerID);
            element.SetAttributeValue("name", model.ServerName);
            element.SetAttributeValue("address", model.ServerAddress);
            element.SetAttributeValue("port", model.ServerPort);
            element.SetAttributeValue("username", model.UserName);
            element.SetAttributeValue("password", model.Password);

            if (model.GroupID == Guid.Empty)
            {
                root.Add(element);
            }
            else
            {
                var parentGroup = root.DescendantsAndSelf("group").Where(d => d.Attribute("id").Value == model.GroupID.ToString()).FirstOrDefault();
                if (parentGroup == null)
                {
                    throw new ArgumentException("分组不存在！");
                }

                parentGroup.Add(element);
            }

            doc.Save(dataPath);
        }
    }
}
