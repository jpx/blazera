using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public enum BorderType
    {
        Left = 0,
        Top = 1,
        Right = 2,
        Bottom = 3
    }

    public enum CornerType
    {
        TopLeft = 0,
        TopRight = 1,
        BottomLeft = 2,
        BottomRight = 3
    }

    public class Border
    {
        public static readonly Color DEFAULT_AMBIENT_COLOR = new Color(71, 60, 139);

        public enum EMode
        {
            Window,
            Box
        }

        static Dictionary<EMode, Dictionary<BorderType, Texture>> Textures;
        static Dictionary<BorderType, Texture> BoxBorderTextures;

        private static Texture WindowCornerTexture { get; set; } // bottom left corner
        private static Texture WindowBackgroundTexture { get; set; }

        private static Texture BoxCornerTexture { get; set; }

        const float BACKGROUND_RESIZE_OFFSET_FACTOR = 1.5F;
        const double DEFAULT_BACKGROUND_ALPHA_FACTOR = 60D;//45D;

        public static void Init()
        {
            Textures = new Dictionary<EMode, Dictionary<BorderType, Texture>>();

            foreach (EMode mode in Enum.GetValues(typeof(EMode)))
                Textures[mode] = new Dictionary<BorderType, Texture>();

            Textures[EMode.Window][BorderType.Left] = Create.Texture("Gui_WindowLeftBorder");
            Textures[EMode.Window][BorderType.Top] = Create.Texture("Gui_WindowTopBorder");
            Textures[EMode.Window][BorderType.Right] = Create.Texture("Gui_WindowRightBorder");
            Textures[EMode.Window][BorderType.Bottom] = Create.Texture("Gui_WindowBottomBorder");

            Textures[EMode.Box][BorderType.Left] = Create.Texture("Gui_BoxLeftBorder");
            Textures[EMode.Box][BorderType.Top] = Create.Texture("Gui_BoxTopBorder");
            Textures[EMode.Box][BorderType.Right] = Create.Texture("Gui_BoxRightBorder");
            Textures[EMode.Box][BorderType.Bottom] = Create.Texture("Gui_BoxBottomBorder");

            foreach (EMode mode in Enum.GetValues(typeof(EMode)))
                foreach (BorderType borderType in Enum.GetValues(typeof(BorderType)))
                    Textures[mode][borderType].Color = DEFAULT_AMBIENT_COLOR;

            WindowCornerTexture = Create.Texture("Gui_WindowCorner");
            WindowBackgroundTexture = Create.Texture("Gui_BorderBackground");

            BoxCornerTexture = Create.Texture("Gui_BoxCorner");

            WindowCornerTexture.Color = Border.DEFAULT_AMBIENT_COLOR;
            WindowBackgroundTexture.Color = Border.DEFAULT_AMBIENT_COLOR;

            BoxCornerTexture.Color = Border.DEFAULT_AMBIENT_COLOR;
        }

        private Texture GetBorder(BorderType borderType)
        {
            return Textures[Mode][borderType];
        }

        private Texture GetCorner()
        {
            switch (this.Mode)
            {
                case EMode.Window:  return Border.WindowCornerTexture;
                case EMode.Box:     return Border.BoxCornerTexture;
                default:            return null;
            }
        }

        private Dictionary<BorderType, List<Sprite>> Borders { get; set; }
        private Dictionary<CornerType, Sprite> Corners { get; set; }

        public Vector2f Position { get; protected set; }
        public Vector2f Dimension { get; protected set; }

        private Texture Background { get; set; }
        private Boolean NoBackgroundMode { get; set; }

        private EMode Mode { get; set; }

        public Boolean Visible { get; set; }

        private RefreshInfo RefreshInfo { get; set; }

        private Color Color { get; set; }
        double BackgroundAlphaFactor;

        public Border(Vector2f dimension, EMode mode = EMode.Window, Boolean noBackgroundMode = false)
        {
            this.Dimension = dimension;

            this.Mode = mode;

            NoBackgroundMode = noBackgroundMode;

            if (!this.NoBackgroundMode)
                this.Background = new Texture(Border.WindowBackgroundTexture);

            this.Visible = true;

            this.Borders = new Dictionary<BorderType, List<Sprite>>();
            foreach (BorderType borderType in Enum.GetValues(typeof(BorderType)))
                this.Borders[borderType] = new List<Sprite>();

            this.Corners = new Dictionary<CornerType, Sprite>();

            this.RefreshInfo = new RefreshInfo();

            this.Color = DEFAULT_AMBIENT_COLOR;

            BackgroundAlphaFactor = DEFAULT_BACKGROUND_ALPHA_FACTOR;

            this.Build();
        }

        private void Build()
        {
            this.Corners.Clear();
            foreach (BorderType borderType in Enum.GetValues(typeof(BorderType)))
                this.Borders[borderType].Clear();

            if (!this.NoBackgroundMode)
                this.Background.Dimension = new Vector2f(
                    this.Dimension.X - this.GetBorderDimension(BorderType.Left).X * BACKGROUND_RESIZE_OFFSET_FACTOR,
                    this.Dimension.Y - this.GetBorderDimension(BorderType.Top).Y * BACKGROUND_RESIZE_OFFSET_FACTOR);

            this.AddCorner(CornerType.TopLeft);
            this.AddCorner(CornerType.TopRight);
            this.AddCorner(CornerType.BottomLeft);
            this.AddCorner(CornerType.BottomRight);

            while (this.GetDimension().X < this.Dimension.X)
            {
                this.AddBorder(BorderType.Top);
                this.AddBorder(BorderType.Bottom);
            }

            while (this.GetDimension().Y < this.Dimension.Y)
            {
                this.AddBorder(BorderType.Left);
                this.AddBorder(BorderType.Right);
            }

            CutLast();

            this.SetColor(this.Color);

            this.UpdatePosition();
        }

        void CutLast()
        {
            if (Borders[BorderType.Left].Count == 0 ||
                Borders[BorderType.Top].Count == 0 ||
                Borders[BorderType.Right].Count == 0 ||
                Borders[BorderType.Bottom].Count == 0)
                return;

            const Int32 MARGIN = 1;

            if (GetDimension().X != Dimension.X)
            {
                Sprite topLast = Borders[BorderType.Top][Borders[BorderType.Top].Count - 1];
                topLast.SubRect = new SFML.Graphics.IntRect(
                    0,
                    0,
                    (Int32)(topLast.Width - (GetDimension().X - Dimension.X)) + MARGIN,
                    (Int32)topLast.Height);

                Sprite bottomLast = Borders[BorderType.Bottom][Borders[BorderType.Bottom].Count - 1];
                bottomLast.SubRect = new SFML.Graphics.IntRect(
                    0,
                    0,
                    (Int32)(bottomLast.Width - (GetDimension().X - Dimension.X)) + MARGIN,
                    (Int32)bottomLast.Height);
            }

            if (GetDimension().Y != Dimension.Y)
            {
                Sprite leftLast = Borders[BorderType.Left][Borders[BorderType.Left].Count - 1];
                leftLast.SubRect = new SFML.Graphics.IntRect(
                    0,
                    0,
                    (Int32)leftLast.Width,
                    (Int32)(leftLast.Height - (GetDimension().Y - Dimension.Y)) + MARGIN);

                Sprite rightLast = Borders[BorderType.Right][Borders[BorderType.Right].Count - 1];
                rightLast.SubRect = new SFML.Graphics.IntRect(
                    0,
                    0,
                    (Int32)rightLast.Width,
                    (Int32)(rightLast.Height - (GetDimension().Y - Dimension.Y)) + MARGIN);
            }
        }

        public void Draw(RenderWindow window)
        {
            if (!this.Visible)
                return;

            if (!this.NoBackgroundMode)
                this.Background.Draw(window);

            foreach (Sprite spr in this.Borders[BorderType.Left])
                window.Draw(spr);
            foreach (Sprite spr in this.Borders[BorderType.Top])
                window.Draw(spr);
            foreach (Sprite spr in this.Borders[BorderType.Right])
                window.Draw(spr);
            foreach (Sprite spr in this.Borders[BorderType.Bottom])
                window.Draw(spr);

            window.Draw(this.Corners[CornerType.TopLeft]);
            window.Draw(this.Corners[CornerType.TopRight]);
            window.Draw(this.Corners[CornerType.BottomLeft]);
            window.Draw(this.Corners[CornerType.BottomRight]);
        }

        public void Resize(Vector2f scale)
        {
            if (scale.X == 1F &&
                scale.Y == 1F)
                return;

            this.Dimension = new Vector2f(
                this.Dimension.X * scale.X,
                this.Dimension.Y * scale.Y);

            this.Build();
        }

        public void Move(Vector2f move)
        {
            if (move.X == 0F &&
                move.Y == 0F)
                return;

            this.Position += move;

            if (!this.NoBackgroundMode)
                this.Background.Position += move;

            this.Corners[CornerType.TopLeft].Position += move;
            this.Corners[CornerType.TopRight].Position += move;
            this.Corners[CornerType.BottomLeft].Position += move;
            this.Corners[CornerType.BottomRight].Position += move;

            foreach (Sprite spr in this.Borders[BorderType.Left])
                spr.Position += move;
            foreach (Sprite spr in this.Borders[BorderType.Top])
                spr.Position += move;
            foreach (Sprite spr in this.Borders[BorderType.Right])
                spr.Position += move;
            foreach (Sprite spr in this.Borders[BorderType.Bottom])
                spr.Position += move;
        }

        public void SetBackgroundAlphaFactor(double backgroundAlphaFactor)
        {
            BackgroundAlphaFactor = backgroundAlphaFactor;
        }

        public void SetColor(Color color)
        {
            this.Color = color;

            if (!this.NoBackgroundMode)
            {
                this.Background.Color = this.Color;
                Background.SetAlpha(BackgroundAlphaFactor);
            }

            this.Corners[CornerType.TopLeft].Color = this.Color;
            this.Corners[CornerType.TopRight].Color = this.Color;
            this.Corners[CornerType.BottomLeft].Color = this.Color;
            this.Corners[CornerType.BottomRight].Color = this.Color;

            foreach (Sprite spr in this.Borders[BorderType.Left])
                spr.Color = this.Color;
            foreach (Sprite spr in this.Borders[BorderType.Top])
                spr.Color = this.Color;
            foreach (Sprite spr in this.Borders[BorderType.Right])
                spr.Color = this.Color;
            foreach (Sprite spr in this.Borders[BorderType.Bottom])
                spr.Color = this.Color;
        }

        public float GetWindowYPos()
        {
            return this.Position.Y + this.Dimension.Y - this.GetBorderDimension(BorderType.Bottom).Y * 2F;
        }

        public static float GetWindowBorderWidth()
        {
            return Textures[EMode.Window][BorderType.Left].Dimension.X;
        }

        public static float GetBoxBorderWidth()
        {
            return Textures[EMode.Box][BorderType.Left].Dimension.X;
        }

        private Vector2f GetScaleFactor()
        {
            Vector2f topLeftCorner = this.GetCornerDimension(CornerType.TopLeft);
            Vector2f bottomLeftCorner = this.GetCornerDimension(CornerType.BottomLeft);
            float minLeftCornerWidth = Math.Min(topLeftCorner.X, bottomLeftCorner.X);
            float minLeftCornerHeight = Math.Min(topLeftCorner.Y, bottomLeftCorner.Y);

            Vector2f topRightCorner = this.GetCornerDimension(CornerType.TopRight);
            Vector2f bottomRightCorner = this.GetCornerDimension(CornerType.BottomRight);
            float minRightCornerWidth = Math.Min(topRightCorner.X, bottomRightCorner.X);
            float minRightCornerHeight = Math.Min(topRightCorner.Y, bottomRightCorner.Y);

            float factor = 1F;

            if (this.Dimension.X < minLeftCornerWidth + minRightCornerWidth)
                factor = this.Dimension.X / (minLeftCornerWidth + minRightCornerWidth);

            if (this.Dimension.Y < minLeftCornerHeight + minRightCornerHeight)
                factor = Math.Min(factor, this.Dimension.Y / (minLeftCornerHeight + minRightCornerHeight));

            return new Vector2f(factor, factor);
        }

        private void AddBorder(BorderType borderType)
        {
            Sprite border = new Sprite(this.GetBorder(borderType).Sprite);

            //border.Scale = this.GetScaleFactor();

            this.Borders[borderType].Add(border);
        }

        private void AddCorner(CornerType cornerType)
        {
            Sprite corner = new Sprite(this.GetCorner().Sprite);

            //corner.Scale = this.GetScaleFactor();

            corner.Origin = new Vector2f(
                corner.Width / 2F,
                corner.Height / 2F);

            switch (cornerType)
            {
                case CornerType.TopLeft:        corner.Rotation = 90F; break;
                case CornerType.TopRight:       corner.Rotation = 180F; break;
                case CornerType.BottomLeft:     corner.Rotation = 0F;   break;
                case CornerType.BottomRight:    corner.Rotation = 270F;  break;
                default:                                                break;
            }

            this.Corners[cornerType] = corner;
        }

        private Vector2f GetDimension()
        {
            Vector2f dimension = new Vector2f();

            dimension += new Vector2f(
                this.GetCorner().Dimension.X + this.GetCorner().Dimension.Y,
                this.GetCorner().Dimension.X + this.GetCorner().Dimension.Y);

            dimension.X += this.Borders[BorderType.Top].Count * this.GetBorder(BorderType.Top).Dimension.X;
            dimension.Y += this.Borders[BorderType.Left].Count * this.GetBorder(BorderType.Left).Dimension.Y;

            return dimension;
        }

        private Vector2f GetCornerDimension(CornerType cornerType)
        {
            switch (cornerType)
            {
                case CornerType.TopLeft: return new Vector2f(this.GetCorner().Dimension.Y, this.GetCorner().Dimension.X);
                case CornerType.TopRight: return this.GetCorner().Dimension;
                case CornerType.BottomLeft: return this.GetCorner().Dimension;
                case CornerType.BottomRight: return new Vector2f(this.GetCorner().Dimension.Y, this.GetCorner().Dimension.X);
                default: return this.GetCorner().Dimension;
            }
        }

        private Vector2f GetBorderDimension(BorderType borderType)
        {
            return GetBorder(borderType).Dimension;
        }

        private void UpdatePosition()
        {
            if (!this.NoBackgroundMode)
                this.Background.Position = new Vector2f(
                    this.Position.X + this.Dimension.X / 2F - this.Background.Dimension.X / 2F,
                    this.Position.Y + this.Dimension.Y / 2F - this.Background.Dimension.Y / 2F);

            this.Corners[CornerType.TopLeft].Position = new Vector2f(
                this.Position.X + this.GetCornerDimension(CornerType.TopLeft).X / 2F,
                this.Position.Y + this.GetCornerDimension(CornerType.TopLeft).Y / 2F);
            this.Corners[CornerType.TopRight].Position = new Vector2f(
                this.Position.X + this.Dimension.X - this.GetCornerDimension(CornerType.TopLeft).X / 2F,
                this.Position.Y + this.GetCornerDimension(CornerType.TopRight).Y / 2F);
            this.Corners[CornerType.BottomLeft].Position = new Vector2f(
                this.Position.X + this.GetCornerDimension(CornerType.BottomLeft).X / 2F,
                this.Position.Y + this.Dimension.Y - this.GetCornerDimension(CornerType.BottomLeft).X / 2F);
            this.Corners[CornerType.BottomRight].Position = new Vector2f(
                this.Position.X + this.Dimension.X - this.GetCornerDimension(CornerType.BottomRight).X / 2F,
                this.Position.Y + this.Dimension.Y - this.GetCornerDimension(CornerType.BottomRight).Y / 2F);

            for (Int32 count = 0; count < this.Borders[BorderType.Left].Count; ++count)
                this.Borders[BorderType.Left][count].Position = new Vector2f(
                    this.Position.X,
                    this.Position.Y + this.GetCornerDimension(CornerType.TopLeft).Y + count * this.GetBorderDimension(BorderType.Left).Y);

            for (Int32 count = 0; count < this.Borders[BorderType.Top].Count; ++count)
                this.Borders[BorderType.Top][count].Position = new Vector2f(
                    this.Position.X + this.GetCornerDimension(CornerType.TopLeft).X + count * this.GetBorderDimension(BorderType.Top).X,
                    this.Position.Y);

            for (Int32 count = 0; count < this.Borders[BorderType.Right].Count; ++count)
                this.Borders[BorderType.Right][count].Position = new Vector2f(
                    this.Position.X + this.Dimension.X - this.GetBorderDimension(BorderType.Right).X,
                    this.Position.Y + this.GetCornerDimension(CornerType.TopRight).Y + count * this.GetBorderDimension(BorderType.Right).Y);

            for (Int32 count = 0; count < this.Borders[BorderType.Bottom].Count; ++count)
                this.Borders[BorderType.Bottom][count].Position = new Vector2f(
                    this.Position.X + this.GetCornerDimension(CornerType.BottomLeft).X + count * this.GetBorderDimension(BorderType.Bottom).X,
                    this.Position.Y + this.Dimension.Y - this.GetBorderDimension(BorderType.Bottom).Y);
        }
    }
}
