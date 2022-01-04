using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Threading;

namespace server
{
    class Program
    {
        public static String mem;
        static void Main(string[] args)
        {

            new Program();
        }
        private Dictionary<string, Socket> m_client = null;
        public Program()
        {
            try
            {
                initialize();
            }
            catch
            {
                Console.WriteLine("client가 연결 종료함");
            }
        }
        protected void initialize()
        {
            m_client = new Dictionary<string, Socket>();
            //Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //m_client.OrderBy(x => x.Key).ToList() ;
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(new IPEndPoint(IPAddress.Any, 8280));
            server.Listen(20);


            //소켓 이벤트 정의
            SocketAsyncEventArgs sockAsync = new SocketAsyncEventArgs();

            //이벤트 발생시 sockAsync_Completed함수 실행
            sockAsync.Completed += new EventHandler<SocketAsyncEventArgs>(sockAsync_Completed);
            
            //접속 수락 대기
            server.AcceptAsync(sockAsync);


            // 클라이언트에게 메세지를 보냄

  
            
            Command();

            



        }

        
        private void sockAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            //서버가 sender
            Socket server = (Socket)sender;

            //accept된 소켓은 client
            Socket client = e.AcceptSocket;
            byte[] name = new byte[100];
            client.Receive(name);
            String strName = Encoding.Unicode.GetString(name).Trim().Replace("\0", "");
            
            //port1 => mem[0] = port1, memory 12345 => mem[0] = memory
            string[] check_data = strName.Split(' ');
            if (!check_data[0].Equals("memory"))
            {
                m_client.Add(strName, client);
                //Console.WriteLine(strName + "의 사용가능한 메모리는 " + mem[1] + "MB 입니다.");
            }
            
            

            //socketAsyncE 변수생성
            SocketAsyncEventArgs recieveAsync = new SocketAsyncEventArgs();

            //수신버퍼 설정
            recieveAsync.SetBuffer(new byte[4], 0, 4);
            //usertoken은 client
            recieveAsync.UserToken = client;
            //
            recieveAsync.Completed += new EventHandler<SocketAsyncEventArgs>(recieveAsync_Completed);

            client.ReceiveAsync(recieveAsync);


            if (check_data[0].Equals("memory"))
            {
                
               
                
                mem = check_data[1];

            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("------[" + strName + "]" + "  Connected------");
                Console.Write("[Send to] ");
            }
            
            e.AcceptSocket = null;
            server.AcceptAsync(e);



        }


        private void recieveAsync_Completed(object sender, SocketAsyncEventArgs e)
        {
            //client가 보내는 사람
            Socket client = (Socket)sender;
            //
            if (client.Connected && e.BytesTransferred > 0)
            {
                int length = BitConverter.ToInt32(e.Buffer, 0);
                byte[] data = new byte[length];
                client.Receive(data, length, SocketFlags.None);
                String data2 = Encoding.Unicode.GetString(data);
                String Name = searchSocket(client);

                //m_client.Where(x => x.Key==data2).ToList();

                if (Name != null)
                {
                    Console.WriteLine();
                    Console.WriteLine("[" + Name + "]" + " receive");
                    Console.WriteLine(data2);
                    Console.Write("[Send to] zzz");
                }
                else
                {
                    Console.WriteLine("fffffffffff");
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

                Console.Write("[Send to] ");
                String sender = Console.ReadLine();
                int i=0;
                while (i!=4) {
                    if (m_client.ContainsKey(sender))
                    {
                        Console.WriteLine("   ***{ 1 : 테스트, 2 : 사용가능 메모리 확인, 3 : 재부팅, 4 : 연결종료}***");
                        Console.Write("   [Send Message] ");
                        
                        String message = Console.ReadLine();


                        if (Int32.Parse(message)==4)
                        {
                            i = Int32.Parse(message);
                        }


                        Socket client = m_client[sender];
                        byte[] data = Encoding.Unicode.GetBytes(message);
                        client.Send(BitConverter.GetBytes(data.Length));
                        client.Send(data, data.Length, SocketFlags.None);

                        
                        if (Int32.Parse(message) == 2)
                        {
                            Thread.Sleep(2000);
                            Console.WriteLine("      사용가능 메모리 : "+mem + " MB");
                        }

                    }
                    
                    else
                    {
                        Console.WriteLine("등록된 client가 아닙니다.");
                    }
                    
                }
            }
        }
    }
}

