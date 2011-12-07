using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using BlazeraLib;

namespace BlazeraServer
{
    public class ClientService : PacketHandler
    {
        #region Members

        static int CurrentGuid = 0;

        Socket SocketClient;

        NetworkStream Ns;

        BinaryReader Br;

        BinaryWriter Bw;

        public Boolean IsDropped { get; private set; }

        public String Login { get; private set; }

        SPlayer SPlayer;

        int Guid;

        #endregion

        public ClientService(Socket socketClient)
        {
            IsDropped = false;
            SocketClient = socketClient;
            Ns = new NetworkStream(SocketClient);
            Br = new BinaryReader(Ns);
            Bw = new BinaryWriter(Ns);

            AddHandler(PacketType.CLIENT_REQUEST_LOGIN_VALIDATION, HandleLoginValidation);
            AddHandler(PacketType.CLIENT_INFO_DECONNECTION, HandleClientDeconnection);
            AddHandler(PacketType.CLIENT_REQUEST_MAP_LOADING, HandleMapLoading);

            Guid = CurrentGuid++;
        }

        public int GetGuid()
        {
            return Guid;
        }

        public void Deco()
        {
            if (IsDropped)
                return;

            IsDropped = true;

            SWorld.Instance.GetMap(SPlayer.Map.Type).RemoveObject(SPlayer);

            SendingPacket sndData = new SendingPacket(PacketType.SERVER_INFO_MAP_OBJECT_DELETION);
            sndData.AddGuid(Guid);
            SendPacket(sndData, true, true);
        }

        public void ReceiveUpdate()
        {
            try
            {
                RefreshReception();

                if (!Ns.DataAvailable)
                {
                    
                    return;
                }

                ReceptionPacket data = new ReceptionPacket(Br);

                Log.Cl("Packet received : " + data.Type.ToString() + " from : " + (Login == null ? "new player ( " + SocketClient.LocalEndPoint.ToString() + " )" : Login), ConsoleColor.DarkYellow);

                AddReceivedData(data);
            }

            catch (Exception ex)
            {
                Log.Clerr(ex.Message);
                Deco();
            }
        }

        #region Handlers

        bool HandleLoginValidation(ReceptionPacket data)
        {
            Login = data.ReadString();

            SendingPacket rspData;

            if (BlazeraServer.LoginIsValid(this, Login))
            {
                rspData = new SendingPacket(PacketType.SERVER_INFO_LOGIN_VALIDATION);
                rspData.AddGuid(Guid);

                Log.Cl("Client connected : " + Login, ConsoleColor.Green);
            }
            else
            {
                rspData = new SendingPacket(PacketType.SERVER_ERROR_LOGIN_VALIDATION);
            }

            SendPacket(rspData, false);

            return true;
        }

        bool HandleClientDeconnection(ReceptionPacket rcvData)
        {
            Deco();

            return true;
        }

        /// <summary>
        /// Happens when the client is loading the map. Its main player is added to the server map and sent to all clients.
        /// </summary>
        /// <param name="rcvData">Received data from client</param>
        /// <returns>If the handling is successful</returns>
        bool HandleMapLoading(ReceptionPacket rcvData)
        {
            /***
             * Chargement des elements dynamiques de la map
             * (DataBase...)
             * 
             * 
             */

            SPlayer = new SPlayer(Create.Player("Vlad"), this);
            SPlayer.Guid = Guid;
            string map = GameDatas.INIT_MAP;
            SWorld.Instance.AddMap(map);
            SPlayer.SetMap(SWorld.Instance.GetMap(map), RandomHelper.Get(40F, 400F), RandomHelper.Get(40F, 400F));

            SetRelay(SPlayer.Handler);

            SendingPacket rspData = new SendingPacket(PacketType.SERVER_INFO_MAP_LOADING);
            // map info ==> to use guid system
            rspData.AddString(map);
            // main player ==> added with all objects
            // rspData.AddDynamicObjectMapAdd(SPlayer);
            // dynamic objects except main player
            rspData.AddCount(SWorld.Instance.GetMap(map).DynamicObjects.GetCount());
            foreach (DynamicWorldObject dObj in SWorld.Instance.GetMap(map).DynamicObjects)
                rspData.AddDynamicObjectMapAdd(dObj);

            SendPacket(rspData, false);

            SendingPacket sndData = new SendingPacket(PacketType.SERVER_INFO_MAP_OBJECT_ADD);
            sndData.AddDynamicObjectMapAdd(SPlayer);
            SendPacket(sndData, true, true);

            return true;
        }

        #endregion

        public bool SendPacket(SendingPacket data, bool broadcast = true, bool broadcastPersonalException = false)
        {
            try
            {
                if (!broadcast)
                {
                    Log.Cl("Packet sent : " + data.Type + " to : " + Login, ConsoleColor.DarkRed);
                    return data.Send(Bw);
                }

                BlazeraServer.Instance.BroadcastPacket(data, broadcastPersonalException ? new int[] { Guid } : null);

                return true;
            }
            catch
            {
                Deco();
                return false;
            }
        }
    }
}
