using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class Element : WorldElement
    {
        public Element() :
            base()
        {

        }

        public Element(Element copy) :
            base(copy)
        {

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
