using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using BlazeraLib;
using SFML.Graphics;

namespace Blazera
{
    /// <summary>
    /// Interface between game client and client connection
    /// Manages information about the client
    /// </summary>
    public class GameSession : PacketHandler
    {
        #region SessionInfo

        class SessionInfo
        {
            public SessionInfo(String login, String initMap, Vector2 initPos)
            {
                this.Login = login;
                this.InitMap = initMap;
                this.InitPos = initPos;
            }

            public String Login;
            public String InitMap;
            public Vector2 InitPos;
        }

        #endregion

        #region Constants

        #endregion

        #region Members

        Boolean OnlineMode;
        ClientConnection ClientConnection;

        SessionInfo Info;

        Thread ReceptionThread;

        int Guid;

        CPlayer Player;
        CMap Map;

        #endregion

        /// <summary>
        /// Default session.
        /// Offline by default.
        /// </summary>
        GameSession()
        {
            OnlineMode = false;

            ReceptionThread = new Thread(Receive);

            AddHandler(PacketType.SERVER_INFO_LOGIN_VALIDATION, HandleLoginValidation);
            AddHandler(PacketType.SERVER_PING, HandlePing);

            SetRelay(CWorld.Instance);
        }

        public void SetPlayer(CPlayer player)
        {
            Player = player;
        }

        public int GetGuid()
        {
            return Guid;
        }

        /// <summary>
        /// Instance of the singleton
        /// </summary>
        static GameSession _instance;
        public static GameSession Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new GameSession();
                return _instance;
            }
            private set { _instance = value; }
        }

        /// <summary>
        /// Initializes the session with online or offline connection mode.
        /// </summary>
        /// <param name="onlineMode">Connection mode of the session</param>
        public void Init(Boolean onlineMode)
        {
            if (OnlineMode = onlineMode)
            {
                ClientConnection = new ClientConnection();

                if (!ConnectionTest())
                {
                    OnlineMode = false;
                    ClientConnection = null;
                    return;
                }

                Info = new SessionInfo(ClientConnection.Login, GameDatas.INIT_MAP, new Vector2());

                ReceptionThread.Start();
            }
        }

        Boolean ConnectionTest()
        {
            if (!ClientConnection.IsConnected)
            {
                Console.Write("Enter a login : ");
                ClientConnection.Login = "Jpx" + RandomHelper.Get(0, 10000);//Console.ReadLine();
                Log.Cl(ClientConnection.Login, ConsoleColor.Green);
                try
                {
                    if (ClientConnection.Connect(GameDatas.SERVER_IP, GameDatas.SERVER_PORT))
                    {
                        Log.Cl("Connected.", ConsoleColor.Green);

                        SendingPacket packet = new SendingPacket(PacketType.CLIENT_REQUEST_LOGIN_VALIDATION);
                        packet.AddString(ClientConnection.Login);
                        return SendPacket(packet);
                    }

                    throw new ConnectionFailedException();
                }
                catch
                {
                    Log.Cl("Connection failed");
                    return false;
                }
            }

            return false;
        }

        void Receive()
        {
            while (ClientConnection.IsConnected)
            {
                RefreshReception();

                ReceptionPacket rcvData = GetPacket();

                if (rcvData == null)
                {
                    Thread.Sleep(5);
                    
                    continue;
                }

                AddReceivedData(rcvData);
            }
        }

        #region Handlers

        bool HandleLoginValidation(ReceptionPacket data)
        {
            Guid = data.ReadGuid();

            Log.Cl("Logged as " + GetLogin() + " Guid " + Guid.ToString(), ConsoleColor.Green);

            SendingPacket sndData = new SendingPacket(PacketType.CLIENT_REQUEST_MAP_LOADING);
            SendPacket(sndData);

            return true;
        }

        bool HandlePing(ReceptionPacket rcvData)
        {
            return true;
        }

        #endregion

        public void Deco()
        {
            ClientConnection.Deco();
        }

        /// <summary>
        /// Specifies if packets were received from the server
        /// </summary>
        /// <returns>If packet were received from the server</returns>
        public Boolean ContainsReceivedPacket()
        {
            return ClientConnection.ContainsReceivedPacket();
        }

        /// <summary>
        /// Get the first packet received from the server since last update
        /// </summary>
        /// <returns>The first packet received from the server</returns>
        public ReceptionPacket GetPacket()
        {
            if (!ClientConnection.ContainsReceivedPacket())
                return null;
            return ClientConnection.GetPacket();
        }

        /// <summary>
        /// Specifies the connection mode of the session
        /// </summary>
        /// <returns>If the session is online</returns>
        public Boolean IsOnline()
        {
            return OnlineMode;
        }

        /// <summary>
        /// Get the login of the session
        /// </summary>
        /// <returns>Login of the session</returns>
        public String GetLogin()
        {
            return Info.Login;
        }

        /// <summary>
        /// Sends a packet of datas to the game server
        /// </summary>
        /// <param name="data">Packet of datas to send</param>
        /// <returns>If the sending was successful</returns>
        public Boolean SendPacket(SendingPacket data)
        {
            try
            {
                if (!ClientConnection.Send(data))
                    return false;

                Log.Cl("Packet sent : " + data.Type.ToString(), ConsoleColor.DarkYellow);
                return true;
            }
            catch (Exception ex)
            {
                Deco();
                return false;
            }
        }
    }
}