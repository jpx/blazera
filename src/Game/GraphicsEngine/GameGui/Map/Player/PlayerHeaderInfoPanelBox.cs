namespace BlazeraLib
{
    public class PlayerHeaderInfoPanelBox : InfoPanelBox
    {
        #region Members

        Label NameLabel;

        Player Player;

        #endregion

        public PlayerHeaderInfoPanelBox() :
            base("Player")
        {
            NameLabel = new Label();
        }

        public override void Build(InfoPanelBox.BuildInfo buildInfo)
        {
            base.Build(buildInfo);

            Player = buildInfo.GetArg<Player>("Player");

            UpdateData();

            AddItem(0, NameLabel);
        }

        public override void UpdateData()
        {
            //!\\ TODO : replace .Name property by login
            NameLabel.Text = "Player !!";
        }
    }
}
