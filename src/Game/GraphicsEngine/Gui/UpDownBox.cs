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

            MainBox = new HAutoSizeBox(true, null, 0);

            ButtonBox = new VAutoSizeBox(true, null, 0);

            DownBtn = new Button(Create.Texture("Gui_DownButtonN"), Create.Texture("Gui_DownButtonO"));
            DownBtn.Dimension *= BUTTON_RESIZE_FACTOR;
            DownBtn.Clicked += new ClickEventHandler(DownBtn_Clicked);
            AddWidget(DownBtn);

            UpBtn = new Button(Create.Texture("Gui_UpButtonN"), Create.Texture("Gui_UpButtonO"));
            UpBtn.Dimension *= BUTTON_RESIZE_FACTOR;
            UpBtn.Clicked += new ClickEventHandler(UpBtn_Clicked);

            ButtonBox.AddItem(UpBtn);
            ButtonBox.AddItem(DownBtn);

            TextBox = new TextBox(BlazeraLib.TextBox.EInputType.Numeric);

            SetCurrentValue(GetDefaultValue());
            TextBox.Dimension = new SFML.Window.Vector2f(40F, UpBtn.Dimension.Y + DownBtn.Dimension.Y);

            TextBox.TextAdded += new TextAddedEventHandler(TextBox_TextAdded);
            AddWidget(TextBox);

            ValueIsModified = false;

            MainBox.AddItem(TextBox);
            MainBox.AddItem(ButtonBox);

            AddLabeledWidget(MainBox);

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

            if (ValueIsModified)
            {
                AdjustValue();
                ValueIsModified = false;
            }
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case SFML.Window.EventType.KeyPressed:

                    if (!TextBox.IsActive)
                        break;

                    switch (evt.Key.Code)
                    {
                        case SFML.Window.Keyboard.Key.Up:

                            Up();

                            return true;

                        case SFML.Window.Keyboard.Key.Down:

                            Down();

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

                    if (!TextBox.IsActive)
                        break;

                    switch (evt.Key.Code)
                    {
                        case SFML.Window.Keyboard.Key.Up:

                            Up();

                            return true;

                        case SFML.Window.Keyboard.Key.Down:

                            Down();

                            return true;
                    }

                    break;
            }
            
            return base.OnPredominantEvent(evt);
        }

        public override void Refresh() { }

        void TextBox_TextAdded(object sender, TextAddedEventArgs e)
        {
            ValueIsModified = true;
        }

        void DownBtn_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Down();
        }

        void UpBtn_Clicked(object sender, SFML.Window.MouseButtonEventArgs e)
        {
            Up();
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
            if (TextBox.Text == "")
                TextBox.Add(GetDefaultValue().ToString());

            if (TextBox.Text == "-")
                return Int32.Parse(TextBox.Text + (MinValue >= 0 ? MinValue.ToString() : MinValue.ToString().Substring(1, MinValue.ToString().Length - 1)));

            return Int32.Parse(TextBox.Text);
        }

        public void ChangeValues(Int32 minValue, Int32 maxValue)
        {
            MinValue = minValue;
            MaxValue = maxValue;

            SetCurrentValue(GetDefaultValue());
        }

        public void SetScaleValue(Int32 scaleValue)
        {
            ScaleValue = scaleValue;

            SetCurrentValue(GetDefaultValue());
        }

        public void SetCurrentValue(Int32 value)
        {
            TextBox.Reset();
            TextBox.Add(value.ToString());
        }

        private void Up()
        {
            if (GetCurrentValue() + ScaleValue > MaxValue)
                return;

            SetCurrentValue(GetCurrentValue() + ScaleValue);

            AdjustValue();
        }

        private void Down()
        {
            if (GetCurrentValue() - ScaleValue < MinValue)
                return;

            SetCurrentValue(GetCurrentValue() - ScaleValue);

            AdjustValue();
        }

        private void AdjustValue()
        {
            if (GetCurrentValue() > MaxValue)
                SetCurrentValue(MaxValue);
            else if (GetCurrentValue() < MinValue)
                SetCurrentValue(MinValue);

            if (ValueChanged != null)
                ValueChanged(this, new ValueChangeEventArgs(GetCurrentValue()));
        }

        public override void Reset()
        {
            SetCurrentValue(GetDefaultValue());
        }

        public event ValueChangeEventHandler ValueChanged;
    }

    public delegate void ValueChangeEventHandler(UpDownBox sender, ValueChangeEventArgs e);

    public class ValueChangeEventArgs : EventArgs
    {
        public Int32 Value { get; private set; }

        public ValueChangeEventArgs(Int32 value)
        {
            Value = value;
        }
    }
}
