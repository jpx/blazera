using BlazeraLib;

namespace Blazera
{
    public class CWorld : World
    {
        #region Singleton

        static CWorld _instance;
        public static CWorld Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new CWorld();
                return _instance;
            }
            set { _instance = value; }
        }

        public void Init() { }

        #endregion

        CMap CurrentMap;

        public DrawOrder DrawOrder { get; set; }

        CWorld() :
            base()
        {
            AddHandler(PacketType.SERVER_INFO_MAP_LOADING, HandleMapLoading);
            AddHandler(PacketType.SERVER_INFO_MAP_OBJECT_ADD, HandleMapObjectAdd);
            AddHandler(PacketType.SERVER_INFO_MAP_OBJECT_DELETION, HandleMapObjectDeletion);
            AddHandler(PacketType.SERVER_INFO_MOVE, HandleObjectMove);
        }

        public override void Update(Time dt)
        {
            base.Update(dt);

            if (CurrentMap == null)
                return;

            CurrentMap.Update(dt);
        }

        public void Draw(SFML.Graphics.RenderWindow window)
        {
            if (CurrentMap == null)
                return;

            CurrentMap.Draw(window);
        }

        public void SetCurrentMap(CMap map)
        {
            CurrentMap = map;
        }

        public CMap GetCurrentMap()
        {
            return CurrentMap;
        }

        public override bool AddMap(string mapType)
        {
            if (!base.AddMap(mapType))
                return false;

            CMap map = new CMap(Create.Map(mapType));

            if (IsEmpty())
                SetCurrentMap(map);

            AddMap(map);

            return true;
        }

        #region Handlers

        bool HandleMapLoading(ReceptionPacket rcvData)
        {
            AddMap(rcvData.ReadString());

            int maxCount = rcvData.ReadCount();
            //!\\ TODO : add all dynamic objects, causes exception on HandleObjectMove //!\\
            for (int count = 0; count < maxCount; ++count)
            {
                if (count == 0)
                {
                    PlayerHdl.Instance.Init(new CPlayer((Player)rcvData.ReadDynamicObjectMapAdd()));
                    PlayerHdl.Warp(CurrentMap);
                }
            }
             
            return true;
        }

        bool HandleMapObjectAdd(ReceptionPacket rcvData)
        {
            DynamicWorldObject dObj = rcvData.ReadDynamicObjectMapAdd();
            dObj.SetMap(CurrentMap, dObj.Position.X, dObj.Position.Y);
            CurrentMap.AddDynamicObject(dObj, dObj.Position.X, dObj.Position.Y);
            return true;
        }

        bool HandleMapObjectDeletion(ReceptionPacket rcvData)
        {
            CurrentMap.RemoveObject(rcvData.ReadGuid());

            return true;
        }

        bool HandleObjectMove(ReceptionPacket rcvData)
        {
            DynamicWorldObject dObj = CurrentMap.GetObject(rcvData.ReadGuid());
            dObj.MoveTo(rcvData.ReadVector2());
            dObj.Direction = rcvData.ReadDirection();
            dObj.ResetDirectionStates(rcvData.ReadDirectionStates());

            Log.Cldebug(dObj.Guid, "Moved oject", System.ConsoleColor.Cyan);

            return true;
        }

        #endregion
    }
}
