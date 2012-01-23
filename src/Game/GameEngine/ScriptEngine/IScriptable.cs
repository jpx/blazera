using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    /// <summary>
    /// Interface for the scriptable objects.
    /// </summary>
    public interface IScriptable
    {
        /// <summary>
        /// Ecrit le script du type par defaut - la variable est definie par le LongType
        /// </summary>
        void ToScript();

        /// <summary>
        /// Returns a string corresponding to the object creation context.
        /// The variable is its Id.
        /// </summary>
        /// <returns>String of object instanciation.</returns>
        string ToScriptString();

        /// <summary>
        /// Tool that handles basic operations to write the script associated to the object.
        /// </summary>
        ScriptWriter Sw { get; set; }

        /// <summary>
        /// Unique identifier.
        /// </summary>
        string Id { get; }

        string LongType { get; }
    }
}
