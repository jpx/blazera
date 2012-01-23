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
        event MoveEventHandler OnMove;

        void Draw(RenderTarget window);

        bool IsVisible { get; set; }

        Vector2f Position { get; set; }
        int Z { get; set; }
        Vector2f Dimension { get; set; }
        int H { get; set; }
        int Summit { get; }

        Vector2f DrawingPosition { get; }

        Color Color { get; set; }

        float Left { get; set; }
        float Top { get; set; }
        float Right { get; set; }
        float Bottom { get; set; }
        Vector2f Halfsize { get; set; }
        Vector2f Center { get; set; }

        float DrawingTop { get; }
        float DrawingBottom { get; }

        /// <summary>
        /// Origin point for position.
        /// </summary>
        Vector2f BasePoint { get; set; }

        /// <summary>
        /// Point in vertical scale where the comparison is made for drawing order.
        /// </summary>
        ComparisonPointYType ComparisonPointYType { get; set; }

        /// <summary>
        /// Rect that represents the whole area the drawable occupies.
        /// </summary>
        FloatRect GetVisibleRect();
    }
}
