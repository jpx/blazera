namespace BlazeraLib
{
    /// <summary>
    /// Base class of the scriptables objects constituting the game.
    /// </summary>
    public abstract class BaseObject : IScriptable
    {
        #region Constants

        /// <summary>
        /// Current unique identifier count of the objects.
        /// </summary>
        private static uint IdCount = 0;

        #endregion

        #region Members

        /// <summary>
        /// Script type name.
        /// </summary>
        public string Type { get; private set; }

        /// <summary>
        /// Unique identifier of the object.
        /// </summary>
        public uint NumId { get; private set; }

        /// <summary>
        /// A litteral name for the object type.
        /// </summary>
        public virtual string Name { get; set; }

        /// <summary>
        /// Tool allowing to write data of the object to its associated script.
        /// </summary>
        public ScriptWriter Sw { get; set; }

        #endregion

        /// <summary>
        /// Constructs a default object attributing a unique identifier to it.
        /// </summary>
        public BaseObject()
        {
            NumId = IdCount++;
        }

        /// <summary>
        /// Constructs an object from another.
        /// </summary>
        /// <param name="copy">Object to copy.</param>
        public BaseObject(BaseObject copy)
        {
            Type = copy.Type;
            NumId = IdCount++;
            Name = copy.Name;
        }

        /// <summary>
        /// Writes properties of the object to its associate script.
        /// </summary>
        public virtual void ToScript()
        {
            if (Name != null)
            {
                Sw.WriteProperty("Name", ScriptWriter.GetStringOf(Name));
            }
        }

        /// <summary>
        /// Virtual method thats set script type name of the object.
        /// </summary>
        /// <param name="type">Script type name to set.</param>
        /// <param name="creation">Specifies if it is the creation of the object.</param>
        public virtual void SetType(string type, bool creation = true)
        {
            Type = type;
        }

        /// <summary>
        /// A string representing a type with more information.
        /// </summary>
        public virtual string LongType
        {
            get { return GetType().Name + "_" + Type; }
        }

        /// <summary>
        /// Full type of the object that allows to find its associate script.
        /// </summary>
        public virtual string FullType
        {
            get { return GetType().Name + "/" + LongType; }
        }

        /// <summary>
        /// String representing the object with its type and its unique identifier.
        /// </summary>
        public string Id
        {
            get { return LongType + "_" + NumId; }
        }
    }
}
