using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    public class ParticleMapEffect : MapEffect
    {
        #region Members

        ParticleEffect Effect;

        #endregion

        public ParticleMapEffect()
            : base()
        {
            DrawOrder = DrawOrder.Normal;
        }

        public ParticleMapEffect(ParticleMapEffect copy)
            : base(copy)
        {
            DrawOrder = DrawOrder.Normal;

            Effect = new ParticleEffect(copy.Effect);
        }

        public override object Clone()
        {
            return new ParticleMapEffect(this);
        }

        public override void Update(Time dt)
        {
            Effect.Update(dt);
        }

        public override void Draw(RenderTarget window)
        {
            Effect.Draw(window);
        }

        public override void Start()
        {
            base.Start();

            Init();
        }

        void Init()
        {
            Effect = new ParticleEffect();

            Effect.OnEnding += new ParticleEffect.ParticleEffectEventHandler(Effect_OnEnding);

            Effect.Texture = Create.Texture("blurredbar");

            Effect.Position = Position;

            Effect.ParticleCount = 150;
            Effect.GenerationPeriod = .1D;
            Effect.GenerationCount = 20;

            Effect.Gravity = 0F;
            Effect.Mass = new Vector2f(10, 10);
            Effect.MinPosition = new Vector2f(-15F, -4F);
            Effect.MaxPosition = new Vector2f(15F, 4F);
            Effect.MinColor = Color.Green;
            Effect.MaxColor = Color.Green;
            Effect.Acceleration = new Vector2f(0f, 5f);
            Effect.AlphaLimit = new Vector2f(0.55f, .8f);
            Effect.Angle = new Vector2f(270f, 270f);
            Effect.DurationTime = new Vector2f(.5f, 1.2f);
            Effect.Rotation = new Vector2f(0, 0);
            Effect.MinScale = new Vector2f(.003f, .003f);
            Effect.MaxScale = new Vector2f(.02f, .02f);
            const float SV = .1F;
            Effect.MinScaleVariation = new Vector2f(SV, SV);
            Effect.MaxScaleVariation = new Vector2f(SV, SV);
            Effect.Velocity = new Vector2f(10F, 70F);

            Effect.Init();
        }

        void Effect_OnEnding(ParticleEffect sender, ParticleEffect.ParticleEffectEventArgs e)
        {
            Effect.OnEnding -= new ParticleEffect.ParticleEffectEventHandler(Effect_OnEnding);

            CallOnStopping();
        }
    }
}
