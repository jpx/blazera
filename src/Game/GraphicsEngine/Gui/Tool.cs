using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class Tool : Widget
    {
        public Texture IconTexture { get; private set; }

        protected MapBox MapBox;

        protected Tool(Texture iconTexture) :
            base()
        {
            IconTexture = iconTexture;
        }

        public virtual void SetMapBox(MapBox mapBox)
        {
            MapBox = mapBox;
        }
    }
}
