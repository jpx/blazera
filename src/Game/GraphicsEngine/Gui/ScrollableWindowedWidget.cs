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
            this.VScrollBar = new VScrollBar();
            this.AddWidget(this.VScrollBar);
            this.HScrollBar = new HScrollBar();
            this.AddWidget(this.HScrollBar);

            //this.Refresh();
        }

        public override void Refresh()
        {
            base.Refresh();

            this.UpdateScrollBars();
        }

        public void ResetScrollBars()
        {
            if (this.VScrollBar != null)
            {
                this.VScrollBar.Reset();
            }

            if (this.HScrollBar != null)
            {
                this.HScrollBar.Reset();
            }
        }

        private void UpdateScrollBars()
        {
            this.VScrollBar.Dimension = new Vector2f(
                this.VScrollBar.Dimension.X,
                this.Dimension.Y + this.Margins);

            this.VScrollBar.Position = this.GetGlobalFromLocal(new Vector2f(
                this.Dimension.X + this.Margins,
                0F));

            this.HScrollBar.Dimension = new Vector2f(
                this.Dimension.X + this.Margins,
                this.HScrollBar.Dimension.Y);

            this.HScrollBar.Position = this.GetGlobalFromLocal(new Vector2f(
                0F,
                this.Dimension.Y + this.Margins));
        }

        protected override Vector2f GetBasePosition()
        {
            return base.GetBasePosition();
        }

        protected override Vector2f GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2f(this.VScrollBar.Dimension.X + this.Margins, this.HScrollBar.Dimension.Y + this.Margins);
        }

        protected VScrollBar VScrollBar { get; set; }
        protected HScrollBar HScrollBar { get; set; }
    }
}
