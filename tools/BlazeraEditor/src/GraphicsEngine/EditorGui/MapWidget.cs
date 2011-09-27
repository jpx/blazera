using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public class MapWidget : Widget
    {
        public Map Map { get; private set; }
        Dictionary<String, WarpPointWidget> WarpPoints = new Dictionary<String, WarpPointWidget>();

        public MapWidget(Map map) :
            base()
        {
            Map = map;

            foreach (WarpPoint warpPoint in Map.WarpPoints.Values)
                AddWarpPoint(warpPoint, false);
        }

        public void AddWarpPoint(WarpPoint warpPoint, Boolean addToMap = true, Boolean defaultWarpPoint = false)
        {
            if (addToMap)
                Map.AddWarpPoint(warpPoint, defaultWarpPoint);

            if (WarpPoints.ContainsKey(warpPoint.Name))
                return;

            WarpPointWidget warpPointWidget = new WarpPointWidget(warpPoint);
            AddWidget(warpPointWidget);

            WarpPoints.Add(warpPointWidget.WarpPoint.Name, warpPointWidget);
        }

        public Boolean RemoveWarpPoint(String name, Boolean removeFromMap = true)
        {
            if (removeFromMap && !Map.RemoveWarpPoint(name))
                return false;

            return
                RemoveWidget(WarpPoints[name]) &&
                WarpPoints.Remove(name);
        }

        public void SetWarpPoint(String name, WarpPoint warpPoint, Boolean defaultWarpPoint = false)
        {
            Map.SetWarpPoint(name, warpPoint, defaultWarpPoint);

            WarpPointWidget warpPointWidget = new WarpPointWidget(warpPoint);
            SetWidget(WarpPoints[name], warpPointWidget);
            WarpPoints[name] = warpPointWidget;
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (!MapMan.Instance.WarpPointAreShown())
                Close();
        }
    }
}
