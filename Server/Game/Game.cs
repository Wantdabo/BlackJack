using Network;
using Network.Inneral;
using SimpleLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public struct Player
    {
        public int session;
        public string name;
        public long money;
        public int roomID;
        public PlayerStatus status;
    }

    public enum PlayerStatus
    {
        Room,  // 房间中
        Hall  // 大厅中
    }

    class Game
    {
        private static Game game;
        private const int MAX_ROOM = 10;
        private const int MAX_ROOM_PLAYER = 5;
        private const int DEF_MONEY = 10000;
        private InneralCall inneralCall;
        private UDPInfo gateInfo;
        private List<Room> rooms;
        private Dictionary<short, SocketInfoCallback> infoCallbacks;
        private Dictionary<int, Player> players;

        public static Game Instance
        {
            get
            {
                if (game == null)
                    game = new Game();
                return game;
            }
        }

        private Game()
        {

            rooms = new List<Room>();
            for (int i = 0; i < MAX_ROOM; i++)
            {
                Room room = new Room();
                room.ROOM_ID = i + 1;
                rooms.Add(room);
            }

            gateInfo.ip = "127.0.0.1";
            gateInfo.port = 10000;
            infoCallbacks = new Dictionary<short, SocketInfoCallback>();
            players = new Dictionary<int, Player>();
            infoCallbacks.Add(10001, Game.Instance.LoginSucc);
            infoCallbacks.Add(10002, Hall.Instance.Response);
            infoCallbacks.Add(10003, Game.Instance.ResponseRoom);
        }

        public void Start()
        {
            Log.Print("game start...", ConsoleColor.Green);
            inneralCall = new InneralCall((_info) =>
            {
                BufferRW brw = new BufferRW();
                brw.BeginRead(_info.buffer);
                short proto = brw.ReadInt16();
                brw.EndRead();
                DisProto(_info, proto);
            }, 10002);

            while (true)
                Console.ReadKey();
        }

        private void LoginSucc(SocketInfo _info)
        {
            Player player;
            BufferRW brw = new BufferRW();
            brw.BeginRead(_info.buffer);
            brw.ReadInt16();
            player.session = brw.ReadInt32();
            player.name = brw.ReadString();
            brw.EndRead();
            player.money = DEF_MONEY;
            player.roomID = 0;
            player.status = PlayerStatus.Hall;
            players.Add(player.session, player);
        }

        public void DisProto(SocketInfo _info, short _proto)
        {
            if (infoCallbacks.ContainsKey(_proto))
                infoCallbacks[_proto].Invoke(_info);
        }

        public List<Player> GetPlayers(int _roomID)
        {
            return rooms[_roomID].GetPlayers();
        }

        public bool CheckPlayer(int _session)
        {
            return players.ContainsKey(_session);
        }

        public Player GetPlayer(int _session)
        {
            return players[_session];
        }

        public void ResponseRoom(SocketInfo _info)
        {
            BufferRW brw = new BufferRW();
            brw.BeginRead(_info.buffer);
            int proto = brw.ReadInt16();
            int session = brw.ReadInt32();
            short opt = brw.ReadInt16();
            short roomID = brw.ReadInt16();
            brw.EndRead();

            Room room = rooms[roomID];
            if (room == null)
                room.Response(_info);
        }

        public int GetPlayerCount(int _session, int _roomID)
        {
            int count = 0;
            foreach (Player player in rooms[_roomID].GetPlayers())
            {
                if (player.session != _session)
                    count++;
            }

            return count;
        }

        public bool JoinGame(int _roomID, Player _player)
        {
            return rooms[_roomID].AddPlayer(_player);
        }

        public List<Room> GetRooms()
        {
            return rooms;
        }

        public int ROOM_COUNT
        {
            get
            {
                return MAX_ROOM;
            }
        }

        public int ROOM_PLAYER_COUNT
        {
            get
            {
                return MAX_ROOM_PLAYER;
            }
        }

        public void Call(SocketInfo _info)
        {
            inneralCall.Call(_info, gateInfo);
        }
    }
}
