using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class GameDatas
    {
        public enum DifficultyLevel
        {
            Easy,
            Normal,
            Hard,
            Insane,
            God
        }

        const float DIFFICULTY_LEVEL_EASY_COEF = .5F;
        const float DIFFICULTY_LEVEL_NORMAL_COEF = 1F;
        const float DIFFICULTY_LEVEL_HARD_COEF = 2F;
        const float DIFFICULTY_LEVEL_INSANE_COEF = 5F;
        const float DIFFICULTY_LEVEL_GOD_COEF = 20F;

        public static float GetFifficultyLevelCoef(DifficultyLevel difficultyLevel)
        {
            switch (difficultyLevel)
            {
                case DifficultyLevel.Easy: return DIFFICULTY_LEVEL_EASY_COEF;
                case DifficultyLevel.Normal: return DIFFICULTY_LEVEL_NORMAL_COEF;
                case DifficultyLevel.Hard: return DIFFICULTY_LEVEL_HARD_COEF;
                case DifficultyLevel.Insane: return DIFFICULTY_LEVEL_INSANE_COEF;
                case DifficultyLevel.God: return DIFFICULTY_LEVEL_GOD_COEF;
                default: return DIFFICULTY_LEVEL_NORMAL_COEF;
            }
        }

        public const float STEP_PIXEL_VALUE = 5F;

        const float ITEM_DROP_RATE = 1F;
        const float XP_RATE = 1F;

        public static void Init()
        {
            SERVER_IO_TIMEOUT = ScriptEngine.GetInt("SERVER_IO_TIMEOUT");
            SERVER_MAXPLAYER = ScriptEngine.GetInt("SERVER_MAXPLAYER");
            SERVER_IP = ScriptEngine.GetString("SERVER_IP");
            SERVER_PORT = ScriptEngine.GetInt("SERVER_PORT");

            DEFAULT_WALK_VELOCITY = ScriptEngine.GetFloat("DEFAULT_WALK_VELOCITY");
            PERSONNAGE_RUN_VELOCITY_FACTOR = ScriptEngine.GetFloat("PERSONNAGE_RUN_VELOCITY_FACTOR");
            ANIMATION_DEFAULT_FRAMERATE = ScriptEngine.GetInt("ANIMATION_DEFAULT_FRAMERATE");
            ANIMATION_DEFAULT_STOPFRAME = ScriptEngine.GetInt("ANIMATION_DEFAULT_STOPFRAME");
            DEFAULT_DIRECTION = ScriptEngine.GetDirection("DEFAULT_DIRECTION");

            DEFAULT_FONT = new Font(GameDatas.DATAS_DEFAULT_PATH + "fonts/" + ScriptEngine.GetString("DEFAULT_FONT"));

            INIT_MAP = ScriptEngine.GetString("INIT_MAP");

            WINDOW_WIDTH = ScriptEngine.GetUInt("WINDOW_WIDTH");
            WINDOW_HEIGHT = ScriptEngine.GetUInt("WINDOW_HEIGHT");
            WINDOW_STYLE = ScriptEngine.GetStyles("WINDOW_STYLE");

            GROUND_DRAW_MARGIN = ScriptEngine.GetInt("GROUND_DRAW_MARGIN");
            TILE_SIZE = ScriptEngine.GetInt("TILE_SIZE");

            JOYSTICK_WALK_SENSIBILITY = ScriptEngine.GetFloat("JOYSTICK_WALK_SENSIBILITY");
            JOYSTICK_RUN_SENSIBILITY = ScriptEngine.GetFloat("JOYSTICK_RUN_SENSIBILITY");
            KEY_UP = ScriptEngine.GetList<Keyboard.Key>("KEY_UP");
            KEY_RIGHT = ScriptEngine.GetList<Keyboard.Key>("KEY_RIGHT");
            KEY_DOWN = ScriptEngine.GetList<Keyboard.Key>("KEY_DOWN");
            KEY_LEFT = ScriptEngine.GetList<Keyboard.Key>("KEY_LEFT");
            KEY_ACTION = ScriptEngine.GetList<Keyboard.Key>("KEY_ACTION");
            KEY_BACK = ScriptEngine.GetList<Keyboard.Key>("KEY_BACK");
            KEY_MISC = ScriptEngine.GetList<Keyboard.Key>("KEY_MISC");
            KEY_UP2 = ScriptEngine.GetList<Keyboard.Key>("KEY_UP2");
            KEY_RIGHT2 = ScriptEngine.GetList<Keyboard.Key>("KEY_RIGHT2");
            KEY_DOWN2 = ScriptEngine.GetList<Keyboard.Key>("KEY_DOWN2");
            KEY_LEFT2 = ScriptEngine.GetList<Keyboard.Key>("KEY_LEFT2");
            KEY_ACTION2 = ScriptEngine.GetList<Keyboard.Key>("KEY_ACTION2");
            KEY_SPECIAL0 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL0");
            KEY_SPECIAL1 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL1");
            KEY_SPECIAL2 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL2");
            KEY_SPECIAL3 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL3");
            KEY_SPECIAL4 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL4");
            KEY_SPECIAL5 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL5");
            KEY_SPECIAL6 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL6");
            KEY_SPECIAL7 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL7");
            KEY_SPECIAL8 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL8");
            KEY_SPECIAL9 = ScriptEngine.GetList<Keyboard.Key>("KEY_SPECIAL9");
        }

        public static int SERVER_IO_TIMEOUT;
        public static int SERVER_MAXPLAYER;
        public static String SERVER_IP;
        public static int SERVER_PORT;
        public static float DEFAULT_WALK_VELOCITY;
        public static float PERSONNAGE_RUN_VELOCITY_FACTOR;
        public static int ANIMATION_DEFAULT_FRAMERATE;
        public static int ANIMATION_DEFAULT_STOPFRAME;
        public static String DATAS_DEFAULT_PATH;
        public static String IMAGES_DEFAULT_PATH;
        public static String SCRIPTS_DEFAULT_PATH;
        public static String SOUNDS_DEFAULT_PATH;
        public static Direction DEFAULT_DIRECTION;
        public static Font DEFAULT_FONT;
        public static String INIT_MAP;
        public static uint WINDOW_WIDTH;
        public static uint WINDOW_HEIGHT;
        public static int GROUND_DRAW_MARGIN;
        public static int TILE_SIZE;
        public static Styles WINDOW_STYLE;
        public static float JOYSTICK_WALK_SENSIBILITY;
        public static float JOYSTICK_RUN_SENSIBILITY;
        public static List<Keyboard.Key> KEY_UP;
        public static List<Keyboard.Key> KEY_RIGHT;
        public static List<Keyboard.Key> KEY_DOWN;
        public static List<Keyboard.Key> KEY_LEFT;
        public static List<Keyboard.Key> KEY_ACTION;
        public static List<Keyboard.Key> KEY_BACK;
        public static List<Keyboard.Key> KEY_MISC;
        public static List<Keyboard.Key> KEY_UP2;
        public static List<Keyboard.Key> KEY_RIGHT2;
        public static List<Keyboard.Key> KEY_DOWN2;
        public static List<Keyboard.Key> KEY_LEFT2;
        public static List<Keyboard.Key> KEY_ACTION2;
        public static List<Keyboard.Key> KEY_SPECIAL0;
        public static List<Keyboard.Key> KEY_SPECIAL1;
        public static List<Keyboard.Key> KEY_SPECIAL2;
        public static List<Keyboard.Key> KEY_SPECIAL3;
        public static List<Keyboard.Key> KEY_SPECIAL4;
        public static List<Keyboard.Key> KEY_SPECIAL5;
        public static List<Keyboard.Key> KEY_SPECIAL6;
        public static List<Keyboard.Key> KEY_SPECIAL7;
        public static List<Keyboard.Key> KEY_SPECIAL8;
        public static List<Keyboard.Key> KEY_SPECIAL9;
    }
}
