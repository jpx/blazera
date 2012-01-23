namespace BlazeraLib
{
    public class SpellCellSelection : TurnPhase
    {
        #region Members

        Spell CurrentSpell;
        CombatSpell Spell;

        bool SpellIsPerforming;

        #endregion

        public SpellCellSelection(Combat combat) :
            base(combat)
        {
        }

        public override void Start(StartInfo startInfo = null)
        {
            SpellIsPerforming = false;

            Log("Spell cell selection... [" + startInfo.GetArg<string>("Spell") + "]");

            Combat.DesactivateMainMenu();

            Combat.Cursor.SetCellPosition(CurrentCombatant.CellPosition);

            Combat.Cursor.OnValidation += new CombatCursorEventHandler(Cursor_OnValidation);
            Combat.Cursor.OnCancellation += new CombatCursorEventHandler(Cursor_OnCancellation);
            Combat.Cursor.OnMove += new CombatCursorMoveEventHandler(Cursor_OnMove);

            CurrentSpell = CurrentCombatant.SpellPanoply.GetSpell(startInfo.GetArg<string>("Spell"));
            Combat.AddCellColorEffect(CurrentSpell.RangeArea, CurrentCombatant.CellPosition, CellSelectionType.SpellRange);

            if (Combat.GetCell(CurrentCombatant.CellPosition).IsWithinArea(CurrentSpell.RangeArea, CurrentCombatant.CellPosition))
                Combat.AddCellColorEffect(CurrentSpell.EffectArea, CurrentCombatant.CellPosition, CellSelectionType.SpellArea);
            else
                Combat.AddCellColorEffect(CurrentSpell.EffectArea, CurrentCombatant.CellPosition, CellSelectionType.OutOfRange);
        }

        void Cursor_OnCancellation(CombatCursor sender, CombatCursorEventArgs e)
        {
            Combat.ChangeState(BlazeraLib.Combat.EState.ActionSelection);
        }

        void Cursor_OnMove(CombatCursor sender, CombatCursorMoveEventArgs e)
        {
            Combat.RemoveCellColorEffect(CurrentSpell.EffectArea, e.OldCellPosition, CellSelectionType.SpellArea);
            Combat.RemoveCellColorEffect(CurrentSpell.EffectArea, e.OldCellPosition, CellSelectionType.OutOfRange);

            if (Combat.GetCell(e.CellPosition).IsWithinArea(CurrentSpell.RangeArea, CurrentCombatant.CellPosition))
                Combat.AddCellColorEffect(CurrentSpell.EffectArea, e.CellPosition, CellSelectionType.SpellArea);
            else
                Combat.AddCellColorEffect(CurrentSpell.EffectArea, e.CellPosition, CellSelectionType.OutOfRange);
        }

        void Cursor_OnValidation(CombatCursor sender, CombatCursorEventArgs e)
        {
            if (!Combat.GetCell(Combat.Cursor.CellPosition).IsWithinArea(CurrentSpell.RangeArea, CurrentCombatant.CellPosition))
                return;

            Spell = new CombatSpell(CurrentSpell, CurrentCombatant);
           // Spell = new RandomEffectAreaSpell(CurrentSpell, CurrentCombatant);

            Log("launches " + Spell.GetName());

            Spell.Launch(e.CellPosition);
            Spell.OnStopping += new CombatSpellEventHandler(Spell_OnStopping);
            SpellIsPerforming = true;

            Combat.Cursor.Close();

            End();
        }

        void Spell_OnStopping(CombatSpell sender, CombatSpellEventArgs e)
        {
            Combat.ChangeState(BlazeraLib.Combat.EState.ActionSelection);
        }

        public override void Perform()
        {
            if (!SpellIsPerforming)
                return;

            Spell.Perform();
        }

        public override void End()
        {
            Combat.Cursor.OnValidation -= new CombatCursorEventHandler(Cursor_OnValidation);
            Combat.Cursor.OnCancellation -= new CombatCursorEventHandler(Cursor_OnCancellation);
            Combat.Cursor.OnMove -= new CombatCursorMoveEventHandler(Cursor_OnMove);

            Combat.ClearCellColorEffect();
        }
    }
}
