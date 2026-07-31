using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal class Quiz
    {
        public string Name { get; set; }
        public bool IsPrivate { get; set; }
        private List<Card> cards;
        public int bestScore {get; set;}
        public List<QuizResult> QuizResults {get; set;}
        public Quiz(string name, bool isPrivate) { 
            Name = name;
            IsPrivate = isPrivate;
            cards = new List<Card>();
            QuizResults = new List<QuizResult>();
        }
        public void AddCard(Card card) {
            cards.Add(card);
        }
        public int GetCardsCount() { 
            return cards.Count;
        }
        public override string ToString()
        {
            return $"Quiz: {Name}, {cards.Count} questions ";
        }
        public List<Card> GetCards() { 
            return cards ;
        }
        public int CheckAnswers(List<int> indexes, Card card)
        {
            int correct = 0;
            bool isCorrect = indexes.SequenceEqual(card.correctAnswIndexes);
            if (isCorrect)
            {
                correct++;
            }
            return correct;
            
        }
    }
}
