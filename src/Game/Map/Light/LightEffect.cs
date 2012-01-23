using System;
using System.Collections.Generic;

using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    //!\\ Set it as 'abstract' and add new derived class 'CircularLightEffect' //!\\
    public class LightEffect : BaseDrawable
    {
        #region Constants

        const float DEFAULT_INTENSITY = 255F;
        const int DEFAULT_TRIANGLE_COUNT = 64;

        #endregion Constants

        #region Members

        protected bool IsActive;

        protected float Radius { get; private set; }
        float Intensity;
        int TriangleCount;

        protected List<Shape> Triangles { get; private set; }

        #endregion Members

        public LightEffect()
            : base()
        {
            IsActive = true;

            Triangles = new List<Shape>();
        }

        public LightEffect(LightEffect copy)
            : base(copy)
        {
            IsActive = true;

            Triangles = new List<Shape>();

            Intensity = copy.Intensity;
            Radius = copy.Radius;
            TriangleCount = copy.TriangleCount;
        }

        public override object Clone()
        {
            return new LightEffect(this);
        }

        public void Activate(bool isActive = true)
        {
            IsActive = isActive;
        }

        public void Init(float radius, float intensity = DEFAULT_INTENSITY, int triangleCount = DEFAULT_TRIANGLE_COUNT)
        {
            Radius = radius;
            Intensity = intensity;
            TriangleCount = triangleCount;
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible || !IsActive)
                return;

            foreach (Shape triangle in Triangles)
                window.Draw(triangle);
        }

        public virtual List<Shape> Generate(List<OpacityWall> walls)
        {
            if (!IsActive)
                return null;

            Triangles.Clear();

            float triangleAngle = (float)(Math.PI * 2D) / (float)TriangleCount;

            for (int count = 0; count < TriangleCount; ++count)
                AddTriangle(
                    new Vector2f((float)(Radius * Math.Cos(count * triangleAngle)), (float)(Radius * Math.Sin(count * triangleAngle))),
                    new Vector2f((float)(Radius * Math.Cos((count + 1) * triangleAngle)), (float)(Radius * Math.Sin((count + 1) * triangleAngle))),
                    walls);

            return Triangles;
        }

        Vector2f GetIntersection(Vector2f point1, Vector2f point2, Vector2f point3, Vector2f point4)
        {
            Vector2f intersection = new Vector2f();

            if((point2.X - point1.X) == 0 && (point4.X - point3.X) == 0)
                return new Vector2f();

            if((point2.X - point1.X) == 0)
            {
                intersection.X = point1.X;

                float c = (point4.Y - point3.Y) / (point4.X - point3.X);
                float d = point3.Y - point3.X * c;

                intersection.Y = c * intersection.X + d;
            }
            else if((point4.X - point3.X) == 0)
            {
                intersection.X = point3.X;

                float a = (point2.Y - point1.Y) / (point2.X - point1.X);
                float b = point1.Y - point1.X * a;

                intersection.Y = a * intersection.X + b;
            }
            else
            {
                float a = (point2.Y - point1.Y) / (point2.X - point1.X);
                float b = point1.Y - point1.X * a;

                float c = (point4.Y - point3.Y) / (point4.X - point3.X);
                float d = point3.Y - point3.X * c;

                intersection.X = (d - b) / (a - c);
                intersection.Y = a * intersection.X + b;
            }

            return intersection;
        }

        const float MARGIN = .1F;
        Vector2f GetCollision(Vector2f point1, Vector2f point2, Vector2f point3, Vector2f point4)
        {
            Vector2f collision = new Vector2f();

            collision = GetIntersection(point1, point2, point3, point4);

            if (((collision.X >= point1.X - MARGIN && collision.X <= point2.X + MARGIN) ||
                (collision.X >= point2.X - MARGIN && collision.X <= point1.X + MARGIN)) &&
                ((collision.X >= point3.X - MARGIN && collision.X <= point4.X + MARGIN) ||
                (collision.X >= point4.X - MARGIN && collision.X <= point3.X + MARGIN)) &&
                ((collision.Y >= point1.Y - MARGIN && collision.Y <= point2.Y + MARGIN) ||
                (collision.Y >= point2.Y - MARGIN && collision.Y <= point1.Y + MARGIN)) &&
                ((collision.Y >= point3.Y - MARGIN && collision.Y <= point4.Y + MARGIN) ||
                (collision.Y >= point4.Y - MARGIN && collision.Y <= point3.Y + MARGIN)))
                return collision;

            return new Vector2f();
        }

        static readonly Color DEFAULT_TRIANGLE_OUTLINE_COLOR = Color.White;
        protected void AddTriangle(Vector2f firstPoint, Vector2f secondPoint, List<OpacityWall> walls)
        {
            AddTriangle(firstPoint, secondPoint, walls, 0);
        }

        void AddTriangle(Vector2f firstPoint, Vector2f secondPoint, List<OpacityWall> walls, int currentCount)
        {
            for (int count = currentCount; count < walls.Count; ++count)
            {
                OpacityWall wall = walls[count];

                Vector2f l1 = new Vector2f(wall.FirstPoint.X - Center.X, wall.FirstPoint.Y - Center.Y);
                Vector2f l2 = new Vector2f(wall.SecondPoint.X - Center.X, wall.SecondPoint.Y - Center.Y);

                if (l1.X * l1.X + l1.Y * l1.Y < Radius * Radius)
                {
                    Vector2f intersection = GetIntersection(firstPoint, secondPoint, new Vector2f(), l1);

                    if (!firstPoint.Equals(intersection) && !secondPoint.Equals(intersection) &&
                        ((firstPoint.X >= intersection.X && secondPoint.X <= intersection.X) ||
                        (firstPoint.X <= intersection.X && secondPoint.X >= intersection.X)) &&
                        ((firstPoint.Y >= intersection.Y && secondPoint.Y <= intersection.Y) ||
                        (firstPoint.Y <= intersection.Y && secondPoint.Y >= intersection.Y)) &&
                        ((l1.Y >= 0 && intersection.Y >= 0) ||
                        (l1.Y <= 0 && intersection.Y <= 0)) &&
                        ((l1.X >= 0 && intersection.X >= 0) ||
                        (l1.X <= 0 && intersection.X <= 0)))
                    {
                        AddTriangle(intersection, secondPoint, walls, count);
                        secondPoint = intersection;
                    }
                }

                if (l2.X * l2.X + l2.Y * l2.Y < Radius * Radius)
                {
                    Vector2f intersection = GetIntersection(firstPoint, secondPoint, new Vector2f(), l2);

                    if (!firstPoint.Equals(intersection) && !secondPoint.Equals(intersection) &&
                        ((firstPoint.X >= intersection.X && secondPoint.X <= intersection.X) ||
                        (firstPoint.X <= intersection.X && secondPoint.X >= intersection.X)) &&
                        ((firstPoint.Y >= intersection.Y && secondPoint.Y <= intersection.Y) ||
                        (firstPoint.Y <= intersection.Y && secondPoint.Y >= intersection.Y)) &&
                        ((l2.Y >= 0 && intersection.Y >= 0) ||
                        (l2.Y <= 0 && intersection.Y <= 0)) &&
                        ((l2.X >= 0 && intersection.X >= 0) ||
                        (l2.X <= 0 && intersection.X <= 0)))
                    {
                        AddTriangle(firstPoint, intersection, walls, count);
                        firstPoint = intersection;
                    }
                }

                Vector2f m = GetCollision(l1, l2, new Vector2f(), firstPoint);
                Vector2f n = GetCollision(l1, l2, new Vector2f(), secondPoint);
                Vector2f o = GetCollision(l1, l2, firstPoint, secondPoint);

                if ((m.X != 0 || m.Y != 0) && (n.X != 0 || n.Y != 0))
                {
                    firstPoint = m;
                    secondPoint = n;
                }
                else
                {
                    if ((m.X != 0 || m.Y != 0) && (o.X != 0 || o.Y != 0))
                    {
                        AddTriangle(m, o, walls, count);
                        firstPoint = o;
                    }

                    if ((n.X != 0 || n.Y != 0) && (o.X != 0 || o.Y != 0))
                    {
                        AddTriangle(o, n, walls, count);
                        secondPoint = o;
                    }
                }
            }

            Shape triangle = new Shape();

            float intensity = Intensity;

            triangle.AddPoint(
                new Vector2f(),
                new Color(
                    (byte)(intensity * (float)Color.R / 255F),
                    (byte)(intensity * (float)Color.G / 255F),
                    (byte)(intensity * (float)Color.B / 255F)),
                DEFAULT_TRIANGLE_OUTLINE_COLOR);

            intensity = GetIntensity(firstPoint);

            triangle.AddPoint(
                firstPoint,
                new Color(
                    (byte)(intensity * (float)Color.R / 255F),
                    (byte)(intensity * (float)Color.G / 255F),
                    (byte)(intensity * (float)Color.B / 255F)),
                DEFAULT_TRIANGLE_OUTLINE_COLOR);

            intensity = GetIntensity(secondPoint);

            triangle.AddPoint(
                secondPoint,
                new Color(
                    (byte)(intensity * (float)Color.R / 255F),
                    (byte)(intensity * (float)Color.G / 255F),
                    (byte)(intensity * (float)Color.B / 255F)),
                DEFAULT_TRIANGLE_OUTLINE_COLOR);

            AddShape(triangle);
        }

        int GetIntensity(Vector2f point)
        {
            int intensity = (int)(Intensity - (float)Math.Sqrt(point.X * point.X + point.Y * point.Y) * Intensity / Radius);

            if (intensity < 0)
                intensity = 0;
            if (intensity > 255)
                intensity = 255;

            return intensity;
        }

        void AddShape(Shape shape)
        {
            shape.BlendMode = BlendMode.Add;
            shape.Position = Center;

            Triangles.Add(shape);
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                foreach (Shape shape in Triangles)
                    shape.Position = Center;
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                return new Vector2f(Radius * 2F, Radius * 2F);
            }
        }

        public override Vector2f BasePoint
        {
            get { return Halfsize; }
        }
    }
}
