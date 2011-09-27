using BlazeraLib;

namespace Blazera
{
    public class CMap : Map
    {
        DynamicObjectDictionary DynamicObjects;
        MapHandler Handler;

        public CMap() :
            base()
        {
            DynamicObjects = new DynamicObjectDictionary();
            Handler = new MapHandler(this);
        }

        public CMap(Map copy) :
            base(copy)
        {
            DynamicObjects = new DynamicObjectDictionary();
        }

        public override void AddObject(WorldObject wObj)
        {
            if (wObj is DynamicWorldObject)
                return;

            base.AddObject(wObj);
        }

        public void AddDynamicObject(DynamicWorldObject dObj, float x, float y)
        {
            DynamicObjects.Add(dObj);

            base.AddObject(dObj);

            dObj.SetMap(this, x, y);
        }

        public override void RemoveObject(WorldObject wObj)
        {
            if (wObj is DynamicWorldObject)
                DynamicObjects.Remove((DynamicWorldObject)wObj);

            base.RemoveObject(wObj);
        }

        public void RemoveObject(int guid)
        {
            try
            {
                RemoveObject(DynamicObjects[guid]);
            }
            catch
            {
                Log.Clerr("Failed to remove object with guid " + guid.ToString() + ".");
            }
        }

        public DynamicWorldObject GetObject(int guid)
        {
            return DynamicObjects[guid];
        }
    }
}
