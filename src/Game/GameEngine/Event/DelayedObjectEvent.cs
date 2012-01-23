namespace BlazeraLib
{
    public class DelayedObjectEvent : ObjectEvent
    {
        #region Members

        ObjectEvent InEvent;
        ObjectEvent OutEvent;

        Timer Timer;

        Time Delay;

        #endregion

        public DelayedObjectEvent(Time delay, bool actionKeyMode = false, InputType actionKey = InputType.Action)
            : base(ObjectEventType.Normal, actionKeyMode, actionKey)
        {
            Delay = delay;

            InEvent = new ObjectEvent(ObjectEventType.In);
            OutEvent = new ObjectEvent(ObjectEventType.Out);

            Timer = new Timer();

            InEvent.AddAction(new DefaultAction((args) => { Timer.Start(); }));
            OutEvent.AddAction(new DefaultAction((args) => { Timer.Reset(false); }));
        }

        public override bool Call(ObjectEventArgs args)
        {
            if (!Timer.IsDelayCompleted(Delay, false))
                return false;

            if (!base.Call(args))
                return false;

            Timer.Reset();

            return true;
        }

        public override void SetParent(EBoundingBox parent)
        {
            base.SetParent(parent);

            parent.AddEvent(InEvent);
            parent.AddEvent(OutEvent);
        }
    }
}
