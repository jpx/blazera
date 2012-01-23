using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class NPC : Personnage
    {
        public NPC() :
            base()
        {

        }

        public NPC(NPC copy) :
            base(copy)
        {
        }

        public override object Clone()
        {
            return new NPC(this);
        }
    }
}
