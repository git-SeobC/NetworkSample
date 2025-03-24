using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Client
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
            string jsonString = "{\"message\" : \"뀨\"}";
            byte[] message = Encoding.UTF8.GetBytes(jsonString);
            ushort length = (ushort)message.Length;
            //길이(lengthBuffer)     자료 (message)
            //[][]        +         [][][][][][][][]
            byte[] lengthBuffer = new byte[2];
            lengthBuffer = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((short)length));

            byte[] buffer = new byte[2 + length]; // 실제 전송 데이터 버퍼

            Buffer.BlockCopy(lengthBuffer, 0, buffer, 0, 2);
            Buffer.BlockCopy(message, 0, buffer, 2, length);

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse("192.168.0.22"), 4000);

            clientSocket.Connect(listenEndPoint);

            //for (int i = 0; i < 100; i++)
            {
                int SendLength = clientSocket.Send(buffer, buffer.Length, SocketFlags.None);

                int RecvLength = clientSocket.Receive(lengthBuffer, 2, SocketFlags.None);
                length = BitConverter.ToUInt16(lengthBuffer, 0);
                length = (ushort)IPAddress.NetworkToHostOrder((short)length);


                byte[] recvBuffer = new byte[4096];
                RecvLength = clientSocket.Receive(recvBuffer, length, SocketFlags.None);

                string JsonString = Encoding.UTF8.GetString(recvBuffer);

                Console.WriteLine(JsonString);

                Thread.Sleep(200);
            }

            clientSocket.Close();


            //MessageObject mo = new MessageObject("안녕하세요");
            //string jsonData = JsonConvert.SerializeObject(mo);

            //Console.WriteLine($"Client : \"{jsonData}\"");

            //Socket serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            //IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);
            //serverSocket.Connect(serverEndPoint); //bind

            //byte[] buffer;

            //buffer = Encoding.UTF8.GetBytes(jsonData);
            //int SendLength = serverSocket.Send(buffer, 0, buffer.Length, SocketFlags.None);

            //byte[] buffer2 = new byte[1024];
            //int RecvLength = serverSocket.Receive(buffer2);

            //Console.WriteLine($"Server : \"{Encoding.UTF8.GetString(buffer2)}\"");

            //serverSocket.Close();
        }
    }
}