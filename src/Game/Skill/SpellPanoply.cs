using System.Collections.Generic;

namespace BlazeraLib
{
    public class SpellPanoply
    {
        #region Members

        List<Spell> Spells;

        #endregion

        public SpellPanoply()
        {
            Spells = new List<Spell>();
        }

        public void AddSpell(Spell spell)
        {
            Spells.Add(spell);
        }

        public Spell GetSpell(string spellName)
        {
            foreach (Spell spell in Spells)
                if (spell.Name == spellName)
                    return spell;

            return null;
        }

        public IEnumerator<Spell> GetEnumrator()
        {
            return Spells.GetEnumerator();
        }
    }
}
