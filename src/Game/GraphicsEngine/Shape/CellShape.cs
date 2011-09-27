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

    public class CellShape : BaseDrawableShape
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
            base()
        {
            Color = CellSelectionTypeColor[type];

            Dimension = new Vector2(size, size);

            SetEffect(ShapeEffect.Shade);

            Build();
        }

        protected override void Build()
        {
            BaseShape.AddPoint(new Vector2(0F, 0F), Color);
            GetShapeFromEffect(ShapeEffect.Shade).AddPoint(new Vector2(0F, 0F), EFFECT_BEGIN_COLOR);

            BaseShape.AddPoint(new Vector2(Dimension.X, 0F), Color);
            GetShapeFromEffect(ShapeEffect.Shade).AddPoint(new Vector2(Dimension.X, 0F), EFFECT_BEGIN_COLOR);

            BaseShape.AddPoint(Dimension, Color);
            GetShapeFromEffect(ShapeEffect.Shade).AddPoint(Dimension, EFFECT_END_COLOR);

            BaseShape.AddPoint(new Vector2(0F, Dimension.Y), Color);
            GetShapeFromEffect(ShapeEffect.Shade).AddPoint(new Vector2(0F, Dimension.Y), EFFECT_END_COLOR);
        }
    }
}
