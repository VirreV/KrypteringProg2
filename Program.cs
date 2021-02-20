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
        static List<Message> msgs = new List<Message>();
        static string xmlPath = "serverLog.xml";

        static void Main(string[] args)
        {
            XmlDocument xmlDoc = new XmlDocument();
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
            for(int i = 0; i < xmlDoc.SelectNodes("msgList/Meddelande").Count; i++){
                msgs.Add(new Message(xmlDoc.SelectNodes("msgList/Meddelande")[i].SelectSingleNode("Avsändare").InnerText, xmlDoc.SelectNodes("msgList/Meddelande")[i].SelectSingleNode("Text").InnerText, i + 1));
            }
            Thread saveThread = new Thread(() => {
                while(true){
                    if(Console.ReadKey(true).Key == ConsoleKey.S){
                        SaveXML();
                    }
                }
            });
            saveThread.Start();
            Server server = new Server();
        }

        protected static void SaveXML(){
            XmlDocument doc = new XmlDocument();
            XmlDeclaration xmlDeclaration = doc.CreateXmlDeclaration("1.0", "utf-8", null);
            doc.AppendChild(xmlDeclaration);
            XmlElement eMsgList = doc.CreateElement("msgList");
            doc.AppendChild(eMsgList);
            System.Console.WriteLine(msgs.Count());
            foreach(Message m in msgs){
                XmlElement eMsg = doc.CreateElement("Meddelande");
                XmlElement eUser = doc.CreateElement("Avsändare");
                XmlElement eText = doc.CreateElement("Text");
                XmlElement eID = doc.CreateElement("MeddelandeID");

                eUser.InnerText = m.User;
                eText.InnerText = m.Text;
                eID.InnerText = m.Id.ToString();

                eMsg.AppendChild(eUser);
                eMsg.AppendChild(eID);
                eMsg.AppendChild(eText);

                eMsgList.AppendChild(eMsg);
            }
            doc.Save(xmlPath);
            Console.WriteLine("Saved messages");
        }

        protected void AddMsg(string str, string user){
            msgs.Add(new Message(user, str, msgs.Count + 1));
        }

        protected string GetAllMsg(){
            string AllMsg = "";
            foreach(Message m in msgs){
                AllMsg += m.UserName + "\t" + m.Text + "\n";
            }
            if(AllMsg == "") AllMsg = "-";
            return AllMsg;
        }

        protected string GetUserMsg(string user){
            string AllMsg = "";
            foreach(Message m in msgs){
                if(m.UserName == user){
                    AllMsg += m.UserName + "\t" + m.Text + "\n";
                }
            }
            if(AllMsg == "") AllMsg = "-";
            return AllMsg;
        }
    }
}
