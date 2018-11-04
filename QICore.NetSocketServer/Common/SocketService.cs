using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.NetSocketServer.Common
{
    public class SocketService
    {
        static Socket listener = null;////定义一个套接字用于监听客户端发来的消息，包含三个参数（IP4寻址协议，流式连接，Tcp协议）  
        //定义一个集合，存储客户端信息
        static Dictionary<string, Socket> clientConnectionItems = new Dictionary<string, Socket> { };
        //监听数据
        public static void Listen()
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(address, 2014));//服务端发送信息需要一个IP地址和端口号  
            listener.Listen(20);////将套接字的监听队列长度限制为20 
            //Thread threadwatch = new Thread(watchClientConnecting);//负责监听客户端的线程:创建一个监听线程 
            //threadwatch.IsBackground = true;//将窗体线程设置为与后台同步，随着主线程结束而结束  
            //threadwatch.Start();//启动线程   

            #region  创建一个监听线程   
            ParameterizedThreadStart sms = new ParameterizedThreadStart(watchClientConnecting);
            Thread smsthread = new Thread(sms);            
            smsthread.IsBackground = true;//设置为后台线程，随着主线程退出而退出 
            smsthread.Start(listener);//启动线程     
            #endregion

            Console.WriteLine("开启监听....");
        }
        /// <summary>
        /// //监听客户端发来的请求  
        /// </summary>
        private static void watchClientConnecting(object socket)
        {
            Socket connection = socket as Socket;
            //持续不断监听客户端发来的请求     
            while (true)
            {
                try
                {
                    connection = listener.Accept();
                }
                catch (Exception ex)
                {
                    //提示套接字监听异常     
                    Console.WriteLine(ex.Message);
                    break;
                }
                //获取客户端的IP和端口号  
                IPAddress clientIP = (connection.RemoteEndPoint as IPEndPoint).Address;
                int clientPort = (connection.RemoteEndPoint as IPEndPoint).Port;

                //让客户显示"连接成功的"的信息  
                string sendmsg = "连接服务端成功！" + "IP:" + clientIP + "，端口" + clientPort.ToString();
                byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
                connection.Send(arrSendMsg);//发送信息到客户端

                //客户端网络结点号  
                string remoteEndPoint = connection.RemoteEndPoint.ToString();
                //显示与客户端连接情况
                Console.WriteLine("成功与" + remoteEndPoint + "客户端建立连接！\t\n");
                //添加客户端信息  
                clientConnectionItems.Add(remoteEndPoint, connection);              
                IPEndPoint netpoint = connection.RemoteEndPoint as IPEndPoint;
                #region  创建一个通信线程 接收消息     
                ParameterizedThreadStart pts = new ParameterizedThreadStart(Receive);
                Thread thread = new Thread(pts);
                //设置为后台线程，随着主线程退出而退出 
                thread.IsBackground = true;
                //启动线程     
                thread.Start(connection);
                #endregion
                #region  创建一个通信线程 发送消息     
                ParameterizedThreadStart sms = new ParameterizedThreadStart(Send);
                Thread smsthread = new Thread(sms);
                //设置为后台线程，随着主线程退出而退出 
                smsthread.IsBackground = true;
                //启动线程     
                smsthread.Start(connection);
                #endregion
            }
        }
        /// <summary>
        /// 接收客户端发来的信息，客户端套接字对象
        /// </summary>
        /// <param name="socketclientpara"></param>    
        static void Receive(object socketclientpara)
        {
            Socket socketServer = socketclientpara as Socket;

            while (true)
            {
                //创建一个内存缓冲区，其大小为1024*1024字节  即1M     
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                //将接收到的信息存入到内存缓冲区，并返回其字节数组的长度    
                try
                {
                    int length = socketServer.Receive(arrServerRecMsg);//收到客户端信息
                    //将机器接受到的字节数组转换为人可以读懂的字符串     
                    string strSRecMsg = Encoding.UTF8.GetString(arrServerRecMsg, 0, length);
                    //将发送的字符串信息附加到文本框txtMsg上     
                    Console.WriteLine("客户端:" + socketServer.RemoteEndPoint + ",时间:" + DateTime.Now + "\r\n" + strSRecMsg + "\r\n\n");
                    socketServer.Send(Encoding.UTF8.GetBytes("测试server 是否可以发送数据给client "));
                }
                catch (Exception ex)
                {
                    var clientIP = socketServer.RemoteEndPoint;
                    var msg = ex.Message;
                    var trace = ex.StackTrace;
                    Console.WriteLine("Client Count:" + clientConnectionItems.Count);
                    //提示套接字监听异常  
                    Console.WriteLine("客户端" + clientIP + "已经中断连接" + "\r\n" + msg + "\r\n" + trace + "\r\n");
                    //关闭之前accept出来的和客户端进行通信的套接字 
                    socketServer.Close();
                    break;
                }
            }
        }
        static void Send(object connection)
        {
            Socket socket = connection as Socket;
            var n = 0;
            while (true)
            {
                try
                {
                    //让客户显示"连接成功的"的信息  
                    string sendmsg = $"{n++}你好吗？";
                    byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
                    socket.Send(arrSendMsg);//发送信息到客户端
                    Thread.Sleep(5);
                }
                catch (Exception ex)
                {
                    var clientIP = socket.RemoteEndPoint;
                    var msg = ex.Message;
                    var trace = ex.StackTrace;
                    Console.WriteLine("Client Count:" + clientConnectionItems.Count);
                    //提示套接字监听异常  
                    Console.WriteLine("客户端" + clientIP + "已经中断连接" + "\r\n" + msg + "\r\n" + trace + "\r\n");
                    //关闭之前accept出来的和客户端进行通信的套接字 
                    socket.Close();
                    break;
                }
            }
        }
    }
}
