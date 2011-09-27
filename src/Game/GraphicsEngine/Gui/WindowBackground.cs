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
                EState oldState = this.State;

                _state = value;

                if (this.State == EState.Restored)
                    this.RefreshButtons();

                if (this.State == oldState)
                    return;

                this.RefreshButtons();

                if (this.StateChanged != null)
                    this.StateChanged(this, new StateChangeEventArgs(this.State));

                this.Refresh();
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
        private Vector2 DragPoint { get; set; }

        public event DragEventHandler Dragged;

        public WindowBackground(Vector2 dimension, float topBorderHeight = DEFAULT_TOPBORDER_HEIGHT) :
            base()
        {
            this.TopBorderHeight = topBorderHeight;

            this.State = EState.Restored;

            this.Dimension = dimension;

            this.UpBorder = new Border(
                new Vector2(
                    this.Dimension.X,
                    this.TopBorderHeight));

            this.DownBorder = new Border(
                new Vector2(
                    this.Dimension.X,
                    this.Dimension.Y));

            // Window close button
            this.CloseBtn = new Button(
                Create.Texture("Gui_CloseButtonN"),
                Create.Texture("Gui_CloseButtonO"));
            this.CloseBtn.Clicked += new ClickEventHandler(CloseBtn_Clicked);
            CloseBtn.IsColorLinked = false;
            
            this.CloseBtn.ClickOffset = BUTTON_CLICK_OFFSET;

            this.AddWidget(this.CloseBtn);

            // window reduce button
            this.ReduceBtn = new Button(
                Create.Texture("Gui_ReduceButtonN"),
                Create.Texture("Gui_ReduceButtonO"));

            this.ReduceBtn.ClickOffset = BUTTON_CLICK_OFFSET;

            this.ReduceBtn.Clicked += new ClickEventHandler(ReduceBtn_Clicked);
            ReduceBtn.IsColorLinked = false;

            this.AddWidget(this.ReduceBtn);

            // window restore button
            this.RestoreBtn = new Button(
                Create.Texture("Gui_restoreButtonN"),
                Create.Texture("Gui_restoreButtonO"));

            this.RestoreBtn.ClickOffset = BUTTON_CLICK_OFFSET;

            this.RestoreBtn.Clicked += new ClickEventHandler(ReduceBtn_Clicked);
            RestoreBtn.IsColorLinked = false;

            this.RestoreBtn.Close();
            this.AddWidget(this.RestoreBtn);
            
            this.IsDragged = false;
        }

        void CloseBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (IsDragged)
                return;

            this.State = EState.Disabled;
        }

        public void SetTitle(String title)
        {
            this.Title = new Label(title);
            Title.Style = LABEL_STYLE;
            Title.IsColorLinked = false;
            this.Title.Center = this.GetGlobalFromLocal(new Vector2(Border.GetWindowBorderWidth() * 2F + this.Title.Halfsize.X, this.TopBorderHeight / 2F));
            this.AddWidget(this.Title);
        }

        void ReduceBtn_Clicked(object sender, MouseButtonEventArgs e)
        {
            if (IsDragged)
                return;

            if (this.State == EState.Restored ||
                this.State == EState.Restoring)
                this.State = EState.Reducing;

            else if (this.State == EState.Reduced ||
                     this.State == EState.Reducing)
                this.State = EState.Restoring;
        }

        private void RefreshButtons()
        {
            if (this.ReduceBtn == null ||
                this.RestoreBtn == null)
                return;

            if (this.State == EState.Restored ||
                this.State == EState.Restoring)
            {
                this.ReduceBtn.Open();
                this.RestoreBtn.Close();
            }

            else if (this.State == EState.Reduced ||
                     this.State == EState.Reducing)
            {
                this.RestoreBtn.Open();
                this.ReduceBtn.Close();
            }

            else
            {
                this.RestoreBtn.Close();
                this.ReduceBtn.Close();
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

                    if (!this.IsDragged)
                        break;

                    this.IsDragged = false;

                    return true;

                case EventType.MouseButtonPressed:

                    if (!this.ContainsMouse())
                        break;

                    if (State != EState.Restored)
                    {
                        if (this.TopBorderContainsMouse())
                        {
                            this.IsDragged = true;
                            this.DragPoint = this.GetLocalFromGlobal(new Vector2(evt.MouseButton.X, evt.MouseButton.Y));

                            return true;
                        }

                        break;
                    }

                    if (this.TopBorderContainsMouse())
                    {
                        this.IsDragged = true;
                        this.DragPoint = this.GetLocalFromGlobal(new Vector2(evt.MouseButton.X, evt.MouseButton.Y));

                        return true;
                    }

                    return true;

                case EventType.MouseButtonReleased:

                    if (evt.MouseButton.Button != MouseButton.Left)
                    {
                        if (this.ContainsMouse())
                            evt.IsHandled = true;

                        break;
                    }

                    if (this.IsDragged)
                        this.IsDragged = false;

                    if (!this.ContainsMouse())
                        break;

                    if (this.TopBorderContainsMouse())
                        return true;

                    if (this.State == EState.Reduced)
                        break;

                    return true;

                case EventType.MouseMoved:

                    if (this.IsDragged)
                    {
                        if (this.Dragged != null)
                        {
                            this.Dragged(this, new DragEventArgs(new Vector2(
                                evt.MouseMove.X - this.DragPoint.X,
                                evt.MouseMove.Y - this.DragPoint.Y)));

                            return true;
                        }
                    }

                    if (!this.ContainsMouse())
                        break;

                    if (this.TopBorderContainsMouse())
                        return true;

                    if (this.State == EState.Reduced)
                        break;

                    return true;

                case EventType.MouseWheelMoved:

                    if (this.ContainsMouse())
                        evt.IsHandled = true;

                    break;
            }

            return base.OnEvent(evt);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (this.State == EState.Reducing)
            {
                this.DownBorder.Resize(new Vector2(1F, 1F - (float)(RESIZE_VELOCITY * dt.Value)));
                if (this.DownBorder.Dimension.Y <= this.UpBorder.Dimension.Y)
                {
                    this.State = EState.Reduced;
                    this.DownBorder.Resize(new Vector2(1F, this.UpBorder.Dimension.Y / this.DownBorder.Dimension.Y));
                }
            }

            else if (this.State == EState.Restoring)
            {
                this.DownBorder.Resize(new Vector2(1F, 1F + (float)(RESIZE_VELOCITY * dt.Value)));
                if (this.DownBorder.Dimension.Y >= this.Dimension.Y)
                {
                    this.State = EState.Restored;
                    this.DownBorder.Resize(new Vector2(1F, this.Dimension.Y / this.DownBorder.Dimension.Y));
                }
            }
        }

        public override void Draw(RenderWindow window)
        {
            this.UpBorder.Draw(window);
            
            base.Draw(window);

            this.DownBorder.Draw(window);
        }

        private Boolean TopBorderContainsMouse()
        {
            return this.GetLocalFromGlobal(new Vector2(this.Root.Window.Input.GetMouseX(), this.Root.Window.Input.GetMouseY())).Y < this.TopBorderHeight &&
                   this.GetLocalFromGlobal(new Vector2(this.Root.Window.Input.GetMouseX(), this.Root.Window.Input.GetMouseY())).Y >= 0F &&
                   this.GetLocalFromGlobal(new Vector2(this.Root.Window.Input.GetMouseX(), this.Root.Window.Input.GetMouseY())).X >= 0F &&
                   this.GetLocalFromGlobal(new Vector2(this.Root.Window.Input.GetMouseX(), this.Root.Window.Input.GetMouseY())).X < this.Dimension.X;
        }

        public override void Refresh()
        {
            if (this.UpBorder == null ||
                this.DownBorder == null)
                return;

            this.UpBorder.Resize(new Vector2(
                this.Dimension.X / this.UpBorder.Dimension.X,
                this.TopBorderHeight / this.UpBorder.Dimension.Y));

            if (this.State == EState.Restored)
                this.DownBorder.Resize(new Vector2(
                    this.Dimension.X / this.DownBorder.Dimension.X,
                    this.Dimension.Y / this.DownBorder.Dimension.Y));

            this.CloseBtn.Dimension = new Vector2(
                this.TopBorderHeight * BUTTON_SCALE_FACTOR,
                this.TopBorderHeight * BUTTON_SCALE_FACTOR);

            this.CloseBtn.Position = this.GetGlobalFromLocal(new Vector2(
                this.Dimension.X - Border.GetWindowBorderWidth() * 2F - this.CloseBtn.Dimension.X,
                this.TopBorderHeight / 2F - this.CloseBtn.Halfsize.Y));

            this.ReduceBtn.Dimension = new Vector2(
                this.TopBorderHeight * BUTTON_SCALE_FACTOR,
                this.TopBorderHeight * BUTTON_SCALE_FACTOR);

            this.ReduceBtn.Position = this.GetGlobalFromLocal(new Vector2(
                this.Dimension.X - this.CloseBtn.Dimension.X * 2 - Border.GetWindowBorderWidth() * 2F,
                this.TopBorderHeight / 2F - this.ReduceBtn.Halfsize.Y));

            this.RestoreBtn.Dimension = new Vector2(
                this.TopBorderHeight * BUTTON_SCALE_FACTOR,
                this.TopBorderHeight * BUTTON_SCALE_FACTOR);

            this.RestoreBtn.Position = this.GetGlobalFromLocal(new Vector2(
                this.Dimension.X - this.CloseBtn.Dimension.X * 2 - Border.GetWindowBorderWidth() * 2F,
                this.TopBorderHeight / 2F - this.RestoreBtn.Halfsize.Y));

            this.UpBorder.Move(this.Position - this.UpBorder.Position);

            this.DownBorder.Move(this.Position - this.DownBorder.Position);
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

                this.UpBorder.SetColor(this.Color);
                this.DownBorder.SetColor(this.Color);
            }
        }
    }

    public delegate void DragEventHandler(object sender, DragEventArgs e);

    public class DragEventArgs : EventArgs
    {
        public DragEventArgs(Vector2 dragValue)
        {
            this.DragValue = dragValue;
        }

        public Vector2 DragValue { get; set; }
    }

    public delegate void StateChangeHandler(WindowBackground sender, StateChangeEventArgs e);

    public class StateChangeEventArgs : EventArgs
    {
        public WindowBackground.EState State { get; private set; }

        public StateChangeEventArgs(WindowBackground.EState state)
        {
            this.State = state;
        }
    }
}