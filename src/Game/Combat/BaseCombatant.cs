using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    #region Enums

    /// <summary>
    /// Animation states of the combatant.
    /// </summary>
    public enum CombatantState
    {
        /// <summary>
        /// Combatant is inactive.
        /// </summary>
        Normal,
        /// <summary>
        /// Combatant is moving.
        /// </summary>
        Moving,
        /// <summary>
        /// Combatant is hurted.
        /// </summary>
        Hurted,
        /// <summary>
        /// Combatant is performing a physical action.
        /// </summary>
        Attacking,
        /// <summary>
        /// Combatant is performing a spell casting.
        /// </summary>
        Casting
    }

    #endregion

    #region Event handlers

    /// <summary>
    /// Combatant event arguments
    /// </summary>
    public class CombatantEventArgs : System.EventArgs
    {
    }

    /// <summary>
    /// Delegate for combatant event.
    /// </summary>
    /// <param name="sender">Source combatant of the event.</param>
    /// <param name="e">Arguments of the event.</param>
    public delegate void CombatantEventHandler(BaseCombatant sender, CombatantEventArgs e);

    /// <summary>
    /// Combatant event arguments about its combat.
    /// </summary>
    public class CombatantCombatEventArgs : CombatantEventArgs
    {
        /// <summary>
        /// Combat of the source combatant.
        /// </summary>
        public Combat Combat { get; private set; }

        /// <summary>
        /// Constructs combatant combat event arguments from the given combat.
        /// </summary>
        /// <param name="combat"></param>
        public CombatantCombatEventArgs(Combat combat) : base() { Combat = combat; }
    }

    /// <summary>
    /// Combatant event about its combat.
    /// </summary>
    /// <param name="sender">Source combatant of the event.</param>
    /// <param name="e">Arguements of the event.</param>
    public delegate void CombatantCombatEventHandler(BaseCombatant sender, CombatantCombatEventArgs e);

    /// <summary>
    /// Combatant move event arguments.
    /// </summary>
    public class CombatantMoveEventArgs : CombatantEventArgs
    {
        /// <summary>
        /// Position of the source combatant before its move.
        /// </summary>
        public Vector2I OldCellPosition { get; private set; }

        /// <summary>
        /// Current position of the source combatant (after its move).
        /// </summary>
        public Vector2I CellPosition { get; private set; }

        /// <summary>
        /// Constructs combatant move event arguments from a previous position and a current position.
        /// </summary>
        /// <param name="oldCellPosition">Position of the source combatant before moving.</param>
        /// <param name="cellPosition">Current position of the source combatant.</param>
        public CombatantMoveEventArgs(Vector2I oldCellPosition, Vector2I cellPosition) :
            base()
        {
            OldCellPosition = oldCellPosition;
            CellPosition = cellPosition;
        }
    }

    /// <summary>
    /// Combatant event based on its moves.
    /// </summary>
    /// <param name="sender">Combatant source of the event which moved.</param>
    /// <param name="e">Arguments of the event.</param>
    public delegate void CombatantMoveEventHandler(BaseCombatant sender, CombatantMoveEventArgs e);

    /// <summary>
    /// Combatant event arguments based on its status.
    /// </summary>
    public class CombatantStatusChangeEventArgs : CombatantEventArgs
    {
        /// <summary>
        /// Statistics of the source combatant that is modified.
        /// </summary>
        public BaseStatistic BaseStatistic { get; private set; }

        /// <summary>
        /// Constructs combatant status event arguments from a given statistics.
        /// </summary>
        /// <param name="baseStatistic"></param>
        public CombatantStatusChangeEventArgs(BaseStatistic baseStatistic) : base() { BaseStatistic = baseStatistic; }
    }

    /// <summary>
    /// Combatant event based on its status changes.
    /// </summary>
    /// <param name="sender">Source combatant of the event whose status changed.</param>
    /// <param name="e">Arguments of the event.</param>
    public delegate void CombatantStatusChangeEventHandler(BaseCombatant sender, CombatantStatusChangeEventArgs e);

    #endregion

    /// <summary>
    /// Base entity that constitutes combat progress.
    /// </summary>
    public abstract class BaseCombatant : BaseDrawable, IUpdateable, ICombatEntity
    {
        #region Classes

        #region MoveInfo

        /// <summary>
        /// Moves handler for combatant.
        /// </summary>
        public class CMoveInfo
        {
            #region Members

            BaseCombatant Combatant;
            Queue<Vector2I> Moves;
            Queue<Vector2I> MovePoints;

            Direction CurrentDirection;
            Vector2I CurrentGoal;
            Vector2f CurrentGoalPoint;
            bool PointIsReached;

            #endregion

            public CMoveInfo(BaseCombatant combatant)
            {
                Combatant = combatant;

                Moves = new Queue<Vector2I>();
                MovePoints = new Queue<Vector2I>();

                PointIsReached = true;
            }

            bool InitNextMove()
            {
                if (Moves.Count == 0)
                    return false;

                CurrentGoal = Combatant.CellPosition + Moves.Dequeue();

                PointIsReached = false;

                if (CurrentGoal == Combatant.CellPosition)
                {
                    PointIsReached = true;
                    return false;
                }

                Combatant.State = CombatantState.Moving;

                CurrentGoalPoint = (CurrentGoal.ToVector2() + new Vector2f(.5F, .5F)) * CombatMap.CELL_SIZE;

                if (CurrentGoal.X < Combatant.CellPosition.X)
                    CurrentDirection = Direction.O;
                else if (CurrentGoal.X > Combatant.CellPosition.X)
                    CurrentDirection = Direction.E;
                else if (CurrentGoal.Y < Combatant.CellPosition.Y)
                    CurrentDirection = Direction.N;
                else if (CurrentGoal.Y > Combatant.CellPosition.Y)
                    CurrentDirection = Direction.S;

                Combatant.Direction = CurrentDirection;

                return true;
            }

            bool InitNextPoint()
            {
                if (MovePoints.Count == 0)
                    return false;

                Moves.Enqueue(MovePoints.Dequeue() - Combatant.CellPosition);

                return true;
            }

            public void Update(Time dt)
            {
                if (PointIsReached)
                {
                    if (!InitNextMove() &&
                        !InitNextPoint())
                    {
                        if (Combatant.IsMoving())
                            Combatant.CallOnMoveEnding();

                        Combatant.State = CombatantState.Normal;
                        return;
                    }
                }

                Vector2f move = BlazeraLib.Map.GetMove(CurrentDirection, DEFAULT_VELOCITY, dt);

                switch (CurrentDirection)
                {
                    case Direction.N:

                        if (Combatant.GetBasePosition().Y + move.Y > CurrentGoalPoint.Y)
                        {
                            Combatant.Position += move;
                            return;
                        }

                        break;

                    case Direction.E:

                        if (Combatant.GetBasePosition().X + move.X < CurrentGoalPoint.X)
                        {
                            Combatant.Position += move;
                            return;
                        }

                        break;

                    case Direction.S:

                        if (Combatant.GetBasePosition().Y + move.Y < CurrentGoalPoint.Y)
                        {
                            Combatant.Position += move;
                            return;
                        }

                        break;

                    case Direction.O:

                        if (Combatant.GetBasePosition().X + move.X > CurrentGoalPoint.X)
                        {
                            Combatant.Position += move;
                            return;
                        }

                        break;
                }

                Combatant.SetBasePoint(CurrentGoalPoint);
                PointIsReached = true;
                Combatant.CellPosition = CurrentGoal;
            }

            public void AddMove(Vector2I move)
            {
                Moves.Enqueue(move);
            }

            public void AddMovePoint(Vector2I cellPosition)
            {
                MovePoints.Enqueue(cellPosition);
            }
        }

        #endregion

        #endregion

        #region Constants

        const CombatantState DEFAULT_STATE = CombatantState.Normal;

        const double BASE_POINT_HEIGHT_PERCENTAGE = 85D;

        //!\\ TEMP //!\\
        const float DEFAULT_VELOCITY = 140F;

        #endregion

        #region Members

        public string Name { get; set; }

        Dictionary<CombatantState, Dictionary<Direction, Animation>> Animations;

        CombatantState State;

        Direction Direction;

        Vector2I _cellPosition;
        public Vector2I CellPosition
        {
            get { return _cellPosition; }
            set
            {
                Vector2I oldCellPosition = CellPosition;

                _cellPosition = value;

                CallOnMove(oldCellPosition, CellPosition);
            }
        }

        CMoveInfo MoveInfo;

        public CombatMap Map { get; protected set; }

        public CombatantStatus Status { get; private set; }
        public SpellPanoply SpellPanoply { get; private set; }

        #endregion

        #region Events

        public event CombatantCombatEventHandler OnJoiningCombat;
        public event CombatantCombatEventHandler OnLeavingCombat;

        bool CallOnJoiningCombat(Combat combat) { if (OnJoiningCombat == null) return false; OnJoiningCombat(this, new CombatantCombatEventArgs(combat)); return true; }
        bool CallOnLeavingCombat(Combat combat) { if (OnLeavingCombat == null) return false; OnLeavingCombat(this, new CombatantCombatEventArgs(combat)); return true; }

        public event CombatantMoveEventHandler OnMove;
        bool CallOnMove(Vector2I oldCellPosition, Vector2I cellPosition) { if (OnMove == null) return false; OnMove(this, new CombatantMoveEventArgs(oldCellPosition, cellPosition)); return true; }

        public event CombatantMoveEventHandler OnMoveEnding;
        bool CallOnMoveEnding() { if (OnMoveEnding == null) return false; OnMoveEnding(this, new CombatantMoveEventArgs(null, CellPosition)); return true; }

        public event CombatantEventHandler OnDeath;

        bool CallOnDeath() { if (OnDeath == null) return false; OnDeath(this, new CombatantEventArgs()); return true; }

        public event CombatantStatusChangeEventHandler OnStatusChange;

        bool CallOnStatusChange(BaseStatistic baseStatistic) { if (OnStatusChange == null) return false; OnStatusChange(this, new CombatantStatusChangeEventArgs(baseStatistic)); return true; }

        #endregion

        public BaseCombatant(Personnage basePersonnage) :
            base()
        {
            State = DEFAULT_STATE;

            CellPosition = new Vector2I();

            Direction = GameDatas.DEFAULT_DIRECTION;

            Animations = new Dictionary<CombatantState, Dictionary<Direction, Animation>>();
            foreach (CombatantState combatantState in System.Enum.GetValues(typeof(CombatantState)))
                Animations.Add(combatantState, new Dictionary<Direction, Animation>());

            AddAnimation(CombatantState.Normal, Direction.N, new Animation(basePersonnage.GetAnimation(MovingState.Normal, Direction.N)));
            AddAnimation(CombatantState.Normal, Direction.E, new Animation(basePersonnage.GetAnimation(MovingState.Normal, Direction.E)));
            AddAnimation(CombatantState.Normal, Direction.S, new Animation(basePersonnage.GetAnimation(MovingState.Normal, Direction.S)));
            AddAnimation(CombatantState.Normal, Direction.O, new Animation(basePersonnage.GetAnimation(MovingState.Normal, Direction.O)));

            AddAnimation(CombatantState.Moving, Direction.N, new Animation(basePersonnage.GetAnimation(MovingState.Walking, Direction.N)));
            AddAnimation(CombatantState.Moving, Direction.E, new Animation(basePersonnage.GetAnimation(MovingState.Walking, Direction.E)));
            AddAnimation(CombatantState.Moving, Direction.S, new Animation(basePersonnage.GetAnimation(MovingState.Walking, Direction.S)));
            AddAnimation(CombatantState.Moving, Direction.O, new Animation(basePersonnage.GetAnimation(MovingState.Walking, Direction.O)));

            foreach (CombatantState state in Animations.Keys)
                foreach (Animation animation in Animations[state].Values)
                    animation.Play();

            MoveInfo = new CMoveInfo(this);

            Status = new CombatantStatus();
            Status.OnChange += new StatusChangeEventHandler(Status_OnChange);
            SpellPanoply = new SpellPanoply();
        }

        void Status_OnChange(Status sender, StatusChangeEventArgs e)
        {
            CallOnStatusChange(e.BaseStatistic);
        }

        public void Update(Time dt)
        {
            MoveInfo.Update(dt);

            GetCurrentAnimation().Update(dt);
        }

        public override void Draw(RenderWindow window)
        {
            GetCurrentAnimation().Draw(window);
        }

        public void AddAnimation(CombatantState state, Direction direction, Animation animation)
        {
            Animations[state].Add(direction, animation);
        }

        Animation GetCurrentAnimation()
        {
            return Animations[State][Direction];
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                foreach (CombatantState state in Animations.Keys)
                    foreach (Animation animation in Animations[state].Values)
                        animation.Position = Position;
            }
        }

        Vector2f GetBasePoint()
        {
            return new Vector2f(
                Halfsize.X,
                Dimension.Y * (float)(BASE_POINT_HEIGHT_PERCENTAGE / 100D));
        }

        Vector2f GetBasePosition()
        {
            return Position + GetBasePoint();
        }

        void SetBasePoint(Vector2f point)
        {
            Position = point - GetBasePoint();
        }

        public override Vector2f Dimension
        {
            get { return GetCurrentAnimation().Dimension; }
        }

        public void Move(Vector2I offset)
        {
            MoveInfo.AddMove(offset);
        }

        public void AddMovePoint(Vector2I cellPosition)
        {
            MoveInfo.AddMovePoint(cellPosition);
        }

        public bool IsMoving()
        {
            return State == CombatantState.Moving;
        }

        public void SetCellPositionTo(Vector2I cellPosition)
        {
            CellPosition = cellPosition;

            SetBasePoint((CellPosition.ToVector2() + new Vector2f(.5F, .5F)) * CombatMap.CELL_SIZE);
        }

        public void SetMap(CombatMap map)
        {
            Map = map;
        }

        //!\\ TODO //!\\
        public void TakeDamage()
        {
            int damages = RandomHelper.Get(0, 10);
            Status[BaseCaracteristic.Hp] -= (uint)damages;

            MapEffectManager.Instance.AddEffect(new MapTextEffect(damages.ToString(), Color.Black), new Vector2f(Center.X, Top));
        }
    }
}
