using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class RadioButton : Widget
    {
        #region Item

        public class Item : LabeledWidget
        {
            public const Boolean DEFAULT_STATE = false;
            public const float RESIZE_FACTOR = .5F;

            private Texture TextureN { get; set; }
            private Texture TextureC { get; set; }

            private Button Box { get; set; }

            public Boolean IsChecked { get; private set; }

            Boolean IsActive;

            public event CheckEventHandler Checked;

            public Item(String label, LabeledWidget.EMode mode = LabeledWidget.DEFAULT_MODE, Boolean isChecked = DEFAULT_STATE, Boolean shortCutMode = DEFAULT_SHORTCUT_MODE) :
                base(label, mode, shortCutMode)
            {
                this.TextureN = Create.Texture("Gui_RadioButtonN");
                this.TextureN.Dimension *= RESIZE_FACTOR;
                this.TextureC = Create.Texture("Gui_RadioButtonC");
                this.TextureC.Dimension *= RESIZE_FACTOR;

                this.Box = new Button(this.TextureN, null);
                this.Box.Clicked += new ClickEventHandler(Button_Clicked);

                this.GetLabelWidget().Clicked += new ClickEventHandler(Button_Clicked);

                this.AddLabeledWidget(Box);

                SetIsChecked(isChecked);

                IsActive = true;
            }

            protected override void CallShortCut()
            {
                this.SetIsChecked(!this.IsChecked);
            }

            public Item(Texture picture, LabeledWidget.EMode mode = LabeledWidget.DEFAULT_MODE, Boolean isChecked = DEFAULT_STATE) :
                base(picture, mode)
            {
                this.TextureN = Create.Texture("Gui_RadioButtonN");
                this.TextureN.Dimension *= RESIZE_FACTOR;
                this.TextureC = Create.Texture("Gui_RadioButtonC");
                this.TextureC.Dimension *= RESIZE_FACTOR;

                this.Box = new Button(this.TextureN, null);
                this.Box.Clicked += new ClickEventHandler(Button_Clicked);

                this.GetLabelWidget().Clicked += new ClickEventHandler(Button_Clicked);

                this.AddLabeledWidget(Box);

                SetIsChecked(isChecked);

                IsActive = true;
            }

            void Button_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
            {
                this.SetIsChecked(!this.IsChecked);
            }

            public override void Reset()
            {
                this.SetIsChecked(false);
            }

            public void SetIsChecked(Boolean isChecked)
            {
                if (!IsActive)
                    return;

                this.IsChecked = isChecked;

                if (this.IsChecked)
                    this.Box.SetTextures(this.TextureC, null);
                else
                    this.Box.SetTextures(this.TextureN, null);

                if (this.Checked != null)
                    this.Checked(this, new CheckEventArgs(this.IsChecked));
            }

            public void Activate(Boolean isActive = true)
            {
                IsActive = isActive;

                if (IsActive)
                    SetIsChecked(false);
            }
        }

        #endregion

        const Alignment DEFAULT_ALIGNMENT = Alignment.Horizontal;
        const LabeledDownList.EMode DEFAULT_BUTTON_MODE = LabeledWidget.EMode.Left;

        Dictionary<Item, CheckEventHandler> RadioButtons;
        Item CurrentItem;

        Alignment Alignment;
        VAutoSizeBox VBox;
        HAutoSizeBox HBox;

        public RadioButton(Alignment alignment = DEFAULT_ALIGNMENT) :
            base()
        {
            RadioButtons = new Dictionary<Item, CheckEventHandler>();

            Alignment = alignment;

            if (Alignment == Alignment.Horizontal)
            {
                HBox = new HAutoSizeBox();
                AddWidget(HBox);
            }
            else
            {
                VBox = new VAutoSizeBox();
                AddWidget(VBox);
            }
        }

        public void AddButton(String label, CheckEventHandler onChecked = null, LabeledWidget.EMode mode = DEFAULT_BUTTON_MODE, Boolean shortcutMode = false, String name = null)
        {
            AddButton(new Item(label, mode, false, shortcutMode) { Name = name }, onChecked);
        }

        public void AddButton(Texture picture, CheckEventHandler onChecked = null, LabeledWidget.EMode mode = DEFAULT_BUTTON_MODE, String name = null)
        {
            AddButton(new Item(picture, mode, RadioButtons.Count == 0) { Name = name }, onChecked);
        }

        void item_Checked(object sender, CheckEventArgs e)
        {
            if (CurrentItem != (Item)sender)
                SetCurrentItem((Item)sender);
        }

        void AddButton(Item item, CheckEventHandler onChecked)
        {
            item.Checked += onChecked;
            item.Checked += new CheckEventHandler(item_Checked);

            RadioButtons.Add(item, onChecked);

            if (RadioButtons.Count == 1)
                SetCurrentItem(item, true);

            if (Alignment == Alignment.Horizontal)
                HBox.AddItem(item);
            else
                VBox.AddItem(item);
        }

        void SetCurrentItem(Item currentItem, Boolean reset = false)
        {
            if (CurrentItem != null)
                CurrentItem.Activate();

            CurrentItem = currentItem;

            if (reset)
                CurrentItem.SetIsChecked(true);
            CurrentItem.Activate(false);
        }

        public override SFML.Window.Vector2f Dimension
        {
            get
            {
                if (VBox == null && HBox == null)
                    return base.Dimension;

                if (VBox == null)
                    return HBox.BackgroundDimension;

                return VBox.BackgroundDimension;
            }
        }
    }
}
