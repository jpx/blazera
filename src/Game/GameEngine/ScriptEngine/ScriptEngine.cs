using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using LuaInterface;
using SFML.Window;

namespace BlazeraLib
{
    public class ScriptEngine
    {
        private ScriptEngine()
        {
            Lua = new Lua();
            ScriptsPath = null;
        }

        public void Init(String dataScript)
        {
            timer.Start();
            GameData.DATAS_DEFAULT_PATH = (String)DoScript("DataPath")[0];

            GameData.SCRIPTS_DEFAULT_PATH = GameData.DATAS_DEFAULT_PATH + "scripts/";
            GameData.IMAGES_DEFAULT_PATH = GameData.DATAS_DEFAULT_PATH + "images/";
            GameData.SOUNDS_DEFAULT_PATH = GameData.DATAS_DEFAULT_PATH + "sounds/";

            ScriptsPath = GameData.SCRIPTS_DEFAULT_PATH;

            Load_Assembly("BlazeraLib");
            Import_Type(typeof(BlazeraLib.Create));
            Import_Type(typeof(BlazeraLib.Ground));
            Import_Type(typeof(BlazeraLib.Map));
            Import_Type(typeof(BlazeraLib.Tile));
            Import_Type(typeof(BlazeraLib.TileSet));
            Import_Type(typeof(BlazeraLib.Texture));
            Import_Type(typeof(BlazeraLib.Player));
            Import_Type(typeof(BlazeraLib.GroundElement));
            Import_Type(typeof(BlazeraLib.Element));
            Import_Type(typeof(BlazeraLib.DisplaceableElement));
            Import_Type(typeof(BlazeraLib.WorldItem));
            Import_Type(typeof(BlazeraLib.QuestItem));
            Import_Type(typeof(BlazeraLib.Wall));
            Import_Type(typeof(BlazeraLib.Teleporter));
            Import_Type(typeof(BlazeraLib.Door));
            Import_Type(typeof(BlazeraLib.Animation));
            Import_Type(typeof(BlazeraLib.BBoundingBox));
            Import_Type(typeof(BlazeraLib.EBoundingBox));
            Import_Type(typeof(BlazeraLib.EBoundingBoxType));
            Import_Type(typeof(BlazeraLib.EventBoundingBoxType));
            Import_Type(typeof(BlazeraLib.ObjectEvent));
            Import_Type(typeof(BlazeraLib.ObjectEventType));
            Import_Type(typeof(BlazeraLib.DelayedObjectEvent));
            Import_Type(typeof(BlazeraLib.Action));
            Import_Type(typeof(BlazeraLib.ActionType));
            Import_Type(typeof(BlazeraLib.WarpAction));
            Import_Type(typeof(BlazeraLib.InputType));
            Import_Type(typeof(BlazeraLib.Direction));
            Import_Type(typeof(BlazeraLib.NPC));
            Import_Type("Vector2", "SFML.Window.Vector2f");
            Import_Type(typeof(SFML.Graphics.Color));
            Import_Type(typeof(BlazeraLib.IntRect));
            Import_Type(typeof(BlazeraLib.FloatRect));
            Import_Type(typeof(SFML.Window.Styles));
            Import_Type(typeof(SFML.Window.Keyboard.Key));
            Import_Type(typeof(BlazeraLib.Log));
            Import_Type(typeof(BlazeraLib.SoundManager));
            Import_Type(typeof(BlazeraLib.ComparisonPointYType));
            Import_Type(typeof(BlazeraLib.Spell));
            Import_Type(typeof(BlazeraLib.CellAreaType));
            Import_Type(typeof(BlazeraLib.OpacityBox));
            Import_Type(typeof(BlazeraLib.OpacityBox.EDesactivatingSideType));

            DoScript(dataScript);

            GameData.Init();
        }

        private static ScriptEngine _instance;
        public static ScriptEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    ScriptEngine.Instance = new ScriptEngine();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        Timer timer = new Timer();
       // static int co = 0;
        public object[] DoScript(String scriptName)
        {
          //  Log.Cl(co + " : " + scriptName + " " + timer.GetElapsedTime().Value.ToString(), ConsoleColor.Yellow);
          //  co++;
            try
            {
                return Lua.DoFile(ScriptsPath + scriptName + ".lua");
            }
            catch (Exception e)
            {
                Log.Cl(e.Message);
                return null;
            }
        }

        public static T Get<T>(String scriptName)
        {
            try
            {
                return (T)ScriptEngine.Instance.DoScript(typeof(T).Name + "/" + scriptName)[0];
            }
            catch
            {
                Log.Clerr("Unable to do script : " + scriptName);
            }

            return (T)typeof(T).GetConstructor(new Type[] { }).Invoke(new Object[] { });
        }

        // Int
        public static int GetInt(String propertyName)
        {
            return Int32.Parse(ScriptEngine.Instance.Lua[propertyName].ToString());
        }

        // UInt
        public static UInt32 GetUInt(String propertyName)
        {
            return UInt32.Parse(ScriptEngine.Instance.Lua[propertyName].ToString());
        }

        // Float
        public static float GetFloat(String propertyName)
        {
            return (float)(double)ScriptEngine.Instance.Lua[propertyName];
        }

        // Double
        public static double GetDouble(String propertyName)
        {
            return (double)ScriptEngine.Instance.Lua[propertyName];
        }

        // Bool
        public static Boolean GetBool(String propertyName)
        {
            return (Boolean)ScriptEngine.Instance.Lua[propertyName];
        }

        // String
        public static String GetString(String propertyName)
        {
            return (String)ScriptEngine.Instance.Lua[propertyName];
        }

        public static Direction GetDirection(String propertyName)
        {
            return (Direction)ScriptEngine.Instance.Lua[propertyName];
        }

        public static Styles GetStyles(String propertyName)
        {
            return (Styles)ScriptEngine.Instance.Lua[propertyName];
        }

        public static Keyboard.Key GetKeyCode(String propertyName)
        {
            return (Keyboard.Key)ScriptEngine.Instance.Lua[propertyName];
        }

        public static List<T> GetList<T>(string propertyName)
        {
            List<T> list = new List<T>();

            LuaTable table = (LuaTable)ScriptEngine.Instance.Lua[propertyName];

            foreach (object obj in table.Values)
                list.Add((T)obj);

            return list;
        }

        public void Load_Assembly(String assembly)
        {
            Lua.DoString("luanet.load_assembly(\"" + assembly + "\")");
        }

        public void Import_Type(Type type)
        {
            Lua.DoString(type.Name + " = luanet.import_type(\"" + type.FullName + "\")");
        }

        public void Import_Type(String name, String fullName)
        {
            Lua.DoString(name + " = luanet.import_type(\"" + fullName + "\")");
        }

        private Lua Lua
        {
            get;
            set;
        }

        public String ScriptsPath
        {
            get;
            set;
        }
    }
}
