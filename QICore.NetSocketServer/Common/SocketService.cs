using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace QICore.NetSocketServer.Common
{
    public class SocketService
    {
        //监听数据
        private void Listen()
        {
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listener.Bind(new IPEndPoint(IPAddress.Any, 2014));
            //不断监听端口
            while (true)
            {
                listener.Listen(0);
                Socket socket = listener.Accept();
                NetworkStream ntwStream = new NetworkStream(socket);
                StreamReader strmReader = new StreamReader(ntwStream);
              
                socket.Close();
            }
            //程序的listener一直不关闭
            //listener.Close();
        }
    }
}
