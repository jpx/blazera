using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class TakeItemAction : Action
    {
        public Item Item { get; private set; }
        public UInt32 Quantity { get; private set; }

        public TakeItemAction(Item item, UInt32 quantity) :
            base(ActionType.TakeItem)
        {
            Item = item;
            Quantity = quantity;
            InterruptsEvents = true;
        }

        public override Boolean Do(ObjectEventArgs args)
        {
            if (!base.Do(args))
                return false;

            if (!args.Player.Inventory.AddItem(Item, Quantity))
                return false;

            args.Map.RemoveObject(args.Source.Holder);

            return true;
        }
    }
}
