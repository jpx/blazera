using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    //!\\ TODO : derive from BaseDrawableShape, rearange BaseDrawableShape //!\\
    public class RoundedRectangleShape : BaseDrawable, IShape
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

        float Radius;
        float OutlineThickness;

        Color EffectBeginColor;
        Color EffectEndColor;
        Color BackgroundColor;
        Color OutlineColor;
        Color ShadowEffectColor;

        Boolean ShadowMode;
        Vector2f ShadowOffset;

        public RoundedRectangleShape(
            Vector2f dimension,
            float radius,
            float outlineThickness,
            Color backgroundColor,
            Color outlineColor,
            Boolean shadowMode = false,
            float shadowOffset = DEFAULT_SHADOW_OFFSET,
            bool alphaMode = false)
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

            EffectBeginColor = EFFECT_BEGIN_COLOR;
            EffectEndColor = EFFECT_END_COLOR;
            ShadowEffectColor = SHADOW_EFFECT_COLOR;

            BackgroundColor = backgroundColor;
            if (!alphaMode)
                BackgroundColor.A = BACKGROUND_COLOR_ALPHA;
            else
            {
                EffectBeginColor.A = BackgroundColor.A;
                EffectEndColor.A = BackgroundColor.A;
                ShadowEffectColor.A = BackgroundColor.A;
            }
            OutlineColor = outlineColor;
            if (!alphaMode)
                OutlineColor.A = BACKGROUND_COLOR_ALPHA;
            else
                OutlineColor.A = BackgroundColor.A;

            Background.OutlineThickness = OutlineThickness;
            Effect.OutlineThickness = OutlineThickness;
            ShadowEffect.OutlineThickness = OutlineThickness;

            Build();
        }

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        void AdjustSize()
        {
            Dimension = new Vector2f(
                Math.Max(Dimension.X, MIN_SIZE),
                Math.Max(Dimension.Y, MIN_SIZE));

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
                Effect.AddPoint(center + offset * Radius, EffectBeginColor, EffectEndColor);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }

            // top right
            for (UInt32 count = pointCount - pointCount / 4; count < pointCount; ++count)
            {
                float angle = count * 2 * (float)Math.PI / pointCount;
                Vector2f offset = new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2f center = GetCenter(CornerType.TopRight);

                Background.AddPoint(center + offset * Radius, BackgroundColor, OutlineColor);
                Effect.AddPoint(center + offset * Radius, EffectBeginColor, EffectEndColor);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }
            // bottom right
            for (UInt32 count = 0; count < pointCount / 4; ++count)
            {
                float angle = count * 2 * (float)Math.PI / pointCount;
                Vector2f offset = new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2f center = GetCenter(CornerType.BottomRight);

                Background.AddPoint(center + offset * Radius, BackgroundColor, OutlineColor);
                Effect.AddPoint(center + offset * Radius, EffectEndColor, EffectEndColor);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }

            // bottom left
            for (UInt32 count = pointCount / 4; count < pointCount / 2; ++count)
            {
                float angle = count * 2 * (float)Math.PI / pointCount;
                Vector2f offset = new Vector2f((float)Math.Cos(angle), (float)Math.Sin(angle));

                Vector2f center = GetCenter(CornerType.BottomLeft);

                Background.AddPoint(center + offset * Radius, BackgroundColor, OutlineColor);
                Effect.AddPoint(center + offset * Radius, EffectEndColor, EffectEndColor);
                ShadowEffect.AddPoint(center + offset * Radius, SHADOW_EFFECT_COLOR, SHADOW_EFFECT_COLOR);
            }
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = Vector2I.FromVector2(value).ToVector2();

                Background.Position = Position + GetBasePosition();
                Effect.Position = Position + GetBasePosition();
                ShadowEffect.Position = Position + ShadowOffset + GetBasePosition();
            }
        }

        public override void Draw(RenderTarget window)
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