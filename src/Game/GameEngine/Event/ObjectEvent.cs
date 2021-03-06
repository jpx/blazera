﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public enum ObjectEventType
    {
        /// <summary>
        /// When the target is entering the event area.
        /// </summary>
        In,
        /// <summary>
        /// While the target is in the event area. Called at each loop.
        /// </summary>
        Normal,
        /// <summary>
        /// When the target is leaving the event area.
        /// </summary>
        Out
    }

    public class ObjectEvent
    {
        #region Members

        static UInt32 EventCount = 0;
        String GetId() { return "Event_" + EventCount++.ToString(); }
        // script variable name
        public String Id { get; private set; }

        public ObjectEventType Type { get; protected set; }

        public Boolean ActionKeyMode { get; protected set; }
        public InputType ActionKey { get; protected set; }

        public List<Action> Actions { get; private set; }

        protected EBoundingBox Parent { get; private set; }

        bool IsActive;

        #endregion Members

        public ObjectEvent(ObjectEventType type, Boolean actionKeyMode = false, InputType actionKey = InputType.Action)
        {
            Type = type;

            ActionKeyMode = actionKeyMode;
            ActionKey = actionKey;

            Actions = new List<Action>();

            IsActive = true;
        }

        public ObjectEvent(ObjectEvent copy)
        {
            Type = copy.Type;

            ActionKeyMode = copy.ActionKeyMode;
            ActionKey = copy.ActionKey;

            Actions = new List<Action>(copy.Actions);

            IsActive = copy.IsActive;
        }

        public void Activate(bool isActive = true)
        {
            IsActive = isActive;
        }

        public void FrontAddAction(Action action)
        {
            Actions.Insert(0, action);
        }

        public void AddAction(Action action)
        {
            Actions.Add(action);
        }

        public bool RemoveAction(Action action)
        {
            return Actions.Remove(action);
        }

        public virtual bool Call(ObjectEventArgs args)
        {
            if (!IsActive)
                return true;

            if (Actions.Count == 0)
                return false;

            if (ActionKeyMode &&
                !Inputs.IsGameInput(ActionKey, true, Inputs.DEFAULT_INPUT_DELAY, true, true))
                return false;

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

            return true;
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

        public virtual void SetParent(EBoundingBox parent)
        {
            Parent = parent;
        }

        public static ObjectEvent operator+(ObjectEvent evt, Action action)
        {
            evt.FrontAddAction(action);
            return evt;
        }

        public static ObjectEvent operator -(ObjectEvent evt, Action action)
        {
            evt.RemoveAction(action);
            return evt;
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
