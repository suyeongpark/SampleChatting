using System;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using Sample.Chatting.Lib;
using Sample.Chatting.Database;

namespace Sample.Chatting.Server.Channel
{
    public static class TcpResponseChannel
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
    }
}
