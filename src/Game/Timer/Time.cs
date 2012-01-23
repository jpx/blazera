namespace BlazeraLib
{
    public class Time
    {
        public Time(double value = 0D)
        {
            Value = value;
        }

        public double MS
        {
            get
            {
                return Value * 1000D;
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
                return Value / 60D;
            }
        }

        public double Hours
        {
            get
            {
                return Minutes / 60D;
            }
        }

        public static Time operator+(Time t1, Time t2)
        {
            return new Time(t1.Value + t2.Value);
        }

        public static Time operator -(Time t1, Time t2)
        {
            return new Time(t1.Value - t2.Value);
        }

        public static implicit operator double(Time t)
        {
            return t.Value;
        }

        public static implicit operator Time(double value)
        {
            return new Time(value);
        }
    }
}
