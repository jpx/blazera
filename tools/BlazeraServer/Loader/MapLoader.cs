using System;
using BlazeraLib;

namespace BlazeraServer
{
    public class MapLoader
    {
        public MapLoader()
        {

        }

        /// <summary>
        /// Load all the maps whose type is present in the given script
        /// </summary>
        /// <param name="mapListScriptName">Name of the script including map types</param>
        public void LoadMaps(String mapListScriptName)
        {
            ScriptEngine.Instance.DoScript(mapListScriptName);
        }

        /// <summary>
        /// Load a map given its type and adds the map to server's world
        /// </summary>
        /// <param name="mapType">Type of the map to be loaded</param>
        public void LoadMap(String mapType)
        {
            try
            {
                SWorld.Instance.AddMap(mapType);
            }
            catch (Exception ex)
            {
                throw new Exception("Unable to load map " + mapType, ex);
            }
        }
    }
}
