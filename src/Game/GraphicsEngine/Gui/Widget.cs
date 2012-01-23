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

    public interface IWidget : IUpdateable, IDrawable, IEventHandler { }

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

            Dimension = new Vector2f(GameData.WINDOW_WIDTH, GameData.WINDOW_HEIGHT);
        }

        public Vector2f GetViewPos()
        {
            return GuiView.Center - GuiView.Size / 2F;
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

        private Dictionary<Keyboard.Key, WindowedWidget> KeyWindowBinding = new Dictionary<Keyboard.Key, WindowedWidget>();

        public EditorBaseWidget(RenderWindow window, View guiView) :
            base(window, guiView)
        {
            Windows = new List<WindowedWidget>();
        }

        public void AddKeyWindowBind(Keyboard.Key keyCode, WindowedWidget window)
        {
            if (!KeyWindowBinding.ContainsKey(keyCode))
                KeyWindowBinding.Add(keyCode, window);
            else
                KeyWindowBinding[keyCode] = window;
        }

        bool SetCurrentWindowFromKey(Keyboard.Key keyCode)
        {
            if (!KeyWindowBinding.ContainsKey(keyCode))
                return false;

            WindowedWidget window = KeyWindowBinding[keyCode];

            if (FocusedWindow != window &&
                window.IsVisible)
            {
                SetCurrentWindow(window);
                return true;
            }

            window.SwitchState();
            if (window.IsVisible)
                SetCurrentWindow(window);

            return true;
        }

        private WindowedWidget GetNextOpenedWindow(WindowedWidget window)
        {
            if (window == null &&
                TabWindowList.Count > 0)
                return TabWindowList[0];

            Int32 count = 0;
            Int32 index = TabWindowList.IndexOf(window);

            while (count < TabWindowList.Count)
            {
                if (index < TabWindowList.Count - 1)
                    ++index;
                else
                    index = 0;

                ++count;

                if (TabWindowList[index].IsVisible)
                    return TabWindowList[index];
            }

            return null;
        }

        public override void Draw(RenderTarget window)
        {
            if (Background != null)
                Background.Draw(window);

            foreach (Widget widget in DrawingWidgets)
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

                    if (evt.MouseButton.Button != Mouse.Button.Left)
                        break;

                    if (FocusedWindow == null)
                        break;

                    FocusedWindow.SetFocused(false);
                  //  FocusedWindow.BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;
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

                    if (SetCurrentWindowFromKey(evt.Key.Code))
                        return true;

                    switch (evt.Key.Code)
                    {
                        case Keyboard.Key.Tab:

                            WindowedWidget nextOpenedWindow = GetNextOpenedWindow(FocusedWindow);

                            if (nextOpenedWindow == null)
                                break;

                            SetCurrentWindow(nextOpenedWindow);

                            return true;
                    }

                    break;
            }

            return base.OnPredominantEvent(evt);
        }

        public override Boolean HandleEvent(BlzEvent evt)
        {
            if (!IsEnabled)
                return false;

            if (evt.GetType() == BlzEvent.EType.MouseMove)
            {
                if (Focused != null)
                {
                    Focused.HandleEvent(evt);
                    evt.IsHandled = true;
                }

                foreach (WindowedWidget window in Windows)
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

            if (Focused != null)
            {
                Focused.HandleEvent(evt);
                return true;
            }

            foreach (WindowedWidget window in Windows)
                if (window.HandleEvent(evt))
                {
                    SetCurrentWindow(window);
                    return true;
                }

            return OnEvent(evt);
        }

        public void SetCurrentWindow(WindowedWidget window)
        {
            if (!RemoveWindow(window))
                return;

            if (FocusedWindow != null)
            {
                FocusedWindow.SetFocused(false);
               // FocusedWindow.BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;
                FocusedWindow.Closed -= new CloseEventHandler(FocusedWindow_Closed);
            }

            FocusedWindow = window;

            AddFirst(window);
            if (!window.GotFocusedWindow() ||
                (window.GotFocusedWindow() && !window.GotFocusedWindow(true)))
                window.SetFocused(true);
              //  window.BackgroundColor = WindowedWidget.FOCUSED_COLOR;
            Windows.Insert(0, window);

            DrawnWindows.Add(window, true);
        }

        public void SetWindowOutDrawing(Boolean drawn, WindowedWidget window)
        {
            DrawnWindows[window] = drawn;
        }

        public void AddWindow(WindowedWidget window, Boolean opened = false)
        {
            TabWindowList.Add(window);

            DrawnWindows.Add(window, true);

            window.SetFocused(false);
          //  window.BackgroundColor = Border.DEFAULT_AMBIENT_COLOR;

            AddWidget(window);
            Windows.Add(window);

            if (opened)
                window.Open();
        }

        public Boolean RemoveWindow(WindowedWidget window)
        {
            if (!RemoveWidget(window))
                return false;
            return
                Windows.Remove(window) &&
                DrawnWindows.Remove(window);
        }

        public Vector2f GetMousePosition()
        {
            return GetGlobalFromLocal(new Vector2f(
                Mouse.GetPosition(Root.Window).X,
                Mouse.GetPosition(Root.Window).Y));
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
        public Vector2f Offset { get; private set; }

        /// <summary>
        /// Position of the view after moving
        /// </summary>
        public Vector2f Position { get; private set; }

        /// <summary>
        /// Dimension of the view
        /// </summary>
        public Vector2f Dimension { get; private set; }

        /// <summary>
        /// Creates game view info move with a given offset, view center and view size
        /// </summary>
        /// <param name="offset">View move offset</param>
        /// <param name="viewCenter">View center after moving</param>
        /// <param name="viewSize">View dimension</param>
        public GameViewEventArgs(Vector2f offset, Vector2f viewCenter, Vector2f viewSize)
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
        public Boolean CallOnGameViewMove(Vector2f offset) { if (OnGameViewMove == null) return false; OnGameViewMove(MapView, new GameViewEventArgs(offset, MapView.Center, MapView.Size)); return true; }

        public GameBaseWidget(RenderWindow window, View mapView, View gameGuiView) :
            base(window, gameGuiView)
        {
            MapView = mapView;
            GameWidgets = new List<GameWidget>();

            OnGameViewMove += new GameViewEventHandler(GameBaseWidget_OnGameViewMove);
        }

        public void MoveGameView(Vector2f offset)
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
            while (!SpeechManager.Instance.IsEmpty())
            {
                SpeechBubble currentSpeechBubble = SpeechManager.Instance.GetNext();
                currentSpeechBubble.OnClosing += new SpeechBubble.ClosingEventHandler(currentSpeechBubble_OnClosing);
                AddGameWidget(currentSpeechBubble);
                currentSpeechBubble.LaunchSpeech();
            }

            base.Update(dt);
            //foreach (GameWidget gameWidget in GameWidgets)
                //gameWidget.Update(dt);
        }

        void GameBaseWidget_OnGameViewMove(View view, GameViewEventArgs e)
        {
            foreach (GameWidget gameWidget in GameWidgets)
                if (gameWidget is SpeechBubble)
                    gameWidget.Refresh();
        }

        void currentSpeechBubble_OnClosing(SpeechBubble sender, SpeechBubble.ClosingEventArgs e)
        {
            RemoveGameWidget(sender);
        }

       /* public override void Draw(RenderTarget window)
        {
            for (int count = GameWidgets.Count - 1; count >= 0; --count)
                if (GameWidgets[count].IsVisible)
                    GameWidgets[count].Draw(window);
        }*/

      /*  public override Boolean HandleEvent(BlzEvent evt)
        {
            foreach (GameWidget gameWidget in GameWidgets)
                if (gameWidget.IsEnabled)
                    if (gameWidget.HandleEvent(evt))
                        return true;

            return OnEvent(evt);
        }*/

        public override BaseWidget Root
        {
            protected set { base.Root = this; }
        }
    }

    #endregion

    public abstract class Widget : BaseDrawable, IWidget
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

                if (Focused == null &&
                    Parent != null)
                {
                    Parent.Focused = null;
                    return;
                }

                if (Parent == null)
                    return;

                Parent.Focused = this;
            }
        }
        
        public void SetFirst(Widget widget)
        {
            if (!RemoveWidget(widget))
                return;

            AddFirst(widget);
        }

        public void SetFirst()
        {
            if (Parent == null)
                return;

            Parent.SetFirst(this);
        }

        public void SetRootFirst()
        {
            SetFirst();

            if (Parent == null)
                return;

            Parent.SetRootFirst();
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
            Widgets = new List<Widget>();
            Items = new List<Widget>();
            DrawingWidgets = new LinkedList<Widget>();

            IsPositionLinked = true;
            IsDimensionLinked = false;

            IsColorLinked = true;
            IsBackgroundColorLinked = false;

            IsSealed = false;

            Dimension = new Vector2f(1F, 1F);

            RefreshInfo = new RefreshInfo();

            WidgetsToRemove = new Queue<Widget>();

            Changed += new ChangeEventHandler(Widget_Changed);

            Open();
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

        public override object Clone()
        {
            throw new NotImplementedException();
        }

        void Widget_Changed(object sender, ChangeEventArgs e)
        {
            switch (e.Type)
            {
                case ChangeEventArgs.EType.Dimension: RefreshInfo.SetDimensionScaleRefresh(((DimensionChangeEventArgs)e).Scale); break;
                case ChangeEventArgs.EType.Position: RefreshInfo.SetPositionOffsetRefresh(((PositionChangeEventArgs)e).Offset); break;
                case ChangeEventArgs.EType.Widget: RefreshInfo.SetWidgetAddedRefresh(((WidgetAddedChangeEventArgs)e).Widget); break;
                case ChangeEventArgs.EType.Text: RefreshInfo.SetTextChangeRefresh(((TextChangeEventArgs)e).Text); break;
            }
        }

        protected void CallChanged(ChangeEventArgs e)
        {
            if (Changed != null)
                Changed(this, e);
        }

        public virtual void Init()
        {
            foreach (Widget widget in Widgets)
                widget.Init();

            Refresh();

            RefreshParent();
        }

        Queue<Widget> WidgetsToRemove;
        public virtual void Update(Time dt)
        {
            while (WidgetsToRemove.Count > 0)
                RemoveWidget(WidgetsToRemove.Dequeue());

            if (RefreshInfo.IsRefreshed)
            {
                Refresh();

                RefreshParent();

                RefreshInfo.Reset();
            }

            foreach (Widget widget in Widgets)
                widget.Update(dt);
        }

        const bool REFRESH_OPTI = false;
        void RefreshParent()
        {
            if (Parent == null)
                return;

            Parent.Refresh();

            if (!REFRESH_OPTI)
                Parent.RefreshParent();
        }

        public override void Draw(RenderTarget window)
        {
            if (Background != null)
                Background.Draw(window);

            foreach (Widget widget in DrawingWidgets)
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
                Focused.HandleEvent(evt);
                evt.IsHandled = true;

                return false;
            }

            Focused.HandleEvent(evt);
            OnPredominantEvent(evt);

            return true;
        }

        protected virtual Boolean ItemHandleEvent(BlzEvent evt)
        {
            if (evt.GetType() == BlzEvent.EType.MouseMove)
            {
                foreach (Widget widget in Items)
                    if (widget.HandleEvent(evt))
                        evt.IsHandled = true;

                return false;
            }

            foreach (Widget widget in Items)
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
            if (!IsEnabled)
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

            Widgets.Add(widget);

            if (addToEvent)
                Items.Add(widget);

            if (addToDrawing)
                DrawingWidgets.AddFirst(widget);

            CallChanged(new WidgetAddedChangeEventArgs(widget));
        }

        public void InsertWidget(Int32 index, Widget widget, Boolean addToDrawing = true, Boolean addToEvent = true)
        {
            widget.SetParent(this);

            widget.Changed += new ChangeEventHandler(Widget_Changed);

            Widgets.Insert(index, widget);

            if (addToEvent)
                Items.Insert(index, widget);

            if (addToDrawing)
                DrawingWidgets.ToList().Insert(DrawingWidgets.Count - 1 - index, widget);

            CallChanged(new WidgetAddedChangeEventArgs(widget));
        }

        public void AddFirst(Widget widget, Boolean addToDrawing = true)
        {
            widget.SetParent(this);

            widget.Changed += new ChangeEventHandler(Widget_Changed);

            Widgets.Insert(0, widget);
            Items.Insert(0, widget);

            if (addToDrawing)
                DrawingWidgets.AddLast(widget);

            CallChanged(new WidgetAddedChangeEventArgs(widget));
        }

        public void SetParent(Widget parent)
        {
            Root = parent.Root;

            Parent = parent;
        }

        public void AsyncRemoveWidget(Widget widget)
        {
            WidgetsToRemove.Enqueue(widget);
        }

        public Boolean RemoveWidget(Widget widget)
        {
            if (Background != null &&
                widget.Equals(Background))
            {
                Background = null;

                return Widgets.Remove(widget);
            }

            CallChanged(new WidgetAddedChangeEventArgs(widget));

            DrawingWidgets.Remove(widget);

            return
                Widgets.Remove(widget) &&
                Items.Remove(widget);
        }

        public Widget GetWidget(String name)
        {
            foreach (Widget w in Widgets)
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

        //!\\ TODO: set virtual and 'Root.GetViewPos' is redefined into the BaseWidget method
        public Vector2f GetLocalFromGlobal(Vector2f point)
        {
            if (Root == null)
                return point - GetBasePosition();

            return point - GetBasePosition() - Root.GetViewPos();
        }

        //!\\ TODO: cf GetLocalFromGlobal
        public Vector2f GetGlobalFromLocal(Vector2f point)
        {
            if (Root == null)
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

                if (Background == null)
                    return;

                Background.SetParent(this);

                Dimension = BackgroundDimension;

                Widgets.Add(Background);
            }
        }

        private Vector2f _position;
        public override Vector2f Position
        {
            get { return _position; }
            set
            {
                Vector2f offset = value - Position;

                _position = value;

                foreach (Widget widget in Widgets)
                    if (widget.IsPositionLinked)
                        widget.Position += offset;

                CallChanged(new PositionChangeEventArgs(offset));
            }
        }

        private Vector2f _dimension;
        public override Vector2f Dimension
        {
            get
            {
                return _dimension;
            }
            set
            {
                if (Background != null && Dimension.X == 0F || Dimension.Y == 0F)
                {
                    _dimension = value;

                    BackgroundDimension = Dimension;

                    return;
                }

                Vector2f factor = new Vector2f(1F, 1F);

                factor = new Vector2f(
                    value.X / Dimension.X,
                    value.Y / Dimension.Y);

                _dimension = value;

                foreach (Widget widget in Items)
                    if (widget.IsDimensionLinked)
                        widget.Dimension = new Vector2f(
                            widget.Dimension.X * factor.X,
                            widget.Dimension.Y * factor.Y);

                if (Background != null)
                    BackgroundDimension = Dimension + GetStructureDimension();

                CallChanged(new DimensionChangeEventArgs(factor));
            }
        }

        protected virtual Vector2f GetBasePosition()
        {
            return Position;
        }

        public virtual Vector2f BackgroundDimension
        {
            get
            {
                if (Background != null)
                    return Background.Dimension;

                return Dimension;
            }
            set
            {
                _dimension = value;

                if (Background == null)
                    return;

                Background.Dimension = value;

                _dimension = value - GetStructureDimension();
            }
        }

        protected virtual Vector2f GetStructureDimension()
        {
            return new Vector2f(0F, 0F);
        }

        public float BackgroundRight
        {
            get { return Position.X + BackgroundDimension.X; }
            set { Position = new Vector2f(value - BackgroundDimension.X, Position.Y); }
        }

        public float BackgroundBottom
        {
            get { return Position.Y + BackgroundDimension.Y; }
            set { Position = new Vector2f(Position.X, value - BackgroundDimension.Y); }
        }

        public Vector2f BackgroundHalfsize
        {
            get { return BackgroundDimension / 2F; }
            set { BackgroundDimension = value * 2F; }
        }

        public Vector2f BackgroundCenter
        {
            get { return Position + BackgroundHalfsize; }
            set { Position = value - BackgroundHalfsize; }
        }

        protected Boolean Contains(float x, float y, float offset = 0F)
        {
            if (offset >= Halfsize.X ||
                offset >= Halfsize.Y)
                return false;

            Vector2f basePosition = GetBasePosition();

            return (x >= basePosition.X + offset &&
                    x < basePosition.X + Dimension.X - offset &&
                    y >=  basePosition.Y + offset &&
                    y < basePosition.Y + Dimension.Y - offset);
        }

        //!\\ to remove (put into EditorBase)
        public Boolean ContainsMouse(float offset = 0F)
        {
            return Contains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        protected Boolean BackgroundContains(float x, float y, float offset = 0F)
        {
            if (offset >= BackgroundHalfsize.X ||
                offset >= BackgroundHalfsize.Y)
                return false;

            return (x >= Left + offset &&
                    x < BackgroundRight - offset &&
                    y >= Top + offset &&
                    y < BackgroundBottom - offset);
        }

        //!\\ to remove (put into EditorBase)
        public Boolean BackgroundContainsMouse(float offset = 0F)
        {
            return BackgroundContains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        protected Boolean Contains(float x, float y, FloatRect offset)
        {
            if (offset.HSum >= Halfsize.X ||
                offset.VSum >= Halfsize.Y)
                return false;

            Vector2f basePosition = GetBasePosition();

            return (x >= basePosition.X + offset.Left &&
                    x < basePosition.X + Dimension.X - offset.Right &&
                    y >= basePosition.Y + offset.Top &&
                    y < basePosition.Y + Dimension.Y - offset.Bottom);
        }

        //!\\ to remove (put into EditorBase)
        public Boolean ContainsMouse(FloatRect offset)
        {
            return Contains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        protected Boolean BackgroundContains(float x, float y, FloatRect offset)
        {
            if (offset.HSum >= BackgroundHalfsize.X ||
                offset.VSum >= BackgroundHalfsize.Y)
                return false;

            Vector2f basePosition = GetBasePosition();

            return (x >= Left + offset.Left &&
                    x < BackgroundRight - offset.Right &&
                    y >= Top + offset.Top &&
                    y < BackgroundBottom - offset.Bottom);
        }

        //!\\ to remove (put into EditorBase)
        public Boolean BackgroundContainsMouse(FloatRect offset)
        {
            return BackgroundContains(
                ((EditorBaseWidget)Root).GetMousePosition().X,
                ((EditorBaseWidget)Root).GetMousePosition().Y,
                offset);
        }

        public Boolean IsEnabled { get; set; }

        public void SwitchState()
        {
            if (IsVisible)
                Close();
            else
                Open();
        }

        public virtual void Open(OpeningInfo openingInfo = null)
        {
            IsVisible = true;
            IsEnabled = true;
            
            foreach (Widget widget in Widgets)
                widget.Open(openingInfo);

            if (openingInfo != null && openingInfo.IsReseted)
                Reset();

            if (Opened != null)
                Opened(this, new OpenEventArgs(openingInfo));

            if (IsSealed)
                Seal(true, false);
        }

        public virtual void Close(ClosingInfo closingInfo = null)
        {
            IsVisible = false;
            IsEnabled = false;

            foreach (Widget widget in Widgets)
                widget.Close(closingInfo);
            
            if (closingInfo != null && closingInfo.IsReseted)
                Reset();

            if (Closed != null)
                Closed(this, new CloseEventArgs(closingInfo));
        }

        public virtual void Reset()
        {
            foreach (Widget widget in Widgets)
                widget.Reset();
        }

        public virtual void Disable()
        {
            IsEnabled = false;
        }

        public virtual void Refresh() { }

        public Int32 GetCount()
        {
            return Items.Count;
        }

        public Boolean Contains(Widget widget)
        {
            return Items.Contains(widget);
        }

        private Color _color;
        public override Color Color
        {
            get { return _color; }
            set
            {
                _color = value;

                foreach (Widget widget in Items)
                    if (widget.IsColorLinked)
                    {
                        widget.Color = Color;

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
                if (Background != null)
                    return Background.Color;

                return _backgroundColor;
            }
            set
            {
                _backgroundColor = value;

                if (Background != null &&
                    IsBackgroundColorLinked)
                    Background.Color = value;

                foreach (Widget widget in Items)
                {
                    widget.BackgroundColor = BackgroundColor;

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

                foreach (Widget widget in Widgets)
                    widget.Root = Root;
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
            Type = type;
        }
    }

    public class DimensionChangeEventArgs : ChangeEventArgs
    {
        public Vector2f Scale { get; private set; }
        public DimensionChangeEventArgs(Vector2f scale) : base(EType.Dimension) { Scale = scale; }
    }

    public class PositionChangeEventArgs : ChangeEventArgs
    {
        public Vector2f Offset { get; private set; }
        public PositionChangeEventArgs(Vector2f offset) : base(EType.Position) { Offset = offset; }
    }

    public class WidgetAddedChangeEventArgs : ChangeEventArgs
    {
        public Widget Widget { get; private set; }
        public WidgetAddedChangeEventArgs(Widget widget) : base(EType.Widget) { Widget = widget; }
    }

    public class TextChangeEventArgs : ChangeEventArgs
    {
        public String Text { get; private set; }
        public TextChangeEventArgs(String text) : base(EType.Text) { Text = text; }
    }

    public class RefreshInfo
    {
        public Boolean IsDimensionRefreshed { get; protected set; }
        public Boolean IsPositionRefreshed { get; protected set; }
        public Boolean IsWidgetRefreshed { get; protected set; }
        public Boolean IsTextfreshed { get; protected set; }

        public Vector2f DimensionScaleRefresh { get; set; }
        public Vector2f PositionOffsetRefresh { get; set; }
        public Widget WidgetAddedRefresh { get; set; }
        public String TextChangeRefresh { get; set; }
        
        public void Reset()
        {
            IsDimensionRefreshed = false;
            IsPositionRefreshed = false;
            IsWidgetRefreshed = false;
            IsTextfreshed = false;

            DimensionScaleRefresh = new Vector2f(1F, 1F);
            PositionOffsetRefresh = new Vector2f(0F, 0F);
            WidgetAddedRefresh = null;
            TextChangeRefresh = null;
        }

        public void SetDimensionScaleRefresh(Vector2f dimensionScaleRefresh)
        {
            if (dimensionScaleRefresh.X == 1F &&
                dimensionScaleRefresh.Y == 1F)
                return;

            IsDimensionRefreshed = true;
            DimensionScaleRefresh = dimensionScaleRefresh;
        }

        public void SetPositionOffsetRefresh(Vector2f positionOffsetRefresh)
        {
            if (positionOffsetRefresh.X == 0F &&
                positionOffsetRefresh.Y == 0F)
                return;

            IsPositionRefreshed = true;
            PositionOffsetRefresh = positionOffsetRefresh;
        }

        public void SetWidgetAddedRefresh(Widget widgetAddedRefresh)
        {
            IsWidgetRefreshed = true;
            WidgetAddedRefresh = widgetAddedRefresh;
        }

        public void SetTextChangeRefresh(String textChangeRefresh)
        {
            IsTextfreshed = true;
            TextChangeRefresh = textChangeRefresh;
        }

        public Boolean IsRefreshed
        {
            get
            {
                return
                    IsDimensionRefreshed ||
                    IsPositionRefreshed ||
                    IsWidgetRefreshed ||
                    IsTextfreshed;
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