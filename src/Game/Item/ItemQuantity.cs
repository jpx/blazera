using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class ItemQuantity
    {
        const UInt32 DEFAULT_VALUE = 1;

        public UInt32 Value { get; private set; }

        public ItemQuantity(UInt32 value = DEFAULT_VALUE)
        {
            Value = value;
        }

        public ItemQuantity(ItemQuantity copy)
        {
            Value = copy.Value;
        }

        public override String ToString()
        {
            return Value.ToString();
        }

        #region operators

        public static Boolean operator <(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return quantity1.Value < quantity2.Value;
        }

        public static Boolean operator >(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return quantity1.Value > quantity2.Value;
        }

        public static Boolean operator <=(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return quantity1.Value <= quantity2.Value;
        }

        public static Boolean operator >=(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return quantity1.Value >= quantity2.Value;
        }

        public static Boolean operator ==(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return quantity1.Value == quantity2.Value;
        }

        public static Boolean operator !=(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return quantity1.Value != quantity2.Value;
        }

        public static ItemQuantity operator +(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1.Value + quantity2.Value);
        }

        public static ItemQuantity operator -(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1.Value - quantity2.Value);
        }

        public static ItemQuantity operator *(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1.Value * quantity2.Value);
        }

        public static ItemQuantity operator /(ItemQuantity quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1.Value / quantity2.Value);
        }

        public static Boolean operator <(ItemQuantity quantity1, UInt32 quantity2)
        {
            return quantity1.Value < quantity2;
        }

        public static Boolean operator >(ItemQuantity quantity1, UInt32 quantity2)
        {
            return quantity1.Value > quantity2;
        }

        public static Boolean operator <=(ItemQuantity quantity1, UInt32 quantity2)
        {
            return quantity1.Value <= quantity2;
        }

        public static Boolean operator >=(ItemQuantity quantity1, UInt32 quantity2)
        {
            return quantity1.Value >= quantity2;
        }

        public static Boolean operator ==(ItemQuantity quantity1, UInt32 quantity2)
        {
            return quantity1.Value == quantity2;
        }

        public static Boolean operator !=(ItemQuantity quantity1, UInt32 quantity2)
        {
            return quantity1.Value != quantity2;
        }

        public static ItemQuantity operator +(ItemQuantity quantity1, UInt32 quantity2)
        {
            return new ItemQuantity(quantity1.Value + quantity2);
        }

        public static ItemQuantity operator -(ItemQuantity quantity1, UInt32 quantity2)
        {
            return new ItemQuantity(quantity1.Value - quantity2);
        }

        public static ItemQuantity operator *(ItemQuantity quantity1, UInt32 quantity2)
        {
            return new ItemQuantity(quantity1.Value * quantity2);
        }

        public static ItemQuantity operator /(ItemQuantity quantity1, UInt32 quantity2)
        {
            return new ItemQuantity(quantity1.Value / quantity2);
        }

        public static Boolean operator <(UInt32 quantity1, ItemQuantity quantity2)
        {
            return quantity1 < quantity2.Value;
        }

        public static Boolean operator >(UInt32 quantity1, ItemQuantity quantity2)
        {
            return quantity1 > quantity2.Value;
        }

        public static Boolean operator <=(UInt32 quantity1, ItemQuantity quantity2)
        {
            return quantity1 <= quantity2.Value;
        }

        public static Boolean operator >=(UInt32 quantity1, ItemQuantity quantity2)
        {
            return quantity1 >= quantity2.Value;
        }

        public static Boolean operator ==(UInt32 quantity1, ItemQuantity quantity2)
        {
            return quantity1 == quantity2.Value;
        }

        public static Boolean operator !=(UInt32 quantity1, ItemQuantity quantity2)
        {
            return quantity1 != quantity2.Value;
        }

        public static ItemQuantity operator +(UInt32 quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1 + quantity2.Value);
        }

        public static ItemQuantity operator -(UInt32 quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1 - quantity2.Value);
        }

        public static ItemQuantity operator *(UInt32 quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1 * quantity2.Value);
        }

        public static ItemQuantity operator /(UInt32 quantity1, ItemQuantity quantity2)
        {
            return new ItemQuantity(quantity1 / quantity2.Value);
        }

        #endregion
    }
}
