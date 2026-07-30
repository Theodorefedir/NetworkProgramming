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
        public Quiz(string name, bool isPrivate) { 
            Name = name;            
            cards = new List<Card>();
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
        public int CheckAnswers(List<int> indexes)
        {
            int correct = 0;
            foreach (var card in cards) {
                bool isCorrect = indexes.SequenceEqual(card.correctAnswIndexes);

                if (isCorrect)
                {
                    correct ++;
                }
            }
            return correct;
            
        }
    }
}
