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
            Texture = texture;
        }

        public override void Draw(RenderTarget window)
        {
            base.Draw(window);

            if (!IsVisible)
                return;

            if (Texture != null)
                Texture.Draw(window);
        }

        public override void Refresh()
        {
            if (Texture == null)
                return;

            Texture.Dimension = Dimension;

            Texture.Position = GetGlobalFromLocal(
                new Vector2f(
                    0F,
                    0F));
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Texture == null)
                    return base.Dimension;

                return Texture.Dimension;
            }
            set
            {
                base.Dimension = value;

                if (Texture != null)
                    Texture.Dimension = value;
            }
        }

        private Texture _texture;
        public Texture Texture
        {
            get { return _texture; }
            set
            {
                _texture = value;

                if (Texture == null)
                    return;

                Dimension = Texture.Dimension;

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
                    Texture.Color = Color;
            }
        }
    }
}
