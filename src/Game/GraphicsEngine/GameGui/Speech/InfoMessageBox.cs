using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class InfoMessageBox : MessageBox
    {
        #region Constants

        const bool DEFAULT_INTERACTIVE_MODE = false;
        const double DEFAULT_MESSAGE_DELAY = 1D;

        #endregion Constants

        #region Members

        #endregion Members

        public InfoMessageBox(bool interactveMode = DEFAULT_INTERACTIVE_MODE, double messageDelay = DEFAULT_MESSAGE_DELAY)
            : base()
        {
            ActivateInteractiveMode(interactveMode, messageDelay);
        }

        protected override IShape GetBoxBackground(Vector2f dimension)
        {
            return new RoundedRectangleShape(dimension, 4F, 2F, new Color(64, 64, 255, 128), new Color(139, 69, 19), false, 2F, true);
        }
    }
}
