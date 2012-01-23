using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using BlazeraLib;

namespace Blazera
{
    public class LoadingScreen : Screen
    {
        public LoadingScreen(RenderWindow window) :
            base(window)
        {
            Type = ScreenType.LoadingScreen;
        }

        public override void Init(ScreenArgs args = null)
        {
            
        }

        public override ScreenType Run(Time dt)
        {
            NextScreen = base.Run(dt);

            NextScreen = ScreenType.LoginScreen;

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

                    if (Inputs.IsGameInput(InputType.Action))
                    {
                        NextScreen = ScreenType.LoginScreen;

                        return true;
                    }

                    break;
            }

            return false;
        }
    }
}
