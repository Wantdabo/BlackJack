using Network.External;
using Network.Inneral;
using System;
using System.Collections.Generic;
using System.Text;
using SimpleLog;
using Network;
using System.Threading;
using System.Net;

namespace Gate
{
    public class Gate
    {
        private int sessionID = 0;
        private static Gate gate;
        private Dictionary<short, UDPInfo> dispInfos;
        public Dictionary<int, SocketInfo> sessions;
        UDPInfo loginInfo;
        UDPInfo gameInfo;
        public static Gate Instance
        {
            get
            {
                if (gate == null)
                    gate = new Gate();

                return gate;
            }
        }

        private Gate()
        {
            sessions = new Dictionary<int, SocketInfo>();
            dispInfos = new Dictionary<short, UDPInfo>();
            loginInfo.ip = "127.0.0.1";
            loginInfo.port = 10001;

            gameInfo.ip = "127.0.0.1";
            gameInfo.port = 10002;

            dispInfos.Add(10001, loginInfo);
            dispInfos.Add(10002, gameInfo);
        }

        public void Start()
        {
            InneralCall ic;
            ExternalCall ec = null;

            Log.Print("gate start...", ConsoleColor.Green);

            ic = new InneralCall((_info) =>
            {
                BufferRW brw = new BufferRW();
                brw.BeginRead(_info.buffer);
                int proto = brw.ReadInt16();
                int session = brw.ReadInt32();
                brw.EndRead();
                if (sessions.ContainsKey(session)) {
                    ec.Call(_info.buffer, sessions[session]);
                }
            }, 10000);

            ec = new ExternalCall((_info) =>
            {
                Log.Print(_info.socket.RemoteEndPoint.ToString() + "\t connected.", ConsoleColor.Blue);
                ec.Call(AddSession(_info), _info);
            }, (_info) =>
            {
                BufferRW brw = new BufferRW();
                brw.BeginRead(_info.buffer);
                short proto = brw.ReadInt16();
                int session = brw.ReadInt32();
                brw.EndRead();
                if (sessions.ContainsKey(session) && dispInfos.ContainsKey(proto))
                    ic.Call(_info, dispInfos[proto]);
            },
            (info) =>
            {
                Log.Print(info.socket.RemoteEndPoint.ToString() + "\t disconnected.", ConsoleColor.Red);
            },
            20000);
        }

        private byte[] AddSession(SocketInfo _info) {
            sessionID++;
            sessions.Add(sessionID, _info);
            BufferRW brw = new BufferRW();
            brw.BeginWrite();
            brw.WriteInt16(10000);
            brw.WriteInt32(sessionID);

            return brw.EndWrite();
        }
    }
}
