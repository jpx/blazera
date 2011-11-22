using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using SFML.Graphics;
using SFML.Window;

namespace BlazeraLib
{
    public enum PacketType
    {
        /** Client
         */
        // login
        CLIENT_REQUEST_LOGIN_VALIDATION,
        CLIENT_ERROR_DECONNECTION,
        CLIENT_INFO_DECONNECTION,
        // start
        CLIENT_REQUEST_MAP_LOADING,
        // move
        CLIENT_REQUEST_DIRECTION_ENABLED,
        CLIENT_REQUEST_DIRECTION_DISABLED,

        CLIENT_PING,
        CLIENT_PONG,
        /** Server
         */
        // login
        SERVER_ERROR_LOGIN_VALIDATION,
        SERVER_INFO_LOGIN_VALIDATION,
        // start
        SERVER_INFO_MAP_LOADING,
        // move
        SERVER_INFO_MOVE,

        SERVER_INFO_MAP_OBJECT_ADD,
        SERVER_INFO_MAP_OBJECT_DELETION,

        SERVER_PING,
        SERVER_PONG
    }

    #region Parser

    public class Parser
    {
        #region Constants

        const char DEFAULT_SEPARATOR = '|';

        #endregion

        #region Members

        string Str;
        string[] Data;
        int CurrentWord;

        #endregion

        public Parser(string str)
        {
            Str = str;

            Data = Str.Split(DEFAULT_SEPARATOR);

            CurrentWord = 0;
        }

        public string GetNext()
        {
            if (CurrentWord >= Data.Length)
                throw new Exception("End of packet");

            return Data[CurrentWord++];
        }

        public PacketType GetPacketType()
        {
            return (PacketType)Enum.ToObject(typeof(PacketType), int.Parse(GetNext()));
        }

        public int GetInt()
        {
            return Int32.Parse(GetNext());
        }

        public float GetFloat()
        {
            return float.Parse(GetNext());
        }

        public Direction GetDirection()
        {
            return (Direction)Enum.ToObject(typeof(Direction), int.Parse(GetNext()));
        }
    }

    #endregion

    #region SendingPacket

    public partial class SendingPacket
    {
        #region Members

        string Data;

        public PacketType Type { get; private set; }

        #endregion

        public SendingPacket(PacketType type)
        {
            Data = "";
            Type = type;
            AddType();
        }

        void AppendObj(object obj)
        {
            Append(obj.ToString());
        }

        void Append(string data)
        {
            Data += (Data == "" ? "" : "|") + data;
        }

        public Boolean Send(BinaryWriter bw)
        {
            try
            {
                bw.Write(Data);

                return true;
            }
            catch
            {
                throw new Exception("Failed to send packet " + Type.ToString() + ".");
            }
        }

        void AddType()
        {
            AppendObj((int)Type);
        }

        public void AddGuid(Int32 data)
        {
            AppendObj(data);
        }

        public void AddVector2(Vector2f data)
        {
            AppendObj(data.X);
            AppendObj(data.Y);
        }

        public void AddString(String data)
        {
            Append(data);
        }

        public void AddCount(int data)
        {
            AppendObj(data);
        }
    }

    #endregion

    #region ReceptionPacket

    public partial class ReceptionPacket
    {
        #region Members

        Parser Parser;
        public PacketType Type { get; private set; }

        #endregion

        public ReceptionPacket(BinaryReader br)
        {
            Parser = new Parser(br.ReadString());
            Type = Parser.GetPacketType();
        }

        public int ReadGuid()
        {
            return Parser.GetInt();
        }

        public int ReadCount()
        {
            return Parser.GetInt();
        }

        public Vector2f ReadVector2()
        {
            return new Vector2f(
                Parser.GetFloat(),
                Parser.GetFloat());
        }

        public string ReadString()
        {
            return Parser.GetNext();
        }

        public float ReadFloat()
        {
            return Parser.GetFloat();
        }
    }

    #endregion
}