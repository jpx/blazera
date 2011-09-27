using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class SpeechHandler
    {
        List<SpeechMessage> Messages;
        public Personnage Speaker { get; private set; }
        public List<Personnage> Listeners { get; private set; }

        public SpeechHandler(Personnage speaker)
        {
            Messages = new List<SpeechMessage>();
            Speaker = speaker;
            Listeners = new List<Personnage>();
        }

        public SpeechHandler(Personnage speaker, Personnage listener) :
            this(speaker)
        {
            Listeners.Add(listener);
        }

        public SpeechHandler(Personnage speaker, SpeechHandler copy)
        {
            Messages = new List<SpeechMessage>(copy.Messages);
            Speaker = speaker;
            Listeners = new List<Personnage>(copy.Listeners);
        }

        public void AddMessage(String message)
        {
            Messages.Add(new SpeechMessage(message));
        }

        public Boolean IsEmpty()
        {
            return Messages.Count == 0;
        }

        public void Generate(SpeechBubble speechBubble, Boolean launch = false)
        {
            foreach (SpeechMessage message in Messages)
                speechBubble.AddMessage(message.Message);

            if (launch)
                speechBubble.ResetSpeech(launch);
        }
    }
}
