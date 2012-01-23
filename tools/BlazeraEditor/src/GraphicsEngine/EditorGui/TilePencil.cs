using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class TilePencil : Pencil
    {
        #region Singleton

        private static TilePencil _instance;
        public static TilePencil Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TilePencil();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        static readonly BlazeraLib.Texture ICON_TEXTURE = Create.Texture("Tile_TileSet11_22");

        Tile CurrentTile;
        UInt32 CurrentLayer;

        private TilePencil() :
            base(ICON_TEXTURE)
        {
            Name = "TilePencil";

            LockValue = (UInt32)GameData.TILE_SIZE;
        }

        public void SetCurrentTile(Tile currentTile)
        {
            CurrentTile = currentTile;
            SetCursorTexture(new BlazeraLib.Texture(CurrentTile.Texture));
        }

        public void SetCurrentLayer(UInt32 currentLayer)
        {
            CurrentLayer = currentLayer;
        }

        protected override void Empty()
        {
            base.Empty();

            CurrentTile = null;
        }

        protected override Boolean CanPaint(Vector2f point)
        {
            if (CurrentTile == null)
                return false;

            return base.CanPaint(point);
        }

        protected override Boolean Paint(Vector2f point)
        {
            Int32 x = (Int32)point.X / GameData.TILE_SIZE;
            Int32 y = (Int32)point.Y / GameData.TILE_SIZE;

            switch (Mode)
            {
                case EMode.Normal:

                    MapBox.Map.Ground.GetCell(x, y).SetTile(CurrentLayer, new Tile(CurrentTile));

                    return true;

                case EMode.Pot:

                    MapBox.Map.Ground.FillWithTile(CurrentLayer, CurrentTile);

                    return true;

                default:
                    return false;
            }
        }
    }
}
