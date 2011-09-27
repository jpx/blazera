using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class SpeechMessage
    {
        public String Message { get; private set; }

        public SpeechMessage(String message)
        {
            Message = message;
        }

        public SpeechMessage(SpeechMessage copy)
        {
            Message = copy.Message;
        }
    }
}
