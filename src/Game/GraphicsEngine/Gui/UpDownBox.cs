using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;

namespace BlazeraLib
{
    public class UpDownBox : LabeledWidget
    {
        const Int32 DEFAULT_MIN_VALUE = 0;
        const Int32 DEFAULT_MAX_VALUE = 99;
        const Int32 DEFAULT_SCALE_VALUE = 1;
        const Int32 DEFAULT_DEFAULT_VALUE = 0;

        const float BUTTON_RESIZE_FACTOR = .4F;
        const float TEXTBOX_TEXT_SIZE_SCALE_FACTOR = .9F;

        private HAutoSizeBox MainBox { get; set; }
        private VAutoSizeBox ButtonBox { get; set; }

        private Button UpBtn { get; set; }
        private Button DownBtn { get; set; }

        private TextBox TextBox { get; set; }
        private Boolean ValueIsModified { get; set; }

        private Int32 MinValue { get; set; }
        private Int32 MaxValue { get; set; }
        private Int32 ScaleValue { get; set; }
        Int32 DefaultValue;

        public UpDownBox(
            Int32 minValue = DEFAULT_MIN_VALUE,
            Int32 maxValue = DEFAULT_MAX_VALUE,
            Int32 scaleValue = DEFAULT_SCALE_VALUE,
            Int32 defaultValue = DEFAULT_DEFAULT_VALUE,
            String label = null,
            LabeledWidget.EMode mode = DEFAULT_MODE,
            Boolean shortCutMode = DEFAULT_SHORTCUT_MODE) :
            base(label, mode, shortCutMode)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            ScaleValue = scaleValue;
            DefaultValue = defaultValue;

            this.MainBox = new HAutoSizeBox(true, null, 0);

            this.ButtonBox = new VAutoSizeBox(true, null, 0);

            this.DownBtn = new Button(Create.Texture("Gui_DownButtonN"), Create.Texture("Gui_DownButtonO"));
            this.DownBtn.Dimension *= BUTTON_RESIZE_FACTOR;
            this.DownBtn.Clicked += new ClickEventHandler(DownBtn_Clicked);
            this.AddWidget(this.DownBtn);

            this.UpBtn = new Button(Create.Texture("Gui_UpButtonN"), Create.Texture("Gui_UpButtonO"));
            this.UpBtn.Dimension *= BUTTON_RESIZE_FACTOR;
            this.UpBtn.Clicked += new ClickEventHandler(UpBtn_Clicked);

            this.ButtonBox.AddItem(this.UpBtn);
            this.ButtonBox.AddItem(this.DownBtn);

            this.TextBox = new TextBox(BlazeraLib.TextBox.EInputType.Numeric);

            this.SetCurrentValue(GetDefaultValue());
            this.TextBox.Dimension = new SFML.Graphics.Vector2(40F, this.UpBtn.Dimension.Y + this.DownBtn.Dimension.Y);

            this.TextBox.TextAdded += new TextAddedEventHandler(TextBox_TextAdded);
            this.AddWidget(this.TextBox);

            this.ValueIsModified = false;

            this.MainBox.AddItem(this.TextBox);
            this.MainBox.AddItem(this.ButtonBox);

            this.AddLabeledWidget(this.MainBox);

            GetLabelWidget().Clicked += new ClickEventHandler(UpDownBox_Clicked);
        }

        protected override void CallShortCut()
        {
            TextBox.IsActive = true;
        }

        void UpDownBox_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            TextBox.IsActive = true;
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (this.ValueIsModified)
            {
                this.AdjustValue();
                this.ValueIsModified = false;
            }
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case SFML.Window.EventType.KeyPressed:

                    if (!this.TextBox.IsActive)
                        break;

                    switch (evt.Key.Code)
                    {
                        case SFML.Window.KeyCode.Up:

                            this.Up();

                            return true;

                        case SFML.Window.KeyCode.Down:

                            this.Down();

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        public override bool OnPredominantEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case SFML.Window.EventType.KeyPressed:

                    if (!this.TextBox.IsActive)
                        break;

                    switch (evt.Key.Code)
                    {
                        case SFML.Window.KeyCode.Up:

                            this.Up();

                            return true;

                        case SFML.Window.KeyCode.Down:

                            this.Down();

                            return true;
                    }

                    break;
            }
            
            return base.OnPredominantEvent(evt);
        }

        public override void Refresh() { }

        void TextBox_TextAdded(object sender, TextAddedEventArgs e)
        {
            this.ValueIsModified = true;
        }

        void DownBtn_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            this.Down();
        }

        void UpBtn_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            this.Up();
        }

        Int32 GetDefaultValue()
        {
            return MaxValue >= DefaultValue && MinValue <= DefaultValue ? DefaultValue : MinValue;
        }

        public void SetDefaultValue(Int32 defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public Int32 GetCurrentValue()
        {
            if (this.TextBox.Text == "")
                this.TextBox.Add(GetDefaultValue().ToString());

            if (this.TextBox.Text == "-")
                return Int32.Parse(this.TextBox.Text + (MinValue >= 0 ? this.MinValue.ToString() : MinValue.ToString().Substring(1, MinValue.ToString().Length - 1)));

            return Int32.Parse(this.TextBox.Text);
        }

        public void ChangeValues(Int32 minValue, Int32 maxValue)
        {
            this.MinValue = minValue;
            this.MaxValue = maxValue;

            this.SetCurrentValue(GetDefaultValue());
        }

        public void SetScaleValue(Int32 scaleValue)
        {
            ScaleValue = scaleValue;

            this.SetCurrentValue(GetDefaultValue());
        }

        public void SetCurrentValue(Int32 value)
        {
            this.TextBox.Reset();
            this.TextBox.Add(value.ToString());
        }

        private void Up()
        {
            if (this.GetCurrentValue() + this.ScaleValue > this.MaxValue)
                return;

            this.SetCurrentValue(this.GetCurrentValue() + this.ScaleValue);

            this.AdjustValue();
        }

        private void Down()
        {
            if (this.GetCurrentValue() - this.ScaleValue < this.MinValue)
                return;

            this.SetCurrentValue(this.GetCurrentValue() - this.ScaleValue);

            this.AdjustValue();
        }

        private void AdjustValue()
        {
            if (this.GetCurrentValue() > this.MaxValue)
                this.SetCurrentValue(this.MaxValue);
            else if (this.GetCurrentValue() < this.MinValue)
                this.SetCurrentValue(this.MinValue);

            if (this.ValueChanged != null)
                this.ValueChanged(this, new ValueChangeEventArgs(this.GetCurrentValue()));
        }

        public override void Reset()
        {
            this.SetCurrentValue(GetDefaultValue());
        }

        public event ValueChangeEventHandler ValueChanged;
    }

    public delegate void ValueChangeEventHandler(UpDownBox sender, ValueChangeEventArgs e);

    public class ValueChangeEventArgs : EventArgs
    {
        public Int32 Value { get; private set; }

        public ValueChangeEventArgs(Int32 value)
        {
            this.Value = value;
        }
    }
}
