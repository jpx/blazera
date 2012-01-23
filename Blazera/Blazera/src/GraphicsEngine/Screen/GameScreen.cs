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

        ProgressBar pb = new ProgressBar(new Vector2f(200, 30));

        FpsLabel Fps = new FpsLabel();

        public GameScreen(RenderWindow window) :
            base(window)
        {
            Type = ScreenType.GameScreen;
            Maps = new MapList(this);
        }

        Platform w, w6, w0, w01;
        public override void FirstInit()
        {
            base.FirstInit();

            int mainH = 8;
            if (!GameSession.Instance.IsOnline())
            {
                string WALLT = "Wall_Test";
                Maps += Create.Map(GameData.INIT_MAP); // loading
                Maps.Current = GameData.INIT_MAP;
                PlayerHdl.Instance.Init(Create.Player("Vlad")); // loading
                PlayerHdl.Vlad.OnMapChange += new WorldObject.MapChangeEventHandler(Vlad_OnMapChange);
                PlayerHdl.Warp(GetCurrentMap());

                w = new Platform();
                w.SetBase(Create.Texture(WALLT), 25, 15, mainH);
                w.SetMap(GetCurrentMap(), 40, 270);

                w0 = new Platform();
                w0.SetBase(Create.Texture(WALLT), 5, 1, 2);
                w0.SetMap(GetCurrentMap(), w.Right, 270 + 5 * 32, mainH - 2);
                w01 = new Platform();
                w01.SetBase(Create.Texture(WALLT), 5, 1, 3);
                w01.SetMap(GetCurrentMap(), w.Right, 270 + 6 * 32, mainH - 3);

                w6 = new Platform();
                w6.SetBase(Create.Texture(WALLT), 3, 8, mainH);
                w6.SetMap(GetCurrentMap(), w0.Right, 270);

                Platform w2 = new Platform();
                w2.SetBase(Create.Texture(WALLT), 10, 4, 3);
                w2.SetMap(GetCurrentMap(), 400, 470, mainH);
                Platform w21 = new Platform();
                w21.SetBase(Create.Texture(WALLT), 5, 2, 2);
                w21.SetMap(GetCurrentMap(), 450, 520, mainH + 3);

                Platform w3 = new Platform();
                w3.SetBase(Create.Texture(WALLT), 20, 10, 2);
                w3.SetMap(GetCurrentMap(), 400, 800);

                Platform p0 = new Platform();
                p0.SetBase(Create.Texture("Wall_Test"), 1, 1, 1);
                p0.SetMap(GetCurrentMap(), 32, 96);
                Platform p01 = new Platform();
                p01.SetBase(Create.Texture("Wall_Test"), 1, 1, 1);
                p01.SetMap(GetCurrentMap(), 32, 128);
                Platform p02 = new Platform();
                p02.SetBase(Create.Texture("Wall_Test"), 1, 1, 1);
                p02.SetMap(GetCurrentMap(), 64, 96);
                Platform p03 = new Platform();
                p03.SetBase(Create.Texture("Wall_Test"), 1, 1, 1);
                p03.SetMap(GetCurrentMap(), 64, 128);
                Platform p04 = new Platform();
                p04.SetBase(Create.Texture("Wall_Test"), 1, 1, 1);
               // p04.SetMap(GetCurrentMap(), 32, 160);
                Platform p05 = new Platform();
                p05.SetBase(Create.Texture("Wall_Test"), 1, 1, 1);
               // p05.SetMap(GetCurrentMap(), 64, 160);
               // p0.MergeWith(new List<Wall>() { p01, p02, p03/*, p04, p05*/ });

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

            pb.Position = new Vector2f(200, 200);
            //  Gui.AddWidget(pb);
            pb.OnCompletion += new ProgressBar.CompletionEventHandler(pb_OnCompletion);



            NPC nt = Create.NPC("Vlad");
            nt.SetMap(GetCurrentMap(), 1200, 720);
            nt.AddMessage("Hello World !");
           // nt.AddMessage("haha...");

            EBoundingBox bb1 = new EBoundingBox(nt, EBoundingBoxType.Event, -60, -60, 60, 60);
            DelayedObjectEvent evt1 = new DelayedObjectEvent(3, true);
            evt1.AddAction(new DefaultAction((args) => { Log.Cl("3 sec done !"); }));
            bb1.AddEvent(evt1);
            nt.AddEventBoundingBox(bb1);

            EffectSkin ee = new EffectSkin();
            ee.Init(Create.Texture("Animation_Vlad_S_N"), new Vector2f(0F, -15F), new Vector2f(-.4F, .4F), -10F, new Vector2f(0F, 200F), new Vector2f(3F, 3F), 255F);
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.S, new EffectSkin(ee));
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.E, new EffectSkin(ee));
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.N, new EffectSkin(ee));
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.O, new EffectSkin(ee));
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.SE, new EffectSkin(ee));
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.SO, new EffectSkin(ee));
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.NE, new EffectSkin(ee));
            PlayerHdl.Vlad.AddSkin("Teleporting", Direction.NO, new EffectSkin(ee));

            Gui.AddGameWidget(Fps);

            GetCurrentMap().ActivateLightEngine();
            #region test light
            LightEffect le = new LightEffect();
            le.Init(64F, 255F, 64);
            le.Position = new Vector2f(150, 330);
            le.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.tan3).ToSFColor();
            le.Z = mainH;
           // GetCurrentMap().AddDynamicLightEffect(le);
            ler = le;
            le = new LightEffect();
            le.Init(5512F, 255F, 64);
            le.Position = new Vector2f(2400, 600);
            le.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.tan3).ToSFColor();
            GetCurrentMap().AddStaticLightEffect(le);
           // ler = le;
            le = new LightEffect();
            le.Init(128F, 255F, 64);
            le.Position = new Vector2f(170, 350);
            le.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.red4).ToSFColor();
           // GetCurrentMap().AddStaticLightEffect(le);


            LightEffect le3 = new LightEffect();
            le3.Init(256F, 255F, 64);
            le3.Position = new Vector2f(650, 110);
            le3.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.LightGray).ToSFColor();
           // GetCurrentMap().AddStaticLightEffect(le3);

            DirectionalLightEffect dle = new DirectionalLightEffect();
            dle.Init(1000F, 255F, 32);
            dle.Position = new Vector2f(100, 100);
            dle.Color = Color.White;
            dle.Angle = 45F;
            dle.Direction = 0F;
            // GetCurrentMap().AddStaticLightEffect(dle);
            DirectionalLightEffect dle2 = new DirectionalLightEffect();
            dle2.Init(1000F, 255F, 32);
            dle2.Position = new Vector2f(100, 100);
            dle2.Color = Color.Red;
            dle2.Angle = 45F;
            dle2.Direction = 180F;
            //  GetCurrentMap().AddStaticLightEffect(dle2);
            dler = dle;
            dler2 = dle2;

            byte g = 42;
            GetCurrentMap().SetGlobalColor(new Color(g, g, 160));
            evtTimer.Completed += new EventTimer.EventTimerEventHandler(evtTimer_Completed);
            #endregion

            GetCurrentMap().Init();

            PlayerHdl.Vlad.MoveTo(new Vector2f(300, 600), mainH);

            Door d1 = Create.Door("Test");
            d1.SetMap(GetCurrentMap(), 200, 723);

            Door td = Create.Door("Test");
            td.SetMap(GetCurrentMap(), 80, 520, mainH);
            d1.BindTo(td);
            td.BindTo(d1);

            td.TrySetState("Closed");
           // td.TrySetState("Locked");

           // Window.SetFramerateLimit(60);

            msgBox.AddMessage("Message numero 1 ...");
            msgBox.AddMessage("Message un peu plus long mais tout de meme numero 2 .....                                                                            on va faire encore plus long !");
            msgBox.AddMessage("... et enfin, le tant attendu message numero 3 !! ...");

            GetCurrentMap().AddWidget(msgBox);
            msgBox.Left = 50;
            msgBox.Top = 200;
           // msgBox.LaunchMessage();
            msgBox.OnStopping += new MessageBox.EventHandler(msgBox_OnStopping);

            GetCurrentMap().Accept(new PrintInfoMapVisitor());

            elt01 = Create.Element("DragonStatue");
            elt01.SetMap(GetCurrentMap(), 270, 500, mainH);
            ltemp = new LightEffect();
            ltemp.Init(64F);
            ltemp.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.tan3).ToSFColor();
           // elt01.AddLightEffect(ltemp);

            for (int i = 0; i < 3; ++i)
            {
                Element elt02 = Create.Element("Torch");
                elt02.SetMap(GetCurrentMap(), 410 + i * 120, 520, mainH);
                LightEffect ltemp2 = new LightEffect();
                ltemp2.Init(80F);
                ltemp2.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.White).ToSFColor();
                elt02.AddLightEffect(ltemp2, new Vector2f(0, 20));
            }

            //GameView.Reset(new SFML.Graphics.FloatRect(0, 0, 1600, 1200));
          //  GameView.Zoom(2);
            float fact = 1f;
            GameView.Viewport = new SFML.Graphics.FloatRect(0, 0, fact, fact);
        }
        Element elt01;
        LightEffect ltemp;

        void Vlad_OnMapChange(WorldObject sender, WorldObject.MapChangeEventArgs e)
        {
            ChangeMap(e.Map.Type);
        }

        void msgBox_OnStopping(MessageBox sender, MessageBox.EventArgs e)
        {
           // Gui.AsyncRemoveGameWidget(msgBox);
        }
        InfoMessageBox msgBox = new InfoMessageBox();

        void AL(Vector2f pos, float rad)
        {
            LightEffect le = new LightEffect();
            le.Init(rad, 255F, 64);
            le.Position = pos;
            le.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.tan3).ToSFColor();
            GetCurrentMap().AddStaticLightEffect(le);
        }

        void AL2(Vector2f pos)
        {
            DirectionalLightEffect le = new DirectionalLightEffect();
            le.Init(256F, 255F, 64);
            le.Angle = 45F;
            le.Direction = 90F;
            le.Position = pos;
            le.Color = BlazeraLib.Graphics.Color.GetColorFromName(BlazeraLib.Graphics.Color.ColorName.tan3).ToSFColor();
            GetCurrentMap().AddStaticLightEffect(le);
        }

        void evtTimer_Completed(EventTimer sender, EventTimer.EventTimerEventArgs e)
        {
            const int coef = 1;
            gc = new Color((byte)(gc.R + coef), (byte)(gc.G + coef), (byte)(gc.B + coef));
           // GetCurrentMap().SetGlobalColor(gc);
           // ler.Position += new Vector2f(5, 5);
            dler.Direction += .008F;
            dler2.Direction += .008F;
        }
        EventTimer evtTimer = new EventTimer(.01D, true);
        Color gc = new Color(0, 0, 0);
        LightEffect ler = null;
        DirectionalLightEffect dler = null;
        DirectionalLightEffect dler2 = null;

        void pb_OnCompletion(ProgressBar sender, ProgressBar.CompletionEventArgs e)
        {
            pb.Reset();
        }

        void CombatTestItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Args = new ScreenArgs(new Dictionary<string, object>() { { "Map", GetCurrentMap() } });
            NextScreen = ScreenType.BattleScreen;
        }

        void QuitItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Window.Close();
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

        public override void Init(ScreenArgs args = null)
        {
            Gui.Init();
            MainMenu.Center = Gui.Center;
            MainMenu.Top = Gui.Top;

        }

        public override ScreenType Run(Time dt)
        {
            Gui.MoveGameView(GetViewMove(dt));

            UpdateMap(dt);

            NextScreen = base.Run(dt);

            evtTimer.Update();

          //  NextScreen = ScreenType.BattleScreen;
           // pb.AddProgressValueOffset(0.01);
            return NextScreen;
        }

        //bool init = false;
        private void UpdateMap(Time dt)
        {
#if false
            if (PlayerHdl.Vlad != null && !init)
            {
                init = true;

                PlayerHeaderInfoPanel ip = new PlayerHeaderInfoPanel(PlayerHdl.Vlad);
                PlayerHeaderInfoPanelBox p = new PlayerHeaderInfoPanelBox();
                p.Build(new InfoPanelBox.BuildInfo(new Dictionary<string, object>() { { "Player", PlayerHdl.Vlad } }));
                ip.AddBox(p);
                Gui.AddGameWidget(ip);
            }
#endif

            if (GameSession.Instance.IsOnline())
            {
                CWorld.Instance.Update(dt);
                CWorld.Instance.Draw(Window);
                return;
            }

            GetCurrentMap().Update(dt);
            GetCurrentMap().Draw(Window);
        }

        public Map GetCurrentMap()
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
                case EventType.MouseMoved:

                  //  ler.Position = new Vector2f(evt.MouseMove.X, evt.MouseMove.Y) + GameView.Center - GameView.Size / 2F;

                  //  MapEffectManager.Instance.AddEffect(new ParticleMapEffect(), new Vector2f(evt.MouseMove.X, evt.MouseMove.Y), 4);
                   /* AnimationMapEffect e = new AnimationMapEffect();
                    e.Init("LightningTest", "LightningTest.wav");
                    MapEffectManager.Instance.AddEffect(e, new Vector2f(evt.MouseMove.X, evt.MouseMove.Y) + GameView.Center - GameView.Size / 2F, 4);*/

                    break;

                case EventType.MouseButtonPressed:

                   /* if (evt.MouseButton.Button == Mouse.Button.Left)
                    AL(new Vector2f(evt.MouseButton.X, evt.MouseButton.Y) + GameView.Center - GameView.Size / 2F);
                    if (evt.MouseButton.Button == Mouse.Button.Right)
                        AL2(new Vector2f(evt.MouseButton.X, evt.MouseButton.Y) + GameView.Center - GameView.Size / 2F);*/


                    return true;

                case EventType.KeyPressed:

                    if (evt.Key.Code == Keyboard.Key.Escape)
                        Window.Close();

                    if (Inputs.IsGameInput(InputType.Action, evt, true, 0D, false))
                    {
                        w.MergeWith(new List<Wall>() { w0, w6, w01 });
                        //elt01.RemoveLightEffect(ltemp);
                        //GetCurrentMap().RemoveObject(elt01);

                       // MapEffectManager.Instance.AddEffect(new TextMapEffect("ARGH!", Color.Black), new Vector2f(RandomHelper.Get(50F, 750F), RandomHelper.Get(50F, 550F)));
                       // MapEffectManager.Instance.AddEffect(new ParticleMapEffect(), new Vector2f(RandomHelper.Get(50F, 750F), RandomHelper.Get(50F, 550F)));
                       // msgBox.LaunchMessage();
                    }

                    if (Inputs.IsGameInput(InputType.Action2, evt, true) &&
                        !PlayerHdl.Vlad.IsActive())
                    {
                        MainMenu.Open(new OpeningInfo(true));
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Back, evt))
                    {
                        NextScreen = ScreenType.LoginScreen;
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
            Input input = Window.Input;
            if (Math.Abs(input.GetJoystickAxis(joyNb, JoyAxis.AxisX)) > GameData.JOYSTICK_RUN_SENSIBILITY ||
                Math.Abs(input.GetJoystickAxis(joyNb, JoyAxis.AxisY)) > GameData.JOYSTICK_RUN_SENSIBILITY)
            {
                PlayerHdl.Vlad.IsRunning = true;
            }
            else
            {
                PlayerHdl.Vlad.IsRunning = false;
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisX) > GameData.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.E);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.E);
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisX) < -GameData.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.O);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.O);
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisY) > GameData.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.S);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.S);
            }
            if (input.GetJoystickAxis(joyNb, JoyAxis.AxisY) < -GameData.JOYSTICK_WALK_SENSIBILITY)
            {
                PlayerHdl.Vlad.EnableDirection(Direction.N);
            }
            else
            {
                PlayerHdl.Vlad.DisableDirection(Direction.N);
            }*/
        }

        const float VIEW_MOVE_TRIGGER_LIMIT = 50F;
        private Vector2f GetViewMove(Time dt)
        {
            float moveX = 0.0f;
            float moveY = 0.0f;

            if (PlayerHdl.Vlad == null)
            {
                return new Vector2f(moveX, moveY);
            }

            Vector2f p = PlayerHdl.Vlad.DrawingPosition + PlayerHdl.Vlad.Halfsize;

            float xWR = (float)GameData.WINDOW_WIDTH / (float)GameData.WINDOW_HEIGHT;
            float yWR = (float)GameData.WINDOW_HEIGHT / (float)GameData.WINDOW_WIDTH;

            if (Math.Abs(p.X - GameView.Center.X) > VIEW_MOVE_TRIGGER_LIMIT * xWR)
                moveX = PlayerHdl.Vlad.Velocity * (p.X - GameView.Center.X) / 180f * yWR * (float)dt.Value;
            if (Math.Abs(p.Y - GameView.Center.Y) > VIEW_MOVE_TRIGGER_LIMIT * yWR)
                moveY = PlayerHdl.Vlad.Velocity * (p.Y - GameView.Center.Y) / 180f * xWR * (float)dt.Value;

            if (GameView.Center.X - GameView.Size.X / 2 + moveX < 0F)
            {
                GameView.Center = new Vector2f(0F, GameView.Center.Y) + new Vector2f(GameView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (GameView.Center.X - GameView.Size.X / 2 + GameView.Size.X + moveX >= GetCurrentMap().Dimension.X)
            {
                GameView.Center = new Vector2f(GetCurrentMap().Dimension.X, GameView.Center.Y) - new Vector2f(GameView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (GameView.Center.Y - GameView.Size.Y / 2 + moveY < 0F)
            {
                GameView.Center = new Vector2f(GameView.Center.X, 0F) + new Vector2f(0F, GameView.Size.Y / 2F);
                moveY = 0.0f;
            }

            if (GameView.Center.Y - GameView.Size.Y / 2 + GameView.Size.Y + moveY >= GetCurrentMap().Dimension.Y)
            {
                GameView.Center = new Vector2f(GameView.Center.X, GetCurrentMap().Dimension.Y) - new Vector2f(0F, GameView.Size.Y / 2F);
                moveY = 0.0f;
            }

            return new Vector2f(moveX, moveY);
        }

        public Map GetCurrent()
        {
            return Maps.GetCurrent();
        }

        public void ChangeMap(String mapName)
        {
            GetCurrentMap().InterruptEvents();
            GetCurrentMap().DisableAllMoves();

            if (!Maps.Contains(mapName))
            {
                CMap tmp = new CMap(Create.Map(mapName));
                Maps += tmp;
            }
            
            Maps.Current = mapName;
        }

        private MapList Maps
        {
            get;
            set;
        }
    }

    #region MapList

    public class MapList
    {
        GameScreen Parent;

        public MapList(GameScreen parent)
        {
            Parent = parent;
            Maps = new Dictionary<String, Map>();
        }

        public static MapList operator +(MapList mapList, Map map)
        {
            map.SetParent(mapList.Parent);
            map.InitGui();
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
            return Maps.ContainsKey(mapName);
        }

        public Map GetCurrent()
        {
            if (Current == null)
            {
                return null;
            }

            return Maps[Current];
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
                if (_current == value)
                    return;

                if (Maps.ContainsKey(value))
                {
                    _current = value;
                }
            }
        }
    }

    #endregion
}
