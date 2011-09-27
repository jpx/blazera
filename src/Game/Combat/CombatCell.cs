using System.Collections.Generic;
using SFML.Graphics;

namespace BlazeraLib
{
    public class CombatCell
    {
        #region Enums

        public enum EState
        {
            Empty,
            Unusable,
            Taken
        }

        #endregion

        #region Members

        public EState State { get; set; }

        public Vector2I Position { get; private set; }

        Dictionary<CellSelectionType, CellShape> ColorEffects;

        public List<BaseCombatant> Combatants { get; private set; }

        #endregion

        public CombatCell(Vector2I position)
        {
            Position = position;

            ColorEffects = new Dictionary<CellSelectionType, CellShape>();
            foreach (CellSelectionType type in System.Enum.GetValues(typeof(CellSelectionType)))
                ColorEffects.Add(type, null);

            Combatants = new List<BaseCombatant>();
        }

        public CellShape AddColorEffect(CellSelectionType type)
        {
            if (!IsUsable())
                return null;

            ColorEffects[type] = new CellShape(type, CombatMap.CELL_SIZE);
            ColorEffects[type].Position = Position.ToVector2() * CombatMap.CELL_SIZE;

            return ColorEffects[type];
        }

        public CellShape GetColorEffect(CellSelectionType type)
        {
            return ColorEffects[type];
        }

        public void AddCombatant(BaseCombatant combatant)
        {
            Combatants.Add(combatant);
        }

        public bool RemoveCombatant(BaseCombatant combatant)
        {
            return Combatants.Remove(combatant);
        }

        public bool IsUsable()
        {
            return State != EState.Unusable;
        }

        public int GetDistanceTo(Vector2I cellPosition)
        {
            return GetDistanceBetween(Position, cellPosition);
        }

        public int GetDistanceTo(CombatCell cell)
        {
            return GetDistanceBetween(this, cell);
        }

        public bool IsWithinArea(CellArea area, Vector2I centerCellPosition)
        {
            return IsWithinArea(this, area, centerCellPosition);
        }

        public static Vector2 GetPositionFromCellPosition(Vector2I cellPosition)
        {
            return cellPosition.ToVector2() * CombatMap.CELL_SIZE;
        }

        public static Vector2 GetCenterFromCellPosition(Vector2I cellPosition)
        {
            return (cellPosition.ToVector2() + new Vector2(.5F, .5F)) * CombatMap.CELL_SIZE;
        }

        public static int GetDistanceBetween(CombatCell cell1, CombatCell cell2)
        {
            return GetDistanceBetween(cell1.Position, cell2.Position);
        }

        public static int GetDistanceBetween(Vector2I cellPosition1, Vector2I cellPosition2)
        {
            return
                System.Math.Abs(cellPosition1.X - cellPosition2.X) +
                System.Math.Abs(cellPosition1.Y - cellPosition2.Y);
        }

        public static bool IsWithinArea(CombatCell cell, CellArea area, Vector2I centerCellPosition)
        {
            return area.ContainsCell(centerCellPosition, cell);
        }
    }
}
