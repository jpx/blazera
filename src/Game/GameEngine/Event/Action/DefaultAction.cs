namespace BlazeraLib
{
    public class DefaultAction : Action
    {
        public delegate void DCallBack(ObjectEventArgs args);

        #region Members

        DCallBack Callback;

        #endregion Members

        public DefaultAction(DCallBack callback)
            : base(ActionType.Default)
        {
            Callback = callback;
        }

        public override bool Do(ObjectEventArgs args)
        {
            if (Callback == null)
                return false;

            Callback(args);

            return true;
        }
    }
}
