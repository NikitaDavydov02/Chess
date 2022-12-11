using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Media.Imaging;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Animation;
using Шахматы.ViewModel;
using Windows.UI.Xaml;

// Шаблон элемента пользовательского элемента управления задокументирован по адресу http://go.microsoft.com/fwlink/?LinkId=234236

namespace Шахматы.View
{
    public sealed partial class FigureControl : UserControl, INotifyPropertyChanged
    {
        TwoPlayersViewModel viewModel = new TwoPlayersViewModel();
        PlayWithComputerViewModel compViewModel = new PlayWithComputerViewModel();
        public FigureControl()
        {
            this.InitializeComponent();
        }
        public string SourceOfImage { get; private set; }
        public double WidthOfImage { get; private set; }
        private int code;
        public bool IsMayMove { get; private set; } = false;
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        public int X { get; private set; }
        public int Y { get; private set; }
        private bool tick = false;
        private bool useComputerViewModel;
        public FigureControl(int code, int x, int y, bool bigImage,bool useCompViewModel=false) : this()
        {
            this.useComputerViewModel = useCompViewModel;
            DataContext = this;
            X = x;
            Y = y;
            this.code = code;
            if (bigImage)
            {
                WidthOfImage = 72;
            }
            else
            {
                WidthOfImage = 39;
            }
            Canvas.SetLeft(this, X * WidthOfImage);
            Canvas.SetTop(this, Y * WidthOfImage);
            OnPropertyChanged("WidthOfImage");
            if (code == 1)
                SourceOfImage = "/Assets/BlackUsual.png";
            if (code == 2)
                SourceOfImage = "/Assets/BlackQueen.png";
            if (code == 3)
                SourceOfImage = "/Assets/WhiteUsual.png";
            if (code == 4)
                SourceOfImage = "/Assets/WhiteQueen.png";
            if (code == 5)
            {
                SourceOfImage = " /Assets/MayMove.png";
                IsMayMove = true;
            }
            OnPropertyChanged("SourceOfImage");
        }

        public void AnimateChangeOfPositon(int endX,int endY)
        {
            if (IsMayMove == true)
                return;
            Storyboard Xstoryboard = new Storyboard();
            DoubleAnimation XdoubleAnimation = new DoubleAnimation();
            Storyboard.SetTarget(XdoubleAnimation, this);
            Storyboard.SetTargetProperty(XdoubleAnimation, "(Canvas.Left)");
            XdoubleAnimation.From = X * 72;
            XdoubleAnimation.To = endX * 72;
            XdoubleAnimation.Duration = TimeSpan.FromMilliseconds(500);
            Xstoryboard.Children.Add(XdoubleAnimation);
            X = endX;

            Storyboard Ystoryboard = new Storyboard();
            DoubleAnimation YdoubleAnimation = new DoubleAnimation();
            Storyboard.SetTarget(YdoubleAnimation, this);
            Storyboard.SetTargetProperty(YdoubleAnimation, "(Canvas.Top)");
            YdoubleAnimation.From = Y * 72;
            YdoubleAnimation.To = endY * 72;
            YdoubleAnimation.Duration = TimeSpan.FromMilliseconds(500);
            Ystoryboard.Children.Add(YdoubleAnimation);
            Y = endY;

            Xstoryboard.Begin();
            Ystoryboard.Begin();
        }

        private void image_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            if (!useComputerViewModel)
            {
                if (IsMayMove)
                    viewModel.MakeMove(X, Y);
                else
                    viewModel.ShowMayMoves(X, Y);
            }
            else
            {
                if (IsMayMove)
                    compViewModel.MakeMove(X, Y);
                else
                    compViewModel.ShowMayMoves(X, Y);
            }
        }
    }
}
