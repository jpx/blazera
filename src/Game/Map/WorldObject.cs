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

    public delegate void DirectionEventHandler(WorldObject sender, DirectionEventArgs e);

    public abstract class WorldObject : DrawableBaseObject, IUpdateable, IVisitable<WorldObject>
    {
        #region Classes

        protected class CMovingStateInfo : IUpdateable
        {
            public delegate void DirectionEventHandler(CMovingStateInfo sender, DirectionEventArgs e);

            public event DirectionEventHandler OnDirectionChange;

            bool CallOnDirectionChange() { if (OnDirectionChange == null) return false; OnDirectionChange(this, new DirectionEventArgs(Direction)); return true; }

            #region Members

            protected WorldObject Parent { get; private set; }

            float BaseVelocity;

            Dictionary<Direction, bool> DirectionStates;

            public Direction Direction { get; private set; }

            #endregion Members

            public CMovingStateInfo(WorldObject parent)
            {
                Parent = parent;

                BaseVelocity = GameData.DEFAULT_WALK_VELOCITY;
                Direction = GameData.DEFAULT_DIRECTION;

                DirectionStates = new Dictionary<Direction, bool>()
                {
                    { Direction.N, false },
                    { Direction.E, false },
                    { Direction.S, false },
                    { Direction.O, false }
                };
                ResetDirectionStates();
            }

            public virtual void Update(Time dt) { }

            protected bool IsEnabled(Direction direction)
            {
                return DirectionStates[direction];
            }

            public void Enable(Direction direction, bool isEnabled = true)
            {
                DirectionStates[direction] = isEnabled;

                RefreshDirection();
            }

            public void ResetDirectionStates(bool isEnabled = false)
            {
                Enable(Direction.N, isEnabled);
                Enable(Direction.E, isEnabled);
                Enable(Direction.S, isEnabled);
                Enable(Direction.O, isEnabled);
            }

            public virtual float GetVelocity()
            {
                return BaseVelocity;
            }

            protected virtual void RefreshDirection()
            {
                if (!IsMoving() || (Parent.IsActive() && !Parent.IsMoving()))
                {
                    if (Parent.IsMoving())
                        Parent.TrySetState("Inactive");
                    return;
                }

                if (IsEnabled(Direction.N) && IsEnabled(Direction.E))
                    SetDirection(Direction.NE);
                else if (IsEnabled(Direction.N) && IsEnabled(Direction.O))
                    SetDirection(Direction.NO);
                else if (IsEnabled(Direction.S) && IsEnabled(Direction.E))
                    SetDirection(Direction.SE);
                else if (IsEnabled(Direction.S) && IsEnabled(Direction.O))
                    SetDirection(Direction.SO);
                else if (IsEnabled(Direction.N))
                    SetDirection(Direction.N);
                else if (IsEnabled(Direction.S))
                    SetDirection(Direction.S);
                else if (IsEnabled(Direction.E))
                    SetDirection(Direction.E);
                else if (IsEnabled(Direction.O))
                    SetDirection(Direction.O);

                Parent.TrySetState("Moving");
            }

            public void SetDirection(Direction direction)
            {
                Direction = direction;

                CallOnDirectionChange();
            }

            public void SetBaseVelocity(float baseVelocity)
            {
                BaseVelocity = baseVelocity;
            }

            /// <summary>
            /// Specifies if the parent object is trying to move.
            /// </summary>
            /// <returns>True if the parent object is trying to move.</returns>
            public bool IsMoving()
            {
                return
                    IsEnabled(Direction.N) ||
                    IsEnabled(Direction.S) ||
                    IsEnabled(Direction.E) ||
                    IsEnabled(Direction.O);
            }
        }

        #endregion Classes

        //!\\ TODO: move from here
        public static Dictionary<string, string[]> JoinableStateTable = new Dictionary<string, string[]>()
        {
            // personnage
            { "Moving", new string[] { "Running", "Rotating" } },
            { "Running", new string[] { "Moving", "Rotating" } },
            { "Rotating", new string[] { "Moving", "Running" } },

            // door
            { "Open", new string[] { "Closed", "Closing" } },
            { "Closed", new string[] { "Open", "Opening", "Locked" } },
            { "Locked", new string[] { "Closed" } },
            { "Opening", new string[] { "Open" } },
            { "Closing", new string[] { "Closed" } }
        };

        public class UpdateEventArgs : EventArgs { }
        public delegate void UpdateEventHandler(WorldObject sender, UpdateEventArgs e);
        public class StateEventArgs : EventArgs
        {
            public State<string> State { get; private set; }
            public StateEventArgs(State<string> state) { State = state; }
        }
        public delegate void StateEventHandler(WorldObject sender, StateEventArgs e);

        public class MapChangeEventArgs : EventArgs
        {
            public Map Map { get; private set; }
            public MapChangeEventArgs(Map map) { Map = map; }
        }
        public delegate void MapChangeEventHandler(WorldObject sender, MapChangeEventArgs e);

        public DrawOrder DrawOrder { get; set; }

        //!\\ TMP //!\\
        public float DrawBottomMargin { get; set; }

        #region Events

        public event DirectionEventHandler OnDirectionEnablement;
        public event DirectionEventHandler OnDirectionDisablement;
        Boolean CallOnDirectionEnablement(Direction direction) { if (OnDirectionEnablement == null) return false; OnDirectionEnablement(this, new DirectionEventArgs(direction)); return true; }
        Boolean CallOnDirectionDisablement(Direction direction) { if (OnDirectionDisablement == null) return false; OnDirectionDisablement(this, new DirectionEventArgs(direction)); return true; }

        public event UpdateEventHandler OnUpdate;
        bool CallOnUpdate() { if (OnUpdate == null) return false; OnUpdate(this, new UpdateEventArgs()); return true; }

        public event StateEventHandler OnStateChange;
        bool CallOnStateChange() { if (OnStateChange == null) return false; OnStateChange(this, new StateEventArgs(State)); return true; }

        public event MapChangeEventHandler OnMapChange;
        bool CallOnMapChange() { if (OnMapChange == null) return false; OnMapChange(this, new MapChangeEventArgs(Map)); return true; }

        #endregion Events

        static readonly string DEFAULT_STATE = SkinSet.DEFAULT_DEFAULT_STATE;

        #region Members

        protected SkinSet Skin { get; private set; }

        #endregion Members

        public DirectionHandler DirectionHandler { get; private set; }

        protected State<string> State { get; private set; }

        protected CMovingStateInfo MovingStateInfo;

        public WorldObject() :
            base()
        {
            DrawBottomMargin = 0F;

            Skin = new SkinSet();

            SetState(DEFAULT_STATE);

            DrawOrder = DrawOrder.Normal;

            BBoundingBoxes = new List<BBoundingBox>();
            BodyBoundingBoxes = new List<EBoundingBox>();
            EventBoundingBoxes = new Dictionary<EventBoundingBoxType, List<EBoundingBox>>();
            foreach (EventBoundingBoxType BBT in Enum.GetValues(typeof(EventBoundingBoxType)))
                EventBoundingBoxes[BBT] = new List<EBoundingBox>();

            Position = new Vector2f();
            Dimension = new Vector2f();

            DirectionHandler = new DirectionHandler(this);

            Map = null;

            MoveHandler = new MoveHandler(this);

            IsVisible = true;

            InitMovingStateInfo();

            Stop();
        }

        public WorldObject(WorldObject copy) :
            base(copy)
        {
            DrawBottomMargin = copy.DrawBottomMargin;

            SetState(copy.State);

            DrawOrder = DrawOrder.Normal;

            DirectionHandler = new DirectionHandler(this);

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
            foreach (EventBoundingBoxType EBBType in Enum.GetValues(typeof(EventBoundingBoxType)))
                foreach (EBoundingBox BB in copy.EventBoundingBoxes[EBBType])
                    AddEventBoundingBox(new EBoundingBox(BB, this), EBBType);

            if (copy.Skin != null)
                Skin = new SkinSet(copy.Skin);

            Position = new Vector2f(copy.Position.X,
                                   copy.Position.Y);

            Map = copy.Map;

            MoveHandler = new MoveHandler(this);

            IsVisible = true;

            InitMovingStateInfo();

            Stop();
        }

        protected virtual void InitMovingStateInfo()
        {
            MovingStateInfo = new CMovingStateInfo(this);
        }

        public override void ToScript()
        {
            base.ToScript();

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

            SkinToScript();
        }

        protected virtual void SkinToScript()
        {
            IEnumerator<KeyValuePair<State<string>, Skin>> skinEnum = Skin.GetEnumerator();
            while (skinEnum.MoveNext())
                Sw.WriteMethod("AddSkin", new string[] { ScriptWriter.GetStringOf(skinEnum.Current.Key), skinEnum.Current.Value.ToScriptString() });
        }

        public Skin GetSkin(string state)
        {
            return Skin.GetSkin(GetDrawingState(state));
        }

        string GetDrawingState(string state)
        {
            if (!Skin.ContainsState(state))
                return "Inactive";

            return state;
        }

        public void SetSkin(Skin skin)
        {
            Skin.AddSkin(skin);
        }

        public void AddSkin(string state, Skin skin)
        {
            Skin.AddSkin(state, skin);
        }

        public virtual void Update(Time dt)
        {
            CallOnUpdate();

            MoveHandler.Update();
            MovingStateInfo.Update(dt);

            if (Skin != null)
                Skin.Update(dt);
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            if (Skin != null)
                Skin.Draw(window);
        }

        /// <summary>
        /// TODO: Change skin into ISkin (represents any kind of graphical representation of the object, i-e anim, texture...).
        /// GetSkin returns a texture from the ISkin.
        /// It may be an animation, thus it returns the current frame.
        /// </summary>
        /// <returns></returns>
        public virtual Texture GetSkinTexture()
        {
            return Skin.GetTexture();
        }

        public void Move(float xOffset, float yOffset)
        {
            Move(new Vector2f(xOffset, yOffset), BaseDrawable.DEFAULT_Z);
        }

        public void Move(float xOffset, float yOffset, int zOffset)
        {
            Move(new Vector2f(xOffset, yOffset), zOffset);
        }

        public void MoveTo(float x, float y)
        {
            MoveTo(new Vector2f(x, y), Z);
        }

        public void MoveTo(float x, float y, int z)
        {
            MoveTo(new Vector2f(x, y), z);
        }

        public void Move(Vector2f move)
        {
            MoveTo(Position + move, Z);
        }

        public void Move(Vector2f move, int z)
        {
            MoveTo(Position + move, z);
        }

        public void MoveTo(Vector2f point)
        {
            MoveTo(point, Z);
        }

        public void MoveTo(Vector2f point, int z)
        {
            if (Map == null)
                return;

            Map.Ground.RemoveObjectBoundingBoxes(this);

            Position = point;
            Z = z;

            UpdateBoundingBoxes();

            Map.Ground.AddObjectBoundingBoxes(this);

            Map.UpdateObjectEvents(this);
        }

        void UpdateBoundingBoxes()
        {
            foreach (BBoundingBox BB in BBoundingBoxes)
                BB.Update();

            foreach (EBoundingBox BB in BodyBoundingBoxes)
                BB.Update();

            foreach (EventBoundingBoxType BBT in Enum.GetValues(typeof(EventBoundingBoxType)))
                foreach (EBoundingBox BB in EventBoundingBoxes[BBT])
                    BB.Update();
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

        public void AddEventBoundingBox(EBoundingBox BB, EventBoundingBoxType type = EventBoundingBoxType.External)
        {
            EventBoundingBoxes[type].Add(BB);
            RefreshMapBoundingBoxes();
        }

        public void AddEvent(EBoundingBox BB, ObjectEvent evt, EventBoundingBoxType type = EventBoundingBoxType.External)
        {
            BB.AddEvent(evt);
            AddEventBoundingBox(BB, type);
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

        public void ActivateEvents(bool eventsAreActive = true)
        {
            foreach (EBoundingBox BB in BodyBoundingBoxes)
                BB.Activate(eventsAreActive);

            foreach (EventBoundingBoxType EventBBType in Enum.GetValues(typeof(EventBoundingBoxType)))
                foreach (EBoundingBox BB in EventBoundingBoxes[EventBBType])
                    BB.Activate(eventsAreActive);

            RefreshMapBoundingBoxes();
        }

        public virtual void SetMap(Map map, float x, float y, int z = BaseDrawable.DEFAULT_Z)
        {
            if (Map == map)
            {
                MoveTo(x, y, z);

                return;
            }

            if (Map != null)
            {
                // Remove this from the old map
                Map.RemoveObject(this);
            }
            // Set the new map
            Map = map;
            // Add this to the new map
            Map.AddObject(this);
            // Set this position to (x, y)
            MoveTo(x, y, z);

            CallOnMapChange();
        }

        public virtual void SetMap(Map map, String warpPointName)
        {
            WarpPoint warpPoint = map.GetWarpPoint(warpPointName);

            if (warpPoint == null)
                warpPoint = map.DefaultWarpPoint;

            Vector2f position = warpPoint.Point;

            SetMap(map, position.X, position.Y, warpPoint.Z);

            MovingStateInfo.SetDirection(warpPoint.Direction);
        }

        public virtual void SetMap(String mapType, String warpPointName)
        {
            SetMap(Create.Map(mapType), warpPointName);
        }

        public virtual void UnsetMap()
        {
            Map = null;
        }

        public List<BBoundingBox> BBoundingBoxes { get; private set; }
        public List<EBoundingBox> BodyBoundingBoxes { get; private set; }
        Dictionary<EventBoundingBoxType, List<EBoundingBox>> EventBoundingBoxes { get; set; }

        public List<EBoundingBox> GetEventBoundingBoxes(EventBoundingBoxType EBBType)
        {
            return EventBoundingBoxes[EBBType];
        }

        public List<EBoundingBox> GetActiveEventBoundingBoxes(EventBoundingBoxType EBBType)
        {
            return EventBoundingBoxes[EBBType];
        }

        public override Color Color
        {
            set
            {
                base.Color = value;

                if (Skin != null)
                    Skin.Color = Color;
            }
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                if (Skin != null)
                    Skin.Position = DrawingPosition;
            }
        }

        public override Vector2f BasePoint
        {
            set
            {
                base.BasePoint = value;

                if (Skin != null)
                    Skin.BasePoint = BasePoint;
            }
        }

        public override int Z
        {
            set
            {
                base.Z = value;

                if (Skin != null)
                    Skin.Position = DrawingPosition;
            }
        }

        public override int H
        {
            get
            {
                return 1 + (int)(Dimension.Y / GameData.TILE_SIZE);
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Skin == null)
                    return base.Dimension;

                return Skin.Dimension;
            }
        }

        public Vector2I TPosition
        {
            get
            {
                return Vector2I.FromVector2(Position) / GameData.TILE_SIZE;
            }
            protected set
            {
                Position = value.ToVector2() * GameData.TILE_SIZE;
            }
        }

        public int TLeft
        {
            get
            {
                return (int)Left / GameData.TILE_SIZE;
            }
            set
            {
                Left = value * GameData.TILE_SIZE;
            }
        }

        public int TTop
        {
            get
            {
                return (int)Top / GameData.TILE_SIZE;
            }
            set
            {
                Top = value * GameData.TILE_SIZE;
            }
        }

        public int TRight
        {
            get
            {
                return (int)Right / GameData.TILE_SIZE;
            }
            set
            {
                Right = value * GameData.TILE_SIZE;
            }
        }

        public int TBottom
        {
            get
            {
                return (int)Bottom / GameData.TILE_SIZE;
            }
            set
            {
                Bottom = value * GameData.TILE_SIZE;
            }
        }

        public Map Map
        {
            get;
            private set;
        }

        protected virtual string GetLogicalState()
        {
            return State;
        }

        public bool IsActive()
        {
            return GetLogicalState() != "Inactive";
        }

        public Boolean IsMoving()
        {
            return GetLogicalState() == "Moving";
        }

        public bool TrySetState(string state)
        {
            if (IsActive())
            {
                if (InternalTrySetState(state))
                    return true;

                if (state != "Inactive")
                    return State == state;

                SetState("Inactive");
                return true;
            }

            SetState(state);

            return true;
        }

        /// <summary>
        /// Tries to set the current state checking in the table what are the enabled state transitions.
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        bool InternalTrySetState(string state)
        {
            if (!JoinableStateTable.ContainsKey(State) ||
                !JoinableStateTable[State].Contains(state))
                return false;

            SetState(state);

            return true;
        }

        void SetState(string state)
        {
            State = state;

            CallOnStateChange();

            if (Skin == null)
                return;

            Skin.Stop();

            Skin.SetCurrentState(GetDrawingState(State));

            Skin.Start();
        }

        /// <summary>
        /// Forces the WorldObject.State property to be "Inactive".
        /// </summary>
        public void Stop()
        {
            SetState("Inactive");
        }

        public void StopMove()
        {
            MovingStateInfo.ResetDirectionStates();
        }

        public void EnableDirection(Direction direction)
        {
            MovingStateInfo.Enable(direction);

            CallOnDirectionEnablement(direction);
        }

        public void DisableDirection(Direction direction)
        {
            MovingStateInfo.Enable(direction, false);

            CallOnDirectionDisablement(direction);
        }

        public virtual Direction Direction
        {
            get { return MovingStateInfo.Direction; }
            set { MovingStateInfo.SetDirection(value); }
        }

        public float Velocity
        {
            get { return MovingStateInfo.GetVelocity(); }
            set { MovingStateInfo.SetBaseVelocity(value); }
        }

        public void Accept(IVisitor<WorldObject> visitor)
        {
            visitor.Visit(this);
        }

        public MoveHandler MoveHandler { get; set; }
    }

    #region MovesInfo

    public class MoveHandler
    {
        private const Int32 XDIR = 0;
        private const Int32 YDIR = 1;

        public MoveHandler(WorldObject holder)
        {
            Holder = holder;
            Directions = new Direction[2];
            GoalX = true;
            GoalY = true;
            Path = new Queue<Vector2f>();
        }

        private void Init(Vector2f goal)
        {
            Goal = goal;

            if (Holder.Position.X == Goal.X)
            {
                GoalX = true;
            }
            else
            {
                GoalX = false;
            }

            if (Holder.Position.Y == Goal.Y)
            {
                GoalY = true;
            }
            else
            {
                GoalY = false;
            }

            if (!IsRunning)
            {
                return;
            }

            if (Holder.Position.X < Goal.X)
            {
                Directions[XDIR] = Direction.E;
                Holder.EnableDirection(Direction.E);
            }
            else
            {
                Directions[XDIR] = Direction.O;
                Holder.EnableDirection(Direction.O);
            }

            if (Holder.Position.Y < Goal.Y)
            {
                Directions[YDIR] = Direction.S;
                Holder.EnableDirection(Direction.S);
            }
            else
            {
                Directions[YDIR] = Direction.N;
                Holder.EnableDirection(Direction.N);
            }
        }

        public void Update()
        {
            if (!IsRunning)
            {
                if (Path.Count > 0)
                {
                    Init(Path.Dequeue());
                }
                else
                {
                    return;
                }
            }

            UpdateDirection();
        }

        private void UpdateDirection()
        {
            if (Directions[XDIR] == Direction.E)
            {
                if (Holder.Position.X >= Goal.X)
                {
                    GoalX = true;
                    Holder.DisableDirection(Direction.E);
                    Holder.MoveTo(new Vector2f(Goal.X, Holder.Position.Y)); //illegal
                }
            }
            else
            {
                if (Holder.Position.X <= Goal.X)
                {
                    GoalX = true;
                    Holder.DisableDirection(Direction.O);
                    Holder.MoveTo(new Vector2f(Goal.X, Holder.Position.Y));
                }
            }

            if (Directions[YDIR] == Direction.S)
            {
                if (Holder.Position.Y >= Goal.Y)
                {
                    GoalY = true;
                    Holder.DisableDirection(Direction.S);
                    Holder.MoveTo(new Vector2f(Holder.Position.X, Goal.Y));
                }
            }
            else
            {
                if (Holder.Position.Y <= Goal.Y)
                {
                    GoalY = true;
                    Holder.DisableDirection(Direction.N);
                    Holder.MoveTo(new Vector2f(Holder.Position.X, Goal.Y));
                }
            }
        }

        public void AddPoint(Vector2f point)
        {
            Path.Enqueue(point);
        }

        public void AddPath(List<Vector2f> path)
        {
            foreach (Vector2f point in path)
            {
                AddPoint(point);
            }
        }

        public void AddPoint(Vector2I point)
        {
            Path.Enqueue(new Vector2f(point.X * GameData.TILE_SIZE, point.Y * GameData.TILE_SIZE));
        }

        public void AddPath(List<Vector2I> path)
        {
            foreach (Vector2I point in path)
            {
                AddPoint(point);
            }
        }

        private Boolean IsRunning
        {
            get
            {
                return !GoalX ||
                       !GoalY;
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

    public class DirectionHandler
    {
        const float PERCENT_CENTER_ZONE = 15F;

        WorldObject Holder;

        public DirectionHandler(WorldObject holder)
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