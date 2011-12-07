using BlazeraLib;

namespace BlazeraServer
{
    public class SPlayer : Player
    {
        ClientService Session;

        public PlayerHandler Handler { get; private set; }

        public SPlayer(Player player, ClientService session) :
            base(player)
        {
            Session = session;

            Handler = new PlayerHandler(this);

            OnDirectionEnablement += new DirectionEventHandler(SPlayer_OnDirectionEnablement);
            OnDirectionDisablement += new DirectionEventHandler(SPlayer_OnDirectionEnablement);
        }

        void SPlayer_OnDirectionEnablement(WorldObject sender, DirectionEventArgs e)
        {
            SendInfoMove();
        }

        public bool SendInfoMove()
        {
            try
            {
                SendingPacket data = new SendingPacket(PacketType.SERVER_INFO_MOVE);
                data.AddObjectMove(this);
                return Session.SendPacket(data, true);
            }
            catch
            {
                return false;
            }
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            Handler.RefreshReception();
        }

        public override string LongType
        {
            get
            {
                return "Player_" + Type;
            }
        }

        public override string FullType
        {
            get
            {
                return "Player/" + LongType;
            }
        }
    }
}
