using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using static WpfApp2.Registration;
using WpfApp2.View;
using System.Windows.Media.Imaging;
using Npgsql;
using NpgsqlTypes;
using System.Data;
using Microsoft.EntityFrameworkCore;
using WpfApp2.Model;
using WpfApp2.Repositories;

namespace WpfApp2.ViewModel
{
    internal class Basket
    {
        public BasketWindow window;
        public static int UserI;
        public List<MobileInBasket> mobiles;
        public ICommand MoveToCatalog { get; }
        public ICommand MinusCounter { get; }
        public ICommand PlusCounter { get; }
        public ICommand DeleteFromBas { get; }
        public ICommand MoveToHistory { get; }
        public ICommand Purchase { get; }
        

        public Basket(BasketWindow wind, int use)
        {
            UserI = use;
            window = wind;
            PopulateMobileList();
            MoveToCatalog = new RelayCommand(MoveCatalog);
            MinusCounter = new RelayCommand(Minus);
            PlusCounter = new RelayCommand(Plus);
            DeleteFromBas = new RelayCommand(DeleteFromBasket);
            MoveToHistory = new RelayCommand(MoveHistory);
            Purchase = new RelayCommand(Buy);
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        private void Buy(object parameter)
        {
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("call PURCHAISE(@user_id)", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, UserI);
                    command.ExecuteScalar();
                }
            }
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
            window.TotalPrice.Text = "Общая стоимость: " + mobiles.Sum(m => m.Counter * m.mobile_price_arg).ToString();
        }
        private void MoveHistory(object parameter)
        {
            HistoryWindow wind = new HistoryWindow(UserI);
            wind.Show();
            window.Close();
        }
        private void DeleteFromBasket(object parameter)
        {
            var item = parameter as MobileInBasket;
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("call DELETE_MOBILE_FROM_BASKET(@id_mobile,@user_id)", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile_arg);
                    command.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, UserI);
                    command.ExecuteScalar();
                }
            }
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
            window.TotalPrice.Text = "Общая стоимость: " + mobiles.Sum(m => m.Counter * m.mobile_price_arg).ToString();
        }
        private void Minus(object parameter)
        {
            var item = parameter as MobileInBasket;
            if (item.Counter!=1)
            {
                using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
                {
                    connection.Open();
                    using (var command = new NpgsqlCommand("call DELETE_MOBILE_COUNT_BASKET(@id_mobile,@user_id)", connection))
                    {
                        command.CommandType = CommandType.Text;
                        command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile_arg);
                        command.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, UserI);
                        command.ExecuteScalar();
                    }
                }
                var mobileRepository = new MobileRepository();
                var mobiles = mobileRepository.GetMobiles();

                window.mobileListView.ItemsSource = mobiles;
                window.TotalPrice.Text = "Общая стоимость: " + mobiles.Sum(m => m.Counter * m.mobile_price_arg).ToString();
            }
        }
        private void Plus(object parameter)
        {
            var item = parameter as MobileInBasket;
            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("call ADD_MOBILE_TO_BASKET(@id_mobile,@user_id)", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile_arg);
                    command.Parameters.AddWithValue("@user_id", NpgsqlDbType.Integer, UserI);
                    command.ExecuteScalar();
                }
            }
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
            window.TotalPrice.Text = "Общая стоимость: " + mobiles.Sum(m => m.Counter * m.mobile_price_arg).ToString();
        }
        public void MoveCatalog(object parameter)
        {
            CatalogWindow cat = new CatalogWindow(UserI);
            cat.Show();
            window.Close();
        }
        public Basket(BasketWindow wind) 
        {
            window = wind;
            PopulateMobileList();
            MoveToCatalog = new RelayCommand(MoveCatalog);
            MinusCounter = new RelayCommand(Minus);
            PlusCounter = new RelayCommand(Plus);
        }
        //public class MobileDbContext : DbContext
        //{
        //    public DbSet<MobileInBasket> Mobiles { get; set; }

        //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //    {
        //        optionsBuilder.UseNpgsql("Server=localhost;User Id=postgres;Password=123;Database=kursach;");
        //    }
        //}
        
        public class MobileRepository
        {
            public List<MobileInBasket> GetMobiles()
            {
                using (var context = new MobileDbContext())
                {
                    var mobiles = context.Basket.FromSqlRaw("SELECT SHOW_MOBILE.*,Basket.counter from SHOW_MOBILE() join Basket on SHOW_MOBILE.id_mobile_arg=Basket.id_mobile WHERE Basket.user_id={0}", UserI).ToList();
                    return mobiles;
                }
            }
        }
        private void PopulateMobileList()
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
            window.TotalPrice.Text = "Общая стоимость: " + mobiles.Sum(m => m.Counter * m.mobile_price_arg).ToString();
        }
    }
}
