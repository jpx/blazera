using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class ScriptWriter
    {
        public ScriptWriter(BaseObject obj)
        {
            Obj = obj;

            Sw = new StreamWriter(File);
        }

        public void InitObject()
        {
            String str = Name + " = " + Obj.GetType().Name + "()";
            Sw.WriteLine(str);
        }

        public void EndObject()
        {
            String str = "return " + Obj.GetType().Name + "_" + Obj.Type;
            Sw.Write(str);

            Sw.Close();
        }

        public void WriteProperty(String propertyName, String value)
        {
            Sw.WriteLine(ScriptWriter.GetStrProperty(Obj.LongType, propertyName, value));
        }

        public void WriteMethod(String methodName, String[] args)
        {
            Sw.WriteLine(ScriptWriter.GetStrMethod(Obj.LongType, methodName, args));
        }

        public void WriteObjectCreation(BaseObject obj)
        {
            String str = obj.Id + " = Create:" + obj.GetType().Name + "(" + ScriptWriter.GetStringOf(obj.Type) + ")";

            Sw.WriteLine(str);
        }

        public void WriteLine(String str)
        {
            Sw.WriteLine(str);
        }

        public void Write(String str)
        {
            Sw.Write(str);
        }

        private StreamWriter Sw { get; set; }

        private BaseObject Obj { get; set; }

        public String Name
        {
            get
            {
                return Obj.GetType().Name + "_" + Obj.Type;
            }
        }

        private String File
        {
            get
            {
                return GameData.SCRIPTS_DEFAULT_PATH + Obj.GetType().Name + "/" + Name + ".lua";
            }
        }

        public static String GetStringOf(String str)
        {
            return "\"" + str + "\"";
        }

        public static String GetStrProperty(String objId, String propertyName, String value)
        {
            return objId + "." + propertyName + " = " + value;
        }

        public static String GetStrMethod(String objId, String methodName, String[] args)
        {
            String str = objId + ":" + methodName + "(";

            for (int i = 0; i < args.Length - 1; i++)
            {
                str += args[i] + ", ";
            }

            str += args[args.Length - 1] + ")";

            return str;
        }

        public static void AddToScript(String scriptName, String add)
        {
            String file = GameData.SCRIPTS_DEFAULT_PATH + scriptName + ".lua";

            try
            {
                StreamWriter sw = new StreamWriter(file, true);
                sw.WriteLine(add);
                sw.Close();
            }
            catch
            {
                Log.Cl("Impossible d'ajouter " + add + " au fichier " + file + ".");
            }
        }

        public static void WriteToFile(String fileName, String add)
        {
            String file = GameData.SCRIPTS_DEFAULT_PATH + fileName;

            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(add);
            }
        }

        public static String GetStrOfVector2(Vector2f vector2)
        {
            return "Vector2 ( " + vector2.X.ToString() + ", " + vector2.Y.ToString() + " )";
        }

        public static String GetStrOfDirection(Direction direction)
        {
            return "Direction." + direction.ToString();
        }
    }
}
