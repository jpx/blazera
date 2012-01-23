using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class GroundElement : WorldElement
    {
        public GroundElement() :
            base()
        {
            DrawOrder = DrawOrder.Under;
        }

        public GroundElement(GroundElement copy) :
            base(copy)
        {
            DrawOrder = DrawOrder.Under;
        }

        public override object Clone()
        {
            return new GroundElement(this);
        }

        public override void ToScript()
        {
            Sw = new ScriptWriter(this);

            Sw.InitObject();

            base.ToScript();

            Sw.EndObject();
        }
    }
}
