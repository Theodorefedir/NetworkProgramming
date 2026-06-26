using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Client
{
    internal class Program
    {
        static string ReceiveMessage(NetworkStream stream) {
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
        static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;
            IPAddress serverIp = IPAddress.Parse("127.0.0.1");
            int serverPort = 5000;
            TcpClient client = new TcpClient();
            try
            {
                client.Connect(serverIp, serverPort);

                var stream = client.GetStream();
                string nameMessage = ReceiveMessage(stream);
                Console.WriteLine(nameMessage);

                string name = Console.ReadLine();
                SendMessage(stream, name);

                while (true)
                {
                    string choiceMessage = ReceiveMessage(stream);
                    Console.WriteLine(choiceMessage);

                    string choice = Console.ReadLine();
                    SendMessage(stream, choice);

                    string quote = ReceiveMessage(stream);
                    Console.WriteLine(quote);
                }
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
