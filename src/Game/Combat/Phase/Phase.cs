using System.Collections.Generic;

namespace BlazeraLib
{
    public abstract class Phase
    {
        #region Classes

        #region StartInfo

        public class StartInfo
        {
            Dictionary<string, object> Args;

            public StartInfo(Dictionary<string, object> args = null)
            {
                Args = args;
            }

            public T GetArg<T>(string key)
            {
                return (T)Args[key];
            }
        }

        #endregion

        #endregion

        #region Members

        protected Combat Combat { get; private set; }

        #endregion

        public Phase(Combat combat)
        {
            Combat = combat;
        }

        public abstract void Start(StartInfo startInfo = null);
        public abstract void Perform();
        public abstract void End();

        public void Log(string message)
        {
            Combat.Log(message);
        }
    }
}
