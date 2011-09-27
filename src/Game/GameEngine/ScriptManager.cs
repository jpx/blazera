using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class ScriptManager
    {
        private Hashtable ScriptTable
        {
            get;
            set;
        }

        private ScriptManager()
        {
            this.ScriptTable = new Hashtable();
        }

        public void Init()
        {
            
        }

        private static ScriptManager _instance;
        public static ScriptManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    ScriptManager.Instance = new ScriptManager();
                }

                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private T Add<T>(String type)
        {
            Object obj = ScriptEngine.Get<T>(type);

            if (ScriptTable.ContainsKey(type))
                ScriptTable[type] = obj;
            else
                ScriptTable.Add(type, obj);

            return (T)obj;
        }

        public static Boolean Remove(String type)
        {
            if (!Instance.ScriptTable.ContainsKey(type))
                return false;

            Instance.ScriptTable.Remove(type);

            return true;
        }

        public static T Get<T>(String type, Boolean reload = false)
        {
            if (!Instance.ScriptTable.ContainsKey(type))
                return Instance.Add<T>(type);

            if (reload)
                return Instance.Add<T>(type);

            return (T)Instance.ScriptTable[type];
        }

        public static void Refresh<T>(String type)
        {
            String longType = typeof(T).Name + "_" + type;

            Instance.Add<T>(longType);
        }
    }
}
