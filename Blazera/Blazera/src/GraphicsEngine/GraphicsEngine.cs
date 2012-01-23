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
            IsRunning = true;
        }

        public void Init(ScreenType initScreen)
        {
            Window = new RenderWindow(new VideoMode(GameData.WINDOW_WIDTH, GameData.WINDOW_HEIGHT), "Blazera", GameData.WINDOW_STYLE);

           // Window.SetFramerateLimit(60);

            WindowEvents.Instance.Init(Window);
            Screens = new ScreenList();
            Screens += new LoadingScreen(Window);
            Screens += new LoginScreen(Window);
            Screens += new MainTitleScreen(Window);
            Screens += new GameScreen(Window);
            Screens += new BattleScreen(Window);
            FPS = new PTimer(0.2, PrintFPS);
            Screens.Current = initScreen;
            Screens.GetCurrent().Init();
        }

        public Boolean Update(Time dt)
        {
           // FPS.Update(dt);

            Inputs.Instance.UpdateState();

            Window.DispatchEvents();
            Window.Clear();

            Screens.Current = Screens.GetCurrent().Run(dt);

            Window.Display();

            IsRunning = Window.IsOpened();
            return IsRunning;
        }

        private void PrintFPS()
        {
            Log.Clear();
            Log.Cl(GameTime.GetTotalTime().Value, "Played time", ConsoleColor.Yellow);
            Log.Cl(1 / GameTime.Dt.Value, "FPS", ConsoleColor.Red);
            Log.Cl(Screens.GetCurrent().Type, "Screen", ConsoleColor.Magenta);
            Log.Cl(GameSession.Instance.IsOnline(), "Connected", ConsoleColor.Blue);
           /* if (GetCurrentMap() != null && PlayerHdl.Vlad != null)
            {
                Log.Cl(GameScreen.GetCurrentMap().Type, "Map", ConsoleColor.Green);
                Log.Cl(PlayerHdl.Vlad.Direction, "Direction", ConsoleColor.Red);
              //  Log.Cl(PlayerHdl.Vlad.State, "State", ConsoleColor.Cyan);
            }*/
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
            Screens = new Dictionary<ScreenType, Screen>();
            FirstInitDone = new List<ScreenType>();
        }

        public static ScreenList operator +(ScreenList screenList, Screen screen)
        {
            if (!screenList.Screens.ContainsKey(screen.Type))
            {
                screenList.Screens.Add(screen.Type, screen);
               // screen.Init();
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
            return Screens[Current];
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
                ScreenType old = Current;
                if (Screens.ContainsKey(value))
                {
                    ScreenArgs args = GetCurrent().GetArgs();

                    _current = value;

                    if (!FirstInitDone.Contains(Current))
                    {
                        GetCurrent().FirstInit();
                        FirstInitDone.Add(Current);
                    }

                    if (old != Current)
                    {
                        GetCurrent().Init(args);
                    }
                }
            }
        }
    }

    #endregion
}