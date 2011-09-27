using System.Collections.Generic;
using SFML.Graphics;

namespace BlazeraLib
{
    #region Event handlers

    public class CombatEventArgs : System.EventArgs
    {
    }
    public delegate void CombatEventHandler(Combat sender, CombatEventArgs e);

    public class CombatCombatantEventArgs : CombatEventArgs
    {
        public BaseCombatant Combatant { get; private set; }
        public CombatCombatantEventArgs(BaseCombatant combatant) : base() { Combatant = combatant; }
    }
    public delegate void CombatCombatantEventHandler(Combat sender, CombatCombatantEventArgs e);

    #endregion

    /// <summary>
    /// A tactical turn based combat system.
    /// </summary>
    public class Combat : IUpdateable
    {
        #region Enums

        public enum EState
        {
            Placement,
            Starting,
            TurnStart,
            MoveCellSelection,
            ActionSelection,
            AttackCellSelection,
            SpellCellSelection,
            ItemCellSelection,
            Fury,
            ExploreCellSelection,
            TurnOver,
            Over
        }

        #endregion

        #region Constants

        #endregion

        #region Members

        public EState State { get; private set; }

        Dictionary<EState, Phase> Phases;

        List<Team> Teams;
        List<BaseCombatant> Combatants;
        CombatantQueue CombatantOrder;

        BaseCombatant CurrentCombatant;

        CombatCellSet CellSet;

        #region Widets

        public CombatMainMenu MainMenu { get; private set; }
        public CombatActionMenu ActionMenu { get; private set; }
        public SpellMenu SpellMenu { get; private set; }

        public CombatCursor Cursor { get; private set; }

        public CombatInfoPanel InfoPanel { get; private set; }

        #endregion

        public CombatMap Map { get; private set; }

        #endregion

        #region Events

        public event CombatEventHandler OnStateChange;
        bool CallOnStateChange() { if (OnStateChange == null) return false; OnStateChange(this, new CombatEventArgs()); return true; }

        public event CombatCombatantEventHandler OnCombatantJoining;
        public event CombatCombatantEventHandler OnCombatantLeaving;

        bool CallOnCombatantJoining(BaseCombatant combatant) { if (OnCombatantJoining == null) return false; OnCombatantJoining(this, new CombatCombatantEventArgs(combatant)); return true; }
        bool CallOnCombatantLeaving(BaseCombatant combatant) { if (OnCombatantLeaving == null) return false; OnCombatantLeaving(this, new CombatCombatantEventArgs(combatant)); return true; }

        public event CombatCombatantEventHandler OnCombatantStartTurning;
        public event CombatCombatantEventHandler OnCombatantStopTurning;

        bool CallOnCombatantStartTurning() { if (OnCombatantStartTurning == null) return false; OnCombatantStartTurning(this, new CombatCombatantEventArgs(CurrentCombatant)); return true; }
        bool CallOnCombatantStopTurning() { if (OnCombatantStopTurning == null) return false; OnCombatantStopTurning(this, new CombatCombatantEventArgs(CurrentCombatant)); return true; }

        #endregion

        public Combat(CombatMap map) :
            base()
        {
            Map = map;

            Phases = new Dictionary<EState, Phase>()
            {
                { EState.Placement,             new Placement(this) },
                { EState.Starting,              new Starting(this) },
                { EState.TurnStart,             new TurnStart(this) },
                { EState.ActionSelection,       new ActionSelection(this) },
                { EState.MoveCellSelection,     new MoveCellSelection(this) },
                { EState.ExploreCellSelection,  new ExploreCellSelection(this) },
                { EState.AttackCellSelection,   new AttackCellSelection(this) },
                { EState.SpellCellSelection,    new SpellCellSelection(this) },
                { EState.ItemCellSelection,     new ItemCellSelection(this) },
                { EState.Fury,                  new Fury(this) },
                { EState.TurnOver,              new TurnOver(this) },
                { EState.Over,                  new Over(this) }
            };

            Teams = new List<Team>();
            Combatants = new List<BaseCombatant>();

            CellSet = new CombatCellSet(Map);

            ActionMenu = new CombatActionMenu(this);
            MainMenu = new CombatMainMenu(this);
            SpellMenu = new SpellMenu(this);
            ActionMenu.AttachSpellMenu(SpellMenu);

            Cursor = new CombatCursor(this);

            InfoPanel = new CombatInfoPanel(this);
            InfoPanel.AddBox(new CombatantInfoPanelBox());

            CombatantOrder = new CombatantQueue();

            ChangeState(EState.Placement, null, true);
        }

        public void Start()
        {
            CombatantOrder.AddCombatants(Combatants);

            ChangeTurn();
        }
        
        public void Update(Time dt)
        {
            foreach (BaseCombatant combatant in Combatants)
                combatant.Update(dt);

            GetCurrentPhase().Perform();
        }

        public void AddTeam(Team team)
        {
            Teams.Add(team);

            IEnumerator<BaseCombatant> combatants = team.GetCombatantEnumerator();
            while (combatants.MoveNext())
                AddCombatant(combatants.Current);
        }

        public bool RemoveTeam(Team team)
        {
            if (!Teams.Remove(team))
                return false;

            IEnumerator<BaseCombatant> combatants = team.GetCombatantEnumerator();
            while (combatants.MoveNext())
                RemoveCombatant(combatants.Current);

            return true;
        }

        void AddCombatant(BaseCombatant combatant)
        {
            CallOnCombatantJoining(combatant);
            Combatants.Add(combatant);

            combatant.OnMove += new CombatantMoveEventHandler(combatant_OnMove);
        }

        void combatant_OnMove(BaseCombatant sender, CombatantMoveEventArgs e)
        {
            CellSet.GetCell(e.OldCellPosition).RemoveCombatant(sender);
            CellSet.GetCell(e.CellPosition).AddCombatant(sender);
        }

        bool RemoveCombatant(BaseCombatant combatant)
        {
            CallOnCombatantLeaving(combatant);
            return Combatants.Remove(combatant);

            combatant.OnMove -= new CombatantMoveEventHandler(combatant_OnMove);
        }

        Phase GetCurrentPhase()
        {
            return Phases[State];
        }

        public void ChangeState(EState state, Phase.StartInfo startInfo = null, bool init = false)
        {
            if (!init)
                GetCurrentPhase().End();

            State = state;

            Log.Cl("Combat state : " + State.ToString(), System.ConsoleColor.Yellow);

            CallOnStateChange();

            GetCurrentPhase().Start(startInfo);
        }

        public void ChangeTurn()
        {
            if (CurrentCombatant != null)
                CallOnCombatantStopTurning();

            CurrentCombatant = CombatantOrder.GetNextAliveCombat();

            CallOnCombatantStartTurning();
        }

        public void AddCellColorEffect(CellArea area, Vector2I centerCellPosition, CellSelectionType type)
        {
            foreach (Vector2I cellPosition in area.GetAreaCellPositions(centerCellPosition))
            {
                if (CellSet.GetCell(cellPosition) == null)
                    continue;

                CellShape colorEffect = GetCell(cellPosition).AddColorEffect(type);
                if (colorEffect != null)
                    Map.AddObjectToDraw(DrawOrder.MapCell, colorEffect);
            }
        }

        public void RemoveCellColorEffect(CellArea area, Vector2I centerCellPosition, CellSelectionType type)
        {
            foreach (Vector2I cellPosition in area.GetAreaCellPositions(centerCellPosition))
            {
                if (CellSet.GetCell(cellPosition) == null ||
                    CellSet.GetCell(cellPosition).GetColorEffect(type) == null)
                    continue;

                Map.RemoveObjectToDraw(DrawOrder.MapCell, GetCell(cellPosition).GetColorEffect(type));
            }
        }

        public void ClearCellColorEffect(CellSelectionType type)
        {
            for (int y = 0; y < Map.Height; ++y)
                for (int x = 0; x < Map.Width; ++x)
                    RemoveCellColorEffect(new CellArea(), new Vector2I(x, y), type);
        }

        public void ClearCellColorEffect()
        {
            foreach (CellSelectionType type in System.Enum.GetValues(typeof(CellSelectionType)))
                ClearCellColorEffect(type);
        }

        public CombatCell GetCell(int x, int y)
        {
            return CellSet.GetCell(x, y);
        }

        public CombatCell GetCell(Vector2I cellPosition)
        {
            return CellSet.GetCell(cellPosition);
        }

        public void ActivateMainMenu()
        {
            MainMenu.SetFirst();
            MainMenu.Reset();
            Cursor.Open();
            Cursor.Disable();
        }

        public void DesactivateMainMenu()
        {
            SpellMenu.Close();
            ActionMenu.Close();
            MainMenu.Close();
            Cursor.SetFirst();
        }

        public List<CombatCell> GetCellFromArea(CellArea area, Vector2I centerCellPosition)
        {
            List<CombatCell> cells = new List<CombatCell>();

            foreach (Vector2I cellPosition in area.GetAreaCellPositions(centerCellPosition))
                if (GetCell(cellPosition) != null)
                    cells.Add(GetCell(cellPosition));

            return cells;
        }
    }
}
