using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

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
            MainBox = new VAutoSizeBox();
            AddWidget(MainBox);

            ScrollBox = new HAutoSizeBox(false, null, Border.GetBoxBorderWidth(), HBox.DEFAULT_X_EXTREMITY_OFFSET, noBackgroundMode);
            MainBox.AddItem(ScrollBox);

            ExtendedBox = new ExtendedVBox(size);
            ScrollBox.AddItem(ExtendedBox);

            VScrollBar = new VScrollBar();
            VScrollBar.MouseWheelVZone = 1F;
            ScrollBox.AddItem(VScrollBar);
            VScrollBar.Scrolled += new ScrollEventHandler(OnScroll);
        }

        protected virtual void OnScroll(object sender, ScrollEventArgs e)
        {
            ExtendedBox.SetCurrentPointer(VScrollBar.CursorPosition);
        }

        protected virtual void AddItem(Widget widget)
        {
            ExtendedBox.AddItem(widget);

            RefreshScrollBar();
        }

        void RefreshScrollBar()
        {
            VScrollBar.DisplayedValue = ExtendedBox.Size;
            VScrollBar.TotalValue = ExtendedBox.GetTotalSize();
        }

        public virtual Boolean RemoveItem(Widget widget)
        {
            if (!ExtendedBox.RemoveItem(widget))
                return false;

            VScrollBar.DisplayedValue = ExtendedBox.Size;
            VScrollBar.TotalValue = ExtendedBox.GetTotalSize();

            return true;
        }

        public Widget GetAt(Int32 index)
        {
            return ExtendedBox.GetAt(index);
        }

        public void ResetScrollBar()
        {
            if (VScrollBar != null)
                VScrollBar.Reset();
        }

        public virtual void Clear()
        {
            ResetScrollBar();

            ExtendedBox.Clear();

            RefreshScrollBar();
        }

        public override Vector2f Dimension
        {
            get
            {
                if (MainBox == null)
                    return base.Dimension;

                return MainBox.BackgroundDimension;
            }
        }

        public override void Refresh()
        {
            VScrollBar.Dimension = new Vector2f(
                VScrollBar.Dimension.X,
                ExtendedBox.GetTotalSize() == 0 ? DEFAULT_EMPTY_SIZE : ExtendedBox.BackgroundDimension.Y);
        }
    }
}
