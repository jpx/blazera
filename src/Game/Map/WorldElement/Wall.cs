using System.Collections.Generic;

namespace BlazeraLib
{
    public class Wall : WorldElement
    {
        #region Constants

        const uint DEFAULT_WIDTH = 1;
        const uint DEFAULT_HEIGHT = 1;
        const uint DEFAULT_HIGHNESS = 1;

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
        List<IDrawable> WallTextures;

        /// <summary>
        /// Length of the wall in tile
        /// </summary>
        uint Width;
        /// <summary>
        /// Deepness of the wall in tile
        /// </summary>
        uint Height;
        /// <summary>
        /// Highness of the wall in tile
        /// </summary>
        uint Highness;

        #endregion

        public Wall() :
            base()
        {
            WallTextures = new List<IDrawable>();
        }

        public Wall(Wall copy) :
            base(copy)
        {
            AutoTile = new AutoTile(copy.AutoTile);
            WallAutoTile = new AutoTile(copy.WallAutoTile);
            WallTextures = new List<IDrawable>(copy.WallTextures);
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

        public override void Draw(SFML.Graphics.RenderWindow window)
        {
            foreach (IDrawable drawableTexture in WallTextures)
                drawableTexture.Draw(window);
        }

        public override SFML.Window.Vector2f Position
        {
            get { return base.Position; }
            set
            {
                SFML.Window.Vector2f offset = value - Position;

                base.Position = value;

                if (WallTextures == null)
                    return;

                foreach (IDrawable drawableTexture in WallTextures)
                    drawableTexture.Position += offset;
            }
        }

        public override SFML.Window.Vector2f Dimension
        {
            get
            {
                return new SFML.Window.Vector2f(Width * GameDatas.TILE_SIZE, Height * GameDatas.TILE_SIZE);
            }
        }

        //!\\ TODO //!\\ (temp)
        public override Texture Skin
        {
            get
            {
                return BaseTexture;
            }
        }

        public void SetBase(Texture baseTexture, uint width = DEFAULT_WIDTH, uint height = DEFAULT_HEIGHT, uint highness = DEFAULT_HIGHNESS)
        {
            BaseTexture = baseTexture;

            AutoTile = new AutoTile(BaseTexture);
            WallAutoTile = new AutoTile(BaseTexture, true);

            SetDimension(width, height, highness);
        }

        public void SetDimension(uint width = DEFAULT_WIDTH, uint height = DEFAULT_HEIGHT, uint highness = DEFAULT_HIGHNESS)
        {
            Width = width;
            Height = height;
            Highness = highness;

            Build();
        }

        Vector2I GetGlobalFromLocal(Vector2I point, bool wallMode = false)
        {
            return point - new Vector2I(0, Highness) + (wallMode ? new Vector2I(0, Height) : new Vector2I());
        }

        int GetYOffset(bool wallMode = false)
        {
            return -((int)Highness) + (wallMode ? (int)Height : 0);
        }

        /// <summary>
        /// Builds the bounding boxes and the texture of the wall
        /// </summary>
        void Build()
        {
            /**
             * BBoundingBoxes
             */

            /* multi version
            for (uint height = 0; height < Height; ++height)
            {
                for (uint width = 0; width < Width; ++width)
                {
                    AddBoundingBox(new BBoundingBox(
                        this,
                        (int)width * GameDatas.TILE_SIZE,
                        (int)height * GameDatas.TILE_SIZE,
                        (int)(width + 1) * GameDatas.TILE_SIZE,
                        (int)(height + 1) * GameDatas.TILE_SIZE));
                }
            }*/
            /* single version */
            AddBoundingBox(new BBoundingBox(
                this,
                0,
                0,
                (int)Width * GameDatas.TILE_SIZE,
                (int)Height * GameDatas.TILE_SIZE));


            /**
             * Textures
             */

            if (Width == 1 &&
                Height == 1 &&
                Highness == 1)
            {
                WallTextures.Add(AutoTile.GetSingle(GetYOffset()));
                WallTextures.Add(WallAutoTile.GetSingle(GetYOffset(true)));
                return;
            }

            if (Width == 1 &&
                Height == 1)
            {
                WallTextures.Add(AutoTile.GetSingle(GetYOffset()));
                WallTextures.AddRange(WallAutoTile.GetSingleColumn((int)Highness, GetYOffset(true)));
                return;
            }

            if (Width == 1 &&
                Highness == 1)
            {
                WallTextures.AddRange(AutoTile.GetSingleColumn((int)Height, GetYOffset()));
                WallTextures.Add(WallAutoTile.GetSingle(GetYOffset(true)));
                return;
            }

            if (Height == 1 &&
                Highness == 1)
            {
                WallTextures.AddRange(AutoTile.GetSingleLine((int)Width, GetYOffset()));
                WallTextures.AddRange(WallAutoTile.GetSingleLine((int)Width, GetYOffset(true)));
                return;
            }

            if (Width == 1)
            {
                WallTextures.AddRange(AutoTile.GetSingleColumn((int)Height, GetYOffset()));
                WallTextures.AddRange(WallAutoTile.GetSingleColumn((int)Highness, GetYOffset(true)));
                return;
            }

            if (Height == 1)
            {
                WallTextures.AddRange(AutoTile.GetSingleLine((int)Width, GetYOffset()));
                WallTextures.AddRange(WallAutoTile.GetArea((int)Width, (int)Highness, GetYOffset(true)));
                return;
            }

            if (Highness == 1)
            {
                WallTextures.AddRange(AutoTile.GetArea((int)Width, (int)Height, GetYOffset()));
                WallTextures.AddRange(WallAutoTile.GetSingleLine((int)Width, GetYOffset(true)));
                return;
            }

            WallTextures.AddRange(AutoTile.GetArea((int)Width, (int)Height, GetYOffset()));
            WallTextures.AddRange(WallAutoTile.GetArea((int)Width, (int)Highness, GetYOffset(true)));
        }
    }
}
