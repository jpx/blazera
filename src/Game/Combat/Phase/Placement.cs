namespace BlazeraLib
{
    public class Placement : Phase
    {
        public Placement(Combat combat) :
            base(combat)
        {

        }

        public override void Start(StartInfo startInfo = null)
        {
            Combat.MainMenu.Close();

            Combat.Cursor.Open();
            Combat.Cursor.OnValidation += new CombatCursorEventHandler(Cursor_OnValidation);

            Combat.Cursor.SetCellPosition(new Vector2I(5, 5));
        }

        public override void Perform()
        {
        }

        public override void End()
        {
            Combat.Cursor.OnValidation -= new CombatCursorEventHandler(Cursor_OnValidation);
        }

        void Cursor_OnValidation(CombatCursor sender, CombatCursorEventArgs e)
        {
            Log.Cl("Your player is placed on cell : " + e.CellPosition.ToString());
          //  Combat.Cursor.Source.SetCellPositionTo(e.CellPosition);
        }
    }
}
