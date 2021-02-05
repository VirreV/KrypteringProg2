using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Xml;

namespace KrypteringProg2
{
    class Program
    {
        protected static string xmlPath = "serverLog.xml";
        protected static int msgCount;
        protected static XmlDocument xmlDoc = new XmlDocument();
        protected static Encrypter encrypter;

        static void Main(string[] args)
        {
            if (!(File.Exists(xmlPath)))
            {
                XmlDocument newXml = new XmlDocument();
                XmlDeclaration xmlDeclaration = newXml.CreateXmlDeclaration("1.0", "utf-8", null);
                newXml.AppendChild(xmlDeclaration);
                XmlElement eMsgList = newXml.CreateElement("msgList");
                newXml.AppendChild(eMsgList);
                newXml.Save(xmlPath);
            }
            xmlDoc.Load(xmlPath);
            msgCount = xmlDoc.SelectNodes("msgList/Meddelande").Count;
            encrypter = new Encrypter();
            Server server = new Server();
        }

        protected void XMLAddMsg(string str, string user){
            msgCount++;
            xmlDoc.Load(xmlPath);
            XmlNode list = xmlDoc.SelectSingleNode("msgList");
            XmlElement msg = xmlDoc.CreateElement("Meddelande");
            XmlElement usr = xmlDoc.CreateElement("Avsändare");
            usr.InnerText = user;
            XmlElement mId = xmlDoc.CreateElement("MeddelandeID");
            mId.InnerText = msgCount.ToString();
            XmlElement txt = xmlDoc.CreateElement("Text");
            txt.InnerText = str;

            msg.AppendChild(usr);
            msg.AppendChild(mId);
            msg.AppendChild(txt);
            list.AppendChild(msg);

            xmlDoc.Save(xmlPath);
        }

        protected string GetAllMsg(){
            string AllMsg = "";
            xmlDoc.Load(xmlPath);
            foreach(XmlNode node in xmlDoc.SelectNodes("msgList/Meddelande")){
                AllMsg += "User: " + encrypter.Decrypt(node.SelectSingleNode("Avsändare").InnerText.Substring(node.SelectSingleNode("Avsändare").InnerText.IndexOf(' ') + 1)) + "\n\t: " + encrypter.Decrypt(node.SelectSingleNode("Text").InnerText) + "\n";
            }
            return AllMsg;
        }
    }
}
