using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public enum HAlignment
    {
        Center,
        Left,
        Right
    }

    public enum VAlignment
    {
        Center,
        Top,
        Bottom
    }

    public abstract class Box : Widget
    {
        // level handling
        public const UInt32 DEFAULT_ITEM_LEVEL = 0;
        const float DEFAULT_LEVEL_OFFSET = 20F;

        public const float DEFAULT_BORDER_WIDTH = 3F;

        private float LevelOffset { get; set; }

        protected Dictionary<Widget, UInt32> Levels { get; set; }

        private Boolean NoBackgroundMode { get; set; }

        public Box(Boolean noBackgroundMode = true, String name = null, Boolean backgroundNoBackgroundMode = true) :
            base()
        {
            IsBackgroundColorLinked = true;

            NoBackgroundMode = noBackgroundMode;

            if (!NoBackgroundMode)
                Background = new BoxBackground(new Vector2f(1F, 1F), name, backgroundNoBackgroundMode);

            Name = name;

            Items = new List<Widget>();

            Levels = new Dictionary<Widget, UInt32>();
            LevelOffset = DEFAULT_LEVEL_OFFSET;
        }

        public override void Draw(RenderTarget window)
        {
            if (!NoBackgroundMode &&
                (Dimension.X <= 1F || Dimension.Y <= 1F))
                return;

            base.Draw(window);
        }

        public override void Refresh()
        {
            UpdatePosition();
        }

        public void SetBackgroundAlphaFactor(double backgroundAlphaFactor)
        {
            if (!NoBackgroundMode)
                ((BoxBackground)Background).SetBackgroundAlphaFactor(backgroundAlphaFactor);
        }

        protected void UpdatePosition()
        {
            for (Int32 i = 0; i < Items.Count; ++i)
            {
                Items[i].Position = GetGlobalFromLocal(GetIPos(i));
            }
        }

        protected abstract Vector2f GetIPos(Int32 i);

        protected float GetLevelOffset(Int32 i)
        {
            return Levels[Items[i]] * LevelOffset;
        }

        protected float GetLevelOffset(Widget item)
        {
            return Levels[item] * LevelOffset;
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);
        }

        protected override Vector2f GetBasePosition()
        {
            if (NoBackgroundMode)
                return base.GetBasePosition();

            if (Name == null)
                return base.GetBasePosition() + new Vector2f(Border.GetBoxBorderWidth() * 2F, Border.GetBoxBorderWidth() * 2F);

            return base.GetBasePosition() + new Vector2f(Border.GetBoxBorderWidth() * 2F, ((BoxBackground)Background).TopBorderHeight + Border.GetBoxBorderWidth());
        }

        protected override Vector2f GetStructureDimension()
        {
            if (NoBackgroundMode)
                return base.GetStructureDimension();

            if (Name == null)
                return base.GetStructureDimension() + new Vector2f(Border.GetBoxBorderWidth() * 4F, Border.GetBoxBorderWidth() * 4F);

            return base.GetStructureDimension() + new Vector2f(Border.GetBoxBorderWidth() * 4F, ((BoxBackground)Background).TopBorderHeight + Border.GetBoxBorderWidth() * 3F);
        }

        public abstract float GetAlignment(Widget widget);

        public override Vector2f Dimension
        {
            set
            {
                base.Dimension = value;

                if (Dimension.X == 0F ||
                    Dimension.Y == 0F)
                    base.Dimension = new Vector2f(10F, 10F);
            }
        }
    }

    public class VBox : Box
    {
        public const float DEFAULT_Y_EXTREMITY_OFFSET = 0F;
        public const HAlignment DEFAULT_HALIGNMENT = HAlignment.Left;

        public float YExtremityOffset { get; set; }

        private Dictionary<Widget, HAlignment> HAlignments { get; set; }

        public VBox(Boolean noBackgroundMode = true, String name = null, float yExtremityOffset = DEFAULT_Y_EXTREMITY_OFFSET, Boolean backgroundNoBackgroundMode = true) :
            base(noBackgroundMode, name, backgroundNoBackgroundMode)
        {
            HAlignments = new Dictionary<Widget, HAlignment>();
            YExtremityOffset = yExtremityOffset;
        }

        public void AddItem(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, HAlignment hAlignement = DEFAULT_HALIGNMENT)
        {
            AddWidget(widget);

            Levels.Add(widget, level);

            HAlignments.Add(widget, hAlignement);
        }

        public void AddItem(List<Widget> widgets, UInt32 level = DEFAULT_ITEM_LEVEL, HAlignment hAlignement = DEFAULT_HALIGNMENT)
        {
            foreach (Widget widget in widgets)
                AddItem(widget, level, hAlignement);
        }

        public void AddItemFirst(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, HAlignment hAlignement = DEFAULT_HALIGNMENT)
        {
            AddFirst(widget);

            Levels.Add(widget, level);

            HAlignments.Add(widget, hAlignement);
        }

        public Boolean RemoveItem(Widget widget)
        {
            if (!RemoveWidget(widget))
                return false;
            return
                Levels.Remove(widget) &&
                HAlignments.Remove(widget);
        }

        public virtual void Clear()
        {
            Queue<Widget> toRemove = new Queue<Widget>();

            foreach (Widget widget in Items)
                toRemove.Enqueue(widget);

            while (toRemove.Count > 0)
                RemoveItem(toRemove.Dequeue());
        }

        protected override Vector2f GetIPos(Int32 i)
        {
            // Total height of items
            float totItemH = 0.0f;
            for (Int32 j = 0; j < Items.Count; ++j) totItemH += Items[j].BackgroundDimension.Y;
            // Total height of items until i-th
            float curItemH = 0.0f;
            for (Int32 j = 0; j < i; ++j) curItemH += Items[j].BackgroundDimension.Y;
            // Height of total space between items
            float totSpaceH = Dimension.Y - totItemH - YExtremityOffset * 2F;
            // Height of space between items
            float spaceH = totSpaceH / (Items.Count > 1 ? Items.Count - 1 : 1F);
            // Current internal YPosition
            float curInternalPos = curItemH + i * spaceH + YExtremityOffset;

            float xAlignment = GetAlignment(Items[i]) + GetLevelOffset(i);

            return new Vector2f(xAlignment,
                               curInternalPos);
        }

        public override float GetAlignment(Widget widget)
        {
            switch (HAlignments[widget])
            {
                case BlazeraLib.HAlignment.Center:
                    return Halfsize.X - widget.BackgroundHalfsize.X;

                case BlazeraLib.HAlignment.Left:
                    return 0F;

                case BlazeraLib.HAlignment.Right:
                    return Dimension.X - widget.BackgroundDimension.X;

                default:
                    return 0F;
            }
        }
    }

    public class HBox : Box
    {
        public const float DEFAULT_X_EXTREMITY_OFFSET = 0F;
        public const VAlignment DEFAULT_VALIGNMENT = VAlignment.Center;

        public float XExtremityOffset { get; set; }

        private Dictionary<Widget, VAlignment> VAlignments { get; set; }

        public HBox(Boolean noBackgroundMode = true, String name = null, float xExtremityOffset = DEFAULT_X_EXTREMITY_OFFSET, Boolean backgroundNoBackgroundMode = true) :
            base(noBackgroundMode, name, backgroundNoBackgroundMode)
        {
            VAlignments = new Dictionary<Widget, VAlignment>();
            XExtremityOffset = xExtremityOffset;
        }

        public void AddItem(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, VAlignment vAlignement = DEFAULT_VALIGNMENT)
        {
            base.AddWidget(widget);

            Levels.Add(widget, level);

            VAlignments.Add(widget, vAlignement);
        }

        public void AddItem(List<Widget> widgets, UInt32 level = DEFAULT_ITEM_LEVEL, VAlignment vAlignement = DEFAULT_VALIGNMENT)
        {
            foreach (Widget widget in widgets)
            {
                base.AddWidget(widget);

                Levels.Add(widget, level);

                VAlignments.Add(widget, vAlignement);
            }
        }

        public void AddItemFirst(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, VAlignment vAlignement = DEFAULT_VALIGNMENT)
        {
            base.AddFirst(widget);

            Levels.Add(widget, level);

            VAlignments.Add(widget, vAlignement);
        }

        public Boolean RemoveItem(Widget widget)
        {
            if (!RemoveWidget(widget))
                return false;
            return
                Levels.Remove(widget) &&
                VAlignments.Remove(widget);
        }

        public virtual void Clear()
        {
            Queue<Widget> toRemove = new Queue<Widget>();

            foreach (Widget widget in Items)
                toRemove.Enqueue(widget);

            while (toRemove.Count > 0)
                RemoveItem(toRemove.Dequeue());
        }

        protected override Vector2f GetIPos(Int32 i)
        {
            // Total width of items
            float totItemW = 0.0f;
            for (Int32 j = 0; j < Items.Count; ++j) totItemW += Items[j].BackgroundDimension.X;
            // Total width of items until i-th
            float curItemW = 0.0f;
            for (Int32 j = 0; j < i; ++j) curItemW += Items[j].BackgroundDimension.X;
            // Width of total space between items
            float totSpaceW = Dimension.X - totItemW - XExtremityOffset * 2F;
            // Width of space between items
            float spaceW = totSpaceW / (Items.Count > 1 ? Items.Count - 1 : 1F);
            // Current internal YPosition
            float curInternalPos = curItemW + i * spaceW + XExtremityOffset;

            float yAlignment = GetAlignment(Items[i]) + GetLevelOffset(i);

            return new Vector2f(curInternalPos,
                               yAlignment);
        }

        public override float GetAlignment(Widget widget)
        {
            switch (VAlignments[widget])
            {
                case BlazeraLib.VAlignment.Center:
                    return Halfsize.Y - widget.BackgroundHalfsize.Y;

                case BlazeraLib.VAlignment.Top:
                    return 0F;

                case BlazeraLib.VAlignment.Bottom:
                    return Dimension.Y - widget.BackgroundDimension.Y;

                default:
                    return 0F;
            }
        }
    }
}