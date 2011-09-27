namespace BlazeraLib
{
    public class TurnOver : TurnPhase
    {
        public TurnOver(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Log("Turn is over...");
            Combat.ChangeState(BlazeraLib.Combat.EState.TurnStart);
        }

        public override void Perform()
        {
        }

        public override void End()
        {
            Combat.ChangeTurn();
        }
    }
}
