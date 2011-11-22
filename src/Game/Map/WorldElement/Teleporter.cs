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

        public void SetSetting(string mapName, string warpPointName, IntRect rect, Texture skin = null)
        {
            Skin = skin;

            if (rect == null && Skin != null)
                rect = new IntRect(0, 0, (int)Skin.Dimension.X, (int)Skin.Dimension.Y);

            EBoundingBox areaBB = new EBoundingBox(this, EBoundingBoxType.Event, rect.Left, rect.Top, rect.Right, rect.Bottom);
            ObjectEvent evt = new ObjectEvent(ObjectEventType.In);
            evt.AddAction(new WarpAction(mapName, warpPointName));
            areaBB.AddEvent(evt);
            AddEventBoundingBox(areaBB, EventBoundingBoxType.Internal);
        }
    }
}
