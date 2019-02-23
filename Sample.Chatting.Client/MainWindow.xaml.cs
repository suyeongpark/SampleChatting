using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using Suyeong.Lib.Net.Tcp;
using Sample.Chatting.Lib;

namespace Sample.Chatting.Client
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {

        }

        void Test()
        {
            TcpConnectorCrypt client = new TcpConnectorCrypt(ip: Servers.IP_LOGIN, port: Servers.PORT_LOGIN, cryptKey: Values.CRYPT_KEY, cryptIV: Values.CRYPT_IV);
            client.Start();

            while (true)
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

        void GetResult(ITcpPacket packet)
        {
            TcpPacketMessage msg = packet as TcpPacketMessage;
            Console.WriteLine("Get Result: {0}", (bool)msg.Data);
        }
    }
}
