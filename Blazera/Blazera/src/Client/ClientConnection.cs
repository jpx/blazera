using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using BlazeraLib;

namespace Blazera
{
    public class ClientConnection
    {
        #region Members

        BinaryWriter Bw;
        BinaryReader Br;
        NetworkStream Ns;

        TcpClient TcpClient;

        Thread ReceptionThread;

        public String Login { get; set; }

        public Boolean IsConnected { get; private set; }

        Queue<ReceptionPacket> Packets;

        #endregion

        public ClientConnection()
        {
            Packets = new Queue<ReceptionPacket>();
        }

        public Boolean Connect(String ip, int port)
        {
            try
            {
                TcpClient = new System.Net.Sockets.TcpClient();
                TcpClient.ReceiveTimeout = 5000;
                TcpClient.SendTimeout = 5000;
                TcpClient.Connect(ip, port);
                Ns = TcpClient.GetStream();

                Bw = new BinaryWriter(TcpClient.GetStream());
                Br = new BinaryReader(TcpClient.GetStream());

                IsConnected = true;
            }
            catch (Exception e)
            {
                IsConnected = false;
                Log.Cl(e.Message);
                return false;
            }

            ReceptionThread = new Thread(new ThreadStart(Run));
            ReceptionThread.IsBackground = true;
            ReceptionThread.Start();

            return true;
        }

        public Boolean TryLogin(String login)
        {
            return true;
        }

        public Boolean Send(SendingPacket packet)
        {
            return packet.Send(Bw);
        }

        public void Close()
        {
            TcpClient.Close();
        }

        public void Deco()
        {
            IsConnected = false;
        }

        public void Run()
        {
            while (IsConnected)
            {
                try
                {
                    if (!Ns.DataAvailable)
                    {
                        Thread.Sleep(5);
                        continue;
                    }

                    ReceptionPacket data = new ReceptionPacket(Br);

                    Log.Cl("Packet received : " + data.Type, ConsoleColor.DarkRed);

                    Packets.Enqueue(data);
                }
                catch (Exception ex)
                {
                    Log.Cl(ex.Message);
                    Deco();
                }
            }

            Close();
        }

        public Boolean ContainsReceivedPacket()
        {
            return Packets.Count > 0;
        }

        public ReceptionPacket GetPacket()
        {
            return Packets.Dequeue();
        }
    }
}
