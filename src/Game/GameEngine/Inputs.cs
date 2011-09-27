using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Window;

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
            List<KeyCode> Keys;

            Timer Timer;

            Boolean pIsActive;
            Boolean IsEventReleased;
            Boolean IsInputReleased;

            Boolean IsHandled;

            public GameInput(List<KeyCode> keys)
            {
                Keys = keys;

                Timer = new Timer();

                pIsActive = false;
                IsEventReleased = true;
                IsInputReleased = true;

                IsHandled = false;
            }

            public void UpdateState()
            {
                if (pIsActive &&
                    IsInputReleased)
                    IsInputReleased = false;

                IsHandled = false;
            }

            public Boolean UpdateState(BlzEvent evt)
            {
                IsHandled = false;

                if (evt.Type == EventType.KeyPressed &&
                    Keys.Contains(evt.Key.Code))
                {
                    if (pIsActive)
                        IsEventReleased = false;

                    pIsActive = true;

                    return true;
                }

                if (evt.Type != EventType.KeyReleased ||
                    !Keys.Contains(evt.Key.Code))
                    return false;

                pIsActive = false;
                IsEventReleased = true;
                IsInputReleased = true;

                return true;
            }

            public Boolean IsActive(double delay, BlzEvent evt, Boolean releaseMode, Boolean reset, bool handle)
            {
                if (evt != null)
                {
                    if (evt.Type == EventType.KeyPressed &&
                        Keys.Contains(evt.Key.Code))
                    {
                        pIsActive = true;

                        if (releaseMode &&
                            !IsEventReleased)
                            return false;

                        IsEventReleased = false;

                        if (!Timer.IsDelayCompleted(delay, reset))
                            return false;

                        if (!handle)
                            return true;

                        if (IsHandled)
                            return false;

                        IsHandled = true;

                        return true;
                    }
                    
                    else if (evt.Type == EventType.KeyReleased &&
                        Keys.Contains(evt.Key.Code))
                    {
                        IsEventReleased = true;
                        pIsActive = false;

                        IsHandled = false;

                        return true;
                    }

                    return false;
                }

                if (!pIsActive)
                    return false;

                if (IsHandled)
                    return false;

                if (releaseMode &&
                    !IsInputReleased)
                    return false;

                return Timer.IsDelayCompleted(delay, reset);
            }
        }

        public const double DEFAULT_INPUT_DELAY = 0D;

        Dictionary<InputType, GameInput> GameInputs;

        private Inputs()
        {
            GameInputs = new Dictionary<InputType, GameInput>()
            {
                { InputType.Up,         new GameInput(GameDatas.KEY_UP) },
                { InputType.Right,      new GameInput(GameDatas.KEY_RIGHT) },
                { InputType.Down,       new GameInput(GameDatas.KEY_DOWN) },
                { InputType.Left,       new GameInput(GameDatas.KEY_LEFT) },
                { InputType.Action,     new GameInput(GameDatas.KEY_ACTION) },
                { InputType.Back,       new GameInput(GameDatas.KEY_BACK) },
                { InputType.Misc,       new GameInput(GameDatas.KEY_MISC) },
                { InputType.Up2,        new GameInput(GameDatas.KEY_UP2) },
                { InputType.Right2,     new GameInput(GameDatas.KEY_RIGHT2) },
                { InputType.Down2,      new GameInput(GameDatas.KEY_DOWN2) },
                { InputType.Left2,      new GameInput(GameDatas.KEY_LEFT2) },
                { InputType.Action2,    new GameInput(GameDatas.KEY_ACTION2) },
                { InputType.Special0,   new GameInput(GameDatas.KEY_SPECIAL0) },
                { InputType.Special1,   new GameInput(GameDatas.KEY_SPECIAL1) },
                { InputType.Special2,   new GameInput(GameDatas.KEY_SPECIAL2) },
                { InputType.Special3,   new GameInput(GameDatas.KEY_SPECIAL3) },
                { InputType.Special4,   new GameInput(GameDatas.KEY_SPECIAL4) },
                { InputType.Special5,   new GameInput(GameDatas.KEY_SPECIAL5) },
                { InputType.Special6,   new GameInput(GameDatas.KEY_SPECIAL6) },
                { InputType.Special7,   new GameInput(GameDatas.KEY_SPECIAL7) },
                { InputType.Special8,   new GameInput(GameDatas.KEY_SPECIAL8) },
                { InputType.Special9,   new GameInput(GameDatas.KEY_SPECIAL9) }
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

        /// <summary>
        /// Modes :
        /// > 
        /// </summary>
        /// <param name="gameInputType"></param>
        /// <param name="evt"></param>
        /// <param name="releaseMode"></param>
        /// <param name="delay"></param>
        /// <returns></returns>
        public static Boolean IsGameInput(InputType gameInputType, BlzEvent evt = null, Boolean releaseMode = false, double delay = DEFAULT_INPUT_DELAY, bool handle = true)
        {
            return Instance.GameInputs[gameInputType].IsActive(delay, evt, releaseMode, true, handle);
        }
    }
}
