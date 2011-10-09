using System.Collections.Generic;
using SFML.Graphics;

namespace BlazeraLib
{
    public enum CellSelectionType
    {
        Move,
        MovePath,
        AttackRange,
        AttackArea,
        SpellRange,
        SpellArea,
        OutOfRange
    }

    public class CellShape : RectangleShape
    {
        #region Constants

        static readonly Dictionary<CellSelectionType, Color> CellSelectionTypeColor = new Dictionary<CellSelectionType, Color>()
        {
            { CellSelectionType.Move, Color.Green },
            { CellSelectionType.MovePath, Color.Blue },
            { CellSelectionType.AttackRange, Color.Blue },
            { CellSelectionType.AttackArea, Color.Red },
            { CellSelectionType.SpellRange, Color.Blue },
            { CellSelectionType.SpellArea, Color.Red },
            { CellSelectionType.OutOfRange, Graphics.Color.GetColorFromName(Graphics.Color.ColorName.Grey).ToSFColor() }
        };

        #endregion

        #region Members

        #endregion

        public CellShape(CellSelectionType type, uint size) :
            base(new Vector2(size, size), CellSelectionTypeColor[type], false, Color.Black)
        {

        }
    }
}
