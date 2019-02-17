using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using SampleChatting.Lib;

namespace SampleChatting.Server.Login
{
    public class TcpServer
    {
        public event Action<string> OnMessage;

        Dictionary<Guid, TestHandlerCrypt> clientDic;
        List<TcpClient> lobbies;

        public TcpServer()
        {
            DataBase.Init();

            this.clientDic = new Dictionary<Guid, TestHandlerCrypt>();

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
            Guid guid;
            TestHandlerCrypt handler;

            while (true)
            {
                client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                guid = Guid.NewGuid();
                handler = new TestHandlerCrypt(client: client, guid: guid, cryptKey: Values.CRYPT_KEY, cryptIV: Values.CRYPT_IV);
                handler.OnDisconnect += Disconnect;
                handler.OnRequest += OnRequest;
                handler.OnMessage += OnMessage;
                handler.StartListen();

                this.clientDic.Add(guid, handler);

                Console.WriteLine("AddClient: {0}", guid);
            }
        }

        void OnRequest(Guid guid, ITcpPacket request)
        {
            TestHandlerCrypt client = this.clientDic[guid];
            Task.Run(() => GetResult(client: client, request: request));
        }

        async Task GetResult(TestHandlerCrypt client, ITcpPacket request)
        {
            ITcpPacket result = request.Type == PacketType.Message 
                ? await TcpResponseLogin.GetResultMessageAsync(request: request) 
                : await TcpResponseLogin.GetResulFileAsync(request: request);

            client.Send(packet: result);
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
