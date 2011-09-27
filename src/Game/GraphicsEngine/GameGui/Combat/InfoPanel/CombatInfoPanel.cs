namespace BlazeraLib
{
    public class CombatInfoPanel : InfoPanel
    {
        #region Members

        Combat Combat;

        #endregion

        public CombatInfoPanel(Combat combat) :
            base()
        {
            Combat = combat;
        }
    }
}
