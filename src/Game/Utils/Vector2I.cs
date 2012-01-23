using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Vector2I
    {
        public Vector2I()
        {
            X = 0;
            Y = 0;
        }

        public Vector2I(int x, int y)
        {
            X = x;
            Y = y;
        }

        public Vector2I(uint x, uint y)
        {
            X = (int)x;
            Y = (int)y;
        }

        public Vector2I(Vector2I copy)
        {
            X = copy.X;
            Y = copy.Y;
        }

        public override string ToString()
        {
            return "Vector2I (" + X.ToString() + ", " + Y.ToString() + ")";
        }

        public static bool operator ==(Vector2I v1, Vector2I v2)
        {
            if ((object)v1 == null)
                return ((object)v2 == null);

            if ((object)v2 == null)
                return ((object)v1 == null);

            return v1.X == v2.X && v1.Y == v2.Y;
        }

        public static bool operator !=(Vector2I v1, Vector2I v2)
        {
            return !(v1 == v2);
        }

        public static Vector2I operator -(Vector2I v)
        {
            return new Vector2I(-v.X, -v.Y);
        }

        public static Vector2I operator -(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X - v2.X, v1.Y - v2.Y);
        }

        public static Vector2I operator +(Vector2I v1, Vector2I v2)
        {
            return new Vector2I(v1.X + v2.X, v1.Y + v2.Y);
        }

        public static Vector2I operator *(Vector2I v, int x)
        {
            return new Vector2I(v.X * x, v.Y * x);
        }

        public static Vector2I operator *(int x, Vector2I v)
        {
            return new Vector2I(v.X * x, v.Y * x);
        }

        public static Vector2I operator /(Vector2I v, int x)
        {
            return new Vector2I(v.X / x, v.Y / x);
        }

        public static Vector2I operator +(Vector2I v, int x)
        {
            return new Vector2I(v.X + x, v.Y + x);
        }

        public static Vector2I operator -(Vector2I v, int x)
        {
            return new Vector2I(v.X - x, v.Y - x);
        }

        public static Vector2I operator +(int x, Vector2I v)
        {
            return new Vector2I(v.X + x, v.Y + x);
        }

        public static Vector2I operator -(int x, Vector2I v)
        {
            return new Vector2I(v.X - x, v.Y - x);
        }

        public Vector2f ToVector2()
        {
            return new Vector2f((float)X, (float)Y);
        }

        public static Vector2I FromVector2(Vector2f vector2)
        {
            return new Vector2I((int)vector2.X, (int)vector2.Y);
        }

        public static Vector2I FromVector2(float x, float y)
        {
            return new Vector2I((int)x, (int)y);
        }

        public static Vector2f ToVector2(Vector2I vector2I)
        {
            return new Vector2f((float)vector2I.X, (float)vector2I.Y);
        }

        public static Vector2f ToVector2(int x, int y)
        {
            return new Vector2f((float)x, (float)y);
        }

        public int X { get; set; }

        public int Y { get; set; }

        public static double GetDistanceBetween(Vector2f point1, Vector2f point2)
        {
            return Math.Sqrt(Math.Pow((point2 - point1).X, 2D) + Math.Pow((point2 - point1).Y, 2D));
        }
    }

    public class IntRect
    {
        public IntRect()
        {
            Rect = new SFML.Graphics.IntRect();
        }

        public IntRect(IntRect copy)
            : this(copy.Left, copy.Top, copy.Right, copy.Bottom)
        {

        }

        public IntRect(int left, int top, int right, int bottom)
        {
            Rect = new SFML.Graphics.IntRect(left, top, right - left, bottom - top);
        }

        public SFML.Graphics.IntRect Rect { get; set; }

        public int Left { get { return Rect.Left; } }
        public int Top { get { return Rect.Top; } }
        public int Right { get { return Rect.Left + Rect.Width; } }
        public int Bottom { get { return Rect.Top + Rect.Height; } }

        public static IntRect operator +(IntRect rect, Vector2I offset)
        {
            return new IntRect(
                rect.Left + offset.X,
                rect.Top + offset.Y,
                rect.Right + offset.X,
                rect.Bottom + offset.Y);
        }

        public static IntRect operator +(Vector2I offset, IntRect rect)
        {
            return rect + offset;
        }

        public static IntRect operator +(IntRect rect, Vector2f offset)
        {
            return rect + Vector2I.FromVector2(offset);
        }

        public static IntRect operator +(Vector2f offset, IntRect rect)
        {
            return rect + offset;
        }
    }

    public class FloatRect
    {
        public FloatRect()
        {
            Rect = new SFML.Graphics.FloatRect();
        }

        public FloatRect(float left, float top, float right, float bottom)
        {
            Rect = new SFML.Graphics.FloatRect(left, top, right - left, bottom - top);
        }

        public FloatRect(FloatRect copy)
            : this(copy.Left, copy.Top, copy.Right, copy.Bottom)
        {
        }

        public SFML.Graphics.FloatRect Rect
        {
            get;
            set;
        }

        public float Left
        {
            get
            {
                return Rect.Left;
            }
        }

        public float Top
        {
            get
            {
                return Rect.Top;
            }
        }

        public float Right
        {
            get
            {
                return Rect.Left + Rect.Width;
            }
        }

        public float Bottom
        {
            get
            {
                return Rect.Top + Rect.Height;
            }
        }

        public float VSum
        {
            get
            {
                return Top + Bottom;
            }
        }

        public float HSum
        {
            get
            {
                return Left + Right;
            }
        }
    }
}
