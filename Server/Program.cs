using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    //public class MessageObject
    //{
    //    public MessageObject(string pMessage)
    //    {
    //        message = pMessage;
    //    }

    //    public string message;
    //}

    class Program
    {
        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, 4000);
            listenSocket.Bind(listenEndPoint);
            listenSocket.Listen(10);
            Socket clientSocket = listenSocket.Accept();

            // 패킷 길이 받기(header)
            byte[] headerBuffer = new byte[2];
            int RecvLength = clientSocket.Receive(headerBuffer, 2, SocketFlags.None);
            short packetLength = BitConverter.ToInt16(headerBuffer, 0);
            packetLength = IPAddress.NetworkToHostOrder(packetLength);

            // 실제 데이터(header 길이 만큼 만)
            byte[] dataBuffer = new byte[4096];
            RecvLength = clientSocket.Receive(dataBuffer, packetLength, SocketFlags.None);

            string JsonString = Encoding.UTF8.GetString(dataBuffer);

            Console.WriteLine(JsonString);

            // custom 패킷 만들기
            // 다시 전송 메세지
            string message = "{ \"message\" : \"클라이언트 받고 서버꺼 추가.\"}";
            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
            ushort length = (ushort)IPAddress.HostToNetworkOrder((short)messageBuffer.Length);

            //길이(headerBuffer)     자료 (message)
            //[][]        +         [][][][][][][][]
            headerBuffer = BitConverter.GetBytes(length);

            byte[] packetBuffer = new byte[headerBuffer.Length + messageBuffer.Length]; // 실제 전송 데이터 버퍼 = 패킷

            Buffer.BlockCopy(headerBuffer, 0, packetBuffer, 0, headerBuffer.Length);
            Buffer.BlockCopy(messageBuffer, 0, packetBuffer, headerBuffer.Length, messageBuffer.Length);

            int SendLength = clientSocket.Send(packetBuffer, packetBuffer.Length, SocketFlags.None);


            clientSocket.Close();
            listenSocket.Close();


            //MessageObject mo2 = new MessageObject("");

            //Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);
            //listenSocket.Bind(listenEndPoint);

            //listenSocket.Listen(10);

            //bool isRunning = true;
            //while (isRunning)
            //{
            //    //동기, 블록킹 
            //    Socket clientSocket = listenSocket.Accept();

            //    byte[] buffer = new byte[1024];
            //    int RecvLength = clientSocket.Receive(buffer);

            //    //100+200
            //    string message = Encoding.UTF8.GetString(buffer);

            //    mo2 = JsonConvert.DeserializeObject<MessageObject>(message);

            //    if (mo2.message.Equals("안녕하세요"))
            //    {
            //        mo2.message = "반갑습니다";
            //    }
            //    else
            //    {
            //        mo2.message = "인사도없네?";
            //    }

            //    message = JsonConvert.SerializeObject(mo2);

            //    Console.WriteLine($"Server : \"{message}\" ");

            //    buffer = Encoding.UTF8.GetBytes(message);
            //    int SendLength = clientSocket.Send(buffer);


            //    clientSocket.Close();
            //}

            //listenSocket.Close();
        }
    }
}