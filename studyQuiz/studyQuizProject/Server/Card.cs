using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Card
    {
        public string Question { get; set; }
        private List<string> answers;
        public List<int> correctAnswIndexes { get; set; }

        public Card()
        {
            answers = new List<string>();
            correctAnswIndexes = new List<int>();
        }
        //public void AddQuestion(string text) { 
        //    Question = text;
        //}

        public List<string> GetAnswers()
        {
            return answers;
        }
        public void AddAnswers(List<string> answrs, List<int> correctAnswIndex) { 
            answers = answrs;
            this.correctAnswIndexes = correctAnswIndex;
        }
        public override string ToString()
        {
            string result = "";
            result += Question + "\n";
            for(int i =0; i<answers.Count; i++) {
                result += $"{i}. {answers[i]}\n";
            }
            return result;
        }        
    }
}
