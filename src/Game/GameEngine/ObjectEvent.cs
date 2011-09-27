using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public enum ObjectEventType
    {
        In,
        Normal,
        Out
    }

    public class ObjectEvent
    {
        static UInt32 EventCount = 0;
        String GetId() { return "Event_" + EventCount++.ToString(); }
        // script variable name
        public String Id { get; private set; }

        public ObjectEventType Type { get; protected set; }

        public Boolean ActionKeyMode { get; protected set; }
        public InputType ActionKey { get; protected set; }

        public List<Action> Actions { get; private set; }

        public ObjectEvent(ObjectEventType type, Boolean actionKeyMode = false, InputType actionKey = InputType.Action)
        {
            Type = type;

            ActionKeyMode = actionKeyMode;
            ActionKey = actionKey;

            Actions = new List<Action>();
        }

        public ObjectEvent(ObjectEvent copy)
        {
            Type = copy.Type;

            ActionKeyMode = copy.ActionKeyMode;
            ActionKey = copy.ActionKey;

            Actions = new List<Action>(copy.Actions);
        }

        public void AddAction(Action action)
        {
            Actions.Add(action);
        }

        public void CallEvent(ObjectEventArgs args)
        {
            if (Actions.Count == 0)
                return;

            if (ActionKeyMode &&
                !Inputs.IsGameInput(ActionKey, null, true))
                return;

            Boolean interruptsEvents = false;
            foreach (Action action in Actions)
                if (action.Do(args) && action.InterruptsEvents)
                    interruptsEvents = true;

            if (interruptsEvents)
            {
                args.Map.InterruptEvents();
                args.Map.RefreshEvents();
                args.Map.EventsAreInterrupted = true;
            }
        }

        public void CallEvent(EBoundingBox source, EBoundingBox trigger)
        {
            CallEvent(new ObjectEventArgs(source, trigger));
        }

        public String ToScript()
        {
            String toScript = "";

            toScript += Id + " = ObjectEvent ( " +
                "ObjectEventType." + Type.ToString() + ", " +
                (ActionKeyMode ? "true" : "false") +
                ", InputType." + ActionKey.ToString() + " )\n";

            foreach (Action action in Actions)
            {
                action.SetId(Id);

                toScript += action.ToScript() + "\n";

                toScript += Id + ":AddAction ( " + action.Id + " )";
            }

            return toScript;
        }

        public void SetId(String BBId)
        {
            Id = BBId + "_" + GetId();
        }
    }

    public class ObjectEventArgs
    {
        public EBoundingBox Source { get; private set; }
        public EBoundingBox Trigger { get; private set; }

        public Player Player { get; private set; }
        public Map Map { get; private set; }

        public ObjectEventArgs(EBoundingBox source, EBoundingBox trigger)
        {
            Source = source;
            Trigger = trigger;

            if (Trigger.Holder is Player)
                Player = (Player)Trigger.Holder;
            else if (Source.Holder is Player)
                Player = (Player)Source.Holder;

            Map = source.Holder.Map;
        }
    }
}
