using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace BlazeraLib
{
    public class FileManager
    {
        private FileManager()
        {

        }

        public void Init()
        {

        }

        private static FileManager _instance;
        public static FileManager Instance
        {
            get
            {
                if (_instance == null)
                    FileManager.Instance = new FileManager();

                return _instance;
            }
            set { _instance = value; }
        }

        private void RemoveFile(String fileName)
        {
            String path = GameData.SCRIPTS_DEFAULT_PATH + fileName;

            try
            {
                
                File.Delete(path);
            }
            catch
            {
                Log.Clerr("Failed to remove file : " + path);
            }
        }

        private void RemoveScript(String scriptName)
        {
            RemoveFile(scriptName + ".lua");
        }

        public void RemoveMap(String mapType)
        {
            String mapScriptName = "Map/Map_" + mapType;
            String groundScriptName = "Ground/Ground_" + mapType;

            RemoveFile(groundScriptName);
            RemoveScript(groundScriptName);
            RemoveScript(mapScriptName);
        }

        public void RemoveObject(String baseType, String type)
        {
            String objectScriptName = baseType + "/" + baseType + "_" + type;

            RemoveScript(objectScriptName);
        }

        public void RemoveTexture(String type)
        {
            String textureScriptName = "Texture/Texture_" + type;

            RemoveScript(textureScriptName);
        }

        public void RemoveTile(String type)
        {
            String tileScriptName = "Tile/Tile_" + type;

            RemoveScript(tileScriptName);
        }

        public void RemoveTileSet(String type)
        {
            String tileSetScriptName = "TileSet/TileSet_" + type;

            RemoveScript(tileSetScriptName);
        }
    }
}
