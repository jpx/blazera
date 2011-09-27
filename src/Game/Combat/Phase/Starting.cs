namespace BlazeraLib
{
    public class Starting : Phase
    {
        public Starting(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Combat.Start();
            Combat.ChangeState(BlazeraLib.Combat.EState.TurnStart);
        }

        public override void Perform()
        {
        }

        public override void End()
        {
        }
    }
}
