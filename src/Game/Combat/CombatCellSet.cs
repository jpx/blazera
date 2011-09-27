using System.Collections.Generic;

namespace BlazeraLib
{
    public class CombatCellSet
    {
        #region Constants

        const double UNUSABLE_STATE_LIMIT_FACTOR = 75D;

        #endregion

        #region Members

        int Width;
        int Height;
        CombatCell[,] CellSet;

        #endregion

        public CombatCellSet(CombatMap map)
        {
            Width = map.Width;
            Height = map.Height;

            BuildCells(map);
        }

        void BuildCells(CombatMap map)
        {
            CellSet = new CombatCell[Height, Width];
            for (int y = 0; y < Height; ++y)
                for (int x = 0; x < Width; ++x)
                    CellSet[y, x] = new CombatCell(new Vector2I(x, y));

            foreach (WorldObject wObj in map.Objects)
            {
                foreach (BBoundingBox BB in wObj.BBoundingBoxes)
                {
                    int
                        left    = (int)((BB.Left + ((1D - UNUSABLE_STATE_LIMIT_FACTOR) / 100D) * CombatMap.CELL_SIZE) / CombatMap.CELL_SIZE),
                        top     = (int)((BB.Top + ((1D - UNUSABLE_STATE_LIMIT_FACTOR) / 100D) * CombatMap.CELL_SIZE) / CombatMap.CELL_SIZE),
                        right   = (int)((BB.Right - ((1D - UNUSABLE_STATE_LIMIT_FACTOR) / 100D) * CombatMap.CELL_SIZE) / CombatMap.CELL_SIZE),
                        bottom  = (int)((BB.Bottom - ((1D - UNUSABLE_STATE_LIMIT_FACTOR) / 100D) * CombatMap.CELL_SIZE) / CombatMap.CELL_SIZE);

                    for (int y = top; y < bottom; ++y)
                        for (int x = left; x < right; ++x)
                            GetCell(x, y).State = CombatCell.EState.Unusable;
                }
            }
        }

        public CombatCell GetCell(int x, int y)
        {
            if (x < 0 ||
                y < 0 ||
                x >= Width ||
                y >= Height)
                return null;

            return CellSet[y, x];
        }

        public CombatCell GetCell(Vector2I cellPosition)
        {
            return GetCell(cellPosition.X, cellPosition.Y);
        }

        public List<CombatCell> GetCircle(Vector2I centerCellPosition, int minRange, int maxRange)
        {
            List<CombatCell> circle = new List<CombatCell>();

            if (minRange > maxRange)
                return circle;

            for (int y = centerCellPosition.Y - maxRange; y < centerCellPosition.Y + maxRange + 1; ++y)
            {
                for (int x = centerCellPosition.X - maxRange; x < centerCellPosition.X + maxRange + 1; ++x)
                {
                    if (GetCell(x, y) == null)
                        continue;

                    int distance = GetCell(x, y).GetDistanceTo(centerCellPosition);

                    if (distance < minRange ||
                        distance > maxRange)
                        continue;

                    circle.Add(GetCell(x, y));
                }
            }

            return circle;
        }
    }
}
