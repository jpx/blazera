namespace BlazeraLib
{
    public class TemporaryEvent : ObjectEvent
    {
        #region Constants

        const int DEFAULT_TEMP_COUNT = 1;
        const ObjectEventType DEFAULT_TYPE = ObjectEventType.Out;
        const bool DEFAULT_ACTION_KEY_MODE = false;
        const InputType DEFAULT_ACTION_KEY = InputType.Action;

        #endregion Constants

        #region Members

        int TempCount;

        #endregion Members

        public TemporaryEvent(int tempCount = DEFAULT_TEMP_COUNT, ObjectEventType type = DEFAULT_TYPE, bool actionKeyMode = DEFAULT_ACTION_KEY_MODE, InputType actionKey = DEFAULT_ACTION_KEY)
            : base(type, actionKeyMode, actionKey)
        {
            TempCount = tempCount;
        }

        public override bool Call(ObjectEventArgs args)
        {
            if (TempCount-- <= 0)
            {
                Parent.RemoveEvent(this);
                return true;
            }

            return base.Call(args);
        }
    }
}
