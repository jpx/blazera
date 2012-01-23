namespace BlazeraLib
{
    public class PlayerHeaderInfoPanel : InfoPanel
    {
        #region Constants

        const float DEFAULT_PLAYER_Y_MARGIN = 10F;

        #endregion

        #region Members

        Player Player;

        #endregion

        public PlayerHeaderInfoPanel(Player player) :
            base()
        {
            Player = player;

           // Player.OnMove += new MoveEventHandler(Player_OnMove);
        }

        void Player_OnMove(IDrawable sender, MoveEventArgs e)
        {
           // Center = GetGuiPointFromMapPoint(new SFML.Window.Vector2f(Player.Center.X, Center.Y));
           // Bottom = Player.Top - DEFAULT_PLAYER_Y_MARGIN;
        }
    }
}
