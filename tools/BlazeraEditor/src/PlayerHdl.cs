using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;

namespace BlazeraEditor
{
    public class MainPlayer : Player
    {
    }

    public class PlayerHdl
    {
        private PlayerHdl()
        {

        }

        public void Init(String login)
        {
            MainP = Create.Player("Vlad");
            MainP.Name = "VLAD";
        }

        public void Draw(SFML.Graphics.RenderWindow window)
        {
            /*foreach (SFML.Graphics.Sprite spr in path)
            {
                window.Draw(spr);
            }*/
        }

        public static void Warp(String mapName, String warpPointName = null)
        {
            Vlad.SetMap(mapName, warpPointName);
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

        private Player MainP
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

        /*public List<SFML.Graphics.Sprite> path = new List<SFML.Graphics.Sprite>();
        public void InitP()
        {
            Map m = GameScreen.GetCurrentMap();
            int[,] b = m.GetWayPoints();
            for (int h = 0; h < m.Height; h++)
            {
                for (int w = 0; w < m.Width; w++)
                {
                    if (b[h, w] == -1)
                    {
                        SFML.Graphics.Sprite spr = new SFML.Graphics.Sprite(new SFML.Graphics.Image(32, 32, new SFML.Graphics.Color(255, 0, 0, 255)));
                        spr.Position = new SFML.Window.Vector2f(w*32, h*32);
                        path.Add(spr);
                    }
                }
            }

            foreach (SFML.Window.Vector2f v in MainP.MovesInfo.Path)
            {
                SFML.Graphics.Sprite spr = new SFML.Graphics.Sprite(new SFML.Graphics.Image(32, 32, new SFML.Graphics.Color(255, 255, 255, 64)));
                spr.Position = v;
                path.Add(spr);
                Log.Cl(v);
            }
        }*/
    }
}
