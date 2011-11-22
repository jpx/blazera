using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class MapBox : Widget
    {
        Vector2f Size;
        float Margins;

        VScrollBar VScrollBar;
        HScrollBar HScrollBar;

        public Map Map { get; private set; }

        GameBaseWidget GameRoot;
        Vector2f ViewMove;

        List<WorldObject> ObjectToDraw = new List<WorldObject>();
        List<Widget> WidgetToDraw = new List<Widget>();

        public Player MainPlayer;

        public event ClickEventHandler MapClicked;
        
        public MapBox(Vector2f size) :
            base()
        {
            Margins = Border.GetBoxBorderWidth();
            Size = size;

            VScrollBar = new VScrollBar();
            AddWidget(VScrollBar);
            HScrollBar = new HScrollBar();
            AddWidget(HScrollBar);
        }

        public void SetGameRoot(RenderWindow window)
        {
            GameRoot = new GameBaseWidget(window, new View(new SFML.Graphics.FloatRect(0F, 0F, Size.X, Size.Y)), new View(new SFML.Graphics.FloatRect(0F, 0F, Size.X, Size.Y)));
            GameRoot.IsPositionLinked = false;
            GameRoot.Dimension = new Vector2f(Size.X, Size.Y);
            AddWidget(GameRoot);
        }
        
        public void AddGameWidget(GameWidget gameWidget)
        {
            GameRoot.AddGameWidget(gameWidget);
        }

        public Boolean RemoveGameWidget(GameWidget gameWidget)
        {
            return GameRoot.RemoveGameWidget(gameWidget);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (Map == null)
                return;

            Map.Update(dt);

            GameRoot.MoveGameView(ViewMove);

            ViewMove = new Vector2f();

            Inputs.Instance.UpdateState();
        }

        public override void Draw(RenderWindow window)
        {
            if (Map == null)
                return;

            window.SetView(GameRoot.MapView);

            Map.Draw(window);

            foreach (WorldObject wObj in ObjectToDraw)
                wObj.Draw(window);

            foreach (Widget widget in WidgetToDraw)
                if (widget.IsVisible)
                    widget.Draw(window);

            window.SetView(window.DefaultView);

            base.Draw(window);
        }

        public override Boolean HandleEvent(BlzEvent evt)
        {
            if (evt.Type == EventType.KeyPressed || evt.Type == EventType.KeyReleased)
                Inputs.Instance.UpdateState(evt);

            return base.HandleEvent(evt);
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (HandlePlayerEvent(evt))
                return true;

            if (evt.IsHandled)
                return base.OnEvent(evt);

            switch (evt.Type)
            {
                case EventType.MouseButtonPressed:

                    if (evt.MouseButton.Button != Mouse.Button.Left)
                        break;

                    if (!MapContainsMouse())
                        break;

                    if (MapClicked == null)
                        break;
                    
                    MouseButtonEvent e = new MouseButtonEvent();
                    Vector2f ePoint = GetMapLocalFromGlobal(new Vector2f(evt.MouseButton.X, evt.MouseButton.Y));
                    e.X = (Int32)ePoint.X;
                    e.Y = (Int32)ePoint.Y;

                    MapClicked(this, new MouseButtonEventArgs(e));

                    return true;
            }

            return base.OnEvent(evt);
        }

        Boolean HandlePlayerEvent(BlzEvent evt)
        {
            switch (evt.Type)
            {
                #region player moves
                case EventType.KeyPressed:

                    if (Keyboard.IsKeyPressed(Keyboard.Key.LControl))
                        break;

                    if (Inputs.IsGameInput(InputType.Misc, evt))
                    {
                        MainPlayer.SetRunning();
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Up, evt))
                    {
                        MainPlayer.EnableDirection(Direction.N);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Right, evt))
                    {
                        MainPlayer.EnableDirection(Direction.E);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Down, evt))
                    {
                        MainPlayer.EnableDirection(Direction.S);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Left, evt))
                    {
                        MainPlayer.EnableDirection(Direction.O);
                        return true;
                    }

                    break;

                case EventType.KeyReleased:

                    if (Inputs.IsGameInput(InputType.Misc, evt))
                    {
                        MainPlayer.SetRunning(false);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Up, evt))
                    {
                        MainPlayer.DisableDirection(Direction.N);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Right, evt))
                    {
                        MainPlayer.DisableDirection(Direction.E);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Down, evt))
                    {
                        MainPlayer.DisableDirection(Direction.S);
                        return true;
                    }

                    if (Inputs.IsGameInput(InputType.Left, evt))
                    {
                        MainPlayer.DisableDirection(Direction.O);
                        return true;
                    }

                    break;
                #endregion
            }

            return false;
        }

        public override void Refresh()
        {
            VScrollBar.Dimension = new Vector2f(
                VScrollBar.Dimension.X,
                Size.Y);

            VScrollBar.Position = GetGlobalFromLocal(new Vector2f(
                Size.X + Margins,
                0F));

            HScrollBar.Dimension = new Vector2f(
                Size.X,
                HScrollBar.Dimension.Y);

            HScrollBar.Position = GetGlobalFromLocal(new Vector2f(
                0F,
                Size.Y + Margins));

            if (!RefreshInfo.IsPositionRefreshed)
                return;

            UpdateView();
        }

        void UpdateView()
        {
            if (GameRoot.MapView == null || Root == null)
                return;

            Vector2f viewPos = GetGlobalFromLocal(new Vector2f());
            GameRoot.MapView.Viewport = new SFML.Graphics.FloatRect(
                viewPos.X / Root.Window.Width,
                viewPos.Y / Root.Window.Height,
                Size.X / Root.Window.Width,
                Size.Y / Root.Window.Height);

            GameRoot.GuiView.Viewport = GameRoot.MapView.Viewport;
        }

        public override Vector2f Dimension
        {
            get
            {
                if (VScrollBar == null ||
                    HScrollBar == null)
                    return base.Dimension;

                return Size + new Vector2f(VScrollBar.Dimension.X + Margins, HScrollBar.Dimension.Y + Margins);
            }
        }

        public void SetMap(Map map)
        {
            Map = map;

            VScrollBar.SetValues((Int32)(GameRoot.MapView.Size.Y / GameDatas.TILE_SIZE), (Int32)(Map.Dimension.Y / GameDatas.TILE_SIZE));
            HScrollBar.SetValues((Int32)(GameRoot.MapView.Size.X / GameDatas.TILE_SIZE), (Int32)(Map.Dimension.X / GameDatas.TILE_SIZE));

            UpdateView();
        }

        const float VIEW_MOVE_MINOR_LIMIT = .5F;
        const float VIEW_MOVE_TRIGGER_LIMIT = 20F;
        Vector2f GetViewMove(Time dt, Player player)
        {
            float moveX = 0.0f;
            float moveY = 0.0f;

            if (player == null)
            {
                return new Vector2f(moveX, moveY);
            }

            Vector2f p = player.Center;

            if (Math.Abs(p.X - GameRoot.MapView.Center.X) > VIEW_MOVE_TRIGGER_LIMIT * (GameDatas.WINDOW_HEIGHT / GameDatas.WINDOW_WIDTH))
                moveX = player.Velocity * (p.X - GameRoot.MapView.Center.X) / 50f * GameDatas.WINDOW_WIDTH / GameDatas.WINDOW_HEIGHT * (float)dt.Value;
            if (Math.Abs(p.Y - GameRoot.MapView.Center.Y) > VIEW_MOVE_TRIGGER_LIMIT * (GameDatas.WINDOW_WIDTH / GameDatas.WINDOW_HEIGHT))
                moveY = player.Velocity * (p.Y - GameRoot.MapView.Center.Y) / 50f * GameDatas.WINDOW_HEIGHT / GameDatas.WINDOW_WIDTH * (float)dt.Value;

            /*if (Math.Abs(moveX) < VIEW_MOVE_MINOR_LIMIT)
                moveX = p.X - Gui.MapView.Center.X;

            if (Math.Abs(moveY) < VIEW_MOVE_MINOR_LIMIT)
                moveY = p.Y - Gui.MapView.Center.Y;*/

            if (GameRoot.MapView.Center.X - GameRoot.MapView.Size.X / 2 + moveX < 0F)
            {
                GameRoot.MapView.Center = new Vector2f(0F, GameRoot.MapView.Center.Y) + new Vector2f(GameRoot.MapView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (GameRoot.MapView.Center.X - GameRoot.MapView.Size.X / 2 + GameRoot.MapView.Size.X + moveX >= Map.Dimension.X)
            {
                GameRoot.MapView.Center = new Vector2f(Map.Dimension.X, GameRoot.MapView.Center.Y) - new Vector2f(GameRoot.MapView.Size.X / 2F, 0F);
                moveX = 0.0f;
            }

            if (GameRoot.MapView.Center.Y - GameRoot.MapView.Size.Y / 2 + moveY < 0F)
            {
                GameRoot.MapView.Center = new Vector2f(GameRoot.MapView.Center.X, 0F) + new Vector2f(0F, GameRoot.MapView.Size.Y / 2F);
                moveY = 0.0f;
            }

            if (GameRoot.MapView.Center.Y - GameRoot.MapView.Size.Y / 2 + GameRoot.MapView.Size.Y + moveY >= Map.Dimension.Y)
            {
                GameRoot.MapView.Center = new Vector2f(GameRoot.MapView.Center.X, Map.Dimension.Y) - new Vector2f(0F, GameRoot.MapView.Size.Y / 2F); ;
                moveY = 0.0f;
            }

            return new Vector2f(moveX, moveY);
        }

        public void UpdateLockedViewMove(Time dt, Player player)
        {
            ViewMove = GetViewMove(dt, player);
        }

        public void UpdateUnlockedViewMove()
        {
            float xMove = HScrollBar.CursorPosition * GameDatas.TILE_SIZE - (GameRoot.MapView.Center.X - GameRoot.MapView.Size.X / 2F);
            float yMove = VScrollBar.CursorPosition * GameDatas.TILE_SIZE - (GameRoot.MapView.Center.Y - GameRoot.MapView.Size.Y / 2F);

            ViewMove = new Vector2f(xMove, yMove);
        }

        public Boolean MapContainsMouse()
        {
            Vector2f mousePos = GetLocalFromGlobal(((EditorBaseWidget)Root).GetMousePosition());

            return !(
                mousePos.X < 0F ||
                mousePos.Y < 0F ||
                mousePos.X >= Size.X ||
                mousePos.Y >= Size.Y);
        }

        public Vector2f GetMapLocalFromGlobal(Vector2f point)
        {
            if (Map == null)
                return point;

            return GetLocalFromGlobal(point) + GameRoot.MapView.Center - GameRoot.MapView.Size / 2F;
        }

        public void AddObjectToDraw(WorldObject wObj)
        {
            ObjectToDraw.Add(wObj);
        }

        public Boolean RemoveObjectToDraw(WorldObject wObj)
        {
            return ObjectToDraw.Remove(wObj);
        }

        public void AddWidgetToDraw(Widget widget)
        {
            WidgetToDraw.Add(widget);
            widget.IsPositionLinked = false;
            AddWidget(widget, false);
        }

        public Boolean RemoveWidgetToDraw(Widget widget)
        {
            if (!WidgetToDraw.Remove(widget))
                return false;

            return Items.Remove(widget);
        }

        public void ClearWidgetToDraw()
        {
            Queue<Widget> toRemove = new Queue<Widget>();

            foreach (Widget widget in WidgetToDraw)
                toRemove.Enqueue(widget);

            while (toRemove.Count > 0)
                RemoveWidgetToDraw(toRemove.Dequeue());
        }

        public FloatRect GetMapOffset()
        {
            return new FloatRect(
                Margins,
                Margins,
                Margins * 2F + VScrollBar.Dimension.X,
                Margins * 2F + HScrollBar.Dimension.Y);
        }
    }
}