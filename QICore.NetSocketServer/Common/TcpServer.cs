﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.NetSocketServer.Common
{
    public class TcpServer
    {

        static readonly StatisticsInfo Statistics = new StatisticsInfo();

        private static TcpClient tcpServer = null;
        private static TcpClient tcpClient = null;
        private static NetworkStream workStream = null;
        public ManualResetEvent connectDone = new ManualResetEvent(false);

        private static TcpListener _Listener { get; set; }

        private static bool _Accept { get; set; } = false;

        public static void StartServer(string ip, int port)
        {
            IPAddress address = IPAddress.Parse(ip);
            _Listener = new TcpListener(address, port);
           // _Listener = new TcpListener(IPAddress.Any, port);
            _Listener.Start();
            _Accept = true;
            Console.WriteLine($"Server Started. 监听客户端的TCP地址和端口： {ip}:{port}");
        }
        public static  void Listen()
        {
            byte[] serverData = Encoding.UTF8.GetBytes("香港人民 ");
            //var sendBuffer = new ArraySegment<byte>(serverData, 0, serverData.Length);
            if (_Listener != null && _Accept)
            {
                //监听 Continue listening.
                while (true)
                {
                    Console.WriteLine("等待客户端：Waiting for client...");
                    var tcpClient =_Listener.AcceptTcpClientAsync(); // 获取客户端
                    if (tcpClient.Result != null)
                    {
                        var endPoint = (System.Net.IPEndPoint)tcpClient.Result.Client.RemoteEndPoint;//获取远程连接IP
                        var clientIP = endPoint.Address;//客户端连的地址
                        var port = endPoint.Port;//客户端连的端口

                        Console.WriteLine($"客户端已连接》{clientIP}:{port}");
                        var client = tcpClient.Result;//接收到客户端的数据
                        string message = "";
                        while (message!=null && !message.StartsWith("quit"))
                        {
                            #region 接收到客户端的消息
                            byte[] buffer = new byte[256];
                            client.GetStream().Read(buffer, 0, buffer.Length);//接收信息
                            message = Encoding.UTF8.GetString(buffer);
                            Console.WriteLine("来自客户端消息：" + message.TrimEnd('\0'));
                            #endregion
                            #region 发送消息到客户端
                            if (!string.IsNullOrEmpty(message))
                            {
                                byte[] data = Encoding.UTF8.GetBytes("******** 欢迎来到服务端 ******** ");
                                client.GetStream().Write(data, 0, data.Length);//发送信息
                            }
                            #endregion

                            // tcpClient.GetAwaiter().GetResult().Client.Send(serverData, SocketFlags.None);//把消息发送回给客户端
                        }
                        Console.WriteLine("关闭连接");
                        client.GetStream().Dispose();
                    }

                }

            }

        }
        public static async void ListenAsync()
        {
            if (_Listener != null && _Accept)
            {
                byte[] buffer = new byte[1024];
                //var buffer = new ArraySegment<byte>(buffer);
                //监听 Continue listening.
                while (true)
                {
                    Console.WriteLine("等待客户端：Waiting for client...");
                    var tcpClient =await _Listener.AcceptTcpClientAsync(); // 获取客户端
                    var endPoint = (System.Net.IPEndPoint)tcpClient.Client.RemoteEndPoint;//获取远程连接IP
                    var clientIP = endPoint.Address;//客户端连的地址
                    var port = endPoint.Port;//客户端连的端口
                    Console.WriteLine($"客户端已连接》{clientIP}:{port}");
                    workStream = tcpClient.GetStream();
                    if (workStream == null)
                    {
                        Console.WriteLine($"接收到客户端数据为 null》{clientIP}:{port}");
                        workStream.Close();
                        workStream.Dispose();
                        Console.WriteLine("关闭连接");
                    }
                    else
                    {
                        while (true)
                        {
                            #region 接收到客户端的消息                       
                            await workStream.ReadAsync(buffer, 0, buffer.Length);//接收信息
                            var message = Encoding.UTF8.GetString(buffer);
                            Console.WriteLine("来自客户端消息：" + message.TrimEnd('\0'));
                            #endregion
                            #region 发送消息
                            byte[] data = Encoding.UTF8.GetBytes("******** 欢迎来到服务端 ******** ");
                            await workStream.WriteAsync(data, 0, data.Length);//发送信息     
                            Console.WriteLine("已把数据发送到客户端");
                            #endregion
                        }
                    }

                    //var len = await tcpClient.Client.ReceiveAsync(buffer, SocketFlags.None);
                    //if (len == 0)
                    //{
                    //    tcpClient.Dispose();
                    //    break;
                    //}
                    //Console.WriteLine($"客户端已连接，接收到的数据:{len}");
                    //var sendBuffer = new ArraySegment<byte>(buffer, 0, len);
                    //await tcpClient.Client.SendAsync(sendBuffer, SocketFlags.None);//把消息发送回给客户端
                    //Console.WriteLine("已把数据发送到客户端");
                    //tcpClient.GetStream().Dispose();
                    //Console.WriteLine("关闭连接");                   

                }

            }

        }
    }
    public class StatisticsInfo
    {
        public int ClientCount;
        public int ReceivedTimes;
        public int SendTimes;

        public void IncrementClient() => Interlocked.Increment(ref ClientCount);
        public void DecrementClient() => Interlocked.Decrement(ref ClientCount);

        public void IncrementReceivedTimes() => Interlocked.Increment(ref ReceivedTimes);
        //public void DecrementReceivedTimes() => Interlocked.Decrement(ref ReceivedTimes);

        public void IncrementSendTimes() => Interlocked.Increment(ref SendTimes);
        //public void DecrementSendTimes() => Interlocked.Decrement(ref SendTimes);

        public override string ToString()
        {
            return $"ClientCount: {ClientCount}, ReceivedTimes: {ReceivedTimes}, SendTimes: {SendTimes}";
        }
    }
}
