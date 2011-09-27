using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public abstract class WorldElement : WorldObject
    {
        public WorldElement() :
            base()
        {

        }

        public WorldElement(WorldElement copy) :
            base(copy)
        {

        }
    }
}
