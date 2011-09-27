using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class TextList : ScrollableWidget
    {
        const float CURSOR_VELOCITY = 50F;

        private Button Current { get; set; }

        private PictureBox Cursor { get; set; }
        private Boolean CursorMode { get; set; }

        List<Button> Texts;

        public TextList(Int32 size = BlazeraLib.ExtendedBox.DEFAULT_SIZE, Boolean cursorMode = true) :
            base(size)
        {
            if (this.CursorMode = cursorMode)
            {
                this.Cursor = new PictureBox(Create.Texture("Gui_TextListSl"));
                this.AddWidget(this.Cursor);
            }

            Texts = new List<Button>();
        }

        public List<String> GetTexts()
        {
            List<String> texts = new List<String>();

            foreach (Button text in Texts)
                texts.Add(text.Text);

            return texts;
        }

        public override bool OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
                return base.OnEvent(evt);

            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    switch (evt.Key.Code)
                    {
                        case KeyCode.Down:

                            if (CursorMode && !Down())
                                break;

                            return true;

                        case KeyCode.Up:

                            if (CursorMode && !Up())
                                break;

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        Boolean ExtendedBoxContainsCurrent(Int32 index)
        {
            return
                ExtendedBox.CurrentPointer <= index &&
                index < ExtendedBox.CurrentPointer + ExtendedBox.Size;
        }

        Boolean Down()
        {
            if (Current == null)
                return false;

            Int32 index = Texts.IndexOf(Current);

            if (index == Texts.Count - 1)
                return false;

            if (!ExtendedBoxContainsCurrent(index))
            {
                VScrollBar.Reset();
                VScrollBar.Scroll(index + 1);
            }

            Current = Texts[index + 1];

            if (!ExtendedBoxContainsCurrent(index + 1))
            {
                ExtendedBox.SetCurrentPointer(ExtendedBox.CurrentPointer + 1);
                VScrollBar.Scroll(1);
            }

            Current.CallClicked(new MouseButtonEventArgs(new MouseButtonEvent()));

            RefreshCursor();

            return true;
        }

        Boolean Up()
        {
            if (Current == null)
                return false;

            Int32 index = Texts.IndexOf(Current);

            if (index == 0)
                return false;

            if (!ExtendedBoxContainsCurrent(index))
            {
                VScrollBar.Reset();
                VScrollBar.Scroll(index - 1);
            }

            Current = Texts[index - 1];

            if (!ExtendedBoxContainsCurrent(index - 1))
            {
                ExtendedBox.SetCurrentPointer(ExtendedBox.CurrentPointer - 1);
                VScrollBar.Scroll(-1);
            }

            Current.CallClicked(new MouseButtonEventArgs(new MouseButtonEvent()));

            RefreshCursor();

            return true;
        }

        public void AddText(Button text, Boolean isCurrent = false)
        {
            text.Clicked += new ClickEventHandler(button_Clicked);
            AddItem(text);

            Texts.Add(text);

            if (CursorMode && Texts.Count > 0)
            {
                Current = Texts[0];
                Current.CallClicked(null);
            }

            ExtendedBox.Reset();
            
            Init(); // pour eviter l'absence de resize de la box

            this.RefreshCursor();
        }

        public void AddText(List<Button> texts, Int32 currentIndex = 0)
        {
            if (texts == null ||
                texts.Count == 0)
                return;

            foreach (Button text in texts)
            {
                text.Clicked += new ClickEventHandler(button_Clicked);
                AddItem(text);
                Texts.Add(text);
            }

            if (CursorMode && Current == null)
            {
                Current = texts[currentIndex];
                Current.CallClicked(null);
                ExtendedBox.Reset();
            }

            Init();

            RefreshCursor();
        }

        public Boolean RemoveText(Button text)
        {
            if (!RemoveItem(text))
                return false;

            if (!Texts.Remove(text))
                return false;

            if (CursorMode && Texts.Count > 0)
            {
                Current = Texts[0];
                Current.CallClicked(null);
            }

            this.RefreshCursor();

            Init();  // pour eviter l'absence de resize de la box

            return true;
        }

        public Boolean RemoveText(String text)
        {
            foreach (Button textButton in Texts)
                if (textButton.Text == text)
                    return RemoveText(textButton);

            return false;
        }

        public Boolean ContainsText(String text)
        {
            foreach (Button textButton in Texts)
                if (textButton.Text == text)
                    return true;

            return false;
        }

        public Boolean RemoveCurrent()
        {
            return RemoveText(Current);
        }

        public String GetCurrent()
        {
            if (Current == null)
                return null;

            return Current.Text;
        }

        public void SetCurrentText(String text)
        {
            Current.Text = text;
        }

        public Int32 GetTextCount()
        {
            return Texts.Count;
        }

        void button_Clicked(object sender, MouseButtonEventArgs e)
        {
            this.Current = (Button)sender;

            this.RefreshCursor();
        }

        protected override void OnScroll(object sender, ScrollEventArgs e)
        {
            base.OnScroll(sender, e);

            this.RefreshCursor();
        }

        public override void Clear()
        {
            base.Clear();

            this.Current = null;

            Texts.Clear();

            this.RefreshCursor();
        }

        private void RefreshCursor()
        {
            if (!this.CursorMode)
                return;

            if (this.ExtendedBox.CurrentContains(this.Current) &&
                Texts.Count != 0)
                this.Cursor.Open();
            else
                this.Cursor.Close();
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (!this.CursorMode)
                return;

            if (this.Current == null)
                return;

            this.Cursor.Dimension += new Vector2(
                (this.Current.Dimension.X - this.Cursor.Dimension.X) / CURSOR_VELOCITY * (float)dt.MS,
                (this.Current.Dimension.Y - this.Cursor.Dimension.Y) / CURSOR_VELOCITY * (float)dt.MS);

            this.Cursor.Position += new Vector2(
                this.Current.Position.X - this.Cursor.Position.X,
                (this.Current.Center.Y - this.Cursor.Center.Y) / CURSOR_VELOCITY * (float)dt.MS);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            RefreshCursor();
        }

        public void AddEvent(ClickEventHandler onClick)
        {
            foreach (Button text in Texts)
                text.Clicked += onClick;
        }

        public void RemoveEvent(ClickEventHandler onClick)
        {
            foreach (Button text in Texts)
                text.Clicked -= onClick;
        }
    }
}