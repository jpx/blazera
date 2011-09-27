using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class Inventory
    {
        Dictionary<Item, ItemQuantity> Items;

        public Inventory()
        {
            Items = new Dictionary<Item, ItemQuantity>();
        }

        public Inventory(Inventory copy)
        {
            Items = new Dictionary<Item, ItemQuantity>(copy.Items);
        }

        public Boolean AddItem(Item item)
        {
            return AddItem(item, new ItemQuantity());
        }

        public Boolean AddItem(Item item, UInt32 quantity)
        {
            return AddItem(item, new ItemQuantity(quantity));
        }

        public Boolean AddItem(Item item, ItemQuantity quantity)
        {
            ItemQuantity validItemQuantity = GetValidQuantity(item, quantity);

            if (validItemQuantity == 0)
                return false;

            if (!Items.ContainsKey(item))
                Items.Add(item, validItemQuantity);
            else
                Items[item] += validItemQuantity;

            return true;
        }

        public Boolean RemoveItem(Item item, Boolean all = false)
        {
            return RemoveItem(item, all ? GetQuantity(item) : new ItemQuantity(1));
        }

        public Boolean RemoveItem(Item item, UInt32 quantity, Boolean forcing = false)
        {
            return RemoveItem(item, new ItemQuantity(quantity), forcing);
        }

        public Boolean RemoveItem(Item item, ItemQuantity quantity, Boolean forcing = false)
        {
            if (quantity == 0)
                return true;

            if (!ContainsItem(item, quantity))
            {
                if (!forcing)
                    return false;

                Items[item] = new ItemQuantity(0);

                return true;
            }

            Items[item] -= quantity;

            return true;
        }

        public Boolean ContainsItem(Item item)
        {
            return ContainsItem(item, new ItemQuantity());
        }

        public Boolean ContainsItem(Item item, UInt32 quantity)
        {
            return ContainsItem(item, new ItemQuantity(quantity));
        }

        public Boolean ContainsItem(Item item, ItemQuantity quantity)
        {
            if (!Items.ContainsKey(item))
                return quantity == 0;

            return Items[item] >= quantity;
        }

        public Boolean ContainsItem(String itemName, UInt32 quantity)
        {
            if (quantity == 0)
                return true;

            foreach (Item item in Items.Keys)
                if (item.Name == itemName)
                    return ContainsItem(item, quantity);

            return false;
        }

        ItemQuantity GetValidQuantity(Item item, ItemQuantity quantity)
        {
            if (IsFull(item, quantity))
                return item.MaxQuantity - GetQuantity(item);

            return quantity;
        }

        Boolean IsFull(Item item)
        {
            return IsFull(item, new ItemQuantity(0));
        }

        Boolean IsFull(Item item, ItemQuantity addQuantity)
        {
            return GetQuantity(item) + addQuantity > item.MaxQuantity;
        }

        public ItemQuantity GetQuantity(Item item)
        {
            if (!Items.ContainsKey(item))
                return new ItemQuantity(0);

            return Items[item];
        }
    }
}
