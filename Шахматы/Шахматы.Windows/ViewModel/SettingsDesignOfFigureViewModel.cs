using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Шахматы.View;

namespace Шахматы.ViewModel
{
    class SettingsDesignOfFigureViewModel
    {
        //Коды фигур:
        //1 - чёрная обычная
        //2 - чёрная в дамках
        //3 - белая обычная
        //4 - белая в дамках
        //Передачяа параметра false в конструктор FigureControl означает изображение 39 * 39, а передача true означает изображение 100 * 100
        public INotifyCollectionChanged Figures { get { return figures; } }
        private ObservableCollection<FigureControl> figures;
        public SettingsDesignOfFigureViewModel()
        {
            figures = new ObservableCollection<FigureControl>();
        }
        public void UpdateFigures()
        {
            for (int i = 1; i < 8; i += 2)
                figures.Add(new FigureControl(1, i, 0, false));
            for (int i = 1; i < 8; i += 2)
                figures.Add(new FigureControl(1, i, 2, false));
            for (int i = 0; i < 8; i += 2)
                figures.Add(new FigureControl(1, i, 1, false));
            for (int i = 1; i < 8; i += 2)
                figures.Add(new FigureControl(3, i, 6, false));
            for (int i = 0; i < 8; i += 2)
                figures.Add(new FigureControl(3, i, 5, false));
            for (int i = 0; i < 8; i += 2)
                figures.Add(new FigureControl(3, i, 7, false));
        }
    }
}
