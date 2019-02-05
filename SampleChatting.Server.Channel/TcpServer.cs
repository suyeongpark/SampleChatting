using System;
namespace SampleChatting.Server.Channel
{
    public class TcpServerChannel
    {
        public event Action<string> OnMessage;

        List<TcpClient> rooms;

        public TcpServerChannel()
        {
            this.rooms = new List<TcpClient>();

            foreach (string ip in Servers.IP_LOBBY)
            {
                this.rooms.Add(new TcpClient(ip, Servers.PORT_ROOM));
            }
        }

        async public Task StartListen()
        {
            TcpListener server = new TcpListener(new IPEndPoint(IPAddress.Any, Servers.PORT_CHANNEL));
            server.Start();

            TcpClient client;

            while (true)
            {
                client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
            }
        }
    }
}
