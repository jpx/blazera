using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Ground : BaseObject
    {
        Cell[,] CellSet;

        public Ground() :
            base()
        {
            
        }

        public Ground(Ground copy) :
            base(copy)
        {
            Width = copy.Width;
            Height = copy.Height;
            CellSet = new Cell[Height, Width];
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    SetCell(x, y, new Cell(copy.GetCell(x, y)));
                }
            }
        }

        public override void SetType(String type, Boolean creation = false)
        {
 	        base.SetType(type);

            if (!creation)
                Generate();
        }

        public void Init(Int32 width, Int32 height)
        {
            Width = width;
            Height = height;

            CellSet = new Cell[Height, Width];

            for (Int32 y = 0; y < Height; ++y)
                for (Int32 x = 0; x < Width; ++x)
                    SetCell(x, y, new Cell());
        }

        public Cell GetCell(Int32 x, Int32 y)
        {
            if (x >= 0 && x < Width &&
                y >= 0 && y < Height)
                return CellSet[y, x];

            return new Cell() { IsBlocking = true };
        }

        public void SetCell(Int32 x, Int32 y, Cell cell)
        {
            cell.Position = new Vector2f(
                GameDatas.TILE_SIZE * x,
                GameDatas.TILE_SIZE * y);
            CellSet[y, x] = cell;
        }

        public override void ToScript()
        {
            this.Sw = new ScriptWriter(this);

            this.Sw.InitObject();

            base.ToScript();

            this.Sw.WriteMethod("Init", new String[]
            {
                this.Width.ToString(),
                this.Height.ToString()
            });

            for (Int32 y = 0; y < this.Height; y++)
                for (Int32 x = 0; x < this.Width; x++)
                    if (this.GetBlock(x, y))
                        this.Sw.WriteMethod("AddBlock", new String[]
                        {
                            x.ToString(),
                            y.ToString()
                        });
            
            this.Sw.EndObject();

            ScriptWriter.WriteToFile("Ground/Ground_" + this.Type, GetTileSetStr());
        }

        #region GroundFile

        String GetTileSetStr()
        {
            Char[] tileSet = new Char[this.Width * this.Height * 20];
            int tot = 0;

            for (Int32 y = 0; y < this.Height; y++)
            {
                for (Int32 x = 0; x < this.Width; x++)
                {
                    if (GetCell(x, y) == null || GetCell(x, y).GetTileCount() == 0)
                    {
                        tileSet[tot] = ';';
                        tot++;
                        continue;
                    }

                    IEnumerator<UInt32> tileLayers = GetCell(x, y).GetLayersEnumerator();
                    while (tileLayers.MoveNext())
                    {
                        UInt32 l = tileLayers.Current;
                        Tile tile = GetCell(x, y).GetTile(l);

                        Int32 m = tot + tile.Type.Length;
                        for (Int32 i = tot; i < m; i++)
                            tileSet[i] = tile.Type[i - tot];

                        if (l < GetCell(x, y).GetTileCount() - 1)
                            tileSet[m] = ',';

                        tot += tile.Type.Length + 1;
                    }

                    tileSet[tot - 1] = ';';
                }
            }

            return new String(tileSet, 0, tot);
        }

        public void Generate()
        {
            List<List<String>> ground = FileReader.Instance.GetGround(Type);

            if (ground == null ||
                ground.Count < Width * Height)
                return;

            for (Int32 y = 0; y < Height; y++)
                for (Int32 x = 0; x < Width; x++)
                {
                    Int32 id =
                        y * Width +
                        x % Width;

                    for (Int32 l = 0; l < ground[id].Count; l++)
                        GetCell(x, y).SetTile((UInt32)l, Create.Tile(ground[id][l]));
                }
        }

        #endregion

        public void Update(Time dt)
        {

        }

        public void Draw(RenderWindow window)
        {
            int left = (int)(window.GetView().Center.X - window.GetView().Size.X / 2) / GameDatas.TILE_SIZE - GameDatas.GROUND_DRAW_MARGIN;
            int top = (int)(window.GetView().Center.Y - window.GetView().Size.Y / 2) / GameDatas.TILE_SIZE - GameDatas.GROUND_DRAW_MARGIN;
            int right = (int)(window.GetView().Center.X + window.GetView().Size.X / 2) / GameDatas.TILE_SIZE + GameDatas.GROUND_DRAW_MARGIN;
            int bottom = (int)(window.GetView().Center.Y + window.GetView().Size.Y / 2) / GameDatas.TILE_SIZE + GameDatas.GROUND_DRAW_MARGIN;

            int minLeft = 0;
            int minTop = 0;
            int maxRight = this.Width;
            int maxBottom = this.Height;

            if (left < minLeft)
                left = minLeft;
            if (top < minTop)
                top = minTop;
            if (right > maxRight)
                right = maxRight;
            if (bottom > maxBottom)
                bottom = maxBottom;

            for (int line = top; line < bottom; ++line)
                for (int column = left; column < right; ++column)
                    GetCell(column, line).Draw(window);
        }

        public Int32 Width;
        public Int32 Height;

        public void FillWithTile(UInt32 layer, Tile tile)
        {
            for (Int32 y = 0; y < Height; ++y)
                for (Int32 x = 0; x < Width; ++x)
                    GetCell(x, y).SetTile(layer, new Tile(tile));
        }

        public void AddBlock(Int32 x, Int32 y)
        {
            GetCell(x, y).IsBlocking = true;
        }

        public void RemoveBlock(Int32 x, Int32 y)
        {
            GetCell(x, y).IsBlocking = false;
        }

        public Boolean GetBlock(Int32 x, Int32 y)
        {
            return GetCell(x, y).IsBlocking;
        }

        public void AddObjectBoundingBoxes(WorldObject obj)
        {
            foreach (BBoundingBox BB in obj.BBoundingBoxes)
                AddBoundingBox(BB);

            foreach (EBoundingBox BB in obj.BodyBoundingBoxes)
                AddBoundingBox(BB);

            foreach (EventBoundingBoxType BBT in Enum.GetValues(typeof(EventBoundingBoxType)))
                foreach (EBoundingBox BB in obj.EventBoundingBoxes[BBT])
                    AddBoundingBox(BB);
        }

        private void AddBoundingBox(BBoundingBox BB)
        {
            for (int y = BB.TTop; y < BB.TBottom + 1; ++y)
                for (int x = BB.TLeft; x < BB.TRight + 1; ++x)
                    GetCell(x, y).AddBoundingBox(BB.Z, BB);
        }

        private void AddBoundingBox(EBoundingBox BB)
        {
            for (int y = BB.TTop; y < BB.TBottom + 1; ++y)
                for (int x = BB.TLeft; x < BB.TRight + 1; ++x)
                    GetCell(x, y).AddBoundingBox(BB.Z, BB);
        }

        public void RemoveObjectBoundingBoxes(WorldObject obj)
        {
            foreach (BBoundingBox BB in obj.BBoundingBoxes)
                RemoveBoundingBox(BB);

            foreach (EBoundingBox BB in obj.BodyBoundingBoxes)
                RemoveBoundingBox(BB);

            foreach (EventBoundingBoxType BBT in Enum.GetValues(typeof(EventBoundingBoxType)))
                foreach (EBoundingBox BB in obj.EventBoundingBoxes[BBT])
                    RemoveBoundingBox(BB);
        }

        private void RemoveBoundingBox(BBoundingBox BB)
        {
            for (int y = BB.TTop; y < BB.TBottom + 1; ++y)
                for (int x = BB.TLeft; x < BB.TRight + 1; ++x)
                    GetCell(x, y).RemoveBoundingBox(BB.Z, BB);
        }

        private void RemoveBoundingBox(EBoundingBox BB)
        {
            for (int y = BB.TTop; y < BB.TBottom + 1; ++y)
                for (int x = BB.TLeft; x < BB.TRight + 1; ++x)
                    GetCell(x, y).RemoveBoundingBox(BB.Z, BB);
        }

        public IEnumerator<BBoundingBox> GetBBoundingBoxesEnumerator(Int32 x, Int32 y, Int32 z)
        {
            return GetCell(x, y).GetBBoundingBoxesEnumerator(z);
        }

        public IEnumerator<EBoundingBox> GetEBoundingBoxesEnumerator(Int32 x, Int32 y, Int32 z)
        {
            return GetCell(x, y).GetEBoundingBoxesEnumerator(z);
        }

        public Boolean CanFit(WorldObject wObj, Vector2f position)
        {
            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                IntRect rect = BB.GetNextTRect(position);

                for (int y = rect.Top; y < rect.Bottom + 1; ++y)
                {
                    for (int x = rect.Left; x < rect.Right + 1; ++x)
                    {
                        if (GetBlock(x, y))
                        {
                            return false;
                        }

                        IEnumerator<BBoundingBox> mapBBsEnum = GetBBoundingBoxesEnumerator(x, y, 0);
                        while (mapBBsEnum.MoveNext())
                        {
                            if (BB.Holder == mapBBsEnum.Current.Holder)
                                continue;

                            if (!BB.BoundingBoxTest(mapBBsEnum.Current, position))
                                continue;

                            return false;
                        }
                    }
                }
            }

            return true;
        }
    }
}
