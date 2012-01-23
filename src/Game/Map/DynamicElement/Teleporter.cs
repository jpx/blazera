using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class Teleporter : DynamicWorldElement
    {
        #region Members

        string MapName;
        string WarpPointName;
        IntRect Area;

        public EBoundingBox TeleportationBB { get; set; }
        public ObjectEvent TeleportationEvent { get; set; }

        #endregion Members

        public Teleporter() :
            base()
        {

        }

        public Teleporter(Teleporter copy) :
            base(copy)
        {
            MapName = copy.MapName;
            WarpPointName = copy.WarpPointName;
            Area = copy.Area == null ? null : new IntRect(copy.Area);
        }

        public override object Clone()
        {
            return new Teleporter(this);
        }

        void SetArea(IntRect rect)
        {
            Area = rect;

            TeleportationBB = new EBoundingBox(this, EBoundingBoxType.Event, Area.Left, Area.Top, Area.Right, Area.Bottom);
            TeleportationEvent = new ObjectEvent(ObjectEventType.In);
            TeleportationEvent.AddAction(new WarpAction(MapName, WarpPointName));
            TeleportationBB.AddEvent(TeleportationEvent);
            AddEventBoundingBox(TeleportationBB, EventBoundingBoxType.Internal);
        }

        public void SetSetting(string mapName, string warpPointName, IntRect rect = null)
        {
            MapName = mapName;
            WarpPointName = warpPointName;

            SetArea(rect != null ? rect : new IntRect(0, 0, (int)Dimension.X, (int)Dimension.Y));
        }

        public void SetSetting(string mapName, string warpPointName, Vector2I tileDimension = null)
        {
            SetSetting(
                mapName,
                warpPointName,
                tileDimension == null ? null : (new IntRect(
                    0, 0, tileDimension.X * GameData.TILE_SIZE,
                    tileDimension.Y * GameData.TILE_SIZE)));
        }
    }
}
