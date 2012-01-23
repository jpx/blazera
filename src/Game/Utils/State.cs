using System;

namespace BlazeraLib
{
    public class State<T> : IEquatable<State<T>> where T : class
    {
        #region Members

        static StateTable States = new StateTable();

        public T Value { get; private set; }

        #endregion

        public State(T value)
        {
            Value = value;

            States.Add(Value);
        }

        public int GetId()
        {
            return States.GetId(Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        public bool Equals(State<T> other)
        {
            return Value.Equals(other.Value);
        }

        public static bool operator ==(State<T> left, State<T> right)
        {
            if ((object)left == null)
                return ((object)right == null);

            if ((object)right == null)
                return ((object)left == null);

            return left.Equals(right);
        }

        public static bool operator !=(State<T> left, State<T> right)
        {
            return !(left == right);
        }

        public static implicit operator T(State<T> state)
        {
            if (state == null)
                return null;

            return state.Value;
        }

        public static implicit operator State<T>(T t)
        {
            if (t == null)
                return null;

            return new State<T>(t);
        }

        public override string ToString()
        {
            return Value.ToString();
        }
    }
}
