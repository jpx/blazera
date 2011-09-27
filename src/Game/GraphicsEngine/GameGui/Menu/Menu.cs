using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class Menu : GameWidget
    {
        #region Constants

        protected const float CURSOR_VELOCITY = 100F;
        protected const float DEFAULT_MARGINS = 20F;

        protected const HAlignment DEFAULT_ITEM_HALIGNMENT = HAlignment.Center;

        protected const Alignment DEFAULT_ALIGNMENT = Alignment.Vertical;

        const bool SWITCHING_RELASED_MODE = false;
        const double SWITCHING_DEFAULT_DELAY = .1D;

        #endregion

        #region Members

        public Alignment Alignment { get; protected set; }

        protected RoundedRectangleShape BackgroundShape;
        VAutoSizeBox VMainBox;
        HAutoSizeBox HMainBox;
        protected List<MenuItem> MenuItems;
        protected Int32 CurrentItem;

        PictureBox Cursor;

        protected float ItemOffset;
        protected float Margins;

        public bool Closable { get; set; }

        #endregion

        public Menu(Alignment alignment = DEFAULT_ALIGNMENT, float itemOffset = DEFAULT_MARGINS, float margins = DEFAULT_MARGINS) :
            base()
        {
            Alignment = alignment;

            Margins = margins;
            ItemOffset = itemOffset;

            MenuItems = new List<MenuItem>();

            Cursor = new PictureBox(Create.Texture("MenuSl_Main"));
            AddWidget(Cursor);

            InitBox();

            Closable = true;
        }

        protected virtual void InitBox()
        {
            if (Alignment == BlazeraLib.Alignment.Vertical)
            {
                VMainBox = new VAutoSizeBox(true, null, ItemOffset);
                VMainBox.Position = GetGlobalFromLocal(new Vector2());
                AddWidget(VMainBox);
            }
            else
            {
                HMainBox = new HAutoSizeBox(true, null, ItemOffset);
                HMainBox.Position = GetGlobalFromLocal(new Vector2());
                AddWidget(HMainBox);
            }
        }

        void RefreshCursor()
        {
            if (MenuItems.Count == 0)
                Cursor.Close();
            else
                Cursor.Open();
        }

        public void ShowCursor(bool show = true)
        {
            if (show)
                Cursor.Open();
            else
                Cursor.Close();
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (MenuItems.Count == 0)
            {
                ShowCursor(false);
                return;
            }

            if (Alignment == Alignment.Vertical)
            {
                Cursor.Dimension += new Vector2(
                    (GetCurrentItem().BackgroundDimension.X - Cursor.BackgroundDimension.X) / CURSOR_VELOCITY * (float)dt.MS,
                    (GetCurrentItem().BackgroundDimension.Y - Cursor.BackgroundDimension.Y) / CURSOR_VELOCITY * (float)dt.MS);

                Cursor.Center += new Vector2(
                    GetCurrentItem().Center.X - Cursor.Center.X,
                    (GetCurrentItem().Center.Y - Cursor.Center.Y) / CURSOR_VELOCITY * (float)dt.MS);
            }
            else
            {
                Cursor.Dimension += new Vector2(
                    (GetCurrentItem().BackgroundDimension.X - Cursor.BackgroundDimension.X) / CURSOR_VELOCITY * (float)dt.MS,
                    (GetCurrentItem().BackgroundDimension.Y - Cursor.BackgroundDimension.Y) / CURSOR_VELOCITY * (float)dt.MS);

                Cursor.Center += new Vector2(
                    (GetCurrentItem().Center.X - Cursor.Center.X) / CURSOR_VELOCITY * (float)dt.MS,
                    GetCurrentItem().Center.Y - Cursor.Center.Y);
            }
        }

        public override void Draw(RenderWindow window)
        {
            BackgroundShape.Draw(window);

            base.Draw(window);
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    if (Alignment == BlazeraLib.Alignment.Vertical)
                    {
                        if (Inputs.IsGameInput(InputType.Down, evt, SWITCHING_RELASED_MODE, SWITCHING_DEFAULT_DELAY))
                        {
                            Down();
                            return true;
                        }

                        if (Inputs.IsGameInput(InputType.Up, evt, SWITCHING_RELASED_MODE, SWITCHING_DEFAULT_DELAY))
                        {
                            Up();
                            return true;
                        }
                    }
                    else
                    {
                        if (Inputs.IsGameInput(InputType.Right, evt, SWITCHING_RELASED_MODE, SWITCHING_DEFAULT_DELAY))
                        {
                            Down();
                            return true;
                        }

                        if (Inputs.IsGameInput(InputType.Left, evt, SWITCHING_RELASED_MODE, SWITCHING_DEFAULT_DELAY))
                        {
                            Up();
                            return true;
                        }
                    }

                    if (Inputs.IsGameInput(InputType.Action, evt))
                    {
                        GetCurrentItem().CallValidated();
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Back, evt) && Closable)
                    {
                        Close();
                        return true;
                    }

                    return true;
            }

            return base.OnEvent(evt);
        }

        public override void Refresh()
        {
            base.Refresh();

            if (BackgroundShape == null)
                return;

            BackgroundShape.SetPosition(Position);

            if (GetRoot() == null)
                return;

            if (Left < GetRoot().Left)
                Left = GetRoot().Left;
            if (Top < GetRoot().Top)
                Top = GetRoot().Top;
            if (BackgroundRight > GetRoot().BackgroundRight)
                BackgroundRight = GetRoot().BackgroundRight;
            if (BackgroundBottom > GetRoot().BackgroundBottom)
                BackgroundBottom = GetRoot().BackgroundBottom;
        }

        protected MenuItem GetCurrentItem()
        {
            if (MenuItems.Count == 0)
                return null;

            return MenuItems[CurrentItem];
        }

        public virtual void AddItem(MenuItem item, HAlignment alignment = DEFAULT_ITEM_HALIGNMENT)
        {
            MenuItems.Add(item);

            if (Alignment == BlazeraLib.Alignment.Vertical)
            {
                VMainBox.AddItem(item, 0, alignment);
                VMainBox.Refresh();
            }
            else
            {
                HMainBox.AddItem(item, 0, VAlignment.Center);
                HMainBox.Refresh();
            }

            BackgroundShape = new RoundedRectangleShape(Dimension, 20F, 3F, Color.Black, Color.Black, true);
            if (Alignment == BlazeraLib.Alignment.Vertical)
                VMainBox.Position = GetGlobalFromLocal(new Vector2());
            else
                HMainBox.Position = GetGlobalFromLocal(new Vector2());
            Refresh();

            GetCurrentItem().CallOnSelection();
        }

        protected virtual void Down()
        {
            GetCurrentItem().CallOnDeselection();

            CurrentItem = (CurrentItem + 1) % MenuItems.Count;

            GetCurrentItem().CallOnSelection();
        }

        protected virtual void Up()
        {
            GetCurrentItem().CallOnDeselection();

            CurrentItem = (CurrentItem + MenuItems.Count - 1) % MenuItems.Count;

            GetCurrentItem().CallOnSelection();
        }

        public override void Reset()
        {
            base.Reset();

            CurrentItem = 0;

            if (MenuItems.Count > 0)
                GetCurrentItem().CallOnSelection();
        }

        public virtual bool RemoveItem(MenuItem item)
        {
            if (!MenuItems.Remove(item))
                return false;

            if (Alignment == BlazeraLib.Alignment.Vertical)
                VMainBox.RemoveItem(item);
            else
                HMainBox.RemoveItem(item);

            BackgroundShape = new RoundedRectangleShape(Dimension, 20F, 3F, Color.Black, Color.Black, true);
            if (Alignment == BlazeraLib.Alignment.Vertical)
                VMainBox.Position = GetGlobalFromLocal(new Vector2());
            else
                HMainBox.Position = GetGlobalFromLocal(new Vector2());
            Refresh();

            if (GetCurrentItem() != null)
                GetCurrentItem().CallOnSelection();

            return true;
        }

        public virtual void Clear()
        {
            Queue<MenuItem> itemsToRemove = new Queue<MenuItem>();

            foreach (MenuItem item in MenuItems)
                itemsToRemove.Enqueue(item);

            while (itemsToRemove.Count > 0)
                RemoveItem(itemsToRemove.Dequeue());
        }

        public override Vector2 Dimension
        {
            get
            {
                if (VMainBox == null &&
                    HMainBox == null)
                    return base.Dimension + GetStructureDimension();

                if (HMainBox == null)
                    return VMainBox.BackgroundDimension + GetStructureDimension();

                return HMainBox.BackgroundDimension + GetStructureDimension();
            }
        }

        public override Vector2 BackgroundDimension
        {
            get
            {
                if (BackgroundShape == null)
                    return base.BackgroundDimension;

                return BackgroundShape.GetBackgroundDimension();
            }
        }

        protected override Vector2 GetBasePosition()
        {
            return base.GetBasePosition() + new Vector2(Margins, Margins) + (BackgroundShape == null ? new Vector2() : BackgroundShape.GetBasePosition());
        }

        protected override Vector2 GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2(Margins * 2F, Margins * 2F);
        }
    }
}
