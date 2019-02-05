using System;
namespace SampleChatting.Server.Login
{
    public class TcpServerLogin
    {
        Dictionary<Guid, TcpClientHandler> clientDic;
        //TcpClient manager;

        //public TcpServerLogin()
        //{
        //    manager = new TcpClient(Servers.IP_MANAGER, Servers.PORT_MANAGER);
        //    this.clientDic = new Dictionary<Guid, TcpClientHandler>();
        //}

        //public int ClientCount { get { return this.clientDic.Count; } }

        //public void AddClient(TcpClient client, Guid guid)
        //{
        //    if (!this.clientDic.ContainsKey(guid))
        //    {
        //        TcpClientHandler handler = new TcpClientHandler(client: client, guid: guid, response: new TcpResponseLogin());
        //        //handler.OnDisconnect += Disconnect;
        //        handler.Start();

        //        this.clientDic.Add(guid, handler);
        //    }
        //}


        public event Action<string> OnMessage;

        List<TcpClient> lobbies;

        public TcpServerLogin()
        {
            this.clientDic = new Dictionary<Guid, TcpClientHandler>();

            this.lobbies = new List<TcpClient>();

            foreach (string ip in Servers.IP_LOBBY)
            {
                this.lobbies.Add(new TcpClient(ip, Servers.PORT_CHANNEL));
            }
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

        //void SendLogin(TcpClient client)
        //{
        //    this.lastLogin = ++this.lastLogin % this.logins.Count;
        //    TcpClient login = this.logins[lastLogin];

        //}

        void AddClient(TcpClient client, Guid guid)
        {
            TcpClientHandler handler = new TcpClientHandler(client: client, guid: guid, response: new TcpResponseLogin());
            handler.OnDisconnect += Disconnect;
            handler.Start();

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
