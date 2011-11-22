using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class ExtendedMenu : Menu
    {
        #region Constants

        const int DEFAULT_SIZE = 4;

        #endregion

        #region Members

        ExtendedVBox ExtendedMainVBox;
        ExtendedHBox ExtendedMainHBox;

        #endregion

        public ExtendedMenu(Alignment alignment = DEFAULT_ALIGNMENT, int size = DEFAULT_SIZE, float itemOffset = DEFAULT_MARGINS, float margins = DEFAULT_MARGINS) :
            base(alignment, itemOffset, margins)
        {
        }

        protected override void InitBox()
        {
            if (Alignment == BlazeraLib.Alignment.Vertical)
            {
                ExtendedMainVBox = new ExtendedVBox(DEFAULT_SIZE);
                ExtendedMainVBox.Position = GetGlobalFromLocal(new Vector2f());
                AddWidget(ExtendedMainVBox);
            }
            else
            {
                ExtendedMainHBox = new ExtendedHBox(DEFAULT_SIZE);
                ExtendedMainHBox.Position = GetGlobalFromLocal(new Vector2f());
                AddWidget(ExtendedMainHBox);
            }
        }

        public override void AddItem(MenuItem item, HAlignment alignment = DEFAULT_ITEM_HALIGNMENT)
        {
            MenuItems.Add(item);

            if (Alignment == BlazeraLib.Alignment.Vertical)
                ExtendedMainVBox.AddItem(item);
            else
                ExtendedMainHBox.AddItem(item);

            GetCurrentItem().CallOnSelection();
        }

        public override bool RemoveItem(MenuItem item)
        {
            if (!MenuItems.Remove(item))
                return false;

            if (Alignment == BlazeraLib.Alignment.Vertical)
                ExtendedMainVBox.RemoveItem(item);
            else
                ExtendedMainHBox.RemoveItem(item);

            if (GetCurrentItem() != null)
                GetCurrentItem().CallOnSelection();

            return true;
        }

        public override Vector2f Dimension
        {
            get
            {
                if (ExtendedMainVBox == null &&
                    ExtendedMainHBox == null)
                    return base.Dimension + GetStructureDimension();

                if (ExtendedMainHBox == null)
                    return ExtendedMainVBox.BackgroundDimension + GetStructureDimension();

                return ExtendedMainHBox.BackgroundDimension + GetStructureDimension();
            }
        }

        public override void Refresh()
        {
            RefreshBackground();

            base.Refresh();
        }

        protected override void Up()
        {
            if (CurrentItem == 0)
                return;

            GetCurrentItem().CallOnDeselection();

            --CurrentItem;

            AdjustBox();

            Refresh();

            GetCurrentItem().CallOnSelection();
        }

        protected override void Down()
        {
            if (CurrentItem >= MenuItems.Count - 1)
                return;

            GetCurrentItem().CallOnDeselection();

            ++CurrentItem;

            AdjustBox();

            Refresh();

            GetCurrentItem().CallOnSelection();
        }

        void AdjustBox()
        {
            if (ExtendedMainHBox == null)
                ExtendedMainVBox.SetCurrentPointer(CurrentItem == 0 ? 0 : CurrentItem / ExtendedMainVBox.Size * ExtendedMainVBox.Size);
            else
                ExtendedMainHBox.SetCurrentPointer(CurrentItem == 0 ? 0 : CurrentItem / ExtendedMainHBox.Size * ExtendedMainHBox.Size);
        }

        void RefreshBackground()
        {
            BackgroundShape = new RoundedRectangleShape(Dimension, 20F, 3F, Color.Black, Color.Black, true);
            if (Alignment == BlazeraLib.Alignment.Vertical)
                ExtendedMainVBox.Position = GetGlobalFromLocal(new Vector2f());
            else
                ExtendedMainHBox.Position = GetGlobalFromLocal(new Vector2f());
        }
    }
}
