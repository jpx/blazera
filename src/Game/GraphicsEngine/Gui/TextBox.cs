using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class TextBox : Widget
    {
        public enum EInputType
        {
            All,
            Alpha,
            Numeric,
            AlphaNumeric
        }

        const float TEXT_HMARGINS = 8F;

        const float DEFAULT_BACKGROUND_SCALE_FACTOR = .28F;

        public const EInputType DEFAULT_INPUT_TYPE = EInputType.All;

        private EInputType InputType { get; set; }

        public event TextAddedEventHandler TextAdded;

        private Label Label { get; set; }

        public TextBox(EInputType inputType = DEFAULT_INPUT_TYPE) :
            base()
        {
            this.InputType = inputType;

            this.Background = new PictureBox(Create.Texture("TextBoxBg"));

            this.Label = new Label(null, BlazeraLib.Label.ESize.VSmall);
            this.AddWidget(this.Label);

            this.BackHappened = false;
            this.IsActive = false;

            this.Dimension *= DEFAULT_BACKGROUND_SCALE_FACTOR;

            this.Label.Changed += new ChangeEventHandler(Label_Changed);
        }

        void Label_Changed(object sender, ChangeEventArgs e)
        {
            if (e.Type != ChangeEventArgs.EType.Text)
                return;

            if (this.TextAdded != null)
                this.TextAdded(this, new TextAddedEventArgs(((TextChangeEventArgs)e).Text));
        }

        public override void Refresh()
        {
            this.Label.Position = this.GetGlobalFromLocal(new Vector2f(TEXT_HMARGINS, this.Halfsize.Y - this.Label.Size / 2));
        }

        public override void Update(Time dt)
        {
            this.BackHappened = false;

            base.Update(dt);
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
            {
                return base.OnEvent(evt);
            }

            if (evt.Type == EventType.MouseButtonPressed)
            {
                if (this.Contains(Mouse.GetPosition(Root.Window).X,
                                  Mouse.GetPosition(Root.Window).Y))
                {
                    this.IsActive = true;

                    return true;
                }
                else
                {
                    if (this.IsActive)
                    {
                        this.IsActive = false;

                        return true;
                    }
                }
            }

            if (evt.Type == EventType.KeyPressed && this.IsActive)
            {
                if (evt.Key.Code == Keyboard.Key.Back)
                {
                    this.RemoveLast();

                    this.BackHappened = true;

                    return true;
                }

                if (evt.Key.Code == Keyboard.Key.Escape || evt.Key.Code == Keyboard.Key.Return)
                {
                    this.IsActive = false;

                    return true;
                }

                if (evt.Key.Code == Keyboard.Key.Delete)
                {
                    this.Reset();

                    return true;
                }
            }

            if (evt.Type == EventType.TextEntered && this.IsActive)
            {
                if (!this.BackHappened &&
                    this.Add((char)evt.Text.Unicode))
                    return true;
            }
            
            return base.OnEvent(evt);
        }

        private Boolean IsValidChar(Char c)
        {
            switch (this.InputType)
            {
                case EInputType.All:
                    return true;

                case EInputType.Alpha:
                    Regex alphaPattern = new Regex("[a-zA-Z]");
                    return alphaPattern.IsMatch(c.ToString());

                case EInputType.Numeric:
                    Regex numericPattern = new Regex("[0-9-]");

                    if (c == '-' &&
                        (this.Text.Contains('-') || this.Text.Length != 0))
                        return false;

                    return numericPattern.IsMatch(c.ToString());

                case EInputType.AlphaNumeric:
                    Regex alphaNumericPattern = new Regex("[a-zA-Z0-9]");
                    return alphaNumericPattern.IsMatch(c.ToString());

                default:
                    return true;
                   
            }
        }

        public Boolean Add(char c)
        {
            if (!this.IsValidChar(c))
                return false;

            this.Text += c;
            this.Label.Text += c;

            while (this.Label.Dimension.X > this.Dimension.X - 2 * TEXT_HMARGINS)
            {
                this.Label.Text = this.Label.Text.Substring(1, this.Label.Text.Length - 1);
            }

            return true;
        }

        public Boolean Add(String str)
        {
            Boolean succeeded = true;

            foreach (Char c in str)
            {
                succeeded &= this.Add(c);
            }

            return succeeded;
        }

        private void RemoveLast()
        {
            if (this.Text.Length == 0 || this.Label.Text.Length == 0)
            {
                return;
            }

            this.Text = this.Text.Remove(this.Text.Length - 1);

            this.Label.Text = this.Label.Text.Remove(this.Label.Text.Length - 1);

            if (this.Text.Length != this.Label.Text.Length)
            {
                this.Label.Text = this.Text.Substring(this.Text.Length - this.Label.Text.Length - 1,
                                                                  this.Label.Text.Length + 1);
            }
        }

        public override void Reset()
        {
            this.Text = "";
            this.Label.Text = "";
        }

        public void Reset(String text)
        {
            Reset();

            Add(text);
        }

        public void SetFont(String font)
        {
            this.Label.Font = new Font(GameDatas.DATAS_DEFAULT_PATH + "/fonts/" + font + ".ttf");
        }

        public String Text { get; private set; }

        public Boolean TextIsValid()
        {
            return (this.Text != null && this.Text.Trim() != "");
        }

        private Boolean BackHappened { get; set; }

        private Boolean _isActive;
        public Boolean IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;

                if (this.IsActive)
                {
                    Parent.Focused = this;
                    ((PictureBox)this.Background).Texture.Color = new Color(128, 255, 128);
                }
                else
                {
                    if (Parent != null)
                        Parent.Focused = null;
                    ((PictureBox)this.Background).Texture.Color = Color.White;
                }
            }
        }
    }

    public delegate void TextAddedEventHandler(object sender, TextAddedEventArgs e);

    public class TextAddedEventArgs : EventArgs
    {
        public TextAddedEventArgs(String textAdded)
        {
            this.TextAdded = textAdded;
        }

        public String TextAdded { get; set; }
    }

    public class LabeledTextBox : LabeledWidget
    {
        public TextBox TextBox;

        public LabeledTextBox(String label = null, EMode mode = DEFAULT_MODE, TextBox.EInputType inputType = TextBox.DEFAULT_INPUT_TYPE, Boolean shortCutMode = DEFAULT_SHORTCUT_MODE) :
            base(label, mode, shortCutMode)
        {
            TextBox = new TextBox(inputType);
            AddLabeledWidget(TextBox);

            GetLabelWidget().Clicked += new ClickEventHandler(LabeledTextBox_Clicked);
        }

        void LabeledTextBox_Clicked(object sender, MouseButtonEventArgs e)
        {
            TextBox.IsActive = true;
        }

        protected override void CallShortCut()
        {
            TextBox.IsActive = true;
        }
    }
}