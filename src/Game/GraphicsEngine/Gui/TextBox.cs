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
            InputType = inputType;

            Background = new PictureBox(Create.Texture("TextBoxBg"));

            Label = new Label(null, BlazeraLib.Label.ESize.VSmall);
            AddWidget(Label);

            BackHappened = false;
            IsActive = false;

            Dimension *= DEFAULT_BACKGROUND_SCALE_FACTOR;

            Label.Changed += new ChangeEventHandler(Label_Changed);
        }

        void Label_Changed(object sender, ChangeEventArgs e)
        {
            if (e.Type != ChangeEventArgs.EType.Text)
                return;

            if (TextAdded != null)
                TextAdded(this, new TextAddedEventArgs(((TextChangeEventArgs)e).Text));
        }

        public override void Refresh()
        {
            Label.Position = GetGlobalFromLocal(new Vector2f(TEXT_HMARGINS, Halfsize.Y - Label.Size / 2));
        }

        public override void Update(Time dt)
        {
            BackHappened = false;

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
                if (Contains(Mouse.GetPosition(Root.Window).X,
                                  Mouse.GetPosition(Root.Window).Y))
                {
                    IsActive = true;

                    return true;
                }
                else
                {
                    if (IsActive)
                    {
                        IsActive = false;

                        return true;
                    }
                }
            }

            if (evt.Type == EventType.KeyPressed && IsActive)
            {
                if (evt.Key.Code == Keyboard.Key.Back)
                {
                    RemoveLast();

                    BackHappened = true;

                    return true;
                }

                if (evt.Key.Code == Keyboard.Key.Escape || evt.Key.Code == Keyboard.Key.Return)
                {
                    IsActive = false;

                    return true;
                }

                if (evt.Key.Code == Keyboard.Key.Delete)
                {
                    Reset();

                    return true;
                }
            }

            if (evt.Type == EventType.TextEntered && IsActive)
            {
                if (!BackHappened &&
                    Add((char)evt.Text.Unicode))
                    return true;
            }
            
            return base.OnEvent(evt);
        }

        private Boolean IsValidChar(Char c)
        {
            switch (InputType)
            {
                case EInputType.All:
                    return true;

                case EInputType.Alpha:
                    Regex alphaPattern = new Regex("[a-zA-Z]");
                    return alphaPattern.IsMatch(c.ToString());

                case EInputType.Numeric:
                    Regex numericPattern = new Regex("[0-9-]");

                    if (c == '-' &&
                        (Text.Contains('-') || Text.Length != 0))
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
            if (!IsValidChar(c))
                return false;

            Text += c;
            Label.Text += c;

            while (Label.Dimension.X > Dimension.X - 2 * TEXT_HMARGINS)
            {
                Label.Text = Label.Text.Substring(1, Label.Text.Length - 1);
            }

            return true;
        }

        public Boolean Add(String str)
        {
            Boolean succeeded = true;

            foreach (Char c in str)
            {
                succeeded &= Add(c);
            }

            return succeeded;
        }

        private void RemoveLast()
        {
            if (Text.Length == 0 || Label.Text.Length == 0)
            {
                return;
            }

            Text = Text.Remove(Text.Length - 1);

            Label.Text = Label.Text.Remove(Label.Text.Length - 1);

            if (Text.Length != Label.Text.Length)
            {
                Label.Text = Text.Substring(Text.Length - Label.Text.Length - 1,
                                                                  Label.Text.Length + 1);
            }
        }

        public override void Reset()
        {
            Text = "";
            Label.Text = "";
        }

        public void Reset(String text)
        {
            Reset();

            Add(text);
        }

        public void SetFont(String font)
        {
            Label.Font = new Font(GameData.DATAS_DEFAULT_PATH + "/fonts/" + font + ".ttf");
        }

        public String Text { get; private set; }

        public Boolean TextIsValid()
        {
            return (Text != null && Text.Trim() != "");
        }

        private Boolean BackHappened { get; set; }

        private Boolean _isActive;
        public Boolean IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;

                if (IsActive)
                {
                    Parent.Focused = this;
                    ((PictureBox)Background).Texture.Color = new Color(128, 255, 128);
                }
                else
                {
                    if (Parent != null)
                        Parent.Focused = null;
                    ((PictureBox)Background).Texture.Color = Color.White;
                }
            }
        }
    }

    public delegate void TextAddedEventHandler(object sender, TextAddedEventArgs e);

    public class TextAddedEventArgs : EventArgs
    {
        public TextAddedEventArgs(String textAdded)
        {
            TextAdded = textAdded;
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