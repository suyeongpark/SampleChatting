using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using Suyeong.Lib.DB.Sqlite;
using SampleChatting.Lib;
using System.Data;

namespace SampleChatting.Server.Login
{
    public class TcpResponseLogin : TcpResponse
    {
        public TcpResponseLogin()
        {
            DataBase.Init();
        }

        async protected override Task<IPacket> GetResultMessage(IPacket request)
        {
            PacketMessage packet = request as PacketMessage;

            switch (packet.Protocol)
            {
                case Protocols.CREATE_USER:
                    return await CreateUser(protocol: Protocols.CREATE_USER, data: packet.Data);

                case Protocols.GET_USER_ACCESS:
                    return await GetAccess(protocol: Protocols.GET_USER_ACCESS, data: packet.Data);

                default:
                    return null;
            }
        }

        async protected override Task<IPacket> GetResulFile(IPacket request)
        {
            PacketFile packet = request as PacketFile;

            switch (packet.Protocol)
            {
                default:
                    return null;
            }
        }

        async Task<PacketMessage> CreateUser(string protocol, object data)
        {
            Dictionary<string, string> dic = data as Dictionary<string, string>;
            string id = dic[Keys.ID];
            string password = dic[Keys.PASSWORD];

            bool isDuplicate = await DataBase.IsDuplicateID(id);

            if (isDuplicate)
            {
                return new PacketMessage(type: PacketType.Message, protocol: protocol, data: !isDuplicate);
            }
            else
            {
                bool isSucceed = await DataBase.CreateUser(id: id, password: password);
                return new PacketMessage(type: PacketType.Message, protocol: protocol, data: isSucceed);
            }
        }

        async Task<PacketMessage> GetAccess(string protocol, object data)
        {
            Dictionary<string, string> dic = data as Dictionary<string, string>;
            string id = dic[Keys.ID];
            string password = dic[Keys.PASSWORD];

            DataTable table = await DataBase.GetAccess(id: id, password: password);

            return new PacketMessage(type: PacketType.Message, protocol: protocol, data: table);
        }
    }
}
