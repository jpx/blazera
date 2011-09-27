using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public enum ActionType
    {
        Warp,
        TakeItem,
        Talk
    }

    public enum ConditionType
    {
        PossessesItem
    }

    public abstract class Action
    {
        protected static UInt32 ActionCount = 0;
        protected String GetId() { return "Action_" + ActionCount++.ToString(); }
        // script variable name
        public String Id { get; private set; }

        public ActionType Type { get; private set; }

        public Boolean InterruptsEvents { get; protected set; }

        List<Condition> Conditions;

        public Action(ActionType type)
        {
            Type = type;
            Conditions = new List<Condition>();
        }

        public Action(Action copy)
        {
            Type = copy.Type;

            InterruptsEvents = copy.InterruptsEvents;

            Conditions = new List<Condition>(copy.Conditions);
        }

        public void AddCondition(Condition condition)
        {
            Conditions.Add(condition);
        }

        public virtual Boolean Do(ObjectEventArgs args)
        {
            foreach (Condition condition in Conditions)
                if (!condition.IsValidated(args))
                    return false;

            return true;
        }

        public virtual String ToScript()
        {
            String toScript = "";

            foreach (Condition condition in Conditions)
            {
                condition.SetId(Id);

                toScript += condition.ToScript();

                toScript += Id + ":AddCondition ( " + condition.Id + " )";
            }

            return toScript;
        }

        public void SetId(String objectEventId)
        {
            Id = objectEventId + "_" + GetId();
        }
    }

    public abstract class Condition
    {
        protected UInt32 ConditionCount = 0;
        protected String GetId() { return "Condition_" + ConditionCount++.ToString(); }
        // script variable name
        public String Id { get; private set; }

        ConditionType Type;

        public Condition(ConditionType type)
        {
            Type = type;
        }

        public abstract Boolean IsValidated(ObjectEventArgs args);

        public abstract String ToScript();

        public void SetId(String actionId)
        {
            Id = actionId + "_" + GetId();
        }
    }
}
