namespace BlazeraLib
{
    public class Timer
    {
        System.Diagnostics.Stopwatch Sw;

        public Timer(bool start = false)
        {
            Sw = new System.Diagnostics.Stopwatch();

            if (start)
                Start();
        }

        public void Start()
        {
            Sw.Start();
        }

        public void Stop()
        {
            Sw.Stop();
        }

        public void Reset(bool start = true)
        {
            Sw.Reset();

            if (start)
                Start();
        }

        public Time GetElapsedTime()
        {
            double elapsedTime = (double)Sw.ElapsedTicks / (double)System.Diagnostics.Stopwatch.Frequency;
            return new Time(elapsedTime);
        }

        public bool IsDelayCompleted(double delay, bool reset = true, bool start = true)
        {
            if (GetElapsedTime().Value < delay)
                return false;

            if (reset)
                Reset(start);

            return true;
        }

        /// <summary>
        /// Obtains the number of cycle(s) elapsed since start
        /// </summary>
        /// <param name="delay">Base period in s</param>
        /// <param name="reset">Reset the timer if period is outdated</param>
        /// <returns>Number of period(s) completed</returns>
        public uint GetDelayFactor(double delay, bool reset = true)
        {
            uint delayFactor = (uint)(GetElapsedTime().Value / delay);

            if (delayFactor > 0 &&
                reset)
                Reset();

            return delayFactor;
        }
    }
}
