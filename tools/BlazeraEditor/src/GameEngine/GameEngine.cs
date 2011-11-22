using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using SFML.Window;
using LuaInterface;
using BlazeraLib;

namespace BlazeraEditor
{
    /// <summary>
    /// Moteur de script
    /// </summary>
    public class GameEngine
    {
        private GameEngine()
        {
            
        }

        public void Init()
        {
            ScriptEngine.Instance.Init("EditorDatas");
            ScriptEngine.Instance.Load_Assembly("BlazeraEditor");
            ScriptEngine.Instance.Import_Type(typeof(PlayerHdl));

            TextureManager.Instance.Init();
            SoundManager.Instance.Init();
        }

        private static GameEngine _instance;
        public static GameEngine Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameEngine.Instance = new GameEngine();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        
    }
}