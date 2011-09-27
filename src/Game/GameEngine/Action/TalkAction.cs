using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class TalkAction : Action
    {
        public TalkAction() :
            base(ActionType.Talk)
        {
            InterruptsEvents = true;
        }

        public TalkAction(TalkAction copy) :
            base(copy)
        {
            InterruptsEvents = true;
        }

        public override Boolean Do(ObjectEventArgs args)
        {
            if (!base.Do(args))
                return false;

            if (!args.Trigger.Holder.DirectionInfo.IsFacing(args.Source.Holder))
                return false;

            if (!SpeechManager.Instance.AddSpeech(((Personnage)args.Source.Holder).SpeechHandler, (Personnage)args.Trigger.Holder))
                return false;

            return true;
        }
    }
}
