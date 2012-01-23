using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public enum ActionType
    {
        Default,
        Warp,
        TakeItem,
        Talk
    }

    public abstract class Action
    {
        #region Members

        protected static UInt32 ActionCount = 0;
        protected String GetId() { return "Action_" + ActionCount++.ToString(); }
        // script variable name
        public String Id { get; private set; }

        public ActionType Type { get; private set; }

        public Boolean InterruptsEvents { get; protected set; }

        #endregion Members

        public Action(ActionType type)
        {
            Type = type;
        }

        public Action(Action copy)
        {
            Type = copy.Type;

            InterruptsEvents = copy.InterruptsEvents;
        }

        public abstract Boolean Do(ObjectEventArgs args);

        public virtual String ToScript() { return string.Empty; }

        public void SetId(String objectEventId)
        {
            Id = objectEventId + "_" + GetId();
        }
    }
}
