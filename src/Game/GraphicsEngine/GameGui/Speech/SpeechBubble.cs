using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    /// <summary>
    /// Dialog bubble that can be displayed on the game gui
    /// </summary>
    public class SpeechBubble : GameWidget
    {
        #region Constants

        const float BUBBLE_MAX_WIDTH = 200F;
        const float BUBBLE_MAX_HEIGHT = 150F;
        const float DEFAULT_MARGINS = 5F;

        const float DEFAULT_SPEAKER_OFFSET = 4F;

        static readonly Color DEFAULT_TEXT_COLOR = Color.Black;
        const Label.ESize DEFAULT_TEXT_SIZE = Label.ESize.SpeechBubbleMedium;

        const double LETTER_DELAY = .02D;
        const double LETTER_DELAY_SPEED_FACTOR = 4D;
        
        const float DEFAULT_BUBBLE_TIP_POSITION = 50F;
        const float DEFAULT_BUBBLE_TIP_SIZE = 8F;
        const BorderType DEFAULT_BUBBLE_TIP_BORDERTYPE = BorderType.Bottom;

        #endregion

        #region Members

        float Margins;

        SpeechBubbleShape Bubble;

        VAutoSizeBox Box;

        List<List<String>> Messages;
        List<List<Label>> MessageLabels;
        Int32 CurrentMessage;
        Int32 CurrentBuiltMessage;

        Timer LetterTimer;
        Boolean MessageIsCompleted;
        Int32 CurrentLine;
        Int32 CurrentLetter;

        Personnage Speaker;
        List<Personnage> Listeners;

        Boolean ActionKeyPlayingMode;
        Boolean IsLooping;
        Boolean IsPlaying;

        #endregion

        public event ClosingEventHandler OnClosing;
        public event LaunchingEventHandler OnLaunching;

        /// <summary>
        /// Constructs a SpeechBubble that follows the given Personnage
        /// </summary>
        /// <param name="speaker">Personnage followed by the SpeechBubble</param>
        public SpeechBubble(Personnage speaker) :
            base()
        {
            Speaker = speaker;
            Listeners = new List<Personnage>();

            Margins = DEFAULT_MARGINS;

            Box = new VAutoSizeBox();
            Box.Position = GetGlobalFromLocal(new Vector2f());
            AddWidget(Box);

            Messages = new List<List<String>>();
            MessageLabels = new List<List<Label>>();
            CurrentBuiltMessage = 0;

            LetterTimer = new Timer();
            MessageIsCompleted = false;

            ActionKeyPlayingMode = true;
            IsLooping = false;
            IsPlaying = false;

            StopSpeech(true);
        }

        public SpeechBubble(SpeechHandler speechHandler) :
            this(speechHandler.Speaker)
        {
            speechHandler.Generate(this);
            Listeners = new List<Personnage>(speechHandler.Listeners);

            OnLaunching += new LaunchingEventHandler(SpeechBubble_OnLaunching);
            OnClosing += new ClosingEventHandler(SpeechBubble_OnClosing);
        }

        void SpeechBubble_OnClosing(SpeechBubble sender, ClosingEventArgs e)
        {
            Speaker.Stop();
            foreach (Personnage listener in Listeners)
                listener.Stop();
        }

        void SpeechBubble_OnLaunching(SpeechBubble sender, LaunchingEventArgs e)
        {
            Speaker.TrySetState(State.Active);
            if (Listeners.Count == 1)
                Speaker.DirectionInfo.SetFacing(Listeners[0]);
            foreach (Personnage listener in Listeners)
                listener.TrySetState(State.Active);
        }

        void NextMessage()
        {
            if (CurrentMessage >= MessageLabels.Count)
            {
                if (IsLooping)
                {
                    ResetSpeech(true);
                    return;
                }
                else
                    StopSpeech();

                if (OnClosing != null)
                    OnClosing(this, new ClosingEventArgs());

                return;
            }

            BuildBubble();

            MessageIsCompleted = false;
        }

        void NextLetter()
        {
            if (MessageIsCompleted)
                return;

            if (CurrentLetter < GetCurrentLine().Length)
            {
                GetCurrentLineLabel().Add(GetCurrentLine()[CurrentLetter++]);
            }
            else
            {
                CurrentLetter = 0;
                if (CurrentLine < GetCurrentMessageLabels().Count - 1)
                    ++CurrentLine;
                else
                {
                    ++CurrentMessage;
                    CurrentLine = 0;
                    MessageIsCompleted = true;

                    if (!ActionKeyPlayingMode)
                        NextMessage();
                }
            }
        }

        Label GetCurrentLineLabel()
        {
            return GetCurrentMessageLabels()[CurrentLine];
        }

        String GetCurrentLine()
        {
            return GetCurrentMessage()[CurrentLine];
        }

        Char GetCurrentLetter()
        {
            return GetCurrentLine()[CurrentLetter];
        }

        void BuildBubble()
        {
            Box.Clear();

            foreach (Label messageLabel in GetCurrentMessageLabels())
                Box.AddItem(messageLabel);

            Box.Refresh();

            if (Box.BackgroundDimension.Y > GetMaxDimension().Y)
            {
                ++CurrentBuiltMessage;

                Messages.Insert(CurrentMessage + 1, new List<String>());
                MessageLabels.Insert(CurrentMessage + 1, new List<Label>());
            }

            while (Box.BackgroundDimension.Y > GetMaxDimension().Y)
            {
                Label lastLine;
                if (Box.RemoveItem(lastLine = GetCurrentMessageLabels()[GetCurrentMessageLabels().Count - 1]))
                {
                    GetCurrentMessageLabels().Remove(lastLine);
                    Messages[CurrentMessage + 1].Insert(0, lastLine.Text);
                    MessageLabels[CurrentMessage + 1].Insert(0, lastLine);
                }

                Box.Refresh();
            }

            const float BUBBLE_MARGIN = 6F;

            Vector2f bubbleDimension = Dimension + GetStructureDimension();
            Vector2f bubblePosition =
                GetGuiPointFromMapPoint(Speaker.Position + new Vector2f(Speaker.Halfsize.X, 0F)) - new Vector2f(0F, bubbleDimension.Y);
            BorderType bubbleTipBorderType = DEFAULT_BUBBLE_TIP_BORDERTYPE;

            if (bubblePosition.Y - DEFAULT_BUBBLE_TIP_SIZE * bubbleDimension.X / 100F * SpeechBubbleShape.TIP_LENGTH_SCALE_FACTOR < BUBBLE_MARGIN)
                bubbleTipBorderType = BorderType.Top;

            float tipPosition = DEFAULT_BUBBLE_TIP_POSITION;
            if (bubblePosition.X - bubbleDimension.X * tipPosition / 100F < BUBBLE_MARGIN)
            {
                tipPosition += (bubblePosition.X - bubbleDimension.X * tipPosition / 100F - BUBBLE_MARGIN) / bubbleDimension.X * 100F;
            }

            if (bubblePosition.X + bubbleDimension.X * tipPosition / 100F >= GetRoot().Right - BUBBLE_MARGIN)
            {
                tipPosition += (bubblePosition.X + bubbleDimension.X * tipPosition / 100F - GetRoot().Right + BUBBLE_MARGIN) / bubbleDimension.X * 100F;
            }

            Bubble = new SpeechBubbleShape(bubbleDimension, 10F, 2F,
                new Color(219, 147, 112), new Color(139, 69, 19), true, bubbleTipBorderType, true, 2F, tipPosition);

            if (Speaker != null)
                SetTipPosition(GetGuiPointFromMapPoint(Speaker.Position + new Vector2f(Speaker.Halfsize.X, Bubble.TipBorderType == BorderType.Bottom ? 0F : Speaker.Dimension.Y)));

            foreach (Label messageLabel in GetCurrentMessageLabels())
                messageLabel.Text = "";

            LetterTimer.Reset();
        }

        /// <summary>
        /// Updates the object
        /// </summary>
        /// <param name="dt">Delay since last update</param>
        public override void Update(Time dt)
        {
            base.Update(dt);

            if (!IsPlaying)
                return;

            double letterDelay = Inputs.IsGameInput(InputType.Action) ? LETTER_DELAY / LETTER_DELAY_SPEED_FACTOR : LETTER_DELAY;
            UInt32 delayCount = LetterTimer.GetDelayFactor(letterDelay);
            for (UInt32 count = 0; count < delayCount; ++count)
                NextLetter();
        }

        Label GetNewMessageLabel(String message)
        {
            Label messageLabel = new Label(message, DEFAULT_TEXT_COLOR, DEFAULT_TEXT_SIZE);
            messageLabel.Style = Text.Styles.Regular;

            return messageLabel;
        }

        List<Label> GetCurrentMessageLabels()
        {
            return MessageLabels[CurrentMessage];
        }

        List<String> GetCurrentMessage()
        {
            return Messages[CurrentMessage];
        }

        void AddMessageLabel(Label messageLabel)
        {
            while (MessageLabels.Count <= CurrentBuiltMessage)
            {
                Messages.Add(new List<String>());
                MessageLabels.Add(new List<Label>());
            }

            Messages[CurrentBuiltMessage].Add(messageLabel.Text);
            MessageLabels[CurrentBuiltMessage].Add(messageLabel);
        }

        /// <summary>
        /// Adds a message that will be displayed
        /// </summary>
        /// <param name="message">Message to add</param>
        public void AddMessage(String message)
        {
            BuildMessage(message);
        }

        public void LaunchSpeech()
        {
            if (IsPlaying)
                return;

            if (OnLaunching != null)
                OnLaunching(this, new LaunchingEventArgs());

            Open();
            IsPlaying = true;
            NextMessage();
        }

        public void StopSpeech(Boolean reset = false)
        {
            while (IsPlaying
                && !MessageIsCompleted)
                NextLetter();

            IsPlaying = false;
            Close();

            if (!reset)
                return;

            CurrentLine = 0;
            CurrentLetter = 0;
            CurrentMessage = 0;
        }

        public void Clear()
        {
            ResetSpeech();

            Box.Clear();

            Messages.Clear();
            MessageLabels.Clear();
        }

        /// <summary>
        /// Goes back to the first message and launches the speech again
        /// </summary>
        public void ResetSpeech(Boolean launchSpeech = false)
        {
            StopSpeech(true);
            if (launchSpeech)
                LaunchSpeech();
        }

        void BuildMessage(String message)
        {
            String[] words = message.Split(' ');

            Int32 wordCount = 0;
            Int32 labelCount = 0;

            List<Label> messageLabels = new List<Label>();
            messageLabels.Add(GetNewMessageLabel(words[wordCount++]));

            while (wordCount < words.Length)
            {
                messageLabels[labelCount].Add(" " + words[wordCount++]);

                if (messageLabels[labelCount].Dimension.X < GetMaxDimension().X)
                    continue;

                messageLabels.Add(GetNewMessageLabel(messageLabels[labelCount].RemoveLastWord(' ')));
                ++labelCount;
            }

            foreach (Label messageLabel in messageLabels)
                AddMessageLabel(messageLabel);

            ++CurrentBuiltMessage;
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    if (!ActionKeyPlayingMode)
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

        public override void Draw(RenderWindow window)
        {
            if (Bubble != null)
                Bubble.Draw(window);

            base.Draw(window);
        }

        public override void Refresh()
        {
            base.Refresh();

            if (Bubble == null)
                return;

            if (Speaker != null)
                Position = GetGuiPointFromMapPoint(Speaker.Position + new Vector2f(Speaker.Halfsize.X, Bubble.TipBorderType == BorderType.Bottom ? 0F : Speaker.Dimension.Y)) -
                    (Bubble.GetTipExtremityPosition(true) + new Vector2f(0F, Bubble.TipBorderType == BorderType.Bottom ? DEFAULT_SPEAKER_OFFSET : -DEFAULT_SPEAKER_OFFSET));

            Bubble.SetPosition(Position, false);
        }

        void SetTipPosition(Vector2f point)
        {
            if (Bubble == null)
            {
                base.Position = point;

                return;
            }

            base.Position =
                point -
                (Bubble.GetTipExtremityPosition() + new Vector2f(0F, Bubble.TipBorderType == BorderType.Bottom ? DEFAULT_SPEAKER_OFFSET : -DEFAULT_SPEAKER_OFFSET));
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Box == null)
                    return base.Dimension;

                return Box.BackgroundDimension;
            }
        }

        protected override Vector2f GetBasePosition()
        {
            return base.GetBasePosition() + new Vector2f(Margins, Margins);
        }

        protected override Vector2f GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2f(Margins * 2F, Margins * 2F);
        }

        Vector2f GetMaxDimension()
        {
            return new Vector2f(BUBBLE_MAX_WIDTH, BUBBLE_MAX_HEIGHT) - GetStructureDimension();
        }
    }

    public delegate void ClosingEventHandler(SpeechBubble sender, ClosingEventArgs e);

    public class ClosingEventArgs : EventArgs
    {
        public ClosingEventArgs()
        {

        }
    }

    public delegate void LaunchingEventHandler(SpeechBubble sender, LaunchingEventArgs e);

    public class LaunchingEventArgs : EventArgs
    {
        public LaunchingEventArgs()
        {

        }
    }
}
