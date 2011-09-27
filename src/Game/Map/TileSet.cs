using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class TileSet : BaseObject
    {
        List<String> TileTypes;

        public TileSet() :
            base()
        {
            TileTypes = new List<String>();
        }

        public TileSet(TileSet copy) :
            base(copy)
        {
            TileTypes = new List<String>(copy.TileTypes);
        }

        public override void ToScript()
        {
            Sw = new ScriptWriter(this);

            Sw.InitObject();

            base.ToScript();

            foreach (String tileType in TileTypes)
                Sw.WriteMethod("AddTileType", new String[] { ScriptWriter.GetStringOf(tileType) });

            Sw.EndObject();
        }

        public void AddTileType(String tileType)
        {
            if (tileType == null ||
                TileTypes.Contains(tileType))
                return;

            TileTypes.Add(tileType);
        }

        public Boolean RemoveTileType(String tileType)
        {
            return TileTypes.Remove(tileType);
        }

        public Tile GetTile(String tileType)
        {
            if (!TileTypes.Contains(tileType))
                return null;

            return Create.Tile(tileType);
        }

        public Tile GetTileAt(Int32 i)
        {
            return GetTile(GetAt(i));
        }

        public Int32 GetTileTypeCount()
        {
            return TileTypes.Count;
        }

        public String GetAt(Int32 i)
        {
            return TileTypes[i];
        }

        public String GetAt(Int32 x, Int32 y, Int32 width)
        {
            return GetAt(
                y * width +
                x % width);
        }
    }
}
