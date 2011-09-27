using System.Collections.Generic;

namespace BlazeraLib
{
    #region Enums

    public enum CellAreaType
    {
        Point,
        Circle,
        Cross,
        Square,
        Line
    }

    #endregion

    public class CellArea
    {
        #region Constants

        const CellAreaType DEFAULT_TYPE = CellAreaType.Point;
        const int DEFAULT_MIN_RANGE = 1;
        const int DEFAULT_MAX_RANGE = 1;

        #endregion

        #region Members

        CellAreaType Type;

        int MinRange;
        int MaxRange;

        Vector2I CenterCellPosition;

        List<Vector2I> CellPositions;

        #endregion

        public CellArea(CellAreaType type = DEFAULT_TYPE, int minRange = DEFAULT_MIN_RANGE, int maxRange = DEFAULT_MAX_RANGE)
        {
            Type = type;

            MinRange = minRange;
            MaxRange = maxRange;

            CenterCellPosition = new Vector2I();

            CellPositions = new List<Vector2I>();

            Build();
        }

        public CellArea(CellArea copy)
        {
            Type = copy.Type;

            MinRange = copy.MinRange;
            MaxRange = copy.MaxRange;

            CenterCellPosition = new Vector2I(copy.CenterCellPosition);

            CellPositions = new List<Vector2I>();

            Build();
        }

        void Build()
        {
            CellPositions.Clear();

            if (MinRange > MaxRange)
                return;

            switch (Type)
            {
                case CellAreaType.Point:

                    CellPositions.Add(CenterCellPosition);

                    break;

                case CellAreaType.Circle:

                    for (int y = CenterCellPosition.Y - MaxRange; y < CenterCellPosition.Y + MaxRange + 1; ++y)
                    {
                        for (int x = CenterCellPosition.X - MaxRange; x < CenterCellPosition.X + MaxRange + 1; ++x)
                        {
                            int distance = CombatCell.GetDistanceBetween(CenterCellPosition, new Vector2I(x, y));

                            if (distance < MinRange ||
                                distance > MaxRange)
                                continue;

                            CellPositions.Add(new Vector2I(x, y));
                        }
                    }

                    break;

                case CellAreaType.Cross:

                    for (int x = CenterCellPosition.X - MaxRange; x < CenterCellPosition.X + MaxRange + 1; ++x)
                    {
                        int distance = CombatCell.GetDistanceBetween(CenterCellPosition, new Vector2I(x, CenterCellPosition.Y));

                        if (distance < MinRange ||
                            distance > MaxRange)
                            continue;

                        CellPositions.Add(new Vector2I(x, CenterCellPosition.Y));
                    }

                    for (int y = CenterCellPosition.Y - MaxRange; y < CenterCellPosition.Y + MaxRange + 1; ++y)
                    {
                        int distance = CombatCell.GetDistanceBetween(CenterCellPosition, new Vector2I(CenterCellPosition.X, y));

                        if (distance < MinRange ||
                            distance > MaxRange)
                            continue;

                        CellPositions.Add(new Vector2I(CenterCellPosition.X, y));
                    }

                    break;

                case CellAreaType.Square:

                    for (int y = CenterCellPosition.Y - MaxRange; y < CenterCellPosition.Y + MaxRange + 1; ++y)
                    {
                        for (int x = CenterCellPosition.X - MaxRange; x < CenterCellPosition.X + MaxRange + 1; ++x)
                        {
                            int distance = CombatCell.GetDistanceBetween(CenterCellPosition, new Vector2I(x, y));

                            if (distance < MinRange)
                                continue;

                            CellPositions.Add(new Vector2I(x, y));
                        }
                    }

                    break;
            }
        }

        public void SetCenterCellPosition(Vector2I centerCellPosition)
        {
            Vector2I offset = centerCellPosition - CenterCellPosition;

            CenterCellPosition = centerCellPosition;

            for (int count = 0; count < CellPositions.Count; ++count)
                CellPositions[count] += offset;
        }

        public bool ContainsCellPosition(Vector2I centerCellPosition, Vector2I cellPosition)
        {
            if (centerCellPosition != null)
                SetCenterCellPosition(centerCellPosition);

            foreach (Vector2I cellPos in CellPositions)
                if (cellPos == cellPosition)
                    return true;

            return false;
        }

        public bool ContainsCell(Vector2I centerCellPosition, CombatCell cell)
        {
            return ContainsCellPosition(centerCellPosition, cell.Position);
        }

        public List<Vector2I> GetAreaCellPositions(Vector2I centerCellPosition)
        {
            SetCenterCellPosition(centerCellPosition);

            return CellPositions;
        }

        public Vector2I GetRandomCellPosition(Vector2I centerCellPosition)
        {
            return CellPositions[RandomHelper.Get(0, CellPositions.Count - 1)];
        }
    }
}
