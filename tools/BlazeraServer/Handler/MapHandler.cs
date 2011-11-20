using BlazeraLib;

namespace BlazeraServer
{
    public class MapHandler : PacketHandler
    {
        SMap Map;

        public MapHandler(SMap map)
        {
            Map = map;
        }
    }
}
