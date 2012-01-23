using System;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Particle
    {
        #region Members

        Texture Texture;

        public bool IsActive { get; private set; }

        float Gravity;
        float GravityPoint;

        float Velocity;

        float Acceleration;

        Time DurationTime;
        Timer Timer;

        Vector2f Position;

        Vector2f Scale;
        Vector2f ScaleVariation;
        Vector2f BaseDimension;

        float Angle; // internal angle
        float Rotation;

        float Direction; // angle of move
        float AngularVelocity;

        Color Color;
        float AlphaLimit;
        float CurrentAlpha;

        float Mass;

        #endregion

        public Particle()
        {

        }

        public void Init(
            Texture texture,
            Vector2f position,
            float velocity,
            float acceleration,
            double durationTime,
            Vector2f scale,
            Vector2f scaleVariation,
            float angle,
            float rotation,
            float direction,
            float angularVelocity,
            Color color,
            float alphaLimit,
            float gravity,
            float gravityPoint,
            float mass)
        {
            IsActive = true;

            Texture = new Texture(texture);

            Position = position;
            Velocity = velocity;
            Acceleration = acceleration;
            DurationTime = new Time(durationTime);

            Scale = scale;
            ScaleVariation = scaleVariation;

            Angle = angle;
            Rotation = rotation;

            Direction = direction;
            AngularVelocity = angularVelocity;

            Color = color;

            AlphaLimit = alphaLimit;
            CurrentAlpha = 255F;

            Gravity = gravity;
            GravityPoint = gravityPoint;
            Mass = mass;

            Timer = new Timer();
            Timer.Reset();

            InitTexture();
        }

        public void Update(Time dt)
        {
            if (Timer.IsDelayCompleted(DurationTime.Value))
            {
                IsActive = false;
                return;
            }

            if (Timer.GetElapsedTime().Value > AlphaLimit * DurationTime.Value)
            {
                CurrentAlpha -= ((float)dt.Value * 255F) / ((float)DurationTime.Value * (1f - AlphaLimit));
                if (CurrentAlpha < 0F)
                    CurrentAlpha = 0F;
                Color = new Color(Color.R,
                                  Color.G,
                                  Color.B,
                                  (byte)CurrentAlpha);
            }

            Position += new Vector2f(Velocity * (float)Math.Cos(Direction * Math.PI / 180),
                                     Velocity * (float)Math.Sin(Direction * Math.PI / 180)
                                     + (0.5F * Mass * Gravity * (GravityPoint - Position.Y) / 100F)) * (float)dt.Value;

            Velocity += Acceleration * (float)dt.Value;
            if (Velocity < 0)
                Velocity = 0;

            Angle += Rotation * (float)dt.Value;
            Direction += AngularVelocity * (float)dt.Value;

            Scale += ScaleVariation * (float)dt.Value;

            UpdateTexture();
        }

        public void Draw(RenderTarget window)
        {
            Texture.Draw(window);
        }

        void InitTexture()
        {
            if (Texture == null)
                return;

            Texture.Sprite.Origin = Texture.Dimension / 2F;

            BaseDimension = new Vector2f(Texture.Dimension.X, Texture.Dimension.Y);
            UpdateTexture();
        }

        void UpdateTexture()
        {
            Texture.Dimension = new Vector2f(
                BaseDimension.X * Scale.X,
                BaseDimension.Y * Scale.Y);
            Texture.Position = Position;
            Texture.Sprite.Rotation = Angle;
            Texture.Color = Color;
        }
    }
}
