using System.Collections.Generic;

namespace BlazeraLib
{
    public delegate bool DPacketHandler(ReceptionPacket data);

    public abstract class PacketHandler
    {
        Dictionary<PacketType, DPacketHandler> Handlers;
        Queue<ReceptionPacket> ReceivedData;
        PacketHandler Relay;

        public PacketHandler()
        {
            Handlers = new Dictionary<PacketType, DPacketHandler>();
            ReceivedData = new Queue<ReceptionPacket>();
        }

        public void SetRelay(PacketHandler relay)
        {
            Relay = relay;
        }

        protected void AddHandler(PacketType type, DPacketHandler handler)
        {
            if (Handlers.ContainsKey(type))
                Handlers[type] = handler;
            else
                Handlers.Add(type, handler);
        }

        public virtual bool HandlePacket(ReceptionPacket data)
        {
            if (!Handlers.ContainsKey(data.Type))
                return false;

            return Handlers[data.Type].Invoke(data);
        }

        public void AddReceivedData(ReceptionPacket rcvData)
        {
            ReceivedData.Enqueue(rcvData);
        }

        protected bool DataIsReceived()
        {
            return ReceivedData.Count > 0;
        }

        protected ReceptionPacket GetReceivedData()
        {
            return ReceivedData.Dequeue();
        }

        public virtual void RefreshReception()
        {
            while (DataIsReceived())
            {
                ReceptionPacket rcvData = GetReceivedData();
                if (!HandlePacket(rcvData) &&
                    Relay != null)
                    Relay.AddReceivedData(rcvData);
            }
        }
    }
}
