using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Шахматы.Model
{
    class QueenCheckerArgs
    {
        public int X;
        public int Y;
        public bool IsWhite;
        public QueenCheckerArgs(int x,int y,int indexOfMover)
        {
            X = x;
            Y = y;
            if (indexOfMover == 1)
                IsWhite = true;
            else
                IsWhite = false;
        }
    }
}
