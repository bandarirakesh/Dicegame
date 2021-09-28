using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOG.Game.Simulation.UIResources.Intefaces
{
    public interface IPlayer
    {
        int PlayerId { get; set; }
        bool IsWinner { get; set; }
    }
}
