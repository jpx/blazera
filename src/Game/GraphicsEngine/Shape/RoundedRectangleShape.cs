using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class RoundedRectangleShape
    {
        const UInt32 BASE_POINT_COUNT = 5;
        const Byte BACKGROUND_COLOR_ALPHA = 192;

        const float DEFAULT_SHADOW_OFFSET = 2F;

        static readonly Color EFFECT_BEGIN_COLOR = Color.White;
        static readonly Color EFFECT_END_COLOR = Color.Black;
        static readonly Color SHADOW_EFFECT_COLOR = new Color(0, 0, 0, 128);

        const float DIFFERENCE_RADIUS_SIZE_LIMIT = 10F;
        const float MIN_SIZE = 20F;

        Shape Background;
        Shape Effect;
        Shape ShadowEffect;

        Vector2f Position;
        Vector2f Dimension;

        float Radius;
        float OutlineThickness;

        Color BackgroundColor;
        Color OutlineColor;

        Boolean ShadowMode;
        Vector2f ShadowOffset;

        public RoundedRectangleShape(
            Vector2f dimension,
            float radius,
            float outlineThickness,
            Color backgroundColor,
            Color outlineColor,
            Boolean shadowMode = false,
            float shadowOffset = DEFAULT_SHADOW_OFFSET)
        {
            Background = new Shape();
            Background.EnableFill(true);
            Background.EnableOutline(true);

            Effect = new Shape();
            Effect.EnableFill(true);
            Effect.EnableOutline(true);

            ShadowEffect = new Shape();
            ShadowEffect.EnableFill(true);
            ShadowEffect.EnableOutline(true);

            ShadowMode = shadowMode;
            ShadowOffset = new Vector2f(shadowOffset, shadowOffset);

            Radius = radius;
            OutlineThickness = outlineThickness;

            Dimension = dimension;

            AdjustSize();

            BackgroundColor = backgroundColor;
            BackgroundColor.A = BACKGROUND_COLOR_ALPHA;
            OutlineColor = outlineColor;
            OutlineColor.A = BACKGROUND_COLOR_ALPHA;

            Background.OutlineThickness = OutlineThickness;
            Effect.OutlineThickness = OutlineThickness;
            ShadowEffect.OutlineThickness = OutlineThickness;

            Build();
        }

        void AdjustSize()
        {
            if (Dimension.X < MIN_SIZE)
                Dimension.X = MIN_SIZE;
            if (Dimension.Y < MIN_SIZE)
                Dimension.Y = MIN_SIZE;

            while (Dimension.X < Radius * 2F + DIFFERENCE_RADIUS_SIZE_LIMIT)
                --Radius;
            while (Dimension.Y < Radius * 2F + DIFFERENCE_RADIUS_SIZE_LIMIT)
                --Radius;
        }

        Vector2f GetCenter(CornerType cornerType)
        {
            Vector2f center = new Vector2f();

            switch (cornerType)
            {
                case CornerType.TopLeft: center = Position + new Vector2f(Radius, Radius); break;
                case CornerType.TopRight: center = new Vector2f(Position.X + Dimension.X - Radius, Position.Y + Radius); break;
                case CornerType.BottomRight: center = Position + Dimension - new Vector2f(Radius, Radius); break;
                case CornerType.BottomLeft: center = new Vector2f(Position.X + Radius, Position.Y + Dimension.Y - Radius); break;
            }

            return center;
        }

        void Build()
        {
            UInt32 pointCount = (UInt32)(BASE_POINT_COUNT * Radius);
            // top left
            for (UInt32 count = pointCount / 2; count < pointCount * .75; ++count)
            {
                float angle = count * 2 * (float)Math.PI / pointCount;
                Vector2f offset = new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2f center = GetCenter(CornerType.TopLeft);

                Background.AddPoint(center + offset * Radius, BackgroundColor, OutlineColor);
                Effect.AddPoint(center + offset * Radius, EFFECT_BEGIN_COLOR, EFFECT_END_COLOR);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }

            // top right
            for (UInt32 count = pointCount - pointCount / 4; count < pointCount; ++count)
            {
                float angle = count * 2 * (float)Math.PI / pointCount;
                Vector2f offset = new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2f center = GetCenter(CornerType.TopRight);

                Background.AddPoint(center + offset * Radius, BackgroundColor, OutlineColor);
                Effect.AddPoint(center + offset * Radius, EFFECT_BEGIN_COLOR, EFFECT_END_COLOR);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }
            // bottom right
            for (UInt32 count = 0; count < pointCount / 4; ++count)
            {
                float angle = count * 2 * (float)Math.PI / pointCount;
                Vector2f offset = new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2f center = GetCenter(CornerType.BottomRight);

                Background.AddPoint(center + offset * Radius, BackgroundColor, OutlineColor);
                Effect.AddPoint(center + offset * Radius, EFFECT_END_COLOR, EFFECT_END_COLOR);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }

            // bottom left
            for (UInt32 count = pointCount / 4; count < pointCount / 2; ++count)
            {
                float angle = count * 2 * (float)Math.PI / pointCount;
                Vector2f offset = new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2f center = GetCenter(CornerType.BottomLeft);

                Background.AddPoint(center + offset * Radius, BackgroundColor, OutlineColor);
                Effect.AddPoint(center + offset * Radius, EFFECT_END_COLOR, EFFECT_END_COLOR);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }
        }

        public void SetPosition(Vector2f position)
        {
            Position = Vector2I.FromVector2(position).ToVector2() + GetBasePosition();

            Background.Position = Position;
            Effect.Position = Position;
            ShadowEffect.Position = Position + ShadowOffset;
        }

        public void Draw(RenderWindow window)
        {
            if (ShadowMode)
                window.Draw(ShadowEffect);
            window.Draw(Effect);
            window.Draw(Background);
        }

        public Vector2f GetBackgroundDimension()
        {
            return Dimension + new Vector2f(OutlineThickness * 2F, OutlineThickness * 2F) + (ShadowMode ? ShadowOffset : new Vector2f());
        }

        public Vector2f GetBasePosition()
        {
            return new Vector2f(OutlineThickness, OutlineThickness);
        }
    }
}