using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Imaging;

namespace Шахматы
{
    struct ComputerSettings
    {
        public string NameOfFirstPlayer;
        public string NameOfSecondPlayer;
        public string AgeOfFirstPlayer;
        public string AgeOfSecondPlayer;
        public Country CountryOfFirstPlayer;
        public Country CountryOfSecondPlayer;
        public BitmapImage ImageOfPlayer;
        public int TimeOfGame;
        public int TimeOfMove;
        public bool HumanIsBlack;
        public int diffecalty;
    }
}
