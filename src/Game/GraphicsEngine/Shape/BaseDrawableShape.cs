using System.Collections.Generic;
using SFML.Graphics;

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

        protected const float DEFAULT_SHADOW_OFFSET = 4F;

        protected static readonly Color EFFECT_BEGIN_COLOR = Color.White;
        protected static readonly Color EFFECT_END_COLOR = Color.Black;
        protected static readonly Color SHADOW_EFFECT_COLOR = new Color(0, 0, 0, 128);

        #endregion

        #region Members

        Dictionary<ShapeEffect, Shape> Effects;

        protected Shape BaseShape;

        float OutlineThickness;
        protected Color OutlineColor { get; private set; }

        protected float ShadowOffset;

        #endregion

        public BaseDrawableShape() :
            base()
        {
            Effects = new Dictionary<ShapeEffect, Shape>();
            foreach (ShapeEffect effect in System.Enum.GetValues(typeof(ShapeEffect)))
                Effects.Add(effect, null);

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

            foreach (Shape shape in Effects.Values)
                if (shape != null)
                {
                    shape.OutlineThickness = OutlineThickness;
                    shape.EnableOutline(true);
                }
        }

        public override Vector2 Position
        {
            set
            {
                base.Position = value;

                if (BaseShape == null)
                    return;

                BaseShape.Position = Position + GetPositionOffset();

                foreach (Shape shape in Effects.Values)
                    if (shape != null)
                        shape.Position = Position + GetPositionOffset();
            }
        }

        public override Vector2 Dimension
        {
            get { return base.Dimension + new Vector2(OutlineThickness * 2F, OutlineThickness * 2F); }
        }

        public Vector2 GetPositionOffset()
        {
            return new Vector2(OutlineThickness, OutlineThickness);
        }

        protected bool IsEffectActive(ShapeEffect effect)
        {
            return Effects[effect] != null;
        }

        protected Shape GetShapeFromEffect(ShapeEffect effect)
        {
            return Effects[effect];
        }
    }
}
