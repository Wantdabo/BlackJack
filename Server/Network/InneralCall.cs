using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace Network
{
    namespace Inneral
    {
        public class InneralCall
        {
            private Socket socket;
            private SocketInfo socketInfo;
            // BUFFER 处理回调
            private SocketInfoCallback receiveCallback;
            // 服务器字典 配合绑定使用
            private Dictionary<string, IPEndPoint> ipEndPointDict;

            // 构造一个 UDP 通信器
            public InneralCall(SocketInfoCallback _receiveCallback, int _port, string _ip = Network.DEFAULT_IP)
            {
                // 构造一个 UDP Socket
                socketInfo.socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socketInfo.socket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));

                // 一系列初始化
                receiveCallback = _receiveCallback;
                ipEndPointDict = new Dictionary<string, IPEndPoint>();

                socketInfo.readByte = 0;
                socketInfo.buffer = new byte[Network.MAX_BUFFER];

                // 开启异步接收
                socketInfo.socket.BeginReceive(socketInfo.buffer, 0, socketInfo.buffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceive), null);
            }

            // 异步接收回调
            public void AsyncReceive(IAsyncResult _ir)
            {
                // 执行调用者传入的回调
                socketInfo.readByte = socketInfo.socket.EndReceive(_ir);
                receiveCallback.Invoke(socketInfo);
                socketInfo.socket.BeginReceive(socketInfo.buffer, 0, socketInfo.buffer.Length, SocketFlags.None, new AsyncCallback(AsyncReceive), null);
            }

            // 绑定 Socket 提供根据名字调用
            public void BindSocketName(string _serverName, string _ip, int _port)
            {
                ipEndPointDict.Add(_serverName, new IPEndPoint(IPAddress.Parse(_ip), _port));
            }

            // 一系列对外开放的 Call 方法
            public void Call(SocketInfo _socketInfo, int _port, string _ip = Network.DEFAULT_IP)
            {
                socketInfo.socket.SendTo(_socketInfo.buffer, 0, _socketInfo.readByte, SocketFlags.None, new IPEndPoint(IPAddress.Parse(_ip), _port));
            }

            public void Call(SocketInfo _socketInfo, IPEndPoint _ipEndPoint)
            {
                socketInfo.socket.SendTo(_socketInfo.buffer, 0, _socketInfo.readByte, SocketFlags.None, _ipEndPoint);
            }

            public void Call(SocketInfo _socketInfo, string _serverName)
            {
                socketInfo.socket.SendTo(_socketInfo.buffer, 0, _socketInfo.readByte, SocketFlags.None, ipEndPointDict[_serverName]);
            }

            public void Call(SocketInfo _socketInfo, UDPInfo _udpInfo) {
                socketInfo.socket.SendTo(_socketInfo.buffer, 0, _socketInfo.readByte, SocketFlags.None, new IPEndPoint(IPAddress.Parse(_udpInfo.ip), _udpInfo.port));
            }
        }
    }
}