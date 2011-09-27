namespace BlazeraLib
{
    public class CombatActionMenu : BlazeraLib.Game.GraphicsEngine.GameGui.Combat.Menu
    {
        #region Members

        MenuItem AttackItem;
        MenuItem SpellItem;
        MenuItem ItemItem;
        MenuItem FuryItem;

        #endregion

        public CombatActionMenu(Combat combat) :
            base(combat)
        {
            AttackItem = new MenuItem("Attack", ITEM_TEXT_SIZE);
            AddItem(AttackItem);

            SpellItem = new MenuItem("Spell", ITEM_TEXT_SIZE);
            AddItem(SpellItem);

            ItemItem = new MenuItem("Item", ITEM_TEXT_SIZE);
            AddItem(ItemItem);

            FuryItem = new MenuItem("Fury", ITEM_TEXT_SIZE);
            AddItem(FuryItem);
        }

        public void AttachSpellMenu(SpellMenu spellMenu)
        {
            SpellItem.AttachMenu(spellMenu);
        }
    }
}
