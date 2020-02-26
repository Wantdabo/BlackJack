using Network;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public struct HallRes
    {
        public short proto;
        public int session;
        public short opt;
        public short roomID;
    }

    class Hall
    {
        private static Hall hall;
        public static Hall Instance
        {
            get
            {
                if (hall == null)
                    hall = new Hall();

                return hall;
            }
        }

        private Hall()
        {

        }

        public void Response(SocketInfo _info)
        {
            BufferRW brw = new BufferRW();
            brw.BeginRead(_info.buffer);
            HallRes hr;
            hr.proto = brw.ReadInt16();
            hr.session = brw.ReadInt32();
            hr.opt = brw.ReadInt16();
            hr.roomID = brw.ReadInt16();
            brw.EndRead();

            if (hr.opt == 0) // 查询
            {
                QueryHall(hr);
            }
            else if (hr.opt == 1) // 进入
            {
                JoinRoom(hr);
            }
        }

        public void QueryHall(HallRes _hr)
        {
            if (Game.Instance.CheckPlayer(_hr.session))
            {
                SocketInfo info;
                Player player = Game.Instance.GetPlayer(_hr.session);
                List<Room> rooms = Game.Instance.GetRooms();
                BufferRW brw = new BufferRW();
                brw.BeginWrite();
                brw.WriteInt16(_hr.proto);
                brw.WriteInt32(_hr.session);
                brw.WriteInt16(_hr.opt);
                brw.WriteInt16(_hr.roomID);
                brw.WriteString(player.name);
                brw.WriteInt64(player.money);
                brw.WriteInt16((short)rooms.Count);
                foreach (Room room in rooms)
                    brw.WriteInt16((short)room.PLAYER_COUNT);

                info.buffer = brw.EndWrite();
                info.socket = null;
                info.readByte = info.buffer.Length;

                Game.Instance.Call(info);
            }
        }

        public void JoinRoom(HallRes _hr)
        {
            SocketInfo info;
            Player player = Game.Instance.GetPlayer(_hr.session);
            bool joinStatus = Game.Instance.JoinGame(_hr.roomID, player);
            List<Player> players = Game.Instance.GetPlayers(_hr.roomID);
            BufferRW brw = new BufferRW();
            brw.BeginWrite();
            brw.WriteInt16(_hr.proto);
            brw.WriteInt32(_hr.session);
            brw.WriteInt16(_hr.opt);
            brw.WriteBoolean(joinStatus);
            info.buffer = brw.EndWrite();
            info.readByte = info.buffer.Length;
            info.socket = null;
            Game.Instance.Call(info);
        }
    }
}
