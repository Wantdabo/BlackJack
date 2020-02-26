using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Network
{
    // SocketInfo Callback
    public delegate void SocketInfoCallback(SocketInfo _socketInfo);
    // 公用
    public class Network
    {
        // 默认 IP
        public const string DEFAULT_IP = "127.0.0.1";

        // BUFFER 缓冲区
        public const int MAX_BUFFER = 65535;

        public const int MAX_BACK_LOG = 1000;
    }

    public struct SocketInfo
    {
        public Socket socket;
        public int readByte;
        public byte[] buffer;
    }

    public struct UDPInfo {
        public string ip;
        public int port;
    }
}
