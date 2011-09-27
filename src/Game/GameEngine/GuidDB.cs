using System.Collections.Generic;

namespace BlazeraLib
{
    public class TypeGuidDB
    {
        #region Singleton

        static TypeGuidDB _instance;
        public static TypeGuidDB Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TypeGuidDB();
                return _instance;
            }
            set { _instance = value; }
        }

        public void Init()
        {

        }

        #endregion

        #region Constants

        const int BASETYPE_GUID_MAX_BYTE_SIZE = 2;

        #endregion

        #region Members

        Dictionary<int, string> BaseTypes;
        Dictionary<int, string> Types;
        Dictionary<string, int> BaseTypeGuids;
        Dictionary<string, int> TypeGuids;

        #endregion

        TypeGuidDB()
        {
            BaseTypes = new Dictionary<int, string>();
            Types = new Dictionary<int, string>();
            BaseTypeGuids = new Dictionary<string, int>();
            TypeGuids = new Dictionary<string, int>();
        }
    }
}
