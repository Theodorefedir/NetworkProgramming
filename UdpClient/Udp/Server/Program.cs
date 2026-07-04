using System.Net;
using System.Net.Sockets;
using System.Numerics;
using System.Text;

namespace Server
{
    internal class Program
    {
        static void SendMessage(string message, UdpClient udpClient, IPEndPoint clientEndPoint) {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            udpClient.Send(bytes, clientEndPoint);
        }

        static string Receive(ref IPEndPoint clientEndPoint, UdpClient udpClient) {
            byte[] bytes = udpClient.Receive(ref clientEndPoint);
            string message = Encoding.UTF8.GetString(bytes);
            return message;
            
        }
        static void Main(string[] args)
        {
            int port = 500;
            UdpClient udpClient = new UdpClient(port);


            IPEndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            
            Game game = new Game();
            
            do
            {
                Console.WriteLine(Receive(ref clientEndPoint, udpClient));
                Player player = new Player(clientEndPoint);
                game.AddPlayer(player);
                game.CountOfPlayers++;
            } while (game.CountOfPlayers<2);

            do {
                SendMessage("1 - rock, 2 - paper, 3 - scissors", udpClient, game.players[0].endPoint);
                SendMessage("1 - rock, 2 - paper, 3 - scissors", udpClient, game.players[1].endPoint);

                var tempEndPoint = game.players[0].endPoint;
                string choiceString = Receive(ref tempEndPoint, udpClient);
                game.players[0].Choice = int.Parse(choiceString);
                tempEndPoint = game.players[1].endPoint;
                choiceString = Receive(ref tempEndPoint, udpClient);
                game.players[1].Choice = int.Parse(choiceString);
                game.Comparer();
                SendMessage(game.players[0].Message, udpClient, game.players[0].endPoint);
                SendMessage(game.players[1].Message, udpClient, game.players[1].endPoint);

            } while (game.Round<=5);

        }
    }
}
