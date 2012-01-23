using System.Collections.Generic;

using SFML.Window;

namespace BlazeraLib
{
    public class OpacityBox
    {
        #region Enums

        public enum EDesactivatingSideType
        {
            None,
            All,
            Left,
            Top,
            Right,
            Bottom
        }

        #endregion Enums

        #region Constants

        const EDesactivatingSideType DEFAULT_DESACTIVATING_SIDE_TYPE = EDesactivatingSideType.None;

        #endregion Constants

        #region Members

        WorldElement Parent;
        FloatRect Rect;
        int Z;

        Dictionary<State<string>, bool> Sides;

        #endregion

        public OpacityBox(FloatRect rect, int z = BaseDrawable.DEFAULT_Z, EDesactivatingSideType desactivatingSideType = DEFAULT_DESACTIVATING_SIDE_TYPE)
        {
            Rect = rect;
            Z = z;

            Sides = new Dictionary<State<string>, bool>()
            {
                { "Left", false },
                { "Top", false },
                { "Right", false },
                { "Bottom", false }
            };

            Desactivate(desactivatingSideType);
        }

        public OpacityBox(OpacityBox copy)
        {
            Rect = new FloatRect(copy.Rect);

            Sides = new Dictionary<State<string>, bool>(copy.Sides);
        }

        public void SetParent(WorldElement parent)
        {
            Parent = parent;
        }

        public FloatRect GetRect()
        {
            return new FloatRect(
                Parent.Left + Rect.Left,
                Parent.Top + Rect.Top - GetZ() * GameData.TILE_SIZE,
                Parent.Left + Rect.Right,
                Parent.Top + Rect.Bottom - GetZ() * GameData.TILE_SIZE);
        }

        public int GetZ()
        {
            return Parent.Z + Z;
        }

        public void Desactivate(EDesactivatingSideType type)
        {
            switch (type)
            {
                case EDesactivatingSideType.None:
                    Sides["Left"] = true;
                    Sides["Top"] = true;
                    Sides["Right"] = true;
                    Sides["Bottom"] = true;
                    break;
                case EDesactivatingSideType.All:
                    Sides["Left"] = false;
                    Sides["Top"] = false;
                    Sides["Right"] = false;
                    Sides["Bottom"] = false;
                    break;
                case EDesactivatingSideType.Left:
                    Sides["Left"] = false;
                    Sides["Top"] = true;
                    Sides["Right"] = true;
                    Sides["Bottom"] = true;
                    break;
                case EDesactivatingSideType.Top:
                    Sides["Left"] = true;
                    Sides["Top"] = false;
                    Sides["Right"] = true;
                    Sides["Bottom"] = true;
                    break;
                case EDesactivatingSideType.Right:
                    Sides["Left"] = true;
                    Sides["Top"] = true;
                    Sides["Right"] = false;
                    Sides["Bottom"] = true;
                    break;
                case EDesactivatingSideType.Bottom:
                    Sides["Left"] = true;
                    Sides["Top"] = true;
                    Sides["Right"] = true;
                    Sides["Bottom"] = false;
                    break;
                default:
                    break;
            }
        }

        public List<OpacityWall> GetActiveWalls()
        {
            List<OpacityWall> activeWalls = new List<OpacityWall>();

            FloatRect boxRect = GetRect();

            Vector2f topLeft = new Vector2f(boxRect.Left, boxRect.Top);
            Vector2f topRight = new Vector2f(boxRect.Right, boxRect.Top);
            Vector2f bottomLeft = new Vector2f(boxRect.Left, boxRect.Bottom);
            Vector2f bottomRight = new Vector2f(boxRect.Right, boxRect.Bottom);

            if (Sides["Left"])
                activeWalls.Add(new OpacityWall(topLeft, bottomLeft));

            if (Sides["Top"])
                activeWalls.Add(new OpacityWall(topLeft, topRight));

            if (Sides["Bottom"])
                activeWalls.Add(new OpacityWall(bottomLeft, bottomRight));

            if (Sides["Right"])
                activeWalls.Add(new OpacityWall(topRight, bottomRight));

            return activeWalls;
        }
    }
}