using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    #region Event handlers

    public class AnimationEventArgs : System.EventArgs
    {

    }

    public delegate void AnimationEventHandler(Animation sender, AnimationEventArgs e);

    #endregion

    public class Animation : DrawableBaseObject, IUpdateable
    {
        #region Constants

        const int DEFAULT_FRAME_RATE = 8;
        const int DEFAULT_STOP_FRAME = 0;

        const bool DEFAULT_LOOP_STATE = true;

        #endregion

        #region Members

        List<Texture> Frames;
        int CurrentFrame;

        public int StopFrame { get; set; }
        bool LoopState;
        public int FrameRate { get; set; }

        bool IsPlaying;

        Timer Timer;

        #endregion

        #region Events

        public event AnimationEventHandler OnPlaying;
        bool CallOnPlaying() { if (OnPlaying == null) return false; OnPlaying(this, new AnimationEventArgs()); return true; }

        public event AnimationEventHandler OnStopping;
        bool CallOnStopping() { if (OnStopping == null) return false; OnStopping(this, new AnimationEventArgs()); return true; }

        #endregion

        public Animation() :
            base()
        {
            Frames = new List<Texture>();
            StopFrame = DEFAULT_STOP_FRAME;
            CurrentFrame = StopFrame;

            FrameRate = DEFAULT_FRAME_RATE;

            LoopState = DEFAULT_LOOP_STATE;

            IsPlaying = false;

            Timer = new Timer();
            Timer.Start();
        }

        public Animation(Animation copy) :
            base(copy)
        {
            Frames = new List<Texture>();
            foreach (Texture frame in copy.Frames)
                AddFrame(new Texture(frame));

            StopFrame = copy.StopFrame;
            CurrentFrame = StopFrame;

            FrameRate = copy.FrameRate;

            LoopState = copy.LoopState;

            IsPlaying = false;

            Timer = new Timer();
            Timer.Start();
        }

        public override void ToScript()
        {
            Sw = new ScriptWriter(this);
            Sw.InitObject();
            base.ToScript();

            Sw.WriteProperty("BasePoint", "Vector2f(" + BasePoint.X.ToString() + ", " + BasePoint.Y.ToString() + ")");
            Sw.WriteProperty("ComparisonPointYType", "ComparisonPointYType." + ComparisonPointYType.ToString());

            Sw.WriteProperty("StopFrame", StopFrame.ToString());
            Sw.WriteProperty("FrameRate", FrameRate.ToString());

            foreach (Texture frame in Frames)
                Sw.WriteMethod("AddFrame", new string[] { "Create:Texture(" + ScriptWriter.GetStringOf(frame.Type) + ")" });

            Sw.EndObject();
        }

        public void AddFrame(Texture frame)
        {
            Frames.Add(frame);
        }

        public void Update(Time dt)
        {
            if (!IsPlaying)
                return;

            double period = 1D / FrameRate;

            if (!Timer.IsDelayCompleted(period))
                return;

            if (++CurrentFrame < Frames.Count)
                return;

            if (LoopState)
            {
                Stop();
                Play();
            }
            else
                Stop();
        }

        public override void Draw(RenderWindow window)
        {
            if (!IsVisible)
                return;

            GetCurrentFrame().Draw(window);
        }

        public void Play(bool loopState = true, bool resetTimer = true)
        {
            CallOnPlaying();

            IsPlaying = true;

            LoopState = loopState;

            if (resetTimer)
                Timer.Reset();
        }

        public void Stop(bool reset = true)
        {
            if (!LoopState)
                CallOnStopping();

            IsPlaying = false;

            if (reset)
                CurrentFrame = StopFrame;
        }

        Texture GetCurrentFrame()
        {
            return Frames[CurrentFrame];
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                foreach (Texture frame in Frames)
                    frame.Position = Position;
            }
        }

        public override Vector2f Dimension
        {
            get { return GetCurrentFrame().Dimension; }
        }
    }
}