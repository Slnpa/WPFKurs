using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using WpfApp2.View;
using static WpfApp2.Registration;
using static WpfApp2.ViewModel.Catalog;
using System.Windows.Controls;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace WpfApp2.ViewModel
{
    internal class Admin : INotifyPropertyChanged
    {
        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged(nameof(ErrorMessage));
            }
        }
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public ICommand AddMobileCommand { get; }
        public ICommand ChooseImg { get; }
        
        AdminWindow window;
        public static int UserI;
        CatalogWindow winddd;
        public Admin(AdminWindow wind, int use, CatalogWindow ooo) 
        {
            UserI = use;
            winddd = ooo;
            window = wind;
            AddMobileCommand = new RelayCommand(AddMob);
            ChooseImg = new RelayCommand(ChooseImage);
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        public string filename;
        private void ChooseImage(object parameter)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.Filter = "Image files (*.png;*.jpeg)|*.png;*.jpeg|All files (*.*)|*.*";
            bool? result = dlg.ShowDialog();
            if (result == true)
            {
                filename = dlg.FileName;
                ImageSource imgSource = new BitmapImage(new Uri(filename));
                // Теперь вы можете использовать imgSource как переменную
                window.MyImage.Source = imgSource;
                // Сохраните путь к файлу в переменной класса
            }

        }
        private void AddMob(object parameter)
        {
            if (window.NameMobile.Text != "" && window.BrendMobile.SelectedItem != null && window.OSMobile.SelectedItem != null && filename != null)
            {
                int price;
                bool isPriceNumeric = int.TryParse(window.PriceMobile.Text, out price);

                int date;
                bool isDateNumeric = int.TryParse(window.DateMobile.Text, out date);

                if (isPriceNumeric && price >= 0 && isDateNumeric && date >= 0)
                {
                    using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
                    {
                        connection.Open();
                        using (var command = new NpgsqlCommand("call ADD_MOBILE(@mobilename,@price,@brend,@OS,@date, @url)", connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.Parameters.AddWithValue("@mobilename", NpgsqlDbType.Varchar, window.NameMobile.Text);
                            command.Parameters.AddWithValue("@price", NpgsqlDbType.Integer, price);
                            command.Parameters.AddWithValue("@brend", NpgsqlDbType.Varchar, ((ComboBoxItem)window.BrendMobile.SelectedItem).Content.ToString());
                            command.Parameters.AddWithValue("@OS", NpgsqlDbType.Varchar, ((ComboBoxItem)window.OSMobile.SelectedItem).Content.ToString());
                            command.Parameters.AddWithValue("@date", NpgsqlDbType.Integer, date);
                            command.Parameters.AddWithValue("@url", NpgsqlDbType.Varchar, filename);
                            command.ExecuteScalar();
                        }
                        ErrorMessage = "";
                        var mobileRepository = new MobileRepository();
                        var mobiles = mobileRepository.GetMobiles();
                        winddd.mobileListView.ItemsSource = mobiles;
                    }
                }
                else
                {
                    ErrorMessage = "Цена и дата выхода должны быть числами и не могут быть отрицательными";
                }
            }
            else
            {
                ErrorMessage = "Все поля должны быть заполнены";
            }


        }
    }
























































    //
}
