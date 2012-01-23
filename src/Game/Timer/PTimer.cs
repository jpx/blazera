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
            Period = period;
            Action = action;
            Dt = 0;
        }

        public void Update(Time dt)
        {
            Dt += dt.Value;
            if (Dt > Period)
            {
                Dt -= Period;
                Action.Invoke();
            }
        }

    }
}
