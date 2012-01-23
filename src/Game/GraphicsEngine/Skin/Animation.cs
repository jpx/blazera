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

    public class Animation : Skin
    {
        #region Constants

        const int DEFAULT_FRAME_RATE = 10;
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

        public override object Clone()
        {
            return new Animation(this);
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

        public override string ToScriptString()
        {
            return "Create:Animation ( " + ScriptWriter.GetStringOf(Type) + " )";
        }

        public void AddFrame(Texture frame)
        {
            Frames.Add(frame);
        }

        public override void Update(Time dt)
        {
            if (!IsPlaying)
                return;

            double period = 1D / FrameRate;

            if (!Timer.IsDelayCompleted(period))
                return;

            NextFrame();
        }

        void NextFrame()
        {
            if (++CurrentFrame < Frames.Count)
                return;

            if (LoopState)
                CurrentFrame %= Frames.Count;
            else
                Stop();
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            GetCurrentFrame().Draw(window);
        }

        public override Texture GetTexture()
        {
            return GetCurrentFrame();
        }

        public void Play(bool loopState = true, bool resetTimer = true)
        {
            CallOnPlaying();

            IsPlaying = true;

            LoopState = loopState;

            if (resetTimer)
                Timer.Reset();

            NextFrame();
        }

        public void Stop(bool reset = true)
        {
            CallOnStopping();

            IsPlaying = false;

            if (reset)
                CurrentFrame = StopFrame;
        }

        public override void Start()
        {
            Play();
        }

        public override void Stop()
        {
            Stop();    
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
            set
            {
                Vector2f factor = new Vector2f(
                    value.X / Dimension.X,
                    value.Y / Dimension.Y);

                base.Dimension = value;
                
                foreach (Texture frame in Frames)
                    frame.Dimension = new Vector2f(
                        frame.Dimension.X * factor.X,
                        frame.Dimension.Y * factor.Y);
            }
        }
    }
}