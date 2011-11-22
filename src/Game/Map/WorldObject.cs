using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public enum Direction
    {
        N = 0,
        NE = 1,
        E = 2,
        SE = 3,
        S = 4,
        SO = 5,
        O = 6,
        NO = 7
    }

    public enum DrawOrder
    {
        Under, // GroundElement
        MapCell, // CombatCell
        Grid, // Grid
        Cursor, // CombatCursor
        WorldItem, // WorldItem
        Normal, // Element, Personnage (sorted)
        Over // Effects
    }

    public class DirectionEventArgs : EventArgs
    {
        public Direction Direction { get; private set; }

        public DirectionEventArgs(Direction direction)
        {
            Direction = direction;
        }
    }

    public class MoveEventArgs : EventArgs
    {
        public Vector2f Move { get; private set; }

        public MoveEventArgs(Vector2f move)
        {
            Move = move;
        }
    }

    public delegate void MoveEventHandler(WorldObject sender, MoveEventArgs e);

    public delegate void DirectionEventHandler(WorldObject sender, DirectionEventArgs e);

    public abstract class WorldObject : DrawableBaseObject, IUpdateable
    {
        public DrawOrder DrawOrder { get; set; }
        public float DrawBottomMargin { get; set; }

        Boolean RunningIsCalled;

        public event DirectionEventHandler OnDirectionEnablement;
        public event DirectionEventHandler OnDirectionDisablement;
        public event MoveEventHandler OnMove;
        Boolean CallOnDirectionEnablement(Direction direction) { if (OnDirectionEnablement == null) return false; OnDirectionEnablement(this, new DirectionEventArgs(direction)); return true; }
        Boolean CallOnDirectionDisablement(Direction direction) { if (OnDirectionDisablement == null) return false; OnDirectionDisablement(this, new DirectionEventArgs(direction)); return true; }
        Boolean CallOnMove(Vector2f move) { if (OnMove == null) return false; OnMove(this, new MoveEventArgs(move)); return true; }

        public DirectionInfo DirectionInfo { get; private set; }

        public WorldObject() :
            base()
        {
            Direction = GameDatas.DEFAULT_DIRECTION;
            Velocity = GameDatas.DEFAULT_WALK_VELOCITY;

            DrawBottomMargin = 0F;

            DrawOrder = DrawOrder.Normal;

            RunningIsCalled = false;

            BBoundingBoxes = new List<BBoundingBox>();
            BodyBoundingBoxes = new List<EBoundingBox>();
            EventBoundingBoxes = new Dictionary<EventBoundingBoxType, List<EBoundingBox>>();
            foreach (EventBoundingBoxType BBT in Enum.GetValues(typeof(EventBoundingBoxType)))
                this.EventBoundingBoxes[BBT] = new List<EBoundingBox>();

            Position = new Vector2f();
            Dimension = new Vector2f();

            DirectionInfo = new DirectionInfo(this);

            Map = null;

            DirectionStates = new Dictionary<Direction, Boolean>()
            {
                { Direction.N, false },
                { Direction.S, false },
                { Direction.E, false },
                { Direction.O, false }
            };

            MoveInfo = new MoveInfo(this);
            UpdateStates();

            IsVisible = true;
        }

        public WorldObject(WorldObject copy) :
            base(copy)
        {
            Direction = GameDatas.DEFAULT_DIRECTION;
            Velocity = GameDatas.DEFAULT_WALK_VELOCITY;

            DrawBottomMargin = copy.DrawBottomMargin;

            DrawOrder = DrawOrder.Normal;

            RunningIsCalled = false;

            DirectionInfo = new DirectionInfo(this);

            BBoundingBoxes = new List<BBoundingBox>();
            foreach (BBoundingBox BB in copy.BBoundingBoxes)
            {
                BBoundingBoxes.Add(new BBoundingBox(BB, this));
            }

            BodyBoundingBoxes = new List<EBoundingBox>();
            foreach (EBoundingBox BB in copy.BodyBoundingBoxes)
                AddBodyBoundingBox(new EBoundingBox(BB, this));

            EventBoundingBoxes = new Dictionary<EventBoundingBoxType, List<EBoundingBox>>();
            EventBoundingBoxes.Add(EventBoundingBoxType.Internal, new List<EBoundingBox>());
            EventBoundingBoxes.Add(EventBoundingBoxType.External, new List<EBoundingBox>());
            foreach (EBoundingBox BB in copy.EventBoundingBoxes[EventBoundingBoxType.External])
                AddEventBoundingBox(new EBoundingBox(BB, this), EventBoundingBoxType.External);

            if (copy.Skin != null)
            {
                Skin = new Texture(copy.Skin);
            }

            Position = new Vector2f(copy.Position.X,
                                   copy.Position.Y);
            Dimension = new Vector2f(copy.Dimension.X,
                                    copy.Dimension.Y);
            Map = copy.Map;

            DirectionStates = new Dictionary<Direction, Boolean>()
            {
                { Direction.N, false },
                { Direction.S, false },
                { Direction.E, false },
                { Direction.O, false }
            };

            MoveInfo = new MoveInfo(this);
            UpdateStates();

            IsVisible = true;
        }

        public override void ToScript()
        {
            base.ToScript();

            if (Skin != null)
            {
                Sw.WriteProperty("Skin", "Create:Texture(" + ScriptWriter.GetStringOf(Skin.Type) + ")");
            }

            foreach (BBoundingBox BB in BBoundingBoxes)
            {
                Sw.WriteMethod("AddBoundingBox",
                                    new String[]
                                        {
                                            "BBoundingBox(" + Sw.Name + ", " +
                                                              BB.BaseLeft.ToString() + ", " +
                                                              BB.BaseTop.ToString() + ", " +
                                                              BB.BaseRight.ToString() + ", " +
                                                              BB.BaseBottom.ToString() + ")"
                                        });
            }

            foreach (EBoundingBox BB in BodyBoundingBoxes)
            {
                Sw.WriteMethod("AddBodyBoundingBox",
                                    new String[]
                                        {
                                            "EBoundingBox(" + Sw.Name + ", " +
                                                              "EBoundingBoxType." + BB.Type.ToString() + ", " +
                                                              BB.BaseLeft.ToString() + ", " +
                                                              BB.BaseTop.ToString() + ", " +
                                                              BB.BaseRight.ToString() + ", " +
                                                              BB.BaseBottom.ToString() + ")"
                                        });
            }

            foreach (EBoundingBox BB in EventBoundingBoxes[EventBoundingBoxType.Internal])
            {
                Sw.WriteMethod("AddEventBoundingBox",
                                    new String[]
                                        {
                                            "EBoundingBox(" + Sw.Name + ", " +
                                                              "EBoundingBoxType." + BB.Type.ToString() + ", " +
                                                              BB.BaseLeft.ToString() + ", " +
                                                              BB.BaseTop.ToString() + ", " +
                                                              BB.BaseRight.ToString() + ", " +
                                                              BB.BaseBottom.ToString() + ")", "EventBoundingBoxType.Internal"
                                        });
            }

        }

        public virtual void Update(Time dt)
        {
            this.MoveInfo.Update();

            this.UpdateStates();
        }

        public override void Draw(RenderWindow window)
        {
            if (!this.IsVisible)
                return;

            if (this.Skin != null)
            {
                this.Skin.Draw(window);
            }
        }

        public void Move(float x, float y)
        {
            this.Move(new Vector2f(x, y));
        }

        public void MoveTo(float x, float y)
        {
            this.MoveTo(new Vector2f(x, y));
        }

        public void Move(Vector2f move)
        {
            this.MoveTo(this.Position + move);
        }
        
        public void MoveTo(Vector2f point)
        {
            if (Map == null)
                return;

            Vector2f offset = point - Position;

            Map.Ground.RemoveObjectBoundingBoxes(this);

            Position = point;

            foreach (BBoundingBox BB in BBoundingBoxes)
                BB.Update();

            foreach (EBoundingBox BB in BodyBoundingBoxes)
                BB.Update();

            foreach (EventBoundingBoxType BBT in Enum.GetValues(typeof(EventBoundingBoxType)))
                foreach (EBoundingBox BB in EventBoundingBoxes[BBT])
                    BB.Update();

            Map.Ground.AddObjectBoundingBoxes(this);

            Map.UpdateObjectEvents(this);

            CallOnMove(offset);
        }

        void RefreshMapBoundingBoxes()
        {
            if (Map == null)
                return;

            Map.Ground.RemoveObjectBoundingBoxes(this);
            Map.Ground.AddObjectBoundingBoxes(this);
        }

        public void AddBoundingBox(BBoundingBox BB)
        {
            BBoundingBoxes.Add(BB);
            RefreshMapBoundingBoxes();
        }

        public void AddBodyBoundingBox(EBoundingBox BB)
        {
            BodyBoundingBoxes.Add(BB);
            RefreshMapBoundingBoxes();
        }

        public void AddEventBoundingBox(EBoundingBox BB, EventBoundingBoxType type)
        {
            EventBoundingBoxes[type].Add(BB);
            RefreshMapBoundingBoxes();
        }

        public Boolean RemoveBoundingBox(BBoundingBox BB)
        {
            Map.Ground.RemoveObjectBoundingBoxes(this);

            if (!BBoundingBoxes.Remove(BB))
                return false;

            Map.Ground.AddObjectBoundingBoxes(this);

            return true;
        }

        public Boolean RemoveBodyBoundingBox(EBoundingBox BB)
        {
            Map.Ground.RemoveObjectBoundingBoxes(this);

            if (!BodyBoundingBoxes.Remove(BB))
                return false;

            Map.Ground.AddObjectBoundingBoxes(this);

            return true;
        }

        public Boolean RemoveEventBoundingBox(EBoundingBox BB, EventBoundingBoxType type)
        {
            Map.Ground.RemoveObjectBoundingBoxes(this);

            if (!EventBoundingBoxes[type].Remove(BB))
                return false;

            Map.Ground.AddObjectBoundingBoxes(this);

            return true;
        }

        public void SetMap(Map map, float x, float y)
        {
            if (Map == map)
            {
                MoveTo(x, y);

                return;
            }

            if (this.Map != null)
            {
                // Remove this from the old map
                this.Map.RemoveObject(this);
            }
            // Set the new map
            this.Map = map;
            // Add this to the new map
            this.Map.AddObject(this);
            // Set this position to (x, y)
            this.MoveTo(x, y);
        }

        public virtual void SetMap(Map map, String warpPointName)
        {
            WarpPoint warpPoint = map.GetWarpPoint(warpPointName);

            if (warpPoint == null)
                warpPoint = map.DefaultWarpPoint;

            Vector2f position = warpPoint.Point;

            SetMap(map, position.X, position.Y);

            Direction = warpPoint.Direction;
        }

        public virtual void SetMap(String mapType, String warpPointName)
        {
            SetMap(Create.Map(mapType), warpPointName);
        }

        public List<BBoundingBox> BBoundingBoxes { get; private set; }
        public List<EBoundingBox> BodyBoundingBoxes { get; private set; }
        public Dictionary<EventBoundingBoxType, List<EBoundingBox>> EventBoundingBoxes { get; private set; }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                if (Skin != null)
                    Skin.Position = Position;
            }
        }

        public Int32 Z { get; set; }

        public Vector2I TPosition
        {
            get
            {
                return Vector2I.FromVector2(this.Position) / GameDatas.TILE_SIZE;
            }
            protected set
            {
                this.Position = value.ToVector2() * GameDatas.TILE_SIZE;
            }
        }

        public int TLeft
        {
            get
            {
                return (int)this.Left / GameDatas.TILE_SIZE;
            }
            set
            {
                this.Left = value * GameDatas.TILE_SIZE;
            }
        }

        public int TTop
        {
            get
            {
                return (int)this.Top / GameDatas.TILE_SIZE;
            }
            set
            {
                this.Top = value * GameDatas.TILE_SIZE;
            }
        }

        public int TRight
        {
            get
            {
                return (int)this.Right / GameDatas.TILE_SIZE;
            }
            set
            {
                this.Right = value * GameDatas.TILE_SIZE;
            }
        }

        public int TBottom
        {
            get
            {
                return (int)this.Bottom / GameDatas.TILE_SIZE;
            }
            set
            {
                this.Bottom = value * GameDatas.TILE_SIZE;
            }
        }

        public Map Map
        {
            get;
            private set;
        }

        public Boolean IsMoving()
        {
            return MovingState == MovingState.Walking
                || MovingState == MovingState.Running;
        }

        private Texture _skin;
        public virtual Texture Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                _skin = value;

                if (Skin != null)
                    Dimension = Skin.Dimension;
            }
        }

        public Direction Direction
        {
            get;
            set;
        }

        public virtual float Velocity
        {
            get;
            set;
        }

        public Dictionary<Direction, Boolean> DirectionStates
        {
            get;
            protected set;
        }

        public void ResetDirectionStates(Dictionary<Direction, bool> directionStates)
        {
            DirectionStates = directionStates;
        }

        public State State { get; private set; }
        public MovingState MovingState { get; private set; }

        public Boolean TrySetState(State state)
        {
            if (state != State.Moving)
            {
                if (State == State.Active)
                    return state == State.Active;

                if (State == State.Moving)
                    StopMove();

                State = state;

                return true;
            }

            if (State == State.Active)
                return false;

            State = state;

            return true;
        }

        protected Boolean TrySetMovingStateFromState(State state)
        {
            if (!TrySetState(state))
                return false;

            if (State != State.Moving)
            {
                MovingState = MovingState.Normal;
                return false;
            }

            if (MovingState == MovingState.Normal)
                MovingState = RunningIsCalled ? MovingState.Running : MovingState.Walking;

            return true;
        }

        /// <summary>
        /// Forces the WorldObject.State property to be State.Inactive
        /// </summary>
        public void Stop()
        {
            State = State.Inactive;
        }

        public void StopMove()
        {
            State = State.Inactive;
            MovingState = MovingState.Normal;

            foreach (Direction direction in Enum.GetValues(typeof(Direction)))
                DirectionStates[direction] = false;
        }

        public void SetRunning(Boolean running = true)
        {
            RunningIsCalled = running;

            if (MovingState == MovingState.Normal)
                return;

            if (running)
                MovingState = MovingState.Running;
            else
                MovingState = MovingState.Walking;
        }

        protected virtual void UpdateStates()
        {
            if (this.DirectionStates[Direction.N] || this.DirectionStates[Direction.S] || this.DirectionStates[Direction.E] || this.DirectionStates[Direction.O])
            {
                if (!TrySetState(State.Moving))
                    return;

                if (this.DirectionStates[Direction.N] && this.DirectionStates[Direction.E])
                    this.Direction = Direction.NE;
                else if (this.DirectionStates[Direction.N] && this.DirectionStates[Direction.O])
                    this.Direction = Direction.NO;
                else if (this.DirectionStates[Direction.S] && this.DirectionStates[Direction.E])
                    this.Direction = Direction.SE;
                else if (this.DirectionStates[Direction.S] && this.DirectionStates[Direction.O])
                    this.Direction = Direction.SO;
                else if (this.DirectionStates[Direction.N])
                    this.Direction = Direction.N;
                else if (this.DirectionStates[Direction.S])
                    this.Direction = Direction.S;
                else if (this.DirectionStates[Direction.E])
                    this.Direction = Direction.E;
                else if (this.DirectionStates[Direction.O])
                    this.Direction = Direction.O;
            }
            else
            {
                TrySetState(State.Inactive);
            }
        }

        public void EnableDirection(Direction dir)
        {
            this.DirectionStates[dir] = true;

            CallOnDirectionEnablement(dir);
        }

        public void DisableDirection(Direction dir)
        {
            this.DirectionStates[dir] = false;

            CallOnDirectionDisablement(dir);
        }

        public MoveInfo MoveInfo { get; set; }
    }

    #region MovesInfo

    public class MoveInfo
    {
        private const Int32 XDIR = 0;
        private const Int32 YDIR = 1;

        public MoveInfo(WorldObject holder)
        {
            this.Holder = holder;
            this.Directions = new Direction[2];
            this.GoalX = true;
            this.GoalY = true;
            this.Path = new Queue<Vector2f>();
        }

        private void Init(Vector2f goal)
        {
            this.Goal = goal;

            if (this.Holder.Position.X == this.Goal.X)
            {
                this.GoalX = true;
            }
            else
            {
                this.GoalX = false;
            }

            if (this.Holder.Position.Y == this.Goal.Y)
            {
                this.GoalY = true;
            }
            else
            {
                this.GoalY = false;
            }

            if (!this.IsRunning)
            {
                return;
            }

            if (this.Holder.Position.X < this.Goal.X)
            {
                this.Directions[XDIR] = Direction.E;
                this.Holder.EnableDirection(Direction.E);
            }
            else
            {
                this.Directions[XDIR] = Direction.O;
                this.Holder.EnableDirection(Direction.O);
            }

            if (this.Holder.Position.Y < this.Goal.Y)
            {
                this.Directions[YDIR] = Direction.S;
                this.Holder.EnableDirection(Direction.S);
            }
            else
            {
                this.Directions[YDIR] = Direction.N;
                this.Holder.EnableDirection(Direction.N);
            }
        }

        public void Update()
        {
            if (!this.IsRunning)
            {
                if (this.Path.Count > 0)
                {
                    this.Init(this.Path.Dequeue());
                }
                else
                {
                    return;
                }
            }

            this.UpdateDirection();
        }

        private void UpdateDirection()
        {
            if (this.Directions[XDIR] == Direction.E)
            {
                if (this.Holder.Position.X >= this.Goal.X)
                {
                    this.GoalX = true;
                    this.Holder.DisableDirection(Direction.E);
                    this.Holder.MoveTo(new Vector2f(this.Goal.X, this.Holder.Position.Y)); //illegal
                }
            }
            else
            {
                if (this.Holder.Position.X <= this.Goal.X)
                {
                    this.GoalX = true;
                    this.Holder.DisableDirection(Direction.O);
                    this.Holder.MoveTo(new Vector2f(this.Goal.X, this.Holder.Position.Y));
                }
            }

            if (this.Directions[YDIR] == Direction.S)
            {
                if (this.Holder.Position.Y >= this.Goal.Y)
                {
                    this.GoalY = true;
                    this.Holder.DisableDirection(Direction.S);
                    this.Holder.MoveTo(new Vector2f(this.Holder.Position.X, this.Goal.Y));
                }
            }
            else
            {
                if (this.Holder.Position.Y <= this.Goal.Y)
                {
                    this.GoalY = true;
                    this.Holder.DisableDirection(Direction.N);
                    this.Holder.MoveTo(new Vector2f(this.Holder.Position.X, this.Goal.Y));
                }
            }
        }

        public void AddPoint(Vector2f point)
        {
            this.Path.Enqueue(point);
        }

        public void AddPath(List<Vector2f> path)
        {
            foreach (Vector2f point in path)
            {
                this.AddPoint(point);
            }
        }

        public void AddPoint(Vector2I point)
        {
            this.Path.Enqueue(new Vector2f(point.X * GameDatas.TILE_SIZE, point.Y * GameDatas.TILE_SIZE));
        }

        public void AddPath(List<Vector2I> path)
        {
            foreach (Vector2I point in path)
            {
                this.AddPoint(point);
            }
        }

        private Boolean IsRunning
        {
            get
            {
                return !this.GoalX ||
                       !this.GoalY;
            }
        }

        private WorldObject Holder
        {
            get;
            set;
        }
        
        private Vector2f Goal
        {
            get;
            set;
        }

        private Boolean GoalX
        {
            get;
            set;
        }

        private Boolean GoalY
        {
            get;
            set;
        }

        private Direction[] Directions
        {
            get;
            set;
        }

        private Queue<Vector2f> Path
        {
            get;
            set;
        }
    }

    #endregion

    #region DirectionInfo

    public class DirectionInfo
    {
        const float PERCENT_CENTER_ZONE = 15F;

        WorldObject Holder;

        public DirectionInfo(WorldObject holder)
        {
            Holder = holder;
        }

        public Boolean IsFacing(WorldObject wObj)
        {
            // left
            if (wObj.Right <= Holder.Center.X - Holder.Dimension.X * PERCENT_CENTER_ZONE / 100F)
            {
                return Holder.Direction == Direction.NO
                    || Holder.Direction == Direction.SO
                    || Holder.Direction == Direction.O;
            }
            // right
            else if (wObj.Left > Holder.Center.X + Holder.Dimension.X * PERCENT_CENTER_ZONE / 100F)
            {
                return Holder.Direction == Direction.NE
                    || Holder.Direction == Direction.SE
                    || Holder.Direction == Direction.E;
            }
            // center
            else
            {
                if (wObj.Center.Y <= Holder.Center.Y)
                    return Holder.Direction == Direction.N;
                else
                    return Holder.Direction == Direction.S;
            }
        }

        public void SetFacing(WorldObject wObj)
        {
            // left
            if (wObj.Right <= Holder.Center.X - Holder.Dimension.X * PERCENT_CENTER_ZONE / 100F)
            {
                if (wObj.Bottom <= Holder.Center.Y - Holder.Dimension.Y * PERCENT_CENTER_ZONE / 100F)
                    Holder.Direction = Direction.NO;
                else if (wObj.Top > Holder.Center.Y + Holder.Dimension.Y * PERCENT_CENTER_ZONE / 100F)
                    Holder.Direction = Direction.SO;
                else
                    Holder.Direction = Direction.O;
            }
            // right
            else if (wObj.Left > Holder.Center.X + Holder.Dimension.X * PERCENT_CENTER_ZONE / 100F)
            {
                if (wObj.Bottom <= Holder.Center.Y - Holder.Dimension.Y * PERCENT_CENTER_ZONE / 100F)
                    Holder.Direction = Direction.NE;
                else if (wObj.Top > Holder.Center.Y + Holder.Dimension.Y * PERCENT_CENTER_ZONE / 100F)
                    Holder.Direction = Direction.SE;
                else
                    Holder.Direction = Direction.E;
            }
            // center
            else
            {
                if (wObj.Center.Y <= Holder.Center.Y)
                    Holder.Direction = Direction.N;
                else
                    Holder.Direction = Direction.S;
            }
        }
    }

    #endregion
}