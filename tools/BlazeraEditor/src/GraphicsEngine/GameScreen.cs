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
            Window = window;
            GuiView = new View(Window.GetView());
            GameGuiView = new View(Window.GetView());
            Gui = new EditorBaseWidget(Window, GuiView);
            Gui.Dimension = GuiView.Size;

            PlayerHdl.Instance.Init("");

            MapHandler.Instance.SetGameRoot(Window);
            MapMan.Instance.InitMap(GameData.INIT_MAP);

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

            Gui.AddKeyWindowBind(Keyboard.Key.M, MapMan.Instance);
            Gui.AddKeyWindowBind(Keyboard.Key.G, MiscWidget.Instance);
            Gui.AddKeyWindowBind(Keyboard.Key.O, ObjectMan.Instance);
            Gui.AddKeyWindowBind(Keyboard.Key.T, TextureMan.Instance);
            Gui.AddKeyWindowBind(Keyboard.Key.H, MapHandler.Instance);
            Gui.AddKeyWindowBind(Keyboard.Key.I, TileMan.Instance);
            Gui.AddKeyWindowBind(Keyboard.Key.L, TileSetMan.Instance);

            PlayerHdl.Vlad.ToScript();
        }

        public void Run(Time dt)
        {
            Window.SetView(GuiView);

            Gui.Update(dt);
            Gui.Draw(Window);

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
            Gui.Init();

            MiscWidget.Instance.BackgroundRight = Gui.Right;
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

                    if (evt.Key.Code == Keyboard.Key.Escape)
                    {
                        Window.Close();
                        return true;
                    }

                    break;
            
                case EventType.Resized:

                    Gui.Dimension = new Vector2f((float)Window.Width, (float)Window.Height);
                    GuiView.Reset(new SFML.Graphics.FloatRect(0F, 0F, (float)Window.Width, (float)Window.Height));
                    GuiView.Viewport = new SFML.Graphics.FloatRect(0F, 0F, 1F, 1F);

                    break;
            }

            return false;
        }
    }
}
