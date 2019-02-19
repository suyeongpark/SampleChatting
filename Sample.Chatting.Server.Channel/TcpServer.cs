using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Sample.Chatting.Lib;
using Suyeong.Lib.Net.Tcp;

namespace Sample.Chatting.Server.Channel
{
    public class TcpServer
    {
        public event Action<string> OnMessage;

        Dictionary<Guid, TcpClientHandlerCrypt> clientDic;
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
            TcpListener server = new TcpListener(new IPEndPoint(IPAddress.Any, Servers.PORT_LOGIN));
            server.Start();

            TcpClient client;
            Guid guid;
            TcpClientHandlerCrypt handler;

            while (true)
            {
                client = await server.AcceptTcpClientAsync().ConfigureAwait(false);
                guid = Guid.NewGuid();
                handler = new TcpClientHandlerCrypt(client: client, guid: guid, cryptKey: Values.CRYPT_KEY, cryptIV: Values.CRYPT_IV);
                handler.OnRequest += OnRequest;
                handler.OnMessage += OnMessage;
                handler.OnDisconnect += Disconnect;
                handler.StartListen();

                this.clientDic.Add(guid, handler);

                Console.WriteLine("AddClient: {0}", guid);
            }
        }

        void OnRequest(Guid guid, ITcpPacket request)
        {
            Task.Run(() => GetResult(client: this.clientDic[guid], request: request));
        }

        async Task GetResult(TcpClientHandlerCrypt client, ITcpPacket request)
        {
            ITcpPacket result = (request.Type == PacketType.Message)
                ? await TcpResponseChannel.GetResultMessageAsync(packet: request as TcpPacketMessage)
                : await TcpResponseChannel.GetResulFileAsync(packet: request as TcpPacketFile);

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
