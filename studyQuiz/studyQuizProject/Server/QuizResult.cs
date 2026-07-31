using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class QuizResult
    {
        public string QuizName { get; set; }
        public int Score { get; set; }
        public int TotalQuestions { get; set; }
        public string UserLogin { get; set; }

        public QuizResult(string quizName, int score, int totalQuestions, string userLogin)
        {
            QuizName = quizName;
            Score = score;
            TotalQuestions = totalQuestions;
            UserLogin = userLogin;
        }
        public override string ToString() {
            return $"{UserLogin} - {Score}/{TotalQuestions}\n";
        }
    }
}
