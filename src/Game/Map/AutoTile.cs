using System.Collections.Generic;

namespace BlazeraLib
{
    /// <summary>
    /// Helper class for RMVX auto-tile pattern
    /// </summary>
    public class AutoTile
    {
        #region Enums

        public enum PartType
        {
            /// <summary>
            /// Opened on the right
            /// </summary>
            Right,
            /// <summary>
            /// Opened on the left
            /// </summary>
            Left,
            /// <summary>
            /// Opened on the top
            /// </summary>
            Top,
            /// <summary>
            /// Opened on the bottom
            /// </summary>
            Bottom,
            /// <summary>
            /// Opened on the left and the right
            /// </summary>
            LeftRight,
            /// <summary>
            /// Opened on the top and the bottom
            /// </summary>
            TopBottom,
            /// <summary>
            /// Opened on the top and the right
            /// </summary>
            TopRight,
            /// <summary>
            /// Opened on the bottom and the right
            /// </summary>
            BottomRight,
            /// <summary>
            /// Opened on the bottom and the left
            /// </summary>
            BottomLeft,
            /// <summary>
            /// Opened on the top and the left
            /// </summary>
            TopLeft,
            /// <summary>
            /// Closed on the left
            /// </summary>
            TopRightBottom,
            /// <summary>
            /// Closed on the right
            /// </summary>
            TopLeftBottom,
            /// <summary>
            /// Closed on the top
            /// </summary>
            LeftBottomRight,
            /// <summary>
            /// Closed on the bottom
            /// </summary>
            LeftTopRight,
            /// <summary>
            /// Opened everywhere
            /// </summary>
            All,
            /// <summary>
            /// Closed everywhere
            /// </summary>
            None
        }

        #endregion

        #region Members

        Texture BaseTexture;
        Dictionary<PartType, IDrawable> Textures;
        bool WallMode;

        #endregion

        public AutoTile(Texture baseTexture, bool wallMode = false)
        {
            Textures = new Dictionary<PartType, IDrawable>();

            BaseTexture = baseTexture;

            Build(wallMode);
        }

        public AutoTile(AutoTile copy) :
            this(new Texture(copy.BaseTexture))
        {

        }

        Texture GetTexture(IntRect rect)
        {
            const int WALL_OFFSET = 64;
            Vector2I wallOffset = new Vector2I();
            wallOffset.Y = WallMode ? WALL_OFFSET : 0;

            return new Texture(BaseTexture, rect + wallOffset);
        }

        //!\\ TODO: every tile on 32x32 to more easily merge
        void Build(bool wallMode)
        {
            WallMode = wallMode;

            Textures[PartType.All] = new ComposedTexture(
                GetTexture(new IntRect(32, 64, 48, 80)),
                GetTexture(new IntRect(16, 64, 32, 80)),
                GetTexture(new IntRect(32, 48, 48, 64)),
                GetTexture(new IntRect(16, 48, 32, 64)));
            Textures[PartType.None] = new ComposedTexture(
                GetTexture(new IntRect(0, 32, 16, 48)),
                GetTexture(new IntRect(48, 32, 64, 48)),
                GetTexture(new IntRect(0, 80, 16, 96)),
                GetTexture(new IntRect(48, 80, 64, 96)));

            Textures[PartType.Bottom] = new ComposedTexture(
                GetTexture(new IntRect(0, 32, 16, 64)),
                GetTexture(new IntRect(48, 32, 64, 64)),
                ComposedTexture.EPattern.Horizontal);
            Textures[PartType.Left] = new ComposedTexture(
                GetTexture(new IntRect(32, 32, 64, 48)),
                GetTexture(new IntRect(32, 80, 64, 96)),
                ComposedTexture.EPattern.Vertical);
            Textures[PartType.Top] = new ComposedTexture(
                GetTexture(new IntRect(0, 64, 16, 96)),
                GetTexture(new IntRect(48, 64, 64, 96)),
                ComposedTexture.EPattern.Horizontal);
            Textures[PartType.Right] = new ComposedTexture(
                GetTexture(new IntRect(0, 32, 32, 48)),
                GetTexture(new IntRect(0, 80, 32, 96)),
                ComposedTexture.EPattern.Vertical);

            Textures[PartType.LeftRight] = new ComposedTexture(
                GetTexture(new IntRect(16, 32, 48, 48)),
                GetTexture(new IntRect(16, 80, 48, 96)),
                ComposedTexture.EPattern.Vertical);
            Textures[PartType.TopBottom] = new ComposedTexture(
                GetTexture(new IntRect(0, 48, 16, 80)),
                GetTexture(new IntRect(48, 48, 64, 80)),
                ComposedTexture.EPattern.Horizontal);

            Textures[PartType.BottomLeft] = GetTexture(new IntRect(32, 32, 64, 64));
            Textures[PartType.BottomRight] = GetTexture(new IntRect(0, 32, 32, 64));
            Textures[PartType.TopLeft] = GetTexture(new IntRect(32, 64, 64, 96));
            Textures[PartType.TopRight] = GetTexture(new IntRect(0, 64, 32, 96));

            Textures[PartType.LeftBottomRight] = new ComposedTexture(
                GetTexture(new IntRect(32, 32, 48, 64)),
                GetTexture(new IntRect(16, 32, 32, 64)));
            Textures[PartType.LeftTopRight] = new ComposedTexture(
                GetTexture(new IntRect(32, 64, 48, 96)),
                GetTexture(new IntRect(16, 64, 32, 96)));
            Textures[PartType.TopLeftBottom] = new ComposedTexture(
                GetTexture(new IntRect(32, 64, 64, 80)),
                GetTexture(new IntRect(32, 48, 64, 64)), ComposedTexture.EPattern.Vertical);
            Textures[PartType.TopRightBottom] = new ComposedTexture(
                GetTexture(new IntRect(0, 64, 32, 80)),
                GetTexture(new IntRect(0, 48, 32, 64)), ComposedTexture.EPattern.Vertical);
        }

        IDrawable GetTexture(PartType partType)
        {
            return Textures[partType];
        }

        IDrawable GetNewTexture(PartType partType)
        {
            return (IDrawable)(GetTexture(partType).Clone());
        }

        IDrawable GetNewTexture(AutoTile.PartType partType, Vector2I position, float xOffset = 0F, float yOffset = 0F)
        {
            IDrawable texture = GetNewTexture(partType);
            texture.Position = (position.ToVector2() + new SFML.Window.Vector2f(xOffset, yOffset)) * GameData.TILE_SIZE;
            return texture;
        }

        public IDrawable GetSingle(int yOffset = 0)
        {
            return GetNewTexture(PartType.None, new Vector2I(0, yOffset));
        }

        public List<IDrawable> GetSingleLine(int length, int yOffset = 0)
        {
            List<IDrawable> textures = new List<IDrawable>();

            textures.Add(GetNewTexture(PartType.Right, new Vector2I(0, yOffset)));
            for (int l = 0; l < length - 2; ++l)
                textures.Add(GetNewTexture(PartType.LeftRight, new Vector2I(l, yOffset), 1F));
            textures.Add(GetNewTexture(PartType.Left, new Vector2I(length - 2, yOffset), 1F));

            return textures;
        }

        public List<IDrawable> GetSingleColumn(int length, int yOffset = 0)
        {
            List<IDrawable> textures = new List<IDrawable>();

            textures.Add(GetNewTexture(PartType.Bottom, new Vector2I(0, yOffset)));
            for (int l = 0; l < length - 2; ++l)
                textures.Add(GetNewTexture(PartType.TopBottom, new Vector2I(0, l + yOffset), 0F, 1F));
            textures.Add(GetNewTexture(PartType.Top, new Vector2I(0, length - 2 + yOffset), 0F, 1F));

            return textures;
        }

        public List<IDrawable> GetBeginLine(int length, int yOffset = 0)
        {
            List<IDrawable> textures = new List<IDrawable>();

            textures.Add(GetNewTexture(PartType.BottomRight, new Vector2I(0, yOffset)));
            for (int l = 0; l < length - 2; ++l)
                textures.Add(GetNewTexture(PartType.LeftBottomRight, new Vector2I(l, yOffset), 1F));
            textures.Add(GetNewTexture(PartType.BottomLeft, new Vector2I(length - 2, yOffset), 1F));

            return textures;
        }

        public List<IDrawable> GetEndLine(int length, int height, int yOffset = 0)
        {
            List<IDrawable> textures = new List<IDrawable>();

            textures.Add(GetNewTexture(PartType.TopRight, new Vector2I(0, yOffset + height - 2), 0F, 1F));
            for (int l = 0; l < length - 2; ++l)
                textures.Add(GetNewTexture(PartType.LeftTopRight, new Vector2I(l, yOffset + height - 2), 1F, 1F));
            textures.Add(GetNewTexture(PartType.TopLeft, new Vector2I(length - 2, yOffset + height - 2), 1F, 1F));

            return textures;
        }

        public List<IDrawable> GetMidleZone(int length, int height, int yOffset = 0)
        {
            List<IDrawable> textures = new List<IDrawable>();

            for (int h = 0; h < height - 2; ++h)
            {
                textures.Add(GetNewTexture(PartType.TopRightBottom, new Vector2I(0, h + yOffset), 0F, 1F));

                for (int l = 0; l < length - 2; ++l)
                    textures.Add(GetNewTexture(PartType.All, new Vector2I(l, h + yOffset), 1F, 1F));

                textures.Add(GetNewTexture(PartType.TopLeftBottom, new Vector2I(length - 2, h + yOffset), 1F, 1F));
            }

            return textures;
        }

        public List<IDrawable> GetArea(int length, int height, int yOffset = 0)
        {
            List<IDrawable> textures = new List<IDrawable>();

            textures.AddRange(GetBeginLine(length, yOffset));
            textures.AddRange(GetMidleZone(length, height, yOffset));
            textures.AddRange(GetEndLine(length, height, yOffset));

            return textures;
        }

        public IDrawable GetJoinPattern(PartType pattern)
        {
            return GetNewTexture(pattern);
        }
    }
}
