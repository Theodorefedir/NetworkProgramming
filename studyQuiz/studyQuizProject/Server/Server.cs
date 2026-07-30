using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Server
    {
        private readonly List<Client> clients;
        private readonly TcpListener listener;
        private List<Quiz> ServerQuizzes;
        public Client currentClient { get; set; }
        public Server(string ip, int port) {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            clients = new List<Client>();
        }
        public void SendMessage(NetworkStream stream, string message) {
            byte[] byteMassage = Encoding.UTF8.GetBytes(message);
            stream.Write(byteMassage);
        }
        public string ReceiveMessage(NetworkStream stream) {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        }
        public bool LoginExists(string login)
        {
            foreach (Client client in clients) {
                if (client.Login == login) { 
                    return true;
                }                
            }
            return false;
        }
        public void SignIn(NetworkStream stream) {
            SendMessage(stream, "Enter your login");
            var login = ReceiveMessage(stream);
            if (!LoginExists(login) && !String.IsNullOrEmpty(login))
            {
                SendMessage(stream, "Enter password");
                var password = ReceiveMessage(stream);
                if (!String.IsNullOrEmpty(password))
                {
                    var newClient = new Client(login, password);
                    SendMessage(stream, "Registration successful!");
                    SendMessage(stream, "Please login to your new account.");
                    clients.Add(newClient);
                }
                else {
                    SendMessage(stream, "This password is not allowed");
                }
            }
            else
            {
                SendMessage(stream, "This login is already taken or empty");
            }
        }
        public bool LogIn(NetworkStream stream) {
            SendMessage(stream, "Enter your login");
            var login = ReceiveMessage(stream);
            SendMessage(stream, "Enter password");
            var password = ReceiveMessage(stream);
            var client = clients.FirstOrDefault(c => c.Login == login && c.Password == password);
            if (client != null)
            {
                currentClient = client;
                SendMessage(stream, $"Welcome, {login}!");
                return true;
            }
            else
            {
                SendMessage(stream, "Wrong login or password! ");
                return false;
            }
        }
        public void Authorization(NetworkStream stream)
        {
            while (true)
            {
                SendMessage(stream, "1 - Sign in, 2 - Log in, 3 - Exit");
                string choice = ReceiveMessage(stream);

                switch (choice)
                {
                    case "1":
                        SignIn(stream);
                        break;

                    case "2":
                        LogIn(stream);
                        if (currentClient != null) { 
                            return;
                        }
                        break;

                    case "3":
                        SendMessage(stream, "Goodbye!");
                        return;

                    default:
                        SendMessage(stream, "Invalid choice!");
                        break;
                }
            }
        }
        public Card CreateCard(NetworkStream stream) {
            Card card = new Card();
            while (true) {
                SendMessage(stream, "write a question: ");
                string question = ReceiveMessage(stream);
                if (string.IsNullOrWhiteSpace(question))
                {
                    SendMessage(stream, "Question cannot be empty!");
                    continue;
                }                
                card.Question = question;
                break;
            }
            List<string> answers = new List<string>();
            List<int> correctIndexes = new List<int>();
            while (true) {
                SendMessage(stream, "Enter answers (comma separated):");
                string answersInput = ReceiveMessage(stream);
                answers = answersInput.Split(',').Select(a => a.Trim()).ToList();
                if (answers.Count < 2)
                {
                    SendMessage(stream, "At least 2 answers required!");
                    continue;
                }
                break;
            }
            while (true) {
                SendMessage(stream, "Enter correct answer numbers (comma separated, 0 - first answer):");
                string correctInput = ReceiveMessage(stream);
                correctIndexes = correctInput.Split(',').Select(a => int.Parse(a.Trim())).ToList();
                foreach (int index in correctIndexes)
                {
                    if (index < 0 || index >= answers.Count)
                    {
                        SendMessage(stream, $"Invalid answer number: {index}");
                        continue;
                    }
                }
                break ;
            }
            card.AddAnswers(answers, correctIndexes);
            return card;
        }
        public void AddCardsToQuiz(NetworkStream stream, Quiz quiz) {
            while (true)
            {
                SendMessage(stream, "1 - Add card, 2 - Save and exit");
                string choice = ReceiveMessage(stream);
                switch (choice)
                {
                    case "1":
                        quiz.AddCard(CreateCard(stream));
                        break;
                    case "2":
                        SendMessage(stream, "Saving quiz...");
                        if (quiz.IsPrivate == true)
                        {
                            currentClient.AddQuiz(quiz);
                        }
                        else { 
                            ServerQuizzes.Add(quiz);
                        }
                            return;
                    default:
                        SendMessage(stream, "Invalid choice! Try again.");
                        break;
                }
            }
        }
        public void CreateQuiz(NetworkStream stream) {
            SendMessage(stream, "Enter quiz name: ");
            var quizeName = ReceiveMessage(stream);
            while (true) {
                SendMessage(stream, "press 1 if you want to make your quiz private or 2 if you want it to be public: ");
                var choice = ReceiveMessage(stream);
                if (choice == "1")
                {
                    Quiz quiz = new Quiz(quizeName, true);
                    AddCardsToQuiz(stream, quiz);
                    break;
                }
                else if (choice == "2")
                {
                    Quiz quiz = new Quiz(quizeName, false);
                    AddCardsToQuiz(stream, quiz);
                    break;
                }
                else {
                    SendMessage(stream, "Wrong choice ");
                }
            }
        }
        public void StartQuiz(NetworkStream stream, Quiz quiz) {
            List<int> indexes = new List<int>();
            List<Card> cards = quiz.GetCards();
            foreach (var card in cards) {
                SendMessage(stream, card.ToString());
                SendMessage(stream, "Enter number of the correct answers: ");
                string choice = ReceiveMessage(stream);
                indexes.Add(int.Parse(choice));
            }
            var result = quiz.CheckAnswers(indexes);
            if (result > quiz.bestScore)
            {
                SendMessage(stream, $"Congratulations! you have a new best result: {result}/{quiz.GetCardsCount}");
            }
            else {
                SendMessage(stream, $"Your current result: {result}/{quiz.GetCardsCount}, your best result: {quiz.bestScore}/{quiz.GetCardsCount()}");
            }
            
        }
        public void TakeQuiz(NetworkStream stream) {
            SendMessage(stream, "1 - your private quizes\n" +
                "2 - public server quizes");
            var choice = ReceiveMessage(stream);
            int quizIndex;
            switch (choice) { 
                case "1":
                    for (int i = 0; i < currentClient.GetQuizzes().Count; i++)
                    {
                        SendMessage(stream, $"{i + 1}. {currentClient.GetQuizzes()[i].ToString()}");                        
                    }
                    SendMessage(stream, "Choze quiz you want to take");
                    quizIndex = int.Parse(ReceiveMessage(stream));
                    StartQuiz(stream, currentClient.GetQuizzes()[quizIndex]);
                    break; 
                case "2":
                    for (int i = 0; i < ServerQuizzes.Count; i++)
                    {
                        SendMessage(stream, $"{i}. {ServerQuizzes[i].ToString()}");                        
                    }
                    SendMessage(stream, "Choze quiz you want to take");
                    quizIndex = int.Parse(ReceiveMessage(stream));
                    StartQuiz(stream, ServerQuizzes[quizIndex]);
                    break;
            }
        }
        public bool MainMenu(NetworkStream stream) {
            while (true) {
                SendMessage(stream, "1 - Create new quiz\n " +
                    "2 - take an existing one\n " +
                    "3 - show results\n" +
                    " 4 - exit");
                var choice = ReceiveMessage(stream);
                switch (choice)
                {
                    case "1":
                        CreateQuiz(stream);
                        return false;
                    case "2":
                        TakeQuiz(stream);
                        return false;
                    case "3":
                        //ShowResults(stream);
                        return false;
                    case "4":
                        return true;
                    default:
                        SendMessage(stream, "Invalid choice!");
                        return false;
                }
            }
        }
        public void Work(TcpClient client)
        {
            try { 
                var stream = client.GetStream();

                //Autorization 
                Authorization(stream);

                //Create new quiz or take an existing one
                while (true) {
                    bool shouldExit = MainMenu(stream);
                    if (shouldExit)
                    {
                        SendMessage(stream, "Goodbye!");
                        break;
                    }
                }



            }
            catch(Exception ex) {
                Console.WriteLine(ex.Message);
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
                    Task.Run(() => Work(client));

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

    }
}
