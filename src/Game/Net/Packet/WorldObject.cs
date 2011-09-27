namespace BlazeraLib
{
    public partial class SendingPacket
    {
        public void AddDirectionStates(System.Collections.Generic.Dictionary<Direction, bool> directionStates)
        {
            string str = string.Empty;

            foreach (bool b in directionStates.Values)
                str += b ? "1" : "0";

            Append(str);
        }

        public void AddDirection(Direction direction)
        {
            AppendObj((int)direction);
        }

        public void AddDynamicObjectMapAdd(DynamicWorldObject dObj)
        {
            AddGuid(dObj.Guid);
            AddString(dObj.LongType);
            AddVector2(dObj.Position);
            AddDirection(dObj.Direction);
        }

        public void AddObjectMove(DynamicWorldObject dObj)
        {
            AddGuid(dObj.Guid);
            AddVector2(dObj.Position);
            AddDirection(dObj.Direction);
            AddDirectionStates(dObj.DirectionStates);
        }
    }

    public partial class ReceptionPacket
    {
        public Direction ReadDirection()
        {
            return (Direction)System.Enum.ToObject(typeof(Direction), Parser.GetInt());
        }

        public DynamicWorldObject ReadDynamicObjectMapAdd()
        {
            int guid = ReadGuid();

            DynamicWorldObject dObj = (DynamicWorldObject)Create.CreateFromLongType(ReadString());
            dObj.Guid = guid;
            dObj.Position = ReadVector2();
            dObj.Direction = ReadDirection();

            return dObj;
        }

        public System.Collections.Generic.Dictionary<Direction, bool> ReadDirectionStates()
        {
            string str = ReadString();

            return new System.Collections.Generic.Dictionary<Direction, bool>()
            {
                { Direction.N, str[0] == '1' },
                { Direction.NE, str[1] == '1' },
                { Direction.E, str[2] == '1' },
                { Direction.SE, str[3] == '1' },
                { Direction.S, str[4] == '1' },
                { Direction.SO, str[5] == '1' },
                { Direction.O, str[6] == '1' },
                { Direction.NO, str[7] == '1' }
            };
        }
    }
}
