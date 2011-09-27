using System.Collections.Generic;

namespace BlazeraLib
{
    public class Team
    {
        #region Constants

        #endregion

        #region Members

        List<BaseCombatant> Combatants;
        public IEnumerator<BaseCombatant> GetCombatantEnumerator() { return Combatants.GetEnumerator(); }

        #endregion

        public Team()
        {
            Combatants = new List<BaseCombatant>();
        }

        public void AddCombatant(BaseCombatant combatant)
        {
            Combatants.Add(combatant);
        }

        public bool RemoveCombatant(BaseCombatant combatant)
        {
            return Combatants.Remove(combatant);
        }
    }
}
