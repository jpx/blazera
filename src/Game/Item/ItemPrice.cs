using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class ItemPrice
    {
        const double DEFAULT_VALUE = 1D;

        public MoneyType MoneyType { get; private set; }
        public double Value { get; private set; }

        public ItemPrice(double value = DEFAULT_VALUE)
        {
            Value = value;
        }

        public ItemPrice(ItemPrice copy)
        {
            MoneyType = copy.MoneyType;
            Value = copy.Value;
        }
    }
}
