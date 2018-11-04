using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace QICore.NetSocketClient.Common
{
    public class SocketClient
    {
        static Socket socketClient = null;////定义一个套接字用于监听客户端发来的消息，包含三个参数（IP4寻址协议，流式连接，Tcp协议）  
            //监听数据
        public static void Listen()
        {
            socketClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socketClient.Connect(new IPEndPoint(IPAddress.Any, 2014));//客户端套接字连接到网络节点上，用的是Connect  
            IPAddress clientIP = (socketClient.RemoteEndPoint as IPEndPoint).Address;
            string sendmsg = "连接服务端成功！\r\n" + "本地IP:" + clientIP.Address + "，本地端口" + clientIP.ToString();
            byte[] arrSendMsg = Encoding.UTF8.GetBytes(sendmsg);
            Console.WriteLine(arrSendMsg);

            Thread threadwatch = new Thread(Receive);//负责监听客户端的线程:创建一个监听线程 
            threadwatch.IsBackground = true;//将窗体线程设置为与后台同步，随着主线程结束而结束  
            threadwatch.Start();//启动线程   
            Console.WriteLine("开始连接....");
        }
        /// <summary>
        ///接收服务端发来信息的方法
        /// </summary>
        private static void Receive()
        {
            while (true)
            {
                //创建一个内存缓冲区，其大小为1024*1024字节  即1M     
                byte[] arrServerRecMsg = new byte[1024 * 1024];
                //将接收到的信息存入到内存缓冲区，并返回其字节数组的长度    
                try
                {
                    int length = socketClient.Receive(arrServerRecMsg);//收到服务端信息
                    //将机器接受到的字节数组转换为人可以读懂的字符串     
                    string strSRecMsg = Encoding.UTF8.GetString(arrServerRecMsg, 0, length);
                    Console.WriteLine($"{DateTime.Now}:{strSRecMsg}");
                    //将发送的字符串信息附加到文本框txtMsg上     
                    Console.WriteLine("客户端:" + socketClient.RemoteEndPoint + ",时间:" + DateTime.Now + "\r\n" + strSRecMsg + "\r\n\n");

                }
                catch (Exception ex)
                {
                    //提示套接字监听异常  
                    Console.WriteLine("客户端" + socketClient.RemoteEndPoint + "已经中断连接" + "\r\n" + ex.Message + "\r\n" + ex.StackTrace + "\r\n");
                    //关闭之前accept出来的和客户端进行通信的套接字 
                    socketClient.Close();
                    break;
                }
            }
        }       
    }
}
