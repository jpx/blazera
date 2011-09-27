using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class TileContainer : Container
    {
        public event ClickEventHandler Clicked;

        public Tile CurrentTile { get; private set; }

        public String GetCurrentTileType()
        {
            return CurrentTile.Type;
        }

        public Texture GetCurrentTexture()
        {
            return CurrentTile.Texture;
        }

        public TileContainer() :
            base() { }

        public TileContainer(String tileType) :
            base()
        {
            SetContent(tileType);
        }

        public TileContainer(Tile tile) :
            base()
        {
            SetContent(tile);
        }

        public void SetContent(String tileType)
        {
            SetContent(Create.Tile(tileType));
        }

        public void SetContent(Tile tile)
        {
            CurrentTile = tile;
            Content = new Button(tile.Texture, null);
            ((Button)Content).Clicked += new ClickEventHandler(TileContainer_Clicked);
        }

        void TileContainer_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (Clicked != null)
                Clicked(this, e);
        }
    }
}
