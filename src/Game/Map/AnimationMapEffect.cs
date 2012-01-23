using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class AnimationMapEffect : MapEffect
    {
        #region Members

        public Animation Effect { get; private set; }
        public string SoundName { get; private set; }

        #endregion

        public AnimationMapEffect() :
            base()
        {
            DrawOrder = BlazeraLib.DrawOrder.Normal;

            OnMove += new MoveEventHandler(AnimationMapEffect_OnMove);
        }

        void AnimationMapEffect_OnMove(IDrawable sender, MoveEventArgs e)
        {
            if (Effect == null)
                return;

            Effect.Position += e.Move;
            Effect.Z += e.ZOffset;
        }

        public AnimationMapEffect(AnimationMapEffect copy) :
            base(copy)
        {
            Effect = new Animation(copy.Effect);
            SoundName = copy.SoundName;

            OnMove += new MoveEventHandler(AnimationMapEffect_OnMove);
        }

        public override object Clone()
        {
            return new AnimationMapEffect(this);
        }

        public override void Update(Time dt)
        {
            Effect.Update(dt);
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            Effect.Draw(window);
        }

        public override void Start()
        {
            base.Start();

            Effect.OnStopping += new AnimationEventHandler(Effect_OnStopping);

            Effect.Play(false);
           // SoundManager.Instance.PlaySound(SoundName);
        }

        public void Init(string animationType, string soundName)
        {
            Effect = Create.Animation(animationType);

            SoundName = soundName;
        }

        void Effect_OnStopping(Animation sender, AnimationEventArgs e)
        {
            Effect.OnStopping -= new AnimationEventHandler(Effect_OnStopping);
            CallOnStopping();
        }

        public override Vector2f Position
        {
            get { return Effect.Position; }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Effect == null)
                    return base.Dimension;

                return Effect.Dimension;
            }
        }

        public override Color Color
        {
            set
            {
                base.Color = value;

                if (Effect != null)
                    Effect.Color = Color;
            }
        }
    }
}
