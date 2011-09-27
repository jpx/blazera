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
            this.IsRunning = true;
        }

        public void Init()
        {
            this.Window = new RenderWindow(
                new VideoMode(GameDatas.WINDOW_WIDTH, GameDatas.WINDOW_HEIGHT),
                "BlazeraEditor",
                GameDatas.WINDOW_STYLE,
                new ContextSettings(24, 8, 0));

            //this.Window.SetFramerateLimit(150);
            Border.Init();
            WindowEvents.Instance.Init(this.Window);

            GameScreen = new GameScreen(Window);
            GameScreen.Init();
        }

        public Boolean Update(Time dt)
        {
            //this.FPS.Update(dt);

            /**
             * window process
             */
            this.Window.DispatchEvents(); // sends the events
            this.Window.Clear(); // clear the window
            
            GameScreen.Run(dt); // run the current screen

            this.Window.Display(); // display the window

            // if closed
            this.IsRunning = this.Window.IsOpened();
            return this.IsRunning;
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