using BlazeraLib;

namespace BlazeraServer
{
    class Pinger
    {
        PTimer Timer;
        BlazeraServer Server;

        public Pinger(BlazeraServer server, double delay)
        {
            Timer = new PTimer(delay, Ping);
            Server = server;
        }

        public void Update(Time dt)
        {
            Timer.Update(dt);
        }

        void Ping()
        {
            SendingPacket sndData = new SendingPacket(PacketType.SERVER_PING);
            Server.BroadcastPacket(sndData);
        }
    }
}
