namespace BlazeraLib
{
    public class ExploreCellSelection : TurnPhase
    {
        public ExploreCellSelection(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Log("Explore cell selection...");
        }

        public override void Perform()
        {
        }

        public override void End()
        {
        }
    }
}