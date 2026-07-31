using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static string ReceiveMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int len = stream.Read(buffer);

            if (len == 0)
                return null;

            return Encoding.UTF8.GetString(buffer, 0, len);

        }
        static void SendMessage(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data);
        }
        static void Main(string[] args)
        {
            IPAddress serverIp = IPAddress.Parse("127.0.0.1");
            int serverPort = 5000;
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(serverIp, serverPort);
                var stream = client.GetStream();
                while (client.Connected)
                {
                    string serverMessage = ReceiveMessage(stream);
                    Console.WriteLine(serverMessage);
                    bool needInput = serverMessage.Contains(": ") ||serverMessage.Contains("?");
                    if (needInput)
                    {
                        string input = Console.ReadLine();
                        SendMessage(stream, input);
                    }

                }

            }
            catch (Exception ex) { 
                Console.WriteLine(ex.ToString());
            }
            finally { client.Close(); }
        }
    }
}
