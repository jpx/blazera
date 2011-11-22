using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Particle
    {
        public Particle(
            Texture texture,
            Vector2f position,
            float velocity,
            float acceleration,
            double durationTime,
            Vector2f scale,
            float angle,
            float rotation,
            Color color,
            Color dColor,
            float alphaLimit)
        {
            this.IsActive = true;
            this.Texture = new Texture(texture);
            this.Position = position;
            this.Velocity = velocity;
            this.Acceleration = acceleration;
            this.DurationTime = new Time(durationTime);
            this.Texture.Dimension = new Vector2f(this.Texture.Dimension.X * scale.X,
                                                 this.Texture.Dimension.Y * scale.Y);
            this.Angle = angle;
            this.Rotation = rotation;
            this.Color = color;
            this.DColor = dColor;
            this.AlphaLimit = alphaLimit;

            this.Timer = new Timer();
            this.Timer.Reset();
        }

        public void Reset()
        {

        }

        public void Update(Time dt)
        {
            if (this.Timer.GetElapsedTime().Value > this.DurationTime.Value)
            {
                this.IsActive = false;
                return;
            }

            if (this.Timer.GetElapsedTime().Value > this.AlphaLimit * this.DurationTime.Value)
            {
                this.Color = new Color(this.Color.R,
                                       this.Color.G,
                                       this.Color.B,
                                       (byte)(this.Color.A - (byte)((float)dt.Value * 255 / (float)this.DurationTime.Value * (1f - this.AlphaLimit))));
            }

            this.Texture.Sprite.Color = new Color((byte)(this.Color.R + this.DColor.R),
                                                  (byte)(this.Color.G + this.DColor.G),
                                                  (byte)(this.Color.B + this.DColor.B),
                                                  this.Color.A);

            this.Position += new Vector2f(this.Velocity * (float)Math.Cos(this.Angle * Math.PI / 180),
                                         this.Velocity * (float)Math.Sin(this.Angle * Math.PI / 180)) * (float)dt.Value;

            this.Velocity += this.Acceleration * (float)dt.Value;

            this.Texture.Sprite.Rotation = this.Angle;
            this.Angle += this.Rotation *(float)dt.Value;
        }

        public void Draw(RenderWindow window)
        {
            this.Texture.Draw(window);
        }

        public Texture Texture
        {
            get;
            set;
        }

        public Boolean IsActive
        {
            get;
            set;
        }

        public float Velocity
        {
            get;
            set;
        }

        public float Acceleration
        {
            get;
            set;
        }

        public Time DurationTime
        {
            get;
            set;
        }

        public Timer Timer
        {
            get;
            set;
        }

        private Vector2f _position;
        public Vector2f Position
        {
            get
            {
                return _position;
            }
            set
            {
                _position = value;

                if (this.Texture != null)
                {
                    this.Texture.Position = this.Position - this.Texture.Dimension / 2;
                }
            }
        }

        public Vector2f Scale
        {
            get;
            set;
        }

        private float _angle;
        public float Angle
        {
            get
            {
                return _angle;
            }
            set
            {
                _angle = value;
                if (this.Texture != null)
                {
                    this.Texture.Sprite.Rotation = this.Angle;
                }
            }
        }

        public float Rotation
        {
            get;
            set;
        }

        public Color Color
        {
            get;
            set;
        }

        public float AlphaLimit
        {
            get;
            set;
        }

        public Color DColor
        {
            get;
            set;
        }
    }
}
