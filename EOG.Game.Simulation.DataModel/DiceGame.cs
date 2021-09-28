using EOG.Game.Simulation.UIResources.Intefaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EOG.Game.Simulation.DataModel
{
    public class DiceGame : IGame
    {
        public DiceGame(int id, int noOfPlayers)
        {
            GameId = id;
            NoOfPlayers = noOfPlayers;
        }

        public int GameId { get; }
        public Player Winner { get; private set; }
        public string GameName { get; set; }
        public List<IPlayer> Players { get; set; }
        IPlayer IGame.Winner => Winner;

        public int NoOfTurns { get; private set; }

        public int NoOfPlayers { get; private set; }

        public void PlayGame(Random random)
        {
            var head = GetHeadPlayers();
            var currentPlayer = head;

            while (true)
            {
                if (IsWinner(head))
                {
                    Winner = currentPlayer;
                    Winner.IsWinner = true;

                    break;
                }

                if (currentPlayer.NoOfChips == 0)
                {
                    currentPlayer = currentPlayer.NextPlayer;
                    continue;
                }

                var dices = currentPlayer.GetPlayerDices();
                NoOfTurns++;

                foreach (var dice in dices)
                {
                    int diceFace = random.Next(0, 6);
                    var dr = dice.Roll(diceFace);
                    switch (dr)
                    {
                        case ".":
                            break;
                        case "C":
                            currentPlayer.NoOfChips--;
                            break;
                        case "R":
                            currentPlayer.NextPlayer.NoOfChips++;
                            currentPlayer.NoOfChips--;
                            break;
                        case "L":
                            currentPlayer.NoOfChips--;
                            currentPlayer.PreviousPlayer.NoOfChips++;
                            break;
                    }
                }

                currentPlayer = currentPlayer.NextPlayer;
            }
        }

        private bool IsWinner(Player head)
        {
            int numOfPlayersWithChips = 0;
            var tempNode = head;
            for (int i = 0; i < NoOfPlayers; i++)
            {
                if (tempNode.NoOfChips > 0)
                {
                    numOfPlayersWithChips++;
                }
                tempNode = tempNode.NextPlayer;
            }
            return numOfPlayersWithChips == 1;
        }

        private Player GetHeadPlayers()
        {
            var dList = new DoubleLinkedList();
            var head = dList.CreateDoublyLinkedList();

            //var headPlayer = new Player(null, null);
            //var linkedList = new LinkedList() { Head = headPlayer };
            Player tail = head;
            head.PlayerId = 0;
            for (int i = 1; i < NoOfPlayers; i++)
            {
                var newPlayer = new Player(i);

                if (i == 1)
                {
                    head.SetNext(newPlayer);
                    newPlayer.SetPrev(head);
                    // newPlayer.SetNext(head);
                    tail = newPlayer;
                }
                else
                {
                    tail.SetNext(newPlayer);
                    newPlayer.SetPrev(tail);
                    //newPlayer.SetNext(head);
                    tail = newPlayer;
                }

                if (i == NoOfPlayers - 1)
                {
                    head.SetPrev(tail);
                    tail.SetNext(head);
                }

                //if (i == 1)
                //{
                //    var player = new Player(null, linkedList.Head);
                //    linkedList.Head.NextPlayer = player;
                //}

                //var pl =new Player(linkedList.Head.NextPlayer, )
            }

            return head;
        }
    }
}
