namespace BlazeraLib
{
    public class EventTimer : Timer
    {
        public class EventTimerEventArgs { }
        public delegate void EventTimerEventHandler(EventTimer sender, EventTimerEventArgs e);

        public event EventTimerEventHandler Completed;
        bool CallCompleted() { if (Completed == null) return false; Completed(this, new EventTimerEventArgs()); return true; }

        #region Members

        double Delay;

        #endregion Members

        public EventTimer(double delay, bool start = false)
            : base(start)
        {
            SetDelay(delay);
        }

        public void SetDelay(double delay)
        {
            Delay = delay;
        }

        public void Update(bool reset = true, bool start = true)
        {
            if (IsDelayCompleted(Delay, reset, start))
                CallCompleted();
        }
    }
}
