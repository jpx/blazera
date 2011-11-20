using BlazeraLib;

namespace BlazeraServer
{
    public class PlayerHandler : PacketHandler
    {
        SPlayer Player;

        public PlayerHandler(SPlayer player) :
            base()
        {
            Player = player;

            AddHandler(PacketType.CLIENT_REQUEST_DIRECTION_ENABLED, HandleDirectionEnabled);
            AddHandler(PacketType.CLIENT_REQUEST_DIRECTION_DISABLED, HandleDirectionDisabled);
        }

        public bool HandleDirectionEnabled(ReceptionPacket data)
        {
            Player.EnableDirection(data.ReadDirection());

            return false;
        }

        public bool HandleDirectionDisabled(ReceptionPacket data)
        {
            Player.DisableDirection(data.ReadDirection());

            return false;
        }
    }
}
