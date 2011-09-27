namespace BlazeraLib
{
    public class ActionSelection : TurnPhase
    {
        public ActionSelection(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            base.Start();

            Combat.ActivateMainMenu();
        }

        public override void Perform()
        {
        }

        public override void End()
        {
        }
    }
}