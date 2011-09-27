using System.Collections.Generic;

namespace BlazeraLib
{
    public class CombatantQueue
    {
        #region Classes

        #region SpeedComparer

        class SpeedComparer : IComparer<BaseCombatant>
        {
            public int Compare(BaseCombatant combatant1, BaseCombatant combatant2)
            {
                return 0;
            }
        }

        #endregion

        #endregion

        #region Members

        List<BaseCombatant> Combatants;
        int CurrentCombatant;

        #endregion

        public CombatantQueue()
        {
            CurrentCombatant = 0;
        }

        public void AddCombatants(List<BaseCombatant> combatants)
        {
            Combatants = new List<BaseCombatant>(combatants);

            Combatants.Sort(new SpeedComparer());
        }

        public BaseCombatant GetNextAliveCombat()
        {
            if (Combatants.Count == 0)
                return null;

            BaseCombatant nextAliveCombatant = Combatants[CurrentCombatant];

            CurrentCombatant = NextIndex();

            while (nextAliveCombatant != null
                && false) // is not alive
            {
                Combatants.Remove(nextAliveCombatant);
                nextAliveCombatant = Combatants[CurrentCombatant];
                CurrentCombatant = NextIndex();
            }

            return nextAliveCombatant;
        }

        int NextIndex()
        {
            if (CurrentCombatant >= Combatants.Count - 1)
                return 0;

            return CurrentCombatant + 1;
        }
    }
}
