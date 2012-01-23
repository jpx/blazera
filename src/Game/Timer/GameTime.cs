using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraLib
{
    public class GameTime
    {
        #region Members

        public static Time Dt { get; private set; }

        Timer Timer;

        Time SessionTime;
        Time TotalTime;

        #endregion Members

        private GameTime()
        {
            Timer = new Timer();
        }

        public void Init()
        {
            Init(0.0D);
        }

        public void Init(double initTotalTime)
        {
            Dt = new Time();

            SessionTime = new Time();
            TotalTime = new Time(initTotalTime);

            Timer.Start();
        }

        private static GameTime _instance;
        public static GameTime Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new GameTime();
                return _instance;
            }
            set
            {
                _instance = new GameTime();
            }
        }

        public static void Update()
        {
            Dt = Instance.Timer.GetElapsedTime() - Instance.SessionTime;
            Instance.SessionTime.Value += Dt.Value;
            Instance.TotalTime.Value += Dt.Value;
        }

        public static Time GetSessionTime()
        {
            return Instance.SessionTime;
        }

        public static Time GetTotalTime()
        {
            return Instance.TotalTime;
        }

    }
}