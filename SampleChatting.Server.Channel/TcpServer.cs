using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SampleChatting.Lib;

namespace SampleChatting.Server.Channel
{
    public class TcpServer
    {
        public event Action<string> OnMessage;

        List<TcpClient> rooms;

        public TcpServer()
        {
            this.rooms = new List<TcpClient>();

            foreach (string ip in Servers.IP_LOBBY)
            {
                this.rooms.Add(new TcpClient(ip, Servers.PORT_CHANNEL));
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
