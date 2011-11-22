using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public enum ComparisonPointYType
    {
        Top,
        Center,
        Bottom
    }

    public interface IDrawable : ICloneable
    {
        void Draw(RenderWindow window);

        bool IsVisible { get; set; }

        Vector2f Position { get; set; }
        Vector2f Dimension { get; set; }

        Color Color { get; set; }

        float Left { get; set; }
        float Top { get; set; }
        float Right { get; set; }
        float Bottom { get; set; }
        Vector2f Halfsize { get; set; }
        Vector2f Center { get; set; }

        Vector2f BasePoint { get; set; }

        ComparisonPointYType ComparisonPointYType { get; set; }
    }
}
