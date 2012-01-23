using System.Collections.Generic;

namespace BlazeraLib
{
    /// <summary>
    /// Base caracteristic of a personnage constituting its status
    /// </summary>
    public enum BaseCaracteristic
    {
        /// <summary>
        /// Health point
        /// </summary>
        Hp,
        /// <summary>
        /// Spell point
        /// </summary>
        Sp,
        /// <summary>
        /// Move point
        /// </summary>
        Mp
    }

    /// <summary>
    /// Caracteristic of a personnage constituting its status
    /// </summary>
    public enum Caracteristic
    {
        Power,
        Resistance,
        Agility,
        Vitality,
        Intelligence,
    }

    /// <summary>
    /// Elemental caracteristic of a personnage contituting its status
    /// </summary>
    public enum ElementalCaracteristic
    {
        // x1
        Earth,
        Air,
        Fire,
        Water,
        // x2
        Nature, // Earth + Water
        Ice, // Air + Water
        Ligthning, // Earth + Air
        Light, // Air + Fire
        Darkness, // Earth + Fire
        Nil, // Fire + Water
        // x3
        Spectrum,// Earth + Air + Fire
        Mist,// Air + Fire + Water
        Sand,// Earth + Air + Water
        Lava,// Earth + Fire + Water
        // x4 ? (ulti)
        Divine,
        Atomic
    };

    public class BaseStatistic
    {
        #region Enums

        public enum Attribute
        {
            Current,
            Max
        }

        #endregion

        #region Members

        BaseCaracteristic Type;

        Dictionary<Attribute, StatisticAmount> Amounts;

        #endregion

        public BaseStatistic(BaseCaracteristic type)
        {
            Type = type;

            Amounts = new Dictionary<Attribute, StatisticAmount>();
            foreach (Attribute attribute in System.Enum.GetValues(typeof(Attribute)))
                Amounts.Add(attribute, new StatisticAmount());
        }

        public uint GetAmount(Attribute attribute)
        {
            return Amounts[attribute].Value;
        }

        public void SetAmount(Attribute attribute, uint amount)
        {
            if (attribute == Attribute.Max)
            {
                Amounts[Attribute.Current].Value = amount;
            }
            else
            {
                if (amount > Amounts[Attribute.Max].Value)
                    amount = Amounts[Attribute.Max].Value;
            }

            Amounts[attribute].Value = amount;
        }
    }

    public class Statistic
    {
        Caracteristic Type;

        StatisticAmount Amount;

        public Statistic(Caracteristic type)
        {
            Type = type;

            Amount = new StatisticAmount();
        }

        public uint GetAmount()
        {
            return Amount.Value;
        }

        public void SetAmount(uint amount)
        {
            Amount.Value = amount;
        }
    }

    public class ElementalStatistic
    {
        #region Enums

        public enum Attribute
        {
            Power,
            Resistance,
            Mastering
        }

        #endregion

        #region Members

        ElementalCaracteristic Type;

        Dictionary<Attribute, StatisticAmount> Amounts;

        #endregion

        public ElementalStatistic(ElementalCaracteristic type)
        {
            Type = type;

            Amounts = new Dictionary<Attribute, StatisticAmount>();
            foreach (Attribute attribute in System.Enum.GetValues(typeof(Attribute)))
                Amounts.Add(attribute, new StatisticAmount());
        }

        public uint GetAmount(Attribute attribute)
        {
            return Amounts[attribute].Value;
        }

        public void SetAmount(Attribute attribute, uint amount)
        {
            Amounts[attribute].Value = amount;
        }
    }
}
