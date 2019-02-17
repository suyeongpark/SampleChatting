using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampleChatting.Lib;
using Suyeong.Lib.Net.Tcp;

namespace SampleChatting.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // 난수 생성용
            //byte[] key = new byte[16];
            //byte[] iv = new byte[16];

            //System.Security.Cryptography.RandomNumberGenerator rand = System.Security.Cryptography.RandomNumberGenerator.Create();
            //rand.GetBytes(key);
            //rand.GetBytes(iv);

            //Console.WriteLine("key: {0}", string.Join(", ", key));
            //Console.WriteLine("iv: {0}", string.Join(", ", iv));

            //MainAsync(args).Wait();

            Test();
        }

        async static Task MainAsync(string[] args)
        {
            Console.WriteLine("Hello World Async!");
        }

        static void Test()
        {
            TcpConnectorCrypt client = new TcpConnectorCrypt(ip: Servers.IP_LOGIN, port: Servers.PORT_LOGIN, cryptKey: Values.CRYPT_KEY, cryptIV: Values.CRYPT_IV);
            client.Start();

            while(true)
            {
                Console.ReadLine();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add(Keys.ID, "ID");
                dic.Add(Keys.PASSWORD, "Password");

                Console.WriteLine("Send Message");
                TcpPacketMessage message = new TcpPacketMessage(type: PacketType.Message, protocol: Protocols.LOGIN, data: dic);

                client.Send(packet: message, callback: GetResult);
            }
        }

        static void GetResult(ITcpPacket packet)
        {
            TcpPacketMessage msg = packet as TcpPacketMessage;
            Console.WriteLine("Get Result: {0}", (bool)msg.Data);
        }
    }
}
