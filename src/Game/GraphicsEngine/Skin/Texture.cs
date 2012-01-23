using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Texture : Skin
    {
        public Texture() :
            base()
        {
            IsVisible = true;
        }

        public Texture(Texture copy) :
            base(copy)
        {
            IsVisible = true;
            ImagePath = copy.ImagePath;
            ImageSubRect = copy.ImageSubRect;
            Color = copy.Color;
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
            Sw = new ScriptWriter(this);
            Sw.InitObject();
            Sw.WriteProperty("ImagePath", "\"" + ImagePath + "\"");
            if (ImageSubRect != null)
            {
                Sw.WriteProperty("ImageSubRect", "IntRect(" + ImageSubRect.Left + ", " +
                                                                   ImageSubRect.Top + ", " +
                                                                   ImageSubRect.Right + ", " +
                                                                   ImageSubRect.Bottom + ")");
            }
            Sw.EndObject();
        }

        public override string ToScriptString()
        {
            return "Create:Texture ( " + ScriptWriter.GetStringOf(Type) + " )";
        }

        public override void Draw(RenderTarget window)
        {
            if (IsVisible)
                window.Draw(Sprite);
        }

        public override Texture GetTexture()
        {
            return this;
        }

        public override void Start()
        {
            base.Start();

            Stop();
        }

        /// <summary>
        /// Set the alpha component of the texture
        /// </summary>
        /// <param name="a">Component alpha in percentage</param>
        public void SetAlpha(double a)
        {
            Sprite.Color = new Color(Sprite.Color.R,
                                          Sprite.Color.G,
                                          Sprite.Color.B,
                                          (Byte)(a * (255D / 100D)));
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
                if (ImagePath != null)
                {
                    Sprite = new Sprite(TextureManager.Instance.GetTexture(ImagePath));
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

                    if (left > Sprite.Texture.Width)
                        left = (Int32)Sprite.Texture.Width;
                    if (left < 0)
                        left = 0;

                    if (top > Sprite.Texture.Height)
                        top = (Int32)Sprite.Texture.Height;
                    if (top < 0)
                        top = 0;

                    if (right > Sprite.Texture.Width)
                        right = (Int32)Sprite.Texture.Width;
                    if (right < left)
                        right = left;

                    if (bottom > Sprite.Texture.Height)
                        bottom = (Int32)Sprite.Texture.Height;
                    if (bottom < top)
                        bottom = top;
                }
                else
                {
                    left = 0;
                    top = 0;
                    right = (Int32)Sprite.Texture.Width;
                    bottom = (Int32)Sprite.Texture.Height;
                }

                _imageSubRect = new IntRect(
                    left,
                    top,
                    right,
                    bottom);

                Sprite.SubRect = ImageSubRect.Rect;
            }
        }

        public override Vector2f Position
        {
            get
            {
                return Sprite.Position;
            }
            set
            {
                Sprite.Position = value; /*Vector2I.FromVector2(value).ToVector2(); // in case of bad alignment
                
                Int32 xAdd = value.X - .5F < (float)(Int32)value.X ? 0 : 1;
                Int32 yAdd = value.Y - .5F < (float)(Int32)value.Y ? 0 : 1;

                Sprite.Position += new Vector2f(xAdd, yAdd);*/
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                return new Vector2f(Sprite.Width,
                                   Sprite.Height);
            }
            set
            {
                Sprite.Width = value.X;
                Sprite.Height = value.Y;
            }
        }

        public Vector2f ImageDimension
        {
            get
            {
                return new Vector2f(Sprite.Texture.Width,
                                    Sprite.Texture.Height);
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
                Sprite.Color = value;
            }
        }

        public ScriptWriter Sw { get; set; }
    }
}