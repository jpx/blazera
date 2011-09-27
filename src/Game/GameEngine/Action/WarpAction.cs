using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class WarpAction : Action
    {
        public String MapName { get; private set; }
        public String WarpPointName { get; private set; }

        public WarpAction(String mapName, String warpPointName = null) :
            base(ActionType.Warp)
        {
            MapName = mapName;
            WarpPointName = warpPointName;
            InterruptsEvents = true;
        }

        public WarpAction(WarpAction copy) :
            base(copy)
        {
            MapName = copy.MapName;
            WarpPointName = copy.WarpPointName;
            InterruptsEvents = true;
        }

        public override Boolean Do(ObjectEventArgs args)
        {
            // checks the conditions
            if (!base.Do(args))
                return false;

            args.Player.SetMap(MapName, WarpPointName);

            return true;
        }

        public override String ToScript()
        {
            String toScript = "";

            toScript += Id + " = WarpAction ( " + ScriptWriter.GetStringOf(MapName) + ", " + ScriptWriter.GetStringOf(WarpPointName) + " )";

            toScript += base.ToScript();

            return toScript;
        }
    }
}
