using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Client
    {
        public string Login { get; set; }
        public string Password { get; set; }
        private List<Quiz> quizzes;

        public Client(string login, string password) { 
            Login = login;
            Password = password;
            quizzes = new List<Quiz>();
        }
        public void AddQuiz(Quiz quiz) {
            quizzes.Add(quiz);
        }
        public List<Quiz> GetQuizzes() {
            return quizzes;
        }

    }
}
