using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;

namespace BlazeraEditor
{
    public class MiscWidget : WindowedWidget
    {
        #region Singleton

        private static MiscWidget _instance;
        public static MiscWidget Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new MiscWidget();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        Label Fps { get; set; }
        PTimer ChangeFps;
        Label MediumFpsLabel = new Label();

        Label Time = new Label(null, Label.ESize.Large);
        PTimer ChangeTime;

        UInt32 MediumFps = 0;
        UInt32 TotalFps = 0;
        UInt32 FpsCount = 0;

        Label EventCountLabel = new Label();
        Label EventHandledLabel = new Label();

        Label LoopCount = new Label();
        Label TexturePosCountLabel = new Label();

        Label MapType = new Label();
        Label ObjectCount = new Label();

        private MiscWidget() :
            base("Misc")
        {
            // fps
            Fps = new Label("Fps");
            Fps.SetSize(Label.ESize.Large);
            Fps.Style = Text.Styles.Bold;
            AddItem(Fps, 0, HAlignment.Center);
            ChangeFps = new PTimer(.5D, UpdateFps);

            AddItem(MediumFpsLabel);

            AddItem(Time);
            ChangeTime = new PTimer(.1D, UpdateTime);

            //AddItem(EventCountLabel);
            //AddItem(EventHandledLabel);

            //AddItem(LoopCount);
            //AddItem(TexturePosCountLabel);

            AddItem(MapType);
            AddItem(ObjectCount);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            ChangeFps.Update(dt);
            ChangeTime.Update(dt);
        }

        const UInt32 FPSCOUNT_MEDIUM_LIMIT = 6;
        void UpdateFps()
        {
            ++FpsCount;
            
            UInt32 fps = (UInt32)(1F / (this.Root.Window.GetFrameTime() / 1000D));

            TotalFps += fps;

            MediumFps = (UInt32)((double)TotalFps / (double)FpsCount);

            if (FpsCount < FPSCOUNT_MEDIUM_LIMIT)
                MediumFps = fps;

            MediumFpsLabel.Text = "Medium FPS : " + MediumFps;

            if (fps < 20)
                Fps.Color = Color.Black;
            else if (fps < 50)
                this.Fps.Color = Color.Red;
            
            else
                this.Fps.Color = Color.Green;

            this.Fps.Text = "FPS : " + fps.ToString();

            EventCountLabel.Text = "Event count : " + GameScoring.EventCount;
            EventHandledLabel.Text = "Event handled : " + GameScoring.EventHandled;

            MapType.Text = "Current map : " + MapMan.GetCurrent().Type;
            ObjectCount.Text = "Object count : " + MapMan.GetCurrent().GetObjectCount();
        }

        void UpdateTime()
        {
            Time.Text = "Time : " + (Int32)GameTime.GetSessionTime().Value;
        }
    }
}