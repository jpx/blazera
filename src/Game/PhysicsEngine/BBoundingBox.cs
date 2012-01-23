using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class BBoundingBox : BoundingBox
    {
        public BBoundingBox(WorldObject holder, int left, int top, int right, int bottom, int z = DEFAULT_BASE_Z) :
            base(holder, left, top, right, bottom, z)
        {
            
        }

        public BBoundingBox(BBoundingBox copy, WorldObject holder) :
            base(copy, holder)
        {
            
        }
    }
}
