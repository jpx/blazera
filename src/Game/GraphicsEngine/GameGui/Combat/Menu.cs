using BlazeraLib;

namespace BlazeraLib.Game.GraphicsEngine.GameGui.Combat
{
    public abstract class Menu : BlazeraLib.Menu
    {
        #region Constants

        protected const Alignment DEFAULT_ALIGNMENT = Alignment.Vertical;
        protected const float ITEM_OFFSET = 5F;
        protected const float MARGINS = 10F;
        protected const Label.ESize ITEM_TEXT_SIZE = Label.ESize.VLarge;

        #endregion

        #region Members

        protected BlazeraLib.Combat Combat;

        #endregion

        public Menu(BlazeraLib.Combat combat) :
            base(DEFAULT_ALIGNMENT, ITEM_OFFSET, MARGINS)
        {
            Combat = combat;
        }
    }
}
