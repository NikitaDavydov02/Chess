using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Шахматы.Model
{
    class TwoPlayersModel
    {
        //Fields;
        private bool mustToCapture = false;
        private List<int> xCoordinatesOfCheckersWhichMustbeRemoved = new List<int>();
        private List<int> yCoordinatesOfCheckersWhichMustbeRemoved = new List<int>();
        private List<int> xCoordinatesOfMayMoves = new List<int>();
        private List<int> yCoordinatesOfMayMoves = new List<int>();
        private List<int> xCoordinatesOfCheckersWhichWasRemovedOnThisMove = new List<int>();
        private List<int> yCoordinatesOfCheckersWhichWasRemovedOnThisMove = new List<int>();
        //Коды фигур:
        //1 - чёрная обычная
        //2 - чёрная в дамках
        //3 - белая обычная
        //4 - белая в дамках
        //Properties;
        public int[,] Field { get; private set; }
        public int IndexOfMover { get; private set; }   //1 - white, 2 - black
        //Events
        public event EventHandler<CheckerChangedEventArgs> CheckerChanged;
        public event EventHandler<MovesChangedEventArgs> MovesChanged;
        public event EventHandler<QueenCheckerArgs> QueenChecker;
        public event EventHandler<QueenCheckerArgs> RemoveChecker;
        public event EventHandler<NotationChangedArgs> NotationChanged;
        public event EventHandler MoverChanged;
        //Constructor;
        public TwoPlayersModel()
        {
            Field = new int[8, 8];
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                    Field[x, y] = 0;
        }
        //Methods;
        public void StartTheGame()
        {
            IndexOfMover = 1;
            ToArrangeThePieces();
        }
        private void ToArrangeThePieces()
        {
            for (int i = 1; i < 8; i += 2)
                Field[i, 0] = 1;
            for (int i = 1; i < 8; i += 2)
                Field[i, 2] = 1;
            for (int i = 0; i < 8; i += 2)
                Field[i, 1] = 1;
            for (int i = 1; i < 8; i += 2)
                Field[i, 6] = 3;
            for (int i = 0; i < 8; i += 2)
                Field[i, 5] = 3;
            for (int i = 0; i < 8; i += 2)
                Field[i, 7] = 3;
            //Field[3, 4] = 3;
            //Field[2, 3] = 1;
            //Field[2, 1] = 1;
        }
        private void OnCheckerChanged(int startX,int startY,int endX, int endY)
        {
            EventHandler<CheckerChangedEventArgs> chekerChanged = CheckerChanged;
            if (chekerChanged != null)
                chekerChanged(this, new CheckerChangedEventArgs(startX, startY, endX, endY));
        }
        private void OnMovesChanged()
        {
            EventHandler<MovesChangedEventArgs> movesChanged = MovesChanged;
            if (movesChanged != null)
                movesChanged(this, new MovesChangedEventArgs(xCoordinatesOfMayMoves, yCoordinatesOfMayMoves));
        }
        private void OnMoverChanged()
        {
            EventHandler moverChanged = MoverChanged;
            if (moverChanged != null)
                moverChanged(this, new EventArgs());
        }
        private void OnQueenChecker(int x,int y)
        {
            EventHandler<QueenCheckerArgs> queenChecker = QueenChecker;
            if (queenChecker != null)
                queenChecker(this, new QueenCheckerArgs(x, y, IndexOfMover));
        }
        private void OnRemoveChecker(int x, int y)
        {
            EventHandler<QueenCheckerArgs> removeChecker = RemoveChecker;
            if (removeChecker != null)
                removeChecker(this, new QueenCheckerArgs(x, y, IndexOfMover));
        }
        private void OnNotationChanged(NotationChangedArgs args)
        {
            EventHandler<NotationChangedArgs> notationChanged = NotationChanged;
            if (notationChanged != null)
                notationChanged(this, args);
        }
        private bool checkerCaptured;
        private bool thisIsSecondMove;
        private int startX;
        private int startY;
        private bool thisIsCapturing = false;
        public bool MakeMove(int endX,int endY)
        {
            Field[endX, endY] = Field[startX, startY];
            Field[startX, startY] = 0;
            if (IndexOfMover == 1 && endY == 0)
            {
                OnQueenChecker(startX, startY);
                Field[endX, endY] = 4;
            }
            if (IndexOfMover == 2 && endY == 7)
            {
                OnQueenChecker(startX, startY);
                Field[endX, endY] = 2;
            }
            checkerCaptured = CheckerCaptured(startX, startY, endX, endY);
            OnNotationChanged(new NotationChangedArgs(CreateNotation(startX, startY, endX, endY, checkerCaptured), IndexOfMover, thisIsSecondMove));
            if (checkerCaptured)
                RemoveCheckers();
            OnCheckerChanged(startX, startY, endX, endY);
            RemoveMoves();
            OnMovesChanged();
            thisIsCapturing = false;
            if (mustToCapture)
                mustToCapture = false;
            if (!CanPlayerMoveAgain(endX, endY)||!checkerCaptured)
            {
                thisIsSecondMove = false;
                if (IndexOfMover == 1)
                    IndexOfMover = 2;
                else
                    IndexOfMover = 1;
                if (!GameIsOver())
                {
                    xCoordinatesOfCheckersWhichMustbeRemoved.Clear();
                    yCoordinatesOfCheckersWhichMustbeRemoved.Clear();
                    RemoveMoves();
                    if (mustToCapture)
                        mustToCapture = false;
                    MustToCapture();
                    xCoordinatesOfCheckersWhichWasRemovedOnThisMove.Clear();
                    yCoordinatesOfCheckersWhichWasRemovedOnThisMove.Clear();
                    OnMoverChanged();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                OnMovesChanged();
                thisIsSecondMove = true;
                startX = endX;
                startY = endY;
                return true;
            }
            //OnCheckerChanged(startX, startY, endX, endY);
        }
        private int xCoordinateOfCapturedChecker;
        private int yCoordinateOfCapturedChecker;
        private string CreateNotation(int startX, int startY, int endX, int endY, bool isCapturing)
        {
            string notation = "";
            if (!thisIsSecondMove)
            {
                if (startX == 0)
                    notation = "a";
                if (startX == 1)
                    notation = "b";
                if (startX == 2)
                    notation = "c";
                if (startX == 3)
                    notation = "d";
                if (startX == 4)
                    notation = "e";
                if (startX == 5)
                    notation = "f";
                if (startX == 6)
                    notation = "g";
                if (startX == 7)
                    notation = "h";
                notation += (7 - startY + 1).ToString();
            }
            if (isCapturing)
                notation += " : ";
            else
                notation += " - ";
            if (endX == 0)
                notation += "a";
            if (endX == 1)
                notation += "b";
            if (endX == 2)
                notation += "c";
            if (endX == 3)
                notation += "d";
            if (endX == 4)
                notation += "e";
            if (endX == 5)
                notation += "f";
            if (endX == 6)
                notation += "g";
            if (endX == 7)
                notation += "h";
            notation += (7 - endY + 1).ToString();
            return notation;
        }
        private bool CheckerCaptured(int startX, int startY, int endX, int endY)
        {
            for(int i = 0; i < xCoordinatesOfCheckersWhichMustbeRemoved.Count; i++)
            {
                if(PointBelongsDiagonal(startX, startY, endX, endY, xCoordinatesOfCheckersWhichMustbeRemoved[i], yCoordinatesOfCheckersWhichMustbeRemoved[i]))
                {
                    xCoordinateOfCapturedChecker = xCoordinatesOfCheckersWhichMustbeRemoved[i];
                    yCoordinateOfCapturedChecker = yCoordinatesOfCheckersWhichMustbeRemoved[i];
                    //xCoordinatesOfCheckersWhichMustbeRemoved.Clear();
                    //yCoordinatesOfCheckersWhichMustbeRemoved.Clear();
                    return true;
                }
            }
            return false;
        }
        private bool PointBelongsDiagonal(int startX, int startY, int endX, int endY, int x, int y)
        {
            if (((startX < x && x < endX) || (startX > x && x > endX)) && ((startY < y && y < endY) || (startY > y && y > endY))&&(((Field[x,y]==3|| Field[x, y] == 4)&&IndexOfMover==2)|| ((Field[x, y] == 1 || Field[x, y] == 2) && IndexOfMover == 1)))
                return true;
            return false;
        }
        private void MustToCapture()
        {
            thisIsCapturing = false;
            xCoordinatesOfCheckersWhichWasRemovedOnThisMove.Clear();
            yCoordinatesOfCheckersWhichWasRemovedOnThisMove.Clear();
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    if((IndexOfMover == 1 && (Field[x, y] == 3 || Field[x, y] == 4)) || (IndexOfMover == 2 && (Field[x, y] == 1 || Field[x, y] == 2)))
                    {
                        UpdateMayMoves(x, y);
                        if (thisIsCapturing)
                            mustToCapture = true;
                    }
                }
        }
        private void RemoveMoves()
        {
            xCoordinatesOfMayMoves.Clear();
            yCoordinatesOfMayMoves.Clear();
        }
        public void ShowMayMoves(int x,int y)
        {
            if (thisIsSecondMove)
                return;
            if (xCoordinatesOfMayMoves.Count > 0)
            {
                RemoveMoves();
                xCoordinatesOfCheckersWhichMustbeRemoved.Clear();
                yCoordinatesOfCheckersWhichMustbeRemoved.Clear();
                //OnMovesChanged();

            }
            if ((IndexOfMover == 1 && (Field[x, y] == 3 || Field[x, y] == 4))|| (IndexOfMover == 2 && (Field[x, y] == 1 || Field[x, y] == 2)))
            {
                //UpdateMayMoves(x, y);
                if (mustToCapture)
                {
                    UpdateMayMovesForSecondMove(x, y);
                    List<int> xCoordinatesOfMovesToDelate = new List<int>();
                    List<int> yCoordinatesOfMovesToDelate = new List<int>();
                    for (int a = 0; a < xCoordinatesOfMayMoves.Count; a++)
                    {
                        if (!CheckIsThereAreCheckerBetween(x, y, xCoordinatesOfMayMoves[a], yCoordinatesOfMayMoves[a], true))
                        {
                            xCoordinatesOfMovesToDelate.Add(a);
                            yCoordinatesOfMovesToDelate.Add(a);
                        }
                    }
                    for (int i = xCoordinatesOfMovesToDelate.Count - 1; i >= 0; i--)
                    {
                        xCoordinatesOfMayMoves.RemoveAt(xCoordinatesOfMovesToDelate[i]);
                        yCoordinatesOfMayMoves.RemoveAt(yCoordinatesOfMovesToDelate[i]);
                    }
                }
                else
                {
                    UpdateMayMoves(x, y);
                }
                OnMovesChanged();
                startX = x;
                startY = y;
            }
        }
        private bool CanPlayerMoveAgain(int x,int y)
        {
            xCoordinatesOfCheckersWhichMustbeRemoved.Clear();
            yCoordinatesOfCheckersWhichMustbeRemoved.Clear();
            if (UpdateMayMovesForSecondMove(x,y))
                return true;
            return false;
        }
        private bool UpdateMayMovesForSecondMove(int x,int y)
        {
            xCoordinatesOfMayMoves.Clear();
            yCoordinatesOfMayMoves.Clear();
            int leftMargin = x - 0;
            int rightMargin = 7 - x;
            int topMargin = y - 0;
            int bottonMargin = 7 - y;
            howManyDidYouAdd = 0;
            wrongDirection = false;
            //<right-top>
            if (rightMargin > topMargin && topMargin != 0)
                for (int i = 1; i <= topMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y - i) && CheckerCaptured(x, y, x + i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (rightMargin < topMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y - i) && CheckerCaptured(x, y, x + i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (rightMargin == topMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y - i) && CheckerCaptured(x, y, x + i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            //</right-top>
            howManyDidYouAdd = 0;
            wrongDirection = false;
            //<left-top>
            if (leftMargin > topMargin && topMargin != 0)
                for (int i = 1; i <= topMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y - i) && CheckerCaptured(x, y, x - i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (leftMargin < topMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y - i) && CheckerCaptured(x, y, x - i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (leftMargin == topMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y - i) && CheckerCaptured(x, y, x - i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            //</left-top>
            howManyDidYouAdd = 0;
            wrongDirection = false;
            //<right-botton>
            if (rightMargin > bottonMargin && bottonMargin != 0)
                for (int i = 1; i <= bottonMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y + i) && CheckerCaptured(x, y, x + i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (rightMargin < bottonMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y + i) && CheckerCaptured(x, y, x + i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (rightMargin == bottonMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y + i) && CheckerCaptured(x, y, x + i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            //</right-botton>
            howManyDidYouAdd = 0;
            wrongDirection = false;
            //<left-botton>
            if (leftMargin > bottonMargin && bottonMargin != 0)
                for (int i = 1; i <= bottonMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y + i) && CheckerCaptured(x, y, x - i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (leftMargin < bottonMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y + i) && CheckerCaptured(x, y, x - i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (leftMargin == bottonMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                    if (ThisMoveIsCorrect(x, y, x - i, y + i) && CheckerCaptured(x, y, x - i, y + i))
                    {
                        if (wrongDirection) break;
                        if (ThisMoveIsCorrect(x, y, x - i, y + i) && CheckerCaptured(x, y, x - i, y + i))
                        {
                            xCoordinatesOfMayMoves.Add(x - i);
                            yCoordinatesOfMayMoves.Add(y + i);
                        }
                    }
            //</left-botton>
            wrongDirection = false;
            howManyDidYouAdd = 0;
            if (xCoordinatesOfMayMoves.Count == 0)
                return false;
            return true;
        }
        private bool ThisMoveIsCorrect(int startX, int startY, int endX, int endY)
        {
            if (IndexOfMover == 1)
            {
                if (Field[startX, startY] == 3)
                {
                    if ((((endX == startX + 1 || endX == startX - 1) && endY == startY - 1) || (Math.Abs(endX - startX) == 2 && Math.Abs(endY - startY) == 2 && CheckIsThereAreCheckerBetween(startX, startY, endX, endY, false))) && Field[endX, endY] == 0)
                        return true;
                    return false;
                }
                if (Field[startX, startY] == 4)
                {
                    if (CheckIsThereAreCheckerBetween(startX, startY, endX, endY, true) && Field[endX, endY] == 0)
                        return true;
                    return false;
                }
            }
            else
            {
                if (Field[startX, startY] == 1)
                {
                    if ((((endX == startX + 1 || endX == startX - 1) && endY == startY + 1) || (Math.Abs(endX - startX) == 2 && Math.Abs(endY - startY) == 2 && CheckIsThereAreCheckerBetween(startX, startY, endX, endY, false))) && Field[endX, endY] == 0)
                        return true;
                    return false;
                }
                if (Field[startX, startY] == 2)
                {
                    if (CheckIsThereAreCheckerBetween(startX, startY, endX, endY, true) && Field[endX, endY] == 0)
                        return true;
                    return false;
                }
            }
            return false;
        }
        private int howManyDidYouAdd = 0;
        private bool CheckIsThereAreCheckerBetween(int startX, int startY, int endX, int endY, bool returnTrueIfThereAreNotCheckers)
        {
            int howManyCheckersBetween = 0;
            //<New>
            for(int i = 0; i < xCoordinatesOfCheckersWhichWasRemovedOnThisMove.Count; i++)
            {
                if (endX == xCoordinatesOfCheckersWhichWasRemovedOnThisMove[i] && endY == yCoordinatesOfCheckersWhichWasRemovedOnThisMove[i])
                {
                    wrongDirection = true;
                    return false;
                }
            }
            //</New>
            if (((Field[endX,endY]==1|| Field[endX, endY] == 2)&&IndexOfMover==2)||((Field[endX, endY] == 3|| Field[endX, endY] == 4) && IndexOfMover == 1))
            {
                wrongDirection = true;
                return false;
            }
            if (howManyDidYouAdd == 1&& Field[endX, endY] != 0)
            {
                wrongDirection = true;
                return false;
            }
            if (Field[endX, endY] != 0)
                return false;
            if (Math.Abs(endX - startX) == Math.Abs(endY - startY)&& howManyDidYouAdd < 2)
            {
                if (Field[startX, startY] == 1 || Field[startX, startY] == 3)
                {
                    if(endY > startY && endX > startX)
                        if((((Field[startX+1,startY+1]==1||Field[startX+1,startY+1]==2)&&IndexOfMover==1)||((Field[startX + 1, startY + 1] == 3 || Field[startX + 1, startY + 1] == 4) && IndexOfMover == 2)) && Field[endX, endY] == 0)
                        {
                            thisIsCapturing = true;
                            xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX + 1);
                            yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY + 1);
                            return true;
                        }
                    if (endY > startY && endX < startX)
                        if ((((Field[startX - 1, startY + 1] == 1 || Field[startX - 1, startY + 1] == 2) && IndexOfMover == 1) || ((Field[startX - 1, startY + 1] == 3 || Field[startX - 1, startY + 1] == 4) && IndexOfMover == 2)) && Field[endX, endY] == 0)
                        {
                            thisIsCapturing = true;
                            xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX - 1);
                            yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY + 1);
                            return true;
                        }
                    if (endY < startY && endX < startX)
                        if ((((Field[startX - 1, startY - 1] == 1 || Field[startX - 1, startY - 1] == 2) && IndexOfMover == 1) || ((Field[startX - 1, startY - 1] == 3 || Field[startX - 1, startY - 1] == 4) && IndexOfMover == 2)) && Field[endX, endY] == 0)
                        {
                            thisIsCapturing = true;
                            xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX - 1);
                            yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY - 1);
                            return true;
                        }
                    if (endY < startY && endX > startX)
                        if ((((Field[startX + 1, startY - 1] == 1 || Field[startX + 1, startY - 1] == 2) && IndexOfMover == 1) || ((Field[startX + 1, startY - 1] == 3 || Field[startX + 1, startY - 1] == 4) && IndexOfMover == 2)) && Field[endX, endY] == 0)
                        {
                            thisIsCapturing = true;
                            xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX + 1);
                            yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY - 1);
                            return true;
                        }
                    if (howManyCheckersBetween == 0)
                        return false;
                }
                else
                {
                    if (endX > startX && endY > startY)
                    {
                        for(int add = 1; add < Math.Abs(endX - startX); add++)
                        {
                            if ((((Field[startX + add, startY + add] == 1 || Field[startX + add, startY + add] == 2) && IndexOfMover == 1) || ((Field[startX + add, startY + add] == 3 || Field[startX + add, startY + add] == 4) && IndexOfMover == 2)) 
                                && Field[endX, endY] == 0&&ThereAreNotThisChecker(startX + add, startY + add))
                            {
                                thisIsCapturing = true;
                                xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX + add);
                                yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY + add);
                                howManyCheckersBetween++;
                                howManyDidYouAdd++;
                            }
                            if (((Field[startX + add, startY + add] == 1 || Field[startX + add, startY + add] == 2) && IndexOfMover == 2) || ((Field[startX + add, startY + add] == 3 || Field[startX + add, startY + add] == 4) && IndexOfMover == 1)||howManyDidYouAdd > 1)
                            {
                                for(int i = 0; i < howManyDidYouAdd; i++)
                                {
                                    xCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(xCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                    yCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(yCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                }
                                return false;
                            }
                        }
                    }
                    if (endX > startX && endY < startY)
                    {
                        for (int add = 1; add < Math.Abs(endX - startX); add++)
                        {
                            if ((((Field[startX + add, startY - add] == 1 || Field[startX + add, startY - add] == 2) && IndexOfMover == 1) || ((Field[startX + add, startY - add] == 3 || Field[startX + add, startY - add] == 4) && IndexOfMover == 2))
                                && Field[endX, endY] == 0 && ThereAreNotThisChecker(startX + add, startY - add))
                            {
                                thisIsCapturing = true;
                                xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX + add);
                                yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY - add);
                                howManyCheckersBetween++;
                                howManyDidYouAdd++;
                            }
                            if (((Field[startX + add, startY - add] == 1 || Field[startX + add, startY - add] == 2) && IndexOfMover == 2) || ((Field[startX + add, startY - add] == 3 || Field[startX + add, startY - add] == 4) && IndexOfMover == 1) || howManyDidYouAdd > 1)
                            {
                                for (int i = 0; i < howManyDidYouAdd; i++)
                                {
                                    xCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(xCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                    yCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(yCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                }
                                return false;
                            }
                        }
                    }
                    if (endX < startX && endY < startY)
                    {
                        for (int add = 1; add < Math.Abs(endX - startX); add++)
                        {
                            if ((((Field[startX - add, startY - add] == 1 || Field[startX - add, startY - add] == 2) && IndexOfMover == 1) || ((Field[startX - add, startY - add] == 3 || Field[startX - add, startY - add] == 4) && IndexOfMover == 2))
                                && Field[endX, endY] == 0 && ThereAreNotThisChecker(startX - add, startY - add))
                            {
                                thisIsCapturing = true;
                                xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX - add);
                                yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY - add);
                                howManyCheckersBetween++;
                                howManyDidYouAdd++;
                            }
                            if (((Field[startX - add, startY - add] == 1 || Field[startX - add, startY - add] == 2) && IndexOfMover == 2) || ((Field[startX - add, startY - add] == 3 || Field[startX - add, startY - add] == 4) && IndexOfMover == 1) || howManyDidYouAdd > 1)
                            {
                                for (int i = 0; i < howManyDidYouAdd; i++)
                                {
                                    xCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(xCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                    yCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(yCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                }
                                return false;
                            }
                        }
                    }
                    if (endX < startX && endY > startY)
                    {
                        for (int add = 1; add < Math.Abs(endX - startX); add++)
                        {
                            if ((((Field[startX - add, startY + add] == 1 || Field[startX - add, startY + add] == 2) && IndexOfMover == 1) || ((Field[startX - add, startY + add] == 3 || Field[startX - add, startY + add] == 4) && IndexOfMover == 2))
                                && Field[endX, endY] == 0 && ThereAreNotThisChecker(startX - add, startY + add))
                            {
                                thisIsCapturing = true;
                                xCoordinatesOfCheckersWhichMustbeRemoved.Add(startX - add);
                                yCoordinatesOfCheckersWhichMustbeRemoved.Add(startY + add);
                                howManyCheckersBetween++;
                                howManyDidYouAdd++;
                            }
                            if (((Field[startX - add, startY + add] == 1 || Field[startX - add, startY + add] == 2) && IndexOfMover == 2) || ((Field[startX - add, startY + add] == 3 || Field[startX - add, startY + add] == 4) && IndexOfMover == 1) || howManyDidYouAdd > 1)
                            {
                                for (int i = 0; i < howManyDidYouAdd; i++)
                                {
                                    xCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(xCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                    yCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(yCoordinatesOfCheckersWhichMustbeRemoved.Count - 1);
                                }
                                return false;
                            }
                        }
                    }
                    if (howManyDidYouAdd == 0 || howManyDidYouAdd == 1)
                        return true;
                }
            }
            return false;
        }
        private bool ThereAreNotThisChecker(int x,int y)
        {
            for(int i = 0; i < xCoordinatesOfCheckersWhichMustbeRemoved.Count; i++)
            {
                if (xCoordinatesOfCheckersWhichMustbeRemoved[i] == x && yCoordinatesOfCheckersWhichMustbeRemoved[i] == y)
                    return false;
            }
            return true;
        }
        private void RemoveCheckers()
        {
            //for(int i = 0; i < xCoordinatesOfCheckersWhichMustbeRemoved.Count; i++)
            //{
            //    Field[xCoordinatesOfCheckersWhichMustbeRemoved[i], yCoordinatesOfCheckersWhichMustbeRemoved[i]] = 0;
            //    OnRemoveChecker(xCoordinatesOfCheckersWhichMustbeRemoved[i], yCoordinatesOfCheckersWhichMustbeRemoved[i]);
            //    xCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(i);
            //    yCoordinatesOfCheckersWhichMustbeRemoved.RemoveAt(i);
            //}
            xCoordinatesOfCheckersWhichWasRemovedOnThisMove.Add(xCoordinateOfCapturedChecker);
            yCoordinatesOfCheckersWhichWasRemovedOnThisMove.Add(yCoordinateOfCapturedChecker);
            Field[xCoordinateOfCapturedChecker, yCoordinateOfCapturedChecker] = 0;
            OnRemoveChecker(xCoordinateOfCapturedChecker, yCoordinateOfCapturedChecker);
        }
        bool wrongDirection = false;
        private bool UpdateMayMoves(int x, int y)
        {
            if (x == 0 && y == 7)
                wrongDirection = false;
            wrongDirection = false;
            howManyDidYouAdd = 0;
            xCoordinatesOfMayMoves.Clear();
            yCoordinatesOfMayMoves.Clear();
            int leftMargin = x - 0;
            int rightMargin = 7 - x;
            int topMargin = y - 0;
            int bottonMargin = 7 - y;
            howManyDidYouAdd = 0;
            //<right-top>
            if (rightMargin > topMargin && topMargin != 0)
                for (int i = 1; i <= topMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (rightMargin < topMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (rightMargin == topMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            //</right-top>
            howManyDidYouAdd = 0;
            wrongDirection = false;
            //<left-top>
            if (leftMargin > topMargin && topMargin != 0)
                for (int i = 1; i <= topMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (leftMargin < topMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            if (leftMargin == topMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y - i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y - i);
                    }
                }
            //</left-top>
            howManyDidYouAdd = 0;
            wrongDirection = false;
            //<right-botton>
            if (rightMargin > bottonMargin && bottonMargin != 0)
                for (int i = 1; i <= bottonMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (rightMargin < bottonMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (rightMargin == bottonMargin && rightMargin != 0)
                for (int i = 1; i <= rightMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x + i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x + i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            //</right-botton>
            howManyDidYouAdd = 0;
            wrongDirection = false;
            //<left-botton>
            if (leftMargin > bottonMargin && bottonMargin != 0)
                for (int i = 1; i <= bottonMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (leftMargin < bottonMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            if (leftMargin == bottonMargin && leftMargin != 0)
                for (int i = 1; i <= leftMargin; i++)
                {
                    if (wrongDirection) break;
                    if (ThisMoveIsCorrect(x, y, x - i, y + i))
                    {
                        xCoordinatesOfMayMoves.Add(x - i);
                        yCoordinatesOfMayMoves.Add(y + i);
                    }
                }
            //</left-botton>
            howManyDidYouAdd = 0;
            wrongDirection = false;
            if (xCoordinatesOfMayMoves.Count == 0)
                return false;
            return true;
        }
        private bool GameIsOver()
        {
            if (IndexOfMover == 1)
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        if (Field[x, y] == 3 || Field[x, y] == 4)
                        {
                            if (UpdateMayMoves(x, y))
                                return false;
                        }
                    }
            }
            else
            {
                for (int x = 0; x < 8; x++)
                    for (int y = 0; y < 8; y++)
                    {
                        if (Field[x, y] == 1 || Field[x, y] == 2)
                        {
                            if (UpdateMayMoves(x, y))
                                return false;
                        }
                    }
            }
            return true;
        }
    }
}
