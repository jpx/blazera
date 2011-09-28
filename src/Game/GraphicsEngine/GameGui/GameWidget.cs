using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    /// <summary>
    /// A widget constituting the game gui
    /// </summary>
    public abstract class GameWidget : Widget
    {
        #region Enums

        public enum ELocation
        {
            Desactivated,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight,
            MidLeft,
            MidTop,
            MidRight,
            MidBottom
        }

        #endregion

        #region Constants

        const ELocation DEFAULT_LOCATION = ELocation.Desactivated;

        #endregion

        #region Members

        ELocation Location;

        #endregion

        /// <summary>
        /// Constructs a GameWidget by default
        /// </summary>
        public GameWidget() :
            base()
        {
            Location = DEFAULT_LOCATION;
        }

        /// <summary>
        /// Base widget of the game
        /// </summary>
        /// <returns>Base widget of the game</returns>
        protected GameBaseWidget GetRoot()
        {
            return (GameBaseWidget)Root;
        }

        public void SetFirst()
        {
            Open();
            GetRoot().SetFirst(this);
        }

        /// <summary>
        /// Converts a map local point into a game gui local point
        /// </summary>
        /// <param name="mapPoint">Map local point</param>
        /// <returns>Game gui local point that results from the conversion</returns>
        protected Vector2 GetGuiPointFromMapPoint(Vector2 mapPoint)
        {
            if (GetRoot() == null)
                return mapPoint;

            return mapPoint - (GetRoot().MapView.Center - GetRoot().MapView.Size / 2F);
        }

        /// <summary>
        /// Converts a game gui local point into a map local point
        /// </summary>
        /// <param name="guiPoint">Game gui local point</param>
        /// <returns>Map local point that results from the conversion</returns>
        protected Vector2 GetMapPointFromGuiPoint(Vector2 guiPoint)
        {
            if (GetRoot() == null)
                return guiPoint;

            return guiPoint + GetRoot().MapView.Center - GetRoot().GuiView.Center;
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            switch (Location)
            {
                case ELocation.Desactivated:
                    return;

                case ELocation.TopLeft:
                    Left = GetRoot().Left;
                    Top = GetRoot().Top;
                    break;

                case ELocation.TopRight:
                    BackgroundRight = GetRoot().BackgroundRight;
                    Top = GetRoot().Top;
                    break;

                case ELocation.BottomLeft:
                    Left = GetRoot().Left;
                    BackgroundBottom = GetRoot().BackgroundBottom;
                    break;

                case ELocation.BottomRight:
                    BackgroundRight = GetRoot().BackgroundRight;
                    BackgroundBottom = GetRoot().BackgroundBottom;
                    break;

                case ELocation.MidLeft:
                    Left = GetRoot().Left;
                    BackgroundCenter = new Vector2(BackgroundCenter.X, GetRoot().BackgroundCenter.Y);
                    break;

                case ELocation.MidTop:
                    Top = GetRoot().Top;
                    BackgroundCenter = new Vector2(GetRoot().BackgroundCenter.X, BackgroundCenter.Y);
                    break;

                case ELocation.MidRight:
                    BackgroundRight = GetRoot().BackgroundRight;
                    BackgroundCenter = new Vector2(BackgroundCenter.X, GetRoot().BackgroundCenter.Y);
                    break;

                case ELocation.MidBottom:
                    BackgroundBottom = GetRoot().BackgroundBottom;
                    BackgroundCenter = new Vector2(GetRoot().BackgroundCenter.X, BackgroundCenter.Y);
                    break;
            }
        }

        public void SetLocation(ELocation location)
        {
            Location = location;
        }
    }
}
