using Network;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Game
{
    public struct RoomRes
    {
        public short proto;
        public int session;
        public short opt;
        public short roomID;
    }

    public enum ROOM_STATUS
    {
        STANDBY,
        GAMING,

    }
    class Room
    {
        private List<Player> players;
        private int roomID;
        private ROOM_STATUS status;
        public int ROOM_ID
        {
            get
            {
                return roomID;
            }
            set
            {
                roomID = value;
            }
        }
        public Room()
        {
            players = new List<Player>();
            status = ROOM_STATUS.STANDBY;
        }

        public void Start()
        {
            if (status != ROOM_STATUS.STANDBY) return;
            status = ROOM_STATUS.GAMING;
            new Thread(() =>
            {
                while (players.Count > 0)
                {
                    Update();
                }
                status = ROOM_STATUS.STANDBY;
            }).Start();
        }

        public void Update()
        {

        }

        public bool AddPlayer(Player _player)
        {
            if (players.Count > Game.Instance.ROOM_PLAYER_COUNT)
                return false;
            players.Add(_player);
            Start();

            return true;
        }

        public void Response(SocketInfo _info)
        {
            RoomRes rr;
            BufferRW brw = new BufferRW();
            brw.BeginRead(_info.buffer);
            rr.proto = brw.ReadInt16();
            rr.session = brw.ReadInt32();
            rr.opt = brw.ReadInt16();
            rr.roomID = brw.ReadInt16();
            brw.EndRead();
            if (rr.opt == 0)
                QueryPlayer(rr);
            else
                HandleRes(rr);
        }

        public void QueryPlayer(RoomRes _rr)
        {
            SocketInfo info;
            BufferRW brw = new BufferRW();
            brw.WriteInt16(_rr.proto);
            brw.WriteInt32(_rr.session);
            brw.WriteInt16(_rr.opt);
            brw.WriteInt16(_rr.roomID);
            brw.WriteInt32(Game.Instance.GetPlayerCount(_rr.session, _rr.roomID));
            foreach (Player p in players)
            {
                if (p.session == _rr.session)
                {
                    brw.WriteString(p.name);
                    brw.WriteInt64(p.money);
                }
            }
            info.buffer = brw.EndWrite();
            info.readByte = info.buffer.Length;
            info.socket = null;
            Game.Instance.Call(info);
        }

        public void HandleRes(RoomRes _rr) { }

        public List<Player> GetPlayers()
        {
            return players;
        }

        public int PLAYER_COUNT
        {

            get
            {
                return players.Count;
            }
        }
    }
}
