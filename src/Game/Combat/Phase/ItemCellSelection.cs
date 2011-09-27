namespace BlazeraLib
{
    public class ItemCellSelection : TurnPhase
    {
        public ItemCellSelection(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Log("Item cell selection...");
        }

        public override void Perform()
        {
        } 

        public override void End()
        {
        }
    }
}
