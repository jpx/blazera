using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public enum Side
    {
        Left,
        Top,
        Right,
        Bottom
    }

    public abstract class Selector : Widget
    {
        protected static readonly Color DEFAULT_OUTLINE_COLOR = Color.White;
        protected static readonly Color DEFAULT_FILL_COLOR = new Color(255, 255, 255, 64);

        protected const float DEFAULT_OUTLINE_THICKNESS = 2F;
        protected const float DEFAULT_MOVE_OFFSET = 1F;

        protected Widget Holder;
        FloatRect HolderOffset;

        Boolean IsActive;

        Shape SelectionShape;

        protected Color FillColor;
        protected Color OutlineColor;

        protected float OutlineThickness;

        Boolean ConstantDrawing;

        Vector2f OriginPoint;
        Vector2f CurrentPoint;

        public float MoveOffset;
        public Boolean MoveIsEnabled;

        public event SelectorLeftEventHandler OnLeft;
        public event SelectionChangeEventHandler OnChange;

        public Selector(
            Widget holder,
            Boolean fillEnabled = false,
            Boolean outlineEnabled = true,
            Boolean constantDrawing = false) :
            base()
        {
            Holder = holder;

            if (Holder != null)
                Holder.AddWidget(this);

            HolderOffset = new FloatRect();

            IsActive = false;

            FillColor = DEFAULT_FILL_COLOR;
            OutlineColor = DEFAULT_OUTLINE_COLOR;
            
            OutlineThickness = DEFAULT_OUTLINE_THICKNESS;

            ConstantDrawing = constantDrawing;

            MoveOffset = DEFAULT_MOVE_OFFSET;
            MoveIsEnabled = true;

            SelectionShape = new Shape();
            SelectionShape.EnableFill(fillEnabled);
            SelectionShape.EnableOutline(outlineEnabled);
            SelectionShape.OutlineThickness = OutlineThickness;
        }

        public void SetHolderOffset(FloatRect holderOffset)
        {
            HolderOffset = holderOffset;
        }

        Vector2f GetLocalHolderPos(Vector2f point)
        {
            return new Vector2f(HolderOffset.Left, HolderOffset.Top) + point;
        }

        public void SetColor(Color fillColor, Color outlineColor)
        {
            FillColor = fillColor;
            OutlineColor = outlineColor;
        }

        public override void Draw(RenderWindow window)
        {
            if (IsActive || ConstantDrawing)
                window.Draw(SelectionShape);
        }

        void MoveCurrentPointTo(Vector2f point)
        {
            Vector2f oldCurrentPoint = CurrentPoint;

            CurrentPoint = point;

            if (oldCurrentPoint.X != CurrentPoint.X ||
                oldCurrentPoint.Y != CurrentPoint.Y)
                AdjustPoints();

            Vector2f originPoint = Holder.GetGlobalFromLocal(GetTopLeftPoint());
            Vector2f endPoint = Holder.GetGlobalFromLocal(GetBottomRightPoint());

            SelectionShape = GetShape(originPoint.X, originPoint.Y, endPoint.X, endPoint.Y);
        }

        protected abstract Shape GetShape(float left, float top, float right, float bottom);

        public void SetHolder(Widget holder)
        {
            Holder = holder;
        }

        void CallOnLeft(Side side)
        {
            if (OnLeft != null)
                OnLeft(this, new SelectorLeftEventArgs(side));
        }

        protected void CallOnChange(FloatRect currentRect)
        {
            if (OnChange != null)
                OnChange(this, new SelectionChangeEventArgs(currentRect));
        }

        public override void Refresh()
        {
            base.Refresh();

            UpdateCurrentPoint();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (Holder == null)
                return base.OnEvent(evt);

            if (evt.IsHandled)
                return base.OnEvent(evt);

            switch (evt.Type)
            {
                case EventType.MouseButtonPressed:

                    if (evt.MouseButton.Button != Mouse.Button.Left)
                        break;

                    if (!HolderContains())
                        break;

                    InitSelection(Holder.GetLocalFromGlobal(new Vector2f(evt.MouseButton.X, evt.MouseButton.Y)));

                    return true;

                case EventType.MouseButtonReleased:

                    if (evt.MouseButton.Button != Mouse.Button.Left)
                        break;

                    if (!IsActive)
                        break;

                    EndSelection(Holder.GetLocalFromGlobal(new Vector2f(evt.MouseButton.X, evt.MouseButton.Y)));

                    return true;

                case EventType.MouseMoved:

                    if (!IsActive)
                        break;

                    MoveCurrentPointTo(Holder.GetLocalFromGlobal(new Vector2f(evt.MouseMove.X, evt.MouseMove.Y)));

                    return true;

                case EventType.LostFocus:
                case EventType.MouseLeft:

                    if (!IsActive)
                        break;

                    EndSelection(Holder.GetLocalFromGlobal(CurrentPoint));

                    return true;

                case EventType.KeyPressed:

                    if (!MoveIsEnabled)
                        break;

                    switch (evt.Key.Code)
                    {
                        case Keyboard.Key.Left:

                            MoveSelection(new Vector2f(-MoveOffset, 0F));

                            return true;

                        case Keyboard.Key.Up:

                            MoveSelection(new Vector2f(0F, -MoveOffset));

                            return true;

                        case Keyboard.Key.Right:

                            MoveSelection(new Vector2f(MoveOffset, 0F));

                            return true;

                        case Keyboard.Key.Down:

                            MoveSelection(new Vector2f(0F, MoveOffset));

                            return true;
                    }

                    break;

            }

            return base.OnEvent(evt);
        }

        void InitSelection(Vector2f originPoint)
        {
            IsActive = true;
            OriginPoint = originPoint;
            MoveCurrentPointTo(OriginPoint);
        }

        void EndSelection(Vector2f endPoint)
        {
            IsActive = false;
            MoveCurrentPointTo(endPoint);
        }

        protected void MoveSelection(Vector2f offset)
        {
            Vector2f originPoint = GetTopLeftPoint();
            Vector2f endPoint = GetBottomRightPoint();

            if (originPoint.X + offset.X < HolderOffset.Left ||
                endPoint.X + offset.X >= Holder.BackgroundDimension.X - HolderOffset.Right)
                offset.X = 0F;

            if (originPoint.Y + offset.Y < HolderOffset.Top ||
                endPoint.Y + offset.Y >= Holder.BackgroundDimension.Y - HolderOffset.Bottom)
                offset.Y = 0F;

            OriginPoint += offset;
            MoveCurrentPointTo(CurrentPoint + offset);
        }

        protected void MoveSelectionTo(Vector2f point)
        {
            Vector2f originPoint = GetTopLeftPoint();

            MoveSelection(point - originPoint);
        }

        public Boolean HolderContains()
        {
            return Holder.BackgroundContainsMouse(new FloatRect(
                HolderOffset.Left,
                HolderOffset.Top,
                HolderOffset.Right,
                HolderOffset.Bottom));
        }

        public override void Reset()
        {
            base.Reset();

            EndSelection(OriginPoint);
        }

        public void Resize(FloatRect rect)
        {
            Vector2f originPoint = GetTopLeftPoint();
            Vector2f endPoint = GetBottomRightPoint();

            SetOriginPoint(originPoint + new Vector2f(-rect.Left, -rect.Top));
            SetEndPoint(endPoint + new Vector2f(-rect.Right, -rect.Bottom));

            MoveCurrentPointTo(CurrentPoint);

            AdjustPoints();
        }

        Vector2f GetTopLeftPoint()
        {
            return new Vector2f(
                OriginPoint.X < CurrentPoint.X ? OriginPoint.X : CurrentPoint.X,
                OriginPoint.Y < CurrentPoint.Y ? OriginPoint.Y : CurrentPoint.Y);
        }

        Vector2f GetBottomRightPoint()
        {
            return new Vector2f(
                OriginPoint.X < CurrentPoint.X ? CurrentPoint.X : OriginPoint.X,
                OriginPoint.Y < CurrentPoint.Y ? CurrentPoint.Y : OriginPoint.Y);
        }

        void SetOriginPoint(Vector2f originPoint)
        {
            if (OriginPoint.X < CurrentPoint.X)
                OriginPoint.X = originPoint.X;
            else
                CurrentPoint.X = originPoint.X;

            if (OriginPoint.Y < CurrentPoint.Y)
                OriginPoint.Y = originPoint.Y;
            else
                CurrentPoint.Y = originPoint.Y;
        }

        void SetEndPoint(Vector2f endPoint)
        {
            if (OriginPoint.X < CurrentPoint.X)
                CurrentPoint.X = endPoint.X;
            else
                OriginPoint.X = endPoint.X;

            if (OriginPoint.Y < CurrentPoint.Y)
                CurrentPoint.Y = endPoint.Y;
            else
                OriginPoint.Y = endPoint.Y;
        }

        void AdjustPoints()
        {
            if (CurrentPoint.X <= HolderOffset.Left)
            {
                CurrentPoint.X = HolderOffset.Left;
                CallOnLeft(Side.Left);
            }
            else if (CurrentPoint.X >= Holder.BackgroundDimension.X - HolderOffset.Right)
            {
                CurrentPoint.X = Holder.BackgroundDimension.X - 1F - HolderOffset.Right;
                CallOnLeft(Side.Right);
            }

            if (OriginPoint.X <= HolderOffset.Left)
                OriginPoint.X = HolderOffset.Left;
            else if (OriginPoint.X >= Holder.BackgroundDimension.X - HolderOffset.Right)
                OriginPoint.X = Holder.BackgroundDimension.X - 1F - HolderOffset.Right;

            if (CurrentPoint.Y <= HolderOffset.Top)
            {
                CurrentPoint.Y = HolderOffset.Top;
                CallOnLeft(Side.Top);
            }
            else if (CurrentPoint.Y >= Holder.BackgroundDimension.Y - HolderOffset.Bottom)
            {
                CurrentPoint.Y = Holder.BackgroundDimension.Y - 1F - HolderOffset.Bottom;
                CallOnLeft(Side.Bottom);
            }

            if (OriginPoint.Y <= HolderOffset.Top)
                OriginPoint.Y = HolderOffset.Top;
            else if (OriginPoint.Y >= Holder.BackgroundDimension.Y - HolderOffset.Bottom)
                OriginPoint.Y = Holder.BackgroundDimension.Y - 1F - HolderOffset.Bottom;
        }

        public FloatRect GetRect()
        {
            Vector2f topLeftPoint = new Vector2f(
                Math.Min(OriginPoint.X, CurrentPoint.X),
                Math.Min(OriginPoint.Y, CurrentPoint.Y));

            Vector2f bottomRightPoint = new Vector2f(
                Math.Max(OriginPoint.X, CurrentPoint.X),
                Math.Max(OriginPoint.Y, CurrentPoint.Y));

            return new FloatRect(
                topLeftPoint.X,
                topLeftPoint.Y,
                bottomRightPoint.X,
                bottomRightPoint.Y);
        }

        void UpdateCurrentPoint()
        {
            AdjustPoints();

            FloatRect rect = GetRect();

            Vector2f topLeftPoint = Holder.GetGlobalFromLocal(new Vector2f(rect.Left, rect.Top));
            Vector2f bottomRight = Holder.GetGlobalFromLocal(new Vector2f(rect.Right, rect.Bottom));

            SelectionShape = GetShape(topLeftPoint.X, topLeftPoint.Y, bottomRight.X, bottomRight.Y);
        }

        public void SetRect(FloatRect rect)
        {
            if (OriginPoint.X < CurrentPoint.X)
            {
                OriginPoint.X = rect.Left;
                CurrentPoint.X = rect.Right;
            }
            else
            {
                CurrentPoint.X = rect.Left;
                OriginPoint.X = rect.Right;
            }

            if (OriginPoint.Y < CurrentPoint.Y)
            {
                OriginPoint.Y = rect.Top;
                CurrentPoint.Y = rect.Bottom;
            }
            else
            {
                CurrentPoint.Y = rect.Top;
                OriginPoint.Y = rect.Bottom;
            }

            UpdateCurrentPoint();
        }
    }

    public delegate void SelectorLeftEventHandler(Selector sender, SelectorLeftEventArgs e);

    public class SelectorLeftEventArgs : EventArgs
    {
        public Side Side { get; private set; }

        public SelectorLeftEventArgs(Side side)
        {
            Side = side;
        }
    }

    public delegate void SelectionChangeEventHandler(Selector sender, SelectionChangeEventArgs e);

    public class SelectionChangeEventArgs : EventArgs
    {
        public FloatRect CurrentRect { get; private set; }

        public SelectionChangeEventArgs(FloatRect currentRect)
        {
            CurrentRect = currentRect;
        }
    }

    public class RectangleSelector : Selector
    {
        public FloatRect CurrentRect { get; private set; }

        public RectangleSelector(
            Widget holder,
            Boolean fillEnabled = false,
            Boolean outlineEnabled = true,
            Boolean constantDrawing = false) :
            base(holder, fillEnabled, outlineEnabled, constantDrawing)
        {

        }

        protected override Shape GetShape(float left, float top, float right, float bottom)
        {
            CurrentRect = new FloatRect(
                    left,
                    top,
                    right,
                    bottom);

            CallOnChange(GetLocalRect());

            return Shape.Rectangle(CurrentRect.Rect, FillColor, OutlineThickness, OutlineColor);
        }

        public void SetRect(Vector2f position, Vector2f size)
        {
            SetRect(new FloatRect(position.X, position.Y, position.X + size.X, position.Y + size.Y));
            /*
            Resize(new FloatRect(
                0F,
                0F,
                size.X - (CurrentRect.Right - CurrentRect.Left),
                size.Y - (CurrentRect.Bottom - CurrentRect.Top)));

            MoveSelectionTo(position);*/
        }

        FloatRect GetLocalRect()
        {
            Vector2f topLeft = Holder.GetLocalFromGlobal(new Vector2f(CurrentRect.Left, CurrentRect.Top));
            Vector2f bottomRight = Holder.GetLocalFromGlobal(new Vector2f(CurrentRect.Right, CurrentRect.Bottom));

            return new FloatRect(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        public FloatRect GetCurrentRectFromOrigin(Vector2f origin)
        {
            FloatRect localRect = GetLocalRect();

            return new FloatRect(
                localRect.Left - origin.X,
                localRect.Top - origin.Y,
                localRect.Right - origin.X,
                localRect.Bottom - origin.Y);
        }
    }

    public class CircleSelector : Selector
    {
        Circle CurrentCircle;

        public CircleSelector(
            Widget holder,
            Boolean fillEnabled = false,
            Boolean outlineEnabled = true,
            Boolean constantDrawing = false) :
            base(holder, fillEnabled, outlineEnabled, constantDrawing)
        {
            
        }

        protected override Shape GetShape(float left, float top, float right, float bottom)
        {
            CurrentCircle = new Circle(new Vector2f(left, top), (float)Vector2I.GetDistanceBetween(new Vector2f(left, top), new Vector2f(right, bottom)));

            return Shape.Circle(CurrentCircle.OriginPoint, CurrentCircle.Radius, FillColor, OutlineThickness, OutlineColor);
        }
    }

    public class Circle
    {
        public Vector2f OriginPoint;
        public float Radius;

        public Circle() :
            this(new Vector2f(), 0F)
        {
        }

        public Circle(Vector2f originPoint, float radius)
        {
            OriginPoint = originPoint;
            Radius = radius;
        }
    }
}