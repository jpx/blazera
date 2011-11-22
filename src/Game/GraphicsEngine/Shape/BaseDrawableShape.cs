using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    #region Enums

    public enum ShapeEffect
    {
        Shadow,
        Shade
    }

    #endregion

    public abstract class BaseDrawableShape : BaseDrawable
    {
        #region Constants

        const byte BACKGROUND_COLOR_ALPHA = 192;

        const float DEFAULT_SHADOW_OFFSET = 4F;

        static readonly Color EFFECT_BEGIN_COLOR = Color.White;
        static readonly Color EFFECT_END_COLOR = Color.Black;
        static readonly Color SHADOW_EFFECT_COLOR = new Color(0, 0, 0, 160);

        #endregion

        #region Members

        Dictionary<ShapeEffect, Shape> Effects;

        Shape BaseShape;

        float OutlineThickness;
        Color OutlineColor;

        float ShadowOffset;

        #endregion

        public BaseDrawableShape() :
            base()
        {
            Effects = new Dictionary<ShapeEffect, Shape>();
            foreach (ShapeEffect effect in System.Enum.GetValues(typeof(ShapeEffect)))
                Effects.Add(effect, null);

            ShadowOffset = DEFAULT_SHADOW_OFFSET;

            OutlineThickness = 0F;

            BaseShape = new Shape();
            BaseShape.EnableFill(true);
        }

        protected void SetEffect(ShapeEffect effect, bool active = true)
        {
            if (!active)
            {
                Effects[effect] = null;
                return;
            }

            Effects[effect] = new Shape();
            Effects[effect].EnableFill(true);
        }

        protected abstract void Build();

        public override void Draw(RenderWindow window)
        {
            foreach (Shape shape in Effects.Values)
                if (shape != null)
                    window.Draw(shape);

            window.Draw(BaseShape);
        }

        public override Color Color
        {
            set
            {
                base.Color = new Color(
                    value.R,
                    value.G,
                    value.B,
                    (byte)(BACKGROUND_COLOR_ALPHA));
            }
        }

        protected void SetOutlineSettings(Color color, float outlineThickness)
        {
            OutlineColor = new Color(
                color.R,
                color.G,
                color.B,
                (byte)(BACKGROUND_COLOR_ALPHA));

            OutlineThickness = outlineThickness;

            if (BaseShape == null)
                return;

            BaseShape.OutlineThickness = OutlineThickness;
            BaseShape.EnableOutline(true);
        }

        protected void SetShadowSettings(float shadowOffset)
        {
            ShadowOffset = shadowOffset;
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                if (BaseShape == null)
                    return;

                BaseShape.Position = Position + GetPositionOffset();

                if (Effects[ShapeEffect.Shade] != null)
                    Effects[ShapeEffect.Shade].Position = Position + GetPositionOffset();

                if (Effects[ShapeEffect.Shadow] != null)
                    Effects[ShapeEffect.Shadow].Position = Position + new Vector2f(ShadowOffset, ShadowOffset) + GetPositionOffset();
            }
        }

        public override Vector2f Dimension
        {
            get { return base.Dimension + OutlineStructureDimension() + ShadowStructureDimension(); }
        }

        Vector2f ShadowStructureDimension()
        {
            return Effects[ShapeEffect.Shadow] != null ? new Vector2f(ShadowOffset, ShadowOffset) : new Vector2f();
        }

        Vector2f OutlineStructureDimension()
        {
            return new Vector2f(OutlineThickness * 2F, OutlineThickness * 2F);
        }

        public Vector2f GetPositionOffset()
        {
            return new Vector2f(OutlineThickness, OutlineThickness);
        }

        protected void AddPoint(Vector2f position)
        {
            BaseShape.AddPoint(position, Color, OutlineColor);
        }

        protected void AddShadePoint(Vector2f position, bool effectBegin = true)
        {
            if (Effects[ShapeEffect.Shade] == null)
                return;

            Effects[ShapeEffect.Shade].AddPoint(position, effectBegin ? EFFECT_BEGIN_COLOR : EFFECT_END_COLOR);
        }

        protected void AddShadowPoint(Vector2f position)
        {
            if (Effects[ShapeEffect.Shadow] == null)
                return;

            Effects[ShapeEffect.Shadow].AddPoint(position, SHADOW_EFFECT_COLOR);
        }
    }
}
