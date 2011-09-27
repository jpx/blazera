using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public interface IScriptable
    {
        /// <summary>
        /// Ecrit le script du type par defaut - la variable est definie par le LongType
        /// </summary>
        void ToScript();

        ScriptWriter Sw { get; set; }
    }
}
