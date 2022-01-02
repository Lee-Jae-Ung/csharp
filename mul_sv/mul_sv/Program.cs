using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
namespace server
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program();
        }
        private Dictionary<string, Socket> m_client = null;
        public Program()
        {
            initialize();
        }
        protected void initialize()
        {
            m_client = new Dictionary<string, Socket>();
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 8280));
            server.Listen(20);
            SocketAsyncEventArgs sockAsync = new SocketAsyncEventArgs();
            sockAsync.Completed += new EventHandler<SocketAsyncEventArgs>(sockAsync_Completed);
            server.AcceptAsync(sockAsync);
            Command();
        }
        private void sockAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket server = (Socket)sender;
            Socket client = e.AcceptSocket;
            byte[] name = new byte[100];
            client.Receive(name);
            String strName = Encoding.Unicode.GetString(name).Trim().Replace("\0", "");
            m_client.Add(strName, client);
            SocketAsyncEventArgs recieveAsync = new SocketAsyncEventArgs();
            recieveAsync.SetBuffer(new byte[4], 0, 4);
            recieveAsync.UserToken = client;
            recieveAsync.Completed += new EventHandler<SocketAsyncEventArgs>(recieveAsync_Completed);
            client.ReceiveAsync(recieveAsync);
            Console.WriteLine();
            Console.WriteLine("***********" + strName + " Connected ***********");
            Console.Write("Sender ?");
            e.AcceptSocket = null;
            server.AcceptAsync(e);
        }
        private void recieveAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket client = (Socket)sender;
            if (client.Connected && e.BytesTransferred > 0)
            {
                int length = BitConverter.ToInt32(e.Buffer, 0);
                byte[] data = new byte[length];
                client.Receive(data, length, SocketFlags.None);
                String data2 = Encoding.Unicode.GetString(data);
                String Name = searchSocket(client);
                if (Name != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("***********" + Name + " receive ***********");
                    Console.WriteLine(data2);
                    Console.Write("Sender ?");
                }
                else
                {
                }
            }
            client.ReceiveAsync(e);
        }
        protected String searchSocket(Socket sender)
        {
            foreach (String key in m_client.Keys)
            {
                if (m_client[key] == sender)
                {
                    return key;
                }
            }
            return null;
        }
        protected void Command()
        {
            while (true)
            {
                Console.Write("Sender ?");
                String sender = Console.ReadLine();
                if (m_client.ContainsKey(sender))
                {
                    Console.Write("Message ?");
                    String message = Console.ReadLine();
                    Socket client = m_client[sender];
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    client.Send(BitConverter.GetBytes(data.Length));
                    client.Send(data, data.Length, SocketFlags.None);
                }
                else
                {
                    Console.WriteLine("Not Socket");
                }
            }
        }
    }
}


//출처: https://nowonbun.tistory.com/296 [명월 일지]asdasd