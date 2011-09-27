using System;
using System.Collections.Generic;
using System.Text;

namespace BlazeraLib
{
    public class Timer
    {
        public Timer()
        {
            this.InitTime = new DateTime();
            this.ElapsedTime = new TimeSpan(0, 0, 0);
        }

        public void Start()
        {
            this.InitTime = DateTime.Now;
        }

        public void Stop()
        {
            this.ElapsedTime.Subtract(this.ElapsedTime);
        }

        public void Reset()
        {
            this.Stop();
            this.Start();
        }

        public Time GetElapsedTime()
        {
            this.ElapsedTime = DateTime.Now.Subtract(this.InitTime);
            return new Time(this.ElapsedTime.TotalSeconds);
        }

        public Boolean IsDelayCompleted(Time time)
        {
            return IsDelayCompleted(time.Value);
        }

        public Boolean IsDelayCompleted(double delay, Boolean reset = true)
        {
            if (GetElapsedTime().Value < delay)
                return false;

            if (reset)
                Reset();

            return true;
        }

        /// <summary>
        /// Obtains the number of cycle(s) elapsed since start
        /// </summary>
        /// <param name="delay">Base period in s</param>
        /// <param name="reset">Reset the timer if period is outdated</param>
        /// <returns>Number of period(s) completed</returns>
        public UInt32 GetDelayFactor(double delay, Boolean reset = true)
        {
            UInt32 delayFactor = (UInt32)(GetElapsedTime().Value / delay);

            if (delayFactor > 0 &&
                reset)
                Reset();

            return delayFactor;
        }

        private DateTime InitTime
        {
            get;
            set;
        }

        private TimeSpan ElapsedTime
        {
            get;
            set;
        }
    }
}
