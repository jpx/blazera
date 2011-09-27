using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;

namespace BlazeraEditor
{
    public class TextureRectDrawer : WindowedWidget
    {
        #region Singleton

        private static TextureRectDrawer _instance;
        public static TextureRectDrawer Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new TextureRectDrawer();

                return _instance;
            }
            private set { _instance = value; }
        }

        #endregion

        const float BUTTON_RESIZE_FACTOR = .4F;

        const Int32 DEFAULT_SELECTOR_OFFSET = 8;

        #region widgets declaration
        HAutoSizeBox MainTextureBox = new HAutoSizeBox();

        VAutoSizeBox TextureBox = new VAutoSizeBox(false, "Texture");

        VAutoSizeBox TopButtonBox = new VAutoSizeBox(true, null, 0F);
        Button TopUpButton = new Button(Create.Texture("Gui_UpButtonN"), Create.Texture("Gui_UpButtonO"));
        Button TopDownButton = new Button(Create.Texture("Gui_DownButtonN"), Create.Texture("Gui_DownButtonO"));

        HAutoSizeBox ScreenMidleBox = new HAutoSizeBox();

        HAutoSizeBox LeftButtonBox = new HAutoSizeBox(true, null, 0F);
        Button LeftLeftButton = new Button(Create.Texture("Gui_LeftButtonN"), Create.Texture("Gui_LeftButtonO"));
        Button LeftRightButton = new Button(Create.Texture("Gui_RightButtonN"), Create.Texture("Gui_RightButtonO"));

        DisplayScreen DisplayScreen = new DisplayScreen(32F * 5F);
        RectangleSelector Selector;

        HAutoSizeBox RightButtonBox = new HAutoSizeBox(true, null, 0F);
        Button RightLeftButton = new Button(Create.Texture("Gui_LeftButtonN"), Create.Texture("Gui_LeftButtonO"));
        Button RightRightButton = new Button(Create.Texture("Gui_RightButtonN"), Create.Texture("Gui_RightButtonO"));

        VAutoSizeBox BottomButtonBox = new VAutoSizeBox(true, null, 0F);
        Button BottomUpButton = new Button(Create.Texture("Gui_UpButtonN"), Create.Texture("Gui_UpButtonO"));
        Button BottomDownButton = new Button(Create.Texture("Gui_DownButtonN"), Create.Texture("Gui_DownButtonO"));

        Label RectLabel = new Label("Rect");

        VAutoSizeBox ToolMainBox = new VAutoSizeBox(false, "Tools");
        
        VAutoSizeBox ToolBox = new VAutoSizeBox();

        UpDownBox MoveOffsetUpDownBox = new UpDownBox(0, 32, DEFAULT_SELECTOR_OFFSET, DEFAULT_SELECTOR_OFFSET, "Offset", LabeledWidget.EMode.Right, true);
        UpDownBox WidthUpDownBox = new UpDownBox(0, 128, DEFAULT_SELECTOR_OFFSET, 0, "Width", LabeledWidget.EMode.Right, true);
        UpDownBox HeightUpDownBox = new UpDownBox(0, 128, DEFAULT_SELECTOR_OFFSET, 0, "Height", LabeledWidget.EMode.Right, true);
        UpDownBox XUpDownBox = new UpDownBox(0, 128, DEFAULT_SELECTOR_OFFSET, 0, "X", LabeledWidget.EMode.Right, true);
        UpDownBox YUpDownBox = new UpDownBox(0, 128, DEFAULT_SELECTOR_OFFSET, 0, "Y", LabeledWidget.EMode.Right, true);
        UpDownBox ZoomUpDownBox = new UpDownBox(25, 100, 25, 100, "Zoom (%)", LabeledWidget.EMode.Right, true);

        CheckBox AdjustToTextureCheckBox = new CheckBox("Adjust to texture", LabeledWidget.EMode.Right);

        HAutoSizeBox AdjustButtonBox = new HAutoSizeBox();
        Button AdjustToTextureButton = new Button("Adjust");
        Button AutoAdjustButton = new Button("Auto adjust");

        HAutoSizeBox ButtonBox = new HAutoSizeBox();
        Button SaveButton = new Button("Save");
        Button CancelButton = new Button("Cancel");
        #endregion

        BlazeraLib.FloatRect CurrentRect;
        Texture CurrentTexture;
        float SelectorResizeValue = DEFAULT_SELECTOR_OFFSET;

        private TextureRectDrawer() :
            base("Rect drawer")
        {
            #region widgets init
            AddItem(MainTextureBox);

            MainTextureBox.AddItem(TextureBox);
            TextureBox.AddItem(TopButtonBox, 0, HAlignment.Center);
            TopUpButton.Dimension *= BUTTON_RESIZE_FACTOR;
            TopUpButton.Clicked += new ClickEventHandler(TopUpButton_Clicked);
            TopButtonBox.AddItem(TopUpButton);
            TopDownButton.Dimension *= BUTTON_RESIZE_FACTOR;
            TopDownButton.Clicked += new ClickEventHandler(TopDownButton_Clicked);
            TopButtonBox.AddItem(TopDownButton);

            TextureBox.AddItem(ScreenMidleBox, 0, HAlignment.Center);

            ScreenMidleBox.AddItem(LeftButtonBox);
            LeftLeftButton.Dimension *= BUTTON_RESIZE_FACTOR;
            LeftLeftButton.Clicked += new ClickEventHandler(LeftLeftButton_Clicked);
            LeftButtonBox.AddItem(LeftLeftButton);
            LeftRightButton.Dimension *= BUTTON_RESIZE_FACTOR;
            LeftRightButton.Clicked += new ClickEventHandler(LeftRightButton_Clicked);
            LeftButtonBox.AddItem(LeftRightButton);

            ScreenMidleBox.AddItem(DisplayScreen);
            Selector = new RectangleSelector(DisplayScreen, false, true, true);
            Selector.OnChange += new SelectionChangeEventHandler(Selector_OnChange);
            Selector.MoveOffset = DEFAULT_SELECTOR_OFFSET;

            ScreenMidleBox.AddItem(RightButtonBox);
            RightLeftButton.Dimension *= BUTTON_RESIZE_FACTOR;
            RightLeftButton.Clicked += new ClickEventHandler(RightLeftButton_Clicked);
            RightButtonBox.AddItem(RightLeftButton);
            RightRightButton.Dimension *= BUTTON_RESIZE_FACTOR;
            RightRightButton.Clicked += new ClickEventHandler(RightRightButton_Clicked);
            RightButtonBox.AddItem(RightRightButton);

            TextureBox.AddItem(BottomButtonBox, 0, HAlignment.Center);
            BottomUpButton.Dimension *= BUTTON_RESIZE_FACTOR;
            BottomUpButton.Clicked += new ClickEventHandler(BottomUpButton_Clicked);
            BottomButtonBox.AddItem(BottomUpButton);
            BottomDownButton.Dimension *= BUTTON_RESIZE_FACTOR;
            BottomDownButton.Clicked += new ClickEventHandler(BottomDownButton_Clicked);
            BottomButtonBox.AddItem(BottomDownButton);

            TextureBox.AddItem(RectLabel);

            MainTextureBox.AddItem(ToolMainBox, 0, VAlignment.Top);
            ToolMainBox.AddItem(ToolBox);
            MoveOffsetUpDownBox.ValueChanged += new ValueChangeEventHandler(MoveOffsetUpDownBox_ValueChanged);
            ToolBox.AddItem(MoveOffsetUpDownBox, 0, HAlignment.Right);
            ToolBox.AddItem(WidthUpDownBox, 0, HAlignment.Right);
            ToolBox.AddItem(HeightUpDownBox, 0, HAlignment.Right);
            ToolBox.AddItem(XUpDownBox, 0, HAlignment.Right);
            ToolBox.AddItem(YUpDownBox, 0, HAlignment.Right);
            ZoomUpDownBox.Seal();
            ToolBox.AddItem(ZoomUpDownBox, 0, HAlignment.Right);

            AdjustToTextureCheckBox.Checked += new CheckEventHandler(AdjustToTextureCheckBox_Checked);
            ToolMainBox.AddItem(AdjustToTextureCheckBox);

            ToolMainBox.AddItem(AdjustButtonBox);
            AdjustToTextureButton.Clicked += new ClickEventHandler(AdjustToTextureButton_Clicked);
            AdjustButtonBox.AddItem(AdjustToTextureButton);
            AutoAdjustButton.Clicked += new ClickEventHandler(AutoAdjustButton_Clicked);
            AdjustButtonBox.AddItem(AutoAdjustButton);

            AddItem(ButtonBox);
            SaveButton.Clicked += new ClickEventHandler(SaveButton_Clicked);
            ButtonBox.AddItem(SaveButton);
            CancelButton.Clicked += new ClickEventHandler(CancelButton_Clicked);
            ButtonBox.AddItem(CancelButton);
            #endregion
        }

        void AutoAdjustButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            AutoAdjustRect();
        }

        void RefreshUpDownBoxes(DisplayScreen.EMode mode)
        {
            if (mode == DisplayScreen.EMode.Scroll)
            {
                MoveOffsetUpDownBox.ChangeValues(0, 64);
                MoveOffsetUpDownBox.SetDefaultValue(16);
                MoveOffsetUpDownBox.SetScaleValue(16);
                WidthUpDownBox.ChangeValues(0, 256);
                WidthUpDownBox.SetScaleValue(16);
                HeightUpDownBox.ChangeValues(0, 256);
                HeightUpDownBox.SetScaleValue(16);
                XUpDownBox.ChangeValues(0, 512);
                YUpDownBox.ChangeValues(0, 512);
            }
            else
            {
                MoveOffsetUpDownBox.ChangeValues(0, 32);
                MoveOffsetUpDownBox.SetDefaultValue(8);
                MoveOffsetUpDownBox.SetScaleValue(8);
                WidthUpDownBox.ChangeValues(0, 128);
                WidthUpDownBox.SetScaleValue(8);
                HeightUpDownBox.ChangeValues(0, 128);
                HeightUpDownBox.SetScaleValue(8);
                XUpDownBox.ChangeValues(0, 128);
                YUpDownBox.ChangeValues(0, 128);
                SelectorResizeValue = 1;
                Selector.MoveOffset = 1;
            }
        }

        void CancelButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Close();
        }

        void SaveButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            CallValidated();
        }

        void AdjustToTextureCheckBox_Checked(object sender, CheckEventArgs e)
        {
            ToolBox.Seal(e.IsChecked);
        }

        void MoveOffsetUpDownBox_ValueChanged(UpDownBox sender, ValueChangeEventArgs e)
        {
            XUpDownBox.SetScaleValue(e.Value);
            YUpDownBox.SetScaleValue(e.Value);

            if (OpeningMode == "TextureCreator_Mode")
            {
                Selector.MoveOffset = e.Value;
                SelectorResizeValue = e.Value;
            }
        }

        void AdjustToTextureButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            AdjustRect();
        }

        void AdjustRect()
        {
            if (CurrentTexture == null)
                return;

            Vector2 position = DisplayScreen.GetLocalFromGlobal(CurrentTexture.Position);
            Vector2 dimension = CurrentTexture.Dimension;

            if (AdjustToTextureCheckBox.IsChecked)
            {
                Selector.SetRect(position, dimension);
                return;
            }

            position = DisplayScreen.GetLocalFromGlobalTexturePoint(position);

            Selector.SetRect(
                new Vector2(
                    XUpDownBox.GetCurrentValue(),
                    YUpDownBox.GetCurrentValue()) + position,
                new Vector2(
                    WidthUpDownBox.GetCurrentValue(),
                    HeightUpDownBox.GetCurrentValue()));
        }

        //!\\ TODO //!\\
        void AutoAdjustRect()
        {
            UInt32
                left = CurrentRect.Left < 0 ? 0 : (UInt32)CurrentRect.Left,
                top = CurrentRect.Top < 0 ? 0 : (UInt32)CurrentRect.Top,
                right = CurrentRect.Right < 0 ? 0 : (UInt32)CurrentRect.Right,
                bottom = CurrentRect.Bottom < 0 ? 0 : (UInt32)CurrentRect.Bottom,
                width = right - left,
                height = bottom - top;

            if (left >= CurrentTexture.ImageDimension.X)
                left = (UInt32)CurrentTexture.ImageDimension.X - 1;
            if (top >= CurrentTexture.ImageDimension.Y)
                top = (UInt32)CurrentTexture.ImageDimension.Y - 1;
            if (right >= CurrentTexture.ImageDimension.X)
                right = (UInt32)CurrentTexture.ImageDimension.X - 1;
            if (bottom >= CurrentTexture.ImageDimension.Y)
                bottom = (UInt32)CurrentTexture.ImageDimension.Y - 1;

            UInt32
                newLeft = left,
                newTop = top,
                newRight = right,
                newBottom = bottom;

            while (newLeft < right)
            {
                UInt32 y = top;
                while (y < bottom)
                {
                    if (CurrentTexture.GetPixel(newLeft, y).A == 0)
                        break;

                    ++y;
                }

                if (y < bottom)
                    break;

                ++newLeft;
            }

            while (newTop < bottom)
            {
                UInt32 x = left;
                while (x < right)
                {
                    if (CurrentTexture.GetPixel(x, newTop).A == 0)
                        break;

                    ++x;
                }

                if (x < right)
                    break;

                ++newTop;
            }

            while (newRight > left)
            {
                UInt32 y = top;
                while (y < bottom)
                {
                    if (CurrentTexture.GetPixel(newRight, y).A == 0)
                        break;

                    ++y;
                }

                if (y < bottom)
                    break;

                --newRight;
            }

            while (newBottom > top)
            {
                UInt32 x = left;
                while (x < right)
                {
                    if (CurrentTexture.GetPixel(x, newBottom).A == 0)
                        break;

                    ++x;
                }

                if (x < right)
                    break;

                --newBottom;
            }

            Selector.SetRect(new BlazeraLib.FloatRect(
                left + newLeft,
                top + newTop,
                right + newRight,
                bottom + newBottom));
        }

        void SetCurrentTexture(Texture currentTexture)
        {
            CurrentTexture = currentTexture;

            DisplayScreen.SetCurrentPicture(CurrentTexture);
        }

        public String RectToString(BlazeraLib.FloatRect rect)
        {
            return "Rect ( " + ((Int32)rect.Left).ToString() + ", " + ((Int32)rect.Top).ToString() + ", " + ((Int32)rect.Right).ToString() + ", " + ((Int32)rect.Bottom).ToString() + " )";
        }

        public String RectToString(BlazeraLib.IntRect rect)
        {
            return "Rect ( " + rect.Left.ToString() + ", " + rect.Top.ToString() + ", " + rect.Right.ToString() + ", " + rect.Bottom.ToString() + " )";
        }

        void BottomDownButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { Selector.Resize(new BlazeraLib.FloatRect(0F, 0F, 0F, -SelectorResizeValue)); }
        void BottomUpButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { Selector.Resize(new BlazeraLib.FloatRect(0F, 0F, 0F, SelectorResizeValue)); }
        void RightRightButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { ;Selector.Resize(new BlazeraLib.FloatRect(0F, 0F, -SelectorResizeValue, 0F)); }
        void RightLeftButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { Selector.Resize(new BlazeraLib.FloatRect(0F, 0F, SelectorResizeValue, 0F)); }
        void LeftRightButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { Selector.Resize(new BlazeraLib.FloatRect(-SelectorResizeValue, 0F, 0F, 0F)); }
        void LeftLeftButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { Selector.Resize(new BlazeraLib.FloatRect(SelectorResizeValue, 0F, 0F, 0F)); }
        void TopDownButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { Selector.Resize(new BlazeraLib.FloatRect(0F, -SelectorResizeValue, 0F, 0F)); }
        void TopUpButton_Clicked(object sender, SFML.Window.MouseButtonEventArgs e) { Selector.Resize(new BlazeraLib.FloatRect(0F, SelectorResizeValue, 0F, 0F)); }

        void Selector_OnChange(Selector sender, SelectionChangeEventArgs e)
        {
            if (CurrentTexture == null)
                return;

            CurrentRect = Selector.GetCurrentRectFromOrigin(DisplayScreen.GetLocalFromGlobal(DisplayScreen.GetLocalFromGlobalTexturePoint(CurrentTexture.Position)));
            RectLabel.Text = RectToString(CurrentRect);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (openingInfo == null)
                return;

            if (!openingInfo.IsModeValid())
                return;

            switch (OpeningMode = openingInfo.GetMode())
            {
                case "TextureCreator_Mode":

                    DisplayScreen.ChangeMode(DisplayScreen.EMode.Scroll);
                    RefreshUpDownBoxes(DisplayScreen.EMode.Scroll);

                    SetCurrentTexture(openingInfo.GetArg<Texture>("Texture"));

                    if (!openingInfo.GetArg<Boolean>("RectIsValid"))
                        break;

                    BlazeraLib.IntRect rect = openingInfo.GetArg<BlazeraLib.IntRect>("Rect");
                    WidthUpDownBox.SetCurrentValue(rect.Rect.Width);
                    HeightUpDownBox.SetCurrentValue(rect.Rect.Height);
                    XUpDownBox.SetCurrentValue(rect.Left);
                    YUpDownBox.SetCurrentValue(rect.Top);

                    AdjustRect();

                    break;

                case "BoundingBoxCreator_Mode":

                    DisplayScreen.ChangeMode(DisplayScreen.EMode.Scroll);
                    RefreshUpDownBoxes(DisplayScreen.EMode.Normal);
                    DisplayScreen.TextureLocalRectMode = true;
                    SetCurrentTexture(openingInfo.GetArg<Texture>("Texture"));

                    break;
            }
        }

        protected override Dictionary<String, Object> OnValidate()
        {
            BlazeraLib.IntRect rect = new BlazeraLib.IntRect(
                (Int32)CurrentRect.Left,
                (Int32)CurrentRect.Top,
                (Int32)CurrentRect.Right,
                (Int32)CurrentRect.Bottom);

            return new Dictionary<String, Object>()
            {
                { "Rect", rect }
            };
        }
    }
}