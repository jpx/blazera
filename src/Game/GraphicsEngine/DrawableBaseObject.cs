namespace BlazeraLib
{
    public abstract class DrawableBaseObject : BaseObject, IDrawable
    {
        public DrawableBaseObject() :
            base()
        {
            IsVisible = true;
            ComparisonPointYType = BaseDrawable.DEFAULT_COMPARISON_POINTY_TYPE;
        }

        public DrawableBaseObject(DrawableBaseObject copy) :
            base(copy)
        {
            IsVisible = true;
            ComparisonPointYType = copy.ComparisonPointYType;
            BasePoint = copy.BasePoint;
        }

        public virtual object Clone()
        {
            return null;
        }

        public abstract void Draw(SFML.Graphics.RenderWindow window);

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
    }
}
