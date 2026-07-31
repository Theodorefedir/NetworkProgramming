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
        //public Client currentClient { get; set; }
        public Server(string ip, int port)
        {
            listener = new TcpListener(IPAddress.Parse(ip), port);
            clients = new List<Client>();
            ServerQuizzes = new List<Quiz>();
        }
        public void SendMessage(NetworkStream stream, string message)
        {
            byte[] byteMessage = Encoding.UTF8.GetBytes(message);
            stream.Write(byteMessage);
        }
        public string ReceiveMessage(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
        }
        public bool LoginExists(string login)
        {
            return clients.Any(c => c.Login == login);
        }
        public void SignIn(NetworkStream stream)
        {
            SendMessage(stream, "Enter your login: ");
            var login = ReceiveMessage(stream);
            if (!LoginExists(login) && !String.IsNullOrEmpty(login))
            {
                SendMessage(stream, "Enter password: ");
                var password = ReceiveMessage(stream);
                if (!String.IsNullOrEmpty(password))
                {
                    var newClient = new Client(login, password);
                    SendMessage(stream, "Registration successful!");
                    SendMessage(stream, "Please login to your new account: ");
                    clients.Add(newClient);
                }
                else
                {
                    SendMessage(stream, "This password is not allowed");
                }
            }
            else
            {
                SendMessage(stream, "This login is already taken or empty");
            }
        }
        public Client LogIn(NetworkStream stream)
        {
            SendMessage(stream, "Enter your login: ");
            var login = ReceiveMessage(stream);
            SendMessage(stream, "Enter password: ");
            var password = ReceiveMessage(stream);
            var client = clients.FirstOrDefault(c => c.Login == login && c.Password == password);
            if (client == null)
            {
                SendMessage(stream, "Wrong login or password!");
                return null;
            }
            SendMessage(stream, $"Welcome, {login}!");
            return client;
        }
        public Client Authorization(NetworkStream stream)
        {
            while (true)
            {
                SendMessage(stream, "1 - Sign in, 2 - Log in, 3 - Exit : ");
                string choice = ReceiveMessage(stream);

                switch (choice)
                {
                    case "1":
                        SignIn(stream);
                        break;

                    case "2":
                        Client client = LogIn(stream);
                        if (client != null)
                            return client;
                        break;

                    case "3":
                        SendMessage(stream, "Goodbye!");
                        return null;

                    default:
                        SendMessage(stream, "Invalid choice!");
                        break;
                }
            }
        }
        public Card CreateCard(NetworkStream stream)
        {
            Card card = new Card();
            while (true)
            {
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
            while (true)
            {
                SendMessage(stream, "Enter answers (comma separated): ");
                string answersInput = ReceiveMessage(stream);
                answers = answersInput.Split(',').Select(a => a.Trim()).ToList();
                if (answers.Count < 2)
                {
                    SendMessage(stream, "At least 2 answers required!");
                    continue;
                }
                break;
            }
            while (true)
            {
                SendMessage(stream, "Enter correct answer numbers (comma separated, 0 - first answer): ");
                string correctInput = ReceiveMessage(stream);
                correctIndexes = correctInput.Split(',').Select(a => int.Parse(a.Trim())).ToList();
                bool valid = true;
                foreach (int index in correctIndexes)
                {
                    if (index < 0 || index >= answers.Count)
                    {
                        SendMessage(stream, $"Invalid answer number: {index}");
                        valid = false;
                        break;
                    }
                }
                if (!valid) {
                    continue;
                }
                break;
            }
            card.AddAnswers(answers, correctIndexes);
            return card;
        }
        public void AddCardsToQuiz(NetworkStream stream, Quiz quiz, Client client)
        {
            while (true)
            {
                SendMessage(stream, "1 - Add card, 2 - Save and exit : ");
                string choice = ReceiveMessage(stream);
                switch (choice)
                {
                    case "1":
                        quiz.AddCard(CreateCard(stream));
                        break;
                    case "2":
                        if (quiz.IsPrivate)
                        {
                            client.AddQuiz(quiz);
                        }
                        else
                        {
                            ServerQuizzes.Add(quiz);
                        }
                        SendMessage(stream, "Quiz saved!");
                        return;
                    default:
                        SendMessage(stream, "Invalid choice! Try again.");
                        break;
                }
            }
        }
        public void CreateQuiz(NetworkStream stream, Client client)
        {
            SendMessage(stream, "Enter quiz name: ");
            var quizeName = ReceiveMessage(stream);
            while (true)
            {
                SendMessage(stream, "press 1 - if you want to make your quiz private or 2 - if you want it to be public: ");
                var choice = ReceiveMessage(stream);
                if (choice == "1")
                {
                    Quiz quiz = new Quiz(quizeName, true);
                    AddCardsToQuiz(stream, quiz, client);
                    break;
                }
                else if (choice == "2")
                {
                    Quiz quiz = new Quiz(quizeName, false);
                    AddCardsToQuiz(stream, quiz, client);
                    break;
                }
                else
                {
                    SendMessage(stream, "Wrong choice ");
                }
            }
        }
        public void StartQuiz(NetworkStream stream, Quiz quiz, Client client)
        {
            List<int> indexes = new List<int>();
            List<Card> cards = quiz.GetCards();
            int result = 0;
            foreach (var card in cards)
            {
                indexes.Clear();
                SendMessage(stream, card.ToString());
                SendMessage(stream, "Enter number of the correct answers: ");
                string choice = ReceiveMessage(stream);
                indexes = choice.Split(',').Select(x => int.Parse(x.Trim())).ToList();
                result += quiz.CheckAnswers(indexes, card);
            }

            quiz.QuizResults.Add(new QuizResult(quiz.Name, result, quiz.GetCardsCount(), client.Login));
            if (result > quiz.bestScore)
            {
                quiz.bestScore = result;
                SendMessage(stream, $"Congratulations! you have a new best result: {result}/{quiz.GetCardsCount()}");
            }
            else
            {
                SendMessage(stream, $"Your current result: {result}/{quiz.GetCardsCount()}, your best result: {quiz.bestScore}/{quiz.GetCardsCount()}\n");
            }

        }
        public void TakeQuiz(NetworkStream stream, Client client)
        {
            SendMessage(stream, "1 - your private quizes\n" +
                "2 - public server quizes\n : ");
            var choice = ReceiveMessage(stream);
            int quizIndex;
            switch (choice)
            {
                case "1":
                    for (int i = 0; i < client.GetQuizzes().Count; i++)
                    {
                        SendMessage(stream, $"{i}. {client.GetQuizzes()[i].ToString()}");
                    }
                    SendMessage(stream, "Choze quiz you want to take: ");
                    quizIndex = int.Parse(ReceiveMessage(stream));
                    StartQuiz(stream, client.GetQuizzes()[quizIndex], client);
                    break;
                case "2":
                    for (int i = 0; i < ServerQuizzes.Count; i++)
                    {
                        SendMessage(stream, $"{i}. {ServerQuizzes[i].ToString()}");
                    }
                    SendMessage(stream, "Choze quiz you want to take: ");
                    quizIndex = int.Parse(ReceiveMessage(stream));
                    StartQuiz(stream, ServerQuizzes[quizIndex], client);
                    break;
                default:
                    SendMessage(stream, "Invalid choice!");
                    break;
            }
        }
        public List<QuizResult> SelectTop20(Quiz quiz)
        {
            var results = quiz.QuizResults;
            var top20 = results.OrderByDescending(r => r.Score).Take(20).ToList();
            return top20;
        }
        public void ShowRating(NetworkStream stream)
        {
            while (true)
            {
                if (ServerQuizzes == null || ServerQuizzes.Count == 0)
                {
                    SendMessage(stream, "No public quizzes available!");
                    return;
                }
                for (int i = 0; i < ServerQuizzes.Count; i++)
                {
                    SendMessage(stream, $"{i}. {ServerQuizzes[i].Name}");
                }
                break;
            }
            while (true)
            {
                SendMessage(stream, "Choose quiz (number): ");
                string input = ReceiveMessage(stream);
                if (!int.TryParse(input, out int quizIndex) || quizIndex < 0 || quizIndex >= ServerQuizzes.Count)
                {
                    SendMessage(stream, "Invalid choice!");
                    continue;
                }
                var selectedQuiz = ServerQuizzes[quizIndex];
                var top20 = SelectTop20(selectedQuiz);
                if (top20.Count == 0)
                {
                    SendMessage(stream, "Nobody has completed this quiz yet.");
                }
                else
                {
                    foreach (var t in top20)
                    {
                        SendMessage(stream, t.ToString());
                    }
                }
                break;
            }
        }
        public bool MainMenu(NetworkStream stream, Client client)
        {
            while (true)
            {
                SendMessage(stream, " 1 - Create new quiz\n " +
                    "2 - take an existing one\n " +
                    "3 - show top 20 in quiz\n " +
                    "4 - exit\n : ");
                var choice = ReceiveMessage(stream);
                switch (choice)
                {
                    case "1":
                        CreateQuiz(stream, client);
                        return false;
                    case "2":
                        TakeQuiz(stream, client);
                        return false;
                    case "3":
                        ShowRating(stream);
                        return false;
                    case "4":
                        return true;
                    default:
                        SendMessage(stream, "Invalid choice!");
                        break;
                }
            }
        }
        public void Work(TcpClient client)
        {
            try
            {
                var stream = client.GetStream();

                //Autorization 
                Client currentClient = Authorization(stream);

                if (currentClient == null)
                    return;
                //Create new quiz or take an existing one
                while (true)
                {
                    bool shouldExit = MainMenu(stream, currentClient);
                    if (shouldExit)
                    {
                        SendMessage(stream, "Goodbye!");
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
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
