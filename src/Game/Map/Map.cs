using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    #region CWarpPoint

    public class WarpPoint
    {
        static readonly WarpPoint DEFAULT = new WarpPoint("Default", new Vector2f(), GameData.DEFAULT_DIRECTION);

        public String Name { get; set; }
        public Vector2f Point { get; set; }
        public Direction Direction { get; set; }
        public int Z { get; set; }

        public WarpPoint(String name, Vector2f point, Direction direction, int z = BaseDrawable.DEFAULT_Z)
        {
            Name = name;
            Point = point;
            Direction = direction;

            Z = z;
        }

        public WarpPoint(WarpPoint copy)
        {
            Name = copy.Name;
            Point = copy.Point;
            Direction = copy.Direction;

            Z = copy.Z;
        }

        public static WarpPoint GetDefault()
        {
            return new WarpPoint(DEFAULT);
        }
    }

    #endregion

    public class Map : BaseObject, IVisitable<Map>, IVisitable<WorldObject>
    {
        public Dictionary<String, WarpPoint> WarpPoints { get; private set; }
        public WarpPoint DefaultWarpPoint { get; private set; }

        #region Members

        LightEffectManager LightEffectManager;

        #endregion Members

        public Map() :
            base()
        {
            Objects = new List<WorldObject>();

            Elements = new List<WorldElement>();

            Players = new List<Player>();

            NPCs = new List<NPC>();

            ObjectsToDraw = new Dictionary<DrawOrder, List<IDrawable>>();
            foreach (DrawOrder drawOrder in Enum.GetValues(typeof(DrawOrder)))
                ObjectsToDraw.Add(drawOrder, new List<IDrawable>());
            ObjectsToSort = new HashSet<IDrawable>();

            PlayerEvents = new Dictionary<EBoundingBox, List<EBoundingBox>>();

            WarpPoints = new Dictionary<String, WarpPoint>();
            DefaultWarpPoint = WarpPoint.GetDefault();

            SetPhysicsIsRunning();

            LightEffectManager = new LightEffectManager(this);
        }

        public Map(Map copy) :
            base(copy)
        {
            Ground = new Ground(copy.Ground);

            PlayerEvents = new Dictionary<EBoundingBox, List<EBoundingBox>>();

            WarpPoints = new Dictionary<string, WarpPoint>(copy.WarpPoints);
            DefaultWarpPoint = new WarpPoint(copy.DefaultWarpPoint);

            SetPhysicsIsRunning();

            MusicType = copy.MusicType;

            ObjectsToDraw = new Dictionary<DrawOrder, List<IDrawable>>();
            foreach (DrawOrder drawOrder in Enum.GetValues(typeof(DrawOrder)))
                ObjectsToDraw.Add(drawOrder, new List<IDrawable>());
            ObjectsToSort = new HashSet<IDrawable>();

            Objects = new List<WorldObject>();
            Elements = new List<WorldElement>();
            Players = new List<Player>();
            NPCs = new List<NPC>();
            foreach (WorldObject wObj in copy.Objects)
                ((WorldObject)wObj.Clone()).SetMap(this, wObj.Position.X, wObj.Position.Y);

            LightEffectManager = new LightEffectManager(this, copy.LightEffectManager);
        }

        public override void SetType(String type, Boolean creation = false)
        {
            base.SetType(type, creation);

            if (!creation)
                return;

            Ground = Create.Ground(Type);
            Ground.Generate();
        }

        public virtual Vector2f GetPositionFromCell(Vector2I cell)
        {
            return cell.ToVector2() * GameData.TILE_SIZE;
        }

        public void Init()
        {
            if (MusicType != null)
            {
                SoundManager.Instance.PlayMusic(MusicType);
            }

            LightEffectManager.Init();
        }

        #region WarpPoint

        public void AddWarpPoint(String name, Vector2f point, Direction direction, Boolean defaultWarpPoint = false)
        {
            AddWarpPoint(new WarpPoint(name, point, direction), defaultWarpPoint);
        }

        public void AddWarpPoint(WarpPoint warpPoint, Boolean defaultWarpPoint = false)
        {
            if (WarpPoints.Count == 0 || defaultWarpPoint)
                DefaultWarpPoint = warpPoint;

            if (WarpPoints.ContainsKey(warpPoint.Name))
                WarpPoints[warpPoint.Name] = warpPoint;
            else
                WarpPoints.Add(warpPoint.Name, warpPoint);
        }

        public Boolean RemoveWarpPoint(String warpPointName)
        {
            if (warpPointName == null)
                return false;

            if (!WarpPoints.Remove(warpPointName))
                return false;

            if (DefaultWarpPoint.Name == warpPointName &&
                WarpPoints.Count > 0)
                DefaultWarpPoint = WarpPoints.Values.ToArray<WarpPoint>()[0];

            return true;
        }

        public WarpPoint GetWarpPoint(String name = null)
        {
            if (name != null &&
                WarpPoints.ContainsKey(name))
                return WarpPoints[name];

            if (DefaultWarpPoint != null)
                return DefaultWarpPoint;

            return null;
        }

        public void SetWarpPoint(String name, WarpPoint warpPoint, Boolean defaultWarpPoint = false)
        {
            if (!WarpPoints.ContainsKey(name))
                return;

            WarpPoints[name] = warpPoint;

            if (defaultWarpPoint || DefaultWarpPoint.Name == name)
                DefaultWarpPoint = warpPoint;
        }

        #endregion

        #region Scripting

        public override void ToScript()
        {
            Sw = new ScriptWriter(this);

            Sw.InitObject();

            base.ToScript();

            Sw.WriteProperty("Ground", "Create:Ground(\"" + Ground.Type + "\")");
            Ground.ToScript();

            foreach (WorldElement element in Elements)
            {
                Sw.WriteObjectCreation(element);

                String str = element.Id + ":SetMap(" + Type + ", " + element.Position.X.ToString() + ", " + element.Position.Y.ToString();
                Sw.WriteLine(ScriptWriter.GetStrMethod(
                    element.Id,
                    "SetMap",
                    new String[]
                    {
                        LongType,
                        element.Position.X.ToString(),
                        element.Position.Y.ToString()
                    }));

                foreach (EBoundingBox BB in element.GetEventBoundingBoxes(EventBoundingBoxType.External))
                    Sw.WriteLine(BB.ToScript());
            }

            foreach (WarpPoint warpPoint in WarpPoints.Values)
                Sw.WriteMethod("AddWarpPoint", new String[]
                {
                    ScriptWriter.GetStringOf(warpPoint.Name),
                    ScriptWriter.GetStrOfVector2(warpPoint.Point),
                    ScriptWriter.GetStrOfDirection(warpPoint.Direction)
                });

            Sw.EndObject();
        }

        #endregion

        #region Update

        public virtual void Update(Time dt)
        {
            LightEffectManager.Update(dt);

            MapEffectManager.Instance.Update(dt);
            while (!MapEffectManager.Instance.IsEmpty())
            {
                MapEffect effect = MapEffectManager.Instance.GetEffect();
                effect.OnStopping += new MapEffectEventHandler(effect_OnStopping);
                effect.Start();

                AddObjectToDraw(effect.DrawOrder, effect);
            }

            if (MoveEngineIsRunning)
                UpdateMoves(dt);

            if (EventEngineIsRunnging)
                UpdateEvents();

            Ground.Update(dt);

            IEnumerator<WorldObject> wObjs = Objects.GetEnumerator();
            while (wObjs.MoveNext())
                wObjs.Current.Update(dt);
        }

        void effect_OnStopping(MapEffect sender, MapEffectEventArgs e)
        {
            RemoveObjectToDraw(sender.DrawOrder, sender);
        }

        #endregion

        #region Draw

        protected void DrawGround(RenderTarget window)
        {
            Ground.Draw(window);
        }

        void DrawObjects(RenderTarget window)
        {
            SortDrawingObjects(ObjectsToDraw[DrawOrder.Normal]);
            foreach (DrawOrder drawOrder in Enum.GetValues(typeof(DrawOrder)))
                foreach (IDrawable iDrawable in ObjectsToDraw[drawOrder])
                   // if (ViewContainsObject(window, iDrawable)) //!\\ decreases perf' //!\\
                        iDrawable.Draw(window);
        }

        void DrawLightEffects(RenderTarget window)
        {
            LightEffectManager.Draw(window);
        }

        public virtual void Draw(RenderTarget window)
        {
            DrawGround(window);

            DrawObjects(window);

            DrawLightEffects(window);
        }

        const float OBJECT_DRAW_MARGIN = 10F;
        Boolean ViewContainsObject(RenderTarget window, IDrawable drawable)
        {
            Vector2f viewTopLeft = window.GetView().Center - window.GetView().Size / 2F;
            Vector2f viewBottomRight = viewTopLeft + window.GetView().Size;

            FloatRect viewRect = new FloatRect(
                viewTopLeft.X - OBJECT_DRAW_MARGIN,
                viewTopLeft.Y - OBJECT_DRAW_MARGIN,
                viewBottomRight.X + OBJECT_DRAW_MARGIN,
                viewBottomRight.Y + OBJECT_DRAW_MARGIN);

            FloatRect objRect = drawable.GetVisibleRect();

            return !(
                objRect.Right < viewRect.Left ||
                objRect.Bottom < viewRect.Top ||
                objRect.Left >= viewRect.Right ||
                objRect.Top >= viewRect.Bottom);
        }

        protected Dictionary<DrawOrder, List<IDrawable>> ObjectsToDraw;

        public void AddObjectToDraw(DrawOrder drawOrder, IDrawable obj)
        {
            ObjectsToDraw[drawOrder].Add(obj);

            if (drawOrder == DrawOrder.Normal)
            {
                ObjectsToSort.Add(obj);
                obj.OnMove += new MoveEventHandler(obj_OnMove);
            }
        }

        void obj_OnMove(IDrawable sender, MoveEventArgs e)
        {
            ObjectsToSort.Add(sender);
        }

        public bool RemoveObjectToDraw(DrawOrder drawOrder, IDrawable obj)
        {
            if (!ObjectsToDraw[drawOrder].Remove(obj))
                return false;

            if (drawOrder == DrawOrder.Normal)
                obj.OnMove -= new MoveEventHandler(obj_OnMove);

            return true;
        }

        public bool ContainsObjectToDraw(DrawOrder drawOrder, IDrawable obj)
        {
            foreach (IDrawable iDrawable in ObjectsToDraw[drawOrder])
                if (iDrawable == obj)
                    return true;

            return false;
        }

        HashSet<IDrawable> ObjectsToSort;
        void SortDrawingObjects(List<IDrawable> drawables)
        {
            foreach (IDrawable drawable in ObjectsToSort)
                if (!drawables.Remove(drawable))
                    ObjectsToSort.Remove(drawable);

            ZOrderComparer comparer = new ZOrderComparer();
            foreach (IDrawable drawable in ObjectsToSort)
            {
                int count = 0;
                for (; count < drawables.Count; ++count)
                {
                    if (comparer.Compare(drawable, drawables[count]) <= 0)
                        break;
                }

                drawables.Insert(count, drawable);
            }

            ObjectsToSort.Clear();
        }

        #endregion

        #region Objects

        protected List<WorldObject> Objects { get; private set; }

        protected List<WorldElement> Elements { get; private set; }

        protected List<Player> Players { get; private set; }

        protected List<NPC> NPCs { get; private set; }

        public virtual void AddObject(WorldObject wObj)
        {
            Objects.Add(wObj);

            if (wObj is WorldElement)
                Elements.Add((WorldElement)wObj);

            else if (wObj is Player)
                Players.Add((Player)wObj);

            else if (wObj is NPC)
                NPCs.Add((NPC)wObj);

            UpdateObjectEvents(wObj);

            AddObjectToDraw(wObj.DrawOrder, wObj);
        }

        public virtual void RemoveObject(WorldObject wObj)
        {
            Ground.RemoveObjectBoundingBoxes(wObj);

            wObj.UnsetMap();

            Objects.Remove(wObj);

            if (wObj is WorldElement)
                Elements.Remove((WorldElement)wObj);

            else if (wObj is Player)
                Players.Remove((Player)wObj);

            else if (wObj is NPC)
                NPCs.Remove((NPC)wObj);

            RemoveObjectEvents(wObj);

            RemoveObjectToDraw(wObj.DrawOrder, wObj);
        }

        public List<WorldObject> GetObjects()
        {
            return Objects;
        }

        public IEnumerator<WorldObject> GetEnumerator()
        {
            return Objects.GetEnumerator();
        }

        public Int32 GetObjectCount()
        {
            return Objects.Count;
        }

        public List<WorldElement> GetElements()
        {
            return Elements;
        }

        #endregion

        #region Physics
        
        bool DecomposedMove;
        bool MoveEngineIsRunning;
        bool EventEngineIsRunnging;

        public void SetPhysicsIsRunning(bool physicsIsRunning = true)
        {
            MoveEngineIsRunning = physicsIsRunning;
            EventEngineIsRunnging = physicsIsRunning;
        }

        public void SetEventEngineIsRunning(bool eventEngineIsRunning = true)
        {
            EventEngineIsRunnging = eventEngineIsRunning;
        }

        public bool PhysicsIsRunning()
        {
            return MoveEngineIsRunning;
        }

        public bool BoundingBoxTest(BBoundingBox BB, bool temporarlyActivate = false)
        {
            if (temporarlyActivate)
                BB.Activate();

            BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2f());

            for (Int32 y = tmp.Top; y < tmp.Bottom + 1; ++y)
                for (Int32 x = tmp.Left; x < tmp.Right + 1; ++x)
                {
                    IEnumerator<BBoundingBox> BBEnum = Ground.GetBBoundingBoxesEnumerator(x, y, BB.Z);
                    while (BBEnum.MoveNext())
                        if (BB.BoundingBoxTest(BBEnum.Current))
                        {
                            if (temporarlyActivate)
                                BB.Activate(false);
                            return true;
                        }
                }

            if (temporarlyActivate)
                BB.Activate(false);

            return false;
        }

        #region Moves

        const float DECOMPOSITION_LIMIT = .5F;
        const Int32 DECOMPOSITION_COUNT = 1;
        const bool DECOMPOSITION_IS_ACTIVE = true;
        public void DisableAllMoves()
        {
            IEnumerator<WorldObject> objectsEnum = Objects.GetEnumerator();
            while (objectsEnum.MoveNext())
                objectsEnum.Current.StopMove();
        }

        public List<WorldObject> GetMovingObjects()
        {
            List<WorldObject> movingObjects = new List<WorldObject>();

            IEnumerator<WorldObject> iEnum = Objects.GetEnumerator();
            while (iEnum.MoveNext())
                if (iEnum.Current.IsMoving())
                    movingObjects.Add(iEnum.Current);

            return movingObjects;
        }

        public void UpdateMoves(Time dt)
        {
            foreach (WorldObject wObj in GetMovingObjects())
                UpdateObjectMoves(dt, wObj);
        }

        public void UpdateObjectMoves(Time dt, WorldObject wObj)
        {
            Vector2f move = GetMove(wObj, dt);

            if (!DECOMPOSITION_IS_ACTIVE ||
                (Math.Abs(move.X) < DECOMPOSITION_LIMIT &&
                Math.Abs(move.Y) < DECOMPOSITION_LIMIT))
            {
                wObj.Move(new Vector2f(GetPhysicalMoveX(wObj, move.X), GetPhysicalMoveY(wObj, move.Y)));

                return;
            }

            float moveAverage = (Math.Abs(move.X) + Math.Abs(move.Y)) / 2F;
            Int32 decompositionMoveFactor = (Int32)(moveAverage >= 1F ? moveAverage : 1F / moveAverage);

            Int32 decompositionCount = (Int32)((float)DECOMPOSITION_COUNT / DECOMPOSITION_LIMIT) * decompositionMoveFactor;

            for (Int32 count = 0; count < decompositionCount; ++count)
            {
                Vector2f decomposedMove = move / (float)decompositionCount;

                wObj.Move(new Vector2f(GetPhysicalMoveX(wObj, decomposedMove.X), GetPhysicalMoveY(wObj, decomposedMove.Y)));
            }
        }

        protected Vector2f GetMove(WorldObject wObj, Time dt)
        {
            Vector2f move = new Vector2f();

            float velocity = wObj.Velocity;

            float d = (float)(velocity * dt.Value);
            float dd = (float)(velocity * dt.Value / Math.Sqrt(2));

            switch (wObj.Direction)
            {
                case Direction.N:   move = new Vector2f(0F, -d);     break;
                case Direction.S:   move = new Vector2f(0F, d);      break;
                case Direction.E:   move = new Vector2f(d, 0F);      break;
                case Direction.O:   move = new Vector2f(-d, 0F);     break;
                case Direction.NE:  move = new Vector2f(dd, -dd);    break;
                case Direction.NO:  move = new Vector2f(-dd, -dd);   break;
                case Direction.SE:  move = new Vector2f(dd, dd);     break;
                case Direction.SO:  move = new Vector2f(-dd, dd);    break;
            }

            return move;
        }

        public static Vector2f GetMove(Direction direction, float velocity, Time dt)
        {
            Vector2f move = new Vector2f();

            float d = (float)(velocity * dt.Value);

            switch (direction)
            {
                case Direction.N: move = new Vector2f(0F, -d); break;
                case Direction.S: move = new Vector2f(0F, d); break;
                case Direction.E: move = new Vector2f(d, 0F); break;
                case Direction.O: move = new Vector2f(-d, 0F); break;
            }

            return move;
        }

        private float GetPhysicalMoveX(WorldObject wObj, float offset)
        {
            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2f(offset, 0.0f));

                for (Int32 y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (Int32 x = tmp.Left; x < tmp.Right + 1; ++x)
                        if (Ground.GetBlock(x, y))
                            return 0.0f;
            }

            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2f(offset, 0.0f));

                for (int y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (int x = tmp.Left; x < tmp.Right + 1; ++x)
                    {
                        IEnumerator<BBoundingBox> iEnum = Ground.GetBBoundingBoxesEnumerator(x, y, BB.Z);
                        while (iEnum.MoveNext())
                        {
                            if (BB.BoundingBoxTest(iEnum.Current, new Vector2f(offset, 0F)))
                                return 0F;
                        }
                    }
            }

            return offset;
        }

        private float GetPhysicalMoveY(WorldObject wObj, float offset)
        {
            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2f(0.0f, offset));

                for (Int32 y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (Int32 x = tmp.Left; x < tmp.Right + 1; ++x)
                        if (Ground.GetBlock(x, y))
                            return 0.0f;
            }

            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2f(0.0f, offset));

                for (int y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (int x = tmp.Left; x < tmp.Right + 1; ++x)
                    {
                        IEnumerator<BBoundingBox> iEnum = Ground.GetBBoundingBoxesEnumerator(x, y, BB.Z);
                        while (iEnum.MoveNext())
                        {
                            if (BB.BoundingBoxTest(iEnum.Current, new Vector2f(0F, offset)))
                                return 0F;
                        }
                    }
            }

            return offset;
        }

        #endregion

        #region Events

        Dictionary<EBoundingBox, List<EBoundingBox>> PlayerEvents; // Dictionary<SourceBB, List<TriggerBB>>

        void CallEvent(ObjectEventType objectEventType, ObjectEventArgs args)
        {
            if (objectEventType == ObjectEventType.In &&
                !AddPlayerEvent(args))
                return;

            if (objectEventType == ObjectEventType.Out &&
                !RemovePlayerEvent(args))
                return;

            args.Source.CallEvent(objectEventType, args);
        }

        void CallEvent(ObjectEventType objectEventType, EBoundingBox sourceBB, EBoundingBox triggerBB)
        {
            CallEvent(objectEventType, new ObjectEventArgs(sourceBB, triggerBB));
        }

        Boolean AddPlayerEvent(ObjectEventArgs args)
        {
            return AddPlayerEvent(args.Source, args.Trigger);
        }

        Boolean AddPlayerEvent(EBoundingBox sourceBB, EBoundingBox triggerBB)
        {
            if (!PlayerEvents.ContainsKey(sourceBB))
                PlayerEvents.Add(sourceBB, new List<EBoundingBox>());

            if (PlayerEvents[sourceBB].Contains(triggerBB))
                return false;

            PlayerEvents[sourceBB].Add(triggerBB);

            return true;
        }

        Boolean RemovePlayerEvent(ObjectEventArgs args)
        {
            return RemovePlayerEvent(args.Source, args.Trigger);
        }

        Boolean RemovePlayerEvent(EBoundingBox sourceBB, EBoundingBox triggerBB = null)
        {
            if (triggerBB == null)
                return PlayerEvents.Remove(sourceBB);

            if (!PlayerEvents.ContainsKey(sourceBB))
                return false;

            if (!PlayerEvents[sourceBB].Remove(triggerBB))
                return false;

            if (PlayerEvents[sourceBB].Count == 0)
                return PlayerEvents.Remove(sourceBB);

            return true;
        }

        void RemoveObjectEvents(WorldObject wObj)
        {
            Queue<KeyValuePair<EBoundingBox, EBoundingBox>> eventsToRemove = new Queue<KeyValuePair<EBoundingBox, EBoundingBox>>();

            foreach (EBoundingBox sourceBB in PlayerEvents.Keys)
            {
                if (sourceBB.Holder == wObj)
                {
                    eventsToRemove.Enqueue(new KeyValuePair<EBoundingBox, EBoundingBox>(sourceBB, null));
                    continue;
                }

                foreach (EBoundingBox triggerBB in PlayerEvents[sourceBB])
                    if (triggerBB.Holder == wObj)
                        eventsToRemove.Enqueue(new KeyValuePair<EBoundingBox, EBoundingBox>(sourceBB, triggerBB));
            }

            while (eventsToRemove.Count > 0)
            {
                KeyValuePair<EBoundingBox, EBoundingBox> eventToRemove = eventsToRemove.Dequeue();
                RemovePlayerEvent(eventToRemove.Key, eventToRemove.Value);
            }
        }

        public Boolean EventsAreInterrupted;

        public void InterruptEvents()
        {
            PlayerEvents.Clear();
        }

        public void RefreshEvents()
        {
            foreach (WorldObject wObj in Objects)
                UpdateObjectEvents(wObj);
        }

        private void UpdateEvents()
        {
            if (EventsAreInterrupted)
                EventsAreInterrupted = false;

            CallEvents();
        }

        public void UpdateObjectEvents(WorldObject wObj)
        {
            if (!EventEngineIsRunnging)
                return;

            if (wObj is Player)
            {
                UpdateEvents((Player)wObj);
                return;
            }

            foreach (Player player in Players)
                UpdateEvents(wObj, player);
        }


        /* TODO
         * ajout de la gestion des hauteurs
         * gerer pour chaque perso sur la map
         * gestion entre items
         * 
         * 
         */

        // NPC NPC has moved --> Handle of event with Player player
        private void UpdateEvents(WorldObject wObj, Player player)
        {
            foreach (EventBoundingBoxType eventBBType in Enum.GetValues(typeof(EventBoundingBoxType)))
            {
                foreach (EBoundingBox sNPCBB in wObj.GetActiveEventBoundingBoxes(eventBBType))
                {
                    foreach (EBoundingBox triggerBB in player.BodyBoundingBoxes)
                    {
                        if (triggerBB.Z != sNPCBB.Z)
                            continue;

                        if (triggerBB.BoundingBoxTest(sNPCBB))
                            CallEvent(ObjectEventType.In, new ObjectEventArgs(sNPCBB, triggerBB));
                        else
                            CallEvent(ObjectEventType.Out, new ObjectEventArgs(sNPCBB, triggerBB));

                        if (EventsAreInterrupted)
                            return;
                    }
                }
            }
        }

        // Player player has moved
        private void UpdateEvents(Player player)
        {
            Queue<KeyValuePair<EBoundingBox, EBoundingBox>> events = new Queue<KeyValuePair<EBoundingBox, EBoundingBox>>();

            foreach (EBoundingBox sourceBB in PlayerEvents.Keys)
            {
                foreach (EBoundingBox triggerBB in PlayerEvents[sourceBB])
                {
                    if (sourceBB.BoundingBoxTest(triggerBB))
                        continue;

                    events.Enqueue(new KeyValuePair<EBoundingBox, EBoundingBox>(sourceBB, triggerBB));
                }

                if (EventsAreInterrupted)
                    return;
            }

            while (events.Count > 0)
            {
                KeyValuePair<EBoundingBox, EBoundingBox> BBs = events.Dequeue();
                CallEvent(ObjectEventType.Out, new ObjectEventArgs(BBs.Key, BBs.Value));

                if (EventsAreInterrupted)
                    return;
            }

            Dictionary<EBoundingBox, List<EBoundingBox>> inEvents = new Dictionary<EBoundingBox, List<EBoundingBox>>();

            foreach (EBoundingBox triggerBB in player.BodyBoundingBoxes)
                for (int y = triggerBB.TTop; y < triggerBB.TBottom + 1; ++y)
                    for (int x = triggerBB.TLeft; x < triggerBB.TRight + 1; ++x)
                    {
                        IEnumerator<EBoundingBox> NPCBBs = Ground.GetEBoundingBoxesEnumerator(x, y, triggerBB.Z);
                        while (NPCBBs.MoveNext())
                        {
                            EBoundingBox NPCBB = NPCBBs.Current;

                            if (NPCBB.Type != EBoundingBoxType.Event)
                                continue;

                            if (!triggerBB.BoundingBoxTest(NPCBB))
                                continue;

                            if (inEvents.ContainsKey(NPCBB) &&
                                inEvents[NPCBB].Contains(triggerBB))
                                continue;

                            KeyValuePair<EBoundingBox, EBoundingBox> evt = new KeyValuePair<EBoundingBox, EBoundingBox>(NPCBB, triggerBB);
                            if (!inEvents.ContainsKey(NPCBB))
                                inEvents.Add(NPCBB, new List<EBoundingBox>());
                            inEvents[NPCBB].Add(triggerBB);
                            events.Enqueue(evt);
                        }
                    }

            
            while (events.Count > 0)
            {
                KeyValuePair<EBoundingBox, EBoundingBox> BBs = events.Dequeue();
                CallEvent(ObjectEventType.In, new ObjectEventArgs(BBs.Key, BBs.Value));

                if (EventsAreInterrupted)
                    return;
            }
        }

        private void CallEvents()
        {
            Queue<ObjectEventArgs> eventsToCall = new Queue<ObjectEventArgs>();

            foreach (EBoundingBox sourceBB in PlayerEvents.Keys)
                foreach (EBoundingBox triggerBB in PlayerEvents[sourceBB])
                    eventsToCall.Enqueue(new ObjectEventArgs(sourceBB, triggerBB));

            while (eventsToCall.Count > 0)
            {
                CallEvent(ObjectEventType.Normal, eventsToCall.Dequeue());

                if (EventsAreInterrupted)
                    return;
            }
        }

        #endregion

        #endregion

        #region Ground

        public Ground Ground { get; protected set; }

        #endregion

        #region Dimension

        public Boolean ContainsPoint(Vector2f point)
        {
            return !(
                point.X < 0F ||
                point.Y < 0F ||
                point.X >= Dimension.X ||
                point.Y >= Dimension.Y);
        }

        public Boolean ContainsPoint(Vector2I point)
        {
            return ContainsPoint(point.ToVector2());
        }

        public Vector2f Center
        {
            get { return Dimension / 2F; }
        }

        public virtual Vector2f Dimension
        {
            get
            {
                return new Vector2f(Width * GameData.TILE_SIZE,
                                   Height * GameData.TILE_SIZE);
            }
        }

        public virtual int Width
        {
            get { return Ground.Width; }
        }

        public virtual int Height
        {
            get { return Ground.Height; }
        }

        public Vector2f Halfsize
        {
            get { return Dimension / 2F; }
        }

        #endregion

        #region Sound

        private String MusicType { get; set; }

        #endregion

        #region Light

        public void AddDynamicLightEffect(LightEffect lightEffect)
        {
            LightEffectManager.AddDynamicEffect(lightEffect);
        }

        public void RemoveDynamicLightEffect(LightEffect lightEffect)
        {
            LightEffectManager.RemoveDynamicEffect(lightEffect);
        }

        public void AddStaticLightEffect(LightEffect lightEffect)
        {
            LightEffectManager.AddStaticEffect(lightEffect);
        }

        public void RemoveStaticLightEffect(LightEffect lightEffect)
        {
            LightEffectManager.RemoveStaticEffect(lightEffect);
        }

        public void SetGlobalColor(Color color)
        {
            LightEffectManager.GlobalColor = color;
        }

        public void ActivateLightEngine(bool isActive = true)
        {
            LightEffectManager.IsActive = isActive;
        }

        #endregion Light

        public void Accept(IVisitor<Map> visitor)
        {
            visitor.Visit(this);
        }

        public void Accept(IVisitor<WorldObject> visitor)
        {
            foreach (WorldObject wObj in Objects)
                wObj.Accept(visitor);
        }

        Screen Parent;
        public void SetParent(Screen parent)
        {
            Parent = parent;
        }

        MapBaseWidget Gui;
        public void InitGui()
        {
            Gui = new MapBaseWidget();

            Parent.Gui.AddGameWidget(Gui);
        }

        public void AddWidget(GameWidget gameWidget)
        {
            Gui.AddWidget(gameWidget);
        }

        public void AsyncRemoveWidget(GameWidget gameWidget)
        {
            Gui.AsyncRemoveWidget(gameWidget);
        }

        public bool RemoveWidget(GameWidget gameWidget)
        {
            return Gui.RemoveWidget(gameWidget);
        }
    }
    
   /* public class ZOrderComparer : IComparer<IDrawable>
    {
        public int Compare(IDrawable left, IDrawable right)
        {
            Int32 dif =
                (int)(BaseDrawable.GetComparisonPointYByType(left, left.ComparisonPointYType) -
                BaseDrawable.GetComparisonPointYByType(right, right.ComparisonPointYType));

            if (left is Platform)
            {
                if (dif > 0)
                {
                     if (left.Z >= right.Summit)
                    // left is in front of right
                    return -1;
                }

                return 1;

            }

            if (dif > 0 )
            {
                //if (right.Z < left.Summit)
                    // left is in front of right
                    return 1;
            }

            if (left.Z >= right.Summit)
                return 1;

            return -1;
        }
    }*/

    public class ZComparer : IComparer<IDrawable>
    {
        public int Compare(IDrawable left, IDrawable right)
        {
            return left.Z - right.Z;
        }
    }
    
    //!\\ TODO : to fix bug of big object in front of a surelevated one ==> it is drawn under //!\\
    public class ZOrderComparer : IComparer<IDrawable>
    {
        public int Compare(IDrawable left, IDrawable right)
        {
            if (left.Z > right.Z)
                return 1;

            if (right.Z > left.Z)
                return -1;

            Int32 dif =
                (int)(BaseDrawable.GetComparisonPointYByType(left, left.ComparisonPointYType) -
                BaseDrawable.GetComparisonPointYByType(right, right.ComparisonPointYType));

            if (dif == 0)
                dif = (Int32)(left.Position.X - right.Position.Y); // just a trick to avoid scintillment

            return dif < 0 ? -1 : 1;
        }
    }
}
