using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Шахматы.Model
{
    class MovesChangedEventArgs
    {
        public List<int> xCoordinatesOfMayMoves = new List<int>();
        public List<int> yCoordinatesOfMayMoves = new List<int>();
        public MovesChangedEventArgs(List<int> xCoordinatesOfMayMoves, List<int> yCoordinatesOfMayMoves)
        {
            this.xCoordinatesOfMayMoves = xCoordinatesOfMayMoves;
            this.yCoordinatesOfMayMoves = yCoordinatesOfMayMoves;
        }
    }
}
