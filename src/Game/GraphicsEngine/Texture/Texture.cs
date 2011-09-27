using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class Texture : DrawableBaseObject
    {
        public Texture() :
            base()
        {
            this.IsVisible = true;
        }

        public Texture(Texture copy) :
            base(copy)
        {
            this.IsVisible = true;
            this.ImagePath = copy.ImagePath;
            this.ImageSubRect = copy.ImageSubRect;
            this.ColorMask = copy.ColorMask;
            this.Color = copy.Color;
        }

        public Texture(Texture copy, IntRect imageSubRect) :
            this(copy)
        {
            ImageSubRect = new IntRect(
                copy.ImageSubRect.Left + imageSubRect.Left,
                copy.ImageSubRect.Top + imageSubRect.Top,
                copy.ImageSubRect.Left + imageSubRect.Right,
                copy.ImageSubRect.Top + imageSubRect.Bottom);
        }

        public override object Clone()
        {
            return new Texture(this);
        }

        public void ToScript()
        {
            this.Sw = new ScriptWriter(this);
            this.Sw.InitObject();
            this.Sw.WriteProperty("ImagePath", "\"" + this.ImagePath + "\"");
            if (this.ImageSubRect != null)
            {
                this.Sw.WriteProperty("ImageSubRect", "IntRect(" + this.ImageSubRect.Left + ", " +
                                                                   this.ImageSubRect.Top + ", " +
                                                                   this.ImageSubRect.Right + ", " +
                                                                   this.ImageSubRect.Bottom + ")");
            }
            this.Sw.EndObject();
        }

        public override void Draw(RenderWindow window)
        {
            if (this.IsVisible)
                window.Draw(this.Sprite);
        }

        /// <summary>
        /// Set the alpha component of the texture
        /// </summary>
        /// <param name="a">Component alpha in percentage</param>
        public void SetAlpha(double a)
        {
            this.Sprite.Color = new Color(this.Sprite.Color.R,
                                          this.Sprite.Color.G,
                                          this.Sprite.Color.B,
                                          (Byte)(a * (255D / 100D)));
        }

        private Image Image
        {
            get;
            set;
        }

        public Sprite Sprite
        {
            get;
            private set;
        }

        private String _imagePath;
        public String ImagePath
        {
            get { return _imagePath; }
            set
            {
                _imagePath = value;
                if (this.ImagePath != null)
                {
                    this.Image = ImageManager.Instance.GetImage(this.ImagePath);
                    this.Sprite = new Sprite(this.Image);
                    ImageSubRect = null;
                }
            }
        }

        private IntRect _imageSubRect;
        public IntRect ImageSubRect
        {
            get
            {
                if (_imageSubRect == null)
                    return new IntRect(
                        Sprite.SubRect.Left,
                        Sprite.SubRect.Top,
                        Sprite.SubRect.Left + Sprite.SubRect.Width,
                        Sprite.SubRect.Top + Sprite.SubRect.Height);

                return _imageSubRect;
            }
            set
            {
                _imageSubRect = value;

                Int32 left, top, right, bottom;

                if (_imageSubRect != null)
                {
                    left = ImageSubRect.Left;
                    top = ImageSubRect.Top;
                    right = ImageSubRect.Right;
                    bottom = ImageSubRect.Bottom;

                    if (left > Image.Width)
                        left = (Int32)Image.Width;
                    if (left < 0)
                        left = 0;

                    if (top > Image.Height)
                        top = (Int32)Image.Height;
                    if (top < 0)
                        top = 0;

                    if (right > Image.Width)
                        right = (Int32)Image.Width;
                    if (right < left)
                        right = left;

                    if (bottom > Image.Height)
                        bottom = (Int32)Image.Height;
                    if (bottom < top)
                        bottom = top;
                }
                else
                {
                    left = 0;
                    top = 0;
                    right = (Int32)Image.Width;
                    bottom = (Int32)Image.Height;
                }

                _imageSubRect = new IntRect(
                    left,
                    top,
                    right,
                    bottom);

                Sprite.SubRect = ImageSubRect.Rect;
            }
        }

        private Color _colorMask;
        public Color ColorMask
        {
            get { return _colorMask; }
            set
            {
                _colorMask = value;
                this.Image.CreateMaskFromColor(this.ColorMask);
            }
        }

        public override Vector2 Position
        {
            get
            {
                return this.Sprite.Position;
            }
            set
            {
                this.Sprite.Position = value; /*Vector2I.FromVector2(value).ToVector2(); // in case of bad alignment
                
                Int32 xAdd = value.X - .5F < (float)(Int32)value.X ? 0 : 1;
                Int32 yAdd = value.Y - .5F < (float)(Int32)value.Y ? 0 : 1;

                this.Sprite.Position += new Vector2(xAdd, yAdd);*/
            }
        }

        public override Vector2 Dimension
        {
            get
            {
                return new Vector2(this.Sprite.Width,
                                   this.Sprite.Height);
            }
            set
            {
                this.Sprite.Width = value.X;
                this.Sprite.Height = value.Y;
            }
        }

        public Vector2 ImageDimension
        {
            get
            {
                return new Vector2(this.Image.Width,
                                   this.Image.Height);
            }
        }

        public override Color Color
        {
            get
            {
                return Sprite.Color;
            }
            set
            {
                this.Sprite.Color = value;
            }
        }

        public ScriptWriter Sw { get; set; }

        public Color GetPixel(UInt32 x, UInt32 y)
        {
            return Image.GetPixel(x, y);
        }

        public void SetPixel(UInt32 x, UInt32 y, Color color)
        {
            this.Image.SetPixel(x, y, color);
        }

        public UInt32 GetWidth(UInt32 y)
        {
            UInt32 space = 0;
            float x = 0;

            while (this.GetPixel((UInt32)x++, y).A == 0)
                space++;

            x = this.Dimension.X - 1F;

            while (this.GetPixel((UInt32)x--, y).A == 0)
                space++;

            return (UInt32)(this.Dimension.X - space);
        }
    }
}
