using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace BlazeraServer
{
    class Program
    {
        static void Main()
        {
            BlazeraServer.Instance.Init();

            BlazeraServer.Instance.Run();
        }
    }
}
