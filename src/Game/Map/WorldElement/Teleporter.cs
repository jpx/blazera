using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class Teleporter : DynamicWorldElement
    {
        public Teleporter() :
            base()
        {

        }

        public Teleporter(Teleporter copy) :
            base(copy)
        {

        }

        public void SetSetting(WarpPoint destinationWarpPoint, IntRect rect, Texture skin = null)
        {
            if (rect == null && skin != null)
                rect = new IntRect(0, 0, (int)skin.Dimension.X, (int)skin.Dimension.Y);
        }
    }
}
