using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace Blazera
{
    public class LoginScreen : Screen
    {
        Menu Menu = new Menu(Alignment.Vertical, 20F);

        void item1_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Log.Cl("Offline");
            NextScreen = ScreenType.MainTitleScreen;
            GameSession.Instance.Init(false);
        }

        void item2_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Log.Cl("Online");
            NextScreen = ScreenType.GameScreen;
            GameSession.Instance.Init(true);
        }

        public LoginScreen(RenderWindow window) :
            base(window)
        {
            Type = ScreenType.LoginScreen;

            MenuItem item1 = new MenuItem("Offline", Label.ESize.GameMenuLarge);
            item1.Validated += new ValidationEventHandler(item1_Validated);

            MenuItem item2 = new MenuItem("Online", Label.ESize.GameMenuLarge);
            item2.Validated += new ValidationEventHandler(item2_Validated);

            Menu.AddItem(item1);
            Menu.AddItem(item2);

            Gui.AddGameWidget(Menu);
        }

        public override void Init(ScreenArgs args = null)
        {
            Gui.Init();

            Menu.Center = Gui.Center;
        }

        public override ScreenType Run(Time dt)
        {
            NextScreen = base.Run(dt);

          //  NextScreen = ScreenType.MainTitleScreen;
          //  GameSession.Instance.Init(true);

            return NextScreen;
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

                    switch (evt.Key.Code)
                    {
                        case Keyboard.Key.Escape:

                            Window.Close();

                            return true;
                    }

                    break;
            }

            return false;
        }
    }
}
