using System.Collections.Generic;

using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class MessageBox : GameWidget
    {
        #region EventHandlers

        public class EventArgs : System.EventArgs {}
        public delegate void EventHandler(MessageBox sender, EventArgs e);

        #endregion EventHandlers

        #region Events

        public event EventHandler OnLaunching;
        bool CallOnLaunching() { if (OnLaunching == null) return false; OnLaunching(this, new EventArgs()); return true; }
        public event EventHandler OnStopping;
        bool CallOnStopping() { if (OnStopping == null) return false; OnStopping(this, new EventArgs()); return true; }

        #endregion Events

        #region Constants

        const float MAX_WIDTH = 200F;
        const float MAX_HEIGHT = 150F;

        const float DEFAULT_PADDING = 5F;

        static readonly Color DEFAULT_TEXT_COLOR = Color.Black;
        const Label.ESize DEFAULT_TEXT_SIZE = Label.ESize.SpeechBubbleMedium;
        const Text.Styles DEFAULT_TEXT_STYLES = Text.Styles.Regular;

        const double LETTER_DELAY = .02D;
        const double LETTER_DELAY_SPEED_FACTOR = 4D;

        #endregion Constants

        #region Members

        float Padding;

        HAutoSizeBox MainBox;

        MessageBuilder MessageBuilder;
        List<string> Messages;
        Label CurrentMessageLabel;
        Vector2f CurrentMessageDimension;
        int CurrentMessage;
        Timer LetterTimer;
        int CurrentLetter;
        bool MessageIsCompleted;

        /// <summary>
        /// Specifies if the player controls the preocess of the message.
        /// </summary>
        bool InteractiveMode;
        EventTimer MessageTimer;

        IShape BoxBackground;

        bool IsPlaying;
        bool IsLooping;

        #endregion Members

        public MessageBox()
            : base()
        {
            Padding = DEFAULT_PADDING;

            MainBox = new HAutoSizeBox();
            MainBox.Position = GetGlobalFromLocal(new Vector2f());
            AddWidget(MainBox);

            Messages = new List<string>();
            MessageBuilder = new MessageBuilder(new MessageBuilder.CInfo(
                new Vector2f(MAX_WIDTH, MAX_HEIGHT),
                GameData.DEFAULT_FONT,
                Label.GetSizeFromESize(DEFAULT_TEXT_SIZE),
                DEFAULT_TEXT_STYLES));

            InteractiveMode = true;

            CurrentMessageLabel = new Label(null, DEFAULT_TEXT_COLOR, DEFAULT_TEXT_SIZE);
            CurrentMessageLabel.Style = DEFAULT_TEXT_STYLES;
            MainBox.AddItem(CurrentMessageLabel, 0, VAlignment.Top);
            LetterTimer = new Timer();

            IsPlaying = false;
            IsLooping = false;
            MessageIsCompleted = false;

            CurrentMessage = 0;
            CurrentLetter = 0;
        }

        void MessageTimer_Completed(EventTimer sender, EventTimer.EventTimerEventArgs e)
        {
            NextMessage();
        }

        public void AddMessage(string message)
        {
            Messages.AddRange(MessageBuilder.GetFormattedMessage(message));
        }

        protected void ActivateInteractiveMode(bool interactiveMode = true, double messageDelay = 0D)
        {
            InteractiveMode = interactiveMode;

            if (!InteractiveMode)
            {
                MessageTimer = new EventTimer(messageDelay);
                MessageTimer.Completed += new EventTimer.EventTimerEventHandler(MessageTimer_Completed);
            }
            else
                MessageTimer = null;
        }

        protected abstract IShape GetBoxBackground(Vector2f dimension);

        void NextMessage()
        {
            if (CurrentMessage >= Messages.Count)
            {
                if (IsLooping)
                {
                    ResetMessage(true);
                    return;
                }
                else
                    StopMessage();

                return;
            }

            BuildMessage();

            MessageIsCompleted = false;
        }

        void NextLetter()
        {
            if (MessageIsCompleted)
                return;

            if (CurrentLetter < GetCurrentMesssage().Length)
            {
                CurrentMessageLabel.Add(GetCurrentLetter());
                ++CurrentLetter;
                return;
            }

            CurrentLetter = 0;
            ++CurrentMessage;
            MessageIsCompleted = true;

            if (!InteractiveMode)
                MessageTimer.Start();                
        }

        string GetCurrentMesssage()
        {
            return Messages[CurrentMessage];
        }

        char GetCurrentLetter()
        {
            return GetCurrentMesssage()[CurrentLetter];
        }

        void BuildMessage()
        {
            CurrentMessageLabel.Text = GetCurrentMesssage();
            MainBox.Refresh();
            CurrentMessageDimension = MainBox.BackgroundDimension;
            CurrentMessageLabel.Text = string.Empty;

            BoxBackground = GetBoxBackground(CurrentMessageDimension + GetStructureDimension());
            BoxBackground.Position = Position;
            MainBox.Position = GetGlobalFromLocal(new Vector2f());

            LetterTimer.Reset();
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (!IsPlaying)
                return;

            double letterDelay = ((Inputs.IsGameInput(InputType.Action) && InteractiveMode) ? LETTER_DELAY / LETTER_DELAY_SPEED_FACTOR : LETTER_DELAY);
            uint delayCount = LetterTimer.GetDelayFactor(letterDelay);
            for (uint count = 0; count < delayCount; ++count)
                NextLetter();

            if (MessageTimer != null)
                MessageTimer.Update(true, false);
        }

        public void LaunchMessage()
        {
            if (IsPlaying)
                return;

            if (Messages.Count == 0)
                return;

            CallOnLaunching();

            Open();
            IsPlaying = true;
            NextMessage();
        }

        public void StopMessage(bool reset = true)
        {
            while (IsPlaying
                && !MessageIsCompleted)
                NextLetter();

            IsPlaying = false;
            Close();

            CallOnStopping();

            if (!reset)
                return;

            CurrentLetter = 0;
            CurrentMessage = 0;
        }

        public void Clear()
        {
            ResetMessage();

            MainBox.Clear();

            Messages.Clear();
        }

        /// <summary>
        /// Goes back to the first message and launches the speech again
        /// </summary>
        public void ResetMessage(bool launchMessage = false)
        {
            StopMessage(true);
            if (launchMessage)
                LaunchMessage();
        }

        public override bool OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    if (!InteractiveMode)
                        break;

                    if (!IsPlaying)
                        break;

                    if (!Inputs.IsGameInput(InputType.Action, evt, true))
                        break;

                    if (!MessageIsCompleted)
                        break;

                    NextMessage();

                    return true;
            }

            return base.OnEvent(evt);
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            if (BoxBackground != null)
                BoxBackground.Draw(window);

            base.Draw(window);
        }

        public override void Refresh()
        {
            base.Refresh();

            if (BoxBackground == null)
                return;

            BoxBackground.Position = Position;
        }

        public override Vector2f Dimension
        {
            get
            {
                if (MainBox == null)
                    return base.Dimension;

                return MainBox.BackgroundDimension;
            }
        }

        protected override Vector2f GetBasePosition()
        {
            return base.GetBasePosition() + new Vector2f(Padding, Padding) + (BoxBackground == null ? new Vector2f() : BoxBackground.GetBasePosition());
        }

        protected override Vector2f GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2f(Padding * 2F, Padding * 2F);
        }

        Vector2f GetMaxDimension()
        {
            return new Vector2f(MAX_WIDTH, MAX_HEIGHT) - GetStructureDimension();
        }
    }
}
