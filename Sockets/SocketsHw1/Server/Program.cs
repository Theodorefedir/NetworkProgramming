using System.Net;
using System.Net.Sockets;
using System.Text;

namespace NPHw1
{
    internal class server
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 12345;

            IPEndPoint endPoint = new IPEndPoint(ip, port);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint);

            try
            {
                server.Listen();
                Console.WriteLine("Server running...");

                while (true)
                {
                    Socket client = server.Accept();
                    Console.WriteLine($"Client connected: {client.RemoteEndPoint}");

                    byte[] clientMessage = new byte[1024];
                    int bytesRead = client.Receive(clientMessage);
                    string receivedMessage = Encoding.UTF8.GetString(clientMessage, 0, bytesRead);

                    string currentTime = DateTime.Now.ToString("HH:mm");
                    IPEndPoint clientEndPoint = (IPEndPoint)client.RemoteEndPoint;
                    Console.WriteLine($"О {currentTime} від {clientEndPoint.Address} отримано рядок: {receivedMessage}");

                    string message = "Hello from server!";
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    client.Send(bytes);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                server.Close();
                Console.WriteLine("Server stoped");
            }
        }
    }
}
