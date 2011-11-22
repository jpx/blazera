using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    #region Event handlers

    public class CombatCursorEventArgs : System.EventArgs
    {
        public Vector2I CellPosition { get; private set; }
        public CombatCursorEventArgs(Vector2I cellPosition) { CellPosition = cellPosition; }
    }

    public class CombatCursorMoveEventArgs : CombatCursorEventArgs
    {
        public Vector2I OldCellPosition { get; private set; }
        public CombatCursorMoveEventArgs(Vector2I oldCellPosition, Vector2I cellPosition) : base(cellPosition) { OldCellPosition = oldCellPosition; }
    }

    public delegate void CombatCursorEventHandler(CombatCursor sender, CombatCursorEventArgs e);
    public delegate void CombatCursorMoveEventHandler(CombatCursor sender, CombatCursorMoveEventArgs e);

    #endregion

    public class CombatCursor : GameWidget
    {
        #region Constants

        public const float TRANSITION_VELOCITY = 50F;
        const double CRUSOR_MOVE_DELAY = .2D;
        const double FAST_CURSOR_MOVE_DELAY = .04D;
        const int MOVE_CELL_OFFSET = 1;

        #endregion

        #region Members

        CombatCursorShape Cursor;
        bool FastCursorMode;

        public Vector2I CellPosition { get; private set; }

        Combat Combat;

        public BaseCombatant Source { get; set; }
        BaseCombatant Target;

        #endregion

        #region Events

        public event CombatCursorEventHandler OnValidation;
        bool CallOnValidation() { if (OnValidation == null) return false; OnValidation(this, new CombatCursorEventArgs(CellPosition)); return true; }

        public event CombatCursorEventHandler OnCancellation;
        bool CallOnCancellation() { if (OnCancellation == null) return false; OnCancellation(this, new CombatCursorEventArgs(CellPosition)); return true; }

        public event CombatCursorMoveEventHandler OnMove;
        bool CallOnMove(Vector2I oldCellPosition, Vector2I cellPosition) { if (OnMove == null) return false; OnMove(this, new CombatCursorMoveEventArgs(oldCellPosition, cellPosition)); return true; }

        #endregion

        public CombatCursor(Combat combat) :
            base()
        {
            Combat = combat;

            Cursor = new CombatCursorShape();
            FastCursorMode = false;

            CellPosition = new Vector2I();
        }

        public override void Refresh()
        {
            base.Refresh();

            Cursor.Position = Position;
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            Cursor.Center += new Vector2f(
                (((CellPosition.ToVector2() + new Vector2f(.5F, 0F)) * (int)CombatMap.CELL_SIZE).X - Cursor.Center.X) / TRANSITION_VELOCITY * (float)dt.MS,
                (((CellPosition.ToVector2() + new Vector2f(0F, .5F)) * (int)CombatMap.CELL_SIZE).Y - Cursor.Center.Y) / TRANSITION_VELOCITY * (float)dt.MS);

            if (!IsEnabled)
                return;

            FastCursorMode = Inputs.IsGameInput(InputType.Misc, null, false);
            double cursorMoveDelay = FastCursorMode ? FAST_CURSOR_MOVE_DELAY : CRUSOR_MOVE_DELAY;

            if (Inputs.IsGameInput(InputType.Left, null, false, cursorMoveDelay))
                Move(new Vector2I(-MOVE_CELL_OFFSET, 0));
            else if (Inputs.IsGameInput(InputType.Up, null, false, cursorMoveDelay))
                Move(new Vector2I(0, -MOVE_CELL_OFFSET));
            else if (Inputs.IsGameInput(InputType.Right, null, false, cursorMoveDelay))
                Move(new Vector2I(MOVE_CELL_OFFSET, 0));
            else if (Inputs.IsGameInput(InputType.Down, null, false, cursorMoveDelay))
                Move(new Vector2I(0, MOVE_CELL_OFFSET));
        }

        public override bool OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    if (Inputs.IsGameInput(InputType.Back, evt, false, Inputs.DEFAULT_INPUT_DELAY, false))
                    {
                        CallOnCancellation();
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Action, evt, true))
                        CallOnValidation();

                    if (Inputs.IsGameInput(InputType.Action2, evt))
                    {
                        if (Combat.State == Combat.EState.Placement)
                            Combat.ChangeState(Combat.EState.Starting);
                    }

                    if (Inputs.IsGameInput(InputType.Special0, evt, true))
                        Combat.Map.SwitchGridVisibility();

                    if (Inputs.IsGameInput(InputType.Special1, evt, true))
                        Combat.Map.SwitchObjectAlphaMode();

                    return true;
            }

            return base.OnEvent(evt);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (Combat == null)
                return;

            if (!Combat.Map.ContainsObjectToDraw(DrawOrder.Cursor, Cursor))
                Combat.Map.AddObjectToDraw(DrawOrder.Cursor, Cursor);
        }

        public override void Close(ClosingInfo closingInfo = null)
        {
            base.Close(closingInfo);

            if (Combat == null)
                return;

            Combat.Map.RemoveObjectToDraw(DrawOrder.Cursor, Cursor);
        }

        public override Vector2f Position
        {
            get { return (CellPosition * (int)CombatMap.CELL_SIZE).ToVector2(); }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Cursor == null)
                    return base.Dimension;

                return Cursor.Dimension;
            }
        }

        void Move(Vector2I offset)
        {
            Vector2I pos = CellPosition + offset;
            if (pos.X < 0 ||
                pos.Y < 0 ||
                pos.X > Combat.Map.Width - 1 ||
                pos.Y > Combat.Map.Height - 1)
                return;

            Vector2I oldCellPosition = CellPosition;

            CellPosition += offset;

            CallOnMove(oldCellPosition, CellPosition);
        }

        public void SetCellPosition(Vector2I cellPosition)
        {
            Move(cellPosition - CellPosition);
        }
    }
}