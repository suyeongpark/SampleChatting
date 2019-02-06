using System.Collections.Generic;
using System.Threading.Tasks;
using Suyeong.Lib.Net;
using Suyeong.Lib.Net.Tcp;
using SampleChatting.Lib;

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

                case Protocols.LOGIN:
                    return await Login(protocol: Protocols.LOGIN, data: packet.Data);

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

            return new PacketMessage(type: PacketType.Message, protocol: protocol, data: true);

            if (await DataBase.IsDuplicated(id))
            {
                return new PacketMessage(type: PacketType.Message, protocol: protocol, data: false);
            }
            else
            {
                bool isSucceed = await DataBase.CreateUser(id: id, password: password);

                return new PacketMessage(type: PacketType.Message, protocol: protocol, data: isSucceed);
            }
        }

        async Task<PacketMessage> Login(string protocol, object data)
        {
            Dictionary<string, string> dic = data as Dictionary<string, string>;
            string id = dic[Keys.ID];
            string password = dic[Keys.PASSWORD];


            return new PacketMessage(type: PacketType.Message, protocol: protocol, data: true);

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
