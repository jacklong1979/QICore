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

        private static async Task RunClientAsync(TcpClient client)
        {
            var receiveBufferBytes = new byte[1024];

            var sendBytes = Encoding.UTF8.GetBytes("中国人民");
            while (true)
            {
                try
                {
                    var sendBuffer = new ArraySegment<byte>(sendBytes, 0, sendBytes.Length);
                    await client.Client.SendAsync(sendBuffer, SocketFlags.None);//发送消息给服务端
                   // Statistics.IncrementSendTimes();

                    var receiveBuffer = new ArraySegment<byte>(receiveBufferBytes);
                    var len = await client.Client.ReceiveAsync(receiveBuffer, SocketFlags.None);//接收到服务端的消息
                    if (len == 0)
                    {
                        client.Dispose();
                       // Statistics.DecrementClient();
                        break;
                    }
                    //var receiveBuffer = new byte[len];
                    string str = System.Text.Encoding.Default.GetString(receiveBuffer.ToArray());
                    Console.WriteLine($"收到服务端的消息: {str.TrimEnd('\0')}");
                    // Statistics.IncrementReceivedTimes();

                    await Task.Delay(2 * 1000);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"RunClientAsync Error: {ex}");
                    client.Dispose();
                   // Statistics.DecrementClient();
                    break;
                }
            }
        }
    }
  
}
