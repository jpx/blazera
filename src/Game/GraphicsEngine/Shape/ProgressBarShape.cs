using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class ProgressBarShape : RectangleShape
    {
        #region Constants

        const float DEFAULT_OUTLINE_THICKNESS = 2F;
        const double DEFAULT_PROGRESS_VALUE = 0D;

        #endregion

        #region Members

        RectangleShape InnerBar;
        public double ProgressValue { get; private set; }

        #endregion

        public ProgressBarShape(Vector2f dimension) :
            base(dimension, new Color(0, 0, 0, 128), true, Color.Black, DEFAULT_OUTLINE_THICKNESS, false, false, 4F)
        {
            Reset();
        }

        public void SetProgressValue(double progressValue)
        {
            ProgressValue = progressValue;
            
            InnerBar = new RectangleShape(
                new Vector2f((float)(ProgressValue / 100D * GetInnerDimension().X), GetInnerDimension().Y),
                new Color(0, 255, 255, 128),
                false,
                Color.Black, // outline color
                2,
                true);

            InnerBar.Position = Position + GetPositionOffset();
        }

        Vector2f GetInnerDimension()
        {
            return Dimension;
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            base.Draw(window);

            InnerBar.Draw(window);
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                if (InnerBar != null)
                    InnerBar.Position = Position + GetPositionOffset();
            }
        }

        public void Reset()
        {
            SetProgressValue(DEFAULT_PROGRESS_VALUE);
        }
    }
}
