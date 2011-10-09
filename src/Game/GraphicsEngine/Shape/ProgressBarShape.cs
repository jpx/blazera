using SFML.Graphics;

namespace BlazeraLib
{
    public class ProgressBarShape : RectangleShape
    {
        #region Constants

        const float DEFAULT_OUTLINE_THICKNESS = 1F;
        const double DEFAULT_PROGRESS_VALUE = 0D;

        #endregion

        #region Members

        RectangleShape InnerBar;
        public double ProgressValue { get; private set; }

        #endregion

        public ProgressBarShape(Vector2 dimension) :
            base(dimension, Color.White, true, Color.Blue, DEFAULT_OUTLINE_THICKNESS, true, true, 4F)
        {
            Reset();
        }

        public void SetProgressValue(double progressValue)
        {
            ProgressValue = progressValue;
            
            InnerBar = new RectangleShape(new Vector2((float)(ProgressValue / 100D * GetInnerDimension().X), GetInnerDimension().Y), Color.Green, false, Color.Black);

            InnerBar.Position = Position + GetPositionOffset();
        }

        Vector2 GetInnerDimension()
        {
            return Dimension;
        }

        public override void Draw(RenderWindow window)
        {
            if (!IsVisible)
                return;

            base.Draw(window);

            InnerBar.Draw(window);
        }

        public override Vector2 Position
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
