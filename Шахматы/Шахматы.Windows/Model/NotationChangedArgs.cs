using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Шахматы.Model
{
    class NotationChangedArgs
    {
        public string Notation;
        public bool NewLine;
        public bool ThisIsSecondMove;
        public NotationChangedArgs(string notation, int indexOfMover, bool thisIsSecondMove)
        {
            this.Notation = notation;
            if (indexOfMover == 1 && !thisIsSecondMove)
                NewLine = false;
            else
                NewLine = true;
            ThisIsSecondMove = thisIsSecondMove;
        }
    }
}
