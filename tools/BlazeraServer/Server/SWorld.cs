using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraServer
{
    public class SWorld : World
    {
        #region Singleton

        static SWorld _instance;
        public static SWorld Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new SWorld();
                return _instance;
            }
            set { _instance = value; }
        }

        public void Init() { }

        #endregion

        public override void Update(Time dt)
        {
            foreach (SMap map in Maps.Values)
                map.Update(dt);
        }

        public override bool AddMap(string mapType)
        {
            if (!base.AddMap(mapType))
                return false;

            SMap map = new SMap(Create.Map(mapType));

            AddMap(map);

            return true;
        }

        public SMap GetMap(string mapType)
        {
            return (SMap)Maps[mapType];
        }
    }
}
