using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfApp2.ViewModel;

namespace WpfApp2.View
{
    /// <summary>
    /// Логика взаимодействия для FavouritesWindow.xaml
    /// </summary>
    public partial class FavouritesWindow : Window
    {
        public FavouritesWindow(int user)
        {
            InitializeComponent();
            DataContext = new Favourites(this, user);
        }
    }
}
