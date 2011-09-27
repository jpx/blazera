using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public delegate void PTimerAction();

    public class PTimer
    {
        private double Period
        {
            get;
            set;
        }

        private PTimerAction Action
        {
            get;
            set;
        }

        private double Dt
        {
            get;
            set;
        }

        public PTimer(double period, PTimerAction action)
        {
            this.Period = period;
            this.Action = action;
            this.Dt = 0;
        }

        public void Update(Time dt)
        {
            this.Dt += dt.Value;
            if (this.Dt > this.Period)
            {
                this.Dt -= this.Period;
                this.Action.Invoke();
            }
        }

    }
}
