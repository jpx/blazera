using SFML.Window;

namespace BlazeraLib
{
    public class Door : DynamicWorldElement
    {
        #region Members

        Teleporter Teleporter;
        WarpPoint WarpPoint;

        IntRect Area;

        Door Binding;

        BBoundingBox BlockBB;

        #endregion Members

        public Door()
            : base()
        {
            OnStateChange += new StateEventHandler(Door_OnStateChange);
        }

        public Door(Door copy)
            : base(copy)
        {
            Area = new IntRect(copy.Area);

            OnStateChange += new StateEventHandler(Door_OnStateChange);
        }

        public override object Clone()
        {
            return new Door(this);
        }

        //!\\ TODO //!\\
        public void SetSettings(Texture skin, IntRect rect)
        {
           // SetSkin(skin);

            AddSkin("Closed", Create.Texture("Door_C"));
            AddSkin("Open", Create.Texture("Door_wood_O"));
            AddSkin("Locked", Create.Texture("Door_C"));

            Area = rect != null ? rect : new IntRect(0, 0, (int)Dimension.X, (int)Dimension.Y);
        }

        const bool ANTI_LOOP_CHECK_IS_ACTIVE = false;
        bool BindingContainsBindedWarpPoint = false;
        ObjectEvent AntiLoopCheckEvent = null;
        public void BindTo(Door door)
        {
            if (Binding != null || door == null)
                return;

            Binding = door;

            // teleporter associate to the door
            Teleporter = Create.Teleporter("Invisible");
            Teleporter.SetSetting(Binding.Map.Type, Binding.WarpPoint.Name, Area);
            Teleporter.SetMap(Map, Position.X, Position.Y, Z);

            OnMove += new MoveEventHandler(Door_OnMove);

            // block BB when the door is closed
            BlockBB = new BBoundingBox(this, 0, 42, 32, 46);
            AddBoundingBox(BlockBB);

            // sides block BB
            AddBoundingBox(new BBoundingBox(this, 0, 42, 2, 46));
            AddBoundingBox(new BBoundingBox(this, 30, 42, 32, 46));

            // switch open/close state event BB
            EBoundingBox doorBB = new EBoundingBox(this, EBoundingBoxType.Event, 0 - 5, 44, 32 + 5, 56);
            AddEventBoundingBox(doorBB, EventBoundingBoxType.Internal);
            ObjectEvent doorEvt = new ObjectEvent(ObjectEventType.Normal, true, InputType.Action);
            doorBB.AddEvent(doorEvt);

            doorEvt.AddAction(new DefaultAction((args) =>
            {
                if (IsLocked() && args.Player.DirectionHandler.IsFacing(this))
                    LaunchLockedMessage();

                if (IsLocked() || IsClosing() || IsOpening())
                    return;

                if (args.Player.DirectionHandler.IsFacing(this))
                {
                    if (IsOpen())
                        Close();
                    else
                        Open();
                }
            }));

            TrySetState("Open");

            // anti loop event
            if (ANTI_LOOP_CHECK_IS_ACTIVE)
                SetAntiLoop();
        }

        //!\\ TODO
        InfoMessageBox LockedMessage = null;
        void InitLockedMessage()
        {
            LockedMessage = new InfoMessageBox();
            LockedMessage.AddMessage("The door is locked.");
            Map.AddWidget(LockedMessage);
        }

        void LaunchLockedMessage()
        {
            LockedMessage.Position = DrawingPosition + new Vector2f(-20F, -40F);
            LockedMessage.LaunchMessage();
        }

        public override void UnsetMap()
        {
            Map.RemoveWidget(LockedMessage);

            base.UnsetMap();
        }

        void Door_OnMove(IDrawable sender, MoveEventArgs e)
        {
            Teleporter.Move(e.Move, e.ZOffset);
            WarpPoint.Point += e.Move;
            WarpPoint.Z += e.ZOffset;
        }

        void Door_OnStateChange(WorldObject sender, WorldObject.StateEventArgs e)
        {
            Log.Cl("Door is: " + e.State);

            if (Teleporter == null)
                return;

            BlockBB.Activate(!IsOpen());
            Teleporter.ActivateEvents(IsOpen());
        }

        void SetAntiLoop()
        {
            if (Binding.Teleporter == null)
            {
                // binded door is not itself binded
                return;
            }

            // if binded warp point contains player future position
            // we avoid to re-teleport the player
            Teleporter.TeleportationEvent += new DefaultAction((args) =>
            {
                Binding.Teleporter.TeleportationEvent.Activate(false);

                ObjectEvent lockEvent = new TemporaryEvent(1, ObjectEventType.Out);
                Binding.Teleporter.TeleportationBB.AddEvent(lockEvent);
                lockEvent += new DefaultAction((args2) =>
                {
                    Binding.Teleporter.TeleportationEvent.Activate(true);
                });

                AntiLoopCheckEvent = new TemporaryEvent(1, ObjectEventType.In);
                Binding.Teleporter.TeleportationBB.AddEvent(AntiLoopCheckEvent);
                AntiLoopCheckEvent += new DefaultAction((args2) =>
                {
                    BindingContainsBindedWarpPoint = true;
                });

                Binding.OnUpdate += new UpdateEventHandler(Binding_OnUpdate);
            });
        }

        void Binding_OnUpdate(WorldObject sender, WorldObject.UpdateEventArgs e)
        {
            Binding.OnUpdate -= new UpdateEventHandler(Binding_OnUpdate);

            if (BindingContainsBindedWarpPoint)
                return;

            Binding.Teleporter.TeleportationBB.RemoveEvent(AntiLoopCheckEvent);
            Binding.Teleporter.TeleportationEvent.Activate(true);
            BindingContainsBindedWarpPoint = false;
        }

        public override void SetMap(Map map, float x, float y, int z = BaseDrawable.DEFAULT_Z)
        {
            base.SetMap(map, x, y, z);

            // to do
            WarpPoint = new WarpPoint(Id, Position + new SFML.Window.Vector2f(10F, 35F), Direction.S, Z);
            map.AddWarpPoint(WarpPoint);

            InitLockedMessage();
        }

        bool IsOpen()
        {
            return GetLogicalState() == "Open";
        }

        bool IsLocked()
        {
            return State == "Locked";
        }

        bool IsOpening()
        {
            return State == "Opening";
        }

        bool IsClosing()
        {
            return State == "Closing";
        }

        void Close()
        {
            if (Map.BoundingBoxTest(BlockBB, true))
                return;

            if (Skin.ContainsState("Closing"))
            {
                GetSkin("Closing").OnStopping += new BlazeraLib.Skin.EventHandler(Door_OnStopping);
                TrySetState("Closing");
            }
            else
            {
                TrySetState("Closed");
            }
        }

        void Door_OnStopping(Skin sender, Skin.EventArgs e)
        {
            TrySetState("Closed");
        }

        void Open()
        {
            if (Skin.ContainsState("Opening"))
            {
                GetSkin("Opening").OnStopping += new BlazeraLib.Skin.EventHandler(Door_OnStopping2);
                TrySetState("Opening");
            }
            else
            {
                TrySetState("Open");
            }
        }

        void Door_OnStopping2(Skin sender, Skin.EventArgs e)
        {
            TrySetState("Open");
        }

        void Lock()
        {
            TrySetState("Locked");
        }

        void Unlock()
        {
            TrySetState("Closed");
        }

        protected override string GetLogicalState()
        {
            switch (State)
            {
                case "Opening":
                case "Closing":
                case "Locked":
                    return "Closed";
            }

            return base.GetLogicalState();
        }
    }
}
