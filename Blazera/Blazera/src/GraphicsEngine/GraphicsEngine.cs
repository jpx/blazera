using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SFML.Graphics;
using SFML.Window;
using BlazeraLib;

namespace Blazera
{
    public enum JoyButton
    {
        Triangle = 0,
        Round = 1,
        Cross = 2,
        Square = 3,
        L2 = 4,
        R2 = 5,
        L1 = 6,
        R1 = 7,
        Select = 8,
        LStick = 9,
        RStick = 10,
        Start = 11
    }
    
    public class GraphicsEngine
    {
        private GraphicsEngine()
        {
            this.IsRunning = true;
        }

        public void Init(ScreenType initScreen)
        {
            this.Window = new RenderWindow(new VideoMode(GameDatas.WINDOW_WIDTH, GameDatas.WINDOW_HEIGHT), "Blazera", GameDatas.WINDOW_STYLE);

            //this.Window.SetFramerateLimit(150);

            WindowEvents.Instance.Init(this.Window);
            this.Screens = new ScreenList();
            this.Screens += new LoadingScreen(this.Window);
            this.Screens += new LoginScreen(this.Window);
            this.Screens += new MainTitleScreen(this.Window);
            this.Screens += new GameScreen(this.Window);
            this.Screens += new BattleScreen(this.Window);
            this.FPS = new PTimer(0.6, PrintFPS);
            this.Screens.Current = initScreen;
            this.Screens.GetCurrent().Init();
        }

        public Boolean Update(Time dt)
        {
           // this.FPS.Update(dt);

            this.Window.DispatchEvents();
            this.Window.Clear();
            
            this.Screens.Current = this.Screens.GetCurrent().Run(dt);

            this.Window.Display();

            this.IsRunning = this.Window.IsOpened();
            return this.IsRunning;
        }

        private void PrintFPS()
        {
            Log.Clear();
            Log.Cl(GameTime.GetTotalTime().Value, "Played time", ConsoleColor.Yellow);
            Log.Cl(1 / GameTime.Dt.Value, "FPS", ConsoleColor.Red);
            Log.Cl(1.0f / this.Window.GetFrameTime(), "trueFPS", ConsoleColor.Red);
            Log.Cl(Screens.GetCurrent().Type, "Screen", ConsoleColor.Magenta);
            Log.Cl(GameSession.Instance.IsOnline(), "Connected", ConsoleColor.Blue);
            if (GameScreen.GetCurrentMap() != null && PlayerHdl.Vlad != null)
            {
                Log.Cl(GameScreen.GetCurrentMap().Type, "Map", ConsoleColor.Green);
                //Log.Cl(PlayerHdl.Vlad.EBoundingBoxes.Count, "BEBB", ConsoleColor.DarkYellow);
            }
            Log.Cl("===================", ConsoleColor.Blue);
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

        private ScreenList Screens
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

    #region ScreenList

    public class ScreenList
    {
        public ScreenList()
        {
            this.Screens = new Dictionary<ScreenType, Screen>();
            this.FirstInitDone = new List<ScreenType>();
        }

        public static ScreenList operator +(ScreenList screenList, Screen screen)
        {
            if (!screenList.Screens.ContainsKey(screen.Type))
            {
                screenList.Screens.Add(screen.Type, screen);
                screen.Init();
            }
            return screenList;
        }

        public static ScreenList operator -(ScreenList screenList, ScreenType screenType)
        {
            screenList.Screens.Remove(screenType);
            return screenList;
        }

        public Screen GetCurrent()
        {
            return this.Screens[this.Current];
        }

        private Dictionary<ScreenType, Screen> Screens
        {
            get;
            set;
        }

        private List<ScreenType> FirstInitDone { get; set; }

        private ScreenType _current;
        public ScreenType Current
        {
            private get
            {
                return _current;
            }
            set
            {
                ScreenType old = this.Current;
                if (this.Screens.ContainsKey(value))
                {
                    _current = value;

                    if (!this.FirstInitDone.Contains(this.Current))
                    {
                        this.GetCurrent().FirstInit();
                        this.FirstInitDone.Add(this.Current);
                    }

                    if (old != this.Current)
                    {
                        this.GetCurrent().Init();
                    }
                }
            }
        }
    }

    #endregion
}