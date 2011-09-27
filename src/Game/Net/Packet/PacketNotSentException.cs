using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlazeraLib
{
    public class PacketNotSentException : Exception
    {
        String Login;
        SendingPacket Packet;

        public PacketNotSentException(String login, SendingPacket packet) :
            base()
        {
            Login = login;
            Packet = packet;
        }

        public override String Message
        {
            get
            {
                return "Failed to send " + Packet.ToString() + " to : " + Login; 
            }
        }
    }
}
