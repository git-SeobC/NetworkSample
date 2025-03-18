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
            // user mode, kernel mode
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, 4000);

            listenSocket.Bind(listenEndPoint);

            listenSocket.Listen(10); // backlog는 동시 접속 가능 자 수
            // 여기까지 Socket 처리

            Socket clientSocket = listenSocket.Accept(); // 접속 대기

            byte[] buffer = new byte[1024];

            // 네트워크에서 바로 받는 것이 아니라, OS가 미리 받아온거에서 일정 크기 잘라옴
            // OS 내부 버퍼에서 복사해옴, -> 자료의 전부를 가져오는 것이 아님
            int RecvLength = clientSocket.Receive(buffer);
            // TCP는 누군가에게 받은 자료가 틀리지 않았다는 전제에 사용하는 프로토콜

            string JsonString = Encoding.UTF8.GetString(buffer);

            JObject json = JObject.Parse(JsonString);
            //JsonConvert.DeserializeObject<Message>(JsonString);

            if (json.Value<string>("message").ToString().CompareTo("안녕하세요") == 0)
            {
                byte[] message;
                JObject result = new JObject();
                result.Add("message", "반가워");
                message = Encoding.UTF8.GetBytes(result.ToString());

                // OS 내부 버퍼에 복사 함, 자료의 전부를 보내는게 아님 (컴퓨터가 바쁠 때)
                int SendLength = clientSocket.Send(message);
                // SendLenght나 RecvLength를 통해 정확한 사이즈가 도착했는지 확인하는 작업이 있어야함
            }

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