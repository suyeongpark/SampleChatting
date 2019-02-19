using System;
using System.Threading.Tasks;
using Sample.Chatting.Lib;
using Suyeong.Lib.Net.Tcp;

namespace Sample.Chatting.Server.Channel
{
    public static class TcpResponseChannel
    {
        async public static Task<ITcpPacket> GetResultMessageAsync(TcpPacketMessage packet)
        {
            switch (packet.Protocol)
            {
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
    }
}
