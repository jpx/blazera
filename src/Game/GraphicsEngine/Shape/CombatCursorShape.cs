using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public class CombatCursorShape : BaseDrawable
    {
        #region Constants

        static readonly Color DEFAULT_CENTER_COLOR = Graphics.Color.GetColorFromName(Graphics.Color.ColorName.DarkGreen).ToSFColor();
        const byte DEFAULT_CENTER_ALPHA = 128;

        const double CENTERSQUARE_SCALE_FACTOR = 65D;

        const double PERCENTAGE_SCALE_FACTOR = 75D;

        #endregion

        #region Members

        uint Size;

        Shape SideSquare;
        Shape CenterSquare;

        #endregion

        public CombatCursorShape(uint size = CombatMap.CELL_SIZE) :
            base()
        {
            Size = (uint)(size * PERCENTAGE_SCALE_FACTOR / 100D);

            SideSquare = new Shape();
            SideSquare.EnableFill(false);
            SideSquare.EnableOutline(true);
            SideSquare.OutlineThickness = 2;

            CenterSquare = new Shape();
            CenterSquare.EnableFill(true);
            CenterSquare.EnableOutline(true);
            CenterSquare.OutlineThickness = 1;

            Build();
        }

        void Build()
        {
            Color sideColor = new SFML.Graphics.Color(
                DEFAULT_CENTER_COLOR.R,
                DEFAULT_CENTER_COLOR.G,
                DEFAULT_CENTER_COLOR.B,
                DEFAULT_CENTER_COLOR.A);

            Color centerColor = new SFML.Graphics.Color(
                DEFAULT_CENTER_COLOR.R,
                DEFAULT_CENTER_COLOR.G,
                DEFAULT_CENTER_COLOR.B,
                DEFAULT_CENTER_ALPHA);

            SideSquare.AddPoint(new Vector2f(0F, 0F), sideColor, sideColor);
            SideSquare.AddPoint(new Vector2f(Size, 0F), sideColor, sideColor);
            SideSquare.AddPoint(new Vector2f(Size, Size), sideColor, sideColor);
            SideSquare.AddPoint(new Vector2f(0F, Size), sideColor, sideColor);

            float centerOffset = Size * (float)((100D - CENTERSQUARE_SCALE_FACTOR) / 100D / 2D);

            CenterSquare.AddPoint(new Vector2f(centerOffset, centerOffset), centerColor, sideColor);
            CenterSquare.AddPoint(new Vector2f(Size - centerOffset, centerOffset), centerColor, sideColor);
            CenterSquare.AddPoint(new Vector2f(Size - centerOffset, Size - centerOffset), centerColor, sideColor);
            CenterSquare.AddPoint(new Vector2f(centerOffset, Size - centerOffset), centerColor, sideColor);
        }

        public override void Draw(RenderTarget window)
        {
            if (!IsVisible)
                return;

            window.Draw(SideSquare);
            window.Draw(CenterSquare);
        }

        public override Vector2f Position
        {
            set
            {
                base.Position = value;

                SideSquare.Position = Position;
                CenterSquare.Position = Position;
            }
        }

        public override Vector2f Dimension
        {
            get { return new Vector2f(Size, Size); }
        }

        public override object Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}
