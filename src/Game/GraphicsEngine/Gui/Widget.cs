using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public enum Alignment
    {
        Vertical,
        Horizontal
    }

    #region BaseWidget

    public abstract class BaseWidget : Widget
    {
        public RenderWindow Window { get; private set; }
        public View GuiView { get; private set; }

        public BaseWidget(RenderWindow window, View guiView) :
            base()
        {
            GuiView = guiView;

            Root = this;
            Window = window;

            Dimension = new Vector2(GameDatas.WINDOW_WIDTH, GameDatas.WINDOW_HEIGHT);
        }

        public Vector2 GetViewPos()
        {
            return this.GuiView.Center - this.GuiView.Size / 2F;
        }
    }

    public class EditorBaseWidget : BaseWidget
    {
        private List<WindowedWidget> Windows { get; set; }
        Dictionary<WindowedWidget, Boolean> DrawnWindows = new Dictionary<WindowedWidget, Boolean>();

        private List<WindowedWidget> TabWindowList = new List<WindowedWidget>();

        private WindowedWidget _focusedWindow;
        private WindowedWidget FocusedWindow
        {
            get { return _focusedWindow; }
            set
            {
                _focusedWindow = value;

                if (FocusedWindow == null)
                    return;

                FocusedWindow.Closed += new CloseEventHandler(FocusedWindow_Closed);
            }
        }

        void FocusedWindow_Closed(Widget sender, CloseEventArgs e)
        {
            if (FocusedWindow == null)
                return;

            FocusedWindow.Closed -= new CloseEventHandler(FocusedWindow_Closed);
            FocusedWindow = null;
        }

        private Dictionary<KeyCode, WindowedWidget> KeyWindowBinding = new Dictionary<KeyCode, WindowedWidget>();

        public EditorBaseWidget(RenderWindow window, View guiView) :
            base(window, guiView)
        {
            this.Windows = new List<WindowedWidget>();
        }

        public void AddKeyWindowBind(KeyCode keyCode, WindowedWidget window)
        {
            if (!KeyWindowBinding.ContainsKey(keyCode))
                KeyWindowBinding.Add(keyCode, window);
            else
                KeyWindowBinding[keyCode] = window;
        }

        bool SetCurrentWindowFromKey(KeyCode keyCode)
        {
            if (!KeyWindowBinding.ContainsKey(keyCode))
                return false;

            WindowedWidget window = KeyWindowBinding[keyCode];

            if (this.FocusedWindow != window &&
                window.IsVisible)
            {
                this.SetCurrentWindow(window);
                return true;
            }

            window.SwitchState();
            if (window.IsVisible)
                this.SetCurrentWindow(window);

            return true;
        }

        private WindowedWidget GetNextOpenedWindow(WindowedWidget window)
        {
            if (window == null &&
                this.TabWindowList.Count > 0)
                return this.TabWindowList[0];

            Int32 count = 0;
            Int32 index = this.TabWindowList.IndexOf(window);

            while (count < this.TabWindowList.Count)
            {
                if (index < this.TabWindowList.Count - 1)
                    ++index;
                else
                    index = 0;

                ++count;

                if (this.TabWindowList[index].IsVisible)
                    return this.TabWindowList[index];
            }

            return null;
        }

        public override void Draw(RenderWindow window)
        {
            if (this.Background != null)
                this.Background.Draw(window);

            foreach (Widget widget in this.DrawingWidgets)
            {
                if (!widget.IsVisible)
                    continue;

                if (widget is WindowedWidget)
                {
                    if (DrawnWindows[(WindowedWidget)widget])
                        widget.Draw(window);

                    continue;
                }

                widget.Draw(window);
            }
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.MouseButtonPressed:

                    if (evt.MouseButton.Button != MouseButton.Left)
                        break;

                    if (FocusedWindow == null)
                        break;

                    FocusedWindow.BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;
                    FocusedWindow.Closed -= new CloseEventHandler(FocusedWindow_Closed);
                    FocusedWindow = null;

                    return true;
            }

            return base.OnEvent(evt);
        }

        public override Boolean OnPredominantEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                case EventType.KeyPressed:

                    if (this.SetCurrentWindowFromKey(evt.Key.Code))
                        return true;

                    switch (evt.Key.Code)
                    {
                        case KeyCode.Tab:

                            WindowedWidget nextOpenedWindow = this.GetNextOpenedWindow(this.FocusedWindow);

                            if (nextOpenedWindow == null)
                                break;

                            this.SetCurrentWindow(nextOpenedWindow);

                            return true;
                    }

                    break;
            }

            return base.OnPredominantEvent(evt);
        }

        public override Boolean HandleEvent(BlzEvent evt)
        {
            if (!this.IsEnabled)
                return false;

            if (evt.GetType() == BlzEvent.EType.MouseMove)
            {
                if (this.Focused != null)
                {
                    this.Focused.HandleEvent(evt);
                    evt.IsHandled = true;
                }

                foreach (WindowedWidget window in this.Windows)
                    if (window.HandleEvent(evt))
                    {
                        evt.IsHandled = true;
                    }

                if (OnEvent(evt))
                    evt.IsHandled = true;

                return evt.IsHandled;
            }
            else if (evt.GetType() == BlzEvent.EType.Key)
            {
                if (FocusedWindow != null)
                {
                    if (!FocusedWindow.HandleEvent(evt))
                        OnPredominantEvent(evt);
                    return true;
                }

                return OnPredominantEvent(evt) || OnEvent(evt);
            }

            if (this.Focused != null)
            {
                this.Focused.HandleEvent(evt);
                return true;
            }

            foreach (WindowedWidget window in this.Windows)
                if (window.HandleEvent(evt))
                {
                    this.SetCurrentWindow(window);
                    return true;
                }

            return this.OnEvent(evt);
        }

        public void SetCurrentWindow(WindowedWidget window)
        {
            if (!this.RemoveWindow(window))
                return;

            if (this.FocusedWindow != null)
            {
                this.FocusedWindow.BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;
                FocusedWindow.Closed -= new CloseEventHandler(FocusedWindow_Closed);
            }

            this.FocusedWindow = window;

            this.AddFirst(window);
            if (!window.GotFocusedWindow() ||
                (window.GotFocusedWindow() && !window.GotFocusedWindow(true)))
                window.BackgroundColor = WindowedWidget.FOCUSED_COLOR;
            this.Windows.Insert(0, window);

            DrawnWindows.Add(window, true);
        }

        public void SetWindowOutDrawing(Boolean drawn, WindowedWidget window)
        {
            DrawnWindows[window] = drawn;
        }

        public void AddWindow(WindowedWidget window, Boolean opened = false)
        {
            this.TabWindowList.Add(window);

            DrawnWindows.Add(window, true);

            window.BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;

            this.AddWidget(window);
            this.Windows.Add(window);

            if (opened)
                window.Open();
        }

        public Boolean RemoveWindow(WindowedWidget window)
        {
            if (!this.RemoveWidget(window))
                return false;
            return
                this.Windows.Remove(window) &&
                DrawnWindows.Remove(window);
        }

        

        public Vector2 GetMousePosition()
        {
            return this.GetGlobalFromLocal(new Vector2(
                this.Window.Input.GetMouseX(),
                this.Window.Input.GetMouseY()));
        }

        public override void Refresh() { }
    }

    #endregion

    #region GameBaseWidget

    /// <summary>
    /// Provides info when a view event is called
    /// </summary>
    public class GameViewEventArgs : EventArgs
    {
        /// <summary>
        /// Move offset of the view
        /// </summary>
        public Vector2 Offset { get; private set; }

        /// <summary>
        /// Position of the view after moving
        /// </summary>
        public Vector2 Position { get; private set; }

        /// <summary>
        /// Dimension of the view
        /// </summary>
        public Vector2 Dimension { get; private set; }

        /// <summary>
        /// Creates game view info move with a given offset, view center and view size
        /// </summary>
        /// <param name="offset">View move offset</param>
        /// <param name="viewCenter">View center after moving</param>
        /// <param name="viewSize">View dimension</param>
        public GameViewEventArgs(Vector2 offset, Vector2 viewCenter, Vector2 viewSize)
        {
            Offset = offset;
            Dimension = viewSize;
            Position = viewCenter - Dimension / 2F;
        }
    }

    public delegate void GameViewEventHandler(View view, GameViewEventArgs e);

    public class GameBaseWidget : BaseWidget
    {
        public View MapView { get; set; }
        List<GameWidget> GameWidgets;

        public event GameViewEventHandler OnGameViewMove;
        public Boolean CallOnGameViewMove(Vector2 offset) { if (OnGameViewMove == null) return false; OnGameViewMove(MapView, new GameViewEventArgs(offset, MapView.Center, MapView.Size)); return true; }

        public GameBaseWidget(RenderWindow window, View mapView, View gameGuiView) :
            base(window, gameGuiView)
        {
            MapView = mapView;
            GameWidgets = new List<GameWidget>();

            OnGameViewMove += new GameViewEventHandler(GameBaseWidget_OnGameViewMove);
        }

        public void MoveGameView(Vector2 offset)
        {
            MapView.Move(offset);

            CallOnGameViewMove(offset);
        }

        public void AddGameWidget(GameWidget gameWidget)
        {
            GameWidgets.Add(gameWidget);

            AddWidget(gameWidget);
        }

        public Boolean RemoveGameWidget(GameWidget gameWidget)
        {
            if (!GameWidgets.Remove(gameWidget))
                return false;

            return RemoveWidget(gameWidget);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            while (!SpeechManager.Instance.IsEmpty())
            {
                SpeechBubble currentSpeechBubble = SpeechManager.Instance.GetNext();
                currentSpeechBubble.OnClosing += new ClosingEventHandler(currentSpeechBubble_OnClosing);
                AddGameWidget(currentSpeechBubble);
                currentSpeechBubble.LaunchSpeech();
            }

            foreach (GameWidget gameWidget in GameWidgets)
                gameWidget.Update(dt);
        }

        void GameBaseWidget_OnGameViewMove(View view, GameViewEventArgs e)
        {
            foreach (GameWidget gameWidget in GameWidgets)
                if (gameWidget is SpeechBubble)
                    gameWidget.Refresh();
        }

        void currentSpeechBubble_OnClosing(SpeechBubble sender, ClosingEventArgs e)
        {
            RemoveGameWidget(sender);
        }

        public override void Draw(RenderWindow window)
        {
            window.SetView(GuiView);

            base.Draw(window);

            for (int count = GameWidgets.Count - 1; count >= 0; --count)
                if (GameWidgets[count].IsVisible)
                    GameWidgets[count].Draw(window);

            window.SetView(window.DefaultView);
        }

        public override Boolean HandleEvent(BlzEvent evt)
        {
            foreach (GameWidget gameWidget in GameWidgets)
                if (gameWidget.IsEnabled)
                    if (gameWidget.HandleEvent(evt))
                        return true;

            return OnEvent(evt);
        }

        public void SetFirst(GameWidget gameWidget)
        {
            if (!GameWidgets.Remove(gameWidget))
                throw new Exception("Game widget is not in the collection.");

            GameWidgets.Insert(0, gameWidget);
        }

        public override BaseWidget Root
        {
            protected set { base.Root = this; }
        }
    }

    #endregion

    public abstract class Widget : BaseDrawable, IUpdateable
    {
        public String Name { get; set; }

        public Widget Parent { get; protected set; }

        protected List<Widget> Widgets { get; set; } // update & properties list
        protected List<Widget> Items { get; set; } // event list
        protected LinkedList<Widget> DrawingWidgets { get; set; } // draw list

        private Widget _focused;
        public Widget Focused
        {
            get { return _focused; }
            set
            {
                _focused = value;

                if (this.Focused == null &&
                    this.Parent != null)
                {
                    this.Parent.Focused = null;
                    return;
                }

                if (this.Parent == null)
                    return;

                this.Parent.Focused = this;
            }
        }

        public event ChangeEventHandler Changed;
        public event OpenEventHandler Opened;
        public event CloseEventHandler Closed;

        public Boolean IsPositionLinked { get; set; }
        public Boolean IsDimensionLinked { get; set; }
        public Boolean IsColorLinked { get; set; }
        public Boolean IsBackgroundColorLinked { get; set; }

        public static readonly Color SEAL_COLOR = new Color(108, 123, 139, 196);
        public Boolean IsSealed { get; private set; }

        protected RefreshInfo RefreshInfo { get; set; }

        public Widget()
        {
            this.Widgets = new List<Widget>();
            this.Items = new List<Widget>();
            this.DrawingWidgets = new LinkedList<Widget>();

            this.IsPositionLinked = true;
            this.IsDimensionLinked = false;

            this.IsColorLinked = true;
            this.IsBackgroundColorLinked = false;

            IsSealed = false;

            this.Dimension = new Vector2(1F, 1F);

            this.RefreshInfo = new RefreshInfo();

            this.Changed += new ChangeEventHandler(Widget_Changed);

            this.Open();
        }

        Color OldBackgroundColor;
        public void Seal(Boolean isSealed = true, Boolean changeColor = true)
        {
            IsSealed = isSealed;

            if (IsSealed)
            {
                Disable();

                if (changeColor)
                    OldBackgroundColor = BackgroundColor;

                Color = SEAL_COLOR;
                BackgroundColor = SEAL_COLOR;
            }
            else
            {
                if (IsVisible)
                    IsEnabled = true;

                Color = Color.White;
                BackgroundColor = OldBackgroundColor;
            }
        }

        void Widget_Changed(object sender, ChangeEventArgs e)
        {
            switch (e.Type)
            {
                case ChangeEventArgs.EType.Dimension: this.RefreshInfo.SetDimensionScaleRefresh(((DimensionChangeEventArgs)e).Scale); break;
                case ChangeEventArgs.EType.Position: this.RefreshInfo.SetPositionOffsetRefresh(((PositionChangeEventArgs)e).Offset); break;
                case ChangeEventArgs.EType.Widget: this.RefreshInfo.SetWidgetAddedRefresh(((WidgetAddedChangeEventArgs)e).Widget); break;
                case ChangeEventArgs.EType.Text: this.RefreshInfo.SetTextChangeRefresh(((TextChangeEventArgs)e).Text); break;
            }
        }

        protected void CallChanged(ChangeEventArgs e)
        {
            if (this.Changed != null)
                this.Changed(this, e);
        }

        public virtual void Init()
        {
            foreach (Widget widget in this.Widgets)
                widget.Init();

            this.Refresh();

            if (this.Parent != null)
                this.Parent.Refresh();
        }

        public virtual void Update(Time dt)
        {
            if (this.RefreshInfo.IsRefreshed)
            {
                this.Refresh();

                if (this.Parent != null)
                    this.Parent.Refresh();

                this.RefreshInfo.Reset();
            }

            foreach (Widget widget in this.Widgets)
                widget.Update(dt);
        }

        public override void Draw(RenderWindow window)
        {
            if (this.Background != null)
                this.Background.Draw(window);

            foreach (Widget widget in this.DrawingWidgets)
                if (widget.IsVisible)
                    widget.Draw(window);
        }

        public virtual Boolean OnEvent(BlzEvent evt)
        {
            return false;
        }

        public virtual Boolean OnPredominantEvent(BlzEvent evt)
        {
            return false;
        }

        protected virtual Boolean FocusedHandleEvent(BlzEvent evt)
        {
            if (Focused == null)
                return false;

            if (evt.GetType() == BlzEvent.EType.MouseMove)
            {
                this.Focused.HandleEvent(evt);
                evt.IsHandled = true;

                return false;
            }

            this.Focused.HandleEvent(evt);
            this.OnPredominantEvent(evt);

            return true;
        }

        protected virtual Boolean ItemHandleEvent(BlzEvent evt)
        {
            if (evt.GetType() == BlzEvent.EType.MouseMove)
            {
                foreach (Widget widget in this.Items)
                    if (widget.HandleEvent(evt))
                        evt.IsHandled = true;

                return false;
            }

            foreach (Widget widget in this.Items)
                if (widget.HandleEvent(evt))
                    return true;

            return false;
        }

        protected virtual Boolean BackgroundHandleEvent(BlzEvent evt)
        {
            if (Background == null)
                return false;

            if (evt.GetType() == BlzEvent.EType.MouseMove)
            {
                if (Background.HandleEvent(evt))
                    evt.IsHandled = true;

                return false;
            }

            return Background.HandleEvent(evt);
        }

        public virtual Boolean HandleEvent(BlzEvent evt)
        {
            if (!this.IsEnabled)
                return false;

            if (FocusedHandleEvent(evt))
                return true;

            if (ItemHandleEvent(evt))
                return true;

            if (BackgroundHandleEvent(evt))
                return true;

            if (evt.GetType() != BlzEvent.EType.MouseMove)
                return OnEvent(evt);

            evt.IsHandled |= OnEvent(evt);

            return evt.IsHandled;
        }

        public virtual void AddWidget(Widget widget, Boolean addToDrawing = true, Boolean addToEvent = true)
        {
            widget.SetParent(this);

            widget.Changed += new ChangeEventHandler(Widget_Changed);

            this.Widgets.Add(widget);

            if (addToEvent)
                this.Items.Add(widget);

            if (addToDrawing)
                this.DrawingWidgets.AddFirst(widget);

            this.CallChanged(new WidgetAddedChangeEventArgs(widget));
        }

        public void InsertWidget(Int32 index, Widget widget, Boolean addToDrawing = true, Boolean addToEvent = true)
        {
            widget.SetParent(this);

            widget.Changed += new ChangeEventHandler(Widget_Changed);

            this.Widgets.Insert(index, widget);

            if (addToEvent)
                this.Items.Insert(index, widget);

            if (addToDrawing)
                this.DrawingWidgets.ToList().Insert(DrawingWidgets.Count - 1 - index, widget);

            this.CallChanged(new WidgetAddedChangeEventArgs(widget));
        }

        public void AddFirst(Widget widget, Boolean addToDrawing = true)
        {
            widget.SetParent(this);

            widget.Changed += new ChangeEventHandler(Widget_Changed);

            this.Widgets.Insert(0, widget);
            this.Items.Insert(0, widget);

            if (addToDrawing)
                this.DrawingWidgets.AddLast(widget);

            this.CallChanged(new WidgetAddedChangeEventArgs(widget));
        }

        public void SetParent(Widget parent)
        {
            this.Root = parent.Root;

            this.Parent = parent;
        }

        public Boolean RemoveWidget(Widget widget)
        {
            if (this.Background != null &&
                widget.Equals(this.Background))
            {
                this.Background = null;

                return this.Widgets.Remove(widget);
            }

            this.CallChanged(new WidgetAddedChangeEventArgs(widget));

            this.DrawingWidgets.Remove(widget);

            return
                this.Widgets.Remove(widget) &&
                this.Items.Remove(widget);
        }

        public Widget GetWidget(String name)
        {
            foreach (Widget w in this.Widgets)
                if (w.Name == name)
                    return w;

            return null;
        }

        public void SetWidget(Widget widget, Widget newWidget)
        {
            if (!Widgets.Contains(widget))
                return;

            RemoveWidget(widget);
            AddWidget(newWidget);
        }

        protected virtual String GetInfoTip()
        {
            return null;
        }

        public Vector2 GetLocalFromGlobal(Vector2 point)
        {
            if (this.Root == null)
                return point - GetBasePosition();

            return point - GetBasePosition() - Root.GetViewPos();
        }

        public Vector2 GetGlobalFromLocal(Vector2 point)
        {
            if (this.Root == null)
                return point + GetBasePosition();

            return Root.GetViewPos() + point + GetBasePosition();
        }

        private Widget _background;
        public Widget Background
        {
            get { return _background; }
            protected set
            {
                _background = value;

                if (this.Background == null)
                    return;

                this.Background.SetParent(this);

                this.Dimension = this.BackgroundDimension;

                this.Widgets.Add(this.Background);
            }
        }

        private Vector2 _position;
        public override Vector2 Position
        {
            get { return _position; }
            set
            {
                Vector2 offset = value - this.Position;

                _position = value;

                foreach (Widget widget in this.Widgets)
                    if (widget.IsPositionLinked)
                        widget.Position += offset;

                this.CallChanged(new PositionChangeEventArgs(offset));
            }
        }

        private Vector2 _dimension;
        public override Vector2 Dimension
        {
            get
            {
                return _dimension;
            }
            set
            {
                if (this.Background != null && this.Dimension.X == 0F || this.Dimension.Y == 0F)
                {
                    _dimension = value;

                    this.BackgroundDimension = this.Dimension;

                    return;
                }

                Vector2 factor = new Vector2(1F, 1F);

                factor = new Vector2(
                    value.X / this.Dimension.X,
                    value.Y / this.Dimension.Y);

                _dimension = value;

                foreach (Widget widget in this.Items)
                    if (widget.IsDimensionLinked)
                        widget.Dimension = new Vector2(
                            widget.Dimension.X * factor.X,
                            widget.Dimension.Y * factor.Y);

                if (this.Background != null)
                    this.BackgroundDimension = this.Dimension + this.GetStructureDimension();

                this.CallChanged(new DimensionChangeEventArgs(factor));
            }
        }

        protected virtual Vector2 GetBasePosition()
        {
            return this.Position;
        }

        public virtual Vector2 BackgroundDimension
        {
            get
            {
                if (this.Background != null)
                    return this.Background.Dimension;

                return this.Dimension;
            }
            set
            {
                _dimension = value;

                if (this.Background == null)
                    return;

                this.Background.Dimension = value;

                _dimension = value - this.GetStructureDimension();
            }
        }

        protected virtual Vector2 GetStructureDimension()
        {
            return new Vector2(0F, 0F);
        }

        public float BackgroundRight
        {
            get { return this.Position.X + this.BackgroundDimension.X; }
            set { this.Position = new Vector2(value - this.BackgroundDimension.X, this.Position.Y); }
        }

        public float BackgroundBottom
        {
            get { return this.Position.Y + this.BackgroundDimension.Y; }
            set { this.Position = new Vector2(this.Position.X, value - this.BackgroundDimension.Y); }
        }

        public Vector2 BackgroundHalfsize
        {
            get { return this.BackgroundDimension / 2F; }
            set { this.BackgroundDimension = value * 2F; }
        }

        protected Boolean Contains(float x, float y, float offset = 0F)
        {
            if (offset >= this.Halfsize.X ||
                offset >= this.Halfsize.Y)
                return false;

            Vector2 basePosition = GetBasePosition();

            return (x >= basePosition.X + offset &&
                    x < basePosition.X + Dimension.X - offset &&
                    y >=  basePosition.Y + offset &&
                    y < basePosition.Y + Dimension.Y - offset);
        }

        public Boolean ContainsMouse(float offset = 0F)
        {
            return this.Contains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        protected Boolean BackgroundContains(float x, float y, float offset = 0F)
        {
            if (offset >= this.BackgroundHalfsize.X ||
                offset >= this.BackgroundHalfsize.Y)
                return false;

            return (x >= this.Left + offset &&
                    x < this.BackgroundRight - offset &&
                    y >= this.Top + offset &&
                    y < this.BackgroundBottom - offset);
        }

        public Boolean BackgroundContainsMouse(float offset = 0F)
        {
            return this.BackgroundContains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        protected Boolean Contains(float x, float y, FloatRect offset)
        {
            if (offset.HSum >= this.Halfsize.X ||
                offset.VSum >= this.Halfsize.Y)
                return false;

            Vector2 basePosition = GetBasePosition();

            return (x >= basePosition.X + offset.Left &&
                    x < basePosition.X + Dimension.X - offset.Right &&
                    y >= basePosition.Y + offset.Top &&
                    y < basePosition.Y + Dimension.Y - offset.Bottom);
        }

        public Boolean ContainsMouse(FloatRect offset)
        {
            return this.Contains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        protected Boolean BackgroundContains(float x, float y, FloatRect offset)
        {
            if (offset.HSum >= this.BackgroundHalfsize.X ||
                offset.VSum >= this.BackgroundHalfsize.Y)
                return false;

            Vector2 basePosition = GetBasePosition();

            return (x >= Left + offset.Left &&
                    x < BackgroundRight - offset.Right &&
                    y >= Top + offset.Top &&
                    y < BackgroundBottom - offset.Bottom);
        }

        public Boolean BackgroundContainsMouse(FloatRect offset)
        {
            return this.BackgroundContains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        public Boolean IsVisible { get; set; }

        public Boolean IsEnabled { get; set; }

        public void SwitchState()
        {
            if (this.IsVisible)
                this.Close();
            else
                this.Open();
        }

        public virtual void Open(OpeningInfo openingInfo = null)
        {
            this.IsVisible = true;
            this.IsEnabled = true;
            
            foreach (Widget widget in this.Widgets)
                widget.Open(openingInfo);

            if (openingInfo != null && openingInfo.IsReseted)
                this.Reset();

            if (Opened != null)
                Opened(this, new OpenEventArgs(openingInfo));

            if (IsSealed)
                Seal(true, false);
        }

        public virtual void Close(ClosingInfo closingInfo = null)
        {
            this.IsVisible = false;
            this.IsEnabled = false;

            foreach (Widget widget in this.Widgets)
                widget.Close(closingInfo);
            
            if (closingInfo != null && closingInfo.IsReseted)
                this.Reset();

            if (Closed != null)
                Closed(this, new CloseEventArgs(closingInfo));
        }

        public virtual void Reset()
        {
            foreach (Widget widget in this.Widgets)
                widget.Reset();
        }

        public virtual void Disable()
        {
            this.IsEnabled = false;
        }

        public virtual void Refresh() { }

        public Int32 GetCount()
        {
            return this.Items.Count;
        }

        public Boolean Contains(Widget widget)
        {
            return this.Items.Contains(widget);
        }

        private Color _color;
        public override Color Color
        {
            get { return _color; }
            set
            {
                _color = value;

                foreach (Widget widget in this.Items)
                    if (widget.IsColorLinked)
                    {
                        widget.Color = this.Color;

                        if (widget.IsSealed)
                            widget.Seal();
                    }
            }
        }

        private Color _backgroundColor;
        public virtual Color BackgroundColor
        {
            get
            {
                if (this.Background != null)
                    return this.Background.Color;

                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;

                if (this.Background != null &&
                    IsBackgroundColorLinked)
                    this.Background.Color = value;

                foreach (Widget widget in this.Items)
                {
                    widget.BackgroundColor = this.BackgroundColor;

                    if (widget.IsSealed)
                        widget.Seal();
                }
            }
        }

        private BaseWidget _root;
        public virtual BaseWidget Root
        {
            get { return _root; }
            protected set
            {
                _root = value;

                foreach (Widget widget in this.Widgets)
                    widget.Root = this.Root;
            }
        }

        public bool IsRefreshed()
        {
            return RefreshInfo.IsRefreshed;
        }
    }

    public class OpeningInfo
    {
        public Boolean IsReseted { get; private set; }

        public Boolean IsValid()
        {
            return Args != null;
        }

        public Boolean IsValid(Int32 minCount)
        {
            return IsValid() && Args.Count >= minCount;
        }

        Dictionary<String, Object> Args;

        public OpeningInfo(Boolean isReseted = false, Dictionary<String, Object> args = null)
        {
            IsReseted = isReseted;

            Args = args;
        }

        public T GetArg<T>(String key)
        {
            return (T)Args[key];
        }

        public String GetMode()
        {
            return GetArg<String>("Mode");
        }

        public Boolean IsModeValid()
        {
            return IsValid(1) && Args.ContainsKey("Mode");
        }
    }

    public class ClosingInfo
    {
        public Boolean IsReseted { get; private set; }

        public Boolean IsValid()
        {
            return Args != null && Args.Count > 0;
        }

        public Boolean IsValid(Int32 minCount)
        {
            return IsValid() && Args.Count >= minCount;
        }

        Dictionary<String, Object> Args;

        public ClosingInfo(Boolean isReseted = false, Dictionary<String, Object> args = null)
        {
            IsReseted = isReseted;

            Args = args;
        }

        public T GetArg<T>(String key)
        {
            return (T)Args[key];
        }
    }

    public delegate void ChangeEventHandler(object sender, ChangeEventArgs e);

    public class ChangeEventArgs : EventArgs
    {
        public enum EType
        {
            Position,
            Dimension,
            Color,
            Text,
            Widget
        }

        public EType Type { get; private set; }

        public ChangeEventArgs(EType type)
        {
            this.Type = type;
        }
    }

    public class DimensionChangeEventArgs : ChangeEventArgs
    {
        public Vector2 Scale { get; private set; }
        public DimensionChangeEventArgs(Vector2 scale) : base(EType.Dimension) { this.Scale = scale; }
    }

    public class PositionChangeEventArgs : ChangeEventArgs
    {
        public Vector2 Offset { get; private set; }
        public PositionChangeEventArgs(Vector2 offset) : base(EType.Position) { this.Offset = offset; }
    }

    public class WidgetAddedChangeEventArgs : ChangeEventArgs
    {
        public Widget Widget { get; private set; }
        public WidgetAddedChangeEventArgs(Widget widget) : base(EType.Widget) { this.Widget = widget; }
    }

    public class TextChangeEventArgs : ChangeEventArgs
    {
        public String Text { get; private set; }
        public TextChangeEventArgs(String text) : base(EType.Text) { this.Text = text; }
    }

    public class RefreshInfo
    {
        public Boolean IsDimensionRefreshed { get; protected set; }
        public Boolean IsPositionRefreshed { get; protected set; }
        public Boolean IsWidgetRefreshed { get; protected set; }
        public Boolean IsTextfreshed { get; protected set; }

        public Vector2 DimensionScaleRefresh { get; set; }
        public Vector2 PositionOffsetRefresh { get; set; }
        public Widget WidgetAddedRefresh { get; set; }
        public String TextChangeRefresh { get; set; }
        
        public void Reset()
        {
            this.IsDimensionRefreshed = false;
            this.IsPositionRefreshed = false;
            this.IsWidgetRefreshed = false;
            this.IsTextfreshed = false;

            this.DimensionScaleRefresh = new Vector2(1F, 1F);
            this.PositionOffsetRefresh = new Vector2(0F, 0F);
            this.WidgetAddedRefresh = null;
            this.TextChangeRefresh = null;
        }

        public void SetDimensionScaleRefresh(Vector2 dimensionScaleRefresh)
        {
            if (dimensionScaleRefresh.X == 1F &&
                dimensionScaleRefresh.Y == 1F)
                return;

            this.IsDimensionRefreshed = true;
            this.DimensionScaleRefresh = dimensionScaleRefresh;
        }

        public void SetPositionOffsetRefresh(Vector2 positionOffsetRefresh)
        {
            if (positionOffsetRefresh.X == 0F &&
                positionOffsetRefresh.Y == 0F)
                return;

            this.IsPositionRefreshed = true;
            this.PositionOffsetRefresh = positionOffsetRefresh;
        }

        public void SetWidgetAddedRefresh(Widget widgetAddedRefresh)
        {
            this.IsWidgetRefreshed = true;
            this.WidgetAddedRefresh = widgetAddedRefresh;
        }

        public void SetTextChangeRefresh(String textChangeRefresh)
        {
            this.IsTextfreshed = true;
            this.TextChangeRefresh = textChangeRefresh;
        }

        public Boolean IsRefreshed
        {
            get
            {
                return
                    this.IsDimensionRefreshed ||
                    this.IsPositionRefreshed ||
                    this.IsWidgetRefreshed ||
                    this.IsTextfreshed;
            }
        }
    }

    public delegate void OpenEventHandler(Widget sender, OpenEventArgs e);
    public class OpenEventArgs : EventArgs
    {
        public OpeningInfo OpeningInfo { get; private set; }

        public OpenEventArgs(OpeningInfo openingInfo)
        {
            OpeningInfo = openingInfo;
        }
    }

    public delegate void CloseEventHandler(Widget sender, CloseEventArgs e);
    public class CloseEventArgs : EventArgs
    {
        public ClosingInfo ClosingInfo { get; private set; }

        public CloseEventArgs(ClosingInfo closingInfo)
        {
            ClosingInfo = closingInfo;
        }
    }
}