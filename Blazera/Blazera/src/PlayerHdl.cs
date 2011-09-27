using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace Blazera
{
    public class MainPlayer : CPlayer
    {
        public MainPlayer(Player player) :
            base(player)
        {
            OnDirectionEnablement += new DirectionEventHandler(MainPlayer_OnDirectionEnablement);
        }

        void MainPlayer_OnDirectionEnablement(WorldObject sender, DirectionEventArgs e)
        {
            
        }

        public override void SetMap(Map map, String warpPointName)
        {
            base.SetMap(map, warpPointName);

            if (GameSession.Instance.IsOnline())
            {
                CWorld.Instance.GetCurrentMap().AddDynamicObject(this, Position.X, Position.Y);
            }

            if (map.Type != GameScreen.GetCurrentMap().Type)
                GameScreen.ChangeMap(map.Type);
        }
    }

    public class PlayerHdl
    {
        private PlayerHdl()
        {

        }

        public void Init(Player player)
        {
            MainP = new MainPlayer(player);

            GameSession.Instance.SetPlayer(MainP);
        }

        public static void Warp(Map map, String warpPointName = null)
        {
            Vlad.SetMap(map, warpPointName);
        }

        private static PlayerHdl _instance;
        public static PlayerHdl Instance
        {
            get
            {
                if (_instance == null)
                {
                    PlayerHdl.Instance = new PlayerHdl();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private MainPlayer MainP
        {
            get;
            set;
        }

        public static Player Vlad
        {
            get { return PlayerHdl.Instance.MainP; }
        }

        public static void Cl(String str)
        {
            Log.Cl(str);
        }
    }
}
