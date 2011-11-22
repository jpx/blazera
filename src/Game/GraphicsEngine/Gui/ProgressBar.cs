using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class ProgressBar : Widget
    {
        #region Event handlers

        public class ProgressValueChangeEventArgs : System.EventArgs
        {
            public double ProgressValue { get; private set; }
            public ProgressValueChangeEventArgs(double progressValue) : base() { ProgressValue = progressValue; }
        }
        public delegate void ProgressValueChangeEventHandler(ProgressBar sender, ProgressValueChangeEventArgs e);

        public class CompletionEventArgs : System.EventArgs { }
        public delegate void CompletionEventHandler(ProgressBar sender, CompletionEventArgs e);

        #endregion

        #region Members

        ProgressBarShape Bar;

        #endregion

        #region Events

        public event ProgressValueChangeEventHandler OnProgressValueChange;
        bool CallOnProgressValueChange(double progressValue) { if (OnProgressValueChange == null) return false; OnProgressValueChange(this, new ProgressValueChangeEventArgs(progressValue)); return true; }

        public event CompletionEventHandler OnCompletion;
        bool CallOnCompletion() { if (OnCompletion == null) return false; OnCompletion(this, new CompletionEventArgs()); return true; }

        #endregion

        public ProgressBar(Vector2f dimension) :
            base()
        {
            Bar = new ProgressBarShape(dimension);
        }

        public void SetProgressValue(double progressValue)
        {
            if (progressValue >= 100D)
            {
                progressValue = 100D;
                Bar.SetProgressValue(progressValue);
                CallOnCompletion();
                return;
            }
            if (progressValue < 0D)
                progressValue = 0D;

            Bar.SetProgressValue(progressValue);

            CallOnProgressValueChange(progressValue);
        }

        public void AddProgressValueOffset(double progressValueOffset)
        {
            SetProgressValue(Bar.ProgressValue + progressValueOffset);
        }

        public override void Draw(RenderWindow window)
        {
            if (!IsVisible)
                return;

            Bar.Draw(window);

            base.Draw(window);
        }

        public override void Refresh()
        {
            base.Refresh();

            Bar.Position = Position;
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Bar == null)
                    return base.Dimension;
                return Bar.Dimension;
            }
        }

        public override void Reset()
        {
            base.Reset();

            Bar.Reset();
        }
    }
}
