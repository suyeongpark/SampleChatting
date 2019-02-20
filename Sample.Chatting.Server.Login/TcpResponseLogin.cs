using System.Collections.Generic;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using Sample.Chatting.Lib;
using Sample.Chatting.Database;

namespace Sample.Chatting.Server.Login
{
    public static class TcpResponseLogin
    {
        static IChattingDB database;

        public static void Init()
        {
            database = new ChattingMySql();
        }

        async public static Task<ITcpPacket> GetResultAsync(ITcpPacket request)
        {
            return (request.Type == PacketType.Message) 
                ? await GetResultMessageAsync(request: request as TcpPacketMessage) 
                : await GetResulFileAsync(request: request as TcpPacketFile);
        }

        async static Task<ITcpPacket> GetResultMessageAsync(TcpPacketMessage request)
        {
            switch (request.Protocol)
            {
                case Protocols.CREATE_USER:
                    return await CreateUser(protocol: Protocols.CREATE_USER, data: request.Data);

                case Protocols.LOGIN:
                    return await Login(protocol: Protocols.LOGIN, data: request.Data);

                default:
                    return null;
            }
        }

        async static Task<ITcpPacket> GetResulFileAsync(TcpPacketFile request)
        {
            switch (request.Protocol)
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

            if (await database.IsDuplicatedAsync(id))
            {
                return new TcpPacketMessage(type: PacketType.Message, protocol: protocol, data: false);
            }
            else
            {
                bool isSucceed = await database.CreateUserAsync(id: id, password: password);

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
