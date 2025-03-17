using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            //Loopback -> 테스트용 내 자신 IP , 127.0.0.1 -> 본인 IP
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);
            serverSocket.Connect(serverEndPoint);

            byte[] buffer;

            string message = "Hello world";
            buffer = Encoding.UTF8.GetBytes(message);
            int sendLength = serverSocket.Send(buffer);

            byte[] buffer2 = new byte[1024];
            int reecvLength = serverSocket.Receive(buffer2);

            Console.WriteLine(Encoding.UTF8.GetString(buffer2));

            serverSocket.Close();
        }
    }
}
