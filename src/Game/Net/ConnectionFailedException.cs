using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class ConnectionFailedException : Exception
    {
        public override string Message
        {
            get
            {
                return "Failed to connect to the server";
            }
        }
    }
}
