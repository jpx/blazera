using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class CheckBox : LabeledWidget
    {
        public const Boolean DEFAULT_STATE = false;
        public const float RESIZE_FACTOR = .6F;
        public const float BUTTON_CLICK_OFFSET = 0F;
        public const float DEFAULT_PICTURE_SIZE = 30F;
        
        private Texture TextureN { get; set; }
        private Texture TextureC { get; set; }

        private Button Box { get; set; }

        public Boolean IsChecked { get; private set; }

        public event CheckEventHandler Checked;

        public CheckBox(String label, LabeledWidget.EMode mode = LabeledWidget.DEFAULT_MODE, Boolean isChecked = DEFAULT_STATE, Boolean shortCutMode = DEFAULT_SHORTCUT_MODE) :
            base(label, mode, shortCutMode)
        {
            TextureN = Create.Texture("CheckBoxN");
            TextureN.Dimension *= RESIZE_FACTOR;
            TextureC = Create.Texture("CheckBoxC");
            TextureC.Dimension *= RESIZE_FACTOR;

            Box = new Button(TextureN, null);
            Box.ClickOffset = BUTTON_CLICK_OFFSET;
            Box.Clicked += new ClickEventHandler(Button_Clicked);

            GetLabelWidget().Clicked += new ClickEventHandler(Button_Clicked);

            AddLabeledWidget(Box);

            SetIsChecked(isChecked);
        }

        protected override void CallShortCut()
        {
            SetIsChecked(!IsChecked);
        }

        public CheckBox(Texture picture, LabeledWidget.EMode mode = LabeledWidget.DEFAULT_MODE, Boolean isChecked = DEFAULT_STATE) :
            base(picture, mode)
        {
            TextureN = Create.Texture("CheckBoxN");
            TextureN.Dimension *= RESIZE_FACTOR;
            TextureC = Create.Texture("CheckBoxC");
            TextureC.Dimension *= RESIZE_FACTOR;

            Box = new Button(TextureN, null);
            Box.ClickOffset = BUTTON_CLICK_OFFSET;
            Box.Clicked += new ClickEventHandler(Button_Clicked);

            GetLabelWidget().Clicked += new ClickEventHandler(Button_Clicked);

            AddLabeledWidget(Box);

            SetIsChecked(isChecked);
        }

        void Button_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            SetIsChecked(!IsChecked);
        }

        public override void Refresh()
        {
            
        }

        public override void Reset()
        {
            SetIsChecked(false);
        }

        public void SetIsChecked(Boolean isChecked)
        {
            IsChecked = isChecked;

            if (IsChecked)
                Box.SetTextures(TextureC, null);
            else
                Box.SetTextures(TextureN, null);

            if (Checked != null)
                Checked(this, new CheckEventArgs(IsChecked));
        }
    }

    public delegate void CheckEventHandler(object sender, CheckEventArgs e);

    public class CheckEventArgs : EventArgs
    {
        public CheckEventArgs(Boolean isChecked)
        {
            IsChecked = isChecked;
        }

        public Boolean IsChecked { get; set; }
    }
}
