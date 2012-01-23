namespace BlazeraLib
{
    public class GameLabel : GameWidget
    {
        #region Members

        public Label InnerLabel { get; private set; }

        #endregion

        public GameLabel(string text = null, Label.ESize size = Label.DEFAULT_TEXT_SIZE)
            : base()
        {
            InnerLabel = new Label(text, size);
            AddWidget(InnerLabel);
        }
    }
}
