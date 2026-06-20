using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            IPAddress ip = IPAddress.Parse("127.0.0.1");
            int port = 12345;

            IPEndPoint endPoint = new IPEndPoint(ip, port);

            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Bind(endPoint);

            try {
                server.Listen();
                Console.WriteLine("Server running...");

                while (true) {
                    Socket client = server.Accept();
                    Console.WriteLine($"Client connected: {client.RemoteEndPoint}");

                    string message = "1 - час 2 - дата";
                    byte[] bytes = Encoding.UTF8.GetBytes(message);
                    client.Send(bytes);

                    byte[] clientMessage = new byte[1024];
                    int bytesRead = client.Receive(clientMessage);
                    string receivedMessage = Encoding.UTF8.GetString(clientMessage, 0, bytesRead);
                    //Console.WriteLine(receivedMessage);

                    if (receivedMessage == "1")
                    {
                        message = DateTime.Now.ToString("HH:mm:ss");
                    }
                    else if (receivedMessage == "2")
                    {
                        message = DateTime.Now.ToString("dd.MM.yyyy");
                    }
                    else { message = "ERROR"; }
                        bytes = Encoding.UTF8.GetBytes(message);
                    client.Send(bytes);

                }
            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
            }
            finally { server.Close(); }
        }
    }
}
