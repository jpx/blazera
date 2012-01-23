using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class WindowBackground : Widget
    {
        public enum EState
        {
            Restored,
            Reducing,
            Reduced,
            Restoring,
            Disabled
        }

        private EState _state;
        public EState State
        {
            get { return _state; }
            set
            {
                EState oldState = State;

                _state = value;

                if (State == EState.Restored)
                    RefreshButtons();

                if (State == oldState)
                    return;

                RefreshButtons();

                if (StateChanged != null)
                    StateChanged(this, new StateChangeEventArgs(State));

                Refresh();
            }
        }

        public const float DEFAULT_TOPBORDER_HEIGHT = 40F;
        private const float BUTTON_CLICK_OFFSET = 4F;
        const float BUTTON_SCALE_FACTOR = .5F;
        private const float RESIZE_VELOCITY = 10F;

        const Text.Styles LABEL_STYLE = Text.Styles.Bold;

        public float TopBorderHeight { get; set; }

        private Border UpBorder { get; set; }
        private Border DownBorder { get; set; }

        private Label Title { get; set; }

        public Button CloseBtn { get; set; }
        public Button ReduceBtn { get; set; }
        public Button RestoreBtn { get; set; }

        private Boolean IsDragged { get; set; }
        private Vector2f DragPoint { get; set; }

        public event DragEventHandler Dragged;

        public WindowBackground(Vector2f dimension, float topBorderHeight = DEFAULT_TOPBORDER_HEIGHT) :
            base()
        {
            TopBorderHeight = topBorderHeight;

            State = EState.Restored;

            Dimension = dimension;

            UpBorder = new Border(
                new Vector2f(
                    Dimension.X,
                    TopBorderHeight));

            DownBorder = new Border(
                new Vector2f(
                    Dimension.X,
                    Dimension.Y));

            // Window close button
            CloseBtn = new Button(
                Create.Texture("Gui_CloseButtonN"),
                Create.Texture("Gui_CloseButtonO"));
            CloseBtn.Clicked += new ClickEventHandler(CloseBtn_Clicked);
            CloseBtn.IsColorLinked = false;
            
            CloseBtn.ClickOffset = BUTTON_CLICK_OFFSET;

            AddWidget(CloseBtn);

            // window reduce button
            ReduceBtn = new Button(
                Create.Texture("Gui_ReduceButtonN"),
                Create.Texture("Gui_ReduceButtonO"));

            ReduceBtn.ClickOffset = BUTTON_CLICK_OFFSET;

            ReduceBtn.Clicked += new ClickEventHandler(ReduceBtn_Clicked);
            ReduceBtn.IsColorLinked = false;

            AddWidget(ReduceBtn);

            // window restore button
            RestoreBtn = new Button(
                Create.Texture("Gui_restoreButtonN"),
                Create.Texture("Gui_restoreButtonO"));

            RestoreBtn.ClickOffset = BUTTON_CLICK_OFFSET;

            RestoreBtn.Clicked += new ClickEventHandler(ReduceBtn_Clicked);
            RestoreBtn.IsColorLinked = false;

            RestoreBtn.Close();
            AddWidget(RestoreBtn);
            
            IsDragged = false;
        }

        void CloseBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (IsDragged)
                return;

            State = EState.Disabled;
        }

        public void SetTitle(String title)
        {
            Title = new Label(title);
            Title.Style = LABEL_STYLE;
            Title.IsColorLinked = false;
            Title.Center = GetGlobalFromLocal(new Vector2f(Border.GetWindowBorderWidth() * 2F + Title.Halfsize.X, TopBorderHeight / 2F));
            AddWidget(Title);
        }

        void ReduceBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (IsDragged)
                return;

            if (State == EState.Restored ||
                State == EState.Restoring)
                State = EState.Reducing;

            else if (State == EState.Reduced ||
                     State == EState.Reducing)
                State = EState.Restoring;
        }

        private void RefreshButtons()
        {
            if (ReduceBtn == null ||
                RestoreBtn == null)
                return;

            if (State == EState.Restored ||
                State == EState.Restoring)
            {
                ReduceBtn.Open();
                RestoreBtn.Close();
            }

            else if (State == EState.Reduced ||
                     State == EState.Reducing)
            {
                RestoreBtn.Open();
                ReduceBtn.Close();
            }

            else
            {
                RestoreBtn.Close();
                ReduceBtn.Close();
            }
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
            {
                return base.OnEvent(evt);
            }

            switch (evt.Type)
            {
                case EventType.MouseLeft:

                    evt.IsHandled = true;

                    if (!IsDragged)
                        break;

                    IsDragged = false;

                    return true;

                case EventType.MouseButtonPressed:

                    if (!ContainsMouse())
                        break;

                    if (State != EState.Restored)
                    {
                        if (TopBorderContainsMouse())
                        {
                            IsDragged = true;
                            DragPoint = GetLocalFromGlobal(new Vector2f(evt.MouseButton.X, evt.MouseButton.Y));

                            return true;
                        }

                        break;
                    }

                    if (TopBorderContainsMouse())
                    {
                        IsDragged = true;
                        DragPoint = GetLocalFromGlobal(new Vector2f(evt.MouseButton.X, evt.MouseButton.Y));

                        return true;
                    }

                    return true;

                case EventType.MouseButtonReleased:

                    if (evt.MouseButton.Button != Mouse.Button.Left)
                    {
                        if (ContainsMouse())
                            evt.IsHandled = true;

                        break;
                    }

                    if (IsDragged)
                        IsDragged = false;

                    if (!ContainsMouse())
                        break;

                    if (TopBorderContainsMouse())
                        return true;

                    if (State == EState.Reduced)
                        break;

                    return true;

                case EventType.MouseMoved:

                    if (IsDragged)
                    {
                        if (Dragged != null)
                        {
                            Dragged(this, new DragEventArgs(new Vector2f(
                                evt.MouseMove.X - DragPoint.X,
                                evt.MouseMove.Y - DragPoint.Y)));

                            return true;
                        }
                    }

                    if (!ContainsMouse())
                        break;

                    if (TopBorderContainsMouse())
                        return true;

                    if (State == EState.Reduced)
                        break;

                    return true;

                case EventType.MouseWheelMoved:

                    if (ContainsMouse())
                        evt.IsHandled = true;

                    break;
            }

            return base.OnEvent(evt);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (State == EState.Reducing)
            {
                DownBorder.Resize(new Vector2f(1F, 1F - (float)(RESIZE_VELOCITY * dt.Value)));
                if (DownBorder.Dimension.Y <= UpBorder.Dimension.Y)
                {
                    State = EState.Reduced;
                    DownBorder.Resize(new Vector2f(1F, UpBorder.Dimension.Y / DownBorder.Dimension.Y));
                }
            }

            else if (State == EState.Restoring)
            {
                DownBorder.Resize(new Vector2f(1F, 1F + (float)(RESIZE_VELOCITY * dt.Value)));
                if (DownBorder.Dimension.Y >= Dimension.Y)
                {
                    State = EState.Restored;
                    DownBorder.Resize(new Vector2f(1F, Dimension.Y / DownBorder.Dimension.Y));
                }
            }
        }

        public override void Draw(RenderTarget window)
        {
            UpBorder.Draw(window);
            
            base.Draw(window);

            DownBorder.Draw(window);
        }

        private Boolean TopBorderContainsMouse()
        {
            return GetLocalFromGlobal(new Vector2f(Mouse.GetPosition(Root.Window).X, Mouse.GetPosition(Root.Window).Y)).Y < TopBorderHeight &&
                   GetLocalFromGlobal(new Vector2f(Mouse.GetPosition(Root.Window).X, Mouse.GetPosition(Root.Window).Y)).Y >= 0F &&
                   GetLocalFromGlobal(new Vector2f(Mouse.GetPosition(Root.Window).X, Mouse.GetPosition(Root.Window).Y)).X >= 0F &&
                   GetLocalFromGlobal(new Vector2f(Mouse.GetPosition(Root.Window).X, Mouse.GetPosition(Root.Window).Y)).X < Dimension.X;
        }

        public override void Refresh()
        {
            if (UpBorder == null ||
                DownBorder == null)
                return;

            UpBorder.Resize(new Vector2f(
                Dimension.X / UpBorder.Dimension.X,
                TopBorderHeight / UpBorder.Dimension.Y));

            if (State == EState.Restored)
                DownBorder.Resize(new Vector2f(
                    Dimension.X / DownBorder.Dimension.X,
                    Dimension.Y / DownBorder.Dimension.Y));

            CloseBtn.Dimension = new Vector2f(
                TopBorderHeight * BUTTON_SCALE_FACTOR,
                TopBorderHeight * BUTTON_SCALE_FACTOR);

            CloseBtn.Position = GetGlobalFromLocal(new Vector2f(
                Dimension.X - Border.GetWindowBorderWidth() * 2F - CloseBtn.Dimension.X,
                TopBorderHeight / 2F - CloseBtn.Halfsize.Y));

            ReduceBtn.Dimension = new Vector2f(
                TopBorderHeight * BUTTON_SCALE_FACTOR,
                TopBorderHeight * BUTTON_SCALE_FACTOR);

            ReduceBtn.Position = GetGlobalFromLocal(new Vector2f(
                Dimension.X - CloseBtn.Dimension.X * 2 - Border.GetWindowBorderWidth() * 2F,
                TopBorderHeight / 2F - ReduceBtn.Halfsize.Y));

            RestoreBtn.Dimension = new Vector2f(
                TopBorderHeight * BUTTON_SCALE_FACTOR,
                TopBorderHeight * BUTTON_SCALE_FACTOR);

            RestoreBtn.Position = GetGlobalFromLocal(new Vector2f(
                Dimension.X - CloseBtn.Dimension.X * 2 - Border.GetWindowBorderWidth() * 2F,
                TopBorderHeight / 2F - RestoreBtn.Halfsize.Y));

            UpBorder.Move(Position - UpBorder.Position);

            DownBorder.Move(Position - DownBorder.Position);
        }

        public event StateChangeHandler StateChanged;

        public override Color Color
        {
            get
            {
                return base.Color;
            }
            set
            {
                base.Color = value;

                UpBorder.SetColor(Color);
                DownBorder.SetColor(Color);
            }
        }
    }

    public delegate void DragEventHandler(object sender, DragEventArgs e);

    public class DragEventArgs : EventArgs
    {
        public DragEventArgs(Vector2f dragValue)
        {
            DragValue = dragValue;
        }

        public Vector2f DragValue { get; set; }
    }

    public delegate void StateChangeHandler(WindowBackground sender, StateChangeEventArgs e);

    public class StateChangeEventArgs : EventArgs
    {
        public WindowBackground.EState State { get; private set; }

        public StateChangeEventArgs(WindowBackground.EState state)
        {
            State = state;
        }
    }
}