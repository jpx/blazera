using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class WorldItem : DynamicWorldElement
    {
        const Boolean DEFAULT_PICKABLE_STATE = true;

        Item Item;
        ItemQuantity ItemQuantity;
        Boolean Pickable;

        public WorldItem() :
            base()
        {
            DrawOrder = DrawOrder.WorldItem;
            Pickable = DEFAULT_PICKABLE_STATE;
        }

        public WorldItem(WorldItem copy) :
            base(copy)
        {
            DrawOrder = DrawOrder.WorldItem;
            Pickable = copy.Pickable;

            ItemQuantity = new ItemQuantity(copy.ItemQuantity);
            if (copy.Item == null)
                return;

            if (copy.Item is QuestItem)
                Item = new QuestItem((QuestItem)copy.Item);
            else if (copy.Item is ConsumableItem)
                Item = new ConsumableItem((ConsumableItem)copy.Item);
            else
                Item = new EquipableItem((EquipableItem)copy.Item);
        }

        public override object Clone()
        {
            return new WorldItem(this);
        }

        public void SetTakingZone(Item item, UInt32 quantity, Texture skin, IntRect rect, Boolean pickable = true, Boolean actionKeyMode = false)
        {
            Item = item;
            ItemQuantity = new ItemQuantity(quantity);
            SetSkin(skin);
            Pickable = pickable;

            // taking event BB
            EBoundingBox takingZoneBB = new EBoundingBox(this, EBoundingBoxType.Event, rect.Left, rect.Top, rect.Right, rect.Bottom);

            // taking event
            ObjectEvent takingEvent = new ObjectEvent(ObjectEventType.Normal, true);

            // taking event action
            takingEvent.AddAction(new TakeItemAction(Item, ItemQuantity.Value));

            takingZoneBB.AddEvent(takingEvent);

            AddEventBoundingBox(takingZoneBB, EventBoundingBoxType.Internal);
        }

        public void SetTakingZone(Item item, UInt32 quantity, Texture skin, Boolean pickable = true, Boolean actionKeyMode = false)
        {
            SetTakingZone(item, quantity, skin, new IntRect(0, 0, (Int32)skin.Dimension.X, (Int32)skin.Dimension.Y), pickable, actionKeyMode);
        }
    }
}
