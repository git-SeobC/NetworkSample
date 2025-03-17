using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {
            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            // 실제 랜선과 연결하는 작업
            IPEndPoint listenEndPoint = new IPEndPoint(IPAddress.Any, 4000);
            listenSocket.Bind(listenEndPoint);

            listenSocket.Listen(10);

            bool isRunning = true;
            while (isRunning)
            {
                // 동기 방식 (블록킹) -> 들어올 때 까지 대기 함
                Socket clientSocket = listenSocket.Accept();

                byte[] buffer = new byte[1024];
                int recvLength = clientSocket.Receive(buffer);
                if (recvLength <= 0)
                {
                    // recvLength == 0 : close
                    // recvLength < 0 : Error
                    isRunning = false;
                }

                int sendLength = clientSocket.Send(buffer);
                if (sendLength <= 0)
                {
                    // close
                    // Error
                    isRunning = false;

                }
                // keep alive time -> 붙어있나 확인하는 시간 보통 3분 무조건 사용 다 하고나선 Close 해주어야함. os에선 모름
                clientSocket.Close();
            }
            listenSocket.Close();
        }
    }
}
