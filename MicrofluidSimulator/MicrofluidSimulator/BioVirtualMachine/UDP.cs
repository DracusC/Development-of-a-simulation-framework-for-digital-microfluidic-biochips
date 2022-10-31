using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace BioVirtualMachine
{
    class UDP
    {
        /*
        // Server
        IPEndPoint ServerEndPoint;
        Socket WinSocket;
        */

        // Client
        IPEndPoint RemoteEndPoint;
        Socket server;

        public UDP()
        {
            /*
            //Server
            ServerEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 9050);// "serveraddr"
            WinSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            WinSocket.Bind(ServerEndPoint);
            */

            //Client
            RemoteEndPoint = new IPEndPoint(IPAddress.Parse("169.254.27.72"), 9050);// "serveraddr"
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        }

        internal void UDPSend(string message)
        {
            // Client
            byte[] senddata = new byte[128];
            //string message = "Hello Georgi!";
            senddata = Encoding.ASCII.GetBytes(message);
            server.SendTo(senddata, senddata.Length, SocketFlags.None, RemoteEndPoint);
            Console.WriteLine("UDP Sent");
        }

        /*
        internal void UDPTestReceive()
        {
            
            // Server
            byte[] receivedata = new byte[128];
            Console.WriteLine("Waiting for client");
            IPEndPoint sender = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0); // "clientaddr"
            EndPoint Remote = (EndPoint)(sender);
            int recv = WinSocket.ReceiveFrom(receivedata, ref Remote);
            Console.WriteLine("Message received from {0}:", Remote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(receivedata, 0, recv));
            
        }
        */
    }
}
