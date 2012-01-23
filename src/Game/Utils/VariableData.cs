using System.Collections.Generic;

namespace BlazeraLib
{
    public abstract class VariableData<T>
    {
        #region Constants

        protected const int DEFAULT_PRECISION = 10000;
        const int DEFAULT_MIN_BOUND = 0;
        const int DEFAULT_MAX_BOUND = DEFAULT_PRECISION;

        #endregion Constants

        #region Members

        /// <summary>
        /// Point count on the curve.
        /// </summary>
        int Precision;

        SortedDictionary<int, T> KeyData;
        T[] Data;

        #endregion Members

        public VariableData(T boundData, int precision)
        {
            Precision = precision;

            KeyData = new SortedDictionary<int, T>();
            Data = new T[Precision];

            AddKeyData(DEFAULT_MIN_BOUND, boundData);
            AddKeyData(Precision, boundData);
        }

        public VariableData(VariableData<T> copy)
        {
            KeyData = new SortedDictionary<int, T>();
        }

        int GetLevel(float percentLevel)
        {
            return (int)(percentLevel * Precision);
        }

        public void AddKeyData(float percentLevel, T data)
        {
            AddKeyData(GetLevel(percentLevel), data);
        }

        public void AddKeyData(int level, T data)
        {
            if (level < 0 || level > Precision)
                throw new System.ArgumentOutOfRangeException("percentLevel");

            if (!KeyData.ContainsKey(level))
                KeyData.Add(level, data);
            else
                KeyData[level] = data;
        }

        void AddData(int level, T data)
        {
            Data[level] = data;
        }

        protected Vector2I GetBounds(int level)
        {
            Vector2I bounds = new Vector2I();

            IEnumerator<int> levelEnum = KeyData.Keys.GetEnumerator();
            int previousLevel = GetLevel(DEFAULT_MIN_BOUND);
            while (levelEnum.MoveNext())
            {
                if (levelEnum.Current >= level)
                {
                    bounds.X = previousLevel;
                    bounds.Y = levelEnum.Current;
                    break;
                }

                previousLevel = levelEnum.Current;
            }

            return bounds;
        }

        protected T GetKeyData(int level)
        {
            return KeyData[level];
        }

        protected T GetKeyData(float percentLevel)
        {
            return GetKeyData(GetLevel(percentLevel));
        }

        protected abstract T ComputeData(int level);

        public void ComputeData()
        {
            for (int level = DEFAULT_MIN_BOUND; level < Precision; ++level)
                AddData(level, ComputeData(level));
        }

        public void RemoveKeyData(int level)
        {
            KeyData.Remove(level);
        }

        public void RemoveKeyData(float percentLevel)
        {
            RemoveKeyData(GetLevel(percentLevel));
        }

        public T GetData(int level)
        {
            return Data[level];
        }

        public T GetData(float percentLevel)
        {
            return GetData(GetLevel(percentLevel));
        }

        public int GetCount()
        {
            return Precision;
        }
    }
}
