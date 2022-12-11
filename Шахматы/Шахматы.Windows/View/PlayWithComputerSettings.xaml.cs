using Шахматы.Common;
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
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.Streams;
using System.ComponentModel;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Popups;
// Документацию по шаблону элемента "Основная страница" см. по адресу http://go.microsoft.com/fwlink/?LinkId=234237

namespace Шахматы.View
{
    /// <summary>
    /// Основная страница, которая обеспечивает характеристики, являющимися общими для большинства приложений.
    /// </summary>
    public sealed partial class PlayWithComputerSettings : Page, INotifyPropertyChanged
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Эту настройку можно изменить на модель строго типизированных представлений.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper используется на каждой странице для облегчения навигации и 
        /// управление жизненным циклом процесса
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        private ComputerSettings settings;
        public PlayWithComputerSettings()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
            DataContext = this;
            countryOfFirstPlayer.Items.Add("Россия");
            countryOfFirstPlayer.Items.Add("USA");
            countryOfSecondPlayer.Items.Add("Россия");
            countryOfSecondPlayer.Items.Add("USA");
            nameOfSecondPlayer.Text = "Deep Blue";
            settings.NameOfSecondPlayer = nameOfSecondPlayer.Text;
            nameOfSecondPlayer.IsEnabled = false;
            nameOfFirstPlayer.IsEnabled = true;
            nameOfFirstPlayer.Text = "";
            ageOfSecondPlayer.Text = "-";
            ageOfSecondPlayer.IsEnabled = false;
            ageOfFirstPlayer.IsEnabled = true;
            ageOfFirstPlayer.Text = "";
            countryOfSecondPlayer.SelectedIndex = 0;
            countryOfSecondPlayer.IsEnabled = false;
            countryOfFirstPlayer.IsEnabled = true;
            countryOfFirstPlayer.SelectedIndex = 0;
            uploadImage.IsEnabled = true;
            uploadImageOfSecond.IsEnabled = false;
            SourceOfSImage = "/Assets/Comp.png";
            OnPropertyChanged("SourceOfSImage");
            settings.HumanIsBlack = false;
            settings.AgeOfSecondPlayer = ageOfSecondPlayer.Text;
            settings.CountryOfSecondPlayer = (Country)countryOfSecondPlayer.SelectedIndex;
        }

        /// <summary>
        /// Заполняет страницу содержимым, передаваемым в процессе навигации. Любое сохраненное состояние также является
        /// при повторном создании страницы из предыдущего сеанса.
        /// </summary>
        /// <param name="sender">
        /// Источник события; обычно <see cref="Common.NavigationHelper"/>
        /// </param>
        /// <param name="e">Данные события, предоставляющие параметр навигации, который передается
        /// <see cref="Frame.Navigate(Type, Object)"/> при первоначальном запросе этой страницы, и
        /// словарь состояния, сохраненного этой страницей в ходе предыдущего
        /// сеансом. Состояние будет равно значению NULL при первом посещении страницы.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Сохраняет состояние, связанное с данной страницей, в случае приостановки приложения или
        /// удаления страницы из кэша навигации.  Значения должны соответствовать требованиям сериализации
        /// требования <see cref="Common.SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">Источник события; обычно <see cref="Common.NavigationHelper"/></param>
        /// <param name="e">Данные события, которые предоставляют пустой словарь для заполнения
        /// сериализуемым состоянием.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region Регистрация NavigationHelper

        /// Методы, предоставленные в этом разделе, используются исключительно для того, чтобы
        /// NavigationHelper для отклика на методы навигации страницы.
        /// 
        /// Логика страницы должна быть размещена в обработчиках событий для 
        /// <see cref="Common.NavigationHelper.LoadState"/>
        /// и <see cref="Common.NavigationHelper.SaveState"/>.
        /// Параметр навигации доступен в методе LoadState 
        /// в дополнение к состоянию страницы, сохраненному в ходе предыдущего сеанса.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        public string SourceOfFirstImage { get; private set; }

        async private void uploadImage_Click(object sender, RoutedEventArgs e)
        {
            //DataContext = this;
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".png");
            IStorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    settings.ImageOfPlayer = bitmapImage;
                    bitmapImage.DecodePixelWidth = 600;
                    await bitmapImage.SetSourceAsync(stream);
                    imageOfFirstPlayer.Source = bitmapImage;
                }
            }
        }
        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler propertyChanged = PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        async private void uploadImageOfSecond_Click(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.FileTypeFilter.Add(".png");
            IStorageFile file = await openPicker.PickSingleFileAsync();
            if (file != null)
            {
                using (IRandomAccessStream stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read))
                {
                    BitmapImage bitmapImage = new BitmapImage();
                    settings.ImageOfPlayer = bitmapImage;
                    bitmapImage.DecodePixelWidth = 600;
                    await bitmapImage.SetSourceAsync(stream);
                    imageOfSecondPlayer.Source = bitmapImage;
                }
            }
        }
        private void nameOfFirstPlayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.NameOfFirstPlayer = nameOfFirstPlayer.Text;
        }

        private void nameOfSecondPlayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.NameOfSecondPlayer = nameOfSecondPlayer.Text;
        }

        private void ageOfFirstPlayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.AgeOfFirstPlayer = ageOfFirstPlayer.Text;
        }

        private void ageOfSecondPlayer_TextChanged(object sender, TextChangedEventArgs e)
        {
            settings.AgeOfSecondPlayer = ageOfSecondPlayer.Text;
        }

        private void countryOfFirstPlayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.CountryOfFirstPlayer = (Country)countryOfFirstPlayer.SelectedIndex;
        }

        private void countryOfSecondPlayer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            settings.CountryOfSecondPlayer = (Country)countryOfSecondPlayer.SelectedIndex;
        }

        private void time_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (timeOfOneMove.Value != 0)
                timeOfOneMove.Value = 0;
            settings.TimeOfMove = 0;
            settings.TimeOfGame = (int)time.Value * 60;
        }

        private void timeOfOneMove_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (time.Value != 0)
                time.Value = 0;
            settings.TimeOfGame = 0;
            settings.TimeOfMove = (int)timeOfOneMove.Value * 60;
        }

        private void play_Click(object sender, RoutedEventArgs e)
        {
            if (settings.TimeOfGame == 0 && settings.TimeOfMove == 0 && !withoutLimit.IsOn)
            {
                MessageDialog dialog = new MessageDialog("Время не выбрано!");
                dialog.ShowAsync();
                return;
            }
            this.Frame.Navigate(typeof(PlayWithComputer), settings);
        }
        public string SourceOfFImage { get; private set; }
        public string SourceOfSImage { get; private set; }
        async private void playAsBlack_Toggled(object sender, RoutedEventArgs e)
        {
            if (playAsBlack.IsOn)
            {
                settings.HumanIsBlack = true;
                nameOfFirstPlayer.IsEnabled = false;
                nameOfSecondPlayer.IsEnabled = true;
                nameOfSecondPlayer.Text = nameOfFirstPlayer.Text;
                nameOfFirstPlayer.Text = "Deep Blue";
                ageOfFirstPlayer.IsEnabled = false;
                ageOfSecondPlayer.IsEnabled = true;
                ageOfSecondPlayer.Text = ageOfFirstPlayer.Text;
                ageOfFirstPlayer.Text = "-";
                countryOfSecondPlayer.SelectedIndex = countryOfFirstPlayer.SelectedIndex;
                countryOfFirstPlayer.SelectedIndex = 0;
                countryOfFirstPlayer.IsEnabled = false;
                countryOfSecondPlayer.IsEnabled = true;
                uploadImage.IsEnabled = false;
                uploadImageOfSecond.IsEnabled = true;
                SourceOfSImage = SourceOfFImage;
                SourceOfFImage = "/Assets/Comp.png";
                OnPropertyChanged("SourceOfFImage");
                OnPropertyChanged("SourceOfSImage");
            }
            else
            {
                settings.HumanIsBlack = false;
                nameOfSecondPlayer.IsEnabled = false;
                nameOfFirstPlayer.IsEnabled = true;
                nameOfFirstPlayer.Text = nameOfSecondPlayer.Text;
                nameOfSecondPlayer.Text = "Deep Blue";
                ageOfSecondPlayer.IsEnabled = false;
                ageOfFirstPlayer.IsEnabled = true;
                ageOfFirstPlayer.Text = ageOfSecondPlayer.Text;
                ageOfSecondPlayer.Text = "-";
                countryOfFirstPlayer.SelectedIndex = countryOfSecondPlayer.SelectedIndex;
                countryOfSecondPlayer.SelectedIndex = 0;
                countryOfSecondPlayer.IsEnabled = false;
                countryOfFirstPlayer.IsEnabled = true;
                uploadImage.IsEnabled = true;
                uploadImageOfSecond.IsEnabled = false;
                SourceOfFImage = SourceOfSImage;
                SourceOfSImage = "/Assets/Comp.png";
                OnPropertyChanged("SourceOfSImage");
                OnPropertyChanged("SourceOfFImage");
            }
        }

        private void complexity_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            settings.diffecalty = (int)complexity.Value;
        }
    }
}
