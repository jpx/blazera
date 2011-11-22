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

            this.NoBackgroundMode = noBackgroundMode;

            if (!this.NoBackgroundMode)
                this.Background = new BoxBackground(new Vector2f(1F, 1F), name, backgroundNoBackgroundMode);

            this.Name = name;

            this.Items = new List<Widget>();

            this.Levels = new Dictionary<Widget, UInt32>();
            this.LevelOffset = DEFAULT_LEVEL_OFFSET;
        }

        public override void Draw(RenderWindow window)
        {
            if (!this.NoBackgroundMode &&
                (this.Dimension.X <= 1F || this.Dimension.Y <= 1F))
                return;

            base.Draw(window);
        }

        public override void Refresh()
        {
            this.UpdatePosition();
        }

        public void SetBackgroundAlphaFactor(double backgroundAlphaFactor)
        {
            if (!NoBackgroundMode)
                ((BoxBackground)Background).SetBackgroundAlphaFactor(backgroundAlphaFactor);
        }

        protected void UpdatePosition()
        {
            for (Int32 i = 0; i < this.Items.Count; ++i)
            {
                this.Items[i].Position = this.GetGlobalFromLocal(this.GetIPos(i));
            }
        }

        protected abstract Vector2f GetIPos(Int32 i);

        protected float GetLevelOffset(Int32 i)
        {
            return this.Levels[this.Items[i]] * this.LevelOffset;
        }

        protected float GetLevelOffset(Widget item)
        {
            return this.Levels[item] * this.LevelOffset;
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);
        }

        protected override Vector2f GetBasePosition()
        {
            if (this.NoBackgroundMode)
                return base.GetBasePosition();

            if (this.Name == null)
                return base.GetBasePosition() + new Vector2f(Border.GetBoxBorderWidth() * 2F, Border.GetBoxBorderWidth() * 2F);

            return base.GetBasePosition() + new Vector2f(Border.GetBoxBorderWidth() * 2F, ((BoxBackground)this.Background).TopBorderHeight + Border.GetBoxBorderWidth());
        }

        protected override Vector2f GetStructureDimension()
        {
            if (this.NoBackgroundMode)
                return base.GetStructureDimension();

            if (this.Name == null)
                return base.GetStructureDimension() + new Vector2f(Border.GetBoxBorderWidth() * 4F, Border.GetBoxBorderWidth() * 4F);

            return base.GetStructureDimension() + new Vector2f(Border.GetBoxBorderWidth() * 4F, ((BoxBackground)this.Background).TopBorderHeight + Border.GetBoxBorderWidth() * 3F);
        }

        public abstract float GetAlignment(Widget widget);

        public override Vector2f Dimension
        {
            /*get
            {
                return base.Dimension;
            }*/
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
            this.HAlignments = new Dictionary<Widget, HAlignment>();
            this.YExtremityOffset = yExtremityOffset;
        }

        public void AddItem(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, HAlignment hAlignement = DEFAULT_HALIGNMENT)
        {
            this.AddWidget(widget);

            this.Levels.Add(widget, level);

            this.HAlignments.Add(widget, hAlignement);
        }

        public void AddItem(List<Widget> widgets, UInt32 level = DEFAULT_ITEM_LEVEL, HAlignment hAlignement = DEFAULT_HALIGNMENT)
        {
            foreach (Widget widget in widgets)
                AddItem(widget, level, hAlignement);
        }

        public void AddItemFirst(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, HAlignment hAlignement = DEFAULT_HALIGNMENT)
        {
            this.AddFirst(widget);

            this.Levels.Add(widget, level);

            this.HAlignments.Add(widget, hAlignement);
        }

        public Boolean RemoveItem(Widget widget)
        {
            if (!this.RemoveWidget(widget))
                return false;
            return
                this.Levels.Remove(widget) &&
                this.HAlignments.Remove(widget);
        }

        public virtual void Clear()
        {
            Queue<Widget> toRemove = new Queue<Widget>();

            foreach (Widget widget in this.Items)
                toRemove.Enqueue(widget);

            while (toRemove.Count > 0)
                this.RemoveItem(toRemove.Dequeue());
        }

        protected override Vector2f GetIPos(Int32 i)
        {
            // Total height of items
            float totItemH = 0.0f;
            for (Int32 j = 0; j < this.Items.Count; ++j) totItemH += this.Items[j].BackgroundDimension.Y;
            // Total height of items until i-th
            float curItemH = 0.0f;
            for (Int32 j = 0; j < i; ++j) curItemH += this.Items[j].BackgroundDimension.Y;
            // Height of total space between items
            float totSpaceH = this.Dimension.Y - totItemH - this.YExtremityOffset * 2F;
            // Height of space between items
            float spaceH = totSpaceH / (this.Items.Count > 1 ? this.Items.Count - 1 : 1F);
            // Current internal YPosition
            float curInternalPos = curItemH + i * spaceH + this.YExtremityOffset;

            float xAlignment = this.GetAlignment(this.Items[i]) + this.GetLevelOffset(i);

            return new Vector2f(xAlignment,
                               curInternalPos);
        }

        public override float GetAlignment(Widget widget)
        {
            switch (this.HAlignments[widget])
            {
                case BlazeraLib.HAlignment.Center:
                    return this.Halfsize.X - widget.BackgroundHalfsize.X;

                case BlazeraLib.HAlignment.Left:
                    return 0F;

                case BlazeraLib.HAlignment.Right:
                    return this.Dimension.X - widget.BackgroundDimension.X;

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
            this.VAlignments = new Dictionary<Widget, VAlignment>();
            this.XExtremityOffset = xExtremityOffset;
        }

        public void AddItem(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, VAlignment vAlignement = DEFAULT_VALIGNMENT)
        {
            base.AddWidget(widget);

            this.Levels.Add(widget, level);

            this.VAlignments.Add(widget, vAlignement);
        }

        public void AddItem(List<Widget> widgets, UInt32 level = DEFAULT_ITEM_LEVEL, VAlignment vAlignement = DEFAULT_VALIGNMENT)
        {
            foreach (Widget widget in widgets)
            {
                base.AddWidget(widget);

                this.Levels.Add(widget, level);

                this.VAlignments.Add(widget, vAlignement);
            }
        }

        public void AddItemFirst(Widget widget, UInt32 level = DEFAULT_ITEM_LEVEL, VAlignment vAlignement = DEFAULT_VALIGNMENT)
        {
            base.AddFirst(widget);

            this.Levels.Add(widget, level);

            this.VAlignments.Add(widget, vAlignement);
        }

        public Boolean RemoveItem(Widget widget)
        {
            if (!this.RemoveWidget(widget))
                return false;
            return
                this.Levels.Remove(widget) &&
                this.VAlignments.Remove(widget);
        }

        public virtual void Clear()
        {
            Queue<Widget> toRemove = new Queue<Widget>();

            foreach (Widget widget in this.Items)
                toRemove.Enqueue(widget);

            while (toRemove.Count > 0)
                this.RemoveItem(toRemove.Dequeue());
        }

        protected override Vector2f GetIPos(Int32 i)
        {
            // Total width of items
            float totItemW = 0.0f;
            for (Int32 j = 0; j < this.Items.Count; ++j) totItemW += this.Items[j].BackgroundDimension.X;
            // Total width of items until i-th
            float curItemW = 0.0f;
            for (Int32 j = 0; j < i; ++j) curItemW += this.Items[j].BackgroundDimension.X;
            // Width of total space between items
            float totSpaceW = this.Dimension.X - totItemW - this.XExtremityOffset * 2F;
            // Width of space between items
            float spaceW = totSpaceW / (this.Items.Count > 1 ? this.Items.Count - 1 : 1F);
            // Current internal YPosition
            float curInternalPos = curItemW + i * spaceW + this.XExtremityOffset;

            float yAlignment = this.GetAlignment(this.Items[i]) + this.GetLevelOffset(i);

            return new Vector2f(curInternalPos,
                               yAlignment);
        }

        public override float GetAlignment(Widget widget)
        {
            switch (this.VAlignments[widget])
            {
                case BlazeraLib.VAlignment.Center:
                    return this.Halfsize.Y - widget.BackgroundHalfsize.Y;

                case BlazeraLib.VAlignment.Top:
                    return 0F;

                case BlazeraLib.VAlignment.Bottom:
                    return this.Dimension.Y - widget.BackgroundDimension.Y;

                default:
                    return 0F;
            }
        }
    }
}