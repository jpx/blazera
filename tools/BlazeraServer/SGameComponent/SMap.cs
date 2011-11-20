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

        public SMap(Map copy) :
            base(copy)
        {
            Handler = new MapHandler(this);

            DynamicObjects = new DynamicObjectDictionary();
        }

        public override void AddObject(WorldObject wObj)
        {
            if (wObj is DynamicWorldObject)
                DynamicObjects.Add((DynamicWorldObject)wObj);

            base.AddObject(wObj);
        }
    }
}
