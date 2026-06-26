using System.Text;

namespace Server
{

    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            TCPserver server = new TCPserver("127.0.0.1", 5000);
            server.Start();

            Console.ReadKey();
        }
    }
}
