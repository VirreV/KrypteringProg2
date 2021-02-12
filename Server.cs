using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace KrypteringProg2
{
    class Server : Program
    {
        private List<bool> connected = new List<bool>();
        private List<Socket> socket = new List<Socket>();
        private List<string> user = new List<string>();
        private List<TcpListener> tcpListener = new List<TcpListener>();
        private Byte[] clientMessage;
        private int clientMessageSize = 0;
        private string clientIp = "";

        public Server()
        {
            clientMessage = null;
            IPAddress myIp = IPAddress.Any;
            int port = 8001;
            Console.WriteLine("IP: " + myIp + ":" + port);
            tcpListener.Add(new TcpListener(myIp, port));
            tcpListener[tcpListener.Count - 1].Start();
            while (true)
            {
                try
                {
                    if (socket.Count == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Waiting for connection...");
                    }
                    socket.Add(tcpListener[tcpListener.Count - 1].AcceptSocket());
                    connected.Add(true);
                    user.Add("User" + socket.Count);
                    Console.WriteLine("Connected with " + socket[socket.Count - 1].RemoteEndPoint);

                    //Start Read threads
                    Thread readerThread = new Thread(() => Listener(socket.Count - 1));
                    readerThread.Start();
                }
                catch (Exception e)
                {
                    Error(e);
                }
            }
        }

        private void Sender(string str, Socket socket)
        {
            try
            {
                socket.Send(Encoding.ASCII.GetBytes(str));
            }
            catch(Exception e)
            {
                Error(e);
            }
        }

        private void Listener(int count)
        {
            while (true)
            {
                try
                {
                    Byte[] clientMessage = new byte[256];
                    clientMessageSize = socket[count].Receive(clientMessage);
                    clientIp = socket[count].RemoteEndPoint.ToString();

                    string action = "";
                    for (int i = 0; i < 4; i++)
                    {
                        action += Convert.ToChar(clientMessage[i]);
                    }
                    switch(action){
                        case "nMes":
                            //Nytt meddelande
                            string message = "";
                            for(int i = 4; i < clientMessageSize; i++){
                                message += Convert.ToChar(clientMessage[i]);
                            }
                            base.XMLAddMsg(encrypter.Encrypt(message), encrypter.Encrypt(clientIp) + " " + user[count]);
                            break;
                        case "sMes":
                            //Skicka alla meddelanden
                            Sender(base.GetAllMsg(), socket[count]);
                            break;
                        case "uMes":
                            //Skicka alla meddelanden från den användaren
                            Sender(base.GetUserMsg(user[count]), socket[count]);
                            break;
                        case "uStr":
                            //Tar emot avsändarens användarnamn
                            string name = "";
                            for(int i = 4; i < clientMessageSize; i++){
                                name += Convert.ToChar(clientMessage[i]);
                            }
                            name.Trim(' ');
                            user[count] = encrypter.Encrypt(name);
                            break;
                        case "disc":
                            //Client disconnected
                            connected[count] = false;
                            break;
                        default:
                            throw new ApplicationException("Incorrect action code given");
                    }
                }
                catch(Exception e)
                {
                    if(e.Message.Contains("forcibly closed by the remote host")){connected[count] = false;}
                    else Error(e);
                }
                if(connected[count] == false){
                    Console.WriteLine(socket[count].RemoteEndPoint.ToString() + " disconnected");
                    break;
                }
            }
        }

        static void Error(Exception e)
        {
            Console.BackgroundColor = ConsoleColor.Red;
            Console.WriteLine("Error: " + e.Message);
            Console.BackgroundColor = ConsoleColor.Black;
        }
    }
}