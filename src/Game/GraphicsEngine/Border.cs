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
        public static readonly Color DEFAULT_AMBIENT_COLOR = BlazeraLib.Graphics.Color.GetColorFromName(Graphics.Color.ColorName.DarkGreen).ToSFColor();//new Color(71, 60, 139);
        public static readonly Color DEFAULT_FOCUSED_AMBIENT_COLOR = BlazeraLib.Graphics.Color.GetColorFromName(Graphics.Color.ColorName.ForestGreen).ToSFColor();

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
            switch (Mode)
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
            Dimension = dimension;

            Mode = mode;

            NoBackgroundMode = noBackgroundMode;

            if (!NoBackgroundMode)
                Background = new Texture(Border.WindowBackgroundTexture);

            Visible = true;

            Borders = new Dictionary<BorderType, List<Sprite>>();
            foreach (BorderType borderType in Enum.GetValues(typeof(BorderType)))
                Borders[borderType] = new List<Sprite>();

            Corners = new Dictionary<CornerType, Sprite>();

            RefreshInfo = new RefreshInfo();

            Color = DEFAULT_AMBIENT_COLOR;

            BackgroundAlphaFactor = DEFAULT_BACKGROUND_ALPHA_FACTOR;

            Build();
        }

        private void Build()
        {
            Corners.Clear();
            foreach (BorderType borderType in Enum.GetValues(typeof(BorderType)))
                Borders[borderType].Clear();

            if (!NoBackgroundMode)
                Background.Dimension = new Vector2f(
                    Dimension.X - GetBorderDimension(BorderType.Left).X * BACKGROUND_RESIZE_OFFSET_FACTOR,
                    Dimension.Y - GetBorderDimension(BorderType.Top).Y * BACKGROUND_RESIZE_OFFSET_FACTOR);

            AddCorner(CornerType.TopLeft);
            AddCorner(CornerType.TopRight);
            AddCorner(CornerType.BottomLeft);
            AddCorner(CornerType.BottomRight);

            while (GetDimension().X < Dimension.X)
            {
                AddBorder(BorderType.Top);
                AddBorder(BorderType.Bottom);
            }

            while (GetDimension().Y < Dimension.Y)
            {
                AddBorder(BorderType.Left);
                AddBorder(BorderType.Right);
            }

            CutLast();

            SetColor(Color);

            UpdatePosition();
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

        public void Draw(RenderTarget window)
        {
            if (!Visible)
                return;

            if (!NoBackgroundMode)
                Background.Draw(window);

            foreach (Sprite spr in Borders[BorderType.Left])
                window.Draw(spr);
            foreach (Sprite spr in Borders[BorderType.Top])
                window.Draw(spr);
            foreach (Sprite spr in Borders[BorderType.Right])
                window.Draw(spr);
            foreach (Sprite spr in Borders[BorderType.Bottom])
                window.Draw(spr);

            window.Draw(Corners[CornerType.TopLeft]);
            window.Draw(Corners[CornerType.TopRight]);
            window.Draw(Corners[CornerType.BottomLeft]);
            window.Draw(Corners[CornerType.BottomRight]);
        }

        public void Resize(Vector2f scale)
        {
            if (scale.X == 1F &&
                scale.Y == 1F)
                return;

            Dimension = new Vector2f(
                Dimension.X * scale.X,
                Dimension.Y * scale.Y);

            Build();
        }

        public void Move(Vector2f move)
        {
            if (move.X == 0F &&
                move.Y == 0F)
                return;

            Position += move;

            if (!NoBackgroundMode)
                Background.Position += move;

            Corners[CornerType.TopLeft].Position += move;
            Corners[CornerType.TopRight].Position += move;
            Corners[CornerType.BottomLeft].Position += move;
            Corners[CornerType.BottomRight].Position += move;

            foreach (Sprite spr in Borders[BorderType.Left])
                spr.Position += move;
            foreach (Sprite spr in Borders[BorderType.Top])
                spr.Position += move;
            foreach (Sprite spr in Borders[BorderType.Right])
                spr.Position += move;
            foreach (Sprite spr in Borders[BorderType.Bottom])
                spr.Position += move;
        }

        public void SetBackgroundAlphaFactor(double backgroundAlphaFactor)
        {
            BackgroundAlphaFactor = backgroundAlphaFactor;
        }

        public void SetColor(Color color)
        {
            Color = color;

            if (!NoBackgroundMode)
            {
                Background.Color = Color;
                Background.SetAlpha(BackgroundAlphaFactor);
            }

            Corners[CornerType.TopLeft].Color = Color;
            Corners[CornerType.TopRight].Color = Color;
            Corners[CornerType.BottomLeft].Color = Color;
            Corners[CornerType.BottomRight].Color = Color;

            foreach (Sprite spr in Borders[BorderType.Left])
                spr.Color = Color;
            foreach (Sprite spr in Borders[BorderType.Top])
                spr.Color = Color;
            foreach (Sprite spr in Borders[BorderType.Right])
                spr.Color = Color;
            foreach (Sprite spr in Borders[BorderType.Bottom])
                spr.Color = Color;
        }

        public float GetWindowYPos()
        {
            return Position.Y + Dimension.Y - GetBorderDimension(BorderType.Bottom).Y * 2F;
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
            Vector2f topLeftCorner = GetCornerDimension(CornerType.TopLeft);
            Vector2f bottomLeftCorner = GetCornerDimension(CornerType.BottomLeft);
            float minLeftCornerWidth = Math.Min(topLeftCorner.X, bottomLeftCorner.X);
            float minLeftCornerHeight = Math.Min(topLeftCorner.Y, bottomLeftCorner.Y);

            Vector2f topRightCorner = GetCornerDimension(CornerType.TopRight);
            Vector2f bottomRightCorner = GetCornerDimension(CornerType.BottomRight);
            float minRightCornerWidth = Math.Min(topRightCorner.X, bottomRightCorner.X);
            float minRightCornerHeight = Math.Min(topRightCorner.Y, bottomRightCorner.Y);

            float factor = 1F;

            if (Dimension.X < minLeftCornerWidth + minRightCornerWidth)
                factor = Dimension.X / (minLeftCornerWidth + minRightCornerWidth);

            if (Dimension.Y < minLeftCornerHeight + minRightCornerHeight)
                factor = Math.Min(factor, Dimension.Y / (minLeftCornerHeight + minRightCornerHeight));

            return new Vector2f(factor, factor);
        }

        private void AddBorder(BorderType borderType)
        {
            Sprite border = new Sprite(GetBorder(borderType).Sprite);

            //border.Scale = GetScaleFactor();

            Borders[borderType].Add(border);
        }

        private void AddCorner(CornerType cornerType)
        {
            Sprite corner = new Sprite(GetCorner().Sprite);

            //corner.Scale = GetScaleFactor();

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

            Corners[cornerType] = corner;
        }

        private Vector2f GetDimension()
        {
            Vector2f dimension = new Vector2f();

            dimension += new Vector2f(
                GetCorner().Dimension.X + GetCorner().Dimension.Y,
                GetCorner().Dimension.X + GetCorner().Dimension.Y);

            dimension.X += Borders[BorderType.Top].Count * GetBorder(BorderType.Top).Dimension.X;
            dimension.Y += Borders[BorderType.Left].Count * GetBorder(BorderType.Left).Dimension.Y;

            return dimension;
        }

        private Vector2f GetCornerDimension(CornerType cornerType)
        {
            switch (cornerType)
            {
                case CornerType.TopLeft: return new Vector2f(GetCorner().Dimension.Y, GetCorner().Dimension.X);
                case CornerType.TopRight: return GetCorner().Dimension;
                case CornerType.BottomLeft: return GetCorner().Dimension;
                case CornerType.BottomRight: return new Vector2f(GetCorner().Dimension.Y, GetCorner().Dimension.X);
                default: return GetCorner().Dimension;
            }
        }

        private Vector2f GetBorderDimension(BorderType borderType)
        {
            return GetBorder(borderType).Dimension;
        }

        private void UpdatePosition()
        {
            if (!NoBackgroundMode)
                Background.Position = new Vector2f(
                    Position.X + Dimension.X / 2F - Background.Dimension.X / 2F,
                    Position.Y + Dimension.Y / 2F - Background.Dimension.Y / 2F);

            Corners[CornerType.TopLeft].Position = new Vector2f(
                Position.X + GetCornerDimension(CornerType.TopLeft).X / 2F,
                Position.Y + GetCornerDimension(CornerType.TopLeft).Y / 2F);
            Corners[CornerType.TopRight].Position = new Vector2f(
                Position.X + Dimension.X - GetCornerDimension(CornerType.TopLeft).X / 2F,
                Position.Y + GetCornerDimension(CornerType.TopRight).Y / 2F);
            Corners[CornerType.BottomLeft].Position = new Vector2f(
                Position.X + GetCornerDimension(CornerType.BottomLeft).X / 2F,
                Position.Y + Dimension.Y - GetCornerDimension(CornerType.BottomLeft).X / 2F);
            Corners[CornerType.BottomRight].Position = new Vector2f(
                Position.X + Dimension.X - GetCornerDimension(CornerType.BottomRight).X / 2F,
                Position.Y + Dimension.Y - GetCornerDimension(CornerType.BottomRight).Y / 2F);

            for (Int32 count = 0; count < Borders[BorderType.Left].Count; ++count)
                Borders[BorderType.Left][count].Position = new Vector2f(
                    Position.X,
                    Position.Y + GetCornerDimension(CornerType.TopLeft).Y + count * GetBorderDimension(BorderType.Left).Y);

            for (Int32 count = 0; count < Borders[BorderType.Top].Count; ++count)
                Borders[BorderType.Top][count].Position = new Vector2f(
                    Position.X + GetCornerDimension(CornerType.TopLeft).X + count * GetBorderDimension(BorderType.Top).X,
                    Position.Y);

            for (Int32 count = 0; count < Borders[BorderType.Right].Count; ++count)
                Borders[BorderType.Right][count].Position = new Vector2f(
                    Position.X + Dimension.X - GetBorderDimension(BorderType.Right).X,
                    Position.Y + GetCornerDimension(CornerType.TopRight).Y + count * GetBorderDimension(BorderType.Right).Y);

            for (Int32 count = 0; count < Borders[BorderType.Bottom].Count; ++count)
                Borders[BorderType.Bottom][count].Position = new Vector2f(
                    Position.X + GetCornerDimension(CornerType.BottomLeft).X + count * GetBorderDimension(BorderType.Bottom).X,
                    Position.Y + Dimension.Y - GetBorderDimension(BorderType.Bottom).Y);
        }
    }
}
