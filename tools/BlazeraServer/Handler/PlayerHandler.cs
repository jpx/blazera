using System;
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
            try
            {
                Player.EnableDirection(data.ReadDirection());
            }
            catch (Exception e)
            {
                Log.Clerr(e.Message);

                throw new Exception("Failed to read Direction from packet.");
            }

            return true;
        }

        public bool HandleDirectionDisabled(ReceptionPacket data)
        {
            Player.DisableDirection(data.ReadDirection());

            return false;
        }
    }
}
