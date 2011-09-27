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

                if (this.Mode == EMode.Background ||
                    this.Mode == EMode.BackgroundLabel)
                {
                    if (this.Background != null)
                    {
                        ((PictureBox)this.Background).Texture = this.BackgroundTextures[EState.Normal];
                        if (this.GetCurrentTexture() != null)
                            ((PictureBox)(this.Background)).Texture = this.GetCurrentTexture();
                    }
                }
                else if (this.Mode == EMode.LabelEffect)
                {
                    if (this.State == EState.MouseOver)
                    {
                        this.Label.Color = MOUSEOVER_LABEL_COLOR;
                        this.Label.Style = MOUSEOVER_LABEL_STYLE;
                    }
                    else
                    {
                        this.Label.Color = Label.DEFAULT_COLOR;
                        this.Label.Style = SFML.Graphics.Text.Styles.Regular;
                    }
                }

                if (IsSealed)
                    Seal();
            }
        }
        private EMode Mode { get; set; }

        public float ClickOffset { get; set; }

        protected const Boolean DEFAULT_SHORTCUT_MODE = false;

        KeyCode ShortCutKey;
        protected Boolean ShortCutMode;

        public Button(Texture backgroundN, Texture backgroundO) :
            base()
        {
            this.Mode = EMode.Background;

            this.BackgroundTextures = new Dictionary<EState, Texture>()
            {
                { EState.Normal, backgroundN },
                { EState.MouseOver, backgroundO }
            };

            this.Background = new PictureBox(this.GetCurrentTexture());

            this.State = EState.Normal;
        }

        public Button(String label, EMode mode = EMode.BackgroundLabel, Boolean shortCutMode = DEFAULT_SHORTCUT_MODE) :
            base()
        {
            this.Mode = mode;

            if (this.Mode == EMode.BackgroundLabel)
            {
                this.BackgroundTextures = new Dictionary<EState, Texture>()
                {
                    { EState.Normal, Create.Texture("ButtonBackgroundN") },
                    { EState.MouseOver, Create.Texture("ButtonBackgroundO") }
                };

                this.Background = new PictureBox(this.GetCurrentTexture());
            }

            else if (this.Mode == EMode.LabelEffect)
            {
                this.Mode = EMode.LabelEffect;
                this.ClickOffset = DEFAULT_LABEL_CLICKOFFSET;
            }

            this.Label = new Label(label);
            this.AddWidget(this.Label);

            this.State = EState.Normal;

            ShortCutMode = shortCutMode && label != null;

            if (label != null && Label.Text.Length > 0)
                ShortCutKey = WindowEvents.KeyCodeFromString(Label.Text[0].ToString());
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
            {
                if (!this.BackgroundContainsMouse(this.ClickOffset))
                    this.State = EState.Normal;

                return base.OnEvent(evt);
            }

            switch (evt.Type)
            {
                case EventType.MouseMoved:

                    if (this.BackgroundContainsMouse(this.ClickOffset))
                    {
                        this.State = EState.MouseOver;

                        return true;
                    }
                    else
                        this.State = EState.Normal;

                    break;

                case EventType.MouseButtonReleased:

                    if (!this.BackgroundContainsMouse(this.ClickOffset))
                        break;

                    return CallClicked(new MouseButtonEventArgs(evt.MouseButton));

                case EventType.MouseButtonPressed:
                    if (evt.MouseButton.Button == MouseButton.Left)
                        if (this.BackgroundContainsMouse(this.ClickOffset))
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
            if (this.Clicked != null)
                this.Clicked(this, null);
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

            this.State = EState.Normal;
        }

        public override void Refresh()
        {
            if (this.Label != null)
            {
                this.Label.Dimension = this.Dimension;

                this.Label.Position = this.GetGlobalFromLocal(new Vector2(0F, 0F));
            }

            if (this.Mode == EMode.Background)
            {
                foreach (EState state in Enum.GetValues(typeof(EState)))
                    if (this.BackgroundTextures[state] != null)
                    {
                        this.BackgroundTextures[state].Dimension = this.Dimension;
                        this.BackgroundTextures[state].Position = this.Position;
                    }
            }

            else if (this.Mode == EMode.BackgroundLabel)
            {
                foreach (EState state in Enum.GetValues(typeof(EState)))
                    if (this.BackgroundTextures[state] != null)
                    {
                        this.BackgroundTextures[state].Dimension = this.BackgroundDimension;
                        this.BackgroundTextures[state].Position = this.Position;
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

                if (this.Label == null)
                    return;

                this.Dimension = this.Label.Dimension;

                this.AddWidget(this.Label);
            }
        }

        protected override Vector2 GetBasePosition()
        {
            if (this.Mode == EMode.BackgroundLabel)
                return
                    base.GetBasePosition() +
                    new Vector2(
                        LABEL_OFFSET,
                        LABEL_OFFSET);

            return base.GetBasePosition();
        }

        protected override Vector2 GetStructureDimension()
        {
            if (this.Mode == EMode.BackgroundLabel)
                return
                    base.GetStructureDimension() +
                    new Vector2(
                        LABEL_OFFSET * 2F,
                        LABEL_OFFSET * 2F);

            return base.GetStructureDimension();
        }

        public String Text
        {
            get { return this.Label.Text; }
            set
            {
                this.Label.Text = value;
                this.CallChanged(new TextChangeEventArgs(this.Text));
            }
        }

        public override Vector2 Dimension
        {
            get
            {
                if (this.Mode == EMode.Background ||
                    this.Mode == EMode.BackgroundLabel ||
                    this.Label == null)
                    return base.Dimension;

                return this.Label.BackgroundDimension;
            }
        }

        public void SetLabelSize(Label.ESize size)
        {
            this.Label.SetSize(size);
        }

        private Texture GetCurrentTexture()
        {
            if (this.BackgroundTextures == null)
                return null;

            return this.BackgroundTextures[this.State];
        }

        public void SetTextures(Texture backgroundN, Texture backgroundO)
        {
            this.BackgroundTextures[EState.Normal] = backgroundN;
            this.BackgroundTextures[EState.MouseOver] = backgroundO;

            ((PictureBox)this.Background).Texture = this.BackgroundTextures[EState.Normal];
            if (this.GetCurrentTexture() != null)
                ((PictureBox)this.Background).Texture = this.GetCurrentTexture();

            this.Refresh();
        }
    }

    public delegate void ClickEventHandler(object sender, MouseButtonEventArgs e);
}
