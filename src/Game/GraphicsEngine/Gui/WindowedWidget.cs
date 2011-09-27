using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class WindowedWidget : Widget
    {
        public static readonly Color FOCUSED_COLOR = new Color(64, 64, 255);

        public const float BORDER_WIDTH = 8F;

        const HAlignment WINDOW_ITEM_DEFAULT_ALIGNMENT = HAlignment.Left;

        const float POSITION_SIDE_FACTOR = .5F;

        protected WindowedWidget FocusedWindow;
        ValidateEventHandler CurrentValidateEventHandler;

        protected String OpeningMode;

        protected void SetFocusedWindow(WindowedWidget focusedWindow, OpeningInfo openingInfo = null, ValidateEventHandler onValidate = null)
        {
            if (focusedWindow == null)
                return;

            FocusedWindow = focusedWindow;

            FocusedWindow.Closed += new CloseEventHandler(FocusedWindow_Closed);

            CurrentValidateEventHandler = onValidate;

            if (CurrentValidateEventHandler != null)
                FocusedWindow.Validated += new ValidateEventHandler(CurrentValidateEventHandler);

            FocusedWindow.Open(openingInfo);

            BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;
            FocusedWindow.BackgroundColor = FOCUSED_COLOR;

            ((EditorBaseWidget)Root).SetWindowOutDrawing(false, FocusedWindow);

            FocusedWindow.Init();

            if (Center.X > Root.Dimension.X * (1F - POSITION_SIDE_FACTOR))
                FocusedWindow.BackgroundRight = Left;
            else
                FocusedWindow.Left = BackgroundRight;
            FocusedWindow.Center = new Vector2(FocusedWindow.Center.X, Center.Y);
        }

        public Boolean GotFocusedWindow(Boolean alive = false)
        {
            if (!alive)
                return FocusedWindow != null;

            return FocusedWindow != null && FocusedWindow.IsVisible;
        }

        void FocusedWindow_Closed(Widget sender, CloseEventArgs e)
        {
            if (CurrentValidateEventHandler != null)
                FocusedWindow.Validated -= new ValidateEventHandler(CurrentValidateEventHandler);
            FocusedWindow.Closed -= new CloseEventHandler(FocusedWindow_Closed);
            FocusedWindow.BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;
            this.BackgroundColor = FOCUSED_COLOR;
            ((EditorBaseWidget)Root).SetWindowOutDrawing(true, FocusedWindow);
            FocusedWindow = null;
        }

        protected VAutoSizeBox MainBox { get; set; }

        public event ValidateEventHandler Validated;

        public WindowedWidget(String title, float windowTopBorderHeight = WindowBackground.DEFAULT_TOPBORDER_HEIGHT) :
            base()
        {
            IsBackgroundColorLinked = true;

            this.Margins = Border.GetWindowBorderWidth();

            this.Background = new WindowBackground(this.BackgroundDimension, windowTopBorderHeight);

            this.GetBackground().SetTitle(title);

            this.GetBackground().Dragged += new DragEventHandler(WindowBackground_Dragged);
            this.GetBackground().StateChanged += new StateChangeHandler(WindowedWidget_StateChanged);

            this.MainBox = new VAutoSizeBox(true, null, this.Margins);
            this.MainBox.Position = this.GetGlobalFromLocal(new Vector2(0F, 0F));
            this.AddWidget(this.MainBox);

            this.Close();
        }

        void WindowedWidget_StateChanged(WindowBackground sender, StateChangeEventArgs e)
        {
            if (this.GetBackground().State == WindowBackground.EState.Disabled)
                this.Close();

            else if (this.GetBackground().State != WindowBackground.EState.Restored)
                this.MainBox.Close();
            else
                this.MainBox.Open();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    switch (evt.Key.Code)
                    {
                        case KeyCode.Return:

                            if (this.Validated == null)
                                break;

                            this.CallValidated();

                            return true;

                        case KeyCode.Escape:

                            this.Close();

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        public override Boolean HandleEvent(BlzEvent evt)
        {
            if (!this.IsEnabled)
                return false;

            if (FocusedWindowHandleEvent(evt))
                return true;

            return base.HandleEvent(evt);
        }

        protected virtual Boolean FocusedWindowHandleEvent(BlzEvent evt)
        {
            if (evt.GetType() == BlzEvent.EType.MouseMove)
            {
                if (FocusedWindow != null &&
                    FocusedWindow.IsVisible)
                {
                    FocusedWindow.HandleEvent(evt);
                    evt.IsHandled = true;
                }
            }
            else
            {
                if (FocusedWindow != null &&
                    FocusedWindow.IsVisible)
                {
                    FocusedWindow.HandleEvent(evt);
                    return true;
                }
            }

            return false;
        }

        protected virtual Dictionary<String, Object> OnValidate()
        {
            return null;
        }

        public void CallValidated()
        {
            if (Validated == null)
                return;

            ValidateEventArgs e = new ValidateEventArgs(OnValidate());

            if (e.IsValid())
            {
                Validated(this, e);
                Close();
            }
        }

        protected WindowBackground GetBackground()
        {
            return (WindowBackground)this.Background;
        }

        public override void Draw(RenderWindow window)
        {
            if (this.GetBackground().State == WindowBackground.EState.Reduced ||
                this.GetBackground().State == WindowBackground.EState.Reducing ||
                this.GetBackground().State == WindowBackground.EState.Restoring)
            {
                this.Background.Draw(window);

                return;
            }

            base.Draw(window);

            if (FocusedWindow == null)
                return;

            if (FocusedWindow.IsVisible)
                FocusedWindow.Draw(window);
        }

        public void AddItem(Widget widget, UInt32 level = Box.DEFAULT_ITEM_LEVEL, HAlignment hAlignment = WINDOW_ITEM_DEFAULT_ALIGNMENT)
        {
            this.MainBox.AddItem(widget, level, hAlignment);
        }

        public override void Refresh()
        {
            if (this.GetBackground() != null &&
                this.GetBackground().State == WindowBackground.EState.Restored)
                this.Dimension = this.MainBox.BackgroundDimension;

            if (this.Root == null)
                return;

            if (this.Left < 0F)
                this.Left = 0F;
            if (this.Top < 0F)
                this.Top = 0F;
            if (this.BackgroundRight >= this.Root.Window.Width)
                this.BackgroundRight = this.Root.Window.Width - 1F;
            if (this.Top + this.GetBackground().TopBorderHeight >= this.Root.Window.Height)
                this.Top = this.Root.Window.Height - this.GetBackground().TopBorderHeight - 1F;
        }

        protected void CallConfirmationDialogBox(String[] messages, DialogBox.DOnValidate onValidate)
        {
            ConfirmationDialogBox.Instance.InitDialog(messages, onValidate);
            SetFocusedWindow(ConfirmationDialogBox.Instance);
        }

        protected void CallInformationDialogBox(InformationDialogBox.EType type, String[] messages)
        {
            InformationDialogBox.Instance.InitDialog(type, messages);
            SetFocusedWindow(InformationDialogBox.Instance);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            OpeningMode = null;

            if (this.Background != null)
                this.GetBackground().State = WindowBackground.EState.Restored;
        }

        void WindowBackground_Dragged(object sender, DragEventArgs e)
        {
            this.Position = e.DragValue;
        }

        protected override Vector2 GetBasePosition()
        {
            return new Vector2(
                base.GetBasePosition().X + this.Margins * 2F,
                base.GetBasePosition().Y + this.GetBackground().TopBorderHeight + this.Margins);
        }

        protected override Vector2 GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2(this.Margins * 4F, this.GetBackground().TopBorderHeight + this.Margins * 3F);
        }

        public float Margins { get; set; }
    }

    public delegate void ValidateEventHandler(WindowedWidget sender, ValidateEventArgs e);

    public class ValidateEventArgs : EventArgs
    {
        Dictionary<String, Object> Args;

        public Boolean IsValid()
        {
            return Args != null;
        }

        public ValidateEventArgs(Dictionary<String, Object> args)
        {
            Args = args;
        }

        public T GetArg<T>(String key)
        {
            return (T)Args[key];
        }
    }
}
