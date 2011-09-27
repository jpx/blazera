namespace BlazeraLib
{
    public class AttackCellSelection : TurnPhase
    {
        public AttackCellSelection(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Log("Attack cell selection...");
        }

        public override void Perform()
        {
        } 

        public override void End()
        {
        }
    }
}
