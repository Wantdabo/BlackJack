using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Network
{
    namespace External
    {
        public class ExternalCall
        {
            private Socket serverSocket;
            private SocketInfoCallback acceptCallback;
            private SocketInfoCallback receiveCallback;
            private SocketInfoCallback disConnCallback;

            public ExternalCall(SocketInfoCallback _acceptCallback, SocketInfoCallback _receiveCallback, SocketInfoCallback _disConnCallback, int _port, string _ip = Network.DEFAULT_IP)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(new IPEndPoint(IPAddress.Parse(_ip), _port));
                serverSocket.Listen(Network.MAX_BACK_LOG);

                acceptCallback = _acceptCallback;
                receiveCallback = _receiveCallback;
                disConnCallback = _disConnCallback;

                new Thread(() =>
                {
                    while (true)
                    {
                        SocketInfo socketInfo;
                        socketInfo.socket = serverSocket.Accept();
                        socketInfo.readByte = 0;
                        socketInfo.buffer = new byte[Network.MAX_BUFFER];
                        acceptCallback.Invoke(socketInfo);
                        new Thread(() => Receive(socketInfo)).Start();
                    }
                }).Start();
            }

            public ExternalCall() { }
            public SocketInfo Connect(SocketInfoCallback _receiveCallback, int _port, string _ip = Network.DEFAULT_IP)
            {
                SocketInfo socketInfo;
                socketInfo.socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socketInfo.socket.Connect(new IPEndPoint(IPAddress.Parse(_ip), _port));
                socketInfo.readByte = 0;
                socketInfo.buffer = new byte[Network.MAX_BUFFER];

                receiveCallback = _receiveCallback;
                new Thread(() => Receive(socketInfo)).Start();

                return socketInfo;
            }

            public void Receive(SocketInfo _socketInfo)
            {
                while (true)
                {
                    try
                    {
                        _socketInfo.readByte = _socketInfo.socket.Receive(_socketInfo.buffer);
                        receiveCallback.Invoke(_socketInfo);
                    }
                    catch (Exception e) {
                        disConnCallback.Invoke(_socketInfo);
                        break;
                    }
                }
            }

            public void Call(byte[] _data, SocketInfo _socketInfo)
            {
                _socketInfo.socket.Send(_data);
            }
        }
    }
}