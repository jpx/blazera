using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public abstract class Personnage : DynamicWorldObject
    {
        #region Classes

        class CMovingStateInfo : WorldObject.CMovingStateInfo
        {
            #region Constants

            const bool DEFAULT_RUNNING_STATE = false;
            const float ROTATING_VELOCITY_FACTOR = .75F;

            #endregion Constants

            #region Members

            public bool IsRunning { get; set; }

            bool IsRotating;

            #endregion

            public CMovingStateInfo(Personnage parent)
                : base(parent)
            {
                IsRunning = DEFAULT_RUNNING_STATE;

                IsRotating = false;
            }

            public override float GetVelocity()
            {
                return base.GetVelocity() * (IsRunning ? GameData.PERSONNAGE_RUN_VELOCITY_FACTOR : 1F)
                    * (IsRotating ? ROTATING_VELOCITY_FACTOR : 1F);
            }

            public override void Update(Time dt)
            {
                base.Update(dt);

                RefreshDirection();
            }

            /*
             * Direction handling
             */
            const float DIRECTION_CHANGE_TIME = 0.05f;

            private int UpDir(int dir)
            {
                return (dir + 1) % 8;
            }

            private int DownDir(int dir)
            {
                return (dir + 7) % 8;
            }

            private int posD(int d, int md)
            {
                int count = 0;

                while (d != md)
                {
                    d = UpDir(d);
                    count++;
                }

                return count;
            }

            private int negD(int d, int md)
            {
                int count = 0;

                while (d != md)
                {
                    d = DownDir(d);
                    count++;
                }

                return count;
            }

            private Boolean IsPos(int d, int md)
            {
                return posD(d, md) <= negD(d, md);
            }

            private Timer RotationCount = new Timer(true);

            protected override void RefreshDirection()
            {
                if (!IsMoving() || (Parent.IsActive() && !Parent.IsMoving()/* && IsRotating*/))
                {
                    if (Parent.IsMoving())
                        Parent.TrySetState("Inactive");
                    return;
                }

                Direction moveDir = Direction;

                if (IsEnabled(Direction.N) && IsEnabled(Direction.E))
                    moveDir = Direction.NE;
                else if (IsEnabled(Direction.N) && IsEnabled(Direction.O))
                    moveDir = Direction.NO;
                else if (IsEnabled(Direction.S) && IsEnabled(Direction.E))
                    moveDir = Direction.SE;
                else if (IsEnabled(Direction.S) && IsEnabled(Direction.O))
                    moveDir = Direction.SO;
                else if (IsEnabled(Direction.N))
                    moveDir = Direction.N;
                else if (IsEnabled(Direction.S))
                    moveDir = Direction.S;
                else if (IsEnabled(Direction.E))
                    moveDir = Direction.E;
                else if (IsEnabled(Direction.O))
                    moveDir = Direction.O;

                int md = (int)moveDir;
                int d = (int)Direction;

                // direction pas encore atteinte
                if (md != d)
                {
                    IsRotating = true;
                    Parent.TrySetState("Rotating");
                    if (RotationCount.IsDelayCompleted(DIRECTION_CHANGE_TIME))
                    {
                        // rotation + (sens aiguilles)
                        if (IsPos(d, md))
                        {
                            if (d < 7)
                            {
                                SetDirection(Direction + 1);
                            }
                            else
                            {
                                SetDirection(Direction.N);
                            }
                        }
                        // rotation -
                        else
                        {
                            if (d > 0)
                            {
                                SetDirection(Direction - 1);
                            }
                            else
                            {
                                SetDirection(Direction.NO);
                            }
                        }
                    }
                }
                // direction atteinte, en avant !
                else
                {
                    IsRotating = false;
                    if (!IsRunning)
                        Parent.TrySetState("Moving");
                    else
                        Parent.TrySetState("Running");

                    RotationCount.Reset();
                }
            }
        }

        #endregion Classes

        #region Constants

        /// <summary>
        /// Specifies that the personnage is moving while rotating.
        /// </summary>
        const bool SMOOTH_ROTATION = true;

        #endregion Constants

        #region Members

        public SpellPanoply SpellPanoply { get; private set; }

        #endregion

        public Personnage() :
            base()
        {
            SpeechHandler = new SpeechHandler(this);
            SetSpeech();
        }

        public Personnage(Personnage copy) :
            base(copy)
        {
            SpeechHandler = new SpeechHandler(this, copy.SpeechHandler);
            SetSpeech();
        }

        protected override void InitMovingStateInfo()
        {
            MovingStateInfo = new CMovingStateInfo(this);

            MovingStateInfo.OnDirectionChange += new WorldObject.CMovingStateInfo.DirectionEventHandler(MovingStateInfo_OnDirectionChange);
        }

        void MovingStateInfo_OnDirectionChange(WorldObject.CMovingStateInfo sender, DirectionEventArgs e)
        {
            IEnumerator<KeyValuePair<State<string>, Skin>> skinSetEnumerator = Skin.GetEnumerator();
            while (skinSetEnumerator.MoveNext())
                GetSkinSet(skinSetEnumerator.Current.Key).SetCurrentState(Direction.ToString());

            Skin.Start();
        }

        SkinSet GetSkinSet(string state)
        {
            return (SkinSet)GetSkin(state);
        }

        public void AddSkin(string state, Direction direction, Skin skin)
        {
            AddSkin(state, new SkinSet());
            GetSkinSet(state).AddSkin(direction.ToString(), skin);
        }

        public Skin GetSkin(string state, Direction direction)
        {
            return GetSkinSet(state).GetSkin(direction.ToString());
        }

        public void SetRunning(bool isRunning = true)
        {
            ((Personnage.CMovingStateInfo)MovingStateInfo).IsRunning = isRunning;
        }

        protected override string GetLogicalState()
        {
            switch (State)
            {
                case "Running":
                    return "Moving";

                case "Rotating":

                    if (!SMOOTH_ROTATION)
                        break;

                    return "Moving";
            }

            return base.GetLogicalState();
        }

        protected override void SkinToScript()
        {
            IEnumerator<KeyValuePair<State<string>, Skin>> skinSetEnum = Skin.GetEnumerator();
            while (skinSetEnum.MoveNext())
            {
                IEnumerator<KeyValuePair<State<string>, Skin>> skinEnum = GetSkinSet(skinSetEnum.Current.Key).GetEnumerator();
                while (skinEnum.MoveNext())
                {
                    Sw.WriteMethod("AddSkin", new string[] { ScriptWriter.GetStringOf(skinSetEnum.Current.Key), "Direction." + skinEnum.Current.Key, skinEnum.Current.Value.ToScriptString() });
                }
            }
        }

        #region DialogEvent

        static readonly IntRect TALK_EVENT_BOUNDINGBOX_RECT = new IntRect(-5, -10, 22, 35);

        public SpeechHandler SpeechHandler { get; private set; }

        void SetSpeech()
        {
            EBoundingBox talkEventBB = new EBoundingBox(this, EBoundingBoxType.Event, TALK_EVENT_BOUNDINGBOX_RECT.Left, TALK_EVENT_BOUNDINGBOX_RECT.Top,
                TALK_EVENT_BOUNDINGBOX_RECT.Right, TALK_EVENT_BOUNDINGBOX_RECT.Bottom);
            ObjectEvent talkEvent = new ObjectEvent(ObjectEventType.Normal, true, InputType.Action);
            TalkAction talkAction = new TalkAction();

            talkEvent.AddAction(talkAction);

            talkEventBB.AddEvent(talkEvent);

            AddEventBoundingBox(talkEventBB, EventBoundingBoxType.Internal);
        }

        public void AddMessage(string message)
        {
            SpeechHandler.AddMessage(message);
        }

        #endregion
    }
}