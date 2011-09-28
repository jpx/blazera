namespace BlazeraLib
{
    public abstract class TurnPhase : Phase
    {
        #region Members

        protected BaseCombatant CurrentCombatant { get; private set; }

        #endregion

        public TurnPhase(Combat combat) :
            base(combat)
        {
            Combat.OnCombatantStartTurning += new CombatCombatantEventHandler(Combat_OnCombatantStartTurning);
        }

        void Combat_OnCombatantStartTurning(Combat sender, CombatCombatantEventArgs e)
        {
            CurrentCombatant = e.Combatant;
        }

        public override void Start(StartInfo startInfo = null)
        {
            Combat.Cursor.SetCellPosition(CurrentCombatant.CellPosition);
            Combat.AttachViewToCursor();
        }

        protected void Log(string message)
        {
            BlazeraLib.Log.Cl("[" + CurrentCombatant.Name + "] " + message);
        }
    }
}
