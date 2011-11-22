using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class RectangleShape : BaseDrawableShape
    {
        #region Constants

        const float DEFAULT_OUTLINE_THICKNESS = 2F;
        const bool DEFAULT_SHADE_MODE = true;
        const bool DEFAULT_SHADOW_MODE = false;
        const float DEFAULT_SHADOW_OFFSET = 4F;

        #endregion

        public RectangleShape(
            Vector2f dimension,
            Color color,
            bool outlineMode,
            Color outlineColor,
            float outlineThickness = DEFAULT_OUTLINE_THICKNESS,
            bool ShadeMode = DEFAULT_SHADE_MODE,
            bool shadowMode = DEFAULT_SHADOW_MODE,
            float shadowOffset = DEFAULT_SHADOW_OFFSET) :
            base()
        {
            Dimension = dimension;

            Color = color;

            if (outlineMode)
                SetOutlineSettings(outlineColor, outlineThickness);

            if (ShadeMode)
                SetEffect(ShapeEffect.Shade);

            if (shadowMode)
            {
                SetEffect(ShapeEffect.Shadow);
                SetShadowSettings(shadowOffset);
            }

            Build();
        }

        protected override void Build()
        {
            AddPoint(new Vector2f(0F, 0F));
            AddShadePoint(new Vector2f(0F, 0F));
            AddShadowPoint(new Vector2f(0F, 0F));

            AddPoint(new Vector2f(Dimension.X, 0F));
            AddShadePoint(new Vector2f(Dimension.X, 0F));
            AddShadowPoint(new Vector2f(Dimension.X, 0F));

            AddPoint(Dimension);
            AddShadePoint(Dimension, false);
            AddShadowPoint(Dimension);

            AddPoint(new Vector2f(0F, Dimension.Y));
            AddShadePoint(new Vector2f(0F, Dimension.Y), false);
            AddShadowPoint(new Vector2f(0F, Dimension.Y));
        }
    }
}
