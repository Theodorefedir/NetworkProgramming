using Amazon.ElasticBeanstalk.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace Server
{
    public class TCPserver
    {
        private readonly TcpListener listener;
        private readonly List<Client> clients;
        private readonly Random random = new Random();
        private readonly string[] serverQuotes = {
            "Життя — це те, що з тобою відбувається, поки ти будуєш плани.",
            "Найкращий спосіб передбачити майбутнє — створити його.",
            "Не бійся йти повільно, бійся стояти на місці.",
            "Успіх — це здатність йти від невдачі до невдачі, не втрачаючи ентузіазму.",
            "Можна здатися, але не програти.",
            "Знання — сила.",
            "Вчора — це історія, завтра — таємниця, сьогодні — подарунок.",
            "Найскладніше — почати, найважче — не зупинитися."
        };   // цитати генерував чат 

        public TCPserver(string ip, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            clients = new List<Client>();
        }
        void SendMessage(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message + "\n");
            stream.Write(data);
        }
        string ReceiveMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer);

            return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        }
        string GetRandomQuote()
        {
            int index = random.Next(serverQuotes.Length);
            return serverQuotes[index];
        }
        public void ClientWork(TcpClient client) {
            string clientName = "noname";
            Console.WriteLine("Client connected: " + client.Client.RemoteEndPoint);

            try
            {
                //byte[] buffer = new byte[1024];
                //string welcomeMessage = "Enter your name: ";
                //byte[] bytesMessage = Encoding.UTF8.GetBytes(welcomeMessage);
                //var stream = client.GetStream();
                //stream.Write(bytesMessage);

                //int len = stream.Read(buffer);
                //clientName = Encoding.UTF8.GetString(buffer, 0, len);

                //Client clientObj = new Client { Name = clientName, TcpClient = client, When = DateTime.Now };
                //clients.Add(clientObj);

                var stream = client.GetStream();
                string message = "Enter your name: ";
                SendMessage(stream, message);
                clientName = ReceiveMessage(stream);
                Client clientObj = new Client { Name = clientName, TcpClient = client, When = DateTime.Now };
                clients.Add(clientObj);
                while (client.Connected)
                {
                    string choiceMessage = "Enter 1 to get quote ot 2 to Exit";
                    SendMessage(stream, choiceMessage);
                    string choice = ReceiveMessage(stream);

                    if (choice == "1")
                    {
                        string quote = GetRandomQuote();
                        SendMessage(stream, quote);
                        clientObj.quotes.Add(quote);
                    }
                    else if (choice == "2")
                    {
                        SendMessage(stream, "Bye");
                        break;
                    }
                    else
                    {
                        SendMessage(stream, "wrong number, only 1 and 2 are acceptable");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally {
                client.Close();
                Console.WriteLine($"client {clientName} disconnected");
            }
        }
        public void Start()
        {
            listener.Start(10);
            Console.WriteLine("Server started");

            while (true)
            {
                try
                {
                    TcpClient client = listener.AcceptTcpClient();
                    Task.Run(() => ClientWork(client));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}
