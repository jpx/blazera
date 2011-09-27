using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public abstract class Pencil : Tool
    {
        public enum EMode
        {
            Normal,
            Pot
        }

        static readonly Texture EMPTY_CURSOR_TEXTURE = Create.Texture("Gui_PencilEmptyCursor");
        const float CURSOR_SCALE_FACTOR = .65F;
        const double CURSOR_ALPHA_FACTOR = 55D;
        static readonly Color CANT_PAINT_CURSOR_COLOR = Color.Red;

        protected EMode Mode;
        protected Boolean IsPainting;

        PictureBox Cursor;

        protected UInt32 LockValue;

        protected Pencil(Texture iconTexture) :
            base(iconTexture)
        {
            IsPainting = false;
            Mode = EMode.Normal;

            Cursor = new PictureBox(null);
            AddWidget(Cursor);

            Empty();
            ShowCursor(false);
        }

        public override void Open(OpeningInfo openingInfo = null)
        {
            base.Open(openingInfo);

            ShowCursor(false);
        }

        protected void SetCursorTexture(Texture cursorTexture)
        {
            Cursor.Texture = cursorTexture;
            Cursor.Dimension *= CURSOR_SCALE_FACTOR;
            Cursor.Texture.SetAlpha(CURSOR_ALPHA_FACTOR);
        }

        void ShowCursor(Boolean shown = true)
        {
            if (Cursor == null)
                return;

            if (shown)
                Cursor.Open();
            else
                Cursor.Close();
        }

        void SetCursorEnability(Boolean enabled)
        {
            if (!enabled && !IsEmpty())
            {
                Cursor.Color = CANT_PAINT_CURSOR_COLOR;
            }
            else
            {
                Cursor.Color = Color.White;
            }

            Cursor.Texture.SetAlpha(CURSOR_ALPHA_FACTOR);
        }

        public void SetLockValue(UInt32 lockValue)
        {
            LockValue = lockValue;
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
            {
                End();

                ShowCursor(false);

                return base.OnEvent(evt);
            }

            switch (evt.Type)
            {
                case EventType.MouseButtonPressed:

                    if (evt.MouseButton.Button == MouseButton.Right)
                    {
                        Empty();

                        if (!MapBox.MapContainsMouse())
                            break;

                        return true;
                    }

                    if (evt.MouseButton.Button != MouseButton.Left)
                        break;

                    if (!MapBox.MapContainsMouse())
                        break;

                    Init();

                    Vector2 clickPaintPoint = GetLocalMouseCenter();
                    if (!CanPaint(clickPaintPoint))
                        break;

                    return Paint(clickPaintPoint);

                case EventType.MouseButtonReleased:

                    if (evt.MouseButton.Button != MouseButton.Left)
                        break;

                    End();

                    if (!MapBox.MapContainsMouse())
                        break;

                    return true;

                case EventType.MouseMoved:

                    if (!MapBox.MapContainsMouse())
                    {
                        ShowCursor(false);
                        break;
                    }

                    Vector2 movePaintPoint = GetLocalMouseCenter();
                    Boolean canPaint = CanPaint(movePaintPoint);

                    ShowCursor();

                    SetCursorEnability(canPaint);
                    
                    if (!IsPainting)
                        break;

                    if (!canPaint)
                        break;

                    return Paint(movePaintPoint);

                case EventType.KeyPressed:

                    switch (evt.Key.Code)
                    {
                        case KeyCode.P:

                            Mode = EMode.Pot;

                            return true;
                    }

                    break;

                case EventType.KeyReleased:

                    switch (evt.Key.Code)
                    {
                        case KeyCode.P:

                            Mode = EMode.Normal;

                            return true;
                    }

                    break;
            }

            return base.OnEvent(evt);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            Cursor.Center = GetLockedPosition(((EditorBaseWidget)Root).GetMousePosition());
        }

        Vector2 GetLockedPosition(Vector2 point)
        {
            if (LockValue < 2)
                return point;

            return new Vector2(
                ((UInt32)point.X) / LockValue * LockValue,
                ((UInt32)point.Y) / LockValue * LockValue);
        }

        Vector2 GetLocalMouseCenter()
        {
            return GetLockedPosition(MapBox.GetMapLocalFromGlobal(((EditorBaseWidget)Root).GetMousePosition()));
        }

        protected void Init()
        {
            IsPainting = true;
        }

        protected void End()
        {
            IsPainting = false;
        }

        protected virtual void Empty()
        {
            SetCursorTexture(new Texture(EMPTY_CURSOR_TEXTURE));
        }

        Boolean IsEmpty()
        {
            return Cursor.Texture.Type == EMPTY_CURSOR_TEXTURE.Type;
        }

        protected virtual Boolean CanPaint(Vector2 point)
        {
            return true;
        }

        protected abstract Boolean Paint(Vector2 point);
    }
}
