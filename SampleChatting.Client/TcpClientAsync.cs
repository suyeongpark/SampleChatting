using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using SampleChatting.Lib;

namespace SampleChatting.Client
{
    public class TcpClientAsync
    {
        public event Action<IPacket> OnNotice;

        TcpClient client;
        Queue<IPacket> receiveQueue;
        Dictionary<string, Action<IPacket>> callbackDic;

        public TcpClientAsync(string serverIP, int portNum)
        {
            this.client = new TcpClient(serverIP, portNum);
            this.receiveQueue = new Queue<IPacket>();
            this.callbackDic = new Dictionary<string, Action<IPacket>>();
        }

        public void Start()
        {
            Task.Run(() => ListenAsync());
            Task.Run(() => CheckResultAsync());
        }

        public void Close()
        {
            this.client.Close();
        }

        async public Task CloseAsync()
        {
            PacketMessage send = new PacketMessage(type: PacketType.Message, protocol: Protocols.EXIT_CLIENT, data: string.Empty);

            await RequestAsync(packet: send, callback: (packet) =>
            {
                PacketMessage receive = packet as PacketMessage;
                bool result = (bool)receive.Data;

                // 클라이언트에서 종료 의사를 밝혔으므로 서버 응답이 어떻게 되든 무조건 종료.
                this.client.Close();

                //if (result)
                //{
                //    this.client.Close();
                //}
            });
        }

        async public Task RequestAsync(IPacket packet, Action<IPacket> callback)
        {
            if (this.callbackDic.ContainsKey(packet.Protocol))
            {
                this.callbackDic[packet.Protocol] = callback;
            }
            else
            {
                this.callbackDic.Add(packet.Protocol, callback);
            }

            await SendPacketAsync(packet: packet);
        }

        async Task SendPacketAsync(IPacket packet)
        {
            try
            {
                using (NetworkStream networkStream = this.client.GetStream())
                {
                    await TcpNetworkStream.SendPacketAsync(networkStream: networkStream, packet: packet, key: Crypts.KEY, iv: Crypts.IV);
                }
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        async Task ListenAsync()
        {
            try
            {
                IPacket result;

                while (this.client.Connected)
                {
                    using (NetworkStream networkStream = this.client.GetStream())
                    {
                        result = await TcpNetworkStream.ReceivePacketAsync(networkStream: networkStream, key: Crypts.KEY, iv: Crypts.IV);

                        if (result != null)
                        {
                            receiveQueue.Enqueue(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Close();
            }
        }

        async Task CheckResultAsync()
        {
            if (this.receiveQueue.Count > 0)
            {
                IPacket result = this.receiveQueue.Dequeue();
                Action<IPacket> callback;

                // protocol이 있었으면 클라이언트가 요청을 보낸 것에 대한 응답
                if (this.callbackDic.TryGetValue(result.Protocol, out callback))
                {
                    this.callbackDic.Remove(result.Protocol);
                    callback(result);
                }
                // protocol이 없었으면 서버에서 보내온 Broadcast
                else
                {
                    this.OnNotice?.Invoke(result);
                }
            }

            await Task.Delay(Values.DELAY_CHECK_QUEUE);
        }
    }
}
