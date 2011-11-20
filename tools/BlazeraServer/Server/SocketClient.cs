using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace BlazeraServer
{
    public class SocketClient : TcpClient
    {
        public Socket Socket { get; private set; }

        public SocketClient(Socket socket)
        {
            Socket = socket;
        }
    }
}
