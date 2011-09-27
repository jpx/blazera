using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public abstract class ScrollableWidget : Widget
    {
        const float DEFAULT_EMPTY_SIZE = 10F;

        protected VAutoSizeBox MainBox { get; set; }
        protected HAutoSizeBox ScrollBox { get; set; }
        protected ExtendedVBox ExtendedBox { get; set; }

        protected VScrollBar VScrollBar { get; set; }

        public ScrollableWidget(Int32 size = BlazeraLib.ExtendedBox.DEFAULT_SIZE, Boolean noBackgroundMode = true) :
            base()
        {
            this.MainBox = new VAutoSizeBox();
            this.AddWidget(this.MainBox);

            this.ScrollBox = new HAutoSizeBox(false, null, Border.GetBoxBorderWidth(), HBox.DEFAULT_X_EXTREMITY_OFFSET, noBackgroundMode);
            this.MainBox.AddItem(this.ScrollBox);

            this.ExtendedBox = new ExtendedVBox(size);
            this.ScrollBox.AddItem(this.ExtendedBox);

            this.VScrollBar = new VScrollBar();
            this.VScrollBar.MouseWheelVZone = 1F;
            this.ScrollBox.AddItem(this.VScrollBar);
            this.VScrollBar.Scrolled += new ScrollEventHandler(OnScroll);
        }

        protected virtual void OnScroll(object sender, ScrollEventArgs e)
        {
            this.ExtendedBox.SetCurrentPointer(this.VScrollBar.CursorPosition);
        }

        protected virtual void AddItem(Widget widget)
        {
            this.ExtendedBox.AddItem(widget);

            RefreshScrollBar();
        }

        void RefreshScrollBar()
        {
            this.VScrollBar.DisplayedValue = this.ExtendedBox.Size;
            this.VScrollBar.TotalValue = this.ExtendedBox.GetTotalSize();
        }

        public virtual Boolean RemoveItem(Widget widget)
        {
            if (!ExtendedBox.RemoveItem(widget))
                return false;

            this.VScrollBar.DisplayedValue = this.ExtendedBox.Size;
            this.VScrollBar.TotalValue = this.ExtendedBox.GetTotalSize();

            return true;
        }

        public Widget GetAt(Int32 index)
        {
            return ExtendedBox.GetAt(index);
        }

        public void ResetScrollBar()
        {
            if (this.VScrollBar != null)
                this.VScrollBar.Reset();
        }

        public virtual void Clear()
        {
            this.ResetScrollBar();

            this.ExtendedBox.Clear();

            RefreshScrollBar();
        }

        public override Vector2 Dimension
        {
            get
            {
                if (this.MainBox == null)
                    return base.Dimension;

                return this.MainBox.BackgroundDimension;
            }
        }

        public override void Refresh()
        {
            this.VScrollBar.Dimension = new Vector2(
                this.VScrollBar.Dimension.X,
                ExtendedBox.GetTotalSize() == 0 ? DEFAULT_EMPTY_SIZE : this.ExtendedBox.BackgroundDimension.Y);
        }
    }
}
