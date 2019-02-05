using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using SampleChatting.Lib;

namespace SampleChatting.Server.Login
{
    public class TcpResponseLogin : TcpResponse
    {
        public TcpResponseLogin()
        {
        }

        async protected override Task<IPacket> GetResultMessage(IPacket request)
        {
            PacketMessage packet = request as PacketMessage;

            switch (packet.Protocol)
            {
                case Protocols.GET_USER_ACCESS:
                    return GetAccess(protocol: Protocols.GET_USER_ACCESS, data: packet.Data);

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

        PacketMessage GetAccess(string protocol, object data)
        {
            Dictionary<string, string> dic = data as Dictionary<string, string>;
            string id = dic[Keys.ID];
            string password = dic[Keys.PASSWORD];

            return new PacketMessage(type: PacketType.Message, protocol: protocol, data: "");
        }
    }
}
