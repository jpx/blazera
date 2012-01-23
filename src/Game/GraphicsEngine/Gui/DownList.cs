using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class DownList : ScrollableWidget
    {
        const float CURSOR_VELOCITY = 50F;

        private Button Top { get; set; }

        List<Button> Texts;

        Boolean IsActive;

        public DownList(Int32 size = BlazeraLib.ExtendedBox.DEFAULT_SIZE) :
            base(size, false)
        {
            Top = new Button(null, Button.EMode.LabelEffect);
            Top.Clicked += new ClickEventHandler(Top_Clicked);
            MainBox.AddItemFirst(Top);

            ScrollBox.SetBackgroundAlphaFactor(100D);
            ScrollBox.Close();

            Texts = new List<Button>();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.MouseButtonPressed:

                    if (evt.MouseButton.Button != Mouse.Button.Left)
                        break;

                    if (!IsActive)
                        break;

                    Reduce();

                    return true;

                case EventType.MouseButtonReleased:

                    if (evt.MouseButton.Button != Mouse.Button.Left)
                        break;

                    if (!IsActive)
                        break;

                    Reduce();

                    return true;

                case EventType.KeyPressed:

                    switch (evt.Key.Code)
                    {
                        case Keyboard.Key.Escape:

                            if (!IsActive)
                                break;

                            Reduce();

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            if (ScrollBox == null)
                return;

            ScrollBox.Close();
        }

        public override void Reset()
        {
            base.Reset();

            if (ScrollBox != null)
                ScrollBox.Close();

            if (Top == null)
                return;

            if (Texts.Count > 0)
                Top.Text = Texts[0].Text;
        }

        void Top_Clicked(object sender, MouseButtonEventArgs e)
        {
            SwitchState();
        }

        public void AddText(Button text)
        {
            text.Clicked += new ClickEventHandler(button_Clicked);
            AddItem(text);

            if (Texts.Count == 0)
                Top.Text = text.Text;

            Texts.Add(text);
        }

        void button_Clicked(object sender, MouseButtonEventArgs e)
        {
            Top.Text = ((Button)sender).Text;

            Reduce();
        }

        public override Vector2f Dimension
        {
            get
            {
                if (Top == null || ScrollBox == null)
                    return base.Dimension;

                return new Vector2f(
                    Top.Dimension.X,
                    Top.Dimension.Y);
            }
        }

        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            base.OnScroll(sender, e);

            foreach (Button text in Texts)
                text.Reset();
        }

        public void Restore()
        {
            ScrollBox.Open();
            Parent.Focused = this;
            IsActive = true;

            Init();
        }

        public void Reduce()
        {
            ScrollBox.Close();
            Parent.Focused = null;
            IsActive = false;

            Init();
        }

        public void SwitchState()
        {
            if (ScrollBox.IsVisible)
                Reduce();
            else
                Restore();
        }

        public String GetCurrent()
        {
            return Top.Text;
        }

        public void SetCurrent(String text)
        {
            Boolean isContained = false;
            foreach (Button button in Texts)
                if (button.Text == text)
                {
                    isContained = true;
                    button.CallClicked(null);
                    break;
                }

            if (isContained)
                Top.Text = text;
        }

        public override void Clear()
        {
            base.Clear();

            Texts.Clear();
        }

        public override Boolean RemoveItem(Widget widget)
        {
            if (!base.RemoveItem(widget))
                return false;

            return Texts.Remove((Button)widget);
        }
    }

    public class LabeledDownList : LabeledWidget
    {
        const EMode DEFAULT_MODE = EMode.Right;

        public DownList DownList { get; set; }

        public LabeledDownList(String label, Int32 size = ExtendedBox.DEFAULT_SIZE, EMode mode = DEFAULT_MODE) :
            base(label, mode, false)
        {
            DownList = new DownList(size);
            
            AddLabeledWidget(DownList);
            GetLabelWidget().Clicked += new ClickEventHandler(LabeledDownList_Clicked);
        }

        void LabeledDownList_Clicked(object sender, MouseButtonEventArgs e)
        {
            DownList.SwitchState();
        }

        public String GetCurrent()
        {
            return DownList.GetCurrent();
        }

        protected override void CallShortCut() { }
    }
}
