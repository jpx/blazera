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
            this.Type = ScreenType.LoadingScreen;
        }

        public override void Init()
        {
            
        }

        public override ScreenType Run(Time dt)
        {
            this.NextScreen = base.Run(dt);

            this.NextScreen = ScreenType.LoginScreen;

            return this.NextScreen;
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
                        this.NextScreen = ScreenType.LoginScreen;

                        return true;
                    }

                    break;
            }

            return false;
        }
    }
}
