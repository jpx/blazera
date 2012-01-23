using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using System.Threading;

namespace Blazera
{
    public class Game
    {
        public Game()
        {
            Game.IsRunning = true;
            Init();
        }

        private void Init()
        {
            GameEngine.Instance.Init();
            GraphicsEngine.Instance.Init(ScreenType.LoadingScreen);
            GameTime.Instance.Init();
        }

        public void Launch()
        {
            while (Game.IsRunning)
            {
                GameTime.Update();

                Game.IsRunning = GraphicsEngine.Instance.Update(GameTime.Dt);
            }

            if (GameSession.Instance.IsOnline())
            {
                SendDeco();
            }
        }

        private void SendDeco()
        {
            GameSession.Instance.Deco();
            SendingPacket packet = new SendingPacket(PacketType.CLIENT_INFO_DECONNECTION);
            packet.AddString(GameSession.Instance.GetLogin());
            GameSession.Instance.SendPacket(packet);

            Log.Clear();
            Log.Cl("You are now disconnected", ConsoleColor.Red);
            Thread.Sleep(1000);
            Log.Cl("Good bye...", ConsoleColor.White);
        }

        public static Boolean IsRunning
        {
            get;
            set;
        }
    }
}
