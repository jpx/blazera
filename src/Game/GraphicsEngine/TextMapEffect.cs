using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class TextMapEffect : MapEffect
    {
        #region Enums

        enum EState
        {
            Zooming,
            Dezooming,
            Disappearing
        }

        #endregion

        #region Constants

        const Label.ESize DEFAULT_LABEL_SIZE = Label.ESize.GameMenuVSmall;
        static readonly Font DEFAULT_FONT = new Font(GameData.DATAS_DEFAULT_PATH + "fonts/BlackCastleMF.ttf");

        const float DEFAULT_ZOOM_FACTOR = 1F;
        const float DEFAULT_TOP_OFFSET = 25F;
        const float DEFAULT_ALPHA = 255F;

        const float ZOOM_MAX_FACTOR = 2.5F;
        const float ZOOM_VELOCITY = 4F;
        const float ZOOM_Y_FACTOR = .4F;

        const float DISAPPEARING_ALPHA_VELOCITY = 300F;
        const float DISAPPEARING_DEFAULT_VELOCITY = 5F;
        const float DISAPPEARING_ACCELERATION = 40F;

        #endregion

        #region Members

        EState State;
        Label Text;

        float CurrentZoomingFactor;
        float CurrentYOffset;
        float CurrentVelocity;
        float CurrentAlpha;

        #endregion

        public TextMapEffect(string text, Color color) :
            base()
        {
            DrawOrder = BlazeraLib.DrawOrder.Over;

            State = EState.Zooming;

            Text = new Label(text, DEFAULT_LABEL_SIZE);
            Text.Font = DEFAULT_FONT;
            Text.Color = color;

            CurrentZoomingFactor = 1F;
            CurrentVelocity = DISAPPEARING_DEFAULT_VELOCITY;
            CurrentYOffset = DEFAULT_TOP_OFFSET + Halfsize.Y;
            CurrentAlpha = DEFAULT_ALPHA;
        }

        public TextMapEffect(TextMapEffect copy) :
            base(copy)
        {
            DrawOrder = copy.DrawOrder;

            State = copy.State;

            Text = new Label(copy.Text.Text);
            Text.Font = copy.Text.Font;
            Text.Color = copy.Text.Color;

            CurrentZoomingFactor = copy.CurrentZoomingFactor;
            CurrentVelocity = copy.CurrentVelocity;
            CurrentYOffset = copy.CurrentYOffset;
            CurrentAlpha = copy.CurrentAlpha;
        }

        public override object Clone()
        {
            return new TextMapEffect(this);
        }

        public override void Start()
        {
            base.Start();

            Position -= new Vector2f(0F, CurrentYOffset);
        }

        public override void Update(Time dt)
        {
            switch (State)
            {
                case EState.Zooming:

                    CurrentZoomingFactor += (float)dt.Value * ZOOM_VELOCITY * ZOOM_MAX_FACTOR;

                    if (CurrentZoomingFactor >= ZOOM_MAX_FACTOR)
                    {
                        CurrentZoomingFactor = ZOOM_MAX_FACTOR;
                        State = EState.Dezooming;
                    }

                    break;

                case EState.Dezooming:

                    CurrentZoomingFactor -= (float)dt.Value * ZOOM_VELOCITY * ZOOM_MAX_FACTOR;

                    if (CurrentZoomingFactor <= 1F)
                    {
                        CurrentZoomingFactor = 1F;
                        State = EState.Disappearing;
                    }

                    break;

                case EState.Disappearing:

                    CurrentYOffset += (float)dt.Value * CurrentVelocity;
                    CurrentVelocity += (float)dt.Value * DISAPPEARING_ACCELERATION;

                    float alphaOffset = (float)dt.Value * DISAPPEARING_ALPHA_VELOCITY;

                    if (alphaOffset >= CurrentAlpha)
                    {
                        CurrentAlpha = 0;
                        CallOnStopping();
                        break;
                    }

                    CurrentAlpha -= alphaOffset;

                    break;

            }

            Text.Scale = new Vector2f(CurrentZoomingFactor, 1F + CurrentZoomingFactor * ZOOM_Y_FACTOR);
            Text.Position = new Vector2f(BasePosition.X - Halfsize.X, BasePosition.Y - CurrentYOffset);
            Text.Refresh();
            Text.Color = new Color(Text.Color.R, Text.Color.G, Text.Color.B, (byte)CurrentAlpha);
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            Text.Draw(window);
        }

        protected override void SetBasePosition()
        {
            Center = BasePosition;
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                if (Text == null)
                    return;

                Text.Position = Position;
                Text.Refresh();
            }
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Text == null)
                    return base.Dimension;

                return Text.Dimension;
            }
        }
    }
}
