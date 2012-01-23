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
            Menu = new Menu();
            MenuItem i = new MenuItem("");
            //Menu.Background = new PictureBox(Create.Texture("MenuBg_Main"));
            //Menu.Background.Texture.SetAlpha(64);

            MenuItem mi1 = new MenuItem("New game");
            mi1.Validated += new ValidationEventHandler(mi1_validated);

            MenuItem mi2 = new MenuItem("Continue");
            mi2.Validated += new ValidationEventHandler(mi2_validated);

            MenuItem mi3 = new MenuItem("Option");
            mi3.Validated += new ValidationEventHandler(mi3_Validated);

            MenuItem mi4 = new MenuItem("Quit");
            mi4.Validated += new ValidationEventHandler(mi4_Validated);

            Menu.AddItem(mi1);
            Menu.AddItem(mi2);
            Menu.AddItem(mi3);
            Menu.AddItem(mi4);

            Gui.AddGameWidget(Menu);
        }

        void mi4_Validated(object sender, EventArgs e)
        {
            Log.Cl("Quit");
            Window.Close();
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
            NextScreen = ScreenType.GameScreen;
        }

        #region particle

        List<ParticleEffect> effects = new List<ParticleEffect>();
        private ParticleEffect InitParticle()
        {
            ParticleEffect pe = new ParticleEffect();
            pe.Texture = Create.Texture("Particle_drop");
            pe.Acceleration = new Vector2f(-60f, -60f);
            pe.AlphaLimit = new Vector2f(0.5f, 0.5f);
            pe.Angle = new Vector2f(0f, 360f);
            pe.MinColor = Color.Black;
            pe.MaxColor = Color.White;
            pe.DurationTime = new Vector2f(1f, 1.5f);
            pe.Position = new Vector2f(RandomHelper.Get(0F, Gui.Dimension.X), RandomHelper.Get(0F, Gui.Dimension.Y));
            pe.Rotation = new Vector2f(0, 0);
            pe.MinScale = new Vector2f(.03f, .03f);
            pe.MaxScale = new Vector2f(.03f, .03f);
            pe.Velocity = new Vector2f(10, 200);

            pe.Init();
            return pe;
        }

        #endregion

        public MainTitleScreen(RenderWindow window) :
            base(window)
        {
            Type = ScreenType.MainTitleScreen;

            base.Init();
           // Gui.AddGameWidget(imb);

            initMenu();

            StraightLineVariableData slvd = new StraightLineVariableData(200F, 100 * 4);
           // slvd.AddKeyData(.25F, 100F);
            slvd.AddKeyData(.5F, 300F);
           // slvd.AddKeyData(.75F, 100F);
            slvd.ComputeData();

            img = new Image(800, 600, new Color(0, 0, 0, 0));
            for (int i = 0; i < slvd.GetCount(); ++i)
            {
               // Log.Cl(slvd.GetData(i));
                img.SetPixel((uint)(100 + i / 4), (uint)slvd.GetData(i), Color.Red);
               // img.SetPixel((uint)(100 + i / 4), (uint)slvd.GetData(i) + 4, Color.Blue);
            }

            tex = new SFML.Graphics.Texture(img);
            spre = new Sprite(tex);

            imb.AddMessage("hoy hoy !");
            imb.OnStopping += new MessageBox.EventHandler(imb_OnStopping);
        }

        void imb_OnStopping(MessageBox sender, MessageBox.EventArgs e)
        {
           // Log.Cl("STOOOOOOOOOOOOOOOOOO");
           // Gui.RemoveGameWidget(imb);
        }
        InfoMessageBox imb = new InfoMessageBox();

        Image img;
        SFML.Graphics.Texture tex;
        Sprite spre;

        public override void Init(ScreenArgs args = null)
        {
            base.Init();
            Menu.Position = GuiView.Center - Menu.Dimension / 2;
            imb.Left = 350;
            imb.Top = 200;
        }

        public override ScreenType Run(Time dt)
        {
            foreach (ParticleEffect pe in effects)
            {
                pe.Update(dt);
                pe.Draw(Window);
            }

           // Window.Draw(spre);

            NextScreen = base.Run(dt);


           // NextScreen = ScreenType.GameScreen;

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

                    if (Inputs.IsGameInput(InputType.Back, evt))
                        Window.Close();

                    switch (evt.Key.Code)
                    {
                        case Keyboard.Key.Space:

                            effects.Add(InitParticle());

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
