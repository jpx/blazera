using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using BlazeraLib;

namespace BlazeraEditor
{
    public class GameScreen
    {
        RenderWindow Window;
        View GuiView;
        View GameGuiView;
        EditorBaseWidget Gui;

        public GameScreen(RenderWindow window)
        {
            this.Window = window;
            this.GuiView = new View(this.Window.GetView());
            GameGuiView = new View(Window.GetView());
            this.Gui = new EditorBaseWidget(this.Window, this.GuiView);
            this.Gui.Dimension = this.GuiView.Size;

            PlayerHdl.Instance.Init("");

            MapHandler.Instance.SetGameRoot(Window);
            MapMan.Instance.InitMap(GameDatas.INIT_MAP);

            Gui.AddWindow(MiscWidget.Instance, true);
            Gui.AddWindow(MapHandler.Instance, true);
            Gui.AddWindow(MapMan.Instance, true);
            Gui.AddWindow(PointCreator.Instance);
            Gui.AddWindow(WarpPointCreator.Instance);
            Gui.AddWindow(MapCreator.Instance);
            Gui.AddWindow(ObjectMan.Instance);
            Gui.AddWindow(ObjectCreator.Instance);
            Gui.AddWindow(TextureMan.Instance);
            Gui.AddWindow(TextureCreator.Instance);
            Gui.AddWindow(TextureRemover.Instance);
            Gui.AddWindow(InformationDialogBox.Instance);
            Gui.AddWindow(ConfirmationDialogBox.Instance);
            Gui.AddWindow(BoundingBoxCreator.Instance);
            Gui.AddWindow(TextureRectDrawer.Instance);
            Gui.AddWindow(TileMan.Instance);
            Gui.AddWindow(TileSetMan.Instance);
            Gui.AddWindow(TileSetCreator.Instance);
            Gui.AddWindow(TileCreator.Instance);
            Gui.AddWindow(EventCreator.Instance);
            Gui.AddWindow(ActionCreator.Instance);
            Gui.AddWindow(ConditionCreator.Instance);

            Gui.AddKeyWindowBind(KeyCode.M, MapMan.Instance);
            Gui.AddKeyWindowBind(KeyCode.G, MiscWidget.Instance);
            Gui.AddKeyWindowBind(KeyCode.O, ObjectMan.Instance);
            Gui.AddKeyWindowBind(KeyCode.T, TextureMan.Instance);
            Gui.AddKeyWindowBind(KeyCode.H, MapHandler.Instance);
            Gui.AddKeyWindowBind(KeyCode.I, TileMan.Instance);
            Gui.AddKeyWindowBind(KeyCode.L, TileSetMan.Instance);
        }

        public void Run(Time dt)
        {
            this.Window.SetView(this.GuiView);

            this.Gui.Update(dt);
            this.Gui.Draw(this.Window);

            while (WindowEvents.EventHappened())
            {
                BlzEvent evt = new BlzEvent(WindowEvents.GetEvent());

                GameScoring.EventCount++;

                if (HandleEvent(evt))
                {
                    GameScoring.EventHandled++;
                }
            }
        }

        public void Init()
        {
            this.Gui.Init();

            MiscWidget.Instance.BackgroundRight = this.Gui.Right;
            MapMan.Instance.Left = MapHandler.Instance.BackgroundRight;
        }

        public Boolean HandleEvent(BlzEvent evt)
        {
            if (Gui.HandleEvent(evt))
            {
                return true;
            }

            if (evt.IsHandled)
            {
                return true;
            }
            
            // Ici s'effectue la gestion d'events
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    if (evt.Key.Code == KeyCode.Escape)
                    {
                        this.Window.Close();
                        return true;
                    }

                    break;
            
                case EventType.Resized:

                    this.Gui.Dimension = new Vector2((float)this.Window.Width, (float)this.Window.Height);
                    this.GuiView.Reset(new SFML.Graphics.FloatRect(0F, 0F, (float)this.Window.Width, (float)this.Window.Height));
                    this.GuiView.Viewport = new SFML.Graphics.FloatRect(0F, 0F, 1F, 1F);

                    break;
            }

            return false;
        }
    }
}
