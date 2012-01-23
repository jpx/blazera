using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class DisplaceableElement : DynamicWorldElement
    {
        public DisplaceableElement() :
            base()
        {

        }

        public DisplaceableElement(DisplaceableElement copy) :
            base(copy)
        {

        }

        public override object Clone()
        {
            return new DisplaceableElement(this);
        }
    }
}
