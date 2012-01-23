using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;
using SFML.Graphics;

namespace BlazeraLib
{
    public enum InputType
    {
        Up,
        Right,
        Down,
        Left,
        Action,
        Back,
        Misc,

        Up2,
        Right2,
        Down2,
        Left2,
        Action2,

        Special0,
        Special1,
        Special2,
        Special3,
        Special4,
        Special5,
        Special6,
        Special7,
        Special8,
        Special9
    }

    public class Inputs
    {
        class GameInput
        {
            List<Keyboard.Key> Keys;

            Timer Timer;

            bool IsDown;
            bool WasReleased;

            bool EventIsHandled;
            bool InputIsHandled;

            public GameInput(List<Keyboard.Key> keys)
            {
                Keys = keys;

                Timer = new Timer(true);

                IsDown = false;
                WasReleased = true;

                EventIsHandled = false;
                InputIsHandled = false;
            }

            public void UpdateState()
            {
                InputIsHandled = false;
            }

            public Boolean UpdateState(BlzEvent evt)
            {
                EventIsHandled = false;

                if (evt.Type == EventType.KeyPressed &&
                    Keys.Contains(evt.Key.Code))
                {
                    if (IsDown)
                        WasReleased = false;

                    IsDown = true;

                    return true;
                }

                if (evt.Type != EventType.KeyReleased ||
                    !Keys.Contains(evt.Key.Code))
                    return false;

                IsDown = false;
                WasReleased = true;

                return true;
            }

            public bool IsActive(BlzEvent evt, bool releaseMode, double delay, bool resetTimer, bool handleEvent, bool checkEventIsHandled)
            {
                if (!Keys.Contains(evt.Key.Code))
                    return false;

                if (evt.Type != EventType.KeyPressed)
                {
                    if (evt.Type != EventType.KeyReleased)
                        return false;

                    return true;
                }

                if (releaseMode &&
                    !WasReleased)
                    return false;

                if (checkEventIsHandled && EventIsHandled)
                    return false;

                if (!Timer.IsDelayCompleted(delay, resetTimer))
                    return false;

                if (handleEvent)
                {
                    EventIsHandled = true;
                    WasReleased = false;
                    //EventWasReleased = false;
                }

                return true;
            }

            public Boolean IsActive(bool releaseMode, double delay, bool resetTimer, bool handleInput, bool checkInputIsHandled)
            {
                if (!IsDown)
                    return false;

                if (releaseMode &&
                    !WasReleased)
                    return false;

                if (checkInputIsHandled && InputIsHandled)
                    return false;

                if (!Timer.IsDelayCompleted(delay, resetTimer))
                    return false;

                if (handleInput)
                {
                    InputIsHandled = true;
                    WasReleased = false;
                }

                return true;
            }
        }

        public const double DEFAULT_INPUT_DELAY = 0D;

        Dictionary<InputType, GameInput> GameInputs;

        private Inputs()
        {
            GameInputs = new Dictionary<InputType, GameInput>()
            {
                { InputType.Up,         new GameInput(GameData.KEY_UP) },
                { InputType.Right,      new GameInput(GameData.KEY_RIGHT) },
                { InputType.Down,       new GameInput(GameData.KEY_DOWN) },
                { InputType.Left,       new GameInput(GameData.KEY_LEFT) },
                { InputType.Action,     new GameInput(GameData.KEY_ACTION) },

                { InputType.Back,       new GameInput(GameData.KEY_BACK) },
                { InputType.Misc,       new GameInput(GameData.KEY_MISC) },

                { InputType.Up2,        new GameInput(GameData.KEY_UP2) },
                { InputType.Right2,     new GameInput(GameData.KEY_RIGHT2) },
                { InputType.Down2,      new GameInput(GameData.KEY_DOWN2) },
                { InputType.Left2,      new GameInput(GameData.KEY_LEFT2) },
                { InputType.Action2,    new GameInput(GameData.KEY_ACTION2) },

                { InputType.Special0,   new GameInput(GameData.KEY_SPECIAL0) },
                { InputType.Special1,   new GameInput(GameData.KEY_SPECIAL1) },
                { InputType.Special2,   new GameInput(GameData.KEY_SPECIAL2) },
                { InputType.Special3,   new GameInput(GameData.KEY_SPECIAL3) },
                { InputType.Special4,   new GameInput(GameData.KEY_SPECIAL4) },
                { InputType.Special5,   new GameInput(GameData.KEY_SPECIAL5) },
                { InputType.Special6,   new GameInput(GameData.KEY_SPECIAL6) },
                { InputType.Special7,   new GameInput(GameData.KEY_SPECIAL7) },
                { InputType.Special8,   new GameInput(GameData.KEY_SPECIAL8) },
                { InputType.Special9,   new GameInput(GameData.KEY_SPECIAL9) }
            };
        }

        public void Init()
        {
        }

        private static Inputs _instance;
        public static Inputs Instance
        {
            get
            {
                if (_instance == null)
                {
                    Inputs.Instance = new Inputs();
                }
                return _instance;
            }
            set
            {
                _instance = value;
            }
        }

        public void UpdateState()
        {
            foreach (GameInput gameInput in GameInputs.Values)
                gameInput.UpdateState();
        }

        public Boolean UpdateState(BlzEvent evt)
        {
            foreach (GameInput gameInput in GameInputs.Values)
                if (gameInput.UpdateState(evt))
                    return true;

            return false;
        }


        public static bool IsGameInput(InputType type, BlzEvent evt, bool releaseMode = false, double delay = DEFAULT_INPUT_DELAY, bool handle = true, bool checkIsHandled = true)
        {
            return Instance.GameInputs[type].IsActive(evt, releaseMode, delay, true, handle, checkIsHandled);
        }

        public static bool IsGameInput(InputType type, bool releaseMode = false, double delay = DEFAULT_INPUT_DELAY, bool handle = true, bool checkIsHandled = false)
        {
            return Instance.GameInputs[type].IsActive(releaseMode, delay, true, handle, checkIsHandled);
        }
    }
}
