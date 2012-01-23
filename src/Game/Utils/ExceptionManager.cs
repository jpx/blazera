using System;

namespace BlazeraLib
{
    public static class ExceptionManager
    {
        public static void ArgIsNull(string message = null, Exception innerException = null)
        {
            throw new ArgumentNullException(message, innerException);
        }

        public static void RefIsNull(string message = null, Exception innerException = null)
        {
            throw new NullReferenceException(message, innerException);
        }
    }
}
