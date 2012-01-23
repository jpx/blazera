using System;

using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    /// <summary>
    /// Displays a skin (texture, animation) with several effects (distortion, particle effect ...)
    /// </summary>
    public class EffectSkin : Skin
    {
        public class EffectSkinEventArgs : EventArgs { }
        public delegate void EffectSkinEventHandler(EffectSkin sender, EffectSkinEventArgs e);

        public event EffectSkinEventHandler OnStopping;
        bool CallOnStopping() { if (OnStopping == null) return false; OnStopping(this, new EffectSkinEventArgs()); return true; }

        #region Members

        Skin BaseSkin;
        Skin Skin;

        Vector2f MoveFactor;
        Vector2f ScaleFactor;
        float AlphaFactor;

        Vector2f OffsetLimit;
        Vector2f ScaleLimit;
        float AlphaLimit;

        Vector2f Offset;
        Vector2f Scale;
        float Alpha;

        bool IsStarted;

        #endregion Members

        public EffectSkin()
            : base()
        {
            IsStarted = false;
        }

        public EffectSkin(EffectSkin copy)
            : base(copy)
        {
            IsStarted = false;

            Init(copy.BaseSkin, copy.MoveFactor, copy.ScaleFactor, copy.AlphaFactor, copy.OffsetLimit, copy.ScaleLimit, copy.AlphaLimit);
        }

        public override object Clone()
        {
            return new EffectSkin(this);
        }

        public void SetBaseSkin(Skin skin)
        {
            BaseSkin = skin;
            Skin = (Skin)skin.Clone();
        }

        public override Texture GetTexture()
        {
            return Skin.GetTexture();
        }

        public void Init(Skin baseSkin, Vector2f moveFactor, Vector2f scaleFactor, float alphaFactor, Vector2f offsetLimit, Vector2f scaleLimit, float alphaLimit)
        {
            SetBaseSkin(baseSkin);

            MoveFactor = moveFactor;
            ScaleFactor = scaleFactor;
            AlphaFactor = alphaFactor;

            OffsetLimit = offsetLimit;
            ScaleLimit = scaleLimit;
            AlphaLimit = alphaLimit;

            Reset();
        }

        public override void Start()
        {
            base.Start();

            IsStarted = true;
        }

        public override void Stop()
        {
            base.Stop();

            CallOnStopping();

            Reset();
        }

        void Reset()
        {
            Offset = new Vector2f();
            Scale = new Vector2f();
            Alpha = 0F;

            Skin.Position = BaseSkin.Position;
            Skin.Dimension = BaseSkin.Dimension;
            Skin.SetAlpha((double)BaseSkin.Color.A / 255D * 100D);
        }

        public override void Update(Time dt)
        {
            if (!IsStarted)
                return;

            bool moveIsComplete = Math.Abs(Offset.X) >= OffsetLimit.X && Math.Abs(Offset.Y) >= OffsetLimit.Y;
            bool scaleIsComplete = Math.Abs(Scale.X) >= ScaleLimit.X && Math.Abs(Scale.Y) >= ScaleLimit.Y;
            bool alphaIsComplete = Math.Abs(Alpha) >= AlphaLimit;

            if (moveIsComplete && scaleIsComplete && alphaIsComplete)
            {
                Stop();
                return;
            }

            Skin.Update(dt);

            if (!moveIsComplete)
            {
                Offset += MoveFactor * (float)dt.Value;
                Skin.Position = Position + Offset;
            }
            if (!scaleIsComplete)
            {
                Scale += ScaleFactor * (float)dt.Value;
                Skin.Dimension = BaseSkin.Dimension + new Vector2f(BaseSkin.Dimension.X * Scale.X, BaseSkin.Dimension.Y * Scale.Y);
                Skin.BasePoint = Skin.Center;
            }
            if (!alphaIsComplete)
            {
                Alpha += AlphaFactor * (float)dt.Value;
                Skin.SetAlpha(BaseSkin.Color.A + Alpha);
            }
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            Skin.Draw(window);
        }

        public override Color Color
        {
            set
            {
                base.Color = value;

                if (Skin != null)
                    Skin.Color = Color;
            }
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                if (Skin != null)
                    Skin.Position = Position + Offset;
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Skin == null)
                    return base.Dimension;

                return Skin.Dimension;
            }
        }
    }
}
