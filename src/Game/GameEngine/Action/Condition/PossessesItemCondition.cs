using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class PossessesItemCondition : Condition
    {
        public String ItemName { get; private set; }
        public UInt32 Quantity { get; private set; }

        public PossessesItemCondition(String itemName, UInt32 quantity) :
            base(ConditionType.PossessesItem)
        {
            ItemName = itemName;
            Quantity = quantity;
        }

        public override Boolean IsValidated(ObjectEventArgs args)
        {
            return args.Player.Inventory.ContainsItem(ItemName, Quantity);
        }

        public override String ToScript()
        {
            return "";
        }
    }
}
