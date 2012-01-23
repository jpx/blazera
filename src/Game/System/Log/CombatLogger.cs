namespace BlazeraLib
{
    public class CombatLogger : ILogger
    {
        public ILogger Log(object obj)
        {
            ConsoleLogger.Instance.Log(obj);

            return this;
        }
    }
}
