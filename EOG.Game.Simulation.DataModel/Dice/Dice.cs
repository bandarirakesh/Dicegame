using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOG.Game.Simulation.DataModel
{
    public class Dice
    {
        public Dice()
        {
            Faces = new string[] { "L", "C", "R", ".", ".", "." };
        }
        public string[] Faces { get; }
        public string Roll(int face)
        {
            return Faces[face];
        }
    }
}
