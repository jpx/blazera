using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraLib
{
    public class Time
    {
        public Time(double value)
        {
            this.Value = value;
        }

        public double MS
        {
            get
            {
                return this.Value * 1000D;
            }
        }

        public double Value
        {
            get;
            set;
        }

        public double Minutes
        {
            get
            {
                return this.Value / 60D;
            }
        }

        public double Hours
        {
            get
            {
                return this.Minutes / 60D;
            }
        }
    }

    public class GameTime
    {
        private GameTime()
        {
            this.Timer = new Timer();
        }

        public void Init()
        {
            this.Init(0.0D);
        }

        public void Init(double initTotalTime)
        {
            GameTime.Dt = new Time(0D);
            this.SessionTime = new Time(0D);
            this.TotalTime = new Time(initTotalTime);
            this.Timer.Start();
        }

        private static GameTime _instance;
        public static GameTime Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameTime.Instance = new GameTime();
                }
                return _instance;
            }
            set
            {
                _instance = new GameTime();
            }
        }

        public static Time Dt
        {
            get;
            private set;
        }

        public static Time GetDt()
        {
            GameTime.Dt.Value = GameTime.Instance.Timer.GetElapsedTime().Value - GameTime.Instance.SessionTime.Value;
            GameTime.Instance.SessionTime.Value += GameTime.Dt.Value;
            GameTime.Instance.TotalTime.Value += GameTime.Dt.Value;
            return GameTime.Dt;
        }

        public static Time GetSessionTime()
        {
            return GameTime.Instance.SessionTime;
        }

        public static Time GetTotalTime()
        {
            return GameTime.Instance.TotalTime;
        }

        private Timer Timer
        {
            get;
            set;
        }

        private Time SessionTime
        {
            get;
            set;
        }

        private Time TotalTime
        {
            get;
            set;
        }
    }
}