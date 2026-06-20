using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            IPAddress serverIp = IPAddress.Parse("127.0.0.1");
            int serverPort = 12345;

            IPEndPoint endPoint = new IPEndPoint(serverIp, serverPort);

            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                client.Connect(endPoint);
                Console.WriteLine($"Connected to {endPoint.Address}");

                byte[] buffer = new byte[1024];
                int len = client.Receive(buffer);
                string serverMessage = Encoding.UTF8.GetString(buffer, 0, len);

                Console.WriteLine(serverMessage);

                string message = Console.ReadLine();
                byte[] bytesMessage = Encoding.UTF8.GetBytes(message);
                client.Send(bytesMessage);

                byte[] buffer2 = new byte[1024];
                int len2 = client.Receive(buffer2);
                string serverMessage2 = Encoding.UTF8.GetString(buffer2, 0, len2);
                Console.WriteLine(serverMessage2);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally { 
                client.Close();
            }
        }
    }
}
