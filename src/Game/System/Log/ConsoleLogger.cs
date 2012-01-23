namespace BlazeraLib
{
    public class ConsoleLogger : ILogger
    {
        #region Singleton

        static ConsoleLogger _instance;
        public static ConsoleLogger Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new ConsoleLogger();

                return _instance;
            }
            set { _instance = value; }
        }

        #endregion Singleton

        public ILogger Log(object obj)
        {
            System.Console.WriteLine(obj);

            return Instance;
        }
    }
}
