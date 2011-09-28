namespace BlazeraLib
{
    public class MoveCellSelection : TurnPhase
    {
        #region Members

        Vector2I CurrentCellPosition;

        #endregion

        public MoveCellSelection(Combat combat) :
            base(combat)
        {
            
        }

        public override void Start(StartInfo startInfo = null)
        {
            base.Start();

            Log("Move cell selection...");

            Combat.MainMenu.Close();
            Combat.InfoPanel.Close();

            Combat.Cursor.Open();

            Combat.Cursor.OnValidation += new CombatCursorEventHandler(Cursor_OnValidation);
            Combat.Cursor.OnCancellation += new CombatCursorEventHandler(Cursor_OnCancellation);

            CurrentCellPosition = CurrentCombatant.CellPosition;

            Combat.AddCellColorEffect(new CellArea(CellAreaType.Circle, 1, (int)CurrentCombatant.Status[BaseCaracteristic.Mp]), CurrentCombatant.CellPosition, CellSelectionType.Move);
        }

        void Cursor_OnCancellation(CombatCursor sender, CombatCursorEventArgs e)
        {
            Combat.ChangeState(BlazeraLib.Combat.EState.ActionSelection);
        }

        void Cursor_OnValidation(CombatCursor sender, CombatCursorEventArgs e)
        {
            if (!Combat.GetCell(e.CellPosition).IsWithinArea(new CellArea(CellAreaType.Circle, 1, (int)CurrentCombatant.Status[BaseCaracteristic.Mp]), CurrentCellPosition) ||
                !Combat.GetCell(e.CellPosition).IsUsable())
            {
                if (CurrentCellPosition == e.CellPosition)
                    Combat.ChangeState(BlazeraLib.Combat.EState.ActionSelection);

                return;
            }

            CurrentCombatant.OnMoveEnding += new CombatantMoveEventHandler(CurrentCombatant_OnMoveEnding);

            System.Collections.Generic.List<Vector2I> path = Combat.Map.GetPath(CurrentCellPosition, e.CellPosition);
            if (path == null)
            {
                Combat.ChangeState(BlazeraLib.Combat.EState.ActionSelection);
                return;
            }

            foreach (Vector2I cellPosition in path)
                CurrentCombatant.AddMovePoint(cellPosition);

            Combat.AttachViewToCurrentCombatant();

            Combat.Cursor.Close();
            End();
        }

        void CurrentCombatant_OnMoveEnding(BaseCombatant sender, CombatantMoveEventArgs e)
        {
            CurrentCombatant.OnMoveEnding -= new CombatantMoveEventHandler(CurrentCombatant_OnMoveEnding);
            Combat.ChangeState(BlazeraLib.Combat.EState.ActionSelection);
        }

        public override void Perform()
        {
        }

        public override void End()
        {
            Combat.Cursor.OnValidation -= new CombatCursorEventHandler(Cursor_OnValidation);
            Combat.Cursor.OnCancellation -= new CombatCursorEventHandler(Cursor_OnCancellation);

            Combat.RemoveCellColorEffect(new CellArea(CellAreaType.Circle, 1, (int)CurrentCombatant.Status[BaseCaracteristic.Mp]), CurrentCombatant.CellPosition, CellSelectionType.Move);
           // Combat.ClearCellColorEffect();
        }
    }
}
