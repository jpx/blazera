using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class TileSelector : Widget
    {
        const Int32 DEFAULT_SIZE = 4;

        const float DEFAULT_EMPTY_VSCROLLBAR_HEIGHT = 3F;
        const float DEFAULT_EMPTY_HSCROLLBAR_WIDTH = 3F;

        Int32 Width;
        Int32 Height;

        VAutoSizeBox VMainBox;
        HAutoSizeBox HMidleBox;

        MultiBox MultiBox;

        VScrollBar VScrollBar;
        HScrollBar HScrollBar;

        TileSet CurrentTileSet;

        public TileSelector(Int32 width = DEFAULT_SIZE, Int32 height = DEFAULT_SIZE) :
            base()
        {
            Width = width;
            Height = height;

            VMainBox = new VAutoSizeBox(false);
            HMidleBox = new HAutoSizeBox();

            MultiBox = new MultiBox(Alignment.Vertical, Height, Width);
            MultiBox.SetSize(Width);
            MultiBox.OnVPointerChange += new PointerChangeEventHandler(MultiBox_OnVPointerChange);
            MultiBox.OnHPointerChange += new PointerChangeEventHandler(MultiBox_OnHPointerChange);

            VScrollBar = new VScrollBar();
            VScrollBar.Scrolled += new ScrollEventHandler(VScrollBar_Scrolled);
            HScrollBar = new HScrollBar();
            HScrollBar.Scrolled += new ScrollEventHandler(HScrollBar_Scrolled);

            AddWidget(VMainBox);

            VMainBox.AddItem(HMidleBox, 0, HAlignment.Right);
            HMidleBox.AddItem(MultiBox);
            HMidleBox.AddItem(VScrollBar);

            VMainBox.AddItem(HScrollBar);
        }

        public void ResetWidth()
        {
            MultiBox.SetSize(Width);
        }

        void MultiBox_OnHPointerChange(Widget sender, PointerChangeEventArgs e)
        {
            RefreshScrollBars();
            HScrollBar.Scroll(e.Value - HScrollBar.CursorPosition);
        }

        void MultiBox_OnVPointerChange(Widget sender, PointerChangeEventArgs e)
        {
            RefreshScrollBars();
            VScrollBar.Scroll(e.Value - VScrollBar.CursorPosition);
        }

        void HScrollBar_Scrolled(object sender, ScrollEventArgs e)
        {
            MultiBox.SetCurrentHPointer(HScrollBar.CursorPosition);
        }

        void VScrollBar_Scrolled(object sender, ScrollEventArgs e)
        {
            MultiBox.SetCurrentVPointer(VScrollBar.CursorPosition);
        }

        void SetTileSet(TileSet tileSet, Int32 tileSetWidth, ClickEventHandler onClick = null)
        {
            if (tileSet == null)
                return;

            MultiBox.SetSize(tileSetWidth);

            CurrentTileSet = tileSet;

            List<Tile> tiles = new List<Tile>();

            for (Int32 count = 0; count < tileSet.GetTileTypeCount(); ++count)
                tiles.Add(tileSet.GetTileAt(count));

            SetTileSet(tiles, tileSetWidth, onClick);
        }

        public void SetTileSet(List<Tile> tiles, Int32 tileSetWidth, ClickEventHandler onClick = null)
        {
            Clear();

            MultiBox.SetSize(tileSetWidth);

            AddTile(tiles, onClick);

            RefreshScrollBars();
        }

        void RefreshScrollBars()
        {
            HScrollBar.SetValues(Width, MultiBox.GetSize());
            VScrollBar.SetValues(Height, MultiBox.GetRowCount());
        }

        public void SetTileSet(TileSet tileSet, ClickEventHandler onClick = null)
        {
            SetTileSet(tileSet, Width, onClick);
        }

        public void AddTile(Tile tile, ClickEventHandler onClick = null)
        {
            TileContainer tileContainer = new TileContainer(tile);
            tileContainer.Clicked += onClick;

            MultiBox.AddItem(tileContainer);

            RefreshScrollBars();
        }

        public void AddTile(List<Tile> tiles, ClickEventHandler onClick = null)
        {
            List<Widget> tileContainers = new List<Widget>();

            foreach (Tile tile in tiles)
            {
                TileContainer tileContainer = new TileContainer(tile);
                tileContainer.Clicked += onClick;

                tileContainers.Add(tileContainer);
            }

            MultiBox.AddItem(tileContainers);
        }

        public Boolean RemoveTile(TileContainer tileContainer)
        {
            if (!MultiBox.RemoveItem(tileContainer))
                return false;

            RefreshScrollBars();

            Init();

            return true;
        }

        public override void Refresh()
        {
            base.Refresh();

            if (VScrollBar == null || HScrollBar == null)
                return;

            if (MultiBox.GetItemCount() == 0)
            {
                VScrollBar.Dimension = new Vector2f(
                    VScrollBar.Dimension.X,
                    DEFAULT_EMPTY_VSCROLLBAR_HEIGHT);

                HScrollBar.Dimension = new Vector2f(
                    DEFAULT_EMPTY_HSCROLLBAR_WIDTH,
                    HScrollBar.Dimension.Y);
            }

            VScrollBar.Dimension = new Vector2f(
                VScrollBar.Dimension.X,
                MultiBox.BackgroundDimension.Y);

            HScrollBar.Dimension = new Vector2f(
                MultiBox.BackgroundDimension.X,
                HScrollBar.Dimension.Y);
        }

        public override Vector2f Dimension
        {
            get
            {
                if (VMainBox == null)
                    return base.Dimension;

                return VMainBox.BackgroundDimension;
            }
        }

        public void Clear()
        {
            MultiBox.Clear();

            RefreshScrollBars();

            Refresh();
        }
    }
}
