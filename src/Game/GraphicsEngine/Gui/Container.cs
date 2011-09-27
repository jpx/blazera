using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class Container : Widget
    {
        public static readonly Texture BACKGROUND_TEXTURE = Create.Texture("BackgroundWidget_Container");
        const float BACKGROUND_DIMENSION = 38F;

        public Container() :
            base()
        {
            Background = new PictureBox(new Texture(BACKGROUND_TEXTURE));
            BackgroundDimension = new Vector2(BACKGROUND_DIMENSION, BACKGROUND_DIMENSION);
        }

        public override void Refresh()
        {
            base.Refresh();

            if (!IsEmpty)
                Content.Center = Center;
        }

        private Widget _content;
        protected Widget Content
        {
            get { return _content; }
            set
            {
                if (Content != null ||
                    (value == null && Content != null))
                    RemoveWidget(Content);

                _content = value;

                if (Content == null)
                    return;

                AddWidget(Content);
            }
        }

        public Boolean IsEmpty
        {
            get { return Content == null; }
        }
    }
}
