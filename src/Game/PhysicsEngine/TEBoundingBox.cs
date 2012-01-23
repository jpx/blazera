using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class TEBoundingBox : BoundingBox
    {
        public TEBoundingBox(WorldObject holder, int left, int top, int right, int bottom, int z = DEFAULT_BASE_Z) :
            base(holder, left, top, right, bottom, z)
        {
            
        }

        public TEBoundingBox(TEBoundingBox copy, WorldObject holder) :
            base(copy, holder)
        {
            Trigger = copy.Trigger;
        }

        public TEBoundingBox Trigger
        {
            get;
            set;
        }
    }
}
