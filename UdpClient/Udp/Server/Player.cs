using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Server
{
    internal class Player
    {
        public IPEndPoint endPoint { get; set; }
        public int Choice { get; set; }
        public int Score { get; set; }
        public string Name { get; set; }
        public string Message { get; set; }
        public Player(IPEndPoint endPoint) { 
            this.endPoint = endPoint;
            Choice = 0;
        }
        public void SetChoice(int choice)
        {
            Choice = choice;
        }
    }
}
