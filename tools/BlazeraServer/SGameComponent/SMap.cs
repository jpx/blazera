using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraServer
{
    public class SMap : Map
    {
        MapHandler Handler;

        public DynamicObjectDictionary DynamicObjects { get; private set; }

        public SMap() :
            base()
        {
            Handler = new MapHandler(this);

            DynamicObjects = new DynamicObjectDictionary();
        }

        public SMap(SMap copy) :
            base(copy)
        {
            Handler = new MapHandler(this);

            DynamicObjects = new DynamicObjectDictionary();
        }

        public SMap(Map copy) :
            base(copy)
        {
            Handler = new MapHandler(this);

            DynamicObjects = new DynamicObjectDictionary();
        }

        public override void AddObject(WorldObject wObj)
        {
            if (wObj is DynamicWorldObject)
            {
                if (DynamicObjects == null)
                    DynamicObjects = new DynamicObjectDictionary();

                DynamicObjects.Add((DynamicWorldObject)wObj);
            }

            base.AddObject(wObj);

            PrintPlayersInfo();
        }

        public override void RemoveObject(WorldObject wObj)
        {
            if (wObj is DynamicWorldObject)
                DynamicObjects.Remove((DynamicWorldObject)wObj);

            base.RemoveObject(wObj);
           
            PrintPlayersInfo();
        }

        public void PrintPlayersInfo()
        {
            if (DynamicObjects == null)
                return;

            Log.Cldebug("#####", ConsoleColor.Cyan);

            foreach (DynamicWorldObject dObj in DynamicObjects)
            {
                Log.C("# ", ConsoleColor.Cyan);
                Log.C(dObj.Guid, ConsoleColor.Magenta);
                Log.Cl(" #", ConsoleColor.Cyan);
            }

            Log.Cldebug("#####", ConsoleColor.Cyan);
            Log.Cl();
        }
    }
}
