using System;
using System.Threading.Tasks;
using Suyeong.Lib.Net;
using Suyeong.Lib.Net.Tcp;

namespace SampleChatting.Server.Channel
{
    public class TcpResponseChannelAsync : TcpResponseAsync
    {
        public TcpResponseChannelAsync()
        {
        }

        protected override Task<ITcpPacket> GetResulFileAsync(ITcpPacket request)
        {
            throw new NotImplementedException();
        }

        protected override Task<ITcpPacket> GetResultMessageAsync(ITcpPacket request)
        {
            throw new NotImplementedException();
        }
    }
}
