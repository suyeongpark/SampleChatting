using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using SampleChatting.Lib;

namespace SampleChatting.Server.Login
{
    public class TcpServer
    {
        public event Action<string> OnMessage;

        Dictionary<Guid, TcpClientHandler> clientDic;
        List<TcpClient> lobbies;

        public TcpServer()
        {
            DataBase.Init();

            this.clientDic = new Dictionary<Guid, TcpClientHandler>();

            //this.lobbies = new List<TcpClient>();

            //foreach (string ip in Servers.IP_LOBBY)
            //{
            //    this.lobbies.Add(new TcpClient(ip, Servers.PORT_CHANNEL));
            //}
        }

        async public Task StartListen()
        {
            TcpListener server = new TcpListener(new IPEndPoint(IPAddress.Any, Servers.PORT_LOGIN));
            server.Start();

            TcpClient client;

            while (true)
            {
                client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                await Task.Run(() => AddClient(client: client, guid: Guid.NewGuid()));
            }
        }

        async Task AddClient(TcpClient client, Guid guid)
        {
            Console.WriteLine("AddClient: {0}", guid);
            TcpClientHandler handler = new TcpClientHandler(client: client, guid: guid, response: new TcpResponseLoginAsync());
            handler.OnDisconnect += Disconnect;
            await handler.StartAsync();

            this.clientDic.Add(guid, handler);
        }

        void Disconnect(Guid guid)
        {
            if (clientDic.ContainsKey(guid))
            {
                clientDic[guid].Close();
                clientDic.Remove(guid);

                OnMessage?.Invoke($"Client Disconnected: {guid}");
            }
            else
            {
                OnMessage?.Invoke($"Client Disconnected Error: {guid}");
            }
        }
    }
}
