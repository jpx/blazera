using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public enum State
    {
        Moving,
        Active,
        Inactive
    }

    public enum MovingState
    {
        Normal, // just for Personnage.Animations
        Walking,
        Running
    }

    public abstract class Personnage : DynamicWorldObject
    {
        #region Members

        public SpellPanoply SpellPanoply { get; private set; }

        #endregion

        public Personnage() :
            base()
        {
            this.Animations = new Dictionary<MovingState, Dictionary<Direction, Animation>>()
            {
                { MovingState.Normal, new Dictionary<Direction, Animation>() },
                { MovingState.Walking, new Dictionary<Direction, Animation>() },
                { MovingState.Running, new Dictionary<Direction, Animation>() }
            };
            
            this.UpdateStates();

            SpeechHandler = new SpeechHandler(this);
            SetSpeech();
        }

        public Personnage(Personnage copy) :
            base(copy)
        {
            this.Direction = copy.Direction;
            this.Velocity = copy.Velocity;
            this.Animations = new Dictionary<MovingState, Dictionary<Direction, Animation>>()
            {
                { MovingState.Normal, new Dictionary<Direction, Animation>() },
                { MovingState.Walking, new Dictionary<Direction, Animation>() },
                { MovingState.Running, new Dictionary<Direction, Animation>() }
            };

            foreach (KeyValuePair<MovingState, Dictionary<Direction, Animation>> anims in copy.Animations)
            {
                foreach (KeyValuePair<Direction, Animation> anim in anims.Value)
                {
                    this.Animations[anims.Key][anim.Key] = new Animation(copy.Animations[anims.Key][anim.Key]);
                }
            }
            
            this.UpdateStates();

            SpeechHandler = new SpeechHandler(this, copy.SpeechHandler);
            SetSpeech();
        }

        public override void ToScript()
        {
            base.ToScript();

            foreach (KeyValuePair<MovingState, Dictionary<Direction, Animation>> anims in Animations)
            {
                foreach (KeyValuePair<Direction, Animation> anim in anims.Value)
                {
                    Sw.WriteMethod("AddAnimation",
                                        new String[]
                                        {
                                            "Create:Animation(\"" + anim.Value.Type + "_" + anim.Key.ToString() + "_" + anims.Key.ToString()[0] + "\")",
                                            "State." + anims.Key.ToString(),
                                            "Direction." + anim.Key.ToString()
                                        } );
                }
            }
        }

        public override void Update(Time dt)
        {
            base.Update(dt);
            Dimension = CurrentAnimation.Dimension;
            if (IsMoving())
                CurrentAnimation.Play(true, false);
            else
                CurrentAnimation.Stop();
            CurrentAnimation.Position = Position;
            CurrentAnimation.Update(dt);
        }

        public override void Draw(RenderWindow window)
        {
            this.CurrentAnimation.Draw(window);
            
            base.Draw(window);
        }

        public void AddAnimation(Animation anim, MovingState state, Direction dir)
        {
            this.Animations[state][dir] = anim;
        }

        public Animation CurrentAnimation
        {
            get { return Animations[MovingState][Direction]; }
        }

        private Dictionary<MovingState, Dictionary<Direction, Animation>> Animations
        {
            get;
            set;
        }

        public Animation GetAnimation(MovingState state, Direction direction)
        {
            return Animations[state][direction];
        }

        public Color Color
        {
            get { return this.CurrentAnimation.Color; }
            set
            {
                foreach (State state in Enum.GetValues(typeof(State)))
                    foreach (Animation animation in Animations[MovingState].Values)
                        animation.Color = value;
            }
        }

        public override float Velocity
        {
            get { return MovingState == MovingState.Running ? GameDatas.PERSONNAGE_RUN_VELOCITY_FACTOR * base.Velocity : base.Velocity; }
            set { base.Velocity = value; }
        }

        #region Direction

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
                d = this.UpDir(d);
                count++;
            }

            return count;
        }

        private int negD(int d, int md)
        {
            int count = 0;

            while (d != md)
            {
                d = this.DownDir(d);
                count++;
            }

            return count;
        }

        private Boolean IsPos(int d, int md)
        {
            return this.posD(d, md) <= this.negD(d, md);
        }

        private Timer RotationCount = new Timer();

        protected override void UpdateStates()
        {
            if (this.DirectionStates[Direction.N] ||
                this.DirectionStates[Direction.S] ||
                this.DirectionStates[Direction.E] ||
                this.DirectionStates[Direction.O])
            {
                if (!TrySetState(State.Moving))
                    return;

                Direction moveDir = this.Direction;

                if (this.DirectionStates[Direction.N] && this.DirectionStates[Direction.E])
                    moveDir = Direction.NE;
                else if (this.DirectionStates[Direction.N] && this.DirectionStates[Direction.O])
                    moveDir = Direction.NO;
                else if (this.DirectionStates[Direction.S] && this.DirectionStates[Direction.E])
                    moveDir = Direction.SE;
                else if (this.DirectionStates[Direction.S] && this.DirectionStates[Direction.O])
                    moveDir = Direction.SO;
                else if (this.DirectionStates[Direction.N])
                    moveDir = Direction.N;
                else if (this.DirectionStates[Direction.S])
                    moveDir = Direction.S;
                else if (this.DirectionStates[Direction.E])
                    moveDir = Direction.E;
                else if (this.DirectionStates[Direction.O])
                    moveDir = Direction.O;

                int md = (int)moveDir;
                int d = (int)this.Direction;

                // direction pas encore atteinte
                if (md != d)
                {
                    if (RotationCount.GetElapsedTime().Value >= DIRECTION_CHANGE_TIME)
                    {
                        // rotation + (sens aiguilles)
                        if (this.IsPos(d, md))
                        {
                            if (d < 7)
                            {
                                this.Direction++;
                            }
                            else
                            {
                                this.Direction = Direction.N;
                            }
                        }
                        // rotation -
                        else
                        {
                            if (d > 0)
                            {
                                this.Direction--;
                            }
                            else
                            {
                                this.Direction = Direction.NO;
                            }
                        }
                        RotationCount.Reset();
                    }
                }
                // direction atteinte, en avant !
                else
                {
                    TrySetMovingStateFromState(State.Moving);

                    RotationCount.Reset();
                }
            }
            else
            {
                TrySetMovingStateFromState(State.Inactive);
            }
        }

        #endregion

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