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
            this.TextureN = Create.Texture("CheckBoxN");
            this.TextureN.Dimension *= RESIZE_FACTOR;
            this.TextureC = Create.Texture("CheckBoxC");
            this.TextureC.Dimension *= RESIZE_FACTOR;

            this.Box = new Button(this.TextureN, null);
            this.Box.ClickOffset = BUTTON_CLICK_OFFSET;
            this.Box.Clicked += new ClickEventHandler(Button_Clicked);

            this.GetLabelWidget().Clicked += new ClickEventHandler(Button_Clicked);

            this.AddLabeledWidget(Box);

            SetIsChecked(isChecked);
        }

        protected override void CallShortCut()
        {
            this.SetIsChecked(!this.IsChecked);
        }

        public CheckBox(Texture picture, LabeledWidget.EMode mode = LabeledWidget.DEFAULT_MODE, Boolean isChecked = DEFAULT_STATE) :
            base(picture, mode)
        {
            this.TextureN = Create.Texture("CheckBoxN");
            this.TextureN.Dimension *= RESIZE_FACTOR;
            this.TextureC = Create.Texture("CheckBoxC");
            this.TextureC.Dimension *= RESIZE_FACTOR;

            this.Box = new Button(this.TextureN, null);
            this.Box.ClickOffset = BUTTON_CLICK_OFFSET;
            this.Box.Clicked += new ClickEventHandler(Button_Clicked);

            this.GetLabelWidget().Clicked += new ClickEventHandler(Button_Clicked);

            this.AddLabeledWidget(Box);

            SetIsChecked(isChecked);
        }

        void Button_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            this.SetIsChecked(!this.IsChecked);
        }

        public override void Refresh()
        {
            
        }

        public override void Reset()
        {
            this.SetIsChecked(false);
        }

        public void SetIsChecked(Boolean isChecked)
        {
            this.IsChecked = isChecked;

            if (this.IsChecked)
                this.Box.SetTextures(this.TextureC, null);
            else
                this.Box.SetTextures(this.TextureN, null);

            if (this.Checked != null)
                this.Checked(this, new CheckEventArgs(this.IsChecked));
        }
    }

    public delegate void CheckEventHandler(object sender, CheckEventArgs e);

    public class CheckEventArgs : EventArgs
    {
        public CheckEventArgs(Boolean isChecked)
        {
            this.IsChecked = isChecked;
        }

        public Boolean IsChecked { get; set; }
    }
}
