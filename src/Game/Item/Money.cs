using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public enum MoneyType
    {
        Copper,
        Silver,
        Gold,
        Platinum,
        Diamond,
        Chrona
    }

    public class Money
    {
        const double BASE_VALUE = 1D;
        const double COPPER_BASE_VALUE = 1D;
        const double SILVER_BASE_VALUE = 100D * COPPER_BASE_VALUE;
        const double GOLD_BASE_VALUE = 100D * SILVER_BASE_VALUE;
        const double PLATINUM_BASE_VALUE = 100D * GOLD_BASE_VALUE;
        const double DIAMOND_BASE_VALUE = 1000D * PLATINUM_BASE_VALUE;
        const double CHRONA_BASE_VALUE = 1000000D * DIAMOND_BASE_VALUE;

        static double GetBaseValue(MoneyType moneyType)
        {
            switch (moneyType)
            {
                case MoneyType.Copper: return BASE_VALUE * COPPER_BASE_VALUE;
                case MoneyType.Silver: return BASE_VALUE * SILVER_BASE_VALUE;
                case MoneyType.Gold: return BASE_VALUE * GOLD_BASE_VALUE;
                case MoneyType.Platinum: return BASE_VALUE * PLATINUM_BASE_VALUE;
                case MoneyType.Diamond: return BASE_VALUE * DIAMOND_BASE_VALUE;
                case MoneyType.Chrona: return BASE_VALUE * CHRONA_BASE_VALUE;
                default: return BASE_VALUE;
            }
        }

        public static ItemPrice ConvertPriceTo(MoneyType moneyType, ItemPrice itemPrice)
        {
            return new ItemPrice(itemPrice.Value * (GetBaseValue(itemPrice.MoneyType) / GetBaseValue(moneyType)));
        }
    }
}
