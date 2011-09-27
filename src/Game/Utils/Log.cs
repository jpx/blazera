using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public static class Log
    {
        public static void Cl()
        {
            Console.WriteLine();
        }

        public static void C(object obj)
        {
            Console.Write(obj);
        }

        public static void Cl(object obj)
        {
            Console.WriteLine(obj);
        }

        public static void Clerr(object obj)
        {
            Log.Cl(obj, ConsoleColor.Red);
        }

        public static void Cl(object obj, String objName)
        {
            Console.WriteLine(objName + " : " + obj);
        }

        public static void C(object obj, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Log.C(obj);
            Console.ResetColor();
        }

        public static void Cl(object obj, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Log.Cl(obj);
            Console.ResetColor();
        }

        public static void Cl(object obj, String objName, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Log.Cl(obj, objName);
            Console.ResetColor();
        }

        public static void Clear()
        {
            try
            {
                Console.Clear();
            }
            catch { }
        }
    }
}
