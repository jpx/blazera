using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class PictureBox : Widget
    {
        public PictureBox(Texture texture) :
            base()
        {
            this.Texture = texture;
        }

        public override void Draw(RenderWindow window)
        {
            base.Draw(window);

            if (!this.IsVisible)
                return;

            if (this.Texture != null)
                this.Texture.Draw(window);
        }

        public override void Refresh()
        {
            if (this.Texture == null)
                return;

            this.Texture.Dimension = this.Dimension;

            this.Texture.Position = this.GetGlobalFromLocal(
                new Vector2f(
                    0F,
                    0F));
        }

        public override Vector2f Dimension
        {
            get
            {
                if (this.Texture == null)
                    return base.Dimension;

                return this.Texture.Dimension;
            }
            set
            {
                base.Dimension = value;

                if (this.Texture != null)
                    this.Texture.Dimension = value;
            }
        }

        private Texture _texture;
        public Texture Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;

                if (this.Texture == null)
                    return;

                this.Dimension = this.Texture.Dimension;

                Texture.Position = GetGlobalFromLocal(new Vector2f());
            }
        }

        public override Color Color
        {
            get { return base.Color; }
            set
            {
                base.Color = value;

                if (Texture != null)
                    this.Texture.Color = this.Color;
            }
        }
    }
}
