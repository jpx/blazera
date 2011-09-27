using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class ParticleEffect
    {
        public ParticleEffect()
        {
            
        }

        public void InitParticles()
        {
            this.Particles = new Particle[this.Quantity.Y];

            for (int i = 0; i < RandomHelper.Get(this.Quantity.X, this.Quantity.Y); ++i)
            {
                this.Particles[i] = this.InitParticle();
            }
        }

        public void Update(Time dt)
        {
            for (int i = 0; i < this.Particles.Length; ++i)
            {
                if (this.Particles[i].IsActive)
                { 
                    this.Particles[i].Update(dt);
                }
                else
                {
                    this.Particles[i] = this.InitParticle();
                }
            }
        }

        public void Draw(RenderWindow window)
        {
            for (int i = 0; i < this.Particles.Length; ++i)
            {
                if (this.Particles[i].IsActive)
                {
                    this.Particles[i].Draw(window);
                }
            }
        }

        private Particle InitParticle()
        {
            Particle tmp = new Particle(
                this.Texture,
                this.Position,
                RandomHelper.Get(this.Velocity.X, this.Velocity.Y),
                RandomHelper.Get(this.Acceleration.X, this.Acceleration.Y),
                RandomHelper.Get(this.DurationTime.X, this.DurationTime.Y),
                new Vector2(RandomHelper.Get(this.MinScale.X, this.MaxScale.X), RandomHelper.Get(this.MinScale.Y, this.MaxScale.Y)),
                RandomHelper.Get(this.Angle.X, this.Angle.Y),
                RandomHelper.Get(this.Rotation.X, this.Rotation.Y),
                this.Color,
                new Color(RandomHelper.Get(this.MinDColor.R, this.MaxDColor.R),
                          RandomHelper.Get(this.MinDColor.G, this.MaxDColor.G),
                          RandomHelper.Get(this.MinDColor.B, this.MaxDColor.B)),
                RandomHelper.Get(this.AlphaLimit.X, this.AlphaLimit.Y));

            return tmp;
        }

        private Particle[] Particles
        {
            get;
            set;
        }

        public Vector2 Position
        {
            get;
            set;
        }

        public Texture Texture
        {
            get;
            set;
        }

        public Color Color
        {
            get;
            set;
        }

        public Color MinDColor
        {
            get;
            set;
        }

        public Color MaxDColor
        {
            get;
            set;
        }

        public Vector2I Quantity
        {
            get;
            set;
        }

        public Vector2 Velocity
        {
            get;
            set;
        }

        public Vector2 Acceleration
        {
            get;
            set;
        }

        public Vector2 DurationTime
        {
            get;
            set;
        }

        public Vector2 MinScale
        {
            get;
            set;
        }

        public Vector2 MaxScale
        {
            get;
            set;
        }

        public Vector2 AlphaLimit
        {
            get;
            set;
        }

        public Vector2 Angle
        {
            get;
            set;
        }

        public Vector2 Rotation
        {
            get;
            set;
        }
    }
}
