using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using BlazeraLib;

namespace Blazera
{
    public class GameScreen : Screen
    {
        Menu MainMenu = new Menu(Alignment.Horizontal, 15F, 15F);
        MenuItem StatutItem = new MenuItem("Status");
        MenuItem OptionItem = new MenuItem("Option");
        MenuItem QuitItem = new MenuItem("quit");
        MenuItem CombatTestItem = new MenuItem("TestCombat");

        public GameScreen(RenderWindow window) :
            base(window)
        {
            this.Type = ScreenType.GameScreen;
            GameScreen.Maps = new MapList();
        }

        public override void FirstInit()
        {
            base.FirstInit();

            if (!GameSession.Instance.IsOnline())
            {
                Log.Cldebug("LOADING MAP...");
                GameScreen.Maps += Create.Map(GameDatas.INIT_MAP); // loading
                Log.Cldebug("END LOADING MAP.");
                GameScreen.Maps.Current = GameDatas.INIT_MAP;
                PlayerHdl.Instance.Init(Create.Player("Vlad")); // loading
                PlayerHdl.Warp(GetCurrentMap());

                Wall w = new Wall();
                w.SetBase(Create.Texture("Wall_Test"));
                w.SetDimension(36, 1, 3);
                //w.SetMap(GetCurrentMap(), 0, 0);
            }

            Gui.AddGameWidget(MainMenu);

            MainMenu.Opened += new OpenEventHandler(MainMenu_Opened);
            MainMenu.Closed += new CloseEventHandler(MainMenu_Closed);
            MainMenu.SetLocation(GameWidget.ELocation.MidTop);

            MainMenu.AddItem(StatutItem);
            MainMenu.AddItem(OptionItem);
            QuitItem.Validated += new ValidationEventHandler(QuitItem_Validated);
            MainMenu.AddItem(QuitItem);
            CombatTestItem.Validated += new ValidationEventHandler(CombatTestItem_Validated);
            MainMenu.AddItem(CombatTestItem);
            MainMenu.Close();
        }

        void CombatTestItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            NextScreen = ScreenType.BattleScreen;
        }

        void QuitItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            this.Window.Close();
        }

        void MainMenu_Closed(Widget sender, CloseEventArgs e)
        {
            if (GetCurrentMap() != null)
                GetCurrentMap().SetPhysicsIsRunning();
        }

        void MainMenu_Opened(Widget sender, OpenEventArgs e)
        {
            if (GetCurrentMap() != null)
                GetCurrentMap().SetPhysicsIsRunning(false);
        }

        public override void Init()
        {
            MainMenu.Center = Gui.Center;
            MainMenu.Top = Gui.Top;
        }

        public override ScreenType Run(Time dt)
        {
            Gui.MoveGameView(GetViewMove(dt));

            UpdateMap(dt);

            Inputs.Instance.UpdateState();

            NextScreen = base.Run(dt);

          //  NextScreen = ScreenType.BattleScreen;

            return NextScreen;
        }

        private void UpdateMap(Time dt)
        {
            if (GameSession.Instance.IsOnline())
            {
                CWorld.Instance.Update(dt);
                CWorld.Instance.Draw(Window);
                return;
            }

            GameScreen.GetCurrentMap().Update(dt);
            GameScreen.GetCurrentMap().Draw(this.Window);
        }

        public static Map GetCurrentMap()
        {
            return GameSession.Instance.IsOnline() ? CWorld.Instance.GetCurrentMap() : GetCurrent();
        }

        public override bool HandleEvent(BlzEvent evt)
        {
            if (base.HandleEvent(evt))
            {
                return true;
            }

            // Ici s'effectue la gestion d'events
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    if (evt.Key.Code == Keyboard.Key.Escape)
                        Window.Close();

                    if (Inputs.IsGameInput(InputType.Action2, evt, true) &&
                        PlayerHdl.Vlad.State == State.Inactive)
                    {
                        MainMenu.Open(new OpeningInfo(true));
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Back, evt))
                    {
                        this.NextScreen = ScreenType.LoginScreen;
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Misc, evt))
                    {
                        PlayerHdl.Vlad.SetRunning();
                        return true;
                    }

                    if (GetCurrentMap() != null && !GetCurrentMap().PhysicsIsRunning())
                        return false;

                    if (Inputs.IsGameInput(InputType.Up, evt, true))
                    {
                        PlayerHdl.Vlad.EnableDirection(Direction.N);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Right, evt, true))
                    {
                        PlayerHdl.Vlad.EnableDirection(Direction.E);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Down, evt, true))
                    {
                        PlayerHdl.Vlad.EnableDirection(Direction.S);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Left, evt, true))
                    {
                        PlayerHdl.Vlad.EnableDirection(Direction.O);
                        return true;
                    }

                    break;

                case EventType.KeyReleased:

                    if (Inputs.IsGameInput(InputType.Misc, evt))
                    {
                        PlayerHdl.Vlad.SetRunning(false);
                        return true;
                    }

                    if (GetCurrentMap() != null && !GetCurrentMap().PhysicsIsRunning())
                        return false;

                    if (Inputs.IsGameInput(InputType.Up, evt))
                    {
                        PlayerHdl.Vlad.DisableDirection(Direction.N);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Right, evt))
                    {
                        PlayerHdl.Vlad.DisableDirection(Direction.E);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Down, evt))
                    {
                        PlayerHdl.Vlad.DisableDirection(Direction.S);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Left, evt))
                    {
                        PlayerHdl.Vlad.DisableDirection(Direction.O);
                        return true;
                    }

                    break;
            }

            return false;
        }

        private void SetDirectionFromInputs()
        {
            if (PlayerHdl.Vlad == null)
            {
                return;
            }

            /*uint joyNb = 1;
            Input input = this.Window.Input;
            if (Math.Abs(input.GetJoystickAxis(joyNb, JoyAxis.AxisX)) > GameDatas.JOYSTICK_RUN_SENSIBILITY ||
                Math.Abs(input.GetJoystickAxis(joyNb, JoyAxis.AxisY)) > GameDatas.JOYSTICK_RUN_SENSIBILITY)
            {
                PlayerHdl.Vlad.IsRunning = true;
            }
            else
            {
                PlayerHdl.Vlad.IsRunning = false;
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisX) > GameDatas.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.E);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.E);
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisX) < -GameDatas.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.O);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.O);
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisY) > GameDatas.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.S);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.S);
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisY) < -GameDatas.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.N);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.N);
            }*/
        }

        const float VIEW_MOVE_MINOR_LIMIT = .08F;
        const float VIEW_MOVE_TRIGGER_LIMIT = 20F;
        private Vector2f GetViewMove(Time dt)
        {
            float moveX = 0.0f;
            float moveY = 0.0f;

            if (PlayerHdl.Vlad == null)
            {
                return new Vector2f(moveX, moveY);
            }

            Vector2f p = PlayerHdl.Vlad.Center;

            if (Math.Abs(p.X - GameView.Center.X) > VIEW_MOVE_TRIGGER_LIMIT * (GameDatas.WINDOW_HEIGHT / GameDatas.WINDOW_WIDTH))
                moveX = PlayerHdl.Vlad.Velocity * (p.X - this.GameView.Center.X) / 50f * GameDatas.WINDOW_WIDTH / GameDatas.WINDOW_HEIGHT * (float)dt.Value;
            if (Math.Abs(p.Y - GameView.Center.Y) > VIEW_MOVE_TRIGGER_LIMIT * (GameDatas.WINDOW_WIDTH / GameDatas.WINDOW_HEIGHT))
                moveY = PlayerHdl.Vlad.Velocity * (p.Y - this.GameView.Center.Y) / 50f * GameDatas.WINDOW_HEIGHT / GameDatas.WINDOW_WIDTH * (float)dt.Value;

            /*if (Math.Abs(moveX) < VIEW_MOVE_MINOR_LIMIT)
                moveX = p.X - Gui.MapView.Center.X;

            if (Math.Abs(moveY) < VIEW_MOVE_MINOR_LIMIT)
                moveY = p.Y - Gui.MapView.Center.Y;*/

            if (this.GameView.Center.X - this.GameView.Size.X / 2 + moveX < 0F)
            {
                GameView.Center = new Vector2f(0F, GameView.Center.Y) + new Vector2f(GameView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (this.GameView.Center.X - this.GameView.Size.X / 2 + this.GameView.Size.X + moveX >= GameScreen.GetCurrentMap().Dimension.X)
            {
                GameView.Center = new Vector2f(GetCurrentMap().Dimension.X, GameView.Center.Y) - new Vector2f(GameView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (this.GameView.Center.Y - this.GameView.Size.Y / 2 + moveY < 0F)
            {
                GameView.Center = new Vector2f(GameView.Center.X, 0F) + new Vector2f(0F, GameView.Size.Y / 2F);
                moveY = 0.0f;
            }

            if (this.GameView.Center.Y - this.GameView.Size.Y / 2 + this.GameView.Size.Y + moveY >= GameScreen.GetCurrentMap().Dimension.Y)
            {
                GameView.Center = new Vector2f(GameView.Center.X, GetCurrentMap().Dimension.Y) - new Vector2f(0F, GameView.Size.Y / 2F);
                moveY = 0.0f;
            }

            return new Vector2f(moveX, moveY);
        }

        public static Map GetCurrent()
        {
            return GameScreen.Maps.GetCurrent();
        }

        public static void ChangeMap(String mapName)
        {
            GameScreen.GetCurrentMap().InterruptEvents();
            GameScreen.GetCurrentMap().DisableAllMoves();

            if (!GameScreen.Maps.Contains(mapName))
            {
                CMap tmp = new CMap(Create.Map(mapName));
                GameScreen.Maps += tmp;
            }
            
            GameScreen.Maps.Current = mapName;
        }

        private static MapList Maps
        {
            get;
            set;
        }
    }

    #region MapList

    public class MapList
    {
        public MapList()
        {
            this.Maps = new Dictionary<String, Map>();
        }

        public static MapList operator +(MapList mapList, Map map)
        {
            mapList.Maps.Add(map.Type, map);
            return mapList;
        }

        public static MapList operator -(MapList mapList, String mapName)
        {
            mapList.Maps.Remove(mapName);
            return mapList;
        }

        public Boolean Contains(String mapName)
        {
            return this.Maps.ContainsKey(mapName);
        }

        public Map GetCurrent()
        {
            if (this.Current == null)
            {
                return null;
            }

            return this.Maps[this.Current];
        }

        private Dictionary<String, Map> Maps
        {
            get;
            set;
        }

        private String _current;
        public String Current
        {
            private get
            {
                return _current;
            }
            set
            {
                if (this.Maps.ContainsKey(value))
                {
                    _current = value;
                }
            }
        }
    }

    #endregion
}
