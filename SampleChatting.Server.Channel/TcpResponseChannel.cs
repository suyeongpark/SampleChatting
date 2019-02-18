using System;
using System.Threading.Tasks;
using SampleChatting.Lib;
using Suyeong.Lib.Net;
using Suyeong.Lib.Net.Tcp;

namespace SampleChatting.Server.Channel
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
