using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using BlazeraLib;

namespace BlazeraServer
{
    /// <summary>
    /// Game server
    /// </summary>
    public class BlazeraServer
    {
        #region Constants

        const int REFRESH_TIME = 50;
        const int WAIT_FOR_START_PERIOD = 1000;
        const int RECEPTION_CHECK_PERIOD = 10;

        const bool PING_MODE = true;
        const int PING_DELAY = 5;

        #endregion

        #region Members

        Boolean IsStarted;

        public Boolean IsKilled { get; private set; }

        List<ClientService> Clients;

        Thread MainThread;
        Thread ReceiveThread;
        Thread UpdateThread;

        Pinger Pinger;

        #endregion

        BlazeraServer()
        {
            Clients = new List<ClientService>();

            MainThread = new Thread(RunServer);
            ReceiveThread = new Thread(RunReceive);
            UpdateThread = new Thread(RunUpdate);

            Pinger = new Pinger(this, PING_DELAY);
        }

        /// <summary>
        /// Initializes unique instance of game server
        /// </summary>
        public void Init()
        {
            ScriptEngine.Instance.Init("GameDatas");
            ImageManager.Instance.Init();

            IsKilled = false;
        }

        /// <summary>
        /// Unique instance of game server
        /// </summary>
        private static BlazeraServer _instance;
        public static BlazeraServer Instance
        {
            get
            {
                if (_instance == null)
                    Instance = new BlazeraServer();
                return _instance;
            }
            private set { _instance = value; }
        }
        
        /// <summary>
        /// Starts game server that waits for clients connections
        /// </summary>
        public void Run()
        {
            MainThread.Start();

            while (!IsStarted)
                Thread.Sleep(WAIT_FOR_START_PERIOD);

            ReceiveThread.Start();
            UpdateThread.Start();

            GameTime.Instance.Init();
        }

        /// <summary>
        /// Main client listening loop
        /// </summary>
        void RunServer()
        {
            TcpListener listener = new TcpListener(IPAddress.Parse(GameDatas.SERVER_IP), GameDatas.SERVER_PORT);

            listener.Start();

            Log.Cl("Server is starting... " + listener.LocalEndpoint, ConsoleColor.Green);

            Log.Cl("______ _                         _____                          \n" +
                   "| ___ \\ |                       /  ___|                         \n" +
                   "| |_/ / | __ _ _______ _ __ __ _\\ `--.  ___ _ ____   _____ _ __ \n" +
                   "| ___ \\ |/ _` |_  / _ \\ '__/ _` |`--. \\/ _ \\ '__\\ \\ / / _ \\ '__|\n" +
                   "| |_/ / | (_| |/ /  __/ | | (_| /\\__/ /  __/ |   \\ V /  __/ |   \n" +
                   "\\____/|_|\\__,_/___\\___|_|  \\__,_\\____/ \\___|_|    \\_/ \\___|_|   \n", ConsoleColor.Blue);
                                                                                                                                                  


            IsStarted = true;

            while (true)
            {
                Socket socket = listener.AcceptSocket();

                if (IsKilled)
                {
                    socket.Close();
                    break;
                }

                Socket socketClient = socket;
                socketClient.ReceiveTimeout = GameDatas.SERVER_IO_TIMEOUT;
                socketClient.SendTimeout = GameDatas.SERVER_IO_TIMEOUT;

                ClientService cs = new ClientService(socketClient);
                AddClient(cs);
            }
        }

        /// <summary>
        /// Clients messages reception main loop
        /// </summary>
        void RunReceive()
        {
            while (!IsKilled)
            {
                for (int i = 0; i < Clients.Count; ++i)
                    ((ClientService)Clients[i]).ReceiveUpdate();

                Thread.Sleep(RECEPTION_CHECK_PERIOD);
            }
        }

        /// <summary>
        /// Clients update main loop
        /// </summary>
        private void RunUpdate()
        {
            BlazeraLib.Timer SleepTimer = new BlazeraLib.Timer();

            while (!IsKilled)
            {
                SleepTimer.Reset();

                Queue<ClientService> droppedClients = new Queue<ClientService>();
                for (int i = 0; i < this.Clients.Count; ++i)
                {
                    ClientService cs = (ClientService)this.Clients[i];
                    if (cs.IsDropped)
                        droppedClients.Enqueue(cs);
                }
                while (droppedClients.Count > 0)
                    RemoveClient(droppedClients.Dequeue());
                
                SWorld.Instance.Update(GameTime.GetDt());

                if (PING_MODE)
                    Pinger.Update(GameTime.Dt);

                Time frameTime = SleepTimer.GetElapsedTime();

                Int32 sleepTime = REFRESH_TIME - (int)frameTime.MS;
                if (sleepTime < 0)
                    sleepTime = 0;

                Thread.Sleep(sleepTime);
            }
        }

        /// <summary>
        /// Sends a packet of data to all the connected clients
        /// </summary>
        /// <param name="packet">Packet of data to send</param>
        public void BroadcastPacket(SendingPacket packet, int[] exceptions = null)
        {
            Log.Cl("Packet broadcasted : " + packet.Type.ToString(), ConsoleColor.DarkMagenta);

            IEnumerator<ClientService> iEnum = this.Clients.GetEnumerator();

            while (iEnum.MoveNext())
                if (exceptions == null || !exceptions.Contains<int>(iEnum.Current.GetGuid()))
                    if (!iEnum.Current.SendPacket(packet, false))
                        iEnum.Current.Deco();
        }

        /// <summary>
        /// Add a client to game server client list
        /// </summary>
        /// <param name="cs">Client to add</param>
        void AddClient(ClientService cs)
        {
            Clients.Add(cs);
        }

        /// <summary>
        /// Remove a client from game server client list
        /// </summary>
        /// <param name="cs">Client to remove</param>
        void RemoveClient(ClientService cs)
        {
            Log.Cl("Client disconnected : " + cs.Login, ConsoleColor.Red);
            Clients.Remove(cs);
        }

        /// <summary>
        /// Specifies if a given login is valid, i-e if it is alpha-numeric and not already existing
        /// </summary>
        /// <param name="login">Login to check</param>
        /// <returns>If the login is valid</returns>
        public static Boolean LoginIsValid(ClientService holder, String login)
        {
            if (login.Contains(" "))
                return false;

            Regex objAlphaNumericPattern = new Regex("[a-zA-Z0-9]");
            if (!objAlphaNumericPattern.IsMatch(login))
                return false;

            foreach (ClientService cs in BlazeraServer.Instance.Clients)
                if (cs != holder &&
                    cs.Login == login)
                    return false;

            return true;
        }
    }
}
