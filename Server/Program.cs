using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
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

            List<Socket> clientSockets = new List<Socket>();
            List<Socket> checkRead = new List<Socket>(); // 연결 감시용 소켓

            while (true)
            {
                checkRead.Clear();
                checkRead = new List<Socket>(clientSockets);
                checkRead.Add(listenSocket);

                Socket.Select(checkRead, null, null, -1);

                foreach (Socket findSocket in checkRead)
                {
                    if (findSocket == listenSocket)
                    {
                        Socket clientSocket = listenSocket.Accept();
                        clientSockets.Add(clientSocket);
                        Console.WriteLine($"Connect client : {clientSocket.RemoteEndPoint}");
                    }
                    else
                    {
                        byte[] headerBuffer = new byte[2];
                        int RecvLength = findSocket.Receive(headerBuffer, 2, SocketFlags.None);
                        if (RecvLength > 0)
                        {
                            short packetLength = BitConverter.ToInt16(headerBuffer, 0);
                            packetLength = IPAddress.NetworkToHostOrder(packetLength);

                            byte[] dataBuffer = new byte[4096];
                            RecvLength = findSocket.Receive(dataBuffer, packetLength, SocketFlags.None);

                            string JsonString = Encoding.UTF8.GetString(dataBuffer);

                            Console.WriteLine(JsonString);

                            JObject clientData = JObject.Parse(JsonString);

                            string message = "{ \"message\" : \"" + clientData.Value<String>("message") + "\"}";
                            byte[] messageBuffer = Encoding.UTF8.GetBytes(message);
                            ushort length = (ushort)IPAddress.HostToNetworkOrder((short)messageBuffer.Length);

                            headerBuffer = BitConverter.GetBytes(length);

                            byte[] packetBuffer = new byte[headerBuffer.Length + messageBuffer.Length]; // 실제 전송 데이터 버퍼 = 패킷

                            Buffer.BlockCopy(headerBuffer, 0, packetBuffer, 0, headerBuffer.Length);
                            Buffer.BlockCopy(messageBuffer, 0, packetBuffer, headerBuffer.Length, messageBuffer.Length);

                            foreach (Socket sendSock in clientSockets) // 클라이언트에 전체 답장
                            {
                                int SendLength = findSocket.Send(packetBuffer, packetBuffer.Length, SocketFlags.None);
                            }
                        }
                        else
                        {
                            findSocket.Close();
                            checkRead.Remove(findSocket);
                        }
                    }
                }
                //Server 작업
                {
                    Console.WriteLine("서버 작업");
                }
            }
            listenSocket.Close();
        }
    }
}