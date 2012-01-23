using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class GridShape : BaseDrawable
    {
        #region Constants

        static readonly Color DEFAULT_COLOR = new Color(84, 84, 84, 96);

        const uint DEFAULT_LINE_THICKNESS = 1;

        #endregion

        #region Members

        uint Scale;
        uint Width;
        uint Height;

        Color Color;

        uint LineThickness;

        Shape[] Lines;

        #endregion

        public GridShape(uint scale, uint width, uint height)
        {
            Scale = scale;

            Width = width;
            Height = height;

            LineThickness = DEFAULT_LINE_THICKNESS;

            Color = DEFAULT_COLOR;

            Build();
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }

        void Build()
        {
            Lines = new Shape[Width + Height + 2];

            for (uint y = 0; y < Height + 1; ++y)
            {
                Lines[y] = Shape.Line(new Vector2f(0F, y * Scale), new Vector2f(Dimension.X, y * Scale), LineThickness, Color);
            }

            for (uint x = 0; x < Width + 1; ++x)
            {
                Lines[Height + 1 + x] = Shape.Line(new Vector2f(x * Scale, 0F), new Vector2f(x * Scale, Dimension.Y), LineThickness, Color);
            }
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            for (uint count = 0; count < Lines.Length; ++count)
                window.Draw(Lines[count]);
        }

        public override Vector2f Dimension
        {
            get
            {
                return new Vector2f(
                    Scale * Width,
                    Scale * Height);
            }
        }

        public void Move(Vector2f offset)
        {
            for (uint count = 0; count < Lines.Length; ++count)
                Lines[count].Position += offset;
        }
    }
}
