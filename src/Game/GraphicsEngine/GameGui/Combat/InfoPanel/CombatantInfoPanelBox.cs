namespace BlazeraLib
{
    public class CombatantInfoPanelBox : InfoPanelBox
    {
        #region Members

        Label NameLabel;
        Label HpLabel;
        Label SpLabel;
        Label MpLabel;

        BaseCombatant CurrentCombatant;

        #endregion

        public CombatantInfoPanelBox() :
            base("Combatant")
        {
            NameLabel = new Label();
            HpLabel = new Label();
            SpLabel = new Label();
            MpLabel = new Label();
        }

        public override void Build(InfoPanelBox.BuildInfo buildInfo)
        {
            base.Build(buildInfo);

            if (CurrentCombatant != null)
                CurrentCombatant.OnStatusChange -= new CombatantStatusChangeEventHandler(CurrentCombatant_OnStatusChange);

            CurrentCombatant = buildInfo.GetArg<BaseCombatant>("Combatant");
            CurrentCombatant.OnStatusChange += new CombatantStatusChangeEventHandler(CurrentCombatant_OnStatusChange);

            UpdateData();

            AddItem(0, NameLabel);
            AddItem(0, HpLabel);
            AddItem(0, SpLabel);
            AddItem(0, MpLabel);
        }

        void CurrentCombatant_OnStatusChange(BaseCombatant sender, CombatantStatusChangeEventArgs e)
        {
            UpdateData();
        }

        public override void UpdateData()
        {
            NameLabel.Text = CurrentCombatant.Name;
            HpLabel.Text = "Hp " + CurrentCombatant.Status[BaseCaracteristic.Hp, BaseStatistic.Attribute.Current] + " / " + CurrentCombatant.Status[BaseCaracteristic.Hp, BaseStatistic.Attribute.Max];
            SpLabel.Text = "Sp " + CurrentCombatant.Status[BaseCaracteristic.Sp, BaseStatistic.Attribute.Current] + " / " + CurrentCombatant.Status[BaseCaracteristic.Sp, BaseStatistic.Attribute.Max];
            MpLabel.Text = "Mp " + CurrentCombatant.Status[BaseCaracteristic.Mp, BaseStatistic.Attribute.Current] + " / " + CurrentCombatant.Status[BaseCaracteristic.Mp, BaseStatistic.Attribute.Max];
        }
    }
}
