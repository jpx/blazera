using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class TEBoundingBox : BoundingBox
    {
        public TEBoundingBox(WorldObject holder, int left, int top, int right, int bottom) :
            base(holder, left, top, right, bottom)
        {
            
        }

        public TEBoundingBox(TEBoundingBox copy, WorldObject holder) :
            base(copy, holder)
        {
            this.Trigger = copy.Trigger;
        }

        public TEBoundingBox Trigger
        {
            get;
            set;
        }
    }
}
