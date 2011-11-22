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
        }

        public AnimationMapEffect(AnimationMapEffect copy) :
            base(copy)
        {
            Effect = new Animation(copy.Effect);
            SoundName = copy.SoundName;
        }

        public override void Update(Time dt)
        {
            Effect.Update(dt);
        }

        public override void Draw(RenderWindow window)
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
            CallOnStopping();
        }

        public override void SetBasePosition(Vector2f basePosition)
        {
            base.SetBasePosition(basePosition);

            Position = BasePosition;
        }

        public override Vector2f Position
        {
            get
            {
                if (Effect == null)
                    return base.Position;

                return Effect.Position;
            }
            set
            {
                Vector2f offset = value - Position;

                base.Position = value;

                if (Effect == null)
                    return;

                Effect.Position += offset;
            }
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
    }
}
