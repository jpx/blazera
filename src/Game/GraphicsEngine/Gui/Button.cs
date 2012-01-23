using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Button : Widget
    {
        private enum EState
        {
            Normal,
            MouseOver
        }

        public enum EMode
        {
            Background,
            Label,
            LabelEffect,
            BackgroundLabel,
        }

        // BackgroundLabelMode only
        const float LABEL_OFFSET = 4F;

        // LabelEffectMode only
        static readonly Color MOUSEOVER_LABEL_COLOR = Color.Red;
        const float DEFAULT_LABEL_CLICKOFFSET = 0F;
        const Text.Styles MOUSEOVER_LABEL_STYLE = SFML.Graphics.Text.Styles.Regular;

        public const String EMPTY_LABEL = "None";

        private Dictionary<EState, Texture> BackgroundTextures { get; set; }

        private EState _state;
        private EState State
        {
            get { return _state; }
            set
            {
                _state = value;

                if (Mode == EMode.Background ||
                    Mode == EMode.BackgroundLabel)
                {
                    if (Background != null)
                    {
                        ((PictureBox)Background).Texture = BackgroundTextures[EState.Normal];
                        if (GetCurrentTexture() != null)
                            ((PictureBox)(Background)).Texture = GetCurrentTexture();
                    }
                }
                else if (Mode == EMode.LabelEffect)
                {
                    if (State == EState.MouseOver)
                    {
                        Label.Color = MOUSEOVER_LABEL_COLOR;
                        Label.Style = MOUSEOVER_LABEL_STYLE;
                    }
                    else
                    {
                        Label.Color = Label.DEFAULT_COLOR;
                        Label.Style = SFML.Graphics.Text.Styles.Regular;
                    }
                }

                if (IsSealed)
                    Seal();
            }
        }
        private EMode Mode { get; set; }

        public float ClickOffset { get; set; }

        protected const Boolean DEFAULT_SHORTCUT_MODE = false;

        Keyboard.Key ShortCutKey;
        protected Boolean ShortCutMode;

        public Button(Texture backgroundN, Texture backgroundO) :
            base()
        {
            Mode = EMode.Background;

            BackgroundTextures = new Dictionary<EState, Texture>()
            {
                { EState.Normal, backgroundN },
                { EState.MouseOver, backgroundO }
            };

            Background = new PictureBox(GetCurrentTexture());

            State = EState.Normal;
        }

        public Button(String label, EMode mode = EMode.BackgroundLabel, Boolean shortCutMode = DEFAULT_SHORTCUT_MODE) :
            base()
        {
            Mode = mode;

            if (Mode == EMode.BackgroundLabel)
            {
                BackgroundTextures = new Dictionary<EState, Texture>()
                {
                    { EState.Normal, Create.Texture("ButtonBackgroundN") },
                    { EState.MouseOver, Create.Texture("ButtonBackgroundO") }
                };

                Background = new PictureBox(GetCurrentTexture());
            }

            else if (Mode == EMode.LabelEffect)
            {
                Mode = EMode.LabelEffect;
                ClickOffset = DEFAULT_LABEL_CLICKOFFSET;
            }

            Label = new Label(label);
            AddWidget(Label);

            State = EState.Normal;

            ShortCutMode = shortCutMode && label != null;

            if (label != null && Label.Text.Length > 0)
                ShortCutKey = WindowEvents.KeyCodeFromString(Label.Text[0].ToString());
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
            {
                if (!BackgroundContainsMouse(ClickOffset))
                    State = EState.Normal;

                return base.OnEvent(evt);
            }

            switch (evt.Type)
            {
                case EventType.MouseMoved:

                    if (BackgroundContainsMouse(ClickOffset))
                    {
                        State = EState.MouseOver;

                        return true;
                    }
                    else
                        State = EState.Normal;

                    break;

                case EventType.MouseButtonReleased:

                    if (!BackgroundContainsMouse(ClickOffset))
                        break;

                    return CallClicked(new MouseButtonEventArgs(evt.MouseButton));

                case EventType.MouseButtonPressed:
                    if (evt.MouseButton.Button == Mouse.Button.Left)
                        if (BackgroundContainsMouse(ClickOffset))
                            return true;

                    break;

                case SFML.Window.EventType.KeyPressed:

                    if (!ShortCutMode)
                        break;

                    if (evt.Key.Code == ShortCutKey)
                    {
                        CallShortCut();

                        return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        protected void CallShortCut()
        {
            if (Clicked != null)
                Clicked(this, null);
        }

        public Boolean CallClicked(MouseButtonEventArgs e)
        {
            if (Clicked == null)
                return false;

            Clicked(this, e);

            return true;
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            if (openingInfo == null)
                openingInfo = new OpeningInfo(true);

            base.Open(openingInfo);
        }

        public override void Reset()
        {
            base.Reset();

            State = EState.Normal;
        }

        public override void Refresh()
        {
            if (Label != null)
            {
                Label.Dimension = Dimension;

                Label.Position = GetGlobalFromLocal(new Vector2f(0F, 0F));
            }

            if (Mode == EMode.Background)
            {
                foreach (EState state in Enum.GetValues(typeof(EState)))
                    if (BackgroundTextures[state] != null)
                    {
                        BackgroundTextures[state].Dimension = Dimension;
                        BackgroundTextures[state].Position = Position;
                    }
            }

            else if (Mode == EMode.BackgroundLabel)
            {
                foreach (EState state in Enum.GetValues(typeof(EState)))
                    if (BackgroundTextures[state] != null)
                    {
                        BackgroundTextures[state].Dimension = BackgroundDimension;
                        BackgroundTextures[state].Position = Position;
                    }
            }
        }

        public event ClickEventHandler Clicked;

        private Label _label;
        private Label Label
        {
            get { return _label; }
            set
            {
                _label = value;

                if (Label == null)
                    return;

                Dimension = Label.Dimension;

                AddWidget(Label);
            }
        }

        protected override Vector2f GetBasePosition()
        {
            if (Mode == EMode.BackgroundLabel)
                return
                    base.GetBasePosition() +
                    new Vector2f(
                        LABEL_OFFSET,
                        LABEL_OFFSET);

            return base.GetBasePosition();
        }

        protected override Vector2f GetStructureDimension()
        {
            if (Mode == EMode.BackgroundLabel)
                return
                    base.GetStructureDimension() +
                    new Vector2f(
                        LABEL_OFFSET * 2F,
                        LABEL_OFFSET * 2F);

            return base.GetStructureDimension();
        }

        public String Text
        {
            get { return Label.Text; }
            set
            {
                Label.Text = value;
                CallChanged(new TextChangeEventArgs(Text));
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Mode == EMode.Background ||
                    Mode == EMode.BackgroundLabel ||
                    Label == null)
                    return base.Dimension;

                return Label.BackgroundDimension;
            }
        }

        public void SetLabelSize(Label.ESize size)
        {
            Label.SetSize(size);
        }

        private Texture GetCurrentTexture()
        {
            if (BackgroundTextures == null)
                return null;

            return BackgroundTextures[State];
        }

        public void SetTextures(Texture backgroundN, Texture backgroundO)
        {
            BackgroundTextures[EState.Normal] = backgroundN;
            BackgroundTextures[EState.MouseOver] = backgroundO;

            ((PictureBox)Background).Texture = BackgroundTextures[EState.Normal];
            if (GetCurrentTexture() != null)
                ((PictureBox)Background).Texture = GetCurrentTexture();

            Refresh();
        }
    }

    public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);
}
