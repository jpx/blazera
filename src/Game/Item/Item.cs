using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public abstract class Item : BaseObject
    {
        const UInt32 DEFAULT_MAX_QUANTITY = 99;

        public Boolean Soldable { get; protected set; }
        public ItemPrice Price { get; protected set; }
        public UInt32 MaxQuantity { get; protected set; }

        public Item() :
            base()
        {
            MaxQuantity = DEFAULT_MAX_QUANTITY;
            Price = new ItemPrice();
        }

        public Item(Item copy) :
            base(copy)
        {
            Soldable = copy.Soldable;
            Price = new ItemPrice(copy.Price);
            MaxQuantity = copy.MaxQuantity;
        }
    }

    public class QuestItem : Item
    {
        public QuestItem() :
            base()
        {
            Soldable = false;
            MaxQuantity = 1;
        }

        public QuestItem(QuestItem copy) :
            base(copy)
        {
            Soldable = false;
            MaxQuantity = 1;
        }
    }

    public class ConsumableItem : Item
    {
        public ConsumableItem() :
            base()
        {
            
        }

        public ConsumableItem(ConsumableItem copy) :
            base(copy)
        {
            
        }
    }

    public enum EquipableItemState
    {
        Normal,
        Broken,
        Fury
    }

    public class EquipableItem : Item
    {
        const EquipableItemState DEFAULT_STATE = EquipableItemState.Normal;

        public EquipableItemState State { get; private set; }

        public EquipableItem() :
            base()
        {
            State = DEFAULT_STATE;
        }

        public EquipableItem(EquipableItem copy) :
            base(copy)
        {
            State = copy.State;
        }
    }
}