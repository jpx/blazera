using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class Cell
    {
        const Int32 DEFAULT_ZDIMENSION = 1;
        const Int32 DEFAULT_MINZ = 0;

        public Boolean IsBlocking { get; set; }

        public Int32 ZDimension { get; private set; }

        public Int32 MinZ { get; private set; }

        Dictionary<Int32, List<BBoundingBox>> BBoundingBoxes;
        Dictionary<Int32, List<EBoundingBox>> EBoundingBoxes;
        Dictionary<UInt32, Tile> TileLayers;

        public Cell()
        {
            Init(DEFAULT_MINZ, DEFAULT_ZDIMENSION);

            IsBlocking = false;

            TileLayers = new Dictionary<UInt32, Tile>();
        }

        public Cell(Cell copy) :
            this()
        {
            IsBlocking = copy.IsBlocking;

            Init(copy.MinZ, copy.ZDimension);

            foreach (KeyValuePair<UInt32, Tile> tLayer in copy.TileLayers)
                this.TileLayers.Add(tLayer.Key, new Tile(tLayer.Value));

            this.Position = copy.Position;
        }

        void Init(Int32 minZ, Int32 zDimension)
        {
            ZDimension = zDimension;

            MinZ = minZ;

            BBoundingBoxes = new Dictionary<Int32, List<BBoundingBox>>();

            EBoundingBoxes = new Dictionary<Int32, List<EBoundingBox>>();

            for (Int32 z = MinZ; z < ZDimension; ++z)
            {
                BBoundingBoxes[z] = new List<BBoundingBox>();

                EBoundingBoxes[z] = new List<EBoundingBox>();
            }
        }

        public void Update(Time dt)
        {

        }

        public void Draw(RenderWindow window)
        {
            foreach (Tile tile in TileLayers.Values)
                tile.Draw(window);
        }

        public Tile GetTile(UInt32 layer)
        {
            if (TileLayers.ContainsKey(layer))
                return TileLayers[layer];

            return null;
        }

        public Int32 GetTileCount()
        {
            return TileLayers.Count;
        }

        public void SetTile(UInt32 layer, Tile tile)
        {
            if (tile == null)
                return;

            tile.Position = Position;

            if (TileLayers.ContainsKey(layer))
                TileLayers[layer] = tile;
            else
                TileLayers.Add(layer, tile);
        }

        private Vector2 _position;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;

                foreach (Tile tile in TileLayers.Values)
                    tile.Position = Position;
            }
        }

        public IEnumerator<UInt32> GetLayersEnumerator()
        {
            return TileLayers.Keys.GetEnumerator();
        }

        public void AddBoundingBox(Int32 z, BBoundingBox BB)
        {
            BBoundingBoxes[z].Add(BB);
        }

        public void RemoveBoundingBox(Int32 z, BBoundingBox BB)
        {
            BBoundingBoxes[z].Remove(BB);
        }

        public void AddBoundingBox(Int32 z, EBoundingBox BB)
        {
            EBoundingBoxes[z].Add(BB);
        }

        public void RemoveBoundingBox(Int32 z, EBoundingBox BB)
        {
            EBoundingBoxes[z].Remove(BB);
        }

        public IEnumerator<BBoundingBox> GetBBoundingBoxesEnumerator(Int32 z)
        {
            return this.BBoundingBoxes[z].GetEnumerator();
        }

        public IEnumerator<EBoundingBox> GetEBoundingBoxesEnumerator(Int32 z)
        {
            return this.EBoundingBoxes[z].GetEnumerator();
        }
    }
}
