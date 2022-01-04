using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Diagnostics;

namespace client
{
    class Program
    {
        static void Main(string[] args)
        {
            //program실행
            new Program();
        }
        public Program()
        {
            //initialize실행

            initialize();

            Console.WriteLine("실행된 함수 ");
            Console.ReadLine();
        }
        private String m_Name;
        public void initialize()
        {

            //client 이름 입력
            Console.Write("[Input Client Name] ");

            //strName에 입력한 이름 저장
            String strName = Console.ReadLine();


            //입력한 client 이름의 앞, 뒤 공백을 모두 제거한뒤 길이가 0 이상이면
            if (strName.Trim().Length > 0)
            {
                m_Name = strName;

                //소켓 객체 생성(TCP)
                Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                //서버에 연결
                client.Connect(IPAddress.Parse("203.250.77.238"), 8280);

                // name 변수 생성
                byte[] name = new byte[100];

                //앞서 입력한 client의 이름을 buffer에 저장
                byte[] buffer = Encoding.Unicode.GetBytes(strName);

                //name애 buffer의 내용 복사
                for (int i = 0; i < 100; i++)
                {
                    if (i < buffer.Length)
                    {
                        name[i] = buffer[i];
                    }
                }

                //서버에 name 전송
                client.Send(name);



                SocketAsyncEventArgs receiveAsync = new SocketAsyncEventArgs();
                receiveAsync.Completed += new EventHandler<SocketAsyncEventArgs>(receiveAsync_Completed);
                receiveAsync.SetBuffer(new byte[4], 0, 4);
                receiveAsync.UserToken = client;
                client.ReceiveAsync(receiveAsync);
            }


        }
        private void receiveAsync_Completed(object sender, SocketAsyncEventArgs e)
        {

            Socket client = (Socket)sender;
            if (client.Connected && e.BytesTransferred > 0)
            {
                byte[] lengthByte = e.Buffer;
                int length = BitConverter.ToInt32(lengthByte, 0);
                byte[] data = new byte[length];
                client.Receive(data, length, SocketFlags.None);
                //StringBuilder sb = new StringBuilder();
                //sb.Append("[" + m_Name + "]");
                //sb.Append(" : ");
                //sb.Append(Encoding.Unicode.GetString(data));

                string rcv = Encoding.Unicode.GetString(data);

                //Console.WriteLine(sb.ToString());

                int result = Int32.Parse(rcv);

                if (result == 1)
                {

                    Type type = typeof(Manager);
                    MethodInfo method = type.GetMethod("Test");
                    Manager c = new Manager();
                    string result1 = (string)method.Invoke(c, null);
                    Console.WriteLine(result1);
                }

                else if (result == 2)
                {
                    Type type = typeof(Manager);
                    MethodInfo method = type.GetMethod("CheckRam");
                    Manager c = new Manager();
                    string result1 = (string)method.Invoke(c, null);
                    Console.WriteLine(result1);
                }

                else if (result == 3)
                {
                    Type type = typeof(Manager);
                    MethodInfo method = type.GetMethod("Rebt");
                    Manager c = new Manager();
                    string result1 = (string)method.Invoke(c, null);
                    Console.WriteLine(result1);
                }

                
            }
            client.ReceiveAsync(e);


        }
    }
}



class Manager
{
    public static void Rebt()
    {

        System.Diagnostics.Process.Start("shutdown.exe", "-r");

    }
    public static void Test()
    {
        Console.Write("test1");
    }

    public static void CheckRam()
    {
        PerformanceCounter memoryload = new PerformanceCounter("Memory", "Available MBytes");
        //Console.WriteLine("사용 가능 메모리 : "+memoryload.NextValue().ToString() + "MB");
        String mem = memoryload.NextValue().ToString();
        Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //서버에 연결
        client.Connect(IPAddress.Parse("203.250.77.238"), 8280);

        mem = "memory " + mem;

        //앞서 입력한 client의 이름을 buffer에 저장
        byte[] mem_buf = Encoding.Unicode.GetBytes(mem);

        

        //Console.WriteLine("최종 넘어갈 데이터 "+mem_buf);
        //서버에 name 전송
        client.Send(mem_buf);
    }

    public static void Quit()
    {
        
    }
}

