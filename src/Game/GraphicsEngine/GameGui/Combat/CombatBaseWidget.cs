namespace BlazeraLib
{
    public class CombatBaseWidget : GameWidget
    {
        Combat Combat;

        public CombatMainMenu MainMenu { get; private set; }
        public CombatActionMenu ActionMenu { get; private set; }
        public SpellMenu SpellMenu { get; private set; }

        public CombatCursor Cursor { get; private set; }

        public CombatInfoPanel InfoPanel { get; private set; }

        public CombatBaseWidget(Combat combat)
            : base()
        {
            Combat = combat;

            ActionMenu = new CombatActionMenu(Combat);
            MainMenu = new CombatMainMenu(Combat);
            SpellMenu = new SpellMenu(Combat);
            ActionMenu.AttachSpellMenu(SpellMenu);

            Cursor = new CombatCursor(Combat);

            InfoPanel = new CombatInfoPanel(Combat);
            InfoPanel.AddBox(new CombatantInfoPanelBox());

            AddWidget(ActionMenu);
            AddWidget(MainMenu);
            AddWidget(SpellMenu);
            AddWidget(Cursor);
            AddWidget(InfoPanel);

            InfoPanel.SetLocation(GameWidget.ELocation.BottomRight);
            MainMenu.SetLocation(GameWidget.ELocation.BottomLeft);

            Cursor.SetFirst();
        }
    }
}
