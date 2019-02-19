using System.Collections.Generic;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using Sample.Chatting.Lib;

namespace Sample.Chatting.Server.Login
{
    public static class TcpResponseLogin
    {
        async public static Task<ITcpPacket> GetResultMessageAsync(TcpPacketMessage packet)
        {
            switch (packet.Protocol)
            {
                case Protocols.CREATE_USER:
                    return await CreateUser(protocol: Protocols.CREATE_USER, data: packet.Data);

                case Protocols.LOGIN:
                    return await Login(protocol: Protocols.LOGIN, data: packet.Data);

                default:
                    return null;
            }
        }

        async public static Task<ITcpPacket> GetResulFileAsync(TcpPacketFile packet)
        {
            switch (packet.Protocol)
            {
                default:
                    return null;
            }
        }

        async static Task<TcpPacketMessage> CreateUser(string protocol, object data)
        {
            Dictionary<string, string> dic = data as Dictionary<string, string>;
            string id = dic[Keys.ID];
            string password = dic[Keys.PASSWORD];

            return new TcpPacketMessage(type: PacketType.Message, protocol: protocol, data: true);

            if (await DataBase.IsDuplicated(id))
            {
                return new TcpPacketMessage(type: PacketType.Message, protocol: protocol, data: false);
            }
            else
            {
                bool isSucceed = await DataBase.CreateUser(id: id, password: password);

                return new TcpPacketMessage(type: PacketType.Message, protocol: protocol, data: isSucceed);
            }
        }

        async static Task<TcpPacketMessage> Login(string protocol, object data)
        {
            Dictionary<string, string> dic = data as Dictionary<string, string>;
            string id = dic[Keys.ID];
            string password = dic[Keys.PASSWORD];


            return new TcpPacketMessage(type: PacketType.Message, protocol: protocol, data: true);

            //if (await DataBase.IsApproved(id: id, password: password))
            //{
            //    // 승인이 되었으면 모든 channel의 사용자 정보를 가져와서 보낸다.

            //    return new PacketMessage(type: PacketType.Message, protocol: protocol, data: true);
            //}
            //else
            //{
            //    return new PacketMessage(type: PacketType.Message, protocol: protocol, data: false);
            //}
        }
    }
}
