namespace BlazeraLib
{
    public class TurnStart : TurnPhase
    {
        public TurnStart(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Log("Turn starts...");
            Combat.InfoPanel.BuildBox("Combatant", new InfoPanelBox.BuildInfo(new System.Collections.Generic.Dictionary<string, object>() { { "Combatant", CurrentCombatant } }));
            Combat.ChangeState(BlazeraLib.Combat.EState.ActionSelection);
        }

        public override void Perform()
        {
        }

        public override void End()
        {
        }
    }
}
