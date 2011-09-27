using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BlazeraLib;

namespace Blazera
{
    public class CPlayer : Player
    {
        GameSession Session;
        public PlayerHandler Handler { get; private set; }

        public CPlayer() :
            base()
        {
            Handler = new PlayerHandler(this);

            Session = GameSession.Instance;

            OnDirectionEnablement += new DirectionEventHandler(CPlayer_OnDirectionEnablement);
            OnDirectionDisablement += new DirectionEventHandler(CPlayer_OnDirectionDisablement);
        }

        public CPlayer(Player copy) :
            base(copy)
        {
            Session = GameSession.Instance;

            OnDirectionEnablement += new DirectionEventHandler(CPlayer_OnDirectionEnablement);
            OnDirectionDisablement += new DirectionEventHandler(CPlayer_OnDirectionDisablement);
        }

        void CPlayer_OnDirectionDisablement(WorldObject sender, DirectionEventArgs e)
        {
            SendDirectionDisablement(e.Direction);
        }

        void CPlayer_OnDirectionEnablement(WorldObject sender, DirectionEventArgs e)
        {
            SendDirectionEnablement(e.Direction);
        }

        public Boolean SendDirectionEnablement(Direction direction)
        {
            try
            {
                SendingPacket data = new SendingPacket(PacketType.CLIENT_REQUEST_DIRECTION_ENABLED);
                data.AddGuid(Guid);
                data.AddDirection(direction);
                return Session.SendPacket(data);
            }
            catch
            {
                return false;
            }
        }

        public Boolean SendDirectionDisablement(Direction direction)
        {
            try
            {
                SendingPacket data = new SendingPacket(PacketType.CLIENT_REQUEST_DIRECTION_DISABLED);
                data.AddGuid(Guid);
                data.AddDirection(direction);
                return Session.SendPacket(data);
            }
            catch
            {
                return false;
            }
        }
    }
}
