using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    #region BlazeraProgramEventHandling

    public class BlazeraProgramEventArgs : System.EventArgs
    {
        public object[] Args { get; private set; }

        public BlazeraProgramEventArgs(object[] args = null)
        {
            Args = args;
        }
    }

    public class BlazeraProgramOnEventEventArgs : BlazeraProgramEventArgs
    {
        public BlzEvent Event { get; private set; }

        public BlazeraProgramOnEventEventArgs(BlzEvent evt)
        {
            Event = evt;
        }
    }

    public delegate void BlazeraProgramEventHandler(BlazeraProgram sender, BlazeraProgramEventArgs e);
    public delegate void BlazeraProgramOnEventEventHandler(BlazeraProgram sender, BlazeraProgramOnEventEventArgs e);

    #endregion

    /// <summary>
    /// A blazera base program providing severals functionalities
    /// </summary>
    public class BlazeraProgram
    {
        #region Singleton

        static BlazeraProgram _instance;
        /// <summary>
        /// Unique instance of BlazeraProgram class
        /// </summary>
        public static BlazeraProgram Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new BlazeraProgram();
                return _instance;
            }
            set { _instance = value; }
        }

        /// <summary>
        /// Initializes BlazeraProgram's instance
        /// </summary>
        public void Init() { }

        #endregion

        #region Members

        RenderWindow Window;
        View GuiView;
        EditorBaseWidget Gui;

        bool IsRunning;

        #endregion

        #region Events

        /// <summary>
        /// Event performed on the end of initialization
        /// </summary>
        public event BlazeraProgramEventHandler OnInit;
        /// <summary>
        /// Event performed on program launching
        /// </summary>
        public event BlazeraProgramEventHandler OnLaunching;
        /// <summary>
        /// Event performed at each loop on the main loop
        /// </summary>
        public event BlazeraProgramEventHandler OnRunning;
        /// <summary>
        /// Event performed before program closing
        /// </summary>
        public event BlazeraProgramEventHandler OnClosing;
        /// <summary>
        /// Event performed when an inner event not handled by the Gui is raised
        /// </summary>
        public event BlazeraProgramOnEventEventHandler OnEvent;

        bool CallOnInit() { if (OnInit == null) return false; OnInit(this, new BlazeraProgramEventArgs()); return true; }
        bool CallOnLaunching() { if (OnLaunching == null) return false; OnLaunching(this, new BlazeraProgramEventArgs()); return true; }
        bool CallOnRunning() { if (OnRunning == null) return false; OnRunning(this, new BlazeraProgramEventArgs()); return true; }
        bool CallOnClosing() { if (OnClosing == null) return false; OnClosing(this, new BlazeraProgramEventArgs()); return true; }
        bool CallOnEvent(BlzEvent evt) { if (OnEvent == null) return false; OnEvent(this, new BlazeraProgramOnEventEventArgs(evt)); return true; }

        #endregion

        BlazeraProgram()
        {
            // GameEngine init
            ScriptEngine.Instance.Init("ProgramDatas");

            TextureManager.Instance.Init();
            SoundManager.Instance.Init();

            // GraphicsEngine init
            Window = new RenderWindow(new VideoMode(GameDatas.WINDOW_WIDTH, GameDatas.WINDOW_HEIGHT), "Blazera program", GameDatas.WINDOW_STYLE);
            Window.Closed += new System.EventHandler(Window_Closed);
            WindowEvents.Instance.Init(Window);

            GuiView = new View(Window.GetView());
            Gui = new EditorBaseWidget(Window, GuiView);
            Gui.Dimension = GuiView.Size;

            Border.Init();

            Gui.AddWindow(ConfirmationDialogBox.Instance);
            Gui.AddWindow(InformationDialogBox.Instance);

            CallOnInit();
        }

        void Window_Closed(object sender, System.EventArgs e)
        {
            Window.Close();
        }

        /// <summary>
        /// Starts the program and its main loop
        /// </summary>
        public void Launch()
        {
            IsRunning = true;

            CallOnLaunching();

            Gui.Init();

            GameTime.Instance.Init();

            while (IsRunning)
            {
                Window.DispatchEvents();
                Window.Clear();

                CallOnRunning();

                Gui.Update(/*GameTime.GetDt()*/new Time(Window.GetFrameTime() / 1000D));
                Gui.Draw(Window);

                while (WindowEvents.EventHappened())
                {
                    BlzEvent evt = new BlzEvent(WindowEvents.GetEvent());

                    if (evt.Type == EventType.KeyPressed || evt.Type == EventType.KeyReleased)
                        Inputs.Instance.UpdateState(evt);

                    if (!Gui.HandleEvent(evt))
                    {
                        CallOnEvent(evt);
                    }
                }

                Window.Display();

                IsRunning = Window.IsOpened();
            }

            CallOnClosing();
        }

        /// <summary>
        /// Adds a given widget to the main window 
        /// </summary>
        /// <param name="widget">Widget to add</param>
        public void AddWidget(Widget widget)
        {
            if (widget is WindowedWidget)
            {
                Gui.AddWindow((WindowedWidget)widget, true);

                return;
            }

            Gui.AddWidget(widget);
        }

        /// <summary>
        /// Removes the given widget from the main window
        /// </summary>
        /// <param name="widget">Widget to remove</param>
        /// <returns>If the deletion is successful</returns>
        public bool RemoveWidget(Widget widget)
        {
            if (widget is WindowedWidget)
                return RemoveWindow((WindowedWidget)widget);

            return Gui.RemoveWidget(widget);
        }

        /// <summary>
        /// Removes the given window from the main window
        /// </summary>
        /// <param name="window">Window to remove</param>
        /// <returns>If the deletion is successful</returns>
        public bool RemoveWindow(WindowedWidget window)
        {
            return Gui.RemoveWindow(window);
        }

        /// <summary>
        /// Adds the window widget to the main window and showes or hides it
        /// </summary>
        /// <param name="window">Window to add</param>
        /// <param name="opened">If the window is shown</param>
        public void AddWindow(WindowedWidget window, bool opened = false)
        {
            Gui.AddWindow(window, opened);
        }

        /// <summary>
        /// Adds the given window widget to the main window associated to a shortcut key
        /// </summary>
        /// <param name="window">Window to add</param>
        /// <param name="key">Shorcut key to show and hide the given window</param>
        public void AddWindow(WindowedWidget window, Keyboard.Key key)
        {
            Gui.AddWindow(window, true);
            Gui.AddKeyWindowBind(key, window);
        }

        /// <summary>
        /// Closes the main window and quits the program
        /// </summary>
        public void Close()
        {
            Window.Close();
        }
    }
}
