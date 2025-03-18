using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
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
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);

            clientSocket.Connect(listenEndPoint);

            string jsonString = "{\"message\" : \"안녕하세요\"}";
            byte[] message = Encoding.UTF8.GetBytes(jsonString);
            int SendLength = clientSocket.Send(message);

            byte[] buffer = new byte[1024];

            int RecvLength = clientSocket.Receive(buffer);

            string JsonString = Encoding.UTF8.GetString(buffer);

            Console.WriteLine(JsonString);

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