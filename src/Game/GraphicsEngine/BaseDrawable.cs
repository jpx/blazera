﻿namespace BlazeraLib
{
    public abstract class BaseDrawable : IDrawable
    {
        #region Constants

        public const ComparisonPointYType DEFAULT_COMPARISON_POINTY_TYPE = ComparisonPointYType.Bottom;

        #endregion

        public BaseDrawable() :
            base()
        {
            IsVisible = true;
            ComparisonPointYType = DEFAULT_COMPARISON_POINTY_TYPE;
        }

        public BaseDrawable(BaseDrawable copy) :
            base()
        {
            IsVisible = true;
            ComparisonPointYType = copy.ComparisonPointYType;
            BasePoint = copy.BasePoint;
        }

        public abstract void Draw(SFML.Graphics.RenderWindow window);
        
        public virtual object Clone()
        {
            return null;
        }

        public virtual SFML.Graphics.Color Color { get; set; }

        SFML.Graphics.Vector2 _position;
        public virtual SFML.Graphics.Vector2 Position
        {
            get { return _position; }
            set { _position = value - BasePoint; }
        }
        public virtual SFML.Graphics.Vector2 Dimension { get; set; }

        public bool IsVisible { get; set; }

        public float Left
        {
            get { return Position.X; }
            set { Position = new SFML.Graphics.Vector2(value, Position.Y); }
        }

        public float Top
        {
            get { return Position.Y; }
            set { Position = new SFML.Graphics.Vector2(Position.X, value); }
        }

        public float Right
        {
            get { return Position.X + Dimension.X; }
            set { Position = new SFML.Graphics.Vector2(value - Dimension.X, Position.Y); }
        }

        public float Bottom
        {
            get { return Position.Y + Dimension.Y; }
            set { Position = new SFML.Graphics.Vector2(Position.X, value - Dimension.Y); }
        }

        public SFML.Graphics.Vector2 Halfsize
        {
            get { return Dimension / 2F; }
            set { Dimension = value * 2F; }
        }

        public SFML.Graphics.Vector2 Center
        {
            get { return Position + Halfsize; }
            set { Position = value - Halfsize; }
        }

        public virtual SFML.Graphics.Vector2 BasePoint { get; set; }

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
    }
}