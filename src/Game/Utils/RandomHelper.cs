using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class RandomHelper
    {
        private RandomHelper()
        {
            Random = new Random();
        }

        public void Init()
        {
            
        }

        private static RandomHelper _instance;
        public static RandomHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    RandomHelper.Instance = new RandomHelper();
                }
                return _instance;
            }
            private set
            {
                _instance = value;
            }
        }

        private Random Random
        {
            get;
            set;
        }

        /// <summary>
        /// Get randomly an int between min (included) and max (included)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int Get(int min, int max)
        {
            return min + RandomHelper.Instance.Random.Next(max + 1 - min);
        }

        public static float Get(float min, float max)
        {
            return min + (float)RandomHelper.Instance.Random.NextDouble() * (max - min);
        }

        public static byte Get(byte min, byte max)
        {
            return (byte)(min + (RandomHelper.Instance.Random.NextDouble() * (max - min)));
        }
    }
}
