using BlazeraLib;

namespace Blazera
{
    public class PlayerHandler : PacketHandler
    {
        CPlayer Player;

        public PlayerHandler(CPlayer player)
        {
            Player = player;
        }
    }
}
