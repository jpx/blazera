using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    #region CWarpPoint

    public class WarpPoint
    {
        static readonly WarpPoint DEFAULT = new WarpPoint("Default", new Vector2(), GameDatas.DEFAULT_DIRECTION);

        public String Name { get; set; }
        public Vector2 Point { get; set; }
        public Direction Direction { get; set; }

        public WarpPoint(String name, Vector2 point, Direction direction)
        {
            this.Name = name;
            this.Point = point;
            this.Direction = direction;
        }

        public WarpPoint(WarpPoint copy)
        {
            Name = copy.Name;
            Point = copy.Point;
            Direction = copy.Direction;
        }

        public static WarpPoint GetDefault()
        {
            return new WarpPoint(DEFAULT);
        }
    }

    #endregion

    public class Map : BaseObject
    {
        public Dictionary<String, WarpPoint> WarpPoints { get; private set; }
        public WarpPoint DefaultWarpPoint { get; private set; }

        public Map() :
            base()
        {
            this.Objects = new List<WorldObject>();

            this.Elements = new List<WorldElement>();

            this.Players = new List<Player>();

            this.NPCs = new List<NPC>();

            ObjectsToDraw = new Dictionary<DrawOrder, List<IDrawable>>();
            foreach (DrawOrder drawOrder in Enum.GetValues(typeof(DrawOrder)))
                ObjectsToDraw.Add(drawOrder, new List<IDrawable>());

            PlayerEvents = new Dictionary<EBoundingBox, List<EBoundingBox>>();

            WarpPoints = new Dictionary<String, WarpPoint>();
            DefaultWarpPoint = WarpPoint.GetDefault();

            SetPhysicsIsRunning();
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

            Objects = new List<WorldObject>();
            Elements = new List<WorldElement>();
            Players = new List<Player>();
            NPCs = new List<NPC>();
            foreach (WorldObject wObj in copy.Objects)
                ((WorldObject)System.Activator.CreateInstance(wObj.GetType(), wObj)).SetMap(this, wObj.Position.X, wObj.Position.Y);
        }

        public override void SetType(String type, Boolean creation = false)
        {
            base.SetType(type, creation);

            if (!creation)
                return;

            Ground = Create.Ground(Type);
            Ground.Generate();
        }

        public virtual Vector2 GetPositionFromCell(Vector2I cell)
        {
            return cell.ToVector2() * GameDatas.TILE_SIZE;
        }

        public void Init()
        {
            if (this.MusicType != null)
            {
                SoundManager.Instance.PlayMusic(this.MusicType);
            }
        }

        #region WarpPoint

        public void AddWarpPoint(String name, Vector2 point, Direction direction, Boolean defaultWarpPoint = false)
        {
            AddWarpPoint(new WarpPoint(name, point, direction), defaultWarpPoint);
        }

        public void AddWarpPoint(WarpPoint warpPoint, Boolean defaultWarpPoint = false)
        {
            if (this.WarpPoints.Count == 0 || defaultWarpPoint)
                this.DefaultWarpPoint = warpPoint;

            if (this.WarpPoints.ContainsKey(warpPoint.Name))
                this.WarpPoints[warpPoint.Name] = warpPoint;
            else
                this.WarpPoints.Add(warpPoint.Name, warpPoint);
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
                this.WarpPoints.ContainsKey(name))
                return this.WarpPoints[name];

            if (this.DefaultWarpPoint != null)
                return this.DefaultWarpPoint;

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
            this.Sw = new ScriptWriter(this);

            this.Sw.InitObject();

            base.ToScript();

            Sw.WriteProperty("Ground", "Create:Ground(\"" + Ground.Type + "\")");
            Ground.ToScript();

            foreach (WorldElement element in this.Elements)
            {
                this.Sw.WriteObjectCreation(element);

                String str = element.Id + ":SetMap(" + this.Type + ", " + element.Position.X.ToString() + ", " + element.Position.Y.ToString();
                this.Sw.WriteLine(ScriptWriter.GetStrMethod(
                    element.Id,
                    "SetMap",
                    new String[]
                    {
                        this.LongType,
                        element.Position.X.ToString(),
                        element.Position.Y.ToString()
                    }));

                foreach (EBoundingBox BB in element.EventBoundingBoxes[EventBoundingBoxType.External])
                    Sw.WriteLine(BB.ToScript());
            }

            foreach (WarpPoint warpPoint in WarpPoints.Values)
                Sw.WriteMethod("AddWarpPoint", new String[]
                {
                    ScriptWriter.GetStringOf(warpPoint.Name),
                    ScriptWriter.GetStrOfVector2(warpPoint.Point),
                    ScriptWriter.GetStrOfDirection(warpPoint.Direction)
                });

            this.Sw.EndObject();
        }

        #endregion

        #region Update

        public virtual void Update(Time dt)
        {
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

        protected void DrawGround(RenderWindow window)
        {
            Ground.Draw(window);
        }

        //!\\ mettre en place un dico<draworder, list> permanent pour une opti (trier uniquement perso en mouvement [et ajoutes])
        protected void DrawObjects(RenderWindow window)
        {
            Dictionary<DrawOrder, List<IDrawable>> drawingObjects = new Dictionary<DrawOrder, List<IDrawable>>();
            foreach (DrawOrder drawOrder in Enum.GetValues(typeof(DrawOrder)))
                drawingObjects.Add(drawOrder, new List<IDrawable>());

            IEnumerator<WorldObject> wObjects = this.Objects.GetEnumerator();
            while (wObjects.MoveNext())
            {
                WorldObject wObj = wObjects.Current;

                if (!wObj.IsVisible)
                    continue;

                if (!ViewContainsObject(window, wObj))
                    continue;

                drawingObjects[wObj.DrawOrder].Add(wObj);
            }

            foreach (DrawOrder drawOrder in ObjectsToDraw.Keys)
                foreach (IDrawable iDrawable in ObjectsToDraw[drawOrder])
                    drawingObjects[drawOrder].Add(iDrawable);

            SortDrawingObjects(drawingObjects[DrawOrder.Normal]);
            foreach (DrawOrder drawOrder in Enum.GetValues(typeof(DrawOrder)))
                foreach (IDrawable iDrawable in drawingObjects[drawOrder])
                    iDrawable.Draw(window);
        }

        public virtual void Draw(RenderWindow window)
        {
            DrawGround(window);

            DrawObjects(window);
        }

        const float OBJECT_DRAW_MARGIN = 10F;
        Boolean ViewContainsObject(RenderWindow window, WorldObject wObj)
        {
            Vector2 viewTopLeft = window.GetView().Center - window.GetView().Size / 2F;
            Vector2 viewBottomRight = viewTopLeft + window.GetView().Size;

            FloatRect viewRect = new FloatRect(
                viewTopLeft.X - OBJECT_DRAW_MARGIN,
                viewTopLeft.Y - OBJECT_DRAW_MARGIN,
                viewBottomRight.X + OBJECT_DRAW_MARGIN,
                viewBottomRight.Y + OBJECT_DRAW_MARGIN);

            return !(
                wObj.Right < viewRect.Left ||
                wObj.Bottom < viewRect.Top ||
                wObj.Left >= viewRect.Right ||
                wObj.Top >= viewRect.Bottom);
        }

        protected Dictionary<DrawOrder, List<IDrawable>> ObjectsToDraw;

        public void AddObjectToDraw(DrawOrder drawOrder, IDrawable obj)
        {
            ObjectsToDraw[drawOrder].Add(obj);
        }

        public bool RemoveObjectToDraw(DrawOrder drawOrder, IDrawable obj)
        {
            return ObjectsToDraw[drawOrder].Remove(obj);
        }

        public bool ContainsObjectToDraw(DrawOrder drawOrder, IDrawable obj)
        {
            foreach (IDrawable iDrawable in ObjectsToDraw[drawOrder])
                if (iDrawable == obj)
                    return true;

            return false;
        }

        void SortDrawingObjects(List<IDrawable> drawables)
        {
            Queue<IDrawable> objectsToSort = new Queue<IDrawable>();

            foreach (IDrawable drawable in drawables)
            {
#if false
                if (!(drawable is WorldObject))
                    continue;

                WorldObject wObj = (WorldObject)drawable;

                if (!wObj.IsMoving())
                    continue;
#endif
                objectsToSort.Enqueue(drawable);
            }

            while (objectsToSort.Count > 0)
            {
                IDrawable drawable = objectsToSort.Dequeue();
                if (!drawables.Remove(drawable))
                    continue;

                int count = 0;
                for (; count < drawables.Count; ++count)
                {
                    if (CompareObjectsZ(drawable, drawables[count]) <= 0)
                        break;
                }

                if (drawable is AnimationMapEffect)
                {

                }

                drawables.Insert(count, drawable);
            }
        }
        // Inclure gestion de Z
        int CompareObjectsZ(IDrawable obj1, IDrawable obj2)
        {
            Int32 dif = (Int32)(BaseDrawable.GetComparisonPointYByType(obj1, obj1.ComparisonPointYType) - BaseDrawable.GetComparisonPointYByType(obj2, obj2.ComparisonPointYType));

            if (dif == 0)
                dif = (Int32)(obj1.Position.X - obj2.Position.Y); // just a trick to avoid scintillment

            return dif < 0 ? -1 : 1;
        }

        #endregion

        #region Objects

        public List<WorldObject> Objects { get; private set; }

        protected List<WorldElement> Elements { get; private set; }

        protected List<Player> Players { get; private set; }

        protected List<NPC> NPCs { get; private set; }

        public virtual void AddObject(WorldObject wObj)
        {
            this.Objects.Add(wObj);

            if (wObj is WorldElement)
                this.Elements.Add((WorldElement)wObj);

            else if (wObj is Player)
                this.Players.Add((Player)wObj);

            else if (wObj is NPC)
                this.NPCs.Add((NPC)wObj);

            UpdateObjectEvents(wObj);
        }

        public virtual void RemoveObject(WorldObject wObj)
        {
            Ground.RemoveObjectBoundingBoxes(wObj);

            this.Objects.Remove(wObj);

            if (wObj is WorldElement)
                this.Elements.Remove((WorldElement)wObj);

            else if (wObj is Player)
                this.Players.Remove((Player)wObj);

            else if (wObj is NPC)
                this.NPCs.Remove((NPC)wObj);

            RemoveObjectEvents(wObj);
        }

        public List<WorldObject> GetObjects()
        {
            return Objects;
        }

        public IEnumerator<WorldObject> GetEnumerator()
        {
            return this.Objects.GetEnumerator();
        }

        public Int32 GetObjectCount()
        {
            return Objects.Count;
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

        #region Moves

        const float DECOMPOSITION_LIMIT = .1F;
        const Int32 DECOMPOSITION_COUNT = 3;

        public void DisableAllMoves()
        {
            IEnumerator<WorldObject> objectsEnum = this.Objects.GetEnumerator();
            while (objectsEnum.MoveNext())
            {
                objectsEnum.Current.DisableDirection(Direction.N);
                objectsEnum.Current.DisableDirection(Direction.E);
                objectsEnum.Current.DisableDirection(Direction.S);
                objectsEnum.Current.DisableDirection(Direction.O);
            }
        }

        public List<WorldObject> GetMovingObjects()
        {
            List<WorldObject> movingObjects = new List<WorldObject>();

            IEnumerator<WorldObject> iEnum = this.Objects.GetEnumerator();
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
            Vector2 move = this.GetMove(wObj, dt);

            if (Math.Abs(move.X) < DECOMPOSITION_LIMIT &&
                Math.Abs(move.Y) < DECOMPOSITION_LIMIT)
            {
                wObj.Move(new Vector2(this.GetPhysicalMoveX(wObj, move.X), this.GetPhysicalMoveY(wObj, move.Y)));

                return;
            }

            float moveAverage = (Math.Abs(move.X) + Math.Abs(move.Y)) / 2F;
            Int32 decompositionMoveFactor = (Int32)(moveAverage >= 1F ? moveAverage : 1F / moveAverage);

            Int32 decompositionCount = (Int32)((float)DECOMPOSITION_COUNT / DECOMPOSITION_LIMIT) * decompositionMoveFactor;

            for (Int32 count = 0; count < decompositionCount; ++count)
            {
                Vector2 decomposedMove = move / (float)decompositionCount;

                wObj.Move(new Vector2(GetPhysicalMoveX(wObj, decomposedMove.X), GetPhysicalMoveY(wObj, decomposedMove.Y)));
            }
        }

        protected Vector2 GetMove(WorldObject wObj, Time dt)
        {
            Vector2 move = new Vector2();

            float velocity = wObj.Velocity;

            float d = (float)(velocity * dt.Value);
            float dd = (float)(velocity * dt.Value / Math.Sqrt(2));

            switch (wObj.Direction)
            {
                case Direction.N:   move = new Vector2(0F, -d);     break;
                case Direction.S:   move = new Vector2(0F, d);      break;
                case Direction.E:   move = new Vector2(d, 0F);      break;
                case Direction.O:   move = new Vector2(-d, 0F);     break;
                case Direction.NE:  move = new Vector2(dd, -dd);    break;
                case Direction.NO:  move = new Vector2(-dd, -dd);   break;
                case Direction.SE:  move = new Vector2(dd, dd);     break;
                case Direction.SO:  move = new Vector2(-dd, dd);    break;
            }

            return move;
        }

        public static Vector2 GetMove(Direction direction, float velocity, Time dt)
        {
            Vector2 move = new Vector2();

            float d = (float)(velocity * dt.Value);

            switch (direction)
            {
                case Direction.N: move = new Vector2(0F, -d); break;
                case Direction.S: move = new Vector2(0F, d); break;
                case Direction.E: move = new Vector2(d, 0F); break;
                case Direction.O: move = new Vector2(-d, 0F); break;
            }

            return move;
        }

        private float GetPhysicalMoveX(WorldObject wObj, float offset)
        {
            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2(offset, 0.0f));

                for (Int32 y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (Int32 x = tmp.Left; x < tmp.Right + 1; ++x)
                        if (Ground.GetBlock(x, y))
                            return 0.0f;
            }

            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2(offset, 0.0f));

                for (int y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (int x = tmp.Left; x < tmp.Right + 1; ++x)
                    {
                        IEnumerator<BBoundingBox> iEnum = Ground.GetBBoundingBoxesEnumerator(x, y, BB.Z);
                        while (iEnum.MoveNext())
                        {
                            if (BB.Z != iEnum.Current.Z)
                                continue;

                            if (BB.Holder.Equals(iEnum.Current.Holder))
                                continue;

                            if (BB.BoundingBoxTest(iEnum.Current, new Vector2(offset, 0F)))
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
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2(0.0f, offset));

                for (Int32 y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (Int32 x = tmp.Left; x < tmp.Right + 1; ++x)
                        if (Ground.GetBlock(x, y))
                            return 0.0f;
            }

            foreach (BBoundingBox BB in wObj.BBoundingBoxes)
            {
                BlazeraLib.IntRect tmp = BB.GetNextTRect(new Vector2(0.0f, offset));

                for (int y = tmp.Top; y < tmp.Bottom + 1; ++y)
                    for (int x = tmp.Left; x < tmp.Right + 1; ++x)
                    {
                        IEnumerator<BBoundingBox> iEnum = Ground.GetBBoundingBoxesEnumerator(x, y, BB.Z);
                        while (iEnum.MoveNext())
                        {
                            if (BB.Z != iEnum.Current.Z)
                                continue;

                            if (BB.Holder.Equals(iEnum.Current.Holder))
                                continue;

                            if (BB.BoundingBoxTest(iEnum.Current, new Vector2(0F, offset)))
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
                foreach (EBoundingBox sNPCBB in wObj.EventBoundingBoxes[eventBBType])
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

                            if (triggerBB.Holder.Equals(NPCBB.Holder))
                                continue;

                            if (triggerBB.Equals(NPCBBs))
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

        public Boolean ContainsPoint(Vector2 point)
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

        public Vector2 Center
        {
            get { return Dimension / 2F; }
        }

        public virtual Vector2 Dimension
        {
            get
            {
                return new Vector2(this.Width * GameDatas.TILE_SIZE, 
                                   this.Height * GameDatas.TILE_SIZE);
            }
        }

        public virtual int Width
        {
            get { return this.Ground.Width; }
        }

        public virtual int Height
        {
            get { return this.Ground.Height; }
        }

        public Vector2 Halfsize
        {
            get { return this.Dimension / 2F; }
        }

        #endregion

        #region Sound

        private String MusicType { get; set; }

        #endregion
    }
}
