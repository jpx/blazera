using System;
using SFML.Graphics;

namespace BlazeraLib
{
    public class ComposedTexture : BaseDrawable
    {
        public enum EPattern
        {
            Single,
            Horizontal,
            Vertical,
            Square
        }

        const EPattern DEFAULT_DOUBLE_PATTERN = EPattern.Horizontal;

        EPattern Pattern;
        Texture[] Textures;

        public bool IsVisible { get; set; }

        public ComposedTexture(Texture texture)
        {
            Pattern = EPattern.Single;
            Textures = new Texture[] { texture };
            IsVisible = true;
        }

        public ComposedTexture(Texture texture1, Texture texture2, EPattern pattern = DEFAULT_DOUBLE_PATTERN)
        {
            Pattern = pattern;
            Textures = new Texture[] { texture1, texture2 };
            IsVisible = true;

            if (Pattern != EPattern.Horizontal &&
                Pattern != EPattern.Vertical)
                throw new Exception("Wrong pattern with two textures");

            AdjustPosition();
        }

        public ComposedTexture(Texture texture1, Texture texture2, Texture texture3, Texture texture4)
        {
            Pattern = EPattern.Square;
            Textures = new Texture[] { texture1, texture2, texture3, texture4 };
            IsVisible = true;

            AdjustPosition();
        }

        public ComposedTexture(ComposedTexture copy)
        {
            Pattern = copy.Pattern;
            Textures = new Texture[copy.Textures.Length];
            for (Int32 count = 0; count < copy.Textures.Length; ++count)
                Textures[count] = new Texture(copy.Textures[count]);
            Position = copy.Position;
            IsVisible = copy.IsVisible;

            AdjustPosition();
        }

        public override object Clone()
        {
            return new ComposedTexture(this);
        }

        void AdjustPosition()
        {
            switch (Pattern)
            {
                case EPattern.Horizontal:
                    Textures[1].Position = new Vector2(
                    Textures[0].Dimension.X,
                    0F);
                    break;

                case EPattern.Vertical:
                    Textures[1].Position = new Vector2(
                    0F,
                    Textures[0].Dimension.Y);
                    break;

                case EPattern.Square:
                    Textures[1].Position = new Vector2(
                        Textures[0].Dimension.X,
                        0F);
                    Textures[2].Position = new Vector2(
                        0F,
                        Textures[0].Dimension.Y);
                    Textures[3].Position = new Vector2(
                        Textures[2].Dimension.X,
                        Textures[1].Dimension.Y);
                    break;
            }
        }

        public override void Draw(RenderWindow window)
        {
            foreach (Texture texture in Textures)
                if (texture != null)
                    if (IsVisible)
                        texture.Draw(window);
        }

        Texture GetBaseTexture()
        {
            return Textures[0];
        }

        public override Vector2 Position
        {
            get { return GetBaseTexture().Position; }
            set
            {
                Vector2 offset = value - GetBaseTexture().Position;

                foreach (Texture texture in Textures)
                    if (texture != null)
                        texture.Position += offset;
            }
        }

        public override Vector2 Dimension
        {
            get
            {
                float
                    xDimension = 0F,
                    yDimension = 0F;

                switch (Pattern)
                {
                    case EPattern.Single:
                        xDimension = GetBaseTexture().Dimension.X;
                        yDimension = GetBaseTexture().Dimension.Y;
                        break;

                    case EPattern.Horizontal:
                        xDimension = Textures[0].Dimension.X + Textures[1].Dimension.X;
                        yDimension = Math.Max(Textures[0].Dimension.Y, Textures[1].Dimension.Y);
                        break;

                    case EPattern.Vertical:
                        xDimension = Math.Max(Textures[0].Dimension.X, Textures[1].Dimension.Y);
                        yDimension = Textures[0].Dimension.Y + Textures[1].Dimension.Y;
                        break;

                    case EPattern.Square:
                        xDimension = Math.Max(Textures[0].Dimension.X, Textures[1].Dimension.Y);
                        yDimension = Math.Max(Textures[0].Dimension.Y, Textures[1].Dimension.Y);
                        break;
                }

                return new Vector2(xDimension, yDimension);
            }
        }
    }
}
