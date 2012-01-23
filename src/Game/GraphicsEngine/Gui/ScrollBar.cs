using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class ScrollBar : Widget
    {
        protected const float CURSOR_MARGINS = 0f;
        protected const float MOUSE_WHEEL_VZONE = .6F;
        protected const float MOUSE_WHEEL_HZONE = 1F - MOUSE_WHEEL_VZONE;

        protected const float BACKGROUND_RESIZE_FACTOR = .5F;
        protected const float CURSOR_RESIZE_FACTOR = .8F;

        const Int32 DEFAULT_SCALE_VALUE = 1;

        public Int32 DisplayedValue { get; set; }

        public Int32 ScaleValue { get; set; }

        public event ScrollEventHandler Scrolled;

        public float MouseWheelVZone { get; set; }

        public ScrollBar() :
            base()
        {
            IsDragged = false;

            MouseWheelVZone = MOUSE_WHEEL_VZONE;

            ScaleValue = DEFAULT_SCALE_VALUE;
        }

        public void SetValues(Int32 displayedValue, Int32 totalValue)
        {
            DisplayedValue = displayedValue;
            TotalValue = totalValue;

            RefreshCursor();
        }

        public override void Refresh()
        {
            RefreshCursor();
        }

        public override void Reset()
        {
            _cursorPosition = 0;
        }

        protected PictureBox ScrollCursor { get; set; }

        private Int32 _cursorPosition;
        public Int32 CursorPosition
        {
            get
            {
                return _cursorPosition;
            }
            private set
            {
                Int32 oldValue = CursorPosition;

                _cursorPosition = value;

                if (CursorPosition > TotalValue - DisplayedValue)
                    _cursorPosition = TotalValue - DisplayedValue;

                if (CursorPosition < 0)
                    _cursorPosition = 0;

                RefreshCursor();

                Int32 newValue = CursorPosition;

                if (Scrolled != null)
                    Scrolled(this, new ScrollEventArgs(oldValue - newValue));
            }
        }

        public void Scroll(Int32 scrollValue)
        {
            CursorPosition += scrollValue;
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (ScrollCursor == null)
                return;

            RefreshCursor();
        }

        protected virtual void RefreshCursor()
        {
            if (TotalValue > DisplayedValue)
                ScrollCursor.Open();
            else
                ScrollCursor.Close();
        }

        protected Boolean IsDragged { get; set; }
        protected float DraggingPoint { get; set; }

        private Int32 _totalValue;
        public Int32 TotalValue
        {
            get
            {
                return _totalValue;
            }
            set
            {
                _totalValue = value;

                if (TotalValue < DisplayedValue)
                    _totalValue = DisplayedValue;

                if (CursorPosition > TotalValue - DisplayedValue)
                    Reset();

                RefreshCursor();
            }
        }
    }

    public class VScrollBar : ScrollBar
    {
        public VScrollBar() :
            base()
        {
            Background = new PictureBox(Create.Texture("VScrollBar_Background"));
            BackgroundDimension = new Vector2f(BackgroundDimension.X * ScrollBar.BACKGROUND_RESIZE_FACTOR, BackgroundDimension.Y);

            ScrollCursor = new PictureBox(Create.Texture("VScrollBar_Cursor"));
            ScrollCursor.Dimension = new Vector2f(ScrollCursor.Dimension.X * ScrollBar.CURSOR_RESIZE_FACTOR, ScrollCursor.Dimension.Y * ScrollBar.CURSOR_RESIZE_FACTOR);
            AddWidget(ScrollCursor);

            RefreshCursor();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
            {
                return base.OnEvent(evt);
            }


            /*
             * TODO --> gestion de la zone de scroll au niveau superieur i.e au sein du scrollableWidget
             * 
             * 
             */
            switch (evt.Type)
            {
                case EventType.MouseWheelMoved:
                    if (Parent.BackgroundContainsMouse() &&
                        Mouse.GetPosition(Root.Window).Y < Parent.Center.Y + Parent.BackgroundDimension.Y * (MouseWheelVZone - .5f))
                    {
                        Scroll(-(Int32)(evt.MouseWheel.Delta * ScaleValue));

                        return true;
                    }

                    break;

                case EventType.MouseButtonPressed:
                    if (evt.MouseButton.Button == Mouse.Button.Left &&
                        ScrollCursor.ContainsMouse())
                    {
                        IsDragged = true;
                        DraggingPoint = Mouse.GetPosition(Root.Window).Y - ScrollCursor.Position.Y;

                        return true;
                    }

                    break;

                case EventType.MouseButtonReleased:
                    if (IsDragged && evt.MouseButton.Button == Mouse.Button.Left)
                    {
                        IsDragged = false;

                        return true;
                    }

                    break;

                case EventType.MouseMoved:
                    if (IsDragged)
                    {
                        Scroll(-CursorPosition + (Int32)(evt.MouseMove.Y - DraggingPoint - Position.Y));

                        return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }
        
        protected override void RefreshCursor()
        {
            base.RefreshCursor();

            ScrollCursor.Position = GetGlobalFromLocal(new Vector2f(
                Halfsize.X - ScrollCursor.Halfsize.X,
                CURSOR_MARGINS + CursorPosition * (Dimension.Y - ScrollCursor.Dimension.Y - CURSOR_MARGINS * 2F) / (TotalValue - DisplayedValue)));
        }
    }

    public class HScrollBar : ScrollBar
    {
        public HScrollBar() :
            base()
        {
            Background = new PictureBox(Create.Texture("HScrollBar_Background"));
            BackgroundDimension = new Vector2f(BackgroundDimension.X, BackgroundDimension.Y * ScrollBar.BACKGROUND_RESIZE_FACTOR);

            ScrollCursor = new PictureBox(Create.Texture("HScrollBar_Cursor"));
            ScrollCursor.Dimension = new Vector2f(ScrollCursor.Dimension.X * ScrollBar.CURSOR_RESIZE_FACTOR, ScrollCursor.Dimension.Y * ScrollBar.CURSOR_RESIZE_FACTOR);
            AddWidget(ScrollCursor);

            RefreshCursor();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
            {
                return base.OnEvent(evt);
            }

            switch (evt.Type)
            {
                case EventType.MouseWheelMoved:
                    if (Parent.ContainsMouse() &&
                        Mouse.GetPosition(Root.Window).Y > Parent.Center.Y - Parent.Dimension.Y * ((1F - MouseWheelVZone) - .5f))
                    {
                        Scroll(-(Int32)(evt.MouseWheel.Delta * ScaleValue));

                        return true;
                    }

                    break;

                case EventType.MouseButtonPressed:
                    if (evt.MouseButton.Button == Mouse.Button.Left &&
                        ScrollCursor.ContainsMouse())
                    {
                        IsDragged = true;
                        DraggingPoint = Mouse.GetPosition(Root.Window).X - ScrollCursor.Position.X;

                        return true;
                    }

                    break;

                case EventType.MouseButtonReleased:
                    if (IsDragged && evt.MouseButton.Button == Mouse.Button.Left)
                    {
                        IsDragged = false;

                        return true;
                    }

                    break;

                case EventType.MouseMoved:
                    if (IsDragged)
                    {
                        Scroll(-CursorPosition + (Int32)(evt.MouseMove.X - DraggingPoint - Position.X));

                        return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }
        
        protected override void RefreshCursor()
        {
            base.RefreshCursor();

            ScrollCursor.Position = GetGlobalFromLocal(new Vector2f(
                CURSOR_MARGINS + CursorPosition * (Dimension.X - ScrollCursor.Dimension.X - CURSOR_MARGINS * 2F) / (TotalValue - DisplayedValue),
                Halfsize.Y - ScrollCursor.Halfsize.Y));
        }
    }

    public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);

    public class ScrollEventArgs : EventArgs
    {
        public ScrollEventArgs(Int32 scrollValue)
        {
            ScrollValue = scrollValue;
        }

        public Int32 ScrollValue { get; set; }
    }
}
