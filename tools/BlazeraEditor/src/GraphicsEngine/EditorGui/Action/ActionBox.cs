using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraEditor
{
    public abstract class ActionBox : VAutoSizeBox
    {
        public abstract BlazeraLib.Action GetAction();
    }
}
