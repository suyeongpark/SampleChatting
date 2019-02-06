using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Lib.Crypt;
using Suyeong.Lib.Net.Tcp;
using Suyeong.Lib.Util;

namespace SampleChatting.Lib
{
    public class TcpClientHandler
    {
        public event Action<Guid> OnDisconnect;

        TcpClient client;
        Guid guid;
        ITcpResponseAsync response;
        Queue<ITcpPacket> requestQueue, resultQueue;

        public TcpClientHandler(TcpClient client, Guid guid, ITcpResponseAsync response)
        {
            this.client = client;
            this.guid = guid;
            this.response = response;
            this.requestQueue = new Queue<ITcpPacket>();
            this.resultQueue = new Queue<ITcpPacket>();
        }

        async public Task StartAsync()
        {
            // 1. request를 받는다.
            await Task.Run(() => ListenAsync());

            // 2. response를 처리한다
            await Task.Run(() => GetResultAsync());

            // 3. send
            await Task.Run(() => CheckResultAsync());
        }

        public void Close()
        {
            this.client.Close();
        }

        async public Task Broadcast(ITcpPacket result)
        {
            await SendResultAsync(packet: result);
        }

        async Task ListenAsync()
        {
            try
            {
                ITcpPacket request;
                byte[] source, decrypt;

                while (this.client.Connected)
                {
                    using (NetworkStream networkStream = this.client.GetStream())
                    {
                        source = await TcpStream.ReceivePacketAsync(networkStream: networkStream);

                        if (source != null)
                        {
                            decrypt = await Crypts.DecryptAsync(data: source, key: Values.CRYPT_KEY, iv: Values.CRYPT_IV);
                            request = Utils.BinaryToObject(decrypt) as ITcpPacket;

                            this.requestQueue.Enqueue(request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ListenAsync Error: {0}", ex);
                this.OnDisconnect?.Invoke(this.guid);
            }
        }

        async Task GetResultAsync()
        {
            while (this.client.Connected)
            {
                if (this.requestQueue.Count > 0)
                {
                    ITcpPacket request = this.requestQueue.Dequeue();
                    ITcpPacket result = await this.response.GetResultAsync(request: request);

                    if (result != null)
                    {
                        this.resultQueue.Enqueue(result);
                    }
                }

                await Task.Delay(Values.DELAY_CHECK_QUEUE);
            }
        }

        async Task CheckResultAsync()
        {
            while (this.client.Connected)
            {
                if (this.resultQueue.Count > 0)
                {
                    ITcpPacket result = this.resultQueue.Dequeue();
                    await SendResultAsync(packet: result);
                }

                await Task.Delay(Values.DELAY_CHECK_QUEUE);
            }
        }

        async Task SendResultAsync(ITcpPacket packet)
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
                Console.WriteLine("SendResultAsync Error: {0}", ex);
                Close();
            }
        }
    }
}
