using System;

using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    public class MoveEventArgs : EventArgs
    {
        public Vector2f Move { get; private set; }
        public int ZOffset { get; private set; }

        public MoveEventArgs(Vector2f move, int zOffset)
        {
            Move = move;
            ZOffset = zOffset;
        }
    }

    public delegate void MoveEventHandler(IDrawable sender, MoveEventArgs e);

    public abstract class BaseDrawable : IDrawable
    {
        public event MoveEventHandler OnMove;
        Boolean CallOnMove(Vector2f move, int zOffset = BaseDrawable.DEFAULT_Z) { if (OnMove == null) return false; OnMove(this, new MoveEventArgs(move, zOffset)); return true; }

        #region Constants

        public const ComparisonPointYType DEFAULT_COMPARISON_POINTY_TYPE = ComparisonPointYType.Bottom;

        public const int DEFAULT_Z = 0;
        public const int DEFAULT_H = 1;

        #endregion

        public BaseDrawable() :
            base()
        {
            IsVisible = true;
            ComparisonPointYType = DEFAULT_COMPARISON_POINTY_TYPE;

            Z = DEFAULT_Z;
            H = DEFAULT_H;
        }

        public BaseDrawable(BaseDrawable copy) :
            base()
        {
            IsVisible = true;
            ComparisonPointYType = copy.ComparisonPointYType;
            BasePoint = copy.BasePoint;

            Z = copy.Z;
            H = copy.H;
        }

        public abstract void Draw(SFML.Graphics.RenderTarget window);

        public abstract object Clone();

        public virtual SFML.Graphics.Color Color { get; set; }

        public void SetAlpha(double alphaPercentage)
        {
            Color = new SFML.Graphics.Color(
                Color.R,
                Color.G,
                Color.B,
                (byte)(255D * (alphaPercentage / 100D)));
        }

        SFML.Window.Vector2f _position;
        public virtual SFML.Window.Vector2f Position
        {
            get { return _position; }
            set
            {
                Vector2f offset = value - Position;

                _position = value - BasePoint;

                CallOnMove(offset);
            }
        }
        public virtual SFML.Window.Vector2f Dimension { get; set; }

        int _z;
        public virtual int Z
        {
            get { return _z; }
            set
            {
                int offset = value - Z;

                _z = value;

                CallOnMove(new Vector2f(), offset);
            }
        }

        public virtual int H { get; set; }
        public int Summit
        {
            get { return Z + H; }
        }

        public virtual SFML.Window.Vector2f DrawingPosition
        {
            get { return new SFML.Window.Vector2f(Position.X, Position.Y + GetDrawingYOffset(this)); }
        }

        public bool IsVisible { get; set; }

        public float Left
        {
            get { return Position.X; }
            set { Position = new SFML.Window.Vector2f(value, Position.Y); }
        }

        public float Top
        {
            get { return Position.Y; }
            set { Position = new SFML.Window.Vector2f(Position.X, value); }
        }

        public float Right
        {
            get { return Position.X + Dimension.X; }
            set { Position = new SFML.Window.Vector2f(value - Dimension.X, Position.Y); }
        }

        public float Bottom
        {
            get { return Position.Y + Dimension.Y; }
            set { Position = new SFML.Window.Vector2f(Position.X, value - Dimension.Y); }
        }

        public float DrawingTop
        {
            get { return DrawingPosition.Y; }
        }

        public float DrawingBottom
        {
            get { return DrawingPosition.Y + Dimension.Y; }
        }

        public Vector2f DrawingCenter
        {
            get { return DrawingPosition + Halfsize; }
        }

        public SFML.Window.Vector2f Halfsize
        {
            get { return Dimension / 2F; }
            set { Dimension = value * 2F; }
        }

        public SFML.Window.Vector2f Center
        {
            get { return Position + Halfsize; }
            set { Position = value - Halfsize; }
        }

        public virtual SFML.Window.Vector2f BasePoint { get; set; }

        public ComparisonPointYType ComparisonPointYType { get; set; }

        public static float GetComparisonPointYByType(IDrawable drawable, ComparisonPointYType type)
        {
            switch (type)
            {
                case ComparisonPointYType.Bottom: return drawable.Bottom;
                case ComparisonPointYType.Center: return drawable.Center.Y;
                case ComparisonPointYType.Top: return drawable.Top;
                default: return GetComparisonPointYByType(drawable, DEFAULT_COMPARISON_POINTY_TYPE);
            }
        }

        public static int GetDrawingYOffset(IDrawable drawable)
        {
            return -drawable.Z * GameData.TILE_SIZE;
        }

        public virtual FloatRect GetVisibleRect()
        {
            return new FloatRect(
                DrawingPosition.X,
                DrawingTop,
                DrawingPosition.X + Dimension.X,
                DrawingBottom);
        }
    }
}
