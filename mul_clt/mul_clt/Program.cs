using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            new Program();
        }
        public Program()
        {
            initialize();
            Console.WriteLine("[Received Message] ");
            Console.ReadLine();
        }
        private String m_Name;
        public void initialize()
        {
            Console.Write("[Input Client Name] ");
            String strName = Console.ReadLine();

            if (strName.Trim().Length > 0)
            {
                m_Name = strName;
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                client.Connect(IPAddress.Parse("172.30.1.45"), 8280);
                byte[] name = new byte[100];
                byte[] buffer = Encoding.Unicode.GetBytes(strName);
                for (int i = 0; i < 100; i++)
                {
                    if (i < buffer.Length)
                    {
                        name[i] = buffer[i];
                    }
                }
                client.Send(name);
                SocketAsyncEventArgs reciveAsync = new SocketAsyncEventArgs();
                reciveAsync.Completed += new EventHandler<SocketAsyncEventArgs>(reciveAsync_Completed);
                reciveAsync.SetBuffer(new byte[4], 0, 4);
                reciveAsync.UserToken = client;
                client.ReceiveAsync(reciveAsync);
            }
        }
        private void reciveAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            Socket client = (Socket)sender;
            if (client.Connected && e.BytesTransferred > 0)
            {
                byte[] lengthByte = e.Buffer;
                int length = BitConverter.ToInt32(lengthByte, 0);
                byte[] data = new byte[length];
                client.Receive(data, length, SocketFlags.None);
                StringBuilder sb = new StringBuilder();
                sb.Append("["+m_Name+"]");
                sb.Append(" - ");
                sb.Append(Encoding.Unicode.GetString(data));

                string rcv = Encoding.Unicode.GetString(data);

                Console.WriteLine(sb.ToString());

                int result = Int32.Parse(rcv);

                if (result == 1)
                {

                    Type type = typeof(Reboot);
                    MethodInfo method = type.GetMethod("Test");
                    Reboot c = new Reboot();
                    string result1 = (string)method.Invoke(c, null);
                    Console.WriteLine(result1);

                }

                else if (result == 3)
                {
                    Type type = typeof(Reboot);
                    MethodInfo method = type.GetMethod("Test");
                    Reboot c = new Reboot();
                    string result1 = (string)method.Invoke(c, null);
                    Console.WriteLine(result1);
                }
                
            }
            client.ReceiveAsync(e);
        }
    }
}


//출처: https://nowonbun.tistory.com/296 [명월 일지]

class Reboot
{
    public static void Rebt()
    {
        System.Diagnostics.Process.Start("shutdow.exe", "-r");
    }
    public static void Test()
    {
        Console.Write("this is mac");
    }
}

class execute
{
    public static void exe(String mtd)
    {
        Type type = typeof(Reboot);
        MethodInfo method = type.GetMethod(mtd);
        Reboot c = new Reboot();
        string result1 = (string)method.Invoke(c, null);
        Console.WriteLine(result1);
    }
}