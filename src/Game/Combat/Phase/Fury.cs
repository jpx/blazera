namespace BlazeraLib
{
    public class Fury : TurnPhase
    {
        public Fury(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Log("Fury");
        }

        public override void Perform()
        {
        } 

        public override void End()
        {
        }
    }
}
