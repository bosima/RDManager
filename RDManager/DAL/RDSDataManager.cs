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
        /// 获取加密密钥
        /// </summary>
        /// <returns></returns>
        public string GetSecrectKey()
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var secKey = root.Descendants("seckey").FirstOrDefault();
            if (secKey == null)
            {
                return string.Empty;
            }

            return secKey.Value;
        }

        public string GetInitTime()
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var initTime = root.Descendants("inittm").FirstOrDefault();
            if (initTime == null)
            {
                return string.Empty;
            }

            return initTime.Value;
        }

        public string GetPassword()
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var passElement = root.Descendants("pass").FirstOrDefault();
            if (passElement == null)
            {
                return string.Empty;
            }

            return passElement.Value;
        }

        /// <summary>
        /// 设置加密密钥
        /// </summary>
        /// <returns></returns>
        public void SetSecrectKey(string secKey)
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var secKeyElement = root.Descendants("seckey").FirstOrDefault();
            if (secKeyElement == null)
            {
                secKeyElement = new XElement("seckey");
                secKeyElement.Value = secKey;
                root.Add(secKeyElement);

                doc.Save(dataPath);
            }
        }

        public void SetInitTime(string time)
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var initTimeElement = root.Descendants("inittm").FirstOrDefault();
            if (initTimeElement == null)
            {
                initTimeElement = new XElement("inittm");
                initTimeElement.Value = time;
                root.Add(initTimeElement);

                doc.Save(dataPath);
            }
        }

        public void SetPassword(string password)
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var passElement = root.Descendants("pass").FirstOrDefault();
            if (passElement == null)
            {
                passElement = new XElement("pass");
                passElement.Value = password;
                root.Add(passElement);

                doc.Save(dataPath);
            }
        }

        public void EncryptServer()
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var servers = root.Descendants("server");
            if (servers.Any())
            {
                foreach (var server in servers)
                {
                    var address = server.Attribute("address");
                    var password = server.Attribute("password");
                    if (address != null && password != null && !string.IsNullOrWhiteSpace(password.Value))
                    {
                        password.Value = EncryptUtils.EncryptServerPassword(address.Value, password.Value);
                    }
                }

                doc.Save(dataPath);
            }
        }

        /// <summary>
        /// 移除指定的组或服务器
        /// </summary>
        /// <param name="id"></param>
        public void Remove(Guid id)
        {
            XDocument doc = GetData();
            var root = doc.Element("rds");

            var elelment = root.Descendants().Where(d => d.Attribute("id") != null).Where(d => d.Attribute("id").Value == id.ToString()).FirstOrDefault();
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

                var docElelment = parentGroup.Descendants("group").Where(d => d.Attribute("id").Value == model.GroupID.ToString()).FirstOrDefault();
                if (docElelment == null)
                {
                    parentGroup.Add(groupElement);
                }
                else
                {
                    docElelment.SetAttributeValue("id", model.GroupID);
                    docElelment.SetAttributeValue("name", model.GroupName);
                }
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

            var docElelment = root.Descendants("server").Where(d => d.Attribute("id").Value == model.ServerID.ToString()).FirstOrDefault();
            if (docElelment == null)
            {
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
            }
            else
            {
                docElelment.SetAttributeValue("id", model.ServerID);
                docElelment.SetAttributeValue("name", model.ServerName);
                docElelment.SetAttributeValue("address", model.ServerAddress);
                docElelment.SetAttributeValue("port", model.ServerPort);
                docElelment.SetAttributeValue("username", model.UserName);
                docElelment.SetAttributeValue("password", model.Password);
            }

            doc.Save(dataPath);
        }
    }
}
