using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class BBoundingBox : BoundingBox
    {
        public BBoundingBox(WorldObject holder, int left, int top, int right, int bottom) :
            base(holder, left, top, right, bottom)
        {
            
        }

        public BBoundingBox(BBoundingBox copy, WorldObject holder) :
            base(copy, holder)
        {
            
        }
    }
}
