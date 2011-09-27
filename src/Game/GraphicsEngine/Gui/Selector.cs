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

        Vector2 OriginPoint;
        Vector2 CurrentPoint;

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

        Vector2 GetLocalHolderPos(Vector2 point)
        {
            return new Vector2(HolderOffset.Left, HolderOffset.Top) + point;
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

        void MoveCurrentPointTo(Vector2 point)
        {
            Vector2 oldCurrentPoint = CurrentPoint;

            CurrentPoint = point;

            if (oldCurrentPoint.X != CurrentPoint.X ||
                oldCurrentPoint.Y != CurrentPoint.Y)
                AdjustPoints();

            Vector2 originPoint = Holder.GetGlobalFromLocal(GetTopLeftPoint());
            Vector2 endPoint = Holder.GetGlobalFromLocal(GetBottomRightPoint());

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

                    if (evt.MouseButton.Button != MouseButton.Left)
                        break;

                    if (!HolderContains())
                        break;

                    InitSelection(Holder.GetLocalFromGlobal(new Vector2(evt.MouseButton.X, evt.MouseButton.Y)));

                    return true;

                case EventType.MouseButtonReleased:

                    if (evt.MouseButton.Button != MouseButton.Left)
                        break;

                    if (!IsActive)
                        break;

                    EndSelection(Holder.GetLocalFromGlobal(new Vector2(evt.MouseButton.X, evt.MouseButton.Y)));

                    return true;

                case EventType.MouseMoved:

                    if (!IsActive)
                        break;

                    MoveCurrentPointTo(Holder.GetLocalFromGlobal(new Vector2(evt.MouseMove.X, evt.MouseMove.Y)));

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
                        case KeyCode.Left:

                            MoveSelection(new Vector2(-MoveOffset, 0F));

                            return true;

                        case KeyCode.Up:

                            MoveSelection(new Vector2(0F, -MoveOffset));

                            return true;

                        case KeyCode.Right:

                            MoveSelection(new Vector2(MoveOffset, 0F));

                            return true;

                        case KeyCode.Down:

                            MoveSelection(new Vector2(0F, MoveOffset));

                            return true;
                    }

                    break;

            }

            return base.OnEvent(evt);
        }

        void InitSelection(Vector2 originPoint)
        {
            IsActive = true;
            OriginPoint = originPoint;
            MoveCurrentPointTo(OriginPoint);
        }

        void EndSelection(Vector2 endPoint)
        {
            IsActive = false;
            MoveCurrentPointTo(endPoint);
        }

        protected void MoveSelection(Vector2 offset)
        {
            Vector2 originPoint = GetTopLeftPoint();
            Vector2 endPoint = GetBottomRightPoint();

            if (originPoint.X + offset.X < HolderOffset.Left ||
                endPoint.X + offset.X >= Holder.BackgroundDimension.X - HolderOffset.Right)
                offset.X = 0F;

            if (originPoint.Y + offset.Y < HolderOffset.Top ||
                endPoint.Y + offset.Y >= Holder.BackgroundDimension.Y - HolderOffset.Bottom)
                offset.Y = 0F;

            OriginPoint += offset;
            MoveCurrentPointTo(CurrentPoint + offset);
        }

        protected void MoveSelectionTo(Vector2 point)
        {
            Vector2 originPoint = GetTopLeftPoint();

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
            Vector2 originPoint = GetTopLeftPoint();
            Vector2 endPoint = GetBottomRightPoint();

            SetOriginPoint(originPoint + new Vector2(-rect.Left, -rect.Top));
            SetEndPoint(endPoint + new Vector2(-rect.Right, -rect.Bottom));

            MoveCurrentPointTo(CurrentPoint);

            AdjustPoints();
        }

        Vector2 GetTopLeftPoint()
        {
            return new Vector2(
                OriginPoint.X < CurrentPoint.X ? OriginPoint.X : CurrentPoint.X,
                OriginPoint.Y < CurrentPoint.Y ? OriginPoint.Y : CurrentPoint.Y);
        }

        Vector2 GetBottomRightPoint()
        {
            return new Vector2(
                OriginPoint.X < CurrentPoint.X ? CurrentPoint.X : OriginPoint.X,
                OriginPoint.Y < CurrentPoint.Y ? CurrentPoint.Y : OriginPoint.Y);
        }

        void SetOriginPoint(Vector2 originPoint)
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

        void SetEndPoint(Vector2 endPoint)
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
            Vector2 topLeftPoint = new Vector2(
                Math.Min(OriginPoint.X, CurrentPoint.X),
                Math.Min(OriginPoint.Y, CurrentPoint.Y));

            Vector2 bottomRightPoint = new Vector2(
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

            Vector2 topLeftPoint = Holder.GetGlobalFromLocal(new Vector2(rect.Left, rect.Top));
            Vector2 bottomRight = Holder.GetGlobalFromLocal(new Vector2(rect.Right, rect.Bottom));

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

        public void SetRect(Vector2 position, Vector2 size)
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
            Vector2 topLeft = Holder.GetLocalFromGlobal(new Vector2(CurrentRect.Left, CurrentRect.Top));
            Vector2 bottomRight = Holder.GetLocalFromGlobal(new Vector2(CurrentRect.Right, CurrentRect.Bottom));

            return new FloatRect(topLeft.X, topLeft.Y, bottomRight.X, bottomRight.Y);
        }

        public FloatRect GetCurrentRectFromOrigin(Vector2 origin)
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
            CurrentCircle = new Circle(new Vector2(left, top), (float)Vector2I.GetDistanceBetween(new Vector2(left, top), new Vector2(right, bottom)));

            return Shape.Circle(CurrentCircle.OriginPoint, CurrentCircle.Radius, FillColor, OutlineThickness, OutlineColor);
        }
    }

    public class Circle
    {
        public Vector2 OriginPoint;
        public float Radius;

        public Circle() :
            this(new Vector2(), 0F)
        {
        }

        public Circle(Vector2 originPoint, float radius)
        {
            OriginPoint = originPoint;
            Radius = radius;
        }
    }
}