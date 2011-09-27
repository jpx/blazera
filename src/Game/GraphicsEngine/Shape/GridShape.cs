﻿using SFML.Graphics;

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

        void Build()
        {
            Lines = new Shape[Width + Height + 2];

            for (uint y = 0; y < Height + 1; ++y)
            {
                Lines[y] = Shape.Line(new Vector2(0F, y * Scale), new Vector2(Dimension.X, y * Scale), LineThickness, Color);
            }

            for (uint x = 0; x < Width + 1; ++x)
            {
                Lines[Height + 1 + x] = Shape.Line(new Vector2(x * Scale, 0F), new Vector2(x * Scale, Dimension.Y), LineThickness, Color);
            }
        }

        public override void Draw(RenderWindow window)
        {
            if (!IsVisible)
                return;

            for (uint count = 0; count < Lines.Length; ++count)
                window.Draw(Lines[count]);
        }

        public override Vector2 Dimension
        {
            get
            {
                return new Vector2(
                    Scale * Width,
                    Scale * Height);
            }
        }

        public void Move(Vector2 offset)
        {
            for (uint count = 0; count < Lines.Length; ++count)
                Lines[count].Position += offset;
        }
    }
}
