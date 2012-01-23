using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class ScrollableWindowedWidget : WindowedWidget
    {
        public ScrollableWindowedWidget(String title) :
            base(title)
        {
            VScrollBar = new VScrollBar();
            AddWidget(VScrollBar);
            HScrollBar = new HScrollBar();
            AddWidget(HScrollBar);

            //Refresh();
        }

        public override void Refresh()
        {
            base.Refresh();

            UpdateScrollBars();
        }

        public void ResetScrollBars()
        {
            if (VScrollBar != null)
            {
                VScrollBar.Reset();
            }

            if (HScrollBar != null)
            {
                HScrollBar.Reset();
            }
        }

        private void UpdateScrollBars()
        {
            VScrollBar.Dimension = new Vector2f(
                VScrollBar.Dimension.X,
                Dimension.Y + Margins);

            VScrollBar.Position = GetGlobalFromLocal(new Vector2f(
                Dimension.X + Margins,
                0F));

            HScrollBar.Dimension = new Vector2f(
                Dimension.X + Margins,
                HScrollBar.Dimension.Y);

            HScrollBar.Position = GetGlobalFromLocal(new Vector2f(
                0F,
                Dimension.Y + Margins));
        }

        protected override Vector2f GetBasePosition()
        {
            return base.GetBasePosition();
        }

        protected override Vector2f GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2f(VScrollBar.Dimension.X + Margins, HScrollBar.Dimension.Y + Margins);
        }

        protected VScrollBar VScrollBar { get; set; }
        protected HScrollBar HScrollBar { get; set; }
    }
}
