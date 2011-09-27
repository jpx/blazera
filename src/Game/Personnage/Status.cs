using System.Collections.Generic;

namespace BlazeraLib
{
    #region Event handlers

    public class StatusChangeEventArgs : System.EventArgs
    {
        public BaseStatistic BaseStatistic { get; private set; }
        public StatusChangeEventArgs(BaseStatistic baseStatistic) { BaseStatistic = baseStatistic; }
    }
    public delegate void StatusChangeEventHandler(Status sender, StatusChangeEventArgs e);

    #endregion

    public class Status
    {
        #region Members

        Dictionary<BaseCaracteristic, BaseStatistic> BaseStats;
        Dictionary<Caracteristic, Statistic> Stats;
        Dictionary<ElementalCaracteristic, ElementalStatistic> ElementalStats;

        List<StatusEffect> Effects;

        #endregion

        #region Events

        public event StatusChangeEventHandler OnChange;
        bool CallOnChange(BaseStatistic baseStatistic) { if (OnChange == null) return false; OnChange(this, new StatusChangeEventArgs(baseStatistic)); return true; }

        #endregion

        public Status()
        {
            BaseStats = new Dictionary<BaseCaracteristic, BaseStatistic>();
            foreach (BaseCaracteristic baseCaracteristic in System.Enum.GetValues(typeof(BaseCaracteristic)))
                BaseStats.Add(baseCaracteristic, new BaseStatistic(baseCaracteristic));

            Stats = new Dictionary<Caracteristic, Statistic>();
            foreach (Caracteristic caracteristic in System.Enum.GetValues(typeof(Caracteristic)))
                Stats.Add(caracteristic, new Statistic(caracteristic));

            ElementalStats = new Dictionary<ElementalCaracteristic, ElementalStatistic>();
            foreach (ElementalCaracteristic elementalCaracteristic in System.Enum.GetValues(typeof(ElementalCaracteristic)))
                ElementalStats.Add(elementalCaracteristic, new ElementalStatistic(elementalCaracteristic));

            Effects = new List<StatusEffect>();
        }

        public uint this[BaseCaracteristic baseCaracteristic, BaseStatistic.Attribute attribute = BaseStatistic.Attribute.Current]
        {
            get { return BaseStats[baseCaracteristic].GetAmount(attribute); }
            set { BaseStats[baseCaracteristic].SetAmount(attribute, value); CallOnChange(BaseStats[baseCaracteristic]); }
        }

        public uint this[Caracteristic caracteristic]
        {
            get { return Stats[caracteristic].GetAmount(); }
            set { Stats[caracteristic].SetAmount(value); }
        }

        public uint this[ElementalCaracteristic elementalCaracteristic, ElementalStatistic.Attribute attribute]
        {
            get { return ElementalStats[elementalCaracteristic].GetAmount(attribute); }
            set { ElementalStats[elementalCaracteristic].SetAmount(attribute, value); }
        }

        public void AddEffect(StatusEffect effect)
        {
            Effects.Add(effect);
        }

        public void RemoveEffect(StatusEffect effect)
        {
            Effects.Remove(effect);
        }
    }
}
