using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class ParticleEffect
    {
        public class ParticleEffectEventArgs : EventArgs { }
        public delegate void ParticleEffectEventHandler(ParticleEffect sender, ParticleEffectEventArgs e);

        public event ParticleEffectEventHandler OnStarting;
        public event ParticleEffectEventHandler OnEnding;
        bool CallOnStarting() { if (OnStarting == null) return false; OnStarting(this, new ParticleEffectEventArgs()); return true; }
        bool CallOnEnding() { if (OnEnding == null) return false; OnEnding(this, new ParticleEffectEventArgs()); return true; }

        #region Members

        List<Particle> Particles;
        Queue<Particle> ParticlesToRemove;
        Queue<Particle> ConsumedParticles;

        public Vector2f Position { get; set; }

        public Texture Texture { get; set; }

        public int ParticleCount { get; set; }
        public double GenerationPeriod { get; set; }

        public int GenerationCount { get; set; }

        int ConsumedParticleCount;
        int ElapsedGenerationCount;
        int GenerationParticleCount;
        double LifeTime;
        Timer GenerationTimer;
        Timer LifeTimer;

        public float Gravity { get; set; }
        public Vector2f Mass { get; set; }

        public Vector2f MinPosition { get; set; }
        public Vector2f MaxPosition { get; set; }
        public Color MinColor { get; set; }
        public Color MaxColor { get; set; }
        public Vector2f Velocity { get; set; }
        public Vector2f Acceleration { get; set; }
        public Vector2f DurationTime { get; set; }
        public Vector2f MinScale { get; set; }
        public Vector2f MaxScale { get; set; }
        public Vector2f MinScaleVariation { get; set; }
        public Vector2f MaxScaleVariation { get; set; }
        public Vector2f AlphaLimit { get; set; }
        public Vector2f Angle { get; set; }
        public Vector2f Rotation { get; set; }

        #endregion

        public ParticleEffect()
        {
            Particles = new List<Particle>();
            ParticlesToRemove = new Queue<Particle>();
            ConsumedParticles = new Queue<Particle>();

            LifeTimer = new Timer();
            GenerationTimer = new Timer();
        }

        public ParticleEffect(ParticleEffect copy)
            : this()
        {
            Position = copy.Position;

            Texture = copy.Texture;

            Gravity = copy.Gravity;
            Mass = copy.Mass;

            MinPosition = copy.MinPosition;
            MaxPosition = copy.MaxPosition;
            MinColor = copy.MinColor;
            MaxColor = copy.MaxColor;
            Velocity = copy.Velocity;
            Acceleration = copy.Acceleration;
            DurationTime = copy.DurationTime;
            MinScale = copy.MinScale;
            MaxScale = copy.MaxScale;
            MinScaleVariation = copy.MinScaleVariation;
            MaxScaleVariation = copy.MaxScaleVariation;
            AlphaLimit = copy.AlphaLimit;
            Angle = copy.Angle;
            Rotation = copy.Rotation;
        }

        public void Init()
        {
            if (IsCompleted())
            {
                CallOnEnding();
                return;
            }

            ElapsedGenerationCount = 0;
            ConsumedParticleCount = 0;

            LifeTime = (GenerationCount - 1) * GenerationPeriod + DurationTime.Y;
            GenerationParticleCount = ParticleCount / GenerationCount;

            CallOnStarting();

            InitGeneration();

            LifeTimer.Start();
            GenerationTimer.Start();
        }

        void InitGeneration()
        {
            ++ElapsedGenerationCount;

            int particleCount = ConsumedParticleCount + GenerationParticleCount > ParticleCount ?
                ParticleCount - ConsumedParticleCount : GenerationParticleCount;

            ConsumedParticleCount += particleCount;

            while (ConsumedParticles.Count > 0 && particleCount > 0)
            {
                Particles.Add(InitParticle(ConsumedParticles.Dequeue()));
                --particleCount;
            }

            for (int count = 0; count < particleCount; ++count)
                Particles.Add(InitParticle(new Particle()));
        }

        public void Update(Time dt)
        {
            if (IsCompleted())
            {
                CallOnEnding();
                return;
            }

            while (ParticlesToRemove.Count > 0)
                Particles.Remove(ParticlesToRemove.Dequeue());

            if (GenerationTimer.IsDelayCompleted(GenerationPeriod)
                && ElapsedGenerationCount < GenerationCount)
                InitGeneration();

            foreach (Particle particle in Particles)
            {
                if (particle.IsActive)
                    particle.Update(dt);
                else
                    RemoveParticle(particle);
            }
        }

        bool IsCompleted()
        {
            return ElapsedGenerationCount >= GenerationCount && LifeTimer.IsDelayCompleted(LifeTime);
        }

        public void Draw(RenderTarget window)
        {
            foreach (Particle particle in Particles)
                if (particle.IsActive)
                    particle.Draw(window);
        }

        Particle InitParticle(Particle particle)
        {
            particle.Init(
                Texture,
                Position + new Vector2f(RandomHelper.Get(MinPosition.X, MaxPosition.X), RandomHelper.Get(MinPosition.Y, MaxPosition.Y)),
                RandomHelper.Get(Velocity.X, Velocity.Y),
                RandomHelper.Get(Acceleration.X, Acceleration.Y),
                RandomHelper.Get(DurationTime.X, DurationTime.Y),
                new Vector2f(RandomHelper.Get(MinScale.X, MaxScale.X), RandomHelper.Get(MinScale.Y, MaxScale.Y)),
                new Vector2f(RandomHelper.Get(MinScaleVariation.X, MaxScaleVariation.X), RandomHelper.Get(MinScaleVariation.Y, MaxScaleVariation.Y)),
                RandomHelper.Get(Angle.X, Angle.Y),
                RandomHelper.Get(Rotation.X, Rotation.Y),
                RandomHelper.Get(Angle.X, Angle.Y),
                RandomHelper.Get(Rotation.X, Rotation.Y),
                new Color(
                    RandomHelper.Get(MinColor.R, MaxColor.R),
                    RandomHelper.Get(MinColor.G, MaxColor.G),
                    RandomHelper.Get(MinColor.B, MaxColor.B)),
                RandomHelper.Get(AlphaLimit.X, AlphaLimit.Y),
                Gravity,
                Position.Y,
                RandomHelper.Get(Mass.X, Mass.Y));

            return particle;
        }

        void RemoveParticle(Particle particle)
        {
            ParticlesToRemove.Enqueue(particle);
            ConsumedParticles.Enqueue(particle);
        }
    }
}
