using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void SendMessage(string message, UdpClient udpClient, IPEndPoint serverEndPoint)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(bytes, bytes.Length, serverEndPoint);
        }

        static string Receive(ref IPEndPoint serverEndPoint, UdpClient udpClient)
        {
            byte[] bytes = udpClient.Receive(ref serverEndPoint);
            string message = Encoding.UTF8.GetString(bytes);
            return message;
        }
        static void Main(string[] args)
        {
            int port = 500;
            UdpClient udpClient = new UdpClient();
            IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
            SendMessage("hi", udpClient, serverEndPoint);



            while (true)
            {
                string menu = Receive(ref serverEndPoint, udpClient);
                Console.WriteLine($"{menu}");
                string choice = Console.ReadLine();
                SendMessage(choice, udpClient, serverEndPoint);
                string result = Receive(ref serverEndPoint, udpClient);
                Console.WriteLine($"{result}");
            }
            udpClient.Close();
        }
    }
}
