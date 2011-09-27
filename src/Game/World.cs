using System.Collections.Generic;

namespace BlazeraLib
{
    public abstract class World : PacketHandler, IUpdateable
    {
        #region Members

        protected Dictionary<string, Map> Maps;

        #endregion

        protected World()
        {
            Maps = new Dictionary<string, Map>();
        }

        protected void AddMap(Map map)
        {
            if (Maps.ContainsKey(map.Type))
                throw new System.Exception("Map " + map.Type + " was already added.");

            Maps.Add(map.Type, map);
        }

        protected bool IsEmpty()
        {
            return Maps.Count == 0;
        }

        public virtual bool AddMap(string mapType)
        {
            return !Maps.ContainsKey(mapType);
        }

        public virtual void Update(Time dt)
        {
            RefreshReception();
        }
    }
}
