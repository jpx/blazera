using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class SpeechManager
    {
        SpeechManager()
        {
            SpeechHandlers = new Queue<SpeechHandler>();
        }

        static SpeechManager _instance;
        public static SpeechManager Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new SpeechManager();
                return _instance;
            }
            set { _instance = value; }
        }

        Queue<SpeechHandler> SpeechHandlers;

        public Boolean AddSpeech(SpeechHandler speech, Personnage listener)
        {
            speech.Listeners.Clear();

            if (speech.IsEmpty())
                return false;

            speech.Listeners.Add(listener);

            SpeechHandlers.Enqueue(speech);

            return true;
        }

        public Boolean IsEmpty()
        {
            return SpeechHandlers.Count == 0;
        }

        public SpeechBubble GetNext()
        {
            return new SpeechBubble(SpeechHandlers.Dequeue());
        }
    }
}
