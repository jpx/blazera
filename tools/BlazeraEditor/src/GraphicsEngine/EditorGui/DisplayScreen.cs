using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraEditor
{
    public class DisplayScreen : Widget
    {
        public enum EMode
        {
            Normal,
            Scroll
        }

        const EMode DEFAULT_MODE = EMode.Normal;
        const float NORMAL_MODE_DEFAULT_SIZE = 5F * 32F;
        const float SCROLL_MODE_DEFAULT_SIZE = 12F * 32F;
        const float PICTURE_SCALE_FACTOR = .7F;

        public EMode Mode { get; private set; }
        Boolean PictureSizeIsValid;

        PictureBox CurrentPicture;

        Boolean DragIsActive = false;
        Vector2 DragPoint;

        public Boolean TextureLocalRectMode = false;

        public event ClickEventHandler ScreenClicked;

        public DisplayScreen(float size = NORMAL_MODE_DEFAULT_SIZE) :
            base()
        {
            Background = new PictureBox(Create.Texture("ObjectScreen"));
            ChangeMode(DEFAULT_MODE, size);
        }

        public void ChangeMode(EMode mode)
        {
            if (mode == EMode.Scroll)
                ChangeMode(mode, SCROLL_MODE_DEFAULT_SIZE);
            else
                ChangeMode(mode, NORMAL_MODE_DEFAULT_SIZE);
        }

        public void ChangeMode(EMode mode, float size)
        {
            Mode = mode;
            BackgroundDimension = new Vector2(size, size);

            TextureLocalRectMode = false;

            EndDrag();
        }

        public override Boolean OnEvent(BlzEvent evt)
        {
            if (evt.IsHandled)
                return base.OnEvent(evt);

            switch (evt.Type)
            {
                case EventType.MouseButtonPressed:

                    if (!BackgroundContainsMouse())
                        break;

                    if (evt.MouseButton.Button != MouseButton.Right)
                        return true;

                    if (Mode != EMode.Scroll)
                        break;

                    if (!PictureSizeIsValid)
                        break;

                    if (!BackgroundContainsMouse())
                        break;

                    InitDrag(GetLocalFromGlobal(new Vector2(evt.MouseButton.X, evt.MouseButton.Y)));

                    return true;

                case EventType.MouseButtonReleased:

                    if (!BackgroundContainsMouse())
                        break;

                    if (evt.MouseButton.Button != MouseButton.Right)
                        return CallScreenClicked(new MouseButtonEventArgs(evt.MouseButton));

                    if (Mode != EMode.Scroll)
                        return CallScreenClicked(new MouseButtonEventArgs(evt.MouseButton));

                    EndDrag();

                    return true;

                case EventType.MouseMoved:
                    
                    if (Mode != EMode.Scroll)
                        break;

                    if (!DragIsActive)
                        break;

                    if (!BackgroundContainsMouse())
                    {
                        EndDrag();
                        break;
                    }

                    Drag(GetLocalFromGlobal(new Vector2(evt.MouseMove.X, evt.MouseMove.Y)));

                    return true;
            }

            return base.OnEvent(evt);
        }

        PictureBox GetBackground()
        {
            return (PictureBox)Background;
        }

        Boolean CallScreenClicked(MouseButtonEventArgs e)
        {
            if (ScreenClicked == null)
                return false;

            ScreenClicked(this, e);

            return true;
        }

        public void SetCurrentPicture(Texture picture)
        {
            if (CurrentPicture == null)
            {
                CurrentPicture = new PictureBox(null);
                AddWidget(CurrentPicture);
            }

            if (picture != null)
            {
                PictureSizeIsValid =
                    picture.Dimension.X > BackgroundDimension.X * PICTURE_SCALE_FACTOR ||
                    picture.Dimension.Y > BackgroundDimension.Y * PICTURE_SCALE_FACTOR;
            }

            CurrentPicture.Texture = picture;

            if (Mode == EMode.Normal)
                AdjustPictureSize();
            else
                AdjustPictureRect();

            CurrentPicture.Center = Center;
        }

        void AdjustPictureSize()
        {
            float factor = 1F;

            if (CurrentPicture.Dimension.X > BackgroundDimension.X * PICTURE_SCALE_FACTOR ||
                CurrentPicture.Dimension.Y > BackgroundDimension.Y * PICTURE_SCALE_FACTOR)
                factor =
                    Math.Max(BackgroundDimension.X * PICTURE_SCALE_FACTOR, BackgroundDimension.Y * PICTURE_SCALE_FACTOR) /
                    Math.Max(CurrentPicture.Dimension.X, CurrentPicture.Dimension.Y);

            CurrentPicture.Dimension *= factor;
        }

        void AdjustPictureRect()
        {
            Int32 right = CurrentPicture.Texture.ImageSubRect.Right;
            Int32 bottom = CurrentPicture.Texture.ImageSubRect.Bottom;

            if (CurrentPicture.Dimension.X > GetAreaDimension().X)
                right = (Int32)(CurrentPicture.Texture.ImageSubRect.Left + GetAreaDimension().X);

            if (CurrentPicture.Dimension.Y > GetAreaDimension().Y)
                bottom = (Int32)(CurrentPicture.Texture.ImageSubRect.Top + GetAreaDimension().Y);

            CurrentPicture.Texture.ImageSubRect = new BlazeraLib.IntRect(
                CurrentPicture.Texture.ImageSubRect.Left,
                CurrentPicture.Texture.ImageSubRect.Top,
                right,
                bottom);
        }

        void InitDrag(Vector2 point)
        {
            DragPoint = point;
            DragIsActive = true;
        }

        void EndDrag()
        {
            DragIsActive = false;
        }

        void Drag(Vector2 point)
        {
            Vector2 areaDimension = GetAreaDimension();

            if (CurrentPicture.Texture.ImageDimension.X <= areaDimension.X &&
                CurrentPicture.Texture.ImageDimension.Y <= areaDimension.Y)
                return;

            Vector2 offset = DragPoint - point;
            DragPoint = point;

            BlazeraLib.IntRect currentImageSubRect = CurrentPicture.Texture.ImageSubRect;

            Int32 left = currentImageSubRect.Left + (Int32)offset.X;
            Int32 top = currentImageSubRect.Top + (Int32)offset.Y;
            Int32 right = currentImageSubRect.Right + (Int32)offset.X;
            Int32 bottom = currentImageSubRect.Bottom + (Int32)offset.Y;

            if (CurrentPicture.Texture.ImageDimension.X > areaDimension.X)
            {
                if (right >= CurrentPicture.Texture.ImageDimension.X && left != currentImageSubRect.Right - (Int32)areaDimension.X)
                    left = currentImageSubRect.Right - (Int32)areaDimension.X;

                else if (left < 0 && right != currentImageSubRect.Left + (Int32)areaDimension.X)
                    right = currentImageSubRect.Left + (Int32)areaDimension.X;
            }
            else
            {
                left = currentImageSubRect.Left;
                right = currentImageSubRect.Right;
            }

            if (CurrentPicture.Texture.ImageDimension.Y > areaDimension.Y)
            {
                if (bottom >= CurrentPicture.Texture.ImageDimension.Y && top != currentImageSubRect.Bottom - (Int32)areaDimension.Y)
                    top = currentImageSubRect.Bottom - (Int32)areaDimension.Y;

                else if (top < 0 && bottom != currentImageSubRect.Top + (Int32)areaDimension.Y)
                    bottom = currentImageSubRect.Top + (Int32)areaDimension.Y;
            }
            else
            {
                top = currentImageSubRect.Top;
                bottom = currentImageSubRect.Bottom;
            }

            CurrentPicture.Texture.ImageSubRect = new BlazeraLib.IntRect(
                left,
                top,
                right,
                bottom);

            AdjustPictureRect();

            CurrentPicture.Center = Center;
        }

        public Vector2 GetGlobalFromLocalTexturePoint(Vector2 point)
        {
            if (CurrentPicture == null || Mode == EMode.Normal || TextureLocalRectMode)
                return point;

            return new Vector2(CurrentPicture.Texture.ImageSubRect.Left, CurrentPicture.Texture.ImageSubRect.Top) + point;
        }

        public Vector2 GetLocalFromGlobalTexturePoint(Vector2 point)
        {
            if (CurrentPicture == null || Mode == EMode.Normal || TextureLocalRectMode)
                return point;

            return point - new Vector2(CurrentPicture.Texture.ImageSubRect.Left, CurrentPicture.Texture.ImageSubRect.Top);
        }

        Vector2 GetAreaDimension()
        {
            return BackgroundDimension * PICTURE_SCALE_FACTOR;
        }

        public override void Reset()
        {
            base.Reset();

            EndDrag();
        }
    }
}
