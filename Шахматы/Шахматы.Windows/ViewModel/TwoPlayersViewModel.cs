using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Шахматы.Model;
using Шахматы.View;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using System.ComponentModel;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace Шахматы.ViewModel
{
    class TwoPlayersViewModel:INotifyCollectionChanged, INotifyPropertyChanged
    {
        //Коды фигур:
        //1 - чёрная обычная
        //2 - чёрная в дамках
        //3 - белая обычная
        //4 - белая в дамках
        //Properties:
        public static INotifyCollectionChanged Figures { get { return figures; } }
        public int WhitesSeconds { get; private set; }
        public int BlacksSeconds { get; private set; }
        private IStorageFile file;
        public static INotifyCollectionChanged Notation { get { return notation; } }
        public static bool ButtonEnable { get; private set; } = false;
        public static bool DownloadButtonEnable { get; private set; } = false;
        //Fields:
        private int countOfMoves;
        private static TwoPlayersModel model;
        private static ObservableCollection<FigureControl> figures;
        private static ObservableCollection<string> notation;
        private static DispatcherTimer whiteTimer;
        private static DispatcherTimer blackTimer;
        private static int moveLimit;
        private static int gameLimit;
        public TwoPlayersViewModel(int gameLimit, int moveLimit)
        {
            TwoPlayersViewModel.moveLimit = moveLimit;
            TwoPlayersViewModel.gameLimit = gameLimit;
            whiteTimer = new DispatcherTimer();
            blackTimer = new DispatcherTimer();
            whiteTimer.Interval = TimeSpan.FromSeconds(1);
            blackTimer.Interval = TimeSpan.FromSeconds(1);
        }
        public TwoPlayersViewModel()
        {

        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        async public void GameOver()
        {
            DownloadButtonEnable = true;
            OnButtonEnableChanged();
            whiteTimer.Stop();
            blackTimer.Stop();
            MessageDialog dialog = new MessageDialog("");
            if (model.IndexOfMover == 1)
            {
                dialog = new MessageDialog("Победили чёрные.");
                notation.Add("[0 - 1]");
            }
            if (model.IndexOfMover == 2)
            {
                dialog = new MessageDialog("Победили белые.");
                notation.Add("[1 - 0]");
            }
            dialog.Commands.Add(new UICommand("Новая игра"));
            dialog.Commands.Add(new UICommand("К настройкам игры"));
            dialog.Commands.Add(new UICommand("Закрыть"));
            UICommand result = await dialog.ShowAsync() as UICommand;
            if (result != null && result.Label == "Новая игра")
                OnNewGame();
            else if (result != null && result.Label == "К настройкам игры")
                OnGameSettings();
        }
        public void Start()
        {
            whiteTimer = new DispatcherTimer();
            blackTimer = new DispatcherTimer();
            whiteTimer.Interval = TimeSpan.FromSeconds(1);
            blackTimer.Interval = TimeSpan.FromSeconds(1);
            whiteTimer.Tick += WhiteTimer_Tick;
            blackTimer.Tick += BlackTimer_Tick;
            if (moveLimit != 0)
            {
                BlacksSeconds = moveLimit;
                WhitesSeconds = moveLimit;
                OnPropertyChanged("BlacksSeconds");
                OnPropertyChanged("WhitesSeconds");
            }
            else
            {
                BlacksSeconds = gameLimit;
                WhitesSeconds = gameLimit;
                OnPropertyChanged("BlacksSeconds");
                OnPropertyChanged("WhitesSeconds");
            }
            if (gameLimit != 0 || moveLimit != 0)
            {
                whiteTimer.Start();
            }
            else
            {
                BlacksSeconds = -1;
                WhitesSeconds = -1;
                OnPropertyChanged("BlacksSeconds");
                OnPropertyChanged("WhitesSeconds");
            }
            figures = new ObservableCollection<FigureControl>();
            notation = new ObservableCollection<string>();
            for (int i = 1; i < 8; i += 2)
                figures.Add(new FigureControl(1, i, 0, true));
            for (int i = 1; i < 8; i += 2)
                figures.Add(new FigureControl(1, i, 2, true));
            for (int i = 0; i < 8; i += 2)
                figures.Add(new FigureControl(1, i, 1, true));
            for (int i = 1; i < 8; i += 2)
                figures.Add(new FigureControl(3, i, 6, true));
            for (int i = 0; i < 8; i += 2)
                figures.Add(new FigureControl(3, i, 5, true));
            for (int i = 0; i < 8; i += 2)
                figures.Add(new FigureControl(3, i, 7, true));
            model = new TwoPlayersModel();
            model.CheckerChanged += Model_CheckerChanged;
            model.MoverChanged += Model_MoverChanged;
            model.MovesChanged += Model_MovesChanged;
            model.QueenChecker += Model_QueenChecker;
            model.RemoveChecker += Model_RemoveChecker;
            model.NotationChanged += Model_NotationChanged;
            model.StartTheGame();
        }
        private int numberOfLine=0;
        private void Model_NotationChanged(object sender, NotationChangedArgs e)
        {
            countOfMoves++;
            if (countOfMoves == 6)
            {
                ButtonEnable = true;
                OnButtonEnableChanged();
            }
            if (!e.NewLine)
            {
                notation.Add((numberOfLine + 1) + ".");
                numberOfLine++;
            }
            if(!e.ThisIsSecondMove)
                notation[notation.Count - 1] += "   " + e.Notation;
            else
                notation[notation.Count - 1] += e.Notation;
            //OnPropertyChanged("Notation");
            //if(!e.IsCapturind)
        }

        private void Model_MoverChanged(object sender, EventArgs e)
        {
            if (moveLimit != 0 || gameLimit != 0)
            {
                if (moveLimit != 0)
                {
                    if (model.IndexOfMover == 1)
                    {
                        blackTimer.Stop();
                        WhitesSeconds = moveLimit;
                        whiteTimer.Start();
                    }
                    else
                    {
                        whiteTimer.Stop();
                        BlacksSeconds = moveLimit;
                        blackTimer.Start();
                    }
                }
                if (model.IndexOfMover == 1)
                {
                    blackTimer.Stop();
                    whiteTimer.Start();
                }
                else
                {
                    whiteTimer.Stop();
                    blackTimer.Start();
                }
            }
        }

        private void BlackTimer_Tick(object sender, object e)
        {
            if (BlacksSeconds == 0)
            {
                GameOver();
                return;
            }
            BlacksSeconds--;
            OnPropertyChanged("BlacksSeconds");
        }

        private void WhiteTimer_Tick(object sender, object e)
        {
            if (WhitesSeconds == 0)
            {
                GameOver();
                return;
            }
            WhitesSeconds--;
            OnPropertyChanged("WhitesSeconds");
        }

        private void Model_RemoveChecker(object sender, QueenCheckerArgs e)
        {
            int indexOfRemoving = 0;
            for (int i = 0; i < figures.Count; i++)
                if (figures[i].X == e.X && figures[i].Y == e.Y)
                    indexOfRemoving = i;
            figures.RemoveAt(indexOfRemoving);
        }

        private void Model_QueenChecker(object sender, QueenCheckerArgs e)
        {
            for(int i=0;i<figures.Count;i++)
                if (figures[i].X == e.X && figures[i].Y == e.Y)
                {
                    if (e.IsWhite)
                        figures[i] = new FigureControl(4, e.X, e.Y, true);
                    else
                        figures[i] = new FigureControl(2, e.X, e.Y, true);
                }
        }

        private void Model_MovesChanged(object sender, MovesChangedEventArgs e)
        {
            List<int> numberOfRemovingElements = new List<int>();
            for (int i = figures.Count - 1; i >= 0; i--)
            {
                if (figures[i].IsMayMove)
                    numberOfRemovingElements.Add(i);
            }
            foreach (int i in numberOfRemovingElements)
                figures.RemoveAt(i);
            for (int i = 0; i < e.xCoordinatesOfMayMoves.Count; i++)
                figures.Add(new FigureControl(5, e.xCoordinatesOfMayMoves[i], e.yCoordinatesOfMayMoves[i],true));
        }

        private void Model_CheckerChanged(object sender, CheckerChangedEventArgs e)
        {
            foreach(FigureControl fg in figures)
            {
                if (fg.X == e.startX && fg.Y == e.startY)
                {
                    fg.AnimateChangeOfPositon(e.endX, e.endY);
                    break;
                }
            }
            List<int> numberOfRemovingElements = new List<int>();
            for(int i = figures.Count-1; i >=0; i--)
            {
                if (figures[i].IsMayMove)
                    numberOfRemovingElements.Add(i);
            }
            foreach (int i in numberOfRemovingElements)
                figures.RemoveAt(i);
        }
        private static int startX;
        private static int startY;
        public void ShowMayMoves(int x,int y)
        {
            model.ShowMayMoves(x, y);
        }
        public async void MakeMove(int x,int y)
        {
            if (!model.MakeMove(x, y))
            {
                GameOver();
            }
        }
        public static event EventHandler NewGame;
        public static event EventHandler ButtonEnableChanged;
        public static event EventHandler GameSettings;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnNewGame()
        {
            EventHandler newGame = NewGame;
            if (newGame != null)
                newGame(this, new EventArgs());
        }
        private void OnButtonEnableChanged()
        {
            EventHandler buttonEnableChanged = ButtonEnableChanged;
            if (buttonEnableChanged != null)
                buttonEnableChanged(this, new EventArgs());
        }
        private void OnGameSettings()
        {
            EventHandler gameSettings = GameSettings;
            if (gameSettings != null)
                gameSettings(this, new EventArgs());
        }
        async public void Draw()
        {
            MessageDialog d = new MessageDialog("Все игроки согласны на ничью?");
            d.Commands.Add(new UICommand("Да"));
            d.Commands.Add(new UICommand("Нет"));
            UICommand result = await d.ShowAsync() as UICommand;
            if(result.Label == "Да")
            {
                notation.Add("[1/2 - 1/2]");
                MessageDialog dialog = new MessageDialog("Ничья!");
                dialog.Commands.Add(new UICommand("Новая игра"));
                dialog.Commands.Add(new UICommand("К настройкам игры"));
                dialog.Commands.Add(new UICommand("Закрыть"));
                UICommand r = await dialog.ShowAsync() as UICommand;
                if (r != null && r.Label == "Новая игра")
                    OnNewGame();
                else if (r != null && r.Label == "К настройкам игры")
                    OnGameSettings();
                DownloadButtonEnable = true;
                OnButtonEnableChanged();
            }
        }
        async public void Resign()
        {
            string winner;
            if (model.IndexOfMover == 1)
            {
                winner = " чёрные!";
                notation.Add("[0 - 1]");
            }
            else
            {
                winner = " белые!";
                notation.Add("[1 - 0]");
            }
            MessageDialog dialog = new MessageDialog("Победили" + winner);
            dialog.Commands.Add(new UICommand("Новая игра"));
            dialog.Commands.Add(new UICommand("К настройкам игры"));
            dialog.Commands.Add(new UICommand("Закрыть"));
            UICommand r = await dialog.ShowAsync() as UICommand;
            if (r != null && r.Label == "Новая игра")
                OnNewGame();
            else if (r != null && r.Label == "К настройкам игры")
                OnGameSettings();
            DownloadButtonEnable = true;
            OnButtonEnableChanged();
        }
        async public void Download()
        {
            if (file == null)
            {
                FileSavePicker picker = new FileSavePicker() { DefaultFileExtension = ".txt", SuggestedStartLocation = PickerLocationId.DocumentsLibrary };
                picker.FileTypeChoices.Add("Text File", new List<string>() { ".txt" });
                file = await picker.PickSaveFileAsync();
                if (file == null)
                    return;
            }
            await FileIO.WriteLinesAsync(file, notation);
            await new MessageDialog("Запись партии сохранена!").ShowAsync();
        }
    }
}
