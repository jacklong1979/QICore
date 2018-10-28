using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace QICore.NetSocketClient.Common
{
    public class TcpSocketClient
    {

        private static NetworkStream workStream = null;
        public static async Task StartClientAsync(string host, int port)
        {
            var client = new TcpClient();
            try
            {
                await client.ConnectAsync(host, port);
                Console.WriteLine($"已连接服务端》 {host}：{port}");
                await RunClientAsync(client);
            }
            catch (SocketException ex)
            {
                Console.WriteLine($"Connect Error: SocketErrorCode = {ex.SocketErrorCode},  {ex}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connect Error: {ex}");
            }
        }

        private static async Task RunClientAsync(TcpClient tcpClient)
        {
            var buffer = new byte[1024];

            var data = Encoding.UTF8.GetBytes("中国人民");
            while (true)
            {
                try
                {
                    var endPoint = (System.Net.IPEndPoint)tcpClient.Client.RemoteEndPoint;//获取远程连接IP
                    var clientIP = endPoint.Address;//客户端连的地址
                    var port = endPoint.Port;//客户端连的端口
                    workStream = tcpClient.GetStream();
                    if (workStream == null)
                    {
                        Console.WriteLine($"接收到服务端数据为 null》{clientIP}:{port}");
                        workStream.Close();
                        workStream.Dispose();
                        Console.WriteLine("关闭连接");
                    }
                    else
                    {
                        while (true)
                        {
                            #region 发送消息                       
                            await workStream.WriteAsync(data, 0, data.Length);//发送信息     
                            Console.WriteLine("已把数据发送到服务端");
                            #endregion
                            #region 接收到客户端的消息                       
                            await workStream.ReadAsync(buffer, 0, buffer.Length);//接收信息
                            var message = Encoding.UTF8.GetString(buffer);
                            Console.WriteLine("来自服务端消息：" + message.TrimEnd('\0'));
                            #endregion
                            await Task.Delay(2 * 1000);
                        }
                    }
                    // var sendBuffer = new ArraySegment<byte>(sendBytes, 0, sendBytes.Length);
                    // await tcpClient.Client.SendAsync(sendBuffer, SocketFlags.None);//发送消息给服务端
                    //// Statistics.IncrementSendTimes();

                    // var receiveBuffer = new ArraySegment<byte>(buffer);
                    // var len = await tcpClient.Client.ReceiveAsync(receiveBuffer, SocketFlags.None);//接收到服务端的消息
                    // if (len == 0)
                    // {
                    //     Console.WriteLine($"收到服务端的消息 null:");
                    //     tcpClient.Dispose();
                    //    // Statistics.DecrementClient();
                    //     break;
                    // }
                    // //var receiveBuffer = new byte[len];
                    // string str = System.Text.Encoding.Default.GetString(receiveBuffer.ToArray());
                    // Console.WriteLine($"收到服务端的消息: {str.TrimEnd('\0')}");
                    // // Statistics.IncrementReceivedTimes();

                    await Task.Delay(2 * 1000);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"发生异常: {ex.ToString()}");
                    //Console.WriteLine($"RunClientAsync Error: {ex}");
                    tcpClient.Dispose();
                   // Statistics.DecrementClient();
                    break;
                }
            }
        }
    }
  
}
