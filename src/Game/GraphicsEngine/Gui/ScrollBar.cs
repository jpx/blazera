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
            this.IsDragged = false;

            this.MouseWheelVZone = MOUSE_WHEEL_VZONE;

            this.ScaleValue = DEFAULT_SCALE_VALUE;
        }

        public void SetValues(Int32 displayedValue, Int32 totalValue)
        {
            this.DisplayedValue = displayedValue;
            this.TotalValue = totalValue;

            this.RefreshCursor();
        }

        public override void Refresh()
        {
            this.RefreshCursor();
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
                Int32 oldValue = this.CursorPosition;

                _cursorPosition = value;

                if (this.CursorPosition > this.TotalValue - this.DisplayedValue)
                    _cursorPosition = this.TotalValue - this.DisplayedValue;

                if (this.CursorPosition < 0)
                    _cursorPosition = 0;

                this.RefreshCursor();

                Int32 newValue = this.CursorPosition;

                if (this.Scrolled != null)
                    this.Scrolled(this, new ScrollEventArgs(oldValue - newValue));
            }
        }

        public void Scroll(Int32 scrollValue)
        {
            this.CursorPosition += scrollValue;
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
            this.Background = new PictureBox(Create.Texture("VScrollBar_Background"));
            this.BackgroundDimension = new Vector2(this.BackgroundDimension.X * ScrollBar.BACKGROUND_RESIZE_FACTOR, this.BackgroundDimension.Y);

            this.ScrollCursor = new PictureBox(Create.Texture("VScrollBar_Cursor"));
            this.ScrollCursor.Dimension = new Vector2(this.ScrollCursor.Dimension.X * ScrollBar.CURSOR_RESIZE_FACTOR, this.ScrollCursor.Dimension.Y * ScrollBar.CURSOR_RESIZE_FACTOR);
            this.AddWidget(this.ScrollCursor);

            this.RefreshCursor();
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
                    if (this.Parent.BackgroundContainsMouse() &&
                        this.Root.Window.Input.GetMouseY() < this.Parent.Center.Y + this.Parent.BackgroundDimension.Y * (this.MouseWheelVZone - .5f))
                    {
                        this.Scroll(-(Int32)(evt.MouseWheel.Delta * this.ScaleValue));

                        return true;
                    }

                    break;

                case EventType.MouseButtonPressed:
                    if (evt.MouseButton.Button == MouseButton.Left &&
                        this.ScrollCursor.ContainsMouse())
                    {
                        this.IsDragged = true;
                        this.DraggingPoint = this.Root.Window.Input.GetMouseY() - this.ScrollCursor.Position.Y;

                        return true;
                    }

                    break;

                case EventType.MouseButtonReleased:
                    if (this.IsDragged && evt.MouseButton.Button == MouseButton.Left)
                    {
                        this.IsDragged = false;

                        return true;
                    }

                    break;

                case EventType.MouseMoved:
                    if (this.IsDragged)
                    {
                        this.Scroll(-this.CursorPosition + (Int32)(evt.MouseMove.Y - this.DraggingPoint - this.Position.Y));

                        return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }
        
        protected override void RefreshCursor()
        {
            base.RefreshCursor();

            this.ScrollCursor.Position = this.GetGlobalFromLocal(new Vector2(
                this.Halfsize.X - this.ScrollCursor.Halfsize.X,
                CURSOR_MARGINS + this.CursorPosition * (this.Dimension.Y - this.ScrollCursor.Dimension.Y - CURSOR_MARGINS * 2F) / (this.TotalValue - this.DisplayedValue)));
        }
    }

    public class HScrollBar : ScrollBar
    {
        public HScrollBar() :
            base()
        {
            this.Background = new PictureBox(Create.Texture("HScrollBar_Background"));
            this.BackgroundDimension = new Vector2(this.BackgroundDimension.X, this.BackgroundDimension.Y * ScrollBar.BACKGROUND_RESIZE_FACTOR);

            this.ScrollCursor = new PictureBox(Create.Texture("HScrollBar_Cursor"));
            this.ScrollCursor.Dimension = new Vector2(this.ScrollCursor.Dimension.X * ScrollBar.CURSOR_RESIZE_FACTOR, this.ScrollCursor.Dimension.Y * ScrollBar.CURSOR_RESIZE_FACTOR);
            this.AddWidget(this.ScrollCursor);

            this.RefreshCursor();
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
                    if (this.Parent.ContainsMouse() &&
                        this.Root.Window.Input.GetMouseY() > this.Parent.Center.Y - this.Parent.Dimension.Y * ((1F - this.MouseWheelVZone) - .5f))
                    {
                        this.Scroll(-(Int32)(evt.MouseWheel.Delta * this.ScaleValue));

                        return true;
                    }

                    break;

                case EventType.MouseButtonPressed:
                    if (evt.MouseButton.Button == MouseButton.Left &&
                        this.ScrollCursor.ContainsMouse())
                    {
                        this.IsDragged = true;
                        this.DraggingPoint = this.Root.Window.Input.GetMouseX() - this.ScrollCursor.Position.X;

                        return true;
                    }

                    break;

                case EventType.MouseButtonReleased:
                    if (this.IsDragged && evt.MouseButton.Button == MouseButton.Left)
                    {
                        this.IsDragged = false;

                        return true;
                    }

                    break;

                case EventType.MouseMoved:
                    if (this.IsDragged)
                    {
                        this.Scroll(-this.CursorPosition + (Int32)(evt.MouseMove.X - this.DraggingPoint - this.Position.X));

                        return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }
        
        protected override void RefreshCursor()
        {
            base.RefreshCursor();

            this.ScrollCursor.Position = this.GetGlobalFromLocal(new Vector2(
                CURSOR_MARGINS + this.CursorPosition * (this.Dimension.X - this.ScrollCursor.Dimension.X - CURSOR_MARGINS * 2F) / (this.TotalValue - this.DisplayedValue),
                this.Halfsize.Y - this.ScrollCursor.Halfsize.Y));
        }
    }

    public delegate void ScrollEventHandler(object sender, ScrollEventArgs e);

    public class ScrollEventArgs : EventArgs
    {
        public ScrollEventArgs(Int32 scrollValue)
        {
            this.ScrollValue = scrollValue;
        }

        public Int32 ScrollValue { get; set; }
    }
}
