using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SampleChatting.Lib;
using Suyeong.Lib.Net;

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

            MainAsync(args).Wait();
        }

        async static Task MainAsync(string[] args)
        {
            Console.WriteLine("Hello World Async!");

            await Test();
        }

        async static Task Test()
        {
            TcpClientAsync client = new TcpClientAsync(ip: Servers.IP_LOGIN, port: Servers.PORT_LOGIN);
            await client.StartAsync();

            while(true)
            {
                Console.ReadLine();

                Dictionary<string, string> dic = new Dictionary<string, string>();
                dic.Add(Keys.ID, "ID");
                dic.Add(Keys.PASSWORD, "Password");

                Console.WriteLine("Send Message");
                PacketMessage message = new PacketMessage(type: PacketType.Message, protocol: Protocols.LOGIN, data: dic);

                await client.RequestAsync(packet: message, callback: GetResult);
            }
        }

        static void GetResult(IPacket packet)
        {
            PacketMessage msg = packet as PacketMessage;
            Console.WriteLine("login: {0}", (bool)msg.Data);
        }
    }
}
