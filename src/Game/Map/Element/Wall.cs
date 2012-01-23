using System.Collections.Generic;

namespace BlazeraLib
{
    public class Wall : WorldElement
    {
        #region Constants

        protected const int DEFAULT_WIDTH = 1;
        protected const int DEFAULT_HEIGHT = 1;
        protected const int DEFAULT_HIGHNESS = 1;

        protected const int SUMMIT_BB_HALF_WIDTH = 1;

        #endregion

        #region Members

        /// <summary>
        /// Base texture for auto-tile pattern
        /// </summary>
        Texture BaseTexture;
        /// <summary>
        /// Auto-tile for the summit platform of the wall (horizontal part)
        /// </summary>
        AutoTile AutoTile;
        /// <summary>
        /// Auto-tile for the facing side of the wall (vertical part)
        /// </summary>
        AutoTile WallAutoTile;
        /// <summary>
        /// List containing each tile of the wall
        /// </summary>
        List<IDrawable> Textures;

        IDrawable[,] GroundTextures;
        IDrawable[,] WallTextures;

        /// <summary>
        /// Length of the wall in tile
        /// </summary>
        protected int Width { get; private set; }
        /// <summary>
        /// Deepness of the wall in tile
        /// </summary>
        protected int Height { get; private set; }
        /// <summary>
        /// Highness of the wall in tile
        /// </summary>
        protected int Highness { get; private set; }

        #endregion

        public Wall() :
            base()
        {
            Textures = new List<IDrawable>();
        }

        public Wall(Wall copy) :
            base(copy)
        {
            
        }

        public override object Clone()
        {
            return new Wall(this);
        }

        public override void ToScript()
        {
            Sw = new ScriptWriter(this);

            Sw.InitObject();

            base.ToScript();

            Sw.WriteMethod("SetBase", new string[]
            {
                "Create:Texture(" + ScriptWriter.GetStringOf(BaseTexture.Type) + ")",
                Width.ToString(),
                Height.ToString(),
                Highness.ToString()
            });

            Sw.EndObject();
        }

        public override void Draw(SFML.Graphics.RenderTarget window)
        {
            foreach (IDrawable drawableTexture in Textures)
                drawableTexture.Draw(window);
        }

        public override SFML.Window.Vector2f Position
        {
            set
            {
                SFML.Window.Vector2f offset = value - Position;

                base.Position = value;

                if (Textures == null)
                    return;

                foreach (IDrawable drawableTexture in Textures)
                    drawableTexture.Position += offset;
            }
        }

        public override SFML.Window.Vector2f Dimension
        {
            get
            {
                return new SFML.Window.Vector2f(Width * GameData.TILE_SIZE, Height * GameData.TILE_SIZE);
            }
        }

        public override int Z
        {
            set
            {
                int offset = value - Z;

                base.Z = value;

                if (Textures == null)
                    return;

                foreach (IDrawable drawableTexture in Textures)
                    drawableTexture.Position -= new SFML.Window.Vector2f(0F, offset * GameData.TILE_SIZE);
            }
        }

        public override Texture GetSkinTexture()
        {
            return BaseTexture;
        }

        public void SetBase(Texture baseTexture, int width = DEFAULT_WIDTH, int height = DEFAULT_HEIGHT, int highness = DEFAULT_HIGHNESS)
        {
            BaseTexture = baseTexture;

            AutoTile = new AutoTile(BaseTexture);
            WallAutoTile = new AutoTile(BaseTexture, true);

            Width = width;
            Height = height;
            Highness = highness;

            GroundTextures = new IDrawable[Height, Width];
            WallTextures = new IDrawable[Highness, Width];

            Build();
        }

        Vector2I GetGlobalFromLocal(Vector2I point, bool wallMode = false)
        {
            return point - new Vector2I(0, Highness) + (wallMode ? new Vector2I(0, Height) : new Vector2I());
        }

        int GetYOffset(bool wallMode = false)
        {
            return -(Highness) + (wallMode ? Height : 0);
        }

        public override int H
        {
            get { return Highness; }
        }

        protected virtual void BuildBoundingBoxes()
        {
            for (int z = 0; z <= H; ++z)
            {
                AddBoundingBox(new BBoundingBox(
                    this,
                    0,
                    0,
                    Width * GameData.TILE_SIZE,
                    Height * GameData.TILE_SIZE,
                    z));
            }
        }

        void BuildOpacityBoxes()
        {
            for (int z = 0; z < H; ++z)
            {
                AddOpacityBox(new OpacityBox(new FloatRect(
                    0,
                    0,
                    Width * GameData.TILE_SIZE,
                    Height * GameData.TILE_SIZE),
                    z,
                    OpacityBox.EDesactivatingSideType.Bottom));
            }

            AddOpacityBox(new OpacityBox(new FloatRect(
                    0,
                    0,
                    Width * GameData.TILE_SIZE,
                    Height * GameData.TILE_SIZE),
                    H));
        }

        //!\\ TODO
        void Clear()
        {
            BBoundingBoxes.Clear();
            Textures.Clear();
        }

        /// <summary>
        /// Builds the bounding boxes and the texture of the wall
        /// </summary>
        void Build()
        {
            BuildBoundingBoxes();

            BuildOpacityBoxes();

            if (Width == 1 &&
                Height == 1 &&
                Highness == 1)
            {
                AddTexture(AutoTile.GetSingle(GetYOffset()));
                AddTexture(WallAutoTile.GetSingle(GetYOffset(true)));
                return;
            }

            if (Width == 1 &&
                Height == 1)
            {
                AddTexture(AutoTile.GetSingle(GetYOffset()));
                AddTextureRange(WallAutoTile.GetSingleColumn(Highness, GetYOffset(true)));
                return;
            }

            if (Width == 1 &&
                Highness == 1)
            {
                AddTextureRange(AutoTile.GetSingleColumn(Height, GetYOffset()));
                AddTexture(WallAutoTile.GetSingle(GetYOffset(true)));
                return;
            }

            if (Height == 1 &&
                Highness == 1)
            {
                AddTextureRange(AutoTile.GetSingleLine(Width, GetYOffset()));
                AddTextureRange(WallAutoTile.GetSingleLine(Width, GetYOffset(true)));
                return;
            }

            if (Width == 1)
            {
                AddTextureRange(AutoTile.GetSingleColumn(Height, GetYOffset()));
                AddTextureRange(WallAutoTile.GetSingleColumn(Highness, GetYOffset(true)));
                return;
            }

            if (Height == 1)
            {
                AddTextureRange(AutoTile.GetSingleLine(Width, GetYOffset()));
                AddTextureRange(WallAutoTile.GetArea(Width, Highness, GetYOffset(true)));
                return;
            }

            if (Highness == 1)
            {
                AddTextureRange(AutoTile.GetArea(Width, Height, GetYOffset()));
                AddTextureRange(WallAutoTile.GetSingleLine(Width, GetYOffset(true)));
                return;
            }

            AddTextureRange(AutoTile.GetArea(Width, Height, GetYOffset()));
            AddTextureRange(WallAutoTile.GetArea(Width, Highness, GetYOffset(true)));
        }

        public override FloatRect GetVisibleRect()
        {
            FloatRect baseRect = base.GetVisibleRect();

            return new FloatRect(
                baseRect.Left,
                baseRect.Top - Highness * GameData.TILE_SIZE,
                baseRect.Right,
                baseRect.Bottom);
        }

        //!\\ TODO: change representation of tiles
        void RemoveTexture(IDrawable tile)
        {
            if (!Textures.Remove(tile))
                return;

            Vector2I position = Vector2I.FromVector2(tile.Position - Position) / GameData.TILE_SIZE;

            int groundWallBoundary = -Summit + Height;

            bool isWall = false;
            if (position.Y >= groundWallBoundary)
            {
                position.Y -= groundWallBoundary;
                isWall = true;
            }
            else
            {
                position.Y += Summit;
            }

            if (isWall)
                WallTextures[position.Y, position.X] = null;
            else
                GroundTextures[position.Y, position.X] = null;
        }

        //!\\ TODO
        void AddTexture(IDrawable tile)
        {
            AddTexture(tile, Vector2I.FromVector2(tile.Position) / GameData.TILE_SIZE, false);
        }

        //!\\ TODO (test, temporary ==> ugly)
        void AddTexture(IDrawable tile, Vector2I position, bool man = true)
        {
            Textures.Add(tile);

            bool isWall = false;
            if (!man)
            {
                int groundWallBoundary = -Highness + Height;
                
                if (position.Y >= groundWallBoundary)
                {
                    position.Y -= groundWallBoundary;
                    isWall = true;
                }
                else
                {
                    position.Y += Highness;
                }
            }

            if (isWall)
                WallTextures[position.Y, position.X] = tile;
            else
                GroundTextures[position.Y, position.X] = tile;
        }

        void AddTextureRange(List<IDrawable> tiles)
        {
            foreach (IDrawable tile in tiles)
                AddTexture(tile);
        }

        FloatRect GetGroundRect()
        {
            return new FloatRect(Left, Top - Summit * GameData.TILE_SIZE, Right, Bottom - Summit * GameData.TILE_SIZE);
        }

        FloatRect GetWallRect()
        {
            return new FloatRect(Left, Bottom - Summit * GameData.TILE_SIZE, Right, Bottom);
        }

        //!\\ TODO
        class JoinInfo
        {
            public Wall Wall;
            public Vector2I TextureCoordinates;
        }
        //!\\ TODO
        /// <summary>
        /// Merge given platforms with this one if they are tile-aligned with it.
        /// </summary>
        /// <param name="walls"></param>
        public void MergeWith(List<Wall> walls)
        {
            List<Wall> groundJoins = new List<Wall>() { this };
            List<Wall> wallJoins = new List<Wall>() { this };

            foreach (Wall wall in walls)
            {
                if ((Position.X - wall.Position.X) % GameData.TILE_SIZE != 0 ||
                    (Position.Y - wall.Position.Y) % GameData.TILE_SIZE != 0)
                {
                    // wall is not aligned, it is ignored
                    continue;
                }

                if (Summit == wall.Summit)
                    groundJoins.Add(wall);
                if (Bottom == wall.Bottom)
                    wallJoins.Add(wall);
            }

            FloatRect groundJoinsRect = GetGroundRect();
            FloatRect wallJoinsRect = GetWallRect();

            foreach (Wall wall in groundJoins)
            {
                FloatRect wallGroundJoinsRect = wall.GetGroundRect();

                groundJoinsRect = new FloatRect(
                    System.Math.Min(groundJoinsRect.Left, wallGroundJoinsRect.Left),
                    System.Math.Min(groundJoinsRect.Top, wallGroundJoinsRect.Top),
                    System.Math.Max(groundJoinsRect.Right, wallGroundJoinsRect.Right),
                    System.Math.Max(groundJoinsRect.Bottom, wallGroundJoinsRect.Bottom));
            }

            foreach (Wall wall in wallJoins)
            {
                FloatRect wallWallJoinsRect = wall.GetWallRect();

                wallJoinsRect = new FloatRect(
                    System.Math.Min(wallJoinsRect.Left, wallWallJoinsRect.Left),
                    System.Math.Min(wallJoinsRect.Top, wallWallJoinsRect.Top),
                    System.Math.Max(wallJoinsRect.Right, wallWallJoinsRect.Right),
                    System.Math.Max(wallJoinsRect.Bottom, wallWallJoinsRect.Bottom));
            }

            IntRect groundJoinsRectBase = new IntRect(
                0,
                0,
                (int)((groundJoinsRect.Right - groundJoinsRect.Left) / GameData.TILE_SIZE),
                (int)((groundJoinsRect.Bottom - groundJoinsRect.Top) / GameData.TILE_SIZE));

            IntRect wallJoinsRectBase = new IntRect(
                0,
                0,
                (int)((wallJoinsRect.Right - wallJoinsRect.Left) / GameData.TILE_SIZE),
                (int)((wallJoinsRect.Bottom - wallJoinsRect.Top) / GameData.TILE_SIZE));

            JoinInfo[,] groundJoinsTextures = new JoinInfo[groundJoinsRectBase.Rect.Height, groundJoinsRectBase.Rect.Width];
            IDrawable[,] wallJoinsTextures = new IDrawable[wallJoinsRectBase.Rect.Height, wallJoinsRectBase.Rect.Width];

            foreach (Wall wall in groundJoins)
            {
                for (int y = 0; y < wall.Height; ++y)
                {
                    for (int x = 0; x < wall.Width; ++x)
                    {
                        Vector2I offset = new Vector2I(
                            (int)((wall.GetGroundRect().Left - groundJoinsRect.Left) / GameData.TILE_SIZE),
                            (int)((wall.GetGroundRect().Top - groundJoinsRect.Top) / GameData.TILE_SIZE));
                        Vector2I index = new Vector2I(
                            x + offset.X,
                            y + offset.Y);
                        if (groundJoinsTextures[index.Y, index.X] != null)
                            continue;

                        JoinInfo joinInfo = new JoinInfo();
                        joinInfo.Wall = wall;
                        joinInfo.TextureCoordinates = new Vector2I(x, y);
                        groundJoinsTextures[index.Y, index.X] = joinInfo;
                    }
                }
            }

            for (int y = 0; y < groundJoinsRectBase.Rect.Height; ++y)
            {
                for (int x = 0; x < groundJoinsRectBase.Rect.Width; ++x)
                {
                    if (groundJoinsTextures[y, x] == null)
                        continue;

                    IntRect indexes = new IntRect(
                        x - 1,
                        y - 1,
                        x + 1,
                        y + 1);

                    bool
                        left = indexes.Left >= 0 && groundJoinsTextures[y, indexes.Left] != null,
                        top = indexes.Top >= 0 && groundJoinsTextures[indexes.Top, x] != null,
                        right = indexes.Right < groundJoinsRectBase.Right && groundJoinsTextures[y, indexes.Right] != null,
                        bottom = indexes.Bottom < groundJoinsRectBase.Bottom && groundJoinsTextures[indexes.Bottom, x] != null;

                    Log.Cl("Left[" + left + "], Top[" + top + "], Right[" + right + "], Bottom[" + bottom + "]");

                    JoinInfo joinInfo = groundJoinsTextures[y, x];
                    IDrawable tile = null;
                    //!\\ TODO + add 4 corners //!\\
                    if (left && top && right && bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.All);
                    else if (left && top && right)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.LeftTopRight);
                    else if (left && top && bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.TopLeftBottom);
                    else if (top && right && bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.TopRightBottom);
                    else if (left && right && bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.LeftBottomRight);
                    else if (left && right)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.LeftRight);
                    else if (left && top)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.TopLeft);
                    else if (left && bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.BottomLeft);
                    else if (top && right)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.TopRight);
                    else if (top && bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.TopBottom);
                    else if (right && bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.BottomRight);
                    else if (left)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.Left);
                    else if (right)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.Right);
                    else if (top)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.Top);
                    else if (bottom)
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.Bottom);
                    else
                        tile = AutoTile.GetJoinPattern(BlazeraLib.AutoTile.PartType.None);

                    joinInfo.Wall.RemoveTexture(joinInfo.Wall.GroundTextures[joinInfo.TextureCoordinates.Y, joinInfo.TextureCoordinates.X]);
                    //!\\ TODO
                    tile.Position = (new Vector2I(joinInfo.TextureCoordinates.X, joinInfo.TextureCoordinates.Y) * GameData.TILE_SIZE).ToVector2() + joinInfo.Wall.Position
                        - new SFML.Window.Vector2f(0F, joinInfo.Wall.Summit * GameData.TILE_SIZE);
                    joinInfo.Wall.AddTexture(tile, new Vector2I(joinInfo.TextureCoordinates.X, joinInfo.TextureCoordinates.Y));
                }
            }
        }
    }
}
