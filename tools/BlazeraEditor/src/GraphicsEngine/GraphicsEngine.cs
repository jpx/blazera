using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using BlazeraLib;

namespace BlazeraEditor
{
    public class GraphicsEngine
    {
        GameScreen GameScreen;

        private GraphicsEngine()
        {
            IsRunning = true;
        }

        public void Init()
        {
            Window = new RenderWindow(
                new VideoMode(GameData.WINDOW_WIDTH, GameData.WINDOW_HEIGHT),
                "BlazeraEditor",
                GameData.WINDOW_STYLE,
                new ContextSettings(24, 8, 0));

            //Window.SetFramerateLimit(150);
            Border.Init();
            WindowEvents.Instance.Init(Window);

            GameScreen = new GameScreen(Window);
            GameScreen.Init();
        }

        public Boolean Update(Time dt)
        {
            //FPS.Update(dt);

            Window.DispatchEvents();
            Window.Clear();

            Time trueDt = new Time(Window.GetFrameTime() / 1000D);
            GameScreen.Run(trueDt);

            Window.Display();

            IsRunning = Window.IsOpened();
            return IsRunning;
        }

        private static GraphicsEngine _instance;
        public static GraphicsEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    GraphicsEngine.Instance = new GraphicsEngine();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private RenderWindow Window
        {
            get;
            set;
        }

        public Boolean IsRunning
        {
            get;
            set;
        }

        private PTimer FPS
        {
            get;
            set;
        }
    }
}