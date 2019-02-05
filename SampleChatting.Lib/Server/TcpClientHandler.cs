using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;

namespace SampleChatting.Lib
{
    public class TcpClientHandler
    {
        public event Action<Guid> OnDisconnect;

        TcpClient client;
        Guid guid;
        IResponse response;
        Queue<IPacket> requestQueue, resultQueue;

        public TcpClientHandler(TcpClient client, Guid guid, IResponse response)
        {
            this.client = client;
            this.guid = guid;
            this.response = response;
            this.requestQueue = new Queue<IPacket>();
            this.resultQueue = new Queue<IPacket>();
        }

        public void Start()
        {
            // 1. request를 받는다.
            Task.Run(() => ListenAsync());

            // 2. response를 처리한다
            Task.Run(() => GetResultAsync());

            // 3. send
            Task.Run(() => CheckResultAsync());
        }

        public void Close()
        {
            this.client.Close();
        }

        async public Task Broadcast(IPacket result)
        {
            await SendResultAsync(packet: result);
        }

        async Task ListenAsync()
        {
            try
            {
                IPacket request;

                while (this.client.Connected)
                {
                    using (NetworkStream networkStream = this.client.GetStream())
                    {
                        request = await TcpNetworkStream.ReceivePacketAsync(networkStream: networkStream);

                        if (request != null)
                        {
                            this.requestQueue.Enqueue(request);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnDisconnect?.Invoke(this.guid);
            }
        }

        async Task GetResultAsync()
        {
            if (this.requestQueue.Count > 0)
            {
                IPacket request = this.requestQueue.Dequeue();
                IPacket result = await this.response.GetResult(request: request);

                if (result != null)
                {
                    this.resultQueue.Enqueue(result);
                }
            }

            await Task.Delay(Values.DELAY_CHECK_QUEUE);
        }

        async Task CheckResultAsync()
        {
            if (this.resultQueue.Count > 0)
            {
                IPacket result = this.resultQueue.Dequeue();
                await SendResultAsync(packet: result);
            }

            await Task.Delay(Values.DELAY_CHECK_QUEUE);
        }

        async Task SendResultAsync(IPacket packet)
        {
            try
            {
                using (NetworkStream networkStream = this.client.GetStream())
                {
                    await TcpNetworkStream.SendPacketAsync(networkStream: networkStream, packet: packet);
                }
            }
            catch (Exception ex)
            {
                Close();
            }
        }
    }
}
