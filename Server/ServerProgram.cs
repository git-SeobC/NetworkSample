using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    public class MessageObject
    {
        public MessageObject(string pMessage)
        {
            message = pMessage;
        }

        public string message;
    }

    class ServerProgram
    {
        static void Main(string[] args)
        {
            MessageObject mo2 = new MessageObject("");

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 4000);
            listenSocket.Bind(listenEndPoint);

            listenSocket.Listen(10);

            bool isRunning = true;
            while (isRunning)
            {
                //동기, 블록킹 
                Socket clientSocket = listenSocket.Accept();

                byte[] buffer = new byte[1024];
                int RecvLength = clientSocket.Receive(buffer);

                //100+200
                string message = Encoding.UTF8.GetString(buffer);

                mo2 = JsonConvert.DeserializeObject<MessageObject>(message);

                if (mo2.message.Equals("안녕하세요"))
                {
                    mo2.message = "반갑습니다";
                }
                else
                {
                    mo2.message = "인사도없네?";
                }

                message = JsonConvert.SerializeObject(mo2);

                Console.WriteLine($"Server : \"{message}\" ");

                buffer = Encoding.UTF8.GetBytes(message);
                int SendLength = clientSocket.Send(buffer);


                clientSocket.Close();
            }

            listenSocket.Close();
        }
    }
}