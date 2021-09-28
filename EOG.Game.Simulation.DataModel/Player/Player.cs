using EOG.Game.Simulation.UIResources;
using EOG.Game.Simulation.UIResources.Intefaces;
using System.Collections.Generic;

namespace EOG.Game.Simulation.DataModel
{
    public class Player : PropertyChnageBase, IPlayer
    {
        public Player(int playerId)
        {
            PlayerId = playerId;
            NoOfChips = 3;

            Dices.Add(new Dice());
            Dices.Add(new Dice());
            Dices.Add(new Dice());
        }

        private List<Dice> Dices { get; set; } = new List<Dice>();

        private int _playerId;
        public int PlayerId
        {
            get { return _playerId; }
            set
            {
                _playerId = value;
                OnPropertyChanged(nameof(PlayerId));
            }
        }

        public Player NextPlayer { get; set; }
        public Player PreviousPlayer { get; set; }

        private bool _isWinner;
        public bool IsWinner
        {
            get { return _isWinner; }
            set 
            {
                _isWinner = value; 
                OnPropertyChanged(nameof(IsWinner)); 
            }
        }
        public int NoOfChips { get; set; }

        public void SetNext(Player p)
        {
            NextPlayer = p;
        }

        public void SetPrev(Player p)
        {
            PreviousPlayer = p;
        }

        public List<Dice> GetPlayerDices()
        {
            return NoOfChips >= 3 ? Dices : Dices.GetRange(0, NoOfChips);
        }
    }


    public class DoubleLinkedList
    {
        Player head;
        Player tail;

        public Player CreateDoublyLinkedList()
        {
            head = new Player(0);
            Player node = new Player(0);
            node.SetNext(null);
            node.SetPrev(null);
            head = node;
            tail = node;
            return head;
        }
    }
}
