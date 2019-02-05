using System;
namespace SampleChatting.Server.Channel
{
    public class TcpResponseChannel : TcpResponse
    {
        public TcpResponseChannel()
        {
        }

        protected override Task<IPacket> GetResulFile(IPacket request)
        {
            throw new NotImplementedException();
        }

        protected override Task<IPacket> GetResultMessage(IPacket request)
        {
            throw new NotImplementedException();
        }
    }
}
