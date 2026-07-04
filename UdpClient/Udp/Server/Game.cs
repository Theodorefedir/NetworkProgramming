using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server
{

    internal class Game
    {
        public List<Player> players = new List<Player>();
        public int Round { get; set; }
        public int CountOfPlayers { get; set; } = 0;

        public void AddPlayer(Player player)
        {
            players.Add(player);
        }
        public void Comparer()
        {
            if (players[0].Choice == 1 && players[1].Choice == 1)
            {
                Round++;
                players[0].Message = $"Draw, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"Draw, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 1 && players[1].Choice == 2)
            {
                players[0].Score++;
                Round++;
                players[0].Message = $"You won, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"You loose, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 1 && players[1].Choice == 3)
            {
                players[1].Score++;
                Round++;
                players[0].Message = $"You loose, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"You won, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 2 && players[1].Choice == 1)
            {
                players[1].Score++;
                Round++;
                players[0].Message = $"You loose, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"You won, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 2 && players[1].Choice == 2)
            {
                Round++;
                players[0].Message = $"Draw, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"Draw, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 2 && players[1].Choice == 3)
            {
                players[0].Score++;
                Round++;
                players[0].Message = $"You won, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"You loose, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 3 && players[1].Choice == 1)
            {
                players[0].Score++;
                Round++;
                players[0].Message = $"You won, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"You loose, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 3 && players[1].Choice == 2)
            {
                players[1].Score++;
                Round++;
                players[0].Message = $"You loose, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"You won, score is {players[1].Score}:{players[0].Score}";
            }
            else if (players[0].Choice == 3 && players[1].Choice == 3)
            {
                Round++;
                players[0].Message = $"Draw, score is {players[0].Score}:{players[1].Score}";
                players[1].Message = $"Draw, score is {players[1].Score}:{players[0].Score}";
            }
            else
            {
                players[0].Message = "Error: invalid choice!";
                players[1].Message = "Error: invalid choice!";
            }
        }


    }
}