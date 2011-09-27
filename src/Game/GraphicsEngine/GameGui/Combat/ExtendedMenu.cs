using BlazeraLib;

namespace BlazeraLib.Game.GraphicsEngine.GameGui.Combat
{
    public abstract class ExtendedMenu : BlazeraLib.ExtendedMenu
    {
        #region Constants

        protected const Alignment DEFAULT_ALIGNMENT = Alignment.Vertical;
        protected const float DEFAULT_ITEM_OFFSET = 5F;
        protected const float DEFAULT_MARGINS = 10F;
        protected const Label.ESize ITEM_TEXT_SIZE = Label.ESize.VLarge;
        protected const int DEFAULT_SIZE = 4;

        #endregion

        #region Members

        protected BlazeraLib.Combat Combat;

        #endregion

        public ExtendedMenu(BlazeraLib.Combat combat) :
            base(DEFAULT_ALIGNMENT, DEFAULT_SIZE, DEFAULT_ITEM_OFFSET, DEFAULT_MARGINS)
        {
            Combat = combat;
        }
    }
}
