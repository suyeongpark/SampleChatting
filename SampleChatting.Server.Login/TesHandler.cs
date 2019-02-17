using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Suyeong.Lib.Net.Tcp;
using Suyeong.Lib.Util;

namespace SampleChatting.Server.Login
{
    public class TestHandlerCrypt
    {
        public event Action<string> OnMessage;
        public event Action<Guid> OnDisconnect;
        public event Action<Guid, ITcpPacket> OnRequest;

        TcpClient client;
        Guid guid;
        byte[] cryptKey, cryptIV;
        bool isConnected;

        public TestHandlerCrypt(TcpClient client, Guid guid, byte[] cryptKey, byte[] cryptIV)
        {
            this.client = client;
            this.guid = guid;
            this.cryptKey = cryptKey;
            this.cryptIV = cryptIV;
        }

        public void StartListen()
        {
            this.isConnected = true;
            Task.Run(() => ListenAsync());
        }

        public void Close()
        {
            this.isConnected = false;
            this.client.Close();
        }

        public void Send(ITcpPacket packet)
        {
            Task.Run(() => SendAsync(packet: packet));
        }

        async Task ListenAsync()
        {
            try
            {
                NetworkStream networkStream;
                ITcpPacket request;
                byte[] source, decrypt;

                while (this.isConnected)
                {
                    networkStream = this.client.GetStream();
                    source = await TcpStream.ReceivePacketAsync(networkStream: networkStream);

                    if (source != null)
                    {
                        // 암호해제에는 압축해제도 포함되어 있다.
                        decrypt = await Crypts.DecryptAsync(data: source, key: this.cryptKey, iv: this.cryptIV);
                        request = Utils.BinaryToObject(decrypt) as ITcpPacket;

                        OnRequest(this.guid, request);
                    }
                }
            }
            catch (Exception ex)
            {
                this.OnMessage?.Invoke(ex.ToString());
                this.OnDisconnect?.Invoke(this.guid);
            }
        }

        async Task SendAsync(ITcpPacket packet)
        {
            try
            {
                NetworkStream networkStream = this.client.GetStream();

                byte[] source = Utils.ObjectToBinary(packet);
                // 암호화에는 압축도 포함되어 있다.
                byte[] encrypt = await Crypts.EncryptAsync(data: source, key: this.cryptKey, iv: this.cryptIV);

                await TcpStream.SendPacketAsync(networkStream: networkStream, packetType: packet.Type, data: encrypt);
            }
            catch (Exception ex)
            {
                this.OnMessage?.Invoke(ex.ToString());
                this.OnDisconnect?.Invoke(this.guid);
            }
        }
    }
}
