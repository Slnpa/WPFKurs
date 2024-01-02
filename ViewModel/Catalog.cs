using Microsoft.EntityFrameworkCore;
using Npgsql;
using NpgsqlTypes;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Input;
using WpfApp2.View;
using static WpfApp2.Registration;
using WpfApp2.Model;
using WpfApp2.Repositories;
using System.Windows.Media.Imaging;
using System;

namespace WpfApp2.ViewModel
{
    internal class Catalog
    {
        public ICommand Return { get; }
        public ICommand MoveToBasket { get; }
        public ICommand Info { get; }
        public ICommand Admin { get; }
        public ICommand Delete { get; }
        public ICommand AppleFilt { get; }
        public ICommand SamsungFilt { get; }
        public ICommand XiaomiFilt { get; }
        public ICommand NameSort { get; }
        public ICommand PriceSort { get; }
        public ICommand Find { get; }
        public ICommand MoveToFavourites { get; }
        public ICommand AddFavourites { get; }
        public ICommand UpdateFavouriteCommand { get; }
        public ICommand AddToBasketCommand { get; }

        public CatalogWindow window;
        private static int UserI;
        public Catalog(CatalogWindow wind, int User)
        {
            UserI = User;
            window = wind;
            PopulateMobileList();
            Return = new RelayCommand(Back);
            AddToBasketCommand = new RelayCommand(AddToBasket);
            MoveToBasket = new RelayCommand(MoveBasket);
            Info = new RelayCommand(InfoMob);
            Admin = new RelayCommand(MoveToAdmin);
            Delete = new RelayCommand(DeleteMobile);
            AppleFilt = new RelayCommand(FiltApple);
            SamsungFilt = new RelayCommand(FiltSamsung);
            XiaomiFilt = new RelayCommand(FiltXiaomi);
            NameSort = new RelayCommand(SortName);
            PriceSort = new RelayCommand(SortPrice);
            Find = new RelayCommand(FindMobile);
            MoveToFavourites = new RelayCommand(ShowFav);
            //AddFavourites = new RelayCommand(AddMobileToFav);
            UpdateFavouriteCommand = new RelayCommand(UpdateFavourite);
            window.Icon = new BitmapImage(new Uri("D:\\3 курс\\Kursach\\WpfApp2\\Resources\\mobilephone_79875.ico"));
        }
        private void PopulateMobileList()
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
        }
        public class MobileRepository
        {
            public List<Mobile> GetMobiles()
            {
                using (var context = new MobileDbContext())
                {
                    if (UserI == 34)
                    {
                        var mobiles = context.Mobiles.FromSqlRaw("SELECT SHOW_MOBILE.*, COALESCE(Mobile.mobile_boolean, TRUE) as IsCurrentUser," +
                            "CASE WHEN Favorites.id_mobile IS NOT NULL THEN TRUE ELSE FALSE END as IsFavorite" +
                            " FROM SHOW_MOBILE() join Mobile on SHOW_MOBILE.id_mobile_arg=Mobile.id_mobile" +
                            " LEFT JOIN Favorites ON SHOW_MOBILE.id_mobile_arg=Favorites.id_mobile AND Favorites.user_id = {0}", UserI).ToList();
                        return mobiles;
                    }
                    else
                    {
                        var mobiles = context.Mobiles.FromSqlRaw("SELECT SHOW_MOBILE.*, COALESCE(Mobile.mobile_boolean, FALSE) as IsCurrentUser," +
                                                    "CASE WHEN Favorites.id_mobile IS NOT NULL THEN TRUE ELSE FALSE END as IsFavorite" +
                                                    " FROM SHOW_MOBILE() join Mobile on SHOW_MOBILE.id_mobile_arg=Mobile.id_mobile" +
                                                    " LEFT JOIN Favorites ON SHOW_MOBILE.id_mobile_arg=Favorites.id_mobile AND Favorites.user_id = {0}", UserI).ToList();
                        return mobiles;
                    }
                }
            }
        }
        public void Back(object parameter)
        {
            AutorisationWindow windof = new AutorisationWindow();
            windof.Show();
            window.Close();
        }
        public void MoveBasket(object parameter)
        {

            BasketWindow move = new BasketWindow(UserI);
            move.Show();
            window.Close();
        }
        private void UpdateFavourite(object parameter)
        {
            if (parameter is Mobile selectedMobile)
            {
                selectedMobile.IsFavorite = !selectedMobile.IsFavorite;
                if (selectedMobile.IsFavorite)
                {
                    // Добавить в избранное
                    AddRemoveMobileToFav(selectedMobile.id_mobile_arg, UserI, false);
                }
                else
                {
                    // Удалить из избранного
                    AddRemoveMobileToFav(selectedMobile.id_mobile_arg, UserI, true);
                }
            }
        }
        private void AddRemoveMobileToFav(int mobileId, int clientId, bool add)
        {
            using (var context = new MobileDbContext())
            {
                if (add)
                {
                    // Добавить в избранное
                    var result = context.Database.ExecuteSqlRaw("CALL add_mobile_to_favourites({0}, {1})", mobileId, clientId);
                }
                else
                {
                    // Удалить из избранного
                    var result = context.Database.ExecuteSqlRaw("CALL DELETE_MOBILE_FROM_FAVOURITES({0}, {1})", mobileId, clientId);
                }
            }
        }
        private void AddMobileToFav(object parameter)
        {
            if (parameter is Mobile selectedMobile)
            {
                using (var context = new MobileDbContext())
                {
                    var result = context.Database.ExecuteSqlRaw("CALL add_mobile_to_favourites({0}, {1})", selectedMobile.id_mobile_arg, UserI);
                }
            }
        }
        private void ShowFav(object parameter)
        {
            FavouritesWindow wind = new FavouritesWindow(UserI);
            wind.Show();
            window.Close();
        }
        private void DeleteMobile(object parameter)
        {
            var item = parameter as Mobile;

            using (var connection = new NpgsqlConnection("Server=localhost;User Id=postgres;Password=123;Database=kursach;"))
            {
                connection.Open();
                using (var command = new NpgsqlCommand("delete from Reviews where id_mobile = @id_mobile", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile_arg);
                    command.ExecuteScalar();
                }
                using (var command = new NpgsqlCommand("delete from Basket where id_mobile = @id_mobile", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile_arg);
                    command.ExecuteScalar();
                }
                using (var command = new NpgsqlCommand("delete from Favorites where id_mobile = @id_mobile", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile_arg);
                    command.ExecuteScalar();
                }
                using (var command = new NpgsqlCommand("delete from Mobile where id_mobile = @id_mobile", connection))
                {
                    command.CommandType = CommandType.Text;
                    command.Parameters.AddWithValue("@id_mobile", NpgsqlDbType.Integer, item.id_mobile_arg);
                    command.ExecuteScalar();
                }
            }
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();

            window.mobileListView.ItemsSource = mobiles;
        }
        private void MoveToAdmin(object parameter)
        {
            AdminWindow wind = new AdminWindow(UserI, window);
            wind.Show();
        }
        private void FiltApple(object parameter)
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.mobileListView.ItemsSource = mobiles.Where(m => m.mobile_brend_arg == "Apple").ToList();
        }
        private void FiltSamsung(object parameter)
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.mobileListView.ItemsSource = mobiles.Where(m => m.mobile_brend_arg == "Samsung").ToList();
        }
        private void FiltXiaomi(object parameter)
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.mobileListView.ItemsSource = mobiles.Where(m => m.mobile_brend_arg == "Xiaomi").ToList();
        }
        private void SortName(object parameter)
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.mobileListView.ItemsSource = mobiles.OrderBy(m => m.mobile_name_arg).ToList();
        }
        private void SortPrice(object parameter)
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.mobileListView.ItemsSource = mobiles.OrderBy(m => m.mobile_price_arg).ToList();
        }
        private void FindMobile(object parameter)
        {
            var mobileRepository = new MobileRepository();
            var mobiles = mobileRepository.GetMobiles();
            window.mobileListView.ItemsSource = mobiles.Where(m => m.mobile_name_arg.Contains(window.FindBox.Text)).ToList();
        }
        private void InfoMob(object parameter)
        {
            var item = parameter as Mobile;
            MobileInfo mobileInfo = new MobileInfo(new MobileInfoWindow(), item, UserI);
            mobileInfo.window.Show();
            window.Close();
        }
        private void AddToBasket(object parameter)
        {
            if (parameter is Mobile selectedMobile)
            {
                using (var context = new MobileDbContext())
                {
                    var result = context.Database.ExecuteSqlRaw("CALL ADD_MOBILE_TO_BASKET({0}, {1})", selectedMobile.id_mobile_arg, UserI);
                }
            }
        }
    }
}
