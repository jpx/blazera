namespace BlazeraLib
{
    public class CombatMainMenu : BlazeraLib.Game.GraphicsEngine.GameGui.Combat.Menu
    {
        #region Members

        MenuItem MoveItem;
        MenuItem ActionItem;
        MenuItem ExploreItem;
        MenuItem PassItem;

        #endregion

        public CombatMainMenu(Combat combat) :
            base(combat)
        {
            MoveItem = new MenuItem("Move", ITEM_TEXT_SIZE);
            MoveItem.Validated += new ValidationEventHandler(MoveItem_Validated);
            AddItem(MoveItem);

            ActionItem = new MenuItem("Action", ITEM_TEXT_SIZE);
            AddItem(ActionItem);
            ActionItem.AttachMenu(Combat.ActionMenu);

            ExploreItem = new MenuItem("Explore", ITEM_TEXT_SIZE);
            ExploreItem.Validated += new ValidationEventHandler(ExploreItem_Validated);
            AddItem(ExploreItem);

            PassItem = new MenuItem("Pass", ITEM_TEXT_SIZE);
            PassItem.Validated += new ValidationEventHandler(PassItem_Validated);
            AddItem(PassItem);
        }

        void ExploreItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Combat.ChangeState(BlazeraLib.Combat.EState.ExploreCellSelection);
        }

        void MoveItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Combat.ChangeState(BlazeraLib.Combat.EState.MoveCellSelection);
        }

        void PassItem_Validated(MenuItem sender, ValidationEventArgs e)
        {
            Combat.ChangeState(BlazeraLib.Combat.EState.TurnOver);
        }
    }
}
