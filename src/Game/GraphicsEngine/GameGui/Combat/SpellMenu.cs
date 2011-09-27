using System.Collections.Generic;

namespace BlazeraLib
{
    public class SpellMenu : BlazeraLib.Game.GraphicsEngine.GameGui.Combat.ExtendedMenu
    {
        #region Members

        BaseCombatant CurrentCombatant;

        #endregion

        public SpellMenu(Combat combat) :
            base(combat)
        {
            Combat.OnCombatantStartTurning += new CombatCombatantEventHandler(Combat_OnCombatantStartTurning);
        }

        void Combat_OnCombatantStartTurning(Combat sender, CombatCombatantEventArgs e)
        {
            Build(e.Combatant);
        }

        public void Build(BaseCombatant combatant)
        {
            Clear();

            CurrentCombatant = combatant;
            IEnumerator<Spell> spells = CurrentCombatant.SpellPanoply.GetEnumrator();
            while (spells.MoveNext())
                AddSpellItem(spells.Current);
        }

        void AddSpellItem(Spell spell)
        {
            MenuItem spellItem = new MenuItem(spell.Name, ITEM_TEXT_SIZE);
            spellItem.Validated += new ValidationEventHandler(spellItem_Validated);
            AddItem(spellItem);
        }

        void spellItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Combat.ChangeState(BlazeraLib.Combat.EState.SpellCellSelection, new Phase.StartInfo(new Dictionary<string,object>()
            {
                { "Spell", sender.GetText() }
            }));
        }
    }
}
