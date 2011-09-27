using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;
using BlazeraLib;

namespace Blazera
{
    public class MainTitleScreen : Screen
    {
        private void initMenu()
        {
            /*this.Menu = new Menu();
            this.Menu.ExtremumOffset = 80.0f;
            //this.Menu.Background = new PictureBox(Create.Texture("MenuBg_Main"));
            //this.Menu.Background.Texture.SetAlpha(64);
            this.Menu.Selector = new PictureBox(Create.Texture("MenuSl_Main"));

            this.Menu.Dimension = new Vector2(500.0f, 500.0f);
            this.Menu.Position = this.GuiView.Center - this.Menu.Dimension / 2;


            MenuItem mi1 = new MenuItem("New game", new Color(0, 64, 0));
            mi1.Validated += new EventHandler(mi1_validated);

            MenuItem mi2 = new MenuItem("Continue", Color.Green);
            mi2.Validated += new EventHandler(mi2_validated);

            MenuItem mi3 = new MenuItem("Option", new Color(0, 64, 0));
            mi3.Validated += new EventHandler(mi3_Validated);

            MenuItem mi4 = new MenuItem("Quit", Color.Green);
            mi4.Validated += new EventHandler(mi4_Validated);

            this.Menu.AddItem(mi1);
            this.Menu.AddItem(mi2);
            this.Menu.AddItem(mi3);
            this.Menu.AddItem(mi4);

            this.Gui.AddWidget(this.Menu);*/
        }

        void mi4_Validated(object sender, EventArgs e)
        {
            Log.Cl("Quit");
            this.Window.Close();
        }

        void mi3_Validated(object sender, EventArgs e)
        {
            Log.Cl("Option");
        }

        private void mi1_validated(object sender, EventArgs evt)
        {
            Log.Cl("New game");
        }

        private void mi2_validated(object sender, EventArgs evt)
        {
            Log.Cl("Continue");
            this.NextScreen = ScreenType.GameScreen;
        }

        #region particle

        private ParticleEffect pe = new ParticleEffect();
        private void InitParticle()
        {
            pe.Texture = Create.Texture("Particle_drop");
            pe.Acceleration = new Vector2(0f, 100f);
            pe.AlphaLimit = new Vector2(0.5f, 0.5f);
            pe.Angle = new Vector2(0f, 360f);
            pe.Color = Color.Blue;
            pe.MinDColor = new Color(0, 0, 0);
            pe.MaxDColor = new Color(0, 0, 0);
            pe.DurationTime = new Vector2(1f, 4f);
            pe.Position = this.GameView.Center;
            pe.Quantity = new Vector2I(1000, 1000);
            pe.Rotation = new Vector2(-90f, 90f);
            pe.MinScale = new Vector2(1f, 1f);
            pe.MaxScale = new Vector2(1f, 1f);

            pe.Velocity = new Vector2(100f, 100f);

            pe.InitParticles();
        }

        #endregion

        public MainTitleScreen(RenderWindow window) :
            base(window)
        {
            this.Type = ScreenType.MainTitleScreen;

            base.Init();

            this.initMenu();
            InitParticle();
        }

        public override void Init()
        {
            
        }

        public override ScreenType Run(Time dt)
        {
            //pe.Update(dt);
            //pe.Draw(this.Window);

            this.NextScreen = base.Run(dt);

            NextScreen = ScreenType.GameScreen;

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

                    switch (evt.Key.Code)
                    {
                        case KeyCode.Return:

                            this.NextScreen = ScreenType.GameScreen;


                            return true;
                    }

                    break;
            }

            return false;
        }

        public Menu Menu
        {
            get;
            set;
        }
    }
}
