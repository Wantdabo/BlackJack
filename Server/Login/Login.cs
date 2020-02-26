using Network;
using Network.Inneral;
using SimpleLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Login
{
    class Login
    {
        private static Login login;
        private UDPInfo gateInfo;
        private UDPInfo gameInfo;
        public static Login Instance
        {
            get
            {
                if (login == null)
                    login = new Login();

                return login;
            }
        }

        private Login()
        {
            gateInfo.ip = "127.0.0.1";
            gateInfo.port = 10000;

            gameInfo.ip = "127.0.0.1";
            gameInfo.port = 10002;
        }

        public void Start()
        {
            Log.Print("login start...", ConsoleColor.Green);
            InneralCall ic = null;
            ic = new InneralCall((_info) =>
            {
                // 登录不校验
                BufferRW brw = new BufferRW();
                brw.BeginRead(_info.buffer);
                int session = brw.ReadInt32();
                short proto = brw.ReadInt16();
                string loginName = brw.ReadString();
                brw.EndRead();
                Log.Print(loginName + "\tlogin success...", ConsoleColor.Blue);
                ic.Call(_info, gateInfo);
                ic.Call(_info, gameInfo);
            }, 10001);
            while(true)
                Console.ReadKey();
        }
    }
}
