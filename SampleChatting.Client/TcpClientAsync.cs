using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using SampleChatting.Lib;
using Suyeong.Lib.Util;
using Suyeong.Lib.Crypt;

namespace SampleChatting.Client
{
    public class TcpClientAsync
    {
        public event Action<ITcpPacket> OnNotice;

        TcpClient client;
        Queue<ITcpPacket> receiveQueue;
        Dictionary<string, Action<ITcpPacket>> callbackDic;

        public TcpClientAsync(string ip, int port)
        {
            this.client = new TcpClient(ip, port);
            this.receiveQueue = new Queue<ITcpPacket>();
            this.callbackDic = new Dictionary<string, Action<ITcpPacket>>();
        }

        async public Task StartAsync()
        {
            await Task.Run(() => ListenAsync());
            await Task.Run(() => CheckResultAsync());
        }

        public void Close()
        {
            this.client.Close();
        }

        async public Task CloseAsync()
        {
            TcpPacketMessage send = new TcpPacketMessage(type: PacketType.Message, protocol: Protocols.EXIT_CLIENT, data: string.Empty);

            await RequestAsync(packet: send, callback: (packet) =>
            {
                TcpPacketMessage receive = packet as TcpPacketMessage;
                bool result = (bool)receive.Data;

                // 클라이언트에서 종료 의사를 밝혔으므로 서버 응답이 어떻게 되든 무조건 종료.
                this.client.Close();

                //if (result)
                //{
                //    this.client.Close();
                //}
            });
        }

        async public Task RequestAsync(ITcpPacket packet, Action<ITcpPacket> callback)
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

        async Task SendPacketAsync(ITcpPacket packet)
        {
            try
            {
                using (NetworkStream networkStream = this.client.GetStream())
                {
                    byte[] source = Utils.ObjectToBinary(packet);
                    byte[] encrypt = await Crypts.EncryptAsync(data: source, key: Values.CRYPT_KEY, iv: Values.CRYPT_IV);

                    await TcpStream.SendPacketAsync(networkStream: networkStream, packetType: packet.Type, data: encrypt);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("SendPacketAsync: {0}", ex);
                Close();
            }
        }

        async Task ListenAsync()
        {
            try
            {
                ITcpPacket result;
                byte[] source, decrypt;

                while (this.client.Connected)
                {
                    using (NetworkStream networkStream = this.client.GetStream())
                    {
                        source = await TcpStream.ReceivePacketAsync(networkStream: networkStream);

                        if (source != null)
                        {
                            decrypt = await Crypts.DecryptAsync(data: source, key: Values.CRYPT_KEY, iv: Values.CRYPT_IV);
                            result = Utils.BinaryToObject(decrypt) as ITcpPacket;

                            this.receiveQueue.Enqueue(result);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListenAsync: {0}", ex);
                Close();
            }
        }

        async Task CheckResultAsync()
        {
            while (this.client.Connected)
            {
                if (this.receiveQueue.Count > 0)
                {
                    ITcpPacket result = this.receiveQueue.Dequeue();
                    Action<ITcpPacket> callback;

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
}
