using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public delegate void WindowedWidgetEventHandler(WindowedWidget sender, WindowedWidgetEventArgs e);
    public class WindowedWidgetEventArgs { }

    public abstract class WindowedWidget : Widget
    {
        public static readonly Color FOCUSED_COLOR = Border.DEFAULT_FOCUSED_AMBIENT_COLOR;//new Color(64, 64, 255);

        public const float BORDER_WIDTH = 8F;

        const HAlignment WINDOW_ITEM_DEFAULT_ALIGNMENT = HAlignment.Left;

        const float POSITION_SIDE_FACTOR = .5F;

        protected WindowedWidget FocusedWindow;
        ValidateEventHandler CurrentValidateEventHandler;

        protected String OpeningMode;

        public event WindowedWidgetEventHandler OnFocusGain;
        bool CallOnFocusGain() { if (OnFocusGain == null) return false; OnFocusGain(this, new WindowedWidgetEventArgs()); return true; }

        public event WindowedWidgetEventHandler OnFocusLoss;
        bool CallOnFocusLoss() { if (OnFocusLoss == null) return false; OnFocusLoss(this, new WindowedWidgetEventArgs()); return true; }

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
            FocusedWindow.Center = new Vector2f(FocusedWindow.Center.X, Center.Y);
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

            BackgroundColor = FOCUSED_COLOR;

            ((EditorBaseWidget)Root).SetWindowOutDrawing(true, FocusedWindow);
            FocusedWindow = null;
        }

        public void SetFocused(bool focused)
        {
            if (focused)
            {
                CallOnFocusGain();
                BackgroundColor = FOCUSED_COLOR;
            }
            else
            {
                CallOnFocusLoss();
                BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;
            }
        }

        protected VAutoSizeBox MainBox { get; set; }

        public event ValidateEventHandler Validated;

        public WindowedWidget(String title, float windowTopBorderHeight = WindowBackground.DEFAULT_TOPBORDER_HEIGHT) :
            base()
        {
            IsBackgroundColorLinked = true;

            Margins = Border.GetWindowBorderWidth();

            Background = new WindowBackground(BackgroundDimension, windowTopBorderHeight);

            GetBackground().SetTitle(title);

            GetBackground().Dragged += new DragEventHandler(WindowBackground_Dragged);
            GetBackground().StateChanged += new StateChangeHandler(WindowedWidget_StateChanged);

            MainBox = new VAutoSizeBox(true, null, Margins);
            MainBox.Position = GetGlobalFromLocal(new Vector2f(0F, 0F));
            AddWidget(MainBox);

            Close();
        }

        void WindowedWidget_StateChanged(WindowBackground sender, StateChangeEventArgs e)
        {
            if (GetBackground().State == WindowBackground.EState.Disabled)
                Close();

            else if (GetBackground().State != WindowBackground.EState.Restored)
                MainBox.Close();
            else
                MainBox.Open();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    switch (evt.Key.Code)
                    {
                        case Keyboard.Key.Return:

                            if (Validated == null)
                                break;

                            CallValidated();

                            return true;

                        case Keyboard.Key.Escape:

                            Close();

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        public override Boolean HandleEvent(BlzEvent evt)
        {
            if (!IsEnabled)
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
            return (WindowBackground)Background;
        }

        public override void Draw(RenderTarget window)
        {
            if (GetBackground().State == WindowBackground.EState.Reduced ||
                GetBackground().State == WindowBackground.EState.Reducing ||
                GetBackground().State == WindowBackground.EState.Restoring)
            {
                Background.Draw(window);

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
            MainBox.AddItem(widget, level, hAlignment);
        }

        public override void Refresh()
        {
            if (GetBackground() != null &&
                GetBackground().State == WindowBackground.EState.Restored)
                Dimension = MainBox.BackgroundDimension;

            if (Root == null)
                return;

            if (Left < 0F)
                Left = 0F;
            if (Top < 0F)
                Top = 0F;
            if (BackgroundRight >= Root.Window.Width)
                BackgroundRight = Root.Window.Width - 1F;
            if (Top + GetBackground().TopBorderHeight >= Root.Window.Height)
                Top = Root.Window.Height - GetBackground().TopBorderHeight - 1F;
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

            if (Background != null)
                GetBackground().State = WindowBackground.EState.Restored;
        }

        void WindowBackground_Dragged(object sender, DragEventArgs e)
        {
            Position = e.DragValue;
        }

        protected override Vector2f GetBasePosition()
        {
            return new Vector2f(
                base.GetBasePosition().X + Margins * 2F,
                base.GetBasePosition().Y + GetBackground().TopBorderHeight + Margins);
        }

        protected override Vector2f GetStructureDimension()
        {
            return base.GetStructureDimension() + new Vector2f(Margins * 4F, GetBackground().TopBorderHeight + Margins * 3F);
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
