using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Cell
    {
        public Boolean IsBlocking { get; set; }

        List<BBoundingBox> BBoundingBoxes;
        List<EBoundingBox> EBoundingBoxes;
        Dictionary<UInt32, Tile> TileLayers;

        public Cell()
        {
            IsBlocking = false;

            BBoundingBoxes = new List<BBoundingBox>();
            EBoundingBoxes = new List<EBoundingBox>();

            TileLayers = new Dictionary<UInt32, Tile>();
        }

        public Cell(Cell copy) :
            this()
        {
            IsBlocking = copy.IsBlocking;

            BBoundingBoxes = new List<BBoundingBox>();
            EBoundingBoxes = new List<EBoundingBox>();

            foreach (KeyValuePair<UInt32, Tile> tLayer in copy.TileLayers)
                TileLayers.Add(tLayer.Key, new Tile(tLayer.Value));

            Position = copy.Position;
        }

        public void Update(Time dt)
        {

        }

        public void Draw(RenderTarget window)
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

        private Vector2f _position;
        public Vector2f Position
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

        public void AddBoundingBox(BBoundingBox BB)
        {
            BBoundingBoxes.Add(BB);
        }

        public void RemoveBoundingBox(BBoundingBox BB)
        {
            BBoundingBoxes.Remove(BB);
        }

        public void AddBoundingBox(EBoundingBox BB)
        {
            EBoundingBoxes.Add(BB);
        }

        public void RemoveBoundingBox(EBoundingBox BB)
        {
            EBoundingBoxes.Remove(BB);
        }

        public IEnumerator<BBoundingBox> GetBBoundingBoxesEnumerator(Int32 z)
        {
            return BBoundingBoxes.FindAll((BB) =>
            { 
                if (BB.Z == z)
                    return true;

                return false;
            }).GetEnumerator();
        }

        public IEnumerator<EBoundingBox> GetEBoundingBoxesEnumerator(Int32 z)
        {
            return EBoundingBoxes.FindAll((BB) =>
            {
                if (BB.Z == z)
                    return true;

                return false;
            }).GetEnumerator();
        }
    }
}
