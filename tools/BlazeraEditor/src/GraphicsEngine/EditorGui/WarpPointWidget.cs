using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class WarpPointWidget : Widget
    {
        public WarpPoint WarpPoint { get; private set; }
        PictureBox Picture;

        public WarpPointWidget(WarpPoint warpPoint) :
            base()
        {
            Picture = new PictureBox(Create.Texture("Gui_WarpPoint"));
            Picture.Texture.SetAlpha(40D);
            AddWidget(Picture);
            SetWarpPoint(warpPoint);
        }

        public void SetWarpPoint(WarpPoint warpPoint)
        {
            WarpPoint = warpPoint;

            if (WarpPoint == null)
                return;

            Picture.Position = WarpPoint.Point - Picture.Halfsize;
        }

        public override string ToString()
        {
            return WarpPoint.Name + " ( " + WarpPoint.Point.X.ToString() + ", " + WarpPoint.Point.Y.ToString() + " )";
        }

        public override void Refresh() { }
    }
}
